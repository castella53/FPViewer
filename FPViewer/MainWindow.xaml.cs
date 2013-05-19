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
        private const int ADDR_SCROLL  = ADDR_LEAF + 0x80 * 1;
        private const int ADDR_RICE    = ADDR_LEAF + 0x80 * 2;
        private const int ADDR_SWORD   = ADDR_LEAF + 0x80 * 3;
        private const int ADDR_ARROW   = ADDR_LEAF + 0x80 * 4;
        private const int ADDR_SHIELD  = ADDR_LEAF + 0x80 * 5;
        private const int ADDR_RING    = ADDR_LEAF + 0x80 * 6;
        private const int ADDR_STAFF   = ADDR_LEAF + 0x80 * 7;
        private const int ADDR_GITAN   = ADDR_LEAF + 0x80 * 8;
        private const int ADDR_MEAT    = ADDR_LEAF + 0x80 * 9;
        private const int ADDR_FEATHER = ADDR_LEAF + 0x80 * 10;
        private const int ADDR_POT     = ADDR_LEAF + 0x80 * 11;
        private const int ADDR_SCROLL2 = ADDR_LEAF + 0x80 * 12;

        private const int ADDR_PALLETE_ITEM = 0x3BC5B9;
        private const int ADDR_PALLETE_SHIREN = 0x3BC5F5;

        private const int ADDR_SHIREN = 0x17F3C2;

        public MainWindow()
        {
            InitializeComponent();

            // 草データを読み込み
            System.IO.FileStream strm = System.IO.File.Open("rom.smc", System.IO.FileMode.Open, System.IO.FileAccess.Read);
            byte[] leaf = new byte[32 * 4]; // 32バイト*4個
            byte[] pallete = new byte[32];
            ReadBitmapAndPallete(strm, ADDR_LEAF, leaf, ADDR_PALLETE_ITEM, pallete);

            byte[] shiren = new byte[32 * 16]; // 32バイト*16個
            byte[] palleteShiren = new byte[32];
            ReadBitmapAndPallete(strm, ADDR_SHIREN, shiren, ADDR_PALLETE_SHIREN, palleteShiren);

            DrawBitmap(image1_1, leaf, 16, 16, pallete);
       }

        private static void ReadBitmapAndPallete(System.IO.FileStream strm, int addrBitmap, byte[] bitmap, int addrPallete, byte[] pallete)
        {
            strm.Seek(addrBitmap, System.IO.SeekOrigin.Begin);
            strm.Read(bitmap, 0, bitmap.Length);

            // 黒色の2バイトはROMに含まれない
            strm.Seek(addrPallete, System.IO.SeekOrigin.Begin);
            strm.Read(pallete, 2, pallete.Length - 2);
        }

        private void DrawBitmap(Image image, byte[] data, int width, int height, byte[] pallete)
        {
            // Define parameters used to create the BitmapSource.
            PixelFormat pf = PixelFormats.Bgra32;
            int rawStride = (width * pf.BitsPerPixel) / 8;
            ItemImage item = new ItemImage(width, height, data, pallete);

            // Create a BitmapSource.
            BitmapSource bitmap = BitmapSource.Create(width, height,
                96, 96, pf, null,
                item.rgbBitmap, rawStride);

            // Set image source.
            image.Width = 100;
            image.Height = 100;
            image.Source = bitmap;
        }
    }
}
