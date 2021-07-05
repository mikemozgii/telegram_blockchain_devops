using System;
using System.Linq;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.WebApp.Middleware;
using TONBRAINS.TONOPS.WebApp.Models.Sessions;
using TONBRAINS.TONOPS.WebApp.Models;
using TONBRAINS.TONOPS.WebApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlKata;
using static TONBRAINS.TONOPS.Core.Handlers.KataHelpers;
using TONBRAINS.TONOPS.Core.DAL;
using Microsoft.EntityFrameworkCore;

namespace TONBRAINS.TONOPS.WebApp.webapp.Controllers
{
    [Route("api/authorization")]
    public class AuthorizationController : ControllerBase
    {
        private readonly IPasswordHashService m_PasswordHashService;
        private readonly TonOpsDbContext _context;

        public AuthorizationController(TonOpsDbContext context, IPasswordHashService passwordHashService)
        {
            m_PasswordHashService = passwordHashService;
            _context = context;
        }

     

        [HttpGet]
        [Route("signin")]
        public async Task<object> Authorization(string name, string password)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(q=>q.Name == name);

                if (account == null || !m_PasswordHashService.IsValid(password, account.Password, EnvironmentService.PasswordSalt))
                {
                    return new { IsAuthentificated = false };
                }

            var token = Guid.NewGuid().ToString();

            var tokenEntity = new Token()
            {
                Id = token.ToString(),
                UserId = account.Id,
                Logined = DateTime.UtcNow
            };

           await _context.Tokens.AddAsync(tokenEntity);
           await _context.SaveChangesAsync();

            UserSessionMiddleware.AddToken(
                token,
                new AccountSession
                {
                    Logined = DateTime.UtcNow,
                    AccountId = account.Id,
                    Token = token
                }
            );

            HttpContext.Response.Cookies.Append(
                "ltoken",
                token,
                new CookieOptions
                {
                    HttpOnly = true,
                    Path = "/",
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.Now.AddDays(7)
                }
            );

            return new { IsAuthentificated = true, Token = token, UserId = account.Id };
        }

    }
}
