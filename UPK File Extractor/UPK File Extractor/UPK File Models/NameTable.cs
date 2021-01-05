using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UPK_File_Extractor
{
    public class NameTable
    {
        public List<NameTableEntry> Entries { get; set; } = new List<NameTableEntry>();

        public void Parse(BinaryReader br, int offset, int count)
        {
            br.BaseStream.Position = offset;

            for(int i = 0; i < count; i++)
            {
                var entry = new NameTableEntry();
                entry.Parse(br);
                Entries.Add(entry);
            }
        }
    }
}