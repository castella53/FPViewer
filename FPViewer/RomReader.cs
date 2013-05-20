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
        System.IO.FileStream strm;

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

        private byte[] data;
        private byte[] pallet;

        public RomReader()
        {
            // 草データを読み込み
            strm = System.IO.File.Open("rom.smc", System.IO.FileMode.Open, System.IO.FileAccess.Read);
        }

        public int[] GetSpriteData(SpriteType type)
        {
            pallet = new byte[32];
            ItemImage image;
            if (type == SpriteType.Item)
            {
                ReadItem();
                image = new ItemImage(16, 16, data, pallet);
                return image.GetRawImage();
            }
            else
            {
                ReadShiren();
                image = new ItemImage(32, 32, data, pallet);
                return image.GetRawImage();
            }
        }

        private void ReadItem()
        {
            data = new byte[32 * 4]; // 32バイト*4個
            ReadBitmapAndPallete(strm, ADDR_LEAF, data, ADDR_PALLETE_ITEM, pallet);
        }

        private void ReadShiren()
        {
            data = new byte[32 * 16]; // 32バイト*16個
            ReadBitmapAndPallete(strm, ADDR_SHIREN, data, ADDR_PALLETE_SHIREN, pallet);
        }

        private void ReadBitmapAndPallete(System.IO.FileStream strm, int addrBitmap, byte[] bitmap, int addrPallete, byte[] pallete)
        {
            strm.Seek(addrBitmap, System.IO.SeekOrigin.Begin);
            strm.Read(bitmap, 0, bitmap.Length);

            // 黒色の2バイトはROMに含まれない
            strm.Seek(addrPallete, System.IO.SeekOrigin.Begin);
            strm.Read(pallete, 2, pallete.Length - 2);
        }
    }
}
