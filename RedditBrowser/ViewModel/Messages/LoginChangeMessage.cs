using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditBrowser.ViewModel.Messages
{
	public class LoginChangeMessage
	{
		public AuthenticatedUser User { get; set; }

		public LoginChangeMessage(AuthenticatedUser user)
		{
			User = user;
		}
	}
}
