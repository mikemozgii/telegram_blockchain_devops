using System;
using System.Collections.Generic;
using System.Text;
using Renci.SshNet;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;
using Renci.SshNet.Sftp;
using TONBRAINS.TONOPS.Core.Handlers;
using System.Diagnostics;

namespace TONBRAINS.TONOPS.Core.SSH
{
    public class SSHSvc
    {
 
        public ConnectionInfo GetConnectionInfo(SSHAuthMdl auth)
        {

            var info = new ConnectionInfo(auth.Host, auth.Port, auth.User, new AuthenticationMethod[] { new PasswordAuthenticationMethod(auth.User, auth.Password) });
            info.Timeout = new TimeSpan(0, 0, 30);
            return info;

            //if (auth != null)
            //{

            //}
            //else
            //{
            //    return new ConnectionInfo(_auth.Host, auth.Port, _auth.User, new AuthenticationMethod[] { new PasswordAuthenticationMethod(_auth.User, _auth.Password) });
            //}
        }

        public SshClient GetSshClient(SSHAuthMdl auth)
        {

            return new SshClient(GetConnectionInfo(auth));
        }

        public bool UploadFileToHost(SSHAuthMdl auth, Stream input, string pathToSave)
        {
            using (var sftp = new SftpClient(GetConnectionInfo(auth)))
            {
                sftp.Connect();
                sftp.UploadFile(input, pathToSave);
                sftp.Disconnect();
            }

            return true;
        }

        public bool UploadFileToHost(SSHAuthMdl auth, byte[] bytea, string pathToSave)
        {
            using (var sftp = new SftpClient(GetConnectionInfo(auth)))
            {
                sftp.Connect();
                sftp.WriteAllBytes(pathToSave, bytea);
                sftp.Disconnect();
            }

            return true;
        }

        public bool DonwloadFileFromHost(SSHAuthMdl auth, string filePath, string donwloadToPath)
        {
            using (var sftp = new SftpClient(GetConnectionInfo(auth)))
            {
                sftp.Connect();

                using (Stream fileStream = File.Create(donwloadToPath))
                {
                    sftp.DownloadFile(filePath, fileStream);
                }
                sftp.Disconnect();
            }

            return true;
        }

        public byte[] DonwloadFileFromHostToBytea(SSHAuthMdl auth,string filePath)
        {
            byte[] bytea;
            using (var sftp = new SftpClient(GetConnectionInfo(auth)))
            {
                sftp.Connect();
                
                using (var fileStream = new MemoryStream())
                {
                    sftp.DownloadFile(filePath, fileStream);
                    bytea = fileStream.ToArray();

                }
                sftp.Disconnect();
            }

            return bytea;
        }

        public byte[] GetFilesBytes(SSHAuthMdl auths, string filePath)
        {
            byte[] temp;
            using (var sftp = new SftpClient(new SSHSvc().GetConnectionInfo(auths)))
            {
                sftp.Connect();
                var stream = sftp.OpenRead(filePath);
                temp = new CommonHelprs().ReadToEnd(stream);
                sftp.Disconnect();
            }
            return temp;
        }


        public IEnumerable<byte[]> GetFilesBytes(SSHAuthMdl auths, IEnumerable<string> filePath)
        {
            var temps = new List<byte[]>();

            foreach (var p in filePath)
            {
                using (var sftp = new SftpClient(new SSHSvc().GetConnectionInfo(auths)))
                {
                    sftp.Connect();
                    var stream = sftp.OpenRead(p);
                    var temp = new CommonHelprs().ReadToEnd(stream);
                    temps.Add(temp);
                    sftp.Disconnect();
                }

            }



            return temps;
        }

        public string GetFilesString(SSHAuthMdl auths, string filePath)
        {
            var buffer = GetFilesBytes(auths, filePath);
            return Encoding.UTF8.GetString(buffer, 0, buffer.Length); ;
        }


        public bool DonwloadFileFromHostToTempDir(SSHAuthMdl auth, string filePath)
        {
            CreateLocalTempDir();
            var fileName = Path.GetFileName(filePath);
            using (var sftp = new SftpClient(GetConnectionInfo(auth)))
            {
                sftp.Connect();
                using (Stream fileStream = File.Create($"{GlobalVarHandler.LOCAL_TEMP_DIR}\\{fileName}"))
                {
                    sftp.DownloadFile(filePath, fileStream);
                }
                sftp.Disconnect();
            }

            return true;
        }

