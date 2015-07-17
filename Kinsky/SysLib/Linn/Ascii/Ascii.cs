using System;
using System.Collections.Generic;
using System.Text;

using Linn;

//     0   1   2   3   4   5   6   7   8   9   A   B   C   D   E   F
// 0  NUL SOH STX ETX EOT ENQ ACK BEL BS  HT  LF  VT  FF  CR  SO  SI
// 1  DLE DC1 DC2 DC3 DC4 NAK SYN ETB CAN EM  SUB ESC FS  GS  RS  US
// 2   SP  !   "   #   $   %   &   '   (   )   *   +   ,   -   .   /
// 3   0   1   2   3   4   5   6   7   8   9   :   ;   <   =   >   ?
// 4   @   A   B   C   D   E   F   G   H   I   J   K   L   M   N   O
// 5   P   Q   R   S   T   U   V   W   X   Y   Z   [   \   ]   ^   _
// 6   `   a   b   c   d   e   f   g   h   i   j   k   l   m   n   o
// 7   p   q   r   s   t   u   v   w   x   y   z   {   |   }   ~ DEL

namespace Linn.Ascii
{
    public class AsciiError : Exception
    {
    }

    public interface IWriterAscii : IWriter
    {
        void WriteSpace();
        void WriteNewline();
        void WriteInt(int aValue);
        void WriteUint(uint aValue);
        void WriteInt64(long aValue);
        void WriteUint64(ulong aValue);
        void WriteHex(uint aValue);
        void WriteHex(byte aValue);
        void WriteHexPrefix();
    }

    public class Ascii
    {
        public const byte kAsciiNul = 0x00;
        public const byte kAsciiSoh = 0x01;
        public const byte kAsciiStx = 0x02;
        public const byte kAsciiEtx = 0x03;
        public const byte kAsciiEot = 0x04;
        public const byte kAsciiEnq = 0x05;
        public const byte kAsciiAck = 0x06;
        public const byte kAsciiBel = 0x07;
        public const byte kAsciiBs = 0x08;
        public const byte kAsciiHt = 0x09;
        public const byte kAsciiLf = 0x0a;
        public const byte kAsciiVt = 0x0b;
        public const byte kAsciiFf = 0x0c;
        public const byte kAsciiCr = 0x0d;
        public const byte kAsciiSo = 0x0e;
        public const byte kAsciiSi = 0x0f;

        public const byte kAsciiDle = 0x10;
        public const byte kAsciiDc1 = 0x11;
        public const byte kAsciiDc2 = 0x12;
        public const byte kAsciiDc3 = 0x13;
        public const byte kAsciiDc4 = 0x14;
        public const byte kAsciiNak = 0x15;
        public const byte kAsciiSyn = 0x16;
        public const byte kAsciiEtb = 0x17;
        public const byte kAsciiCan = 0x18;
        public const byte kAsciiEm = 0x19;
        public const byte kAsciiSub = 0x1a;
        public const byte kAsciiEsc = 0x1b;
        public const byte kAsciiFs = 0x1c;
        public const byte kAsciiGs = 0x1d;
        public const byte kAsciiRs = 0x1e;
        public const byte kAsciiUs = 0x1f;

        public const byte kAsciiSp = 0x20;

        public const byte kAsciiMinus = 0x2d;
        public const byte kAsciiDot = 0x2e;
        public const byte kAsciiColon = 0x3a;
        public const byte kAsciiEquals = 0x3d;
        public const byte kAsciiAngleOpen = 0x3c;
        public const byte kAsciiAngleClose = 0x3e;
        public const byte kAsciiHyphen = 0x2d;

        public const byte kAsciiDel = 0x7f;

        public const string kAsciiNewline = "\r\n";
        public const string kAsciiHexPrefix = "0x";

        public static byte[] Trim(byte[] aBuffer)
        {
            int start = 0;
            int bytes = aBuffer.Length;

            for (int i = 0; i < bytes; i++) {
                if (!IsWhitespace(aBuffer[start])) {
                    break;
                }
                start++;
            }

            if (start == bytes) {
                return (new byte[0]);
            }

            int end = bytes;

            while (IsWhitespace(aBuffer[end - 1])) {
                end--;
            }

            if (start == 0 && end == bytes) {
                return (aBuffer);
            }

            byte[] result = new byte[end - start];

            Array.Copy(aBuffer, start, result, 0, end - start);

            return (result);
        }

        public static bool IsWhitespace(byte aValue)
        {
            return (aValue <= kAsciiSp);
        }

        public static uint Uint(byte[] aBuffer)
        {
            uint value;

            try
            {
                value = UInt32.Parse(ASCIIEncoding.UTF8.GetString(aBuffer, 0, aBuffer.Length));
            }
            catch (FormatException)
            {
                throw (new AsciiError());
            }
            catch (OverflowException)
            {
                throw (new AsciiError());
            }

            return (value);
        }

        public static int Int(byte[] aBuffer)
        {
            int value;

            try
            {
                value = Int32.Parse(ASCIIEncoding.UTF8.GetString(aBuffer, 0, aBuffer.Length));
            }
            catch (FormatException)
            {
                throw (new AsciiError());
            }
            catch (OverflowException)
            {
                throw (new AsciiError());
            }

            return (value);
        }
    }

    public class WriterAscii : IWriterAscii
    {
        public WriterAscii(IWriter aWriter)
        {
            iWriter = aWriter;
        }

        // IWriter

        public void Write(byte aValue)
        {
            iWriter.Write(aValue);
        }

        public void Write(byte[] aBuffer)
        {
            iWriter.Write(aBuffer);
        }

        public void WriteFlush()
        {
            iWriter.WriteFlush();
        }

        // IWriterAscii

        public void WriteSpace()
        {
            iWriter.Write(Ascii.kAsciiSp);
        }

        public void WriteNewline()
        {
            iWriter.Write(ASCIIEncoding.UTF8.GetBytes(Ascii.kAsciiNewline));
        }

        public void WriteInt(int aValue)
        {
            iWriter.Write(ASCIIEncoding.UTF8.GetBytes(aValue.ToString()));
        }

        public void WriteUint(uint aValue)
        {
            iWriter.Write(ASCIIEncoding.UTF8.GetBytes(aValue.ToString()));
        }

        public void WriteInt64(long aValue)
        {
            iWriter.Write(ASCIIEncoding.UTF8.GetBytes(aValue.ToString()));
        }

        public void WriteUint64(ulong aValue)
        {
            iWriter.Write(ASCIIEncoding.UTF8.GetBytes(aValue.ToString()));
        }

        public void WriteHex(uint aValue)
        {
        }

        public void WriteHex(byte aValue)
        {
        }

        public void WriteHexPrefix()
        {
            iWriter.Write(ASCIIEncoding.UTF8.GetBytes(Ascii.kAsciiHexPrefix));
        }

        private IWriter iWriter;
    }

}
