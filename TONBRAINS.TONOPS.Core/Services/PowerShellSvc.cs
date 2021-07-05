using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation.Runspaces;
using System.Configuration;
using System.Security;
using System.Management.Automation;
using TONBRAINS.TONOPS.Core.Models;

namespace TONBRAINS.TONOPS.Core.Services
{
    public class PowerShellSvc
    {
        public string ExecuteCommamds(PowerShellConnMdl conn, IEnumerable<string> cmds)
        {
            string userName = conn.UserName;
            string password = conn.Password;
            var securestring = new SecureString();
            foreach (var c in password)
            {
                securestring.AppendChar(c);
            }
            var finalResult = "";
            var creds = new PSCredential(userName, securestring);
            // Remove logging if not needed
            //log.Info(String.Format("\tPOWERSHEL : Running Powershell {0} at location {1}", scriptToBeRun, location));
            string shellUri = "http://schemas.microsoft.com/powershell/Microsoft.PowerShell";
           // var connectionInfo = new WSManConnectionInfo(false, "142.11.250.90", 5985, "/wsman", shellUri, creds);
            var connectionInfo = new WSManConnectionInfo(false, conn.HostIp, conn.Port, "/wsman", shellUri, creds);
            Runspace runspace = RunspaceFactory.CreateRunspace(connectionInfo);
            runspace.Open();
            using (PowerShell ps = PowerShell.Create())
            {
                ps.Runspace = runspace;
               // ps.AddScript(@"cd " + location);             
                foreach (var c in cmds)
                {
                    ps.AddScript(c);
                }
                try
                {
                   
                    var results = ps.Invoke();
                    //log.Info("\tPOWERSHEL : Results from Powershell Script is ---------------------------");
                    foreach (var x in results)
                    {
                        //log.Info(x.ToString());
                        finalResult = $"{x}{Environment.NewLine}";
                    }
             
                   // log.Info("\tPOWERSHEL : End of results--------------------------------- ---------------------------");
                }
                catch (Exception e)
                {
                   // log.Error("\tPOWERSHEL : Exception from running Powershell Script is" + e.ToString());
                }

            }
            runspace.Close();

            if (string.IsNullOrWhiteSpace(finalResult)) return null;


            return finalResult;
        }


        public string ExecuteCommamd(PowerShellConnMdl conn,string cmd)
        {
           return ExecuteCommamds(conn, new string[] { cmd });
        }


        public bool AddNewVM(PowerShellConnMdl conn, string coreVmPath, string coreVmName, string newVmPath, string newVmName, int memory_gb, int cores)
        {
            var NewVmFullPathvhdxPath = $"{newVmPath}\\{newVmName}";
            var NewVmFullvhdxPath = $"{NewVmFullPathvhdxPath}\\Virtual Hard Disks\\{newVmName}.vhdx";
            var cmd = new List<string>() {
            $"Stop-VM -Name {coreVmName}",
            $"New-Item -ItemType directory -Path '{NewVmFullPathvhdxPath}'",
            $"New-Item -ItemType directory -Path '{NewVmFullPathvhdxPath}\\Virtual Hard Disks'",
            $"Copy-Item -Path '{coreVmPath}\\Virtual Hard Disks\\{coreVmName}.vhdx' -Destination '{NewVmFullvhdxPath}'",
            $"New-VM -Name {newVmName} -MemoryStartupBytes {memory_gb}GB -BootDevice VHD -VHDPath '{NewVmFullvhdxPath}' -Path '{newVmPath}' -Generation 1 -Switch NatSwitch",
            $"Set-VMProcessor {newVmName} -Count {cores}",
            $"Remove-VMDvdDrive -VMName {newVmName} -ControllerNumber 1 -ControllerLocation 0",
            $"Start-VM -Name {newVmName}"
            };
            var r = ExecuteCommamds(conn,cmd);
            return true;
        }
        public bool RestarVM(PowerShellConnMdl conn, string VmName)
        {
            var cmd = new List<string>() {
            $"Restart-VM -Name {VmName} -Force",
            };
            var r = ExecuteCommamds(conn, cmd);
            return true;
        }


        public bool StartVM(PowerShellConnMdl conn, string VmName)
        {
            var cmd = new List<string>() {
            $"Start-VM -Name {VmName}",
            };
            var r = ExecuteCommamds(conn, cmd);
            return true;
        }

        public bool StopVM(PowerShellConnMdl conn, string VmName)
        {
            var cmd = new List<string>() {
            $"Stop-VM -Name {VmName}",
            };
            var r = ExecuteCommamds(conn, cmd);
            return true;
        }

        public bool DeleteVM(PowerShellConnMdl conn, string vmPath, string VmName)
        {
            var cmd = new List<string>() {
           
            $"Stop-VM -Name {VmName} -Force",
            $"Remove-VM -Name {VmName} -Force",
            $"Remove-Item '{vmPath}\\{VmName}' -Recurse  -Force -Confirm:$false",
            };
            var r = ExecuteCommamds(conn, cmd);
            return true;
        }

        public bool TestConnection(PowerShellConnMdl conn)
        {
            var cmd = new List<string>() {
            $"echo test",
            };
            var r = ExecuteCommamds(conn, cmd);
            return true;
        }


        public bool OpenFireWallPortTCP(PowerShellConnMdl conn, string name, int port)
        {
            var cmd = new List<string>() {
            $"netsh advfirewall firewall add rule name='{name}' dir=in action=allow protocol=TCP localport={port}",
            };
            var r = ExecuteCommamds(conn, cmd);
            return true;
        }
        //

        //public bool AddNewUbuntuTonCoreVM(int id)
        //{
        //    var newVmName = $"UbuntuTon{id}";
        //    var coreVmName = "UbuntuTonCore";
        //    AddNewVM(coreVmName, newVmName);
        //    return true;
        //}

        //public bool AddNewUbuntuCoreVM(int id)
        //{
        //    var newVmName = $"Ubuntu{id}";
        //    var coreVmName = "UbuntuCore";
        //    AddNewVM(coreVmName, newVmName);
        //    return true;
        //}

        public bool AddNewForwardSSHRule(PowerShellConnMdl conn, string ipAddress, int exrnalPort)
        {
            var cmd = new List<string>() {
            $"Add-NetNatStaticMapping -ExternalIPAddress '0.0.0.0/24' -ExternalPort {exrnalPort} -Protocol TCP -InternalIPAddress '{ipAddress}' -InternalPort 22 -NatName NATNetwork",
            };
            var r = ExecuteCommamds(conn,cmd);
            return true;
        }

    }
}


//New-VM -Name Win10VM -MemoryStartupBytes 4GB -BootDevice VHD -VHDPath .\VMs\Win10.vhdx -Path .\VMData -Generation 2 -Switch ExternalSwitch
//