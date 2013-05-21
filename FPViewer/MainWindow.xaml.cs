using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FPViewer
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool IsActivated = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void DrawBitmap(Image image, int[] data, int width, int height)
        {
            // Define parameters used to create the BitmapSource.
            PixelFormat pf = PixelFormats.Bgra32;
            int rawStride = (width * pf.BitsPerPixel) / 8;

            // Create a BitmapSource.
            BitmapSource bitmap = BitmapSource.Create(width, height,
                96, 96, pf, null,
                data, rawStride);

            // Set image source.
            image.Width = width * 2;
            image.Height = height * 2;
            image.Source = bitmap;
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            if (!IsActivated)
            {
                IsActivated = true;

                RomReader item = new RomReader(RomReader.SpriteType.Item);
                DrawBitmap(image1_1, item.GetSpriteData(), item.Width, item.Height);
                RomReader shiren = new RomReader(RomReader.SpriteType.Shiren);
                DrawBitmap(image1_2, shiren.GetSpriteData(), shiren.Width, shiren.Height);
            }
        }
    }
}
