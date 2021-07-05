using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TONBRAINS.TONOPS.Core.DAL;
using TONBRAINS.TONOPS.Core.Handlers;

namespace TONBRAINS.TONOPS.Core.SSH
{
    public class SSHAuthMdl
    {

        public SSHAuthMdl() { }

        public SSHAuthMdl(string _host, int _port, string _user, string _pass) { Host = _host; Id = _host; Port = _port; User = _user; Password = _pass; }
        public SSHAuthMdl(string _host, string _user, string _pass) { Host = _host; Id = _host; Port = 22; User = _user; Password = _pass; }
        public SSHAuthMdl(string _host, string _pass) { Host = _host; Id = _host; Port = 22; User = "root"; Password = _pass; }
        public SSHAuthMdl(string _host) { Host = _host; Id = _host; Port = 22; User = "root"; Password = "admin32"; }

        public SSHAuthMdl(string _nodeId, bool useNode) {

            if (!useNode) return;
            using (var _context = new TonOpsDbContext(GlobalAppConfHandler.GetTonOpsDbContextOption()))
            {

                var node = _context.Nodes.FirstOrDefault(q=> q.Id == _nodeId);
                if (node == null) return;

                var creds = _context.Credentials.FirstOrDefault(q => q.Id == node.CredentialId);
                if (creds == null) return;
                Id = node.Id;
                Host = node.SshIp;
                Port = node.SshPort;
                User = creds.UserName;
                Password = creds.Password;
            }
             
        }

        public SSHAuthMdl(Node node)
        {
            using (var _context = new TonOpsDbContext(GlobalAppConfHandler.GetTonOpsDbContextOption()))
            {
                var creds = _context.Credentials.FirstOrDefault(q => q.Id == node.CredentialId);
                if (creds == null) return;
                Id = node.Id;
                Host = node.SshIp;
                Port = node.SshPort;
                User = creds.UserName;
                Password = creds.Password;
            }

        }

        public string Id
        {
            get;
            set;
        }


        public string Host
        {
            get;
            set;
        }

        public int Port
        {
            get;
            set;
        }
        public string User
        {
            get;
            set;
        }
        public string Password
        {
            get;
            set;
        }
    }
}