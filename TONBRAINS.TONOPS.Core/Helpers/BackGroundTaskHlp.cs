using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.Handlers;
using TONBRAINS.TONOPS.Core.QuartzJobs;

namespace TONBRAINS.TONOPS.Core.Helpers
{
    public class BackGroundTaskHlp
    {

        private IScheduler _scheduler { get; set; }
        public BackGroundTaskHlp()
        {

        }
        public BackGroundTaskHlp(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public bool RunTasksInBackgorund(IEnumerable<Task> tasks, int delay_milliseconds = 0)
        {
            if (_scheduler != null)
            {
                new QuartzSvc().ScheduleFutureJob(tasks, _scheduler, delay_milliseconds);
            }
            else
            {
                foreach (var task in tasks)
                {
                    if (delay_milliseconds == 0)
                    {
                        task.Start();
                    }
                    else
                    {
                        Task.Run(async () => {
                            await Task.Delay(delay_milliseconds);
                            task.Start();
                        });
                    }
                }
            }
            return true;
        }

        public bool RunTasksInBackgorund(Task task, int delay_milliseconds = 0)
        {
            return RunTasksInBackgorund(new Task[] { task },delay_milliseconds);
        }
    }
}
