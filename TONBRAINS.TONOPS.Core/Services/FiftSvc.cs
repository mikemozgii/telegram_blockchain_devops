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
    public class FiftSvc
    {
        //rm -f
        //public bool NewWallet(IEnumerable<string> nodeIds)
        //{
        //    var test_wallet = "test_wallet";
        //    var s = "/root/ton/crypto/smartcont/new-wallet.fif";
        //    Execute($"-s {s.ToQQ()} 0 test_wallet", nodeIds);

        //    var wallet_boc = $"{GlobalVarHandler.ROOT_DIR}/{test_wallet}-query.boc";
        //    new TONClientSvc().SendFile(wallet_boc, nodeIds);
        //    var cmds = new List<string>
        //    {
        //        $"rm -f {GlobalVarHandler.ROOT_DIR}/{test_wallet}.addr",
        //        $"rm -f {GlobalVarHandler.ROOT_DIR}/{test_wallet}.pk",
        //        $"rm -f {wallet_boc}",
        //    };

        //    new SSHSvc().ExecuteCommands(cmds, nodeIds);

        //    return true;
        //}


        //public bool GenZeroState(SSHAuthMdl auth = null)
        //{
        //    var test_wallet = "test_wallet";
        //    var s = "/root/ton/crypto/smartcont/new-wallet.fif";
        //    Execute($"-s {s.ToQQ()} 0 test_wallet", auth);

        //    new SSHSvc().ExecuteCommands(cmds, auth);

        //    return true;
        //}

        public bool Execute(string command, SSHAuthMdl auth = null)
        {
            var cmds = new List<string>
            {
                $"{GetBaseCommand()} {command}",
            };

            new SSHSvc().ExecuteCommands(auth, cmds);
            return true;

        }

        public bool ExecuteCreateZeroState(string command, IEnumerable<string> nodeIds)
        {
            var cmds = new List<string>
            {
                $"cd {GlobalVarHandler.SMARTCONT_DIR}",
                $"{GetBaseCreateState()} {command}",
            };
            new SSHCommandHlp(nodeIds).ExecuteCommandsAsBashParallel(cmds);
            return true;

        }

        //public IEnumerable<string> ExecuteWithResult(string command, SSHAuthMdl auth = null)
        //{
        //    var cmds = new List<string>
        //    {
        //        $"{GetBaseCommand()} {command}",
        //    };

        //    var rs = new SSHSvc().ExecuteCommandsWithResult(cmds, auth);
        //    return rs;

        //}

        public string GetBaseCreateState()
        {

            return $"{GlobalVarHandler.CREATE_STATE} -I \"{GlobalVarHandler.FIFT_LIB_DIR}:{GlobalVarHandler.SMARTCONT_DIR}\"";
        }

        public string GetBaseCommand()
        {

            return $"{GlobalVarHandler.FIFT} -I {GlobalVarHandler.FIFT_LIB_DIR.ToQQ()} ";
        }
    }
}
