using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;


namespace FPViewer
{
    class RomReader
    {
        public enum SpriteType { Item, Shiren }
        public int Width { get; private set; }
        public int Height { get; private set; }
        private SpriteType sprite;

        private const int ADDR_LEAF = 0x3DD5E9;
        private const int ADDR_SCROLL = ADDR_LEAF + 0x80 * 1;
        private const int ADDR_RICE = ADDR_LEAF + 0x80 * 2;
        private const int ADDR_SWORD = ADDR_LEAF + 0x80 * 3;
        private const int ADDR_ARROW = ADDR_LEAF + 0x80 * 4;
        private const int ADDR_SHIELD = ADDR_LEAF + 0x80 * 5;
        private const int ADDR_RING = ADDR_LEAF + 0x80 * 6;
        private const int ADDR_STAFF = ADDR_LEAF + 0x80 * 7;
        private const int ADDR_GITAN = ADDR_LEAF + 0x80 * 8;
        private const int ADDR_MEAT = ADDR_LEAF + 0x80 * 9;
        private const int ADDR_FEATHER = ADDR_LEAF + 0x80 * 10;
        private const int ADDR_POT = ADDR_LEAF + 0x80 * 11;
        private const int ADDR_SCROLL2 = ADDR_LEAF + 0x80 * 12;

        private const int ADDR_PALLETE_ITEM = 0x3BC5B9;
        private const int ADDR_PALLETE_SHIREN = 0x3BC5F5;

        private const int ADDR_SHIREN = 0x17F3C2;

        private const int ITEM_WIDTH = 16;
        private const int ITEM_HEIGHT = 16;
        private const int ITEM_4BPP_NUM = 4;

        private const int SHIREN_WIDTH = 32;
        private const int SHIREN_HEIGHT = 32;
        private const int SHIREN_4BPP_NUM = 16;

        private byte[] data;
        private byte[] pallet;

        public RomReader(SpriteType sprite)
        {
            this.sprite = sprite;
            pallet = new byte[SnesPallete.Size];

            using (System.IO.FileStream strm = System.IO.File.Open("rom.smc", System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                if (this.sprite == SpriteType.Item)
                {
                    ReadItem(strm);
                    Width = ITEM_WIDTH;
                    Height = ITEM_HEIGHT;
                }
                else
                {
                    ReadShiren(strm);
                    Width = SHIREN_WIDTH;
                    Height = SHIREN_HEIGHT;
                }
            }
        }

        public int[] GetSpriteData()
        {
            SpriteImage image;
            if (sprite == SpriteType.Item)
            {
                image = new SpriteImage(ITEM_WIDTH, ITEM_HEIGHT, data, pallet);
                return image.GetRawImage();
            }
            else
            {
                image = new SpriteImage(SHIREN_WIDTH, SHIREN_HEIGHT, data, pallet);
                return image.GetRawImage();
            }
        }

        private void ReadItem(System.IO.FileStream strm)
        {
            data = new byte[Snes4BppBitmap.Size * ITEM_4BPP_NUM]; 
            ReadBitmapAndPallete(strm, ADDR_LEAF, data, ADDR_PALLETE_ITEM, pallet);
        }

        private void ReadShiren(System.IO.FileStream strm)
        {
            data = new byte[Snes4BppBitmap.Size * SHIREN_4BPP_NUM];
            ReadBitmapAndPallete(strm, ADDR_SHIREN, data, ADDR_PALLETE_SHIREN, pallet);
            // 0 1 2 3 8 9 a b 4 5 6 7 c d e f というSnes4Bppの列から、
            // 0 1 2 3 4 5 6 7 8 9 a b c d e f に変換する
            byte[] tmp = new byte[data.Length];
            Array.Copy(data, tmp, data.Length);
            Array.Copy(tmp, 4 * Snes4BppBitmap.Size, data, 8 * Snes4BppBitmap.Size, 4 * Snes4BppBitmap.Size);
            Array.Copy(tmp, 8 * Snes4BppBitmap.Size, data, 4 * Snes4BppBitmap.Size, 4 * Snes4BppBitmap.Size);
        }

        private void ReadBitmapAndPallete(System.IO.FileStream strm, int addrBitmap, byte[] bitmap, int addrPallete, byte[] pallete)
        {
            strm.Seek(addrBitmap, System.IO.SeekOrigin.Begin);
            strm.Read(bitmap, 0, bitmap.Length);

            // 透明色の2バイトはROMに含まれない
            strm.Seek(addrPallete, System.IO.SeekOrigin.Begin);
            strm.Read(pallete, 2, pallete.Length - 2);
        }
    }
}
