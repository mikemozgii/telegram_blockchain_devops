using Renci.SshNet;
using Renci.SshNet.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.DAL;
using TONBRAINS.TONOPS.Core.Extensions;
using TONBRAINS.TONOPS.Core.Handlers;

namespace TONBRAINS.TONOPS.Core.SSH
{
    public class SSHCommandHlp : NodeBase
    {
        public SSHCommandHlp(params SSHAuthMdl[] _nodes) : base(_nodes)
        {
        }

        public SSHCommandHlp(params string[] _nodeIds) : base(_nodeIds)
        {
        }

        public SSHCommandHlp(params Node[] _nodes) : base(_nodes)
        {
        }

        public SSHCommandHlp(IEnumerable<Node> _nodes) : base(_nodes)
        {
        }

        public SSHCommandHlp(IEnumerable<string> _nodeIds) : base(_nodeIds)
        {
        }




        public Dictionary<string, bool> ExecuteCommands(IEnumerable<string> cmds)
        {
            var rs = DictionaryExt.GetDefaultDic();

            foreach (var n in nodes)
            {
                rs.Add(n.Id, new SSHSvc().ExecuteCommands(n, cmds));
            }
            return rs;
        }

        public Dictionary<string, bool> ExecuteCommands(params string[] cmds)
        {
            return ExecuteCommands(cmds.ToList());
        }

        public Dictionary<string, IEnumerable<string>> ExecuteCommandsWithResult(IEnumerable<string> cmds)
        {

            var rs = DictionaryExt.GetDefaultResultDic();
            foreach (var n in nodes)
            {
                rs.Add(n.Id, new SSHSvc().ExecuteCommandsWithResult(n, cmds));
            }
            return rs;
        }

        public Dictionary<string, IEnumerable<string>> ExecuteCommandsWithResult(params string[] cmds)
        {
            return ExecuteCommandsWithResult(cmds.ToList());
        }

        public Dictionary<string, bool> ExecuteCommandsParallel(IEnumerable<string> cmds)
        {
            var rs = DictionaryExt.GetConcurrentDefaultDic();
            var tasks = nodes.Select(q=> Task.Run(() => { rs.TryAdd(q.Id, new SSHSvc().ExecuteCommands(q, cmds)); } ));
            Task.WaitAll(tasks.ToArray());
            return rs.ConvertConcurrentToDic();
        }

        public void OpenTunnels(int seconds, ConcurrentDictionary<string, SshClient> cd)
        {
            nodes.Select(q => Task.Run(() => OpenTunnel(q, seconds, cd))).ToList();
        }

