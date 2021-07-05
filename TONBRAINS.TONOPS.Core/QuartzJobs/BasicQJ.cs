using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TONBRAINS.TONOPS.Core.QuartzJobs
{
    public class BasicQJ : IJob//, IDisposable
    {
        private readonly ILogger<BasicQJ> logger;
        public BasicQJ(ILogger<BasicQJ> logger)
        {
            this.logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            logger.LogInformation(context.JobDetail.Key + " job executing, triggered by " + context.Trigger.Key);
            var tasks =  (IEnumerable<Task>)context.JobDetail.JobDataMap.Get("tasks");
            foreach (var t in tasks)
            {
                t.Start();
            }

            Task.WaitAll(tasks.ToArray());
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
}
