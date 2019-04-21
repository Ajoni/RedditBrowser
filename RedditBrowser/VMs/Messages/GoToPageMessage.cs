using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditBrowser.VMs.Messages
{
    public class GoToPageMessage
    {
								public IViewModel Page { get; set; }

								public GoToPageMessage(IViewModel page)
								{
												this.Page = page;
								}
				}
}
