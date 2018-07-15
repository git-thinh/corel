namespace Rpc
{
    public partial class RpcException: System.ComponentModel.Win32Exception // Defined in resources
    {
        /// <summary>
        /// Exception class: RpcException : System.ComponentModel.Win32Exception
        /// Unspecified rpc error
        /// </summary>
        public RpcException(RpcError errorCode) : base(unchecked((int) errorCode))
        {
        }

        /// <summary>
        /// Returns the RPC Error as an enumeration
        /// </summary>
        public RpcError RpcError
        {
            get { return (RpcError)NativeErrorCode; }
        }

        [System.Diagnostics.DebuggerNonUserCode]
        internal static void Assert(int rawError)
        {
            Assert((RpcError)rawError);
        }

        /// <summary>
        /// Asserts that the argument is set to RpcError.RPC_S_OK or throws a new exception.
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCode]
        public static void Assert(RpcError errorCode)
        {
            if (errorCode != RpcError.RPC_S_OK)
            {
                RpcException ex = new RpcException(errorCode);
                Log.Error("RpcError.{0} - {1}", errorCode, ex.Message);
                throw ex;
            }
        }
    }
}