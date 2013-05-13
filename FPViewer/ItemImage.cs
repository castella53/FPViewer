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

        public ItemImage(byte[] romBgData, byte[] romPallete)
        {
            System.Diagnostics.Debug.Assert(romBgData.Length == 128);

            Snes4BppBitmap[] snes4BppBitmapArray = new Snes4BppBitmap[4];
            ConcatBitmapArray(romBgData, snes4BppBitmapArray);

            SnesPallete pallete = new SnesPallete(romPallete);

            Create16x16ItemImage(snes4BppBitmapArray, pallete);
        }

        private void Create16x16ItemImage(Snes4BppBitmap[] snes4BppBitmapArray, SnesPallete pallete)
        {
            rgbBitmap = new int[16 * 16];
            int rgbIndex = 0;
            for (int arrayOffset = 0; arrayOffset < 2; ++arrayOffset)
            {
                for (int row = 0; row < 8; ++row)
                {
                    for (int arrayNum = 0; arrayNum < 2; ++arrayNum)
                    {
                        for (int column = 0; column < 8; ++column)
                        {
                            int palleteIndex = snes4BppBitmapArray[arrayOffset * 2 + arrayNum].bitmap[row, column];
                            rgbBitmap[rgbIndex++] = pallete.data[palleteIndex];
                        }
                    }
                }
            }
        }

        private void ConcatBitmapArray(byte[] romBgData, Snes4BppBitmap[] snes4BppBitmapArray)
        {
            byte[][] bmpArray = new byte[4][];
            for (int i = 0; i < bmpArray.Length; ++i)
            {
                bmpArray[i] = new byte[32];
                Array.Copy(romBgData, i * 32, bmpArray[i], 0, bmpArray[i].Length);
                snes4BppBitmapArray[i] = new Snes4BppBitmap(bmpArray[i]);
            }
        }
    }
}
