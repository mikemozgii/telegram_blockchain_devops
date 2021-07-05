using Microsoft.Extensions.Logging;
using Quartz.Listener;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TONBRAINS.TONOPS.WebApp.Quartz
{
    public class QuartzSchedulerListener : SchedulerListenerSupport
    {
        private readonly ILogger<QuartzSchedulerListener> logger;

        public QuartzSchedulerListener(ILogger<QuartzSchedulerListener> logger)
        {
            this.logger = logger;
        }

        public override Task SchedulerStarted(CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Observed scheduler start");
            return Task.CompletedTask;
        }
    }
}
