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
    public class TONBlockchainExplorerSvc
    {



        public Dictionary<string, bool> Run(IEnumerable<string> nodeIds)
        {

            var cmds = new List<string>
            {
                $"{GetBaseCommand()}",
            };

            return new SSHCommandHlp(nodeIds).ExecuteCommandsParallel(cmds);

        }

        //public Dictionary<string, IEnumerable<string>> ExecuteWithResult(string command, IEnumerable<string> nodeIds)
        //{
        //    var cmds = new List<string>
        //    {
        //        $"{GetBaseCommand()} -c {command.ToQQ()} -c \"quit\"",
        //    };

        //    var rs = new SSHCommandHlp(nodeIds).ExecuteCommandsWithResultParallel(cmds);
        //    return rs;

        //}

        public string GetBaseCommand()
        {
            return $"{GlobalVarHandler.BLOCKCHAIN_EXPLORER.ToQQ()} -p {GlobalVarHandler.LITE_SERVER_PUB.ToQQ()} -C {GlobalVarHandler.TON_WORK_GLOBAL_CONFIG_PATH.ToQQ()} -a {GlobalVarHandler.LITESERVER_IP}:{GlobalVarHandler.LITESERVER_PORT}";
        }
        
        
        //"/root/ton/build/blockchain-explorer/blockchain-explorer" -p "/root/ton-keys/liteserver.pub" -C "/var/ton-work/etc/ton-global.config.json" -a 127.0.0.1:3031

    }
}
