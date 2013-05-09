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
        private byte[] leaf;
        private byte[] pallete;
        public MainWindow()
        {
            InitializeComponent();

            
            // 草データを読み込み
            System.IO.FileStream strm = System.IO.File.Open("rom.smc", System.IO.FileMode.Open, System.IO.FileAccess.Read);
            strm.Seek(0x3DD5E9, System.IO.SeekOrigin.Begin);
            //leaf = new byte[128]; // 32*4
            leaf = new byte[32]; // 32*4
            strm.Read(leaf, 0, leaf.Length);

            // 黒色の2バイトはROMに含まれない
            pallete = new byte[30];
            strm.Seek(0x3BC5B9, System.IO.SeekOrigin.Begin); 
            strm.Read(pallete, 0, pallete.Length);

            ConvertRom4BPPToBitmap(leaf, pallete);
        }


        private byte[] ConvertRom4BPPToBitmap(byte[] buf, byte[] romPallete)
        {
            byte[] pallete = new byte[32];
            pallete[0] = 0;
            pallete[1] = 0;
            System.Array.Copy(romPallete, 0, pallete, 2, romPallete.Length);

            byte[,] bmp = new byte[8, 8];
            for (int k = 0; k < 2; ++k)
            {
                for (int i = 0; i < 16; i+= 2)
                {
                    BitPlane bp0 = new BitPlane(buf[i + k * 16]);
                    BitPlane bp1 = new BitPlane(buf[i + 1 + k * 16]);

                    for (int j = 0; j < 8; ++j)
                    {
                        bmp[i / 2, j] += (byte)(bp0.At(7 - j) << k * 2);
                        bmp[i / 2, j] += (byte)(bp1.At(7 - j) << 1 + k * 2);
                    }
                }
            }
            byte[] returnArray = new byte[8 * 8];

            // 変換する
            return returnArray;
        }


        private void Window_Activated_1(object sender, EventArgs e)
        {
            // Define parameters used to create the BitmapSource.
            PixelFormat pf = PixelFormats.Bgr555;
            int width = 16;
            int height = 16;
            int rawStride = (width * pf.BitsPerPixel) / 8;
            ushort[] rawImage = new ushort[width * height];

            // Initialize the image with data.
            for (int i = 0; i < width * height; ++i)
            {
                // BGR(5bitずつ)
                rawImage[i] =  (ushort)(0x1f << 11 | 0x00 << 6 | 0x00 << 1);
            }

            // Create a BitmapSource.
            BitmapSource bitmap = BitmapSource.Create(width, height,
                96, 96, pf, null,
                rawImage, rawStride);

            // Set image source.
            image1.Width = 400;
            image1.Height = 400;
            image1.Source = bitmap;

        }
    }
}
