using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.Extensions;
using TONBRAINS.TONOPS.Core.Handlers;
using TONBRAINS.TONOPS.Core.SSH;

namespace TONBRAINS.TONOPS.Core.Services
{
    public class TONClientSvc
    {


        public Dictionary<string, IEnumerable<string>> GetConfig(params string[] nodeIds)
        {
            return ExecuteWithResult($"getconfig", nodeIds);
        }

        public Dictionary<string, IEnumerable<string>> SendFile(string filePath, params string[] nodeIds)
        {
            return ExecuteWithResult($"sendfile {filePath}", nodeIds);
        }

        public Dictionary<string, IEnumerable<string>> GetAccount(string account, params string[] nodeIds)
        {
            return ExecuteWithResult($"getaccount {account}", nodeIds);
        }

        public Dictionary<string, IEnumerable<string>> GetLastBlock(IEnumerable<string> nodeIds)
        {
            return ExecuteWithResult("last", nodeIds);
        }

        public Dictionary<string, IEnumerable<string>> GetTime(IEnumerable<string> nodeIds)
        {
            return ExecuteWithResult("time", nodeIds);
        }

        public Dictionary<string, bool> GetRemoteVersion(IEnumerable<string> nodeIds)
        {
            return Execute("remote-version", nodeIds);
        }

        public Dictionary<string, bool> Execute(string command, IEnumerable<string> nodeIds)
        {

            var cmds = new List<string>
            {
                $"{GetBaseCommand()} -rc {command.ToQQ()} -rc \"quit\"",
            };
            return new SSHCommandHlp(nodeIds).ExecuteCommandsParallel(cmds);

        }

        public Dictionary<string, IEnumerable<string>> ExecuteWithResult(string command, IEnumerable<string> nodeIds)
        {
            var cmds = new List<string>
            {
                $"{GetBaseCommand()} -rc {command.ToQQ()} -rc \"quit\"",
            };

            var rs = new SSHCommandHlp(nodeIds).ExecuteCommandsWithResultParallel(cmds);
            return rs;

        }

        public string GetBaseCommand()
        {

            return $"{GlobalVarHandler.LITE_CLIENT.ToQQ()} -p {GlobalVarHandler.LITE_SERVER_PUB.ToQQ()} -a {GlobalVarHandler.LITESERVER_IP}:{GlobalVarHandler.LITESERVER_PORT}";
        }

    }
}
