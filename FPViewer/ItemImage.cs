using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPViewer
{
    class ItemImage
    {
        public int[] rgbBitmap { get; private set; }

        public ItemImage(int width, int height, byte[] romBgData, byte[] romPallete)
        {
            int arrayNum = width / Snes4BppBitmap.Width * height / Snes4BppBitmap.Height;
            Snes4BppBitmap[] snes4BppBitmapArray = new Snes4BppBitmap[arrayNum];
            ConcatBitmapArray(romBgData, snes4BppBitmapArray);

            SnesPallete pallete = new SnesPallete(romPallete);

            Create(width, height, snes4BppBitmapArray, pallete);
        }

        private void SetRgbBitmapRaw(int width, int height, Snes4BppBitmap bitmap, SnesPallete pallete, int rowOffset, int columnOffset)
        {
            for (int i = 0; i < Snes4BppBitmap.Width * Snes4BppBitmap.Height; ++i)
            {
                int bitmapRow = (i / Snes4BppBitmap.Width) + rowOffset;
                int bitmapColumn = (i % Snes4BppBitmap.Height) + columnOffset;

                int palletIndex = bitmap.data[i /Snes4BppBitmap.Width, i % Snes4BppBitmap.Height];
                rgbBitmap[bitmapRow * width + bitmapColumn] = pallete.data[palletIndex];
            }
        }

        private void Create(int width, int height, Snes4BppBitmap[] snes4BppBitmapArray, SnesPallete pallete)
        {
            rgbBitmap = new int[width * height];

            // 1 2
            // 3 4
            for (int i = 0; i < snes4BppBitmapArray.Length; ++i)
            {
                int rowOffset = Snes4BppBitmap.Height * (i / 2);
                int columnOffset = Snes4BppBitmap.Width * (i % 2);
                SetRgbBitmapRaw(width, height, snes4BppBitmapArray[i], pallete, rowOffset, columnOffset);
            }
        }

        private void ConcatBitmapArray(byte[] romBgData, Snes4BppBitmap[] snes4BppBitmapArray)
        {
            byte[][] bmpArray = new byte[snes4BppBitmapArray.Length][];
            for (int i = 0; i < bmpArray.Length; ++i)
            {
                bmpArray[i] = new byte[Snes4BppBitmap.Size];
                Array.Copy(romBgData, i * Snes4BppBitmap.Size, bmpArray[i], 0, bmpArray[i].Length);
                snes4BppBitmapArray[i] = new Snes4BppBitmap(bmpArray[i]);
            }
        }
    }
}
