using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using static TONBRAINS.TONOPS.Core.Handlers.KataHelpers;
using SqlKata;
using System.Linq;
using TONBRAINS.TONOPS.WebApp;
using System.IO;
using System;
using TONBRAINS.TONOPS.WebApp.Common.Models;
using TONBRAINS.TONOPS.WebApp.Common.Models.ConfigurationTemplate;
using TONBRAINS.TONOPS.WebApp.Services;
using TONBRAINS.TONOPS.WebApp.WebApp.Helpers;

namespace TONBRAINS.TONOPS.WebApp.Controllers
{
    [Route("api/cronrules")]
    [ApiController]
    public class CronRulesController : ControllerBase
    {
        private readonly ISshStream _sshService;

        public CronRulesController(ISshStream sshService)
            => _sshService = sshService ?? throw new ArgumentNullException(nameof(sshService));

        private Stream GenerateCronFile(IEnumerable<CronRuleModel> rules)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write("SHELL=/bin/sh\n");
            writer.Write("PATH=/usr/local/sbin:/usr/local/bin:/sbin:/bin:/usr/sbin:/usr/bin\n\n");

            foreach (var rule in rules) writer.Write($"{rule.Rule} {rule.Command} #{rule.Name}\n");

            writer.Flush();
            stream.Position = 0;

            return stream;
        }

        private async Task UpdateCronTab()
        {
            //var rules = await ExecuteQuery<CronRuleModel>(new Query("cron_rules"));

            //var sshAuthorization = new ServerTemplate
            //{
            //    Host = "172.17.1.81", //TODO: change to actual!!!
            //    User = "root",
            //    Password = "temp3232",
            //    Port = 22
            //};
            //using var client = _sshService.GetSshClient(sshAuthorization);
            //client.Connect();
            //await _sshService.UploadFileToHost(client.ConnectionInfo, GenerateCronFile(rules), "/root/crontabfile.txt");
            //await _sshService.ExecuteCommands(client, new string[] {
            //        "crontab /root/crontabfile.txt",
            //        "rm /root/crontabfile.txt" 
            //});
            //client.Disconnect();
        }


        [Route("grid")]
        public async Task<IEnumerable<CronRuleModel>> Rules() => await ExecuteQuery<CronRuleModel>(new Query("cron_rules"));

        [Route("single")]
        public async Task<CronRuleModel> Rule(string id)
        {
            var cronRules = await ExecuteQuery<CronRuleModel>(new Query("cron_rules").Where("id", id));

            return cronRules.FirstOrDefault();
        }

        [Route("addoredit")]
        [HttpPost]
        public async Task<CronRuleModel> AddOrEdit([FromBody] CronRuleModel item)
        {
            var insert = string.IsNullOrEmpty(item.Id);
            if (insert) item.Id = IdGenerator.Generate();
            var savedItem = await ExecuteInsertOrUpdate(item, new Query("cron_rules"), insert);

            await UpdateCronTab();

            return savedItem;
        }

        [Route("delete")]
        [HttpDelete]
        public async Task<bool> Delete(string id)
        {
            var result = false;

            try
            {
                await ExecuteQuery(new Query("cron_rules").Where("id", id).AsDelete());
                result = true;
            }
            catch
            {
            }

            return result;
        }


    }
}
