using System;

namespace UPK_File_Extractor.UPK_File_Models
{
    [Flags]
    public enum FunctionSpecifierFlags
    {

    }

    //Function flags to specifiers mapping:
    //0x00002000 Static
    //0x00000020 Singular
    //0x00000400 Native
    //0x00004000 NoExport
    //0x00000200 Exec
    //0x00000008 Latent
    //0x00000004 Iterator
    //0x00000100 Simulated
    //0x00200000 Server
    //0x01000000 Client
    //0x00000080 Reliable
    //??? Unreliable
    //0x00020000 Public
    //0x00040000 Private
    //0x00080000 Protected
    //0x00001000 Operator
    //0x00000010 PreOperator
    //??? PostOperator
    //0x00000800 Event
    //0x00008000 Const
    //0x00000001 Final
}
