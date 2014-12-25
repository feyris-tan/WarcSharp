using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace Warc
{
    public class GzipHeader
    {
        public const ushort GZIP = 35615;

        Stream parentStream;
        CompressionMethod method;
        bool isText;
        bool hasChecksum;
        bool hasExtraFields;
        bool hasOriginalFileNameString;
        bool containsComment;
        uint lastModificationTime;
        byte extraFlags;
        GzipOperatingSystem operatingSystem;
        ushort extraSize;
        string originalFilename;
        string comment;
        ushort crc;

        long gzipOffset;
        long extraDataOffset;
        long compressedDataOffset;
        WARC_EXTRA_DATA extraData;

        uint crc32;
        uint uncompressedSize;

        internal GzipHeader(Stream parent)
        {
            parentStream = parent;
            BinaryReader br = new BinaryReader(parent);
            if (br.ReadUInt16() != GZIP)
                throw new InvalidDataException();
            gzipOffset = br.BaseStream.Position - 2;

            method = (CompressionMethod)br.ReadByte();
            byte mask = br.ReadByte();
            isText = (mask & 0x01) != 0;
            hasChecksum = (mask & 0x02) != 0;
            hasExtraFields = (mask & 0x04) != 0;
            hasOriginalFileNameString = (mask & 0x08) != 0;
            containsComment = (mask & 0x10) != 0;
            bool reserved1 = (mask & 0x20) != 0;
            bool reserved2 = (mask & 0x40) != 0;
            bool reserved3 = (mask & 0x80) != 0;
            lastModificationTime = br.ReadUInt32();
            extraFlags = br.ReadByte();
            operatingSystem = (GzipOperatingSystem)br.ReadByte();
            if (hasExtraFields)
            {
                extraSize = br.ReadUInt16();
                extraDataOffset = br.BaseStream.Position;
                if (extraSize != 12)
                {
                    throw new InvalidDataException("That doesn't look like a WARC file, because the Gzip Extra Data Length seems uncommon.");
                }
                extraData = new WARC_EXTRA_DATA(br.ReadBytes(extraSize));
                
            }
            if (hasOriginalFileNameString)
            {
                originalFilename = readNullTerminatedString(br);
            }
            if (containsComment)
            {
                comment = readNullTerminatedString(br);
            }
            if (hasChecksum)
            {
                crc = br.ReadUInt16();
            }

            compressedDataOffset = br.BaseStream.Position;

            br.BaseStream.Position = gzipOffset + (CompressedSize - 8);
            crc32 = br.ReadUInt32();
            uncompressedSize = br.ReadUInt32();
        }

        private string readNullTerminatedString(BinaryReader ber)
        {
            string result = "";
            char temp = char.MaxValue;
            do
            {
                temp = (char)ber.ReadByte();
                if (temp != 0)
                {
                    result += temp;
                }
                else
                {
                    return result;
                }
            } while (true);
            throw new EndOfStreamException();
        }

        public ushort ExtraDataSize
        {
            get
            {
                return extraSize;
            }
        }

        public long ExtraDataOffset
        {
            get
            {
                return extraDataOffset;
            }
        }

        public long GzipHeaderOffset
        {
            get
            {
                return gzipOffset;
            }
        }

        public long CompressedDataOffset
        {
            get
            {
                return compressedDataOffset;
            }
        }

        public long CompressedSize
        {
            get
            {
                return extraData.size;
            }
        }

        public int UncompressedSize
        {
            get
            {
                return (int)uncompressedSize;
            }
        }
        public override string ToString()
        {
            return "GZIP Data @" + gzipOffset;
        }

        public Stream GetStream()
        {
            parentStream.Position = compressedDataOffset;

            switch (method)
            {
                case CompressionMethod.Deflate:
                    return new DeflateStream(parentStream, CompressionMode.Decompress, true);
                default:
                    throw new NotImplementedException("Unimplemented compression method.");
            }
        }
    }

    enum CompressionMethod
    {
        Store = 0,
        Compress = 1,
        Pack = 2,
        LZH = 3,
        Deflate = 8
    }

    enum GzipOperatingSystem
    {
        FAT = 0,
        Amiga = 1,
        VMS = 2,
        Unix = 3,
        VM_CMS = 4,
        AtariTOS = 5,
        HPFSFilesystem = 6,
        Macintosh = 7,
        Z_System = 8,
        CP_M = 9,
        TOPS_20 = 10,
        NTFSFilesystem = 11,
        QDOS = 12,
        AcornRISCOS = 13,
        Unknown = 255
    }

    struct WARC_EXTRA_DATA
    {
        public uint unknown1;
        public uint size;
        public uint unknown2;

        public WARC_EXTRA_DATA(byte[] data)
        {
            unknown1 = BitConverter.ToUInt32(data, 0);
            size = BitConverter.ToUInt32(data, 4);
            unknown2 = BitConverter.ToUInt32(data, 8);
        }
    }
}
