using System.IO;
using System.Text;

namespace UPK_File_Extractor
{
    public class NameTableEntry
    {
        public int NameLength { get; set; }

        public string Name { get; set; }

        //ObjectFlags
        public int NameFlagsH { get; set; }

        //ObjectFlags
        public int NameFlagsL { get; set; }

        public void Parse(BinaryReader br)
        {
            NameLength = br.ReadInt32();
            Name = Encoding.UTF8.GetString(br.ReadBytes(NameLength));
            NameFlagsH = br.ReadInt32();
            NameFlagsL = br.ReadInt32();
        }
    }
}