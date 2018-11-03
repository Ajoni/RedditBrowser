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
            
            using (var fileStream = new System.IO.FileStream(path, System.IO.FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }
    }

}
