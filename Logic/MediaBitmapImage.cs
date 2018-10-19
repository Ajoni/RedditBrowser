using System.Windows.Media.Imaging;

namespace Logic
{
    public class MediaBitmapImage : Media
    {
        public BitmapImage image;

        public MediaBitmapImage(BitmapImage image)
        {
            this.image = image;
        }

    }

}