        private async Task OpenTunnel(SSHAuthMdl auth, int seconds, ConcurrentDictionary<string, SshClient> cd)
        {
            var started = DateTime.UtcNow;
            PasswordAuthenticationMethod passordAuth = new PasswordAuthenticationMethod(auth.User, auth.Password);
            ConnectionInfo conInfo = new ConnectionInfo(auth.Host, auth.Port, auth.User, new AuthenticationMethod[] { passordAuth });
            using (var client = new SshClient(conInfo))
            {
                try
                {
                    cd.TryRemove(auth.Id, out var old);
                    old?.Disconnect();
                    ForwardedPort port = new ForwardedPortLocal("127.0.0.1", (uint)auth.Port, "127.0.0.1", 80);
                    client.Connect();
                    client.AddForwardedPort(port);
                    port.RequestReceived += 
                        (object sender, PortForwardEventArgs e) => Console.WriteLine("Tunnel connection: {0}:{1}", e.OriginatorHost, e.OriginatorPort); //new EventHandler<PortForwardEventArgs>(port_RequestReceived);
                    port.Exception += 
                        (object sender, ExceptionEventArgs e) => Console.WriteLine("Tunnel exception: {0} (Innner: {1})");
                    port.Start();
                    if (cd.TryAdd(auth.Id, client))
                        while (true)
                        {
                            await Task.Delay(1000);
                            if ((DateTime.UtcNow - started).TotalSeconds >= seconds) break;
                        }

                    cd.TryRemove(auth.Id, out var closed);
                    closed?.Disconnect();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public Dictionary<string, bool> ExecuteCommandsAsBashParallel(IEnumerable<string> cmds)
        {
            var rs = DictionaryExt.GetConcurrentDefaultDic();
            var tasks = nodes.Select(q => Task.Run(() => { rs.TryAdd(q.Id, new SSHSvc().ExecuteCommandsAsBash(q, cmds)); }));
            Task.WaitAll(tasks.ToArray());
            return rs.ConvertConcurrentToDic();
        }

        public Dictionary<string, bool> ExecuteCommandsAsBashParallel(params string[] cmds)
        {
            return ExecuteCommandsAsBashParallel(cmds.ToList());
        }

        public Dictionary<string, IEnumerable<string>> ExecuteCommandsWithResultParallel(IEnumerable<string> cmds)
        {

            var rs = DictionaryExt.GetConcurrentDefaulResultDic();
            var tasks = nodes.Select(q => Task.Run(() => { rs.TryAdd(q.Id, new SSHSvc().ExecuteCommandsWithResult(q, cmds)); }));
            Task.WaitAll(tasks.ToArray());
            return rs.ConvertConcurrentToDic();
        }

        public Dictionary<string, IEnumerable<string>> ExecuteCommandsWithResultParallel(params string[] cmds)
        {
            return ExecuteCommandsWithResultParallel(cmds.ToList());
        }




        //public Dictionary<string, IEnumerable<string>> ExecuteCommandsWithResultParallelForEachNode(IEnumerable<IEnumerable<string>> cmds)
        //{

        //    var nodess = nodes.ToList();
        //    var rs = GetConcurrentDefaulResultDic();
        //    var cmdss = cmds.ToList();
        //    var tasks = nodes.Select(q => Task.Run(() => {

        //        var index =nodess.IndexOf(q);
        //        rs.TryAdd(q.Id, new SSHSvc().ExecuteCommandsWithResult(q, cmdss[index])); 
            
        //    }));
            
        //    Task.WaitAll(tasks.ToArray());
        //    return rs.ConvertConcurrentToDic();
        //}



        //public Dictionary<string, bool> ExecuteCommandsParallel(IEnumerable<IEnumerable<string>> cmds)
        //{


        //    var nodess = nodes.ToList();
        //    var rs = GetConcurrentDefaultDic();
        //    var cmdss = cmds.ToList();
        //    var tasks = nodes.Select(q => Task.Run(() => {
        //        var index = nodess.IndexOf(q);
        //        rs.TryAdd(q.Id, new SSHSvc().ExecuteCommandsAsBash(q, cmdss[index])); }));
        //    Task.WaitAll(tasks.ToArray());
        //    return rs.ConvertConcurrentToDic();
        //}


        public Dictionary<string, bool> ExecuteCommandsParallel(params string[] cmds)
        {
            return ExecuteCommandsParallel(cmds.ToList());
        }

        public Dictionary<string, bool> UploadFileToHostParallel(byte[] bytea, string path)
        {
            var rs = DictionaryExt.GetConcurrentDefaultDic();
            var tasks = nodes.Select(q => Task.Run(() => {rs.TryAdd(q.Id, new SSHSvc().UploadFileToHost(q, bytea, path));}));

            Task.WaitAll(tasks.ToArray());

            return rs.ConvertConcurrentToDic();
        }

        public Dictionary<string, bool> UploadFileToHost(byte[] bytea, string path)
        {

            var rs = DictionaryExt.GetDefaultDic();

            foreach (var n in nodes)
            {
                rs.Add(n.Id, new SSHSvc().UploadFileToHost(n, bytea, path));
            }

            return rs;
        }

        public Dictionary<string, byte[]> DownloadFileToByteaHostParallel(string path)
        {
     
            var byteas = new List<byte[]>();


            var rs = DictionaryExt.GetConcurrentDefaultByteaDic();
            var tasks = nodes.Select(q => Task.Run(() => { rs.TryAdd(q.Id, new SSHSvc().DonwloadFileFromHostToBytea(q, path)); }));

            Task.WaitAll(tasks.ToArray());

            return rs.ConvertConcurrentToDic();
        }

        public Dictionary<string, byte[]> DownloadFileToByteaHost(string path)
        {

            var rs = DictionaryExt.GetDefaultByteaDic();
            foreach (var n in nodes)
            {
                rs.Add(n.Id, new SSHSvc().DonwloadFileFromHostToBytea(n, path));    
            }
        

            return rs;
        }

        //public Dictionary<string, IEnumerable<byte[]>> GetFilesBytesParallel(IEnumerable<string> paths)
        //{



        //    var nodess = nodes.ToList();
        //    var rs = GetConcurrentDefaulResultByteasDic();
        //    var tasks = nodes.Select(q => Task.Run(() => {

        //        var index = nodess.IndexOf(q);
        //        rs.TryAdd(q.Id, new SSHSvc().GetFilesBytes(q, paths));

        //    }));

        //    Task.WaitAll(tasks.ToArray());
        //    return rs.ConvertConcurrentToDic();
        //}
        
        public Dictionary<string, bool> ExecuteCommandsParallelBash(string bashName)
        {
            var content = new CommonHelprs().GetBashContentFromFile(bashName);
            return ExecuteCommandsParallelBashByContent(content);
        }

        public Dictionary<string, bool> ExecuteCommandsParallelBashByContent(string bashContent)
        {


            var rs = DictionaryExt.GetConcurrentDefaultDic();
            var bytea = new CommonHelprs().GenerateByteaFromString(bashContent);
            var tasks = nodes.Select(q => Task.Run(() => { 
                
               
                rs.TryAdd(q.Id, new SSHSvc().ExecuteBashOnHost(q, bytea)); 
            
            
            }));

            Task.WaitAll(tasks.ToArray());

            return rs.ConvertConcurrentToDic();
        }


        public Dictionary<string, bool> DeleteCreateDirectoryParallel(string dirPathpath)
        {
            var cmds = new List<string>
            {
                $"rm -rf {dirPathpath.ToQQ()}",
                $"mkdir -p {dirPathpath.ToQQ()}",
            };

            return ExecuteCommandsParallel(cmds);
        }

        public Dictionary<string, bool> DeleteCreateDirectory(string dirPathpath)
        {
            var cmds = new List<string>
            {
                $"rm -rf {dirPathpath.ToQQ()}",
                $"mkdir -p {dirPathpath.ToQQ()}",
            };

            return ExecuteCommands(cmds);
        }

        public Dictionary<string, bool> CreateDirectoryParallel(string dirPathpath)
        {
            var cmds = new List<string>
            {
                $"mkdir -p {dirPathpath.ToQQ()}",
            };

            return ExecuteCommandsParallel(cmds);
        }

        public Dictionary<string, bool> CreateDirectory(string dirPathpath)
        {
            var cmds = new List<string>
            {
                $"mkdir -p {dirPathpath.ToQQ()}",
            };

            return ExecuteCommands(cmds);
        }


        public Dictionary<string, bool> DeleteFileParallel(string path)
        {
            var cmds = new List<string>
            {
                $"rm -rf {path.ToQQ()}",
            };

            return ExecuteCommandsParallel(cmds);
        }

        public Dictionary<string, bool> DeleteFile(string path)
        {
            var cmds = new List<string>
            {
                $"rm -rf {path.ToQQ()}",
            };

            return ExecuteCommands(cmds);
        }



    }
}
