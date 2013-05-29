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
        public enum SpriteType { Item, Shiren, Character }
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
        private const int ADDR_PALLET_CHARACTER1 = 0x3BC613;
        private const int ADDR_PALLET_CHARACTER2 = 0x3BC631;

        private const int ADDR_SHIREN = 0x18180C + 1;

        private const int ADDR_KOZOU_TENGU = 0x12A57B;
        private const int ADDR_SHINO_TSUKAI = 0x713A2;
    
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
                else if (this.sprite == SpriteType.Shiren)
                {
                    ReadShiren(strm);
                    Width = SHIREN_WIDTH;
                    Height = SHIREN_HEIGHT;
                }
                else
                {
                    ReadCharacter(strm);
                }
            }
        }

        public int[] GetSpriteData()
        {
            SpriteImage image;
            if (sprite == SpriteType.Item)
            {
                image = new SpriteImage(Width, Height, data, pallet);
                return image.GetRawImage();
            }
            else
            {
                image = new SpriteImage(Width, Height, data, pallet);
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

        private void ReadCharacter(System.IO.FileStream strm)
        {
            CompressedImage image = new CompressedImage();
            CompressedImage.Header header = image.GetHeader(strm, ADDR_SHINO_TSUKAI);
            Width = header.width;
            Height = header.height;
            ReadPallet(strm, ADDR_PALLET_CHARACTER1, pallet);

            data = image.Extract(strm, ADDR_SHINO_TSUKAI);
            // 0 4 8 c 1 5 9 d 2 6 a e 3 7 b f というSnes4Bppの列から
            // 0 1 2 3 4 5 6 7 8 9 a b c d e f に変換する
            byte[] tmp = new byte[data.Length];
            Array.Copy(data, tmp, data.Length);
            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    Array.Copy(tmp, (i + j * 4) * Snes4BppBitmap.Size, 
                        data, (i * 4 + j) * Snes4BppBitmap.Size, Snes4BppBitmap.Size);
                }
            }
        }

        private void ReadBitmapAndPallete(System.IO.FileStream strm, int addrBitmap, byte[] bitmap, int addrPallete, byte[] pallete)
        {
            ReadBitmap(strm, addrBitmap, bitmap);
            ReadPallet(strm, addrPallete, pallete);
        }

        private void ReadPallet(System.IO.FileStream strm, int addrPallete, byte[] pallete)
        {
            // 透明色の2バイトはROMに含まれない
            strm.Seek(addrPallete, System.IO.SeekOrigin.Begin);
            strm.Read(pallete, 2, pallete.Length - 2);
        }

        private void ReadBitmap(System.IO.FileStream strm, int addrBitmap, byte[] bitmap)
        {
            strm.Seek(addrBitmap, System.IO.SeekOrigin.Begin);
            strm.Read(bitmap, 0, bitmap.Length);
        }
    }
}
