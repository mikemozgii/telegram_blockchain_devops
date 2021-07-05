using TONBRAINS.TONOPS.WebApp.Common.Models;
using TONBRAINS.TONOPS.WebApp.Common.Models.ConfigurationTemplate;
using Renci.SshNet;
using System;
using System.IO;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.DAL;

namespace TONBRAINS.TONOPS.WebApp.Services
{
    public interface ISshStream
    {
        ConnectionInfo GetConnectionInfo(ServerTemplate options);
        Task UploadFileToHost(ConnectionInfo info, Stream input, string pathToSave);

        Task ExecuteBashOnHost(SshClient client, Stream bash, string pathToSave = "/root/", string parameters = "");

        Task ExecuteCommands(SshClient client, string[] commands, string hostId);

        SshClient GetSshClient(ServerTemplate options);

        SshClient GetSshClient(ConnectionInfo info);

        Task DeployRoot(SshClient client, Common.Models.Credential initEntry, Common.Models.Credential root);
        Task WriteHistory(string host, string message);
        Task CheckServer(v_Node_Shh credential, string groupId);
    }
}
