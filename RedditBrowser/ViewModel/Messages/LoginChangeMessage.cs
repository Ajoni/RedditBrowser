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
		public Classes.UserLoginResult UserLoginResult { get; set; }

		public LoginChangeMessage(Classes.UserLoginResult userLoginResult)
		{
			UserLoginResult = userLoginResult;
		}
	}
}
