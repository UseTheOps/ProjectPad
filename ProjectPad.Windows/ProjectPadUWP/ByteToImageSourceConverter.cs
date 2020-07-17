using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace ProjectPadUWP
{
    public class ByteToImageSourceConverter : IValueConverter
    {
        public static ImageSource ByteToImage(byte[] imageData)
        {
            BitmapImage biImg = new BitmapImage();

            InMemoryRandomAccessStream ms = new InMemoryRandomAccessStream();
            ms.AsStreamForWrite().Write(imageData, 0, imageData.Length);
            ms.Seek(0);

            biImg.SetSource(ms);

            ImageSource imgSrc = biImg as ImageSource;

            return imgSrc;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is byte[])
                return ByteToImage(value as byte[]);
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
