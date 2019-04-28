using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditBrowser.Config
{
    public static class GlobalConfig
    {
        private static Dictionary<string, object> config;

        public static void Set(Dictionary<string, object> config)
        {
            GlobalConfig.config = config;
        }

        public static T Get<T>(string key)
        {
            return (T)config[key];
        }
    }
}
