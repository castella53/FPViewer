using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPViewer
{
    class Snes4BppBitmap
    {
        public const int Width = 8;
        public const int Height = 8;
        public const int Bpp = 4;
        public const int Size = 32;

        public byte[,] data { get; private set; }
        public Snes4BppBitmap(byte[] buf)
        {
            System.Diagnostics.Debug.Assert(buf.Length == 32);

            data = new byte[8, 8];
            for (int k = 0; k < 2; ++k)
            {
                for (int i = 0; i < 16; i += 2)
                {
                    BitPlane bp0 = new BitPlane(buf[i + k * 16]);
                    BitPlane bp1 = new BitPlane(buf[i + 1 + k * 16]);

                    for (int j = 0; j < 8; ++j)
                    {
                        data[i / 2, j] += (byte)(bp0.At(7 - j) << k * 2);
                        data[i / 2, j] += (byte)(bp1.At(7 - j) << 1 + k * 2);
                    }
                }
            }
        }
    }
}
