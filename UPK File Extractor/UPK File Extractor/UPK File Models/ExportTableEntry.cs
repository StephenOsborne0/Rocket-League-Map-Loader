using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UPK_File_Extractor
{
    public class ExportTableEntry
    {
        public int ObjTypeRef { get; set; }

        public int ParentClassRef { get; set; }

        public int OwnerRef { get; set; }

        public int NameTableIndex { get; set; }

        public int NameCount { get; set; }

        public int ArchetypeRef { get; set; }

        public int ObjectFlagsH { get; set; }

        public int ObjectFlagsL { get; set; }

        public int ObjectFileSize { get; set; }

        public int ObjectDataOffset { get; set; }

        public int ExportFlags { get; set; }

        public int NumAdditionalFields { get; set; }

        public string FGuid { get; set; }

        public int Unknown1 { get; set; }

        public int Unknown2 { get; set; }

        public List<int> AdditionalFields { get; set; } = new List<int>();

        public void Parse(BinaryReader br)
        {
            ObjTypeRef = br.ReadInt32();
            ParentClassRef = br.ReadInt32();
            OwnerRef = br.ReadInt32();
            NameTableIndex = br.ReadInt32();
            NameCount = br.ReadInt32();
            ArchetypeRef = br.ReadInt32();
            ObjectFlagsH = br.ReadInt32();
            ObjectFlagsL = br.ReadInt32();
            ObjectFileSize = br.ReadInt32();
            ObjectDataOffset = br.ReadInt32();
            ExportFlags = br.ReadInt32();
            NumAdditionalFields = br.ReadInt32();
            FGuid = $"{br.ReadInt32()}.{br.ReadInt32()}.{br.ReadInt32()}.{br.ReadInt32()}";
            Unknown1 = br.ReadInt32();
            Unknown2 = br.ReadInt32();

            for (int i = 0; i < NumAdditionalFields; i++)
                AdditionalFields.Add(br.ReadInt32());
        }
    }
}