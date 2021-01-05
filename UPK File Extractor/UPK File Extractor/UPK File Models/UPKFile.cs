using System;
using System.IO;

namespace UPK_File_Extractor
{
    /// <summary>
    ///     http://wiki.tesnexus.com/index.php/UPK_File_Format_-_XCOM:EU_2012
    /// </summary>
    public class UPKFile
    {
        public string Filepath { get; set; }

        public PackageHeader PackageHeader { get; set; } = new PackageHeader();

        public NameTable NameTable { get; set; } = new NameTable();

        public ExportTable ExportTable { get; set; } = new ExportTable();

        public ImportTable ImportTable { get; set; } = new ImportTable();

        public UPKFile(string file)
        {
            Filepath = file;

            using (var filestream = File.OpenRead(file))
            {
                using (var br = new BinaryReader(filestream))
                {
                    var signature = br.ReadBytes(4);
                    Array.Reverse(signature);

                    if (BitConverter.ToString(signature) != BitConverter.ToString(new byte[] { 0x9E, 0x2A, 0x83, 0xC1 }))
                        return;

                    PackageHeader.Parse(br);
                    NameTable.Parse(br, PackageHeader.NameOffset, PackageHeader.NameCount);
                    ExportTable.Parse(br, PackageHeader.ExportOffset, PackageHeader.ExportCount);
                    ImportTable.Parse(br, PackageHeader.ImportOffset, PackageHeader.ImportCount);
                }
            }
        }

        public void ExtractAssets()
        {
            foreach(var export in ExportTable.Entries)
            {
                var nameIndex = export.NameTableIndex;
                var objectFileSize = export.ObjectFileSize;
                var objectDataOffset = export.ObjectDataOffset;

                string name;

                try { name = NameTable.Entries[nameIndex].Name; }
                catch
                {
                    name = $"Name Index {nameIndex}";

                }
                Console.WriteLine($"File name: {name}");
                Console.WriteLine($"File size: {objectFileSize} bytes");
                Console.WriteLine($"File offset: 0x{objectDataOffset:X8}");
                Console.WriteLine();

                //using (var filestream = File.OpenRead(Filepath))
                //{
                //    using(var br = new BinaryReader(filestream))
                //    {
                //        br.BaseStream.Position = objectDataOffset;
                //        var data = br.ReadBytes(objectFileSize);
                //    }
                //}
            }
        }
    }
}
