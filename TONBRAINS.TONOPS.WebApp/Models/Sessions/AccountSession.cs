using System;

namespace TONBRAINS.TONOPS.WebApp.Models.Sessions
{
    public class AccountSession
    {
        public string AccountId { get; set; }
        public string Token { get; set; }
        public DateTime Logined { get; set; }
    }
}
