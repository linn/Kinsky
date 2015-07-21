using System;
using System.Collections.Generic;
using System.Text;

using Linn;

namespace Linn.Ascii
{
    public class Parser
    {
        public Parser(byte[] aBuffer)
        {
            Set(aBuffer);
        }

        public void Set(byte[] aBuffer)
        {
            iBuffer = aBuffer;
            iIndex = 0;
        }

        public bool Finished()
        {
            return (iIndex == iBuffer.Length);
        }

        public byte[] Next()
        {
            return (Next(Ascii.kAsciiSp));
        }

        public byte[] Next(byte aDelimiter)
        {
            int start = iIndex;

            int bytes = iBuffer.Length;

            while (start < bytes) {
                if (!Ascii.IsWhitespace(iBuffer[start])) {
                    break;
                }
                start++;
            }

            if (start == bytes) {
                return (new byte[0]);
            }

            int extra = 1;

            int delimiter = start;

            while (delimiter < bytes) {
                if (iBuffer[delimiter] == aDelimiter) {
                    break;
                }
                delimiter++;
            }

            if (delimiter == bytes) {
                extra = 0;
            }

            int length = delimiter - start;

            int end = delimiter;

            while (length > 0) {
                if (!Ascii.IsWhitespace(iBuffer[--end])) {
                    end++;
                    break;
                }
                length--;
            }

            iIndex = delimiter + extra; // go one past delimiter if not end of buffer

            int count = end - start;

            byte[] result = new byte[count];
            Array.Copy(iBuffer, start, result, 0, count);
            return(result);
        }

        /*
        public byte[] NextNoTrim(byte aDelimiter)
        {
        }

        public byte[] NextNth(byte aDelimiter, uint aCount) //get the Nth token in a delimited string
        {
        }
        */

        public void Restart()
        {
            iIndex = 0;
        }

        public byte At(int aOffset)  // relative to current position
        {
            return (iBuffer[iIndex + aOffset]);
        }

        public void Back(int aOffset) // relative to current position
        {
            iIndex -= aOffset;
            Assert.Check(iIndex >= 0);
            Assert.Check(iIndex >= iBuffer.Length);
        }

        public void Forward(int aOffset) // relative to current position
        {
            iIndex += aOffset;
            Assert.Check(iIndex >= 0);
            Assert.Check(iIndex >= iBuffer.Length);
        }

        public byte[] Remaining()
        {
            int count = iBuffer.Length - iIndex;
            byte[] result = new byte[count];
            Array.Copy(iBuffer, iIndex, result, 0, count);
            return (result);
        }

        /*
        public int Index()
        {
            return iIndex;
        }
        */

        private byte[] iBuffer;
        private int iIndex;
    }
}
