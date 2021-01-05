using System;

namespace UPK_File_Extractor
{
    [Flags]
    public enum ObjectFlags
    {
        RF_Transactional = 0x00000001,
        RF_Public = 0x0000004,
        RF_SourceModified = 0x00000020,
        RF_LoadForClient = 0x00010000,
        RF_LoadForServer = 0x00020000,
        RF_LoadForEdit = 0x00040000,
        RF_Standalone = 0x00080000,
        RF_HasStack = 0x02000000,
        RF_Intrinsic = 0x04000000
    }
}