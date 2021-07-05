using System.Collections.Generic;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.WebApp.Models.AccountModels;
using Microsoft.AspNetCore.Mvc;
using static TONBRAINS.TONOPS.Core.Handlers.KataHelpers;
using Newtonsoft.Json;
using SqlKata;
using System.Linq;
using TONBRAINS.TONOPS.WebApp.Services;
using System;
using TONBRAINS.TONOPS.WebApp.Common.Models;
using TONBRAINS.TONOPS.WebApp.Models;
using TONBRAINS.TONOPS.WebApp.WebApp.Helpers;
using TONBRAINS.TONOPS.Core.DAL;
using Microsoft.EntityFrameworkCore;

namespace TONBRAINS.TONOPS.WebApp.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : SessionController
    {
        private readonly IPasswordHashService m_PasswordHashService;
        private readonly TonOpsDbContext _context;

        public AccountController(TonOpsDbContext context, IPasswordHashService passwordHashService)
        {
            m_PasswordHashService = passwordHashService ?? throw new ArgumentNullException(nameof(passwordHashService));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        [HttpGet("menu")]
        public async Task<IEnumerable<SecurityMenuModel>> Menu()
        {
            var securities = await _context.Securities.Where(i => i.IsActive).OrderBy(q=>q.Order).ThenByDescending(o => o.IsGroup).AsNoTracking().ToListAsync();

            var menu = securities.Where(security => security.IsGroup && string.IsNullOrEmpty(security.GroupName))
                .Select(security => new SecurityMenuModel { Id = security.Id, Title = security.Name, GroupId = security.GroupName, Route = security.Route, Icon = security.Icon }).ToList();

            foreach (var menuItem in menu)
            {
                menuItem.Links = securities.Where(i => i.GroupName == menuItem.Id)
                    .OrderBy(o => o.Order)
                     .Select(security => new SecurityMenuModel { Id = security.Id, Title = security.Name, GroupId = security.GroupName, Route = security.Route, Icon = security.Icon }).ToList();
            }
            return menu;
            //GenerateMenu(menu, securities);
            //foreach (var security in securities)
            //{
            //    var model = new SecurityMenuModel { Id = security.Id, Name = security.Name, GroupName = security.GroupName };

            //    if (security.IsGroup && string.IsNullOrEmpty(security.GroupName))
            //    {
            //        model.Items = new List<SecurityMenuModel>();
            //        menu.Add(model);
            //        continue;
            //    }

            //    if (!string.IsNullOrEmpty(security.GroupName))
            //    {
            //        var parent = menu.FirstOrDefault(i => i.Id == security.GroupName);
            //        parent?.Items.Add(model);
            //    }
            //}
            //var rs = await ExecuteQuery<SecurityMenuModel>(new Query("securities").Where("is_active", true));

            //var groups = rs.Where(q => q.isGroup).ToList();
            //var groupsIds = groups.Select(q => q.Id).ToList();
            //var final_rs = rs.Where(q => groupsIds.Contains(q.GroupName)).ToList();
            //final_rs.AddRange(groups);
            //var d = final_rs.OrderByDescending(q=>q.isGroup).ThenBy(q => q.Order);
            //return d;
        }

        //private void GenerateMenu(List<SecurityMenuModel> menu, List<Security> securities)
        //{
        //    foreach (var menuItem in menu)
        //    {
        //        menuItem.Items = securities.Where(i => i.Id == menuItem.Id)
        //             .Select(security => new SecurityMenuModel { Id = security.Id, Name = security.Name, GroupName = security.GroupName }).ToList();
        //    }
        //}


        //[HttpGet]
        //[Route("securities")]
        //public async Task<IEnumerable<string>> AccountSecurity(string accountId)
        //{
        //    var session = Session;
        //    var account = (await ExecuteQuery<Account>(new Query("accounts").Where("id", string.IsNullOrEmpty(accountId) ? session.AccountId : accountId))).FirstOrDefault();
        //    if (account == null) return Enumerable.Empty<string>();

        //    if (string.IsNullOrEmpty(account.Security))
        //        return Enumerable.Empty<string>();

        //    var rs = await ExecuteQuery<SecurityMenuModel>(new Query("securities").Where("is_active", true));
        //    var groups = rs.Where(q => q.isGroup).ToList();
        //    var groupsIds = groups.Select(q => q.Id).ToList();
        //    var active_securities = rs.Where(q => groupsIds.Contains(q.GroupName)).Select(q => q.Id).ToList();
        //    active_securities.AddRange(groups.Select(q => q.Id));

        //    var account_securities = JsonConvert.DeserializeObject<IEnumerable<string>>(account.Security);
        //    account_securities = account_securities.Where(q => active_securities.Contains(q)).ToList();


        //    return account_securities;
        //}

        //[HttpGet]
        //[Route("securitiesmenu")]
        //public async Task<IEnumerable<SecurityMenuModel>> AccountSecurityMenu(string accountId)
        //{
        //    var session = Session;
        //    var account = (await ExecuteQuery<Account>(new Query("accounts").Where("id", string.IsNullOrEmpty(accountId) ? session.AccountId : accountId))).FirstOrDefault();
        //    if (account == null) return Enumerable.Empty<SecurityMenuModel>();

        //    if (string.IsNullOrEmpty(account.Security))
        //        return Enumerable.Empty<SecurityMenuModel>();

        //    //var account_securities = JsonConvert.DeserializeObject<IEnumerable<string>>(account.Security);

        //    //if (!account_securities.Any())
        //    //    return Enumerable.Empty<SecurityMenuModel>();


        //    //var rs = await ExecuteQuery<SecurityMenuModel>(new Query("securities").Where("is_active", true));
        //    //rs = rs.Where(q=> account_securities.Contains(q.Id));
        //    //var groups = rs.Where(q => q.isGroup).ToList();
        //    //var groupsIds = groups.Select(q => q.Id).ToList();
        //    //var active_securities = rs.Where(q => groupsIds.Contains(q.GroupName)).ToList();
        //    //active_securities.AddRange(groups);


        //    return active_securities.OrderBy(s => s.Order == null ? 1 : 0).ThenBy(q => q.Order);
        //}

        //[HttpPost]
        //[Route("setsecurities")]
        //public async Task<bool> SetAccountSecurity([FromBody] SetAccountSecurityModel model)
        //{
        //    var account = (await ExecuteQuery<Account>(new Query("accounts").Where("id", model.Id))).FirstOrDefault();
        //    if (account == null) return false;

        //    account.Security = JsonConvert.SerializeObject(model.Securities.ToList());

        //    await ExecuteInsertOrUpdate(account, new Query("accounts"), false);

        //    return true;
        //}

        [Route("grid")]
        public async Task<IEnumerable<AccountModel>> Environments()
        {
            return (await ExecuteQuery<AccountModel>(new Query("accounts")))
                .Select(
                    a =>
                    {
                        a.Password = "";
                        return a;
                    }
                )
                .ToList();
        }

        [Route("single")]
        public async Task<AccountModel> Environment(string id)
        {
            var accounts = await ExecuteQuery<AccountModel>(new Query("accounts").Where("id", id));

            return accounts.First();
        }

        [Route("addoredit")]
        [HttpPost]
        public async Task<AccountModel> AddOrEdit([FromBody] AccountModel item)
        {
            var insert = string.IsNullOrEmpty(item.Id);
            if (insert) item.Id = IdGenerator.Generate();
            if (!string.IsNullOrEmpty(item.Password)) item.Password = m_PasswordHashService.Hash(item.Password, EnvironmentService.PasswordSalt);
            var savedItem = await ExecuteInsertOrUpdate(item, new Query("accounts"), insert);

            return savedItem;
        }

        [Route("delete")]
        [HttpDelete]
        public async Task<bool> Delete(string id)
        {
            var result = false;

            try
            {
                await ExecuteQuery(new Query("accounts").Where("id", id).AsDelete());
                return true;
            }
            catch
            {
            }

            return result;
        }

    }

}
