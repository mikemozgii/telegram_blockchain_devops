using Renci.SshNet;
using System.Collections.Concurrent;

namespace TONBRAINS.TONOPS.WebApp.Helpers
{
    public static class TunnelsData
    {
        public static readonly ConcurrentDictionary<string, SshClient> ActiveTunnels = new ConcurrentDictionary<string, SshClient>();
    }
}
