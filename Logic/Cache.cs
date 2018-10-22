using System.Collections.Generic;

namespace Logic
{
    class Cache
    {
        private static Cache _instance = new Cache(5);
        private int _maxSize;
        private int index = -1;
        public List<Media> medias { get; set; } = new List<Media>();
        private Cache(int size = 10 ) { _maxSize = size; }
        static public Cache Instance { get
            {
                return _instance;
            }
        }

        public Media GetCurrent() { return  medias[index]; }
        public bool hasPrevious() { return index > 0; }
        public bool hasNext() { return index < medias.Count - 1; }
        public void Next()
        {
            if(hasNext())
            index++;
        }
        public void Previous()
        {
            if(hasPrevious())
            index--;
        }
        public void addElem(Media media) {
            if(medias.Count == _maxSize) { 
                medias.RemoveAt(0);
            }
            medias.Add(media);
        }
        public void invalidateCache() { medias.Clear(); index = -1; }
    }
}
