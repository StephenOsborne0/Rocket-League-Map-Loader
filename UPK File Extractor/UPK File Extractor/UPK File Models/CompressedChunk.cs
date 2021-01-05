using System.IO;

namespace UPK_File_Extractor.UPK_File_Models
{
    public class CompressedChunk
    {
        public int UncompressedOffset { get; set; }

        public int UncompressedSize { get; set; }

        public int CompressedOffset { get; set; }

        public int CompressedSize { get; set; }

        public void Parse(BinaryReader br)
        {
            UncompressedOffset = br.ReadInt32();
            UncompressedSize = br.ReadInt32();
            CompressedOffset = br.ReadInt32();
            CompressedSize = br.ReadInt32();
        }
    }
}
