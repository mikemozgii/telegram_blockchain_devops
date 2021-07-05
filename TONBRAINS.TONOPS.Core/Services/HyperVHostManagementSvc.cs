using System;
using System.Collections.Generic;
using System.IO;
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
    public class HyperVHostManagementSvc
    {



        public PowerShellConnMdl GetPowerShellConnectionByHost(Host host)
        {
            var powerShellCredential = new CredentialDbSvc().GetById(host.PowershellCredentialId);
            var powerShellConn = new PowerShellConnMdl
            {
                HostIp = host.Ip,
                Port = host.PowershellPort,
                UserName = powerShellCredential.UserName,
                Password = powerShellCredential.Password
            };


            return powerShellConn;

        }

        public bool AddNewVm(Host host, string nodeCoreType, int Index, int externSSHPort)
        {

            var vmCorePath = "";
            var newVmName = "";
            var Os = "";
            var NodeType = NodeTypes.Undefined;
            var GroupId = GlobalAppConfHandler.NewNodeGroupId;
            NodeCoreType NodeCoreType = null;
            if (nodeCoreType == "ubuntuCore20_04")
            {
                vmCorePath = host.UbuntuCore2004Path;
                newVmName = $"Ubuntu{Index}";
                Os = GlobalAppConfHandler.UnuntuServer2004LTS;
                NodeCoreType = new NodeCoreTypeDbSvc().GetById("ubuntuCore20_04");
            }
            else if (nodeCoreType == "ubuntuTONCore20_04")
            {
                vmCorePath = host.UbuntuTonCore2004Path;
                newVmName = $"UbuntuTon{Index}";
                Os = GlobalAppConfHandler.UnuntuServer2004LTS;
                NodeType = NodeTypes.QUANTONReady;
                GroupId = GlobalAppConfHandler.NewNodeGroupId;
                NodeCoreType = new NodeCoreTypeDbSvc().GetById("ubuntuTONCore20_04");
            }

            var powerShellConn = GetPowerShellConnectionByHost(host);


            var newVmInternalAddress = $"{host.IpSubset}{Index}";
            var coreVmName = Path.GetFileName(vmCorePath);
            var PowerShellSvc = new PowerShellSvc();
            PowerShellSvc.AddNewVM(powerShellConn, vmCorePath, coreVmName, host.NodeLocationPath, newVmName, 8, host.VCpuCores-2);
            AddNewForwardSSHRule(powerShellConn, newVmInternalAddress, externSSHPort);
            OpenFireWallPortTCP(powerShellConn, $"{newVmName}_SSH_External {externSSHPort}", externSSHPort);

            //NodeCOnfiguration
            Thread.Sleep(60000);
            var r = new NodeManagementSvc().ConfigureNewNode(host, NodeCoreType, newVmName, newVmInternalAddress, externSSHPort);

            // final restart fixes some issues;
            RestarVM(powerShellConn, newVmName);


            var node = new Node
            {
                Id = IdGenerator.Generate(),
                CredentialId = GlobalAppConfHandler.CredentialsDefaultId,
                HostName = newVmName,
                InstanceName = newVmName,
                IsNtp = true,
                IsRoot = true,
                IsStaticIp = true,
                Modules = "[]",
                Os = Os,
                Status = "run",              
                GroupId = GroupId,
                DeploymentType = DeploymentNodeTypes.VM,
                Ip = newVmInternalAddress,
                SshIp = host.Ip,
                SshPort = externSSHPort,
                Name = newVmName,
                Description = "-",
                Type = NodeType,
                HostId = host.Id
            };

            new NodeDbSvc().Add(node);

            return true;
        }

        public bool AddNewForwardSSHRule(PowerShellConnMdl conn, string newIpAddress, int externPort)
        {
            return new PowerShellSvc().AddNewForwardSSHRule(conn, newIpAddress, externPort);
        }

        public bool OpenFireWallPortTCP(PowerShellConnMdl conn, string name, int externPort)
        {
            return new PowerShellSvc().OpenFireWallPortTCP(conn, name, externPort);
        }

        public bool RestarVM(PowerShellConnMdl conn, string vmName)
        {
            return new PowerShellSvc().RestarVM(conn, vmName);
        }

        public bool RestarVM(Host host, string vmName)
        {
            return RestarVM(GetPowerShellConnectionByHost(host), vmName);
        }


        public bool StopVM(PowerShellConnMdl conn, string vmName)
        {
            return new PowerShellSvc().StopVM(conn, vmName);
        }
        public bool StopVM(Host host, string vmName)
        {
            var conn = GetPowerShellConnectionByHost(host);
            return StopVM(conn,vmName);
        }

        public bool StartVM(PowerShellConnMdl conn, string vmName)
        {
            return new PowerShellSvc().StartVM(conn, vmName);
        }


        public bool StartVM(Host host, string vmName)
        {
            var conn = GetPowerShellConnectionByHost(host);
            return StartVM(conn, vmName);
        }

        public bool DeleteVm(PowerShellConnMdl conn, string vmPath, string vmName)
        {
            return new PowerShellSvc().DeleteVM(conn, vmPath, vmName);
        }

        public bool DeleteVm(Host host, string vmPath, string vmName)
        {
            var conn = GetPowerShellConnectionByHost(host);
            return DeleteVm(conn, vmPath, vmName);
        }

        public bool TestPowerShellConnection(Host host)
        {
            var conn = GetPowerShellConnectionByHost(host);
            return new PowerShellSvc().TestConnection(conn);
        }


    }
}
