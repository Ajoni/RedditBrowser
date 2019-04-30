using RedditBrowser.Config;
using RedditSharp;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RedditBrowser.Helpers
{
	public static class LoginHelper
	{
		public static async Task<Classes.UserLoginResult> LoginUser(string username, string password)
		{
			string keyPath = GlobalConfig.Get<string>(GlobalKeys.KeyConfigPath);

			string[] keys = System.IO.File.ReadAllText(keyPath).Split(',');
			AuthProvider auth = new AuthProvider(keys[0], keys[1], keys[2]);

			var accessToken = await Task.Run(() => {
				try { return auth.GetOAuthToken(username, password); }
				catch (WebException) { return null; }
			});

			if (accessToken == null)
				return null;

			var agent = new WebAgent() { AccessToken = accessToken };
			WebAgent.RootDomain = "oauth.reddit.com";

			var user = await Task.Run(() => {
				try { return auth.GetUser(accessToken); }
				catch (WebException) { return null; }
			});
			return new Classes.UserLoginResult(agent, user);
		}
	}
}
