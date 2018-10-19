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
        private int _maxSize;
        private int index = -1;
        public List<Media> medias { get; set; }
        private Cache(int size = 10 ) { _maxSize = size; }
        static public Cache Instance { get
            {
                return _instance;
            }
        }

        public Media GetCurrent() { return  medias[index]; }
        public bool hasPrevious() { return index > 0; }
        public bool hasNext() { return index < _maxSize  && index < medias.Capacity - 1; }
        public void Next()
        {
            if(index<_maxSize)
            index++;
        }
        public void Previous()
        {
            if(index>0)
            index--;
        }
        public void addElem(Media media) {
            if(index >= _maxSize) { 
                medias.RemoveAt(0);
            }
            medias.Add(media);
        }
        public void invalidateCache() { medias.Clear(); index = -1; }
    }
}
