using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.Handlers;
using TONBRAINS.TONOPS.Core.SSH;

namespace TONBRAINS.TONOPS.Core.Services
{
    public class UbuntuServerSvc
    {
        public bool SetupStaticIp(string sshIp, int sshPort, string userName, string password, string newIp, int externPort)
        {

            var dns1 = "8.8.8.8";
            var dns2 = "8.8.4.4";
            var gateway = "192.168.0.1";
            var nodeAuth = new SSHAuthMdl(sshIp, sshPort, userName, password);
            var commonHelprs = new CommonHelprs();
            var bashFile = commonHelprs.GetBashContentFromFile("static_ip");
            bashFile = bashFile.Replace("${ip_address}", newIp).Replace("${dns_ip_address_1}", dns1).Replace("${dns_ip_address_2}", dns2).Replace("${gateway_ip_address}", gateway);
            var bytea = commonHelprs.GenerateByteaFromString(bashFile);
            var bashPAth = new SSHSvc().ExecuteBashOnHostByCommandReturnBashPath(nodeAuth, bytea);

            nodeAuth.Port = externPort;
            new SSHSvc().ExecuteCommands(nodeAuth, $"rm {bashPAth}");
            return true;
        }

        public bool SetupHostName(string sshIp, int sshPort, string userName, string password, string hostName)
        {
            var nodeAuth = new SSHAuthMdl(sshIp, sshPort, userName, password);
            var commonHelprs = new CommonHelprs();
            var bashFile = commonHelprs.GetBashContentFromFile("set_host_name");
            bashFile = bashFile.Replace("${new_host_name}", hostName);
            var bytea = commonHelprs.GenerateByteaFromString(bashFile);
            new SSHSvc().ExecuteBashOnHost(nodeAuth, bytea);
            return true;
        }

        public bool Reboot(string sshIp, int sshPort, string userName, string password, string hostName)
        {
            var nodeAuth = new SSHAuthMdl(sshIp, sshPort, userName, password);
            new SSHSvc().RunCommand(nodeAuth, "reboot");
            return true;
        }


    }
}
