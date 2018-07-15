using System;
using System.Runtime.InteropServices;

namespace Rpc.Interop.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct RPC_SERVER_INTERFACE
    {
        public uint Length;
        public RPC_SYNTAX_IDENTIFIER InterfaceId;
        public RPC_SYNTAX_IDENTIFIER TransferSyntax;
        public IntPtr /*PRPC_DISPATCH_TABLE*/ DispatchTable;
        public uint RpcProtseqEndpointCount;
        public IntPtr /*PRPC_PROTSEQ_ENDPOINT*/ RpcProtseqEndpoint;
        public IntPtr DefaultManagerEpv;
        public IntPtr InterpreterInfo;
        public uint Flags;

        public static readonly Guid IID_SYNTAX = new Guid(0x8A885D04u, 0x1CEB, 0x11C9, 0x9F, 0xE8, 0x08, 0x00, 0x2B,
                                                          0x10,
                                                          0x48, 0x60);

        public RPC_SERVER_INTERFACE(RpcHandle handle, Ptr<MIDL_SERVER_INFO> pServer, Guid iid)
        {
            Length = (uint) Marshal.SizeOf(typeof (RPC_CLIENT_INTERFACE));
            InterfaceId = new RPC_SYNTAX_IDENTIFIER() {SyntaxGUID = iid, SyntaxVersion = RPC_VERSION.INTERFACE_VERSION};
            TransferSyntax = new RPC_SYNTAX_IDENTIFIER()
                                 {SyntaxGUID = IID_SYNTAX, SyntaxVersion = RPC_VERSION.SYNTAX_VERSION};

            RPC_DISPATCH_TABLE fnTable = new RPC_DISPATCH_TABLE();
            fnTable.DispatchTableCount = 1;
            fnTable.DispatchTable =
                handle.Pin(new RPC_DISPATCH_TABLE_Entry()
                               {DispatchMethod = RpcApi.ServerEntry.Handle, Zero = IntPtr.Zero});
            fnTable.Reserved = IntPtr.Zero;

            DispatchTable = handle.Pin(fnTable);
            RpcProtseqEndpointCount = 0u;
            RpcProtseqEndpoint = IntPtr.Zero;
            DefaultManagerEpv = IntPtr.Zero;
            InterpreterInfo = pServer.Handle;
            Flags = 0x04000000u;
        }
    }
}