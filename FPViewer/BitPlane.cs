using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPViewer
{
    class BitPlane
    {
        private int m_Data;
        public BitPlane(int data)
        {
            m_Data = data;
        }

        public int At(int index)
        {
             return (m_Data >> index) & 0x01;
        }
    }
}
