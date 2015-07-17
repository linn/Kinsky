
using System;
using System.IO;


namespace Linn
{
    internal static class BitConverterHelper
    {
        public static byte[] Reverse(byte[] aData, int aOffset, int aLength)
        {
            byte[] reverse = new byte[aLength];
            for (int i = 0; i < aLength; i++)
            {
                reverse[i] = aData[aOffset + aLength - 1 - i];
            }
            return reverse;
        }
    }


    public static class BigEndianConverter
    {
        public static int BigEndianToInt32(byte[] aValue, int aStartIndex)
        {
            if (System.BitConverter.IsLittleEndian)
            {
                byte[] value = BitConverterHelper.Reverse(aValue, aStartIndex, 4);
                return System.BitConverter.ToInt32(value, 0);
            }
            else
            {
                return System.BitConverter.ToInt32(aValue, aStartIndex);
            }
        }

        public static uint BigEndianToUint32(byte[] aValue, int aStartIndex)
        {
            if (System.BitConverter.IsLittleEndian)
            {
                byte[] value = BitConverterHelper.Reverse(aValue, aStartIndex, 4);
                return System.BitConverter.ToUInt32(value, 0);
            }
            else
            {
                return System.BitConverter.ToUInt32(aValue, aStartIndex);
            }
        }
    }


    // class for converting from native types to big/little endian binary data
    public class EndianConverter
    {
        public EndianConverter(bool aConvertToBigEndian)
        {
            iConvertToBigEndian = aConvertToBigEndian;
        }

        public byte[] FromInt16(Int16 aValue)
        {
            return Reorder(System.BitConverter.GetBytes(aValue));
        }

        public byte[] FromUint16(UInt16 aValue)
        {
            return Reorder(System.BitConverter.GetBytes(aValue));
        }

        public byte[] FromInt32(Int32 aValue)
        {
            return Reorder(System.BitConverter.GetBytes(aValue));
        }

        public byte[] FromUint32(UInt32 aValue)
        {
            return Reorder(System.BitConverter.GetBytes(aValue));
        }

        private byte[] Reorder(byte[] aValue)
        {
            if ((iConvertToBigEndian && !System.BitConverter.IsLittleEndian) ||
                (!iConvertToBigEndian && System.BitConverter.IsLittleEndian))
            {
                return aValue;
            }
            else
            {
                return BitConverterHelper.Reverse(aValue, 0, aValue.Length);
            }
        }

        private readonly bool iConvertToBigEndian;
    }


    // class for writing native type data to a stream using a given endian converter
    public class BitWriter
    {
        public BitWriter(Stream aStream, bool aConvertToBigEndian)
        {
            iStream = aStream;
            iConverter = new EndianConverter(aConvertToBigEndian);
        }

        public void WriteInt16(Int16 aValue)
        {
            byte[] data = iConverter.FromInt16(aValue);
            iStream.Write(data, 0, data.Length);
        }

        public void WriteUint16(UInt16 aValue)
        {
            byte[] data = iConverter.FromUint16(aValue);
            iStream.Write(data, 0, data.Length);
        }

        public void WriteInt32(Int32 aValue)
        {
            byte[] data = iConverter.FromInt32(aValue);
            iStream.Write(data, 0, data.Length);
        }

        public void WriteUint32(UInt32 aValue)
        {
            byte[] data = iConverter.FromUint32(aValue);
            iStream.Write(data, 0, data.Length);
        }

        private Stream iStream;
        private EndianConverter iConverter;
    }
}


