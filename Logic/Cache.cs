using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    class Cache
    {
        private static Cache _instance = new Cache();
        private List<Media> medias { get; set; }
        private Cache() { }
        static public Cache Instance { get
            {
                return _instance;
            }
        }
    }
}
