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

    }

}
