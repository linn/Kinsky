
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using Linn;


namespace Linn.Kinsky
{
    public class FileItc2Exception : Exception
    {
    }

    public class FileItc2
    {
        public class Section
        {
            public Section(byte[] aHeader)
            {
                if (HeaderBytes > aHeader.Length)
                {
                    throw new FileItc2Exception();
                }

                // bytes 0-3 : section length
                // bytes 4-7 : section type
                Bytes = Linn.BigEndianConverter.BigEndianToInt32(aHeader, 0);
                Type = ASCIIEncoding.ASCII.GetString(aHeader, 4, 4);
            }

            public const int HeaderBytes = 8;
            public readonly int Bytes;
            public readonly string Type;
        }

        public class Item
        {
            public Item(Section aSection, Stream aStream)
            {
                // read the item header
                byte[] header = new byte[Item.HeaderBytes];
                int bytesRead = aStream.Read(header, 0, Item.HeaderBytes);

                if (bytesRead != Item.HeaderBytes)
                {
                    throw new FileItc2Exception();
                }

                // bytes 0-3     : header length - including the 8 bytes of section header i.e. 208
                // bytes 4-19    : unknown
                // bytes 20-27   : library ID
                // bytes 28-35   : track ID
                // bytes 36-39   : item location e.g. "locl"
                // bytes 40-43   : item format e.g. "PNGf"
                // bytes 44-47   : unknown
                // bytes 48-51   : image width
                // bytes 52-55   : image height
                // bytes 56-67   : unknown
                // bytes 68-71   : display width
                // bytes 72-75   : display height
                // bytes 76-195  : unknown
                // bytes 196-199 : "data" - next byte is start of data
                if (Linn.BigEndianConverter.BigEndianToInt32(header, 0) != Item.HeaderBytes + Section.HeaderBytes ||
                    ASCIIEncoding.ASCII.GetString(header, 196, 4) != "data")
                {
                    throw new FileItc2Exception();
                }

                // read header information
                LibraryId = ASCIIEncoding.ASCII.GetString(header, 20, 8);
                TrackId = ASCIIEncoding.ASCII.GetString(header, 28, 8);
                Location = ASCIIEncoding.ASCII.GetString(header, 36, 4);
                Format = ASCIIEncoding.ASCII.GetString(header, 40, 4);
                ImageWidth = Linn.BigEndianConverter.BigEndianToInt32(header, 48);
                ImageHeight = Linn.BigEndianConverter.BigEndianToInt32(header, 52);
                DisplayWidth = Linn.BigEndianConverter.BigEndianToInt32(header, 68);
                DisplayHeight = Linn.BigEndianConverter.BigEndianToInt32(header, 72);

                // set the stream information
                if (Format.ToLower() == "argb")
                {
                    // The itc file contains raw image data - need to read out the data and convert it into
                    // a format that is ready for streaming

                    // read in the image data
                    byte[] imageData = new byte[aSection.Bytes - Section.HeaderBytes - Item.HeaderBytes];
                    bytesRead = aStream.Read(imageData, 0, imageData.Length);

                    if (bytesRead != imageData.Length)
                    {
                        throw new FileItc2Exception();
                    }

                    // a basic bitmap header consists of (all ints are little endian):
                    //
                    // Bitmap file header:
                    // bytes 0-1      : 'BM' identifier
                    // bytes 2-5      : size of the file in bytes
                    // bytes 6-9      : reserved
                    // bytes 10-13    : offset relative to start of file of the pixel data
                    //
                    // BITMAPINFOHEADER:
                    // bytes 14-17    : size of this BITMAPINFOHEADER (40 bytes)
                    // bytes 18-21    : image width in pixels
                    // bytes 22-25    : image height in pixels
                    // bytes 26-27    : number of colour planes (must be 1)
                    // bytes 28-29    : bits per pixel
                    // bytes 30-33    : compression method (0 for none)
                    // bytes 34-37    : pixel data size
                    // bytes 38-41    : horizontal pixels per metre
                    // bytes 42-45    : vertical pixels per metre
                    // bytes 46-49    : number of colours in the palette (0 for no palette)
                    // bytes 50-53    : number of important colours used (0 if all important)
                    //
                    // The pixel data follows. Rows of pixel data must be padded to contain multiple of 4 bytes
                    //

                    iStream = new MemoryStream();

                    // write all headers
                    BitWriter writer = new BitWriter(iStream, false);
                    iStream.WriteByte(0x42);
                    iStream.WriteByte(0x4D);
                    writer.WriteInt32(imageData.Length + 54);
                    writer.WriteInt32(0);
                    writer.WriteInt32(54);
                    writer.WriteInt32(40);
                    writer.WriteInt32(ImageWidth);
                    writer.WriteInt32(ImageHeight);
                    writer.WriteInt16(1);
                    writer.WriteInt16(32);
                    writer.WriteInt32(0);
                    writer.WriteInt32(imageData.Length);
                    writer.WriteInt32(2835);
                    writer.WriteInt32(2835);
                    writer.WriteInt32(0);
                    writer.WriteInt32(0);

                    for (int i =0 ; i<ImageHeight ; i++)
                    {
                        int rowOffset = (ImageHeight - 1 - i) * ImageWidth * 4;

                        for (int j=0 ; j<ImageWidth ; j++)
                        {
                            int pixelOffset = rowOffset + j*4;

                            iStream.WriteByte(imageData[pixelOffset + 3]);
                            iStream.WriteByte(imageData[pixelOffset + 2]);
                            iStream.WriteByte(imageData[pixelOffset + 1]);
                            iStream.WriteByte(imageData[pixelOffset]);
                        }
                    }

                    iStreamOffset = 0;
                    StreamBytes = (int)iStream.Length;
                }
                else
                {
                    // The itc file contains an image that is ready to stream e.g. a png so this item
                    // can just reference the original stream
                    iStream = aStream;
                    iStreamOffset = iStream.Position;
                    StreamBytes = aSection.Bytes - Section.HeaderBytes - Item.HeaderBytes;

                    // move the stream position to the end of the item
                    iStream.Position += StreamBytes;
                }
            }

