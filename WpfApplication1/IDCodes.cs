using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundMixerServer
{
    class IDCodes
    {
        private static Stack<byte> codes = new Stack<byte>();

        public static void fill()
        {
            for(byte i = 0; i < 255; i++)
            {
                codes.Push(i);
            }
        }
        /// <summary>
        /// Claims a free ID Code
        /// </summary>
        /// <returns>A free ID Code or null if there are none</returns>
        public static byte? claim()
        {
            byte? code;
            lock(codes)
            {
                if (codes.Count > 0)
                    code = codes.Pop();
                else
                    code = null;
            }
            return code;
        }

        public static void free(byte IdCode)
        {
            lock(codes)
            {
                codes.Push(IdCode);
            }
        }
    }
}
