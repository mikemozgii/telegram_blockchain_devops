using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.DAL;
using TONBRAINS.TONOPS.Core.Handlers;
using TONBRAINS.TONOPS.Core.Models;
using TONBRAINS.TONOPS.Core.SSH;
using TONBRAINS.TONOPS.WebApp.WebApp.Helpers;

namespace TONBRAINS.TONOPS.Core.Services
{
    public class NodeManagementSvc
    {

        public bool ConfigureNewNode(Host host, NodeCoreType nodeCoreType, string newNodeName, string newNodeIp, int nodeSSHExtenral)
        {
            var creds = new CredentialDbSvc().GetById(host.VmCoreCredentialId);
            var hostName = newNodeName;
            var UbuntuServerSvc = new UbuntuServerSvc();        
            UbuntuServerSvc.SetupHostName(host.Ip, nodeCoreType.Port, creds.UserName, creds.Password, hostName);
            UbuntuServerSvc.SetupStaticIp(host.Ip, nodeCoreType.Port, creds.UserName, creds.Password, newNodeIp, nodeSSHExtenral);
            return true;
        }

        public void RebootNodes(IEnumerable<string> nodeIds)
        {
            new SSHCommandHlp(nodeIds).ExecuteCommandsWithResultParallel(new List<string> { "reboot" });
        }

        public void StopNodes(IEnumerable<string> nodeIds)
        {
            new SSHCommandHlp(nodeIds).ExecuteCommandsWithResultParallel(new List<string> { "shutdown -h now" });
        }


        //public bool AddNewTonNodeto_UbuntuServer_ByHypervBYCount(int initStartRange, int initEndRange)
        //{
        //    for (var i = initStartRange; i <= initEndRange;)
        //    {
        //        AddNewTonNodeto_UbuntuServer_ByHyperv(i);
        //        i++;
        //    }
        //    return true;
        //}
    }
}
