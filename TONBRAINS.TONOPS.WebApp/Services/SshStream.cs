using TONBRAINS.TONOPS.WebApp.Common.Models;
using TONBRAINS.TONOPS.WebApp.Common.Models.ConfigurationTemplate;
using TONBRAINS.TONOPS.WebApp.Hubs;
using TONBRAINS.TONOPS.WebApp.WebApp;
using Microsoft.AspNetCore.SignalR;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.DAL;

namespace TONBRAINS.TONOPS.WebApp.Services
{
    public class SshStream : ISshStream
    {
        private readonly IHubContext<NodeHub> _hubContext;

        public SshStream(IHubContext<NodeHub> hubContext)
            => _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));

        public ConnectionInfo GetConnectionInfo(ServerTemplate options) => new ConnectionInfo(
                options.Host,
                options.Port,
                options.User,
                new AuthenticationMethod[] {
                    new PasswordAuthenticationMethod(options.User, options.Password),
                }
        );

        public async Task UploadFileToHost(ConnectionInfo info, Stream input, string pathToSave)
        {
            await Task.Run(
                 async () =>
                 {
                     await WriteHistory(info.Host, $"Starting upload file to {info.Host}@{info.Username}:{pathToSave}");
                     using (var sftp = new SftpClient(info))
                     {
                         try
                         {
                             sftp.Connect();
                             sftp.UploadFile(input, pathToSave, async uploaded =>
                             {
                                 var percent = Math.Round((double)uploaded / input.Length * 100, 2);
                                 if(percent%10==0)
                                    await WriteHistory(info.Host, $"Uploaded {percent}%");
                             });
                         }
                         catch(Exception exception)
                         {
                             await WriteHistory(info.Host, $"Failed upload file. {exception.Message}");
                         }
                         finally
                         {
                             sftp.Disconnect();
                         }
                        
                     }
                 }
             );
        }

        public async Task ExecuteBashOnHost(SshClient client, Stream bash, string pathToSave = "/root/", string parameters ="")
        {
            var tempBashFileName = pathToSave + Guid.NewGuid();
            await UploadFileToHost(client.ConnectionInfo, bash, tempBashFileName);

            //await Task.Run(
            //     async () =>
            //    {
            //        await ExecuteCommands(client, new string[] {
            //            "chmod +x " + tempBashFileName,
            //            "sed -i -e 's/\r$//' " + tempBashFileName,
            //            tempBashFileName + parameters,
            //            "rm " + tempBashFileName
            //        });
            //    }
            //);
        }


        public async Task ExecuteCommands(SshClient client, string [] commands, string hostId)
        {
            var host = hostId;
            foreach (var c in commands)
            {
                try
                {
                    var cmd = client.CreateCommand(c);
                    cmd.CommandTimeout = new TimeSpan(0, 0, 20);
                    var result = cmd.BeginExecute();
                    await WriteHistory(host, c);
                    using (var reader = new StreamReader(cmd.OutputStream))
                    {
                        while (!result.IsCompleted || !reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            if (line != null)
                            {
                                await WriteHistory(host, line);
                            }
                        }
                        if (cmd.ExitStatus != 0)
                            await WriteHistory(host, cmd.Error);
                    }

                }
                catch(Exception exception)
                {
                    await WriteHistory(host, $"Faield command: '{c}'. Exception message: {exception.Message}");
                    if (c == "reboot")
                    {
                        await WriteHistory(host, "rebooting host....");
                    }
                    await WriteHistory(host, "execution error");
                    client.Disconnect();
                    client.Dispose();
                }

            }
        }

        public  SshClient GetSshClient(ServerTemplate options)
            => new SshClient(GetConnectionInfo(options));

        public SshClient GetSshClient(ConnectionInfo info)
            => new SshClient(info);

        public async Task WriteHistory(string host, string message)
        {
            if (!Program.History.ContainsKey(host)) Program.History.Add(host, new List<string>());
            Program.History[host].Add(message);
            await _hubContext.Clients.All.SendAsync($"Send", message, host);
        }

        public async Task WriteHistoryStream(string host, ShellStream stream, string command)
        {
            await WriteHistory(host, command);
            stream.Write(command);
        }

        public async Task DeployRoot(SshClient client, Common.Models.Credential initEntry, Common.Models.Credential root)
        {
            var host = client.ConnectionInfo.Host;
            using (var stream = client.CreateShellStream("xterm", 0, 0, 0, 0, 1024))
            {
                await WriteHistory(client.ConnectionInfo.Host, "Setup root");
                await WriteHistoryStream(host, stream,  $"echo -e \"{initEntry.Password}\n{root.Password}\n{root.Password}\" | sudo -S passwd root");
                if (initEntry.UserName != "root") {
                    await WriteHistoryStream(host, stream,  "sudo su root");
                    Thread.Sleep(1000);
                    stream.Expect($"{initEntry.UserName}");
                    Thread.Sleep(1000);
                    await WriteHistoryStream(host, stream, $"{root.Password}");
                    Thread.Sleep(1000);
                    await WriteHistoryStream(host, stream,  "echo \"\nPermitRootLogin yes\" >> /etc/ssh/sshd_config");
                    Thread.Sleep(1000);
                    await WriteHistoryStream(host, stream,  "service ssh restart");
                }
                stream.Close();
            }
            await WriteHistory(client.ConnectionInfo.Host, "root is deployed!");

        }

        public async Task CheckServer(v_Node_Shh credential, string groupId)
        {
            using var client = GetSshClient(new ServerTemplate
            {
                Host = credential.SshIp,
                Port = credential.SshPort,
                User = credential.UserName,
                Password = credential.Password
            });
            try
            {
                client.Connect();
                var result = client.RunCommand("df -BG --total --output=source,avail,pcent,size | grep total").Result?.Replace("total", "").Trim();
                await _hubContext.Clients.All.SendAsync($"Status", result, credential.Id, groupId);
            }
            catch
            {
                await _hubContext.Clients.All.SendAsync($"Status", "Disconnect", credential.Id, groupId);
            }
            finally
            {
                client?.Disconnect();
                client.Dispose();
            }
           
        }


    }
}
