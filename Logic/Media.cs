using System;

namespace Logic
{
    public abstract class Media : ISaveable
    {
        public abstract void Save(string path);
    }
}
