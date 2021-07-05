using TONBRAINS.TONOPS.WebApp.Common.Models.ConfigurationTemplate;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TONBRAINS.TONOPS.WebApp.Services
{
   [Obsolete("Use SshStream.")]
    public class SshService
    {
        private ConnectionInfo GetConnectionInfo(ServerTemplate options)
        {
            return new ConnectionInfo(
                options.Host,
                options.Port,
                options.User,
                new AuthenticationMethod[] {
                    new PasswordAuthenticationMethod(options.User, options.Password),
                }
            );
        }

        public string DeployNodeOS(string host, string password)
        {
            var filePaths = new Dictionary<string, string>
            {
                [@"C:\temp\monitoring.service"] = "/etc/systemd/system/monitoring.service",
                [@"C:\temp\mon.zip"] = "/root/packages/mon.zip"
            };
            var commands = new List<string>
            {
                "timedatectl set-timezone UTC",
                "systemctl stop monitoring",
                "rm -r /root/mon",
                "apt-get install unzip",
                "unzip /root/packages/mon.zip",
                "chmod -R 777 /root/mon",
                "systemctl daemon-reload",
                "systemctl start monitoring",
                "systemctl enable monitoring",
            };
            return ExecuteCommands(host, "root", password, filePaths, commands);
        }

        public string CreateRoot(string host, string user, string password)
        {
            var filePaths = new Dictionary<string, string> { };
            var commands = new List<string>
            {
                $"echo -e \"{password}\n{password}\n{password}\" | sudo -S passwd root",
                "su echo \"\nPermitRootLogin yes\" >> /etc/ssh/sshd_config",
                "su systemctl restart sshd"
            };

            return ExecuteCommands(host, user, password, filePaths, commands);
        }

        public string SetStaticIp(string host, string password)
        {
            var filePaths = new Dictionary<string, string>
            {
                [@"C:\temp\ip.sh"] = "/root/ip.sh"
            };
            var commands = new List<string>
            {
                "chmod -R 777 /root/ip.sh",
                "./ip.sh",
                "netplan apply"
            };
            return ExecuteCommands(host, "root", password, filePaths, commands);
        }

        public string InstallZabbixAgent(string host, string password)
        {
            var filePaths = new Dictionary<string, string> { };
            var commands = new List<string>
            {
                "apt-get install zabbix-agent",
                "sed -i 's/ServerActive=127.0.0.1/ServerActive=0.0.0.0/\0/g' /etc/zabbix/zabbix_agentd.conf",
                "service zabbix-agent start"
            };
            return ExecuteCommands(host, "root", password, filePaths, commands);
        }

        public string ExecuteCommands(string host, string user, string password, IDictionary<string, string> deployFilePaths, IEnumerable<string> executeCommands)
        {
            try
            {
                var connectionInfo = new ConnectionInfo(host, 22, user,
                    new AuthenticationMethod[] {
                    new PasswordAuthenticationMethod(user, password),
                    }
                );

                if (deployFilePaths.Any())
                {
                    using (var sftp = new SftpClient(connectionInfo))
                    {
                        sftp.Connect();
                        foreach (var item in deployFilePaths)
                        {
                            var t = File.Exists(item.Key);
                            using (Stream fileStream = File.OpenRead(item.Key))
                            {
                                sftp.UploadFile(fileStream, item.Value);
                            }
                        }
                        sftp.Disconnect();
                    }
                }
                if (executeCommands.Any())
                {
                    using (var client = new SshClient(connectionInfo))
                    {
                        client.Connect();
                        foreach (var item in executeCommands)
                        {
                            var modes = new Dictionary<Renci.SshNet.Common.TerminalModes, uint>();
                            if (item.Contains("su") && user != "root")
                            {
                                using (var stream = client.CreateShellStream("xterm", 255, 50, 800, 600, 1024, modes))
                                {
                                    stream.WriteLine("sudo su root");
                                    Thread.Sleep(1000);
                                    stream.Expect("password");
                                    Thread.Sleep(1000);
                                    stream.WriteLine($"{password}");
                                    Thread.Sleep(1000);
                                    stream.WriteLine(item.Replace("su ", ""));
                                }
                                continue;
                            }
                            if (item == "createpostgrespassword")
                            {
                                using (var stream = client.CreateShellStream("xterm", 255, 50, 800, 600, 1024, modes))
                                {
                                    stream.WriteLine("su - postgres");
                                    stream.Read();
                                    stream.WriteLine("psql -c \"ALTER USER postgres PASSWORD 'postgres';\"");
                                    var result = stream.Read();
                                    //TODO: check result!!!
                                }
                                continue;
                            }
                            using (var cmd = client.RunCommand(item))
                            {
                                if (cmd.ExitStatus == 0)
                                    Console.WriteLine(cmd.Result);
                                else
                                    Console.WriteLine(cmd.Error);
                            }
                        }
                        client.Disconnect();
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.ToString());
#endif
                return ex.ToString();
            }
        }

        public async Task UploadFileToHost(ServerTemplate options, Stream input, string pathToSave)
        {
            await Task.Run(
                () =>
                {
                    var connectionInfo = new ConnectionInfo(
                        options.Host,
                        22,
                        options.User,
                        new AuthenticationMethod[] {
                            new PasswordAuthenticationMethod(options.User, options.Password),
                        }
                    );

                    using (var sftp = new SftpClient(connectionInfo))
                    {
                        sftp.Connect();
                        sftp.UploadFile(input, pathToSave);
                        sftp.Disconnect();
                    }
                }
            );
        }

        private void RunCommand(SshClient client, string command)
        {
            using (var sshCommand = client.RunCommand(command))
            {
#if DEBUG
                Debug.WriteLine(sshCommand.Error);
#endif
                if (sshCommand.ExitStatus != 0) throw new ArgumentException($"Command {command} executed with error {sshCommand.Error}");
            }
        }

        public async Task ExecuteBashOnHost(ServerTemplate options, Stream bash)
        {
            var tempBashFileName = "/root/" + Guid.NewGuid();
            await UploadFileToHost(options, bash, tempBashFileName);

            await Task.Run(
                () =>
                {
                    using (var client = new SshClient(GetConnectionInfo(options)))
                    {
                        client.Connect();

                        RunCommand(client, "chmod +x " + tempBashFileName);
                        RunCommand(client, tempBashFileName);
                        RunCommand(client, "rm " + tempBashFileName);

                        client.Disconnect();
                    }
                }
            );
        }

        public async Task ExecuteSingleCommand(ServerTemplate options, string command)
        {
            await Task.Run(
                () =>
                {
                    using (var client = new SshClient(GetConnectionInfo(options)))
                    {
                        client.Connect();

                        RunCommand(client, command);

                        client.Disconnect();
                    }
                }
            );
        }

    }
}
