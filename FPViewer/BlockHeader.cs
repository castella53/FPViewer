using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPViewer
{
    class BlockHeader
    {
        public enum Type
        {
            RAW_DATA            = 0,
            NONE                = 1,
            ZERO_ABBREVIATION   = 2,
            REPEAT_ABBREVIATION = 3,
        }

        byte m_Data;
        public BlockHeader(byte data)
        {
            m_Data = data;
        }

        public Type At(int index)
        {
            // 7 6 5 4 3 2 1 0
            //   3   2   1   0
            return (Type)((m_Data >> index * 2) & 0x3);
        }
    }
}
