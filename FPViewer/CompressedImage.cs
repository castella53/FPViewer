using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace FPViewer
{
    class CompressedImage
    {
        class Abbreviation
        {
            short m_Data;
            public Abbreviation(byte[] data)
            {
                m_Data = (short)(data[0] | data[1] << 8);
            }

            public bool IsAbbreviated(int index)
            {
                return ((m_Data >> index / 2) & 0x1) == 0;
            }
        }

        public enum Calibration
        {
            RIGHT = 0,
            LEFT  = 1,
        }

        public struct Header
        {
            public int width;
            public int height;
            public bool isCompressed;
            public Calibration calibration;
            public int calibrationPixels;
        }

        private Header MakeHeader(int data)
        {
            Header header = new Header();
            switch (data >> 6)
            {
                case 0:
                    header.width = 32;
                    header.height = 32;
                    header.isCompressed = false;
                    break;
                case 1:
                    header.width = 16;
                    header.height = 32;
                    header.isCompressed = true;
                    break;
                case 2:
                    header.width = 24;
                    header.height = 32;
                    header.isCompressed = true;
                    break;
                case 3:
                default:
                    header.width = 32;
                    header.height = 32;
                    header.isCompressed = true;
                    break;
            }

            if(((data >> 5) & 0x01) == 0)
            {
                header.calibration = Calibration.RIGHT;
            }
            else
            {
                header.calibration = Calibration.LEFT;
            }

            header.calibrationPixels = data & 0x0f;

            return header;
        }

        public Header GetHeader(System.IO.FileStream strm, int addr)
        {
            byte[] type = new byte[1];
            strm.Seek(addr, System.IO.SeekOrigin.Begin);
            strm.Read(type, 0, type.Length);

            return MakeHeader(type[0]);
        }

        private void ExtractCompressedBlock(System.IO.FileStream strm, ArrayList data, BlockHeader.Type type)
        {
            byte[] romData = new byte[Snes4BppBitmap.Size];
    
            switch (type)
            {
                case BlockHeader.Type.RAW_DATA:
                    FillRawData(strm, data, romData);
                    return;
                case BlockHeader.Type.NONE:
                    FillZero(data, romData);
                    return;
                case BlockHeader.Type.ZERO_ABBREVIATION:
                    FillZeroAbbreviation(strm, data, romData);
                    return;
                case BlockHeader.Type.REPEAT_ABBREVIATION:
                    FillRepeatAbbreviation(strm, data, romData);
                    return;
            }
        }

        private void FillRepeatAbbreviation(System.IO.FileStream strm, ArrayList data, byte[] romData)
        {
            byte[] abbreviationRaw = new byte[2];
            strm.Read(abbreviationRaw, 0, abbreviationRaw.Length);
            Abbreviation blockData = new Abbreviation(abbreviationRaw);
            byte[] previousTwoByte = new byte[2];
            for (int i = 0; i < romData.Length; i += 2)
            {
                if (blockData.IsAbbreviated(i))
                {
                    foreach (var v in previousTwoByte)
                    {
                        data.Add(v);
                    }
                }
                else
                {
                    strm.Read(previousTwoByte, 0, previousTwoByte.Length);
                    foreach (var v in previousTwoByte)
                    {
                        data.Add(v);
                    }
                }
            }
        }

        private void ConvertTo4bppBitmap(ArrayList source, ArrayList destination)
        {
            // source
            // [r0, bp1], [r0, bp2], [r0, bp3], [r0, bp4], [r1, bp1], [r1, bp2], [r1, bp3], [r1, bp4]
            // [r2, bp1], [r2, bp2], [r2, bp3], [r2, bp4], [r3, bp1], [r3, bp2], [r3, bp3], [r3, bp4]
            // [r4, bp1], [r4, bp2], [r4, bp3], [r4, bp4], [r5, bp1], [r5, bp2], [r5, bp3], [r5, bp4]
            // [r6, bp1], [r6, bp2], [r6, bp3], [r6, bp4], [r7, bp1], [r7, bp2], [r7, bp3], [r7, bp4]
            // destination
            // [r0, bp1], [r0, bp2], [r1, bp1], [r1, bp2], [r2, bp1], [r2, bp2], [r3, bp1], [r3, bp2]
            // [r4, bp1], [r4, bp2], [r5, bp1], [r5, bp2], [r6, bp1], [r6, bp2], [r7, bp1], [r7, bp2]
            // [r0, bp3], [r0, bp4], [r1, bp3], [r1, bp4], [r2, bp3], [r2, bp4], [r3, bp3], [r3, bp4]
            // [r4, bp3], [r4, bp4], [r5, bp3], [r5, bp4], [r6, bp3], [r6, bp4], [r7, bp3], [r7, bp4]
            
            // destinationの容量を増やす
            int destinationIndex = destination.Count;
            byte[] dummy = new byte[source.Count];
            foreach (var i in dummy)
            {
                destination.Add(i);
            }

            for (int i = 0; i < source.Count; i += 4)
            {
                destination[destinationIndex] = source[i];
                destination[destinationIndex + 1] = source[i + 1];
                destination[destinationIndex + 16] = source[i + 2];
                destination[destinationIndex + 17] = source[i + 3];
                destinationIndex += 2;
            }
        }

        private void FillZeroAbbreviation(System.IO.FileStream strm, ArrayList data, byte[] romData)
        {
            ArrayList tmpData = new ArrayList();
            byte[] abbreviationRaw = new byte[2];
            strm.Read(abbreviationRaw, 0, abbreviationRaw.Length);
            Abbreviation blockData = new Abbreviation(abbreviationRaw);
            for (int i = 0; i < romData.Length; i += 2)
            {
                byte[] twoByte = new byte[2];
                if (blockData.IsAbbreviated(i))
                {
                    foreach (var v in twoByte)
                    {
                        tmpData.Add(v);
                    }
                }
                else
                {
                    strm.Read(twoByte, 0, twoByte.Length);
                    foreach (var v in twoByte)
                    {
                        tmpData.Add(v);
                    }
                }
            }

            ConvertTo4bppBitmap(tmpData, data);
        }

        private void FillZero(ArrayList data, byte[] romData)
        {
            foreach (var i in romData)
            {
                data.Add(i);
            }
        }

        private void FillRawData(System.IO.FileStream strm, ArrayList data, byte[] romData)
        {
            strm.Read(romData, 0, romData.Length);
            foreach (var i in romData)
            {
                data.Add(i);
            }
        }

        public byte[] Extract(System.IO.FileStream strm, int addr)
        {
            Header header = GetHeader(strm, addr);
            ArrayList data = new ArrayList();

            // ブロック数の計算
            int blockNum = header.width / Snes4BppBitmap.Width * header.height / Snes4BppBitmap.Height;

            byte[] oneByte = new byte[1];
            for (int i = 0; i < blockNum / 4; ++i)
            {
                strm.Read(oneByte, 0, 1);
                // 各ブロックのタイプ取得
                BlockHeader type = new BlockHeader(oneByte[0]);
                for (int j = 0; j < 4; ++j)
                {
                    ExtractCompressedBlock(strm, data, type.At(j));
                }
            }
            
            return (byte[])data.ToArray(typeof(byte));
        }
    }
}
