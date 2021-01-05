using System.IO;

namespace UPK_File_Extractor
{
    public class Generation
    {
        public int ExportCount { get; set; }

        public int NameCount { get; set; }

        public int NetObjectCount { get; set; }

        public void Parse(BinaryReader br)
        {
            ExportCount = br.ReadInt32();
            NameCount = br.ReadInt32();
            NetObjectCount = br.ReadInt32();
        }
    }
}