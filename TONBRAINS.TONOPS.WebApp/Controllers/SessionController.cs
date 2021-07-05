using TONBRAINS.TONOPS.WebApp.Models.Sessions;
using Microsoft.AspNetCore.Mvc;

namespace TONBRAINS.TONOPS.WebApp.Controllers
{
    public class SessionController : Controller
    {

        protected AccountSession Session {
            get
            {
                return HttpContext.Items["PageSession"] as AccountSession;
            }
        }

    }
}
