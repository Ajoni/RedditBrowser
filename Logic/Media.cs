using System;

namespace Logic
{
    public abstract class Media : ISaveable
    {
        public abstract void Save(string path);

        // Will return format or empty string or gibberish
        protected string GetFormat(string path)
        {
            string format;
            int lastDotIndex = path.LastIndexOf('.');
            if (lastDotIndex != -1)
            {
                format = path.Substring(lastDotIndex);
            }
            else
            {
                format = string.Empty;
            }

            return format;
        }
    }
}
