using TONBRAINS.TONOPS.WebApp.WebApp;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TONBRAINS.TONOPS.WebApp.Hubs
{
    public class NodeHub: Hub
    {
        public async Task GetHistory(string host)
        {
            if(!Program.History.ContainsKey(host))
            {
                Program.History.Add(host, new List<string>());
                Program.History[host].Add($"Host: {host}");
            }
            await Clients.All
                .SendAsync($"History", JsonConvert.SerializeObject((Program.History[host])), host);
        }
    }
}
