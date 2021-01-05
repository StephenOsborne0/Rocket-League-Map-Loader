using System.IO;
using System.Text;

namespace UPK_File_Extractor
{
    public class ImportTableEntry
    {
        public int PackageIdx { get; set; }

        public int Unknown1 { get; set; }

        public int ObjTypeIdx { get; set; }

        public int Unknown2 { get; set; }

        public int OwnerRef { get; set; }

        public int NameTableIdx { get; set; }

        public int Unknown3 { get; set; }

        public void Parse(BinaryReader br)
        {
            PackageIdx = br.ReadInt32();
            Unknown1 = br.ReadInt32();
            ObjTypeIdx = br.ReadInt32();
            Unknown2 = br.ReadInt32();
            OwnerRef = br.ReadInt32();
            NameTableIdx = br.ReadInt32();
            Unknown3 = br.ReadInt32();
        }
    }
}