using RedditBrowser.Config;
using RedditSharp;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditBrowser.Helpers
{
	public static class LoginHelper
	{
		public static WebAgent LoginApp()
		{
			string keyPath = GlobalConfig.Get<string>(GlobalKeys.KeyConfigPath);

			string[] keys = System.IO.File.ReadAllText(keyPath).Split(',');
			AuthProvider auth = new AuthProvider(keys[0], keys[1], keys[2]);
			var accessToken = auth.GetOAuthToken(keys[3], keys[4]);
			var agent = new WebAgent() { AccessToken = accessToken };
			WebAgent.RootDomain = "oauth.reddit.com";
			return agent;
		}

		public static async Task<AuthenticatedUser> LoginUser(string username, string password)
		{
			string keyPath = GlobalConfig.Get<string>(GlobalKeys.KeyConfigPath);

			string[] keys = System.IO.File.ReadAllText(keyPath).Split(',');
			AuthProvider auth = new AuthProvider(keys[0], keys[1], keys[2]);
			var accesToken = await Task.Run(() => auth.GetOAuthToken(keys[3], keys[4]));
			return await Task.Run(() => auth.GetUser(accesToken));
		}
	}
}
