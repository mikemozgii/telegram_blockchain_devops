using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.DAL;

namespace TONBRAINS.TONOPS.Core.Handlers
{
    public static class GlobalAppConfHandler
    {
        public static string TonOpsDbConnectionString = $"Host=192.168.1.100;Database=quanet_matrix;Port=2432;Username=postgres;Password=temp3232";

        public static string NewNodeGroupId = $"-MMka2OMVmKmmqJmCyhG";
        public static string TonReadyGroupId = $"-MMkat8l3cXLkVRgFS8i";
        public static string NewNodeCredentialsId = $"init";
        public static string CredentialsInitId = $"init";
        public static string CredentialsDefaultId = $"default";
        public static string UnuntuServer2004LTS = $"Ubuntu 20.04 LTS";
        public static string MainWalletAddress = $"-1:1111111111111111111111111111111111111111111111111111111111111111";
        public static string MainConfigAddress = $"-1:5555555555555555555555555555555555555555555555555555555555555555";
        public static int  TonNetMsgExecWaitTime = 10000;
        public static int BackgroundTasDefaultDelay = 1000;
        public static string MainConfigFakeMainConfigSCId = $"FakeMainConfigSC";

        

        public static DbContextOptions GetTonOpsDbContextOption()
        {
            var builder = new DbContextOptionsBuilder<TonOpsDbContext>().UseNpgsql(TonOpsDbConnectionString).EnableSensitiveDataLogging()
        .EnableDetailedErrors()
        .LogTo(s => System.Diagnostics.Debug.WriteLine(s));
            return builder.Options;
        }
    }
}
