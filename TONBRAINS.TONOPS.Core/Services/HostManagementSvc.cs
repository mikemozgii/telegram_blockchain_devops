using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.DAL;
using TONBRAINS.TONOPS.Core.Handlers;
using TONBRAINS.TONOPS.Core.Models;
using TONBRAINS.TONOPS.WebApp.WebApp.Helpers;

namespace TONBRAINS.TONOPS.Core.Services
{
    public class HostManagementSvc
    {

        public bool AddNode(string hostId, string nodeCoreType, int Index, int externSSHPort)
        {
            var HostDbSvc = new HostDbSvc();
            var host = HostDbSvc.GetById(hostId);


            if (host.HostTypeId == "hyperv2016")
            {
                var HyperVHostManagementSvc = new HyperVHostManagementSvc();
                HyperVHostManagementSvc.AddNewVm(host, nodeCoreType, Index, externSSHPort);
            }
            else if (host.HostTypeId == "hyperv2019")
            {
                var HyperVHostManagementSvc = new HyperVHostManagementSvc();
                HyperVHostManagementSvc.AddNewVm(host, nodeCoreType, Index, externSSHPort);
            }

            return true;      
        }


        public bool DeleteNode(string nodeId)
        {
            var NodeDbSvc = new NodeDbSvc();
            var node = NodeDbSvc.GetById(nodeId);
            var host = new HostDbSvc().GetById(node.HostId);


            if (host.HostTypeId == "hyperv2016")
            {
                new HyperVHostManagementSvc().DeleteVm(host, host.NodeLocationPath, node.InstanceName);
            }


           var r = NodeDbSvc.SoftDeleteById(nodeId);

            return r;
        }

        public bool StartNode(string nodeId)
        {

            var NodeDbSvc = new NodeDbSvc();
            var node = NodeDbSvc.GetById(nodeId);
            var host = new HostDbSvc().GetById(node.HostId);

            if (host.HostTypeId == "hyperv2016")
            {
                new HyperVHostManagementSvc().StartVM(host, node.InstanceName);
                node.Status = "run";
                NodeDbSvc.Update(node);
            }

            return true;
        }

        public bool StopNode(string nodeId)
        {
            var NodeDbSvc = new NodeDbSvc();
            var node = NodeDbSvc.GetById(nodeId);
            var host = new HostDbSvc().GetById(node.HostId);


            if (host.HostTypeId == "hyperv2016")
            {
                new HyperVHostManagementSvc().StopVM(host, node.InstanceName);
                node.Status = "stop";
                NodeDbSvc.Update(node);
            }

            return true;
        }

        public bool RestartNode(string nodeId)
        {
            var NodeDbSvc = new NodeDbSvc();
            var node = NodeDbSvc.GetById(nodeId);
            var host = new HostDbSvc().GetById(node.HostId);


            if (host.HostTypeId == "hyperv2016")
            {
                new HyperVHostManagementSvc().RestarVM(host, node.InstanceName);
                node.Status = "restart";
                NodeDbSvc.Update(node);
            }

            return true;
        }
        public bool TestPowerShellConnection(string hostId)
        {
            var host = new HostDbSvc().GetById(hostId);

            if (host.HostTypeId == "hyperv2016")
            {
                new HyperVHostManagementSvc().TestPowerShellConnection(host);
            }

            return true;
        }


        //

    }
}
