using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPViewer
{
    class SpriteImage
    {
        public enum ImageType { Item, Shiren, Character }
        private int[,] rgbBitmap;

        public SpriteImage(int width, int height, byte[] romBgData, byte[] romPallete)
        {
            Snes4BppBitmap[,] snes4BppBitmapArray = new Snes4BppBitmap[height / Snes4BppBitmap.Height, width / Snes4BppBitmap.Width];
            SetupBitmapArray(romBgData, snes4BppBitmapArray);

            SnesPallete pallete = new SnesPallete(romPallete);

            Create(width, height, snes4BppBitmapArray, pallete);
        }

        public int[] GetRawImage()
        {
            int[] rawData = new int[rgbBitmap.Length];
            int i = 0;
            foreach (var bitmap in rgbBitmap)
            {
                rawData[i++] = bitmap;
            }

            return rawData;
        }

        private void SetRgbBitmap(int width, int height, Snes4BppBitmap bitmap, SnesPallete pallete, int rowOffset, int columnOffset)
        {
            int index = 0;
            foreach (var i in bitmap)
            {
                int row = rowOffset * Snes4BppBitmap.Width + index / Snes4BppBitmap.Width;
                int column = columnOffset * Snes4BppBitmap.Height + index % Snes4BppBitmap.Height;

                rgbBitmap[row, column] = pallete.data[i];
                index++;
            }
        }

        private void Create(int width, int height, Snes4BppBitmap[,] snes4BppBitmapArray, SnesPallete pallete)
        {
            rgbBitmap = new int[height, width];

            int index = 0;
            foreach (var i in snes4BppBitmapArray)
            {
                SetRgbBitmap(width, height, i, pallete, index / (width / Snes4BppBitmap.Width), index % (width / Snes4BppBitmap.Width));
                index++;
            }
        }

        private void SetupBitmapArray(byte[] romBgData, Snes4BppBitmap[,] snes4BppBitmapArray)
        {
            System.Diagnostics.Debug.Assert(snes4BppBitmapArray.Rank == 2);

            for (int i = 0; i < snes4BppBitmapArray.GetLength(0); ++i)
            {
                for (int j = 0; j < snes4BppBitmapArray.GetLength(1); ++j)
                {
                    // [0,0] [0,1] [1,0] [1,1]
                    //  0,   32,    64,   96
                    // [0,0] [0,1] [0,2] [0,3] [1,0] [1,1] [1,2] [1,3]...
                    //  0,   32,    64,   96    128   160   192   224
                    // [0,0] [0,1] [0,2] [1,0] [1,1] [1,2] [2,0] [2,2]...
                    //  0,   32,    64,   96    128   160   192   224
                    byte[] subArray = new byte[Snes4BppBitmap.Size];
                    Array.Copy(romBgData, (i * snes4BppBitmapArray.GetLength(1) + j) * Snes4BppBitmap.Size, subArray, 0, Snes4BppBitmap.Size);
                    snes4BppBitmapArray[i, j] = new Snes4BppBitmap(subArray);
                }
            }
        }
    }
}
