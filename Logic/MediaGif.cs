﻿using System.Drawing;

namespace Logic
{
    public class MediaGIF : Media
    {
        public Image image;

        public MediaGIF(Image image)
        {
            this.image = image;
        }

        public override void Save(string path)
        {
            string format = GetFormat(path);

            if (format != ".gif")
            {
                path += ".gif";
            }

            image.Save(path);
        }
    }
}