        public void DeleteLocalFile(string filePath)
        {
            File.Delete(filePath);
        }

        public void DeleteLocalTempDir()
        {
            File.Delete(GlobalVarHandler.LOCAL_TEMP_DIR);
        }

        public void CreateLocalTempDir()
        {
            if (Directory.Exists(GlobalVarHandler.LOCAL_TEMP_DIR))
            {
                System.IO.Directory.CreateDirectory(GlobalVarHandler.LOCAL_TEMP_DIR);
            }
        }

        public void ClearLocalTempDir()
        {
            DeleteLocalTempDir();
            CreateLocalTempDir();
        }



        public bool RunCommand(SshClient client, string command)
        {
            using (var sshCommand = client.RunCommand(command))
            {
                if (sshCommand.ExitStatus != 0) throw new ArgumentException($"Command {command} executed with error {sshCommand.Error}");
            }

            return true;
        }

        public bool RunCommand(SSHAuthMdl auth, string command, int count =0)
        {
            try
            {
                using (var client = GetSshClient(auth))
                {
                    using (var sshCommand = client.RunCommand(command))
                    {
                        if (sshCommand.ExitStatus != 0) throw new ArgumentException($"Command {command} executed with error {sshCommand.Error}");
                    }

                }
            }
            catch (Exception ex) 
            {
                if (ex.Message == "Client not connected")
                {
                    if (count < 4)
                    {
                        Thread.Sleep(5000);
                        RunCommand(auth, command);
                    }
                }

                return false;
            }


            return true;

        }

        public bool ExecuteBashOnHost(SSHAuthMdl auth, Stream bash)
        {
            var tempBashFileName = $"{GlobalVarHandler.ROOT_DIR}/{Guid.NewGuid()}.sh";
            UploadFileToHost(auth, bash, tempBashFileName);

            using (var client = GetSshClient(auth))
            {
                client.Connect();

                var cmds = new List<string>
            {
                $"chmod +x {tempBashFileName}",
               // $"sed -i -e 's/\r$//' {tempBashFileName}",
                tempBashFileName,
                $"rm {tempBashFileName}"

            };

                ExecuteCommands(auth,cmds);

                client.Disconnect();
            }

            return true;
        }

        public bool ExecuteBashOnHost(SSHAuthMdl auth, byte[] bashBytea)
        {
            var tempBashFileName = $"{GlobalVarHandler.ROOT_DIR}/{Guid.NewGuid()}.sh";
            UploadFileToHost(auth, bashBytea, tempBashFileName);

            using (var client = GetSshClient(auth))
            {
                client.Connect();

                var cmds = new List<string>
            {
                $"chmod +x {tempBashFileName}",
                $"sed -i -e 's/\r$//' {tempBashFileName}",
                tempBashFileName,
                $"rm {tempBashFileName}"

            };

                ExecuteCommands(auth,cmds);

                client.Disconnect();
            }

            return true;
        }


        public string ExecuteBashOnHostByCommandReturnBashPath(SSHAuthMdl auth, byte[] bashBytea)
        {
            var tempBashFileName = $"{GlobalVarHandler.ROOT_DIR}/{Guid.NewGuid()}.sh";
            UploadFileToHost(auth, bashBytea, tempBashFileName);
            ExecuteCommands(auth, $"chmod +x {tempBashFileName}");
            ExecuteCommands(auth, $"sed -i -e 's/\r$//' {tempBashFileName}");
            ExecuteCommands(auth, tempBashFileName);

            return tempBashFileName;
        }

        public bool ExecuteCommandsAsBash(SSHAuthMdl auth, IEnumerable<string> commands)
        {
            var tempBashFileName = "/root/" + Guid.NewGuid() + ".sh";
            using (var bash = new SSHBash().GenerateBashStream(commands))
            {
                UploadFileToHost(auth, bash, tempBashFileName);
            }

            var cmds = new List<string>
            {
                "chmod +x " + tempBashFileName,
                tempBashFileName,
                "rm " + tempBashFileName

            };

          return  ExecuteCommands(auth,cmds);
        }

