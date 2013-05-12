using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPViewer
{
    class SnesPallete
    {
        private int[] m_Pallete { get; set; }
        public SnesPallete(byte[] romPallete)
        {
            m_Pallete = new int[16];
            for (int i = 0; i < romPallete.Length; i+= 2)
            {
                int palleteHex = romPallete[i] + (romPallete[i + 1] << 8);
                byte r = (byte)((palleteHex >> 10) & 0x1f);
                byte g = (byte)((palleteHex >>  5) & 0x1f);
                byte b = (byte)((palleteHex >>  0) & 0x1f);

                byte normalizedR = (byte)(r << 3 | (r & 0x1c) >> 2);
                byte normalizedG = (byte)(g << 3 | (g & 0x1c) >> 2);
                byte normalizedB = (byte)(b << 3 | (b & 0x1c) >> 2);
                m_Pallete[i / 2] = (int)(normalizedB<< 16 | normalizedG << 8| normalizedR);
            }
        }

        public int At(int index)
        {
            return m_Pallete[index];
        }
    }
}