            public readonly string LibraryId;
            public readonly string TrackId;
            public readonly string Location;
            public readonly string Format;
            public readonly int ImageWidth;
            public readonly int ImageHeight;
            public readonly int DisplayWidth;
            public readonly int DisplayHeight;

            public Stream GetStream()
            {
                iStream.Position = iStreamOffset;
                return iStream;
            }

            public readonly int StreamBytes;

            private const int HeaderBytes = 200;
            private Stream iStream;
            private long iStreamOffset;
        }


        public FileItc2(Stream aStream)
        {
            iStream = aStream;
            iItems = new List<Item>();

            // read first section
            Section section = ReadSectionHeader(iStream);
            if (section == null || section.Type != "itch")
            {
                throw new FileItc2Exception();
            }

            // read the "itch" section
            // bytes 0-15  : unknown
            // bytes 16-19 : content type
            // bytes 20+   : unknown
            byte[] itchData = new byte[20];
            int bytesRead = iStream.Read(itchData, 0, itchData.Length);

            if (bytesRead != itchData.Length ||
                ASCIIEncoding.ASCII.GetString(itchData, 16, 4) != "artw")
            {
                throw new FileItc2Exception();
            }

            // position stream at start of next section
            iStream.Position += section.Bytes - Section.HeaderBytes - 20;

            // read remaining sections
            while (true)
            {
                section = ReadSectionHeader(iStream);

                // check for EOS
                if (section == null)
                {
                    break;
                }

                if (section.Type == "item")
                {
                    // create the item - moves stream position to end of section
                    Item item = new Item(section, iStream);
                    iItems.Add(item);
                }
                else
                {
                    // unrecognised section - skip
                    iStream.Position += section.Bytes - Section.HeaderBytes;
                }
            }
        }

        public IList<Item> Items
        {
            get { return iItems.AsReadOnly(); }
        }

        private Section ReadSectionHeader(Stream aStream)
        {
            byte[] data = new byte[Section.HeaderBytes];

            int bytesRead = aStream.Read(data, 0, Section.HeaderBytes);
            if (bytesRead == Section.HeaderBytes)
            {
                // enough header data
                return new Section(data);
            }
            else if (bytesRead == 0)
            {
                // EOS
                return null;
            }

            // incomplete header
            throw new FileItc2Exception();
        }

        private Stream iStream;
        private List<Item> iItems;
    }
}



