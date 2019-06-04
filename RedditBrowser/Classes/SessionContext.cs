using RedditSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditBrowser.Classes
{
    public static class SessionContext
    {
        public static Reddit Reddit { get; set; } = new Reddit();
        public static bool IsUserLoggedIn { get => Reddit.User != null; }

        internal static void Update(UserLoginResult userLoginResult)
        {
            if (userLoginResult != null)
                Reddit = new Reddit(userLoginResult.WebAgent, true);
            else
                Reddit = new Reddit();
        }
    }
}
