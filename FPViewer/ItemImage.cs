using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPViewer
{
    class ItemImage
    {
        private int[] m_RbgBitmap;
        public ItemImage(byte[] romBgData, byte[] romPallete)
        {
            Snes4BppBitmap bmp = new Snes4BppBitmap(romBgData);
            SnesPallete pallete = new SnesPallete(romPallete);

            m_RbgBitmap = new int[8 * 8];

            for (int i = 0; i < m_RbgBitmap.Length; ++i)
            {
                m_RbgBitmap[i] = pallete.At(bmp.Get()[i / 8, i % 8]);
            }
        }

        public int[] Get()
        {
            return m_RbgBitmap;
        }
    }
}
