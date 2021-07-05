using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Listener;
using System.Threading;
using System.Threading.Tasks;

namespace TONBRAINS.TONOPS.WebApp.Quartz
{
    public class QuartzJobListener : JobListenerSupport
    {
        private readonly ILogger<QuartzJobListener> logger;

        public QuartzJobListener(ILogger<QuartzJobListener> logger)
        {
            this.logger = logger;
        }

        public override string Name => "Sample Job Listener";

        public override Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("The job is about to be executed, prepare yourself!");
            return Task.CompletedTask;
        }
    }
}
