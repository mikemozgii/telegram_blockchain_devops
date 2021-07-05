using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Listener;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TONBRAINS.TONOPS.WebApp.Quartz
{
    public class QuartzTriggerListener : TriggerListenerSupport
    {
        private readonly ILogger<QuartzTriggerListener> logger;

        public QuartzTriggerListener(ILogger<QuartzTriggerListener> logger)
        {
            this.logger = logger;
        }

        public override string Name => "Sample Trigger Listener";

        public override Task TriggerMisfired(ITrigger trigger, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Observed trigger fire by trigger {TriggerKey}", trigger.Key);
            return Task.CompletedTask;
        }
    }
}
