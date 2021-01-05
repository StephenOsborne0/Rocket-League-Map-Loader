using System.Collections.Generic;
using System.IO;

namespace UPK_File_Extractor
{
    public class ImportTable
    {
        public List<ImportTableEntry> Entries { get; set; } = new List<ImportTableEntry>();

        public void Parse(BinaryReader br, int offset, int count)
        {
            br.BaseStream.Position = offset;

            for (int i = 0; i < count; i++)
            {
                var entry = new ImportTableEntry();
                entry.Parse(br);
                Entries.Add(entry);
            }
        }
    }
}