using System.Collections.Generic;
using System.IO;

namespace UPK_File_Extractor
{
    public class ExportTable
    {
        public List<ExportTableEntry> Entries { get; set; } = new List<ExportTableEntry>();

        public void Parse(BinaryReader br, int offset, int count)
        {
            br.BaseStream.Position = offset;

            for (int i = 0; i < count; i++)
            {
                var entry = new ExportTableEntry();
                entry.Parse(br);
                Entries.Add(entry);
            }
        }
    }
}