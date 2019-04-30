using RedditSharp;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditBrowser.Classes
{
	public class UserLoginResult
	{
		public WebAgent WebAgent { get; set; }
		public AuthenticatedUser AuthenticatedUser { get; set; }

		public UserLoginResult(WebAgent webAgent, AuthenticatedUser  authenticatedUser)
		{
			WebAgent = webAgent;
			AuthenticatedUser = authenticatedUser;
		}
	}
}
