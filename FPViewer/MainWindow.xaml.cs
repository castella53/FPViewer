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
        private const int ADDR_LEAF    = 0x3DD5E9;
        private const int ADDR_SCROLL  = ADDR_LEAF + 0x80;
        private const int ADDR_RICE    = ADDR_LEAF + 0x80 * 2;
        private const int ADDR_SWORD   = ADDR_LEAF + 0x80 * 3;
        private const int ADDR_ARROW   = ADDR_LEAF + 0x80 * 4;
        private const int ADDR_SHIELD  = ADDR_LEAF + 0x80 * 5;
        private const int ADDR_RING    = ADDR_LEAF + 0x80 * 6;
        private const int ADDR_STAFF   = ADDR_LEAF + 0x80 * 7;
        private const int ADDR_GITAN   = ADDR_LEAF + 0x80 * 8;
        private const int ADDR_MEAT    = ADDR_LEAF + 0x80 * 8;
        private const int ADDR_FEATHER = ADDR_LEAF + 0x80 * 9;
        private const int ADDR_POT     = ADDR_LEAF + 0x80 * 10;
        private const int ADDR_SCROLL2 = ADDR_LEAF + 0x80 * 11;

        public MainWindow()
        {
            InitializeComponent();

            // 草データを読み込み
            System.IO.FileStream strm = System.IO.File.Open("rom.smc", System.IO.FileMode.Open, System.IO.FileAccess.Read);
            strm.Seek(ADDR_LEAF, System.IO.SeekOrigin.Begin);
            //leaf = new byte[128]; // 32*4
            byte[] leaf1 = new byte[32]; // 32*4
            byte[] leaf2 = new byte[32]; // 32*4
            byte[] leaf3 = new byte[32]; // 32*4
            byte[] leaf4 = new byte[32]; // 32*4
            strm.Read(leaf1, 0, leaf1.Length);
            strm.Read(leaf2, 0, leaf2.Length);
            strm.Read(leaf3, 0, leaf3.Length);
            strm.Read(leaf4, 0, leaf4.Length);

            // 黒色の2バイトはROMに含まれない
            byte[] pallete = new byte[32];
            strm.Seek(0x3BC5B9, System.IO.SeekOrigin.Begin); 
            strm.Read(pallete, 2, pallete.Length - 2);

            DrawBitmap(image1_1, leaf1, pallete);
            DrawBitmap(image1_2, leaf2, pallete);
            DrawBitmap(image1_3, leaf3, pallete);
            DrawBitmap(image1_4, leaf4, pallete);
        }

        private void DrawBitmap(Image image, byte[] data, byte[] pallete)
        {
            // Define parameters used to create the BitmapSource.
            PixelFormat pf = PixelFormats.Bgr32;
            int width = 8;
            int height = 8;
            int rawStride = (width * pf.BitsPerPixel) / 8;
            ItemImage item = new ItemImage(data, pallete);
            int[] rawImage = item.Get();

            // Create a BitmapSource.
            BitmapSource bitmap = BitmapSource.Create(width, height,
                96, 96, pf, null,
                rawImage, rawStride);

            // Set image source.
            image.Width = 100;
            image.Height = 100;
            image.Source = bitmap;
        }
    }
}
