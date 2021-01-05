using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using UPK_File_Extractor.UPK_File_Models;

namespace UPK_File_Extractor
{
    public class PackageHeader
    {
        public int Signature { get; set; }

        public int Version { get; set; }

        public int HeaderSize { get; set; }

        public int FolderNameLength { get; set; }

        public string FolderName { get; set; }

        public int PackageFlags { get; set; }

        public int NameCount { get; set; }

        public int NameOffset { get; set; }

        public int ExportCount { get; set; }

        public int ExportOffset { get; set; }

        public int ImportCount { get; set; }

        public int ImportOffset { get; set; }

        public int DependsOffset { get; set; }

        public int SerialOffset { get; set; }

        public int Unknown2 { get; set; }

        public int Unknown3 { get; set; }

        public int Unknown4 { get; set; }

        //4 consecutive ints
        public string FGuid { get; set; }

        public int GenerationsCount { get; set; }

        public List<Generation> Generations { get; set; } = new List<Generation>();

        public int EngineVersion { get; set; }

        public int CookerVersion { get; set; }

        public int CompressionFlags { get; set; }

        public int NumCompressedChunks { get; set; }

        public List<CompressedChunk> CompressedChunks { get; set; } = new List<CompressedChunk>();

        public void Parse(BinaryReader br)
        {
            Version = br.ReadInt32();
            HeaderSize = br.ReadInt32();
            FolderNameLength = br.ReadInt32();
            FolderName = Encoding.UTF8.GetString(br.ReadBytes(FolderNameLength));
            PackageFlags = br.ReadInt32();
            NameCount = br.ReadInt32();
            NameOffset = br.ReadInt32();
            ExportCount = br.ReadInt32();
            ExportOffset = br.ReadInt32();
            ImportCount = br.ReadInt32();
            ImportOffset = br.ReadInt32();
            DependsOffset = br.ReadInt32();
            SerialOffset = br.ReadInt32();
            Unknown2 = br.ReadInt32();
            Unknown3 = br.ReadInt32();
            Unknown4 = br.ReadInt32();
            FGuid = $"{br.ReadInt32()}.{br.ReadInt32()}.{br.ReadInt32()}.{br.ReadInt32()}";
            GenerationsCount = br.ReadInt32();

            for(int i = 0; i < GenerationsCount; i++)
            {
                var generation = new Generation();
                generation.Parse(br);
                Generations.Add(generation);
            }

            EngineVersion = br.ReadInt32();
            CookerVersion = br.ReadInt32();
            CompressionFlags = br.ReadInt32();
            NumCompressedChunks = br.ReadInt32();

            for (int i = 0; i < NumCompressedChunks; i++)
            {
                var compressedChunk = new CompressedChunk();
                compressedChunk.Parse(br);
                CompressedChunks.Add(compressedChunk);
            }
        }
    }
}