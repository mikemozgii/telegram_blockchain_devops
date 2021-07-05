using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.Handlers;

namespace TONBRAINS.TONOPS.Core.QuartzJobs
{
    public class QuartzSvc
    {
        public bool ScheduleFutureJob(IEnumerable<Task> tasks, IScheduler scheduler, int milliseconds = 1000)
        {
            IJobDetail job = JobBuilder.Create<BasicQJ>().WithIdentity(Guid.NewGuid().ToString()).Build();
            job.JobDataMap.Add("tasks", tasks);
            ITrigger trigger;
            if (milliseconds == 0)
            {
                trigger = TriggerBuilder.Create().WithIdentity(Guid.NewGuid().ToString()).StartNow().Build();        
            }
            else
            {
                trigger = TriggerBuilder.Create().WithIdentity(Guid.NewGuid().ToString()).StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddMilliseconds(milliseconds))).Build();
               
            }
            scheduler.ScheduleJob(job, trigger).GetAwaiter().GetResult();
            return true;
        }


        public bool ScheduleFutureJob(Task task, IScheduler scheduler, int milliseconds = 1000)
        {
            return ScheduleFutureJob(new List<Task> { task }, scheduler, milliseconds);
        }

        //public bool ScheduleFutureJobTonNet(Task task, IScheduler scheduler)
        //{
        //    return ScheduleFutureJobTonNet(new List<Task> { task }, scheduler);
        //}

        //public bool ScheduleFutureJobTonNet(IEnumerable<Task> tasks, IScheduler scheduler)
        //{
        //    return ScheduleFutureJob(tasks, scheduler, GlobalAppConfHandler.TonNetworkTrasferWaitTime);
        //}
    }
}
