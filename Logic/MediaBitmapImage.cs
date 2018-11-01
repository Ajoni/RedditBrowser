using System.Drawing;
using System.Windows.Media.Imaging;

namespace Logic
{
    public class MediaBitmapImage : Media
    {
        public BitmapImage Image { get; }

        public MediaBitmapImage(BitmapImage image)
        {
            this.Image = image;
        }

        public override void Save(string path)
        {

            BitmapEncoder encoder;
            string format;

            format = GetFormat(path);

            switch (format)
            {
                case ".png":
                    encoder = new PngBitmapEncoder();
                    break;
                case ".jpg":
                    encoder = new JpegBitmapEncoder();
                    break;
                default:
                    encoder = new PngBitmapEncoder();
                    path += ".png";
                    break;
            }
            encoder.Frames.Add(BitmapFrame.Create(Image));

            // TODO: Find what happens when path already exists
            using (var fileStream = new System.IO.FileStream(path, System.IO.FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }

        // Will return format or empty string or gibberish
        private static string GetFormat(string path)
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
