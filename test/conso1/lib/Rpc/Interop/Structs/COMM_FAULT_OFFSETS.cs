using System;
using System.Runtime.InteropServices;

namespace Rpc.Interop.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct COMM_FAULT_OFFSETS
    {
        public short CommOffset;
        public short FaultOffset;
    }
}