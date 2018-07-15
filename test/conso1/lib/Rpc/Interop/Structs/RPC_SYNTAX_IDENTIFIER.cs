using System;
using System.Runtime.InteropServices;

namespace Rpc.Interop.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct RPC_SYNTAX_IDENTIFIER
    {
        public Guid SyntaxGUID;
        public RPC_VERSION SyntaxVersion;
    }
}