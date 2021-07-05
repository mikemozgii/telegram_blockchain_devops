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
    public class TONNodeMaitananceSvc
    {
        public bool CheckNodeStatus(IEnumerable<string> nodeIds)
        {
            var content = new CommonHelprs().GetBashContentFromFile("check_node_sync_status").ReplaceByConfigValues();
            new SSHCommandHlp(nodeIds).ExecuteCommandsParallelBashByContent(content);
            return true;
        }
    }
}