        public IEnumerable<string> ExecuteCommandsWithResult(SSHAuthMdl auth, IEnumerable<string> commands)
        {
            var results = new List<string>();
            var cmds = commands.ToList();

            using (var client = GetSshClient(auth))
            {
                client.Connect();
                // ConsoleHandler.DrawConnected();

                foreach (var c in cmds)
                {
                    Console.WriteLine($"{auth.Port}: run ${c};");
                    try
                    {
                        var cmd = client.CreateCommand(c);
                        cmd.CommandTimeout = new TimeSpan(0, 0, 30);
                        using (cmd)
                        {
                            cmd.Execute();
                            //ConsoleHandler.ExcutingCommand(c, auth);
                            if (!string.IsNullOrWhiteSpace(cmd.Result))
                            {
                                var r = cmd.Result.TrimEnd('\r', '\n');
                                results.Add(r);
                                Debug.WriteLine(cmd.Result);
                                //ConsoleHandler.DrawResult(new CommonHelprs().ProcessResult(cmd.Result), auth);
                                Debug.WriteLine(cmd.Error);
                            }
                            else
                            {
                                // ConsoleHandler.EmptyResult(auth);
                            }

                            if (cmd.ExitStatus != 0)
                            {
                                //ConsoleHandler.Error(cmd.Error, auth);
                                break;
                            }

                            // ConsoleHandler.ExcutedCommand(c, auth);
                        }
                    }
                    catch (Exception ex)
                    {
                        //if (c == "reboot")
                        //{
                        //    //ConsoleHandler.Error("reboot", auth);
                        //}
                        break;

                    }
                    Console.WriteLine($"{auth.Port}: done ${c};");

                }

                client.Disconnect();

            }

            return results;
        }



        public bool ExecuteCommands(SSHAuthMdl auth, params string[] commands)
        {
           return ExecuteCommands(auth, commands.ToList());
        }

        public bool ExecuteCommands(SSHAuthMdl auth, IEnumerable<string> commands)
        {

            var cmds = commands.ToList();
            using (var client = GetSshClient(auth))
            {
                client.Connect();
                foreach (var c in cmds)
                {
                    try
                    {
                        Debug.WriteLine($"{auth.Host}:{auth.Port} Execute command: {c}");
                        var cmd = client.CreateCommand(c);
                        var result = cmd.BeginExecute();
                        //string output;
                        using (var reader = new StreamReader(cmd.OutputStream, Encoding.UTF8, true, 1024, true))
                        {
                            while (!result.IsCompleted)
                            {
                                var output1 = reader.ReadToEnd();
                                if (!string.IsNullOrWhiteSpace(output1))
                                {
                                    Debug.WriteLine($"${auth.Host}:{auth.Port} {output1}");
                                }
                                
                                //string line = reader.ReadLine();
                                //if (line != null)
                                //{
                                //    Debug.WriteLine($"${auth.Host}:{auth.Port} {(line + Environment.NewLine)}");
                                //    //Console.Write($"${auth.Host}: {(line + Environment.NewLine)}", Color.BlueViolet);
                                //}
                            }

                            var output2 = reader.ReadToEnd();
                            if (!string.IsNullOrWhiteSpace(output2))
                            {
                                Debug.WriteLine($"${auth.Host}:{auth.Port} {output2}");
                            }
                        }



                        cmd.EndExecute(result);



                    }
                    catch (Exception ex)
                    {
                        if (c == "reboot")
                        {
                            break;
                        }
                    }

                }

                client.Disconnect();

            }

            return true;
        }

        //private string CreatenCommand(SshClient client, string command)
        //{
        //    using (var sc = client.CreateCommand(command))
        //    {
        //        sc.Execute();
        //        if (sc.ExitStatus != 0) throw new ArgumentException($"Command {command} executed with error {sc.Error}");
        //        return sc.Result;
        //    }
        //}


        //public void DowbloadFileToHost(SSHAuthMdl auth, string pathToFile, string nametoFile)
        //{
        //    using (var sftp = new SftpClient(GetConnectionInfo(auth)))
        //    {
        //        sftp.Connect();
        //        using (Stream fileStream = File.Create($"C:\\pubfiles\\{nametoFile}"))
        //        {
        //            sftp.DownloadFile(pathToFile, fileStream);
        //        }
        //        sftp.Disconnect();
        //    }
        //}

    }
}
