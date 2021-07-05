using TONBRAINS.TONOPS.WebApp.Models.Sessions;
using TONBRAINS.TONOPS.WebApp.Models;
using Microsoft.AspNetCore.Http;
using SqlKata;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static TONBRAINS.TONOPS.Core.Handlers.KataHelpers;

namespace TONBRAINS.TONOPS.WebApp.Middleware
{
    public class UserSessionMiddleware
    {

        private readonly RequestDelegate m_RequestDelegate;

        private static Dictionary<string, AccountSession> m_Tokens;

        public static void AddToken(string token, AccountSession accountSession)
        {
            if (m_Tokens.ContainsKey(token))
            {
                m_Tokens[token] = accountSession;
            }
            else
            {
                m_Tokens.Add(token, accountSession);
            }
        }

        public static async Task FillTokens()
        {
            if (m_Tokens != null) return;

            m_Tokens = new Dictionary<string, AccountSession>();

            var tokens = await ExecuteQuery<TokenModel>(new Query("tokens"));
            foreach (var token in tokens)
            {
                m_Tokens.Add(
                    token.Id, new AccountSession
                    {
                        AccountId = token.UserId,
                        Logined = token.Logined,
                        Token = token.Id
                    }
                );
            }
        }

        public UserSessionMiddleware(RequestDelegate requestDelegate)
        {
            m_RequestDelegate = requestDelegate ?? throw new ArgumentNullException(nameof(requestDelegate));
        }

        private static List<string> m_PathsModules = new List<string> {
            "/signin",
            "/component/signin/",
            "/api/authorization/signin",
            "/api/external/getservices",
            "/api/external/getenvironmentbyorganizationname",
            "/api/external/setorganizationname",
            "/api/external/checkenvironmentdomain",
            "/api/external/checkorganizationname",
        };

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.HasValue ? context.Request.Path.Value.ToLowerInvariant() : "";

            var isSessionPath = !m_PathsModules.Contains(path);

            if (!isSessionPath)
            {
                await m_RequestDelegate(context);
                return;
            }

            var token = context.Request.Cookies.ContainsKey("ltoken") ? context.Request.Cookies["ltoken"] : null;

            var sessionChecking = string.IsNullOrEmpty(token) ? false : m_Tokens.ContainsKey(token);

            if (!sessionChecking)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentLength = 0;
                context.Response.Redirect("/signin", permanent: true);
                return;
            }

            context.Items["PageSession"] = m_Tokens[token];

            await m_RequestDelegate(context);

            context.Items["PageSession"] = null;
        }

    }

}
