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
    public class TONValidatorEngineConsoleSvc
    {


        public Dictionary<string, IEnumerable<string>> GetStats(IEnumerable<string> nodeIds)
        {
            return ExecuteWithResult($"getstats", nodeIds);
        }

        //TIMEDIFF
        public Dictionary<string, int>  GetMasterBlockTimeDifference(IEnumerable<string> nodeIds)
        {

            var frs = new Dictionary<string, int>();
            var rs = ExecuteWithResult($"getstats", nodeIds);
            foreach (var r in rs)
            {
                try
                {
                    if (r.Value.Count() != 0)
                    {
                        var fr = r.Value.First().Split("\n");
                        var unixtime = fr[4].ReplaceWihtEmpty("unixtime").Trim();
                        var masterchainblocktime = fr[6].ReplaceWihtEmpty("masterchainblocktime").Trim();
                        var diff = Convert.ToInt32(unixtime) - Convert.ToInt32(masterchainblocktime);
                        frs.Add(r.Key, diff);
                    }
                    else
                    {
                        frs.Add(r.Key, 9999);
                    }
                }
                catch
                {
                    frs.Add(r.Key, 9999);
                }


            }
            return frs;
        }

        public Dictionary<string, int> GetMasterBlockTimeDifferenceForTonNetwork(string tonNetworkdId)
        {
            var TonNetworkSvc = new TonNetworkSvc();
            var nodeIds = TonNetworkSvc.GetNodesByTonNetworkId(tonNetworkdId).Select(q=>q.Id);
            return GetMasterBlockTimeDifference(nodeIds);
        }


        public Dictionary<string, bool> Execute(string command, IEnumerable<string> nodeIds)
        {



            var cmds = new List<string>
            {
                $"{GetBaseCommand()} -rc {command.ToQQ()} -rc \"quit\"",
            };

           
            return new SSHCommandHlp(nodeIds).ExecuteCommandsParallel(cmds);

        }

        //"/root/ton/build/blockchain-explorer/blockchain-explorer" -p "/root/ton-keys/server.pub" -c "/var/ton-work/etc/ton-global.config.json" -a 127.0.0.1:3030
        //"/root/ton/build/blockchain-explorer/blockchain-explorer" -p "/root/ton-keys/liteserver.pub" -C "/var/ton-work/etc/ton-global.config.json" -a 127.0.0.1:3031
        public Dictionary<string, IEnumerable<string>> ExecuteWithResult(string command, IEnumerable<string> nodeIds)
        {
            var cmds = new List<string>
            {
                $"{GetBaseCommand()} -c {command.ToQQ()} -c \"quit\"",
            };

            var rs = new SSHCommandHlp(nodeIds).ExecuteCommandsWithResultParallel(cmds);
            return rs;

        }

        public string GetBaseCommand()
        {
            return $"{GlobalVarHandler.VALIDATOR_ENGINE_CONSOLE.ToQQ()} -p {GlobalVarHandler.VALIDATOR_ENGINE_PUB.ToQQ()} -k {GlobalVarHandler.CLIENT_KEY.ToQQ()} -a {GlobalVarHandler.ENGINESERVER_IP}:{GlobalVarHandler.ENGINESERVER_PORT}";
        }


    }
}
