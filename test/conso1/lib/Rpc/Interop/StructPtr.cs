using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Rpc.Interop
{
    internal class Ptr<T> : IDisposable
    {
        private readonly GCHandle _handle;

        public Ptr(T data)
        {
            _handle = GCHandle.Alloc(data, GCHandleType.Pinned);
        }

        public T Data
        {
            get { return (T) _handle.Target; }
        }

        public IntPtr Handle
        {
            get { return _handle.AddrOfPinnedObject(); }
        }

        public void Dispose()
        {
            _handle.Free();
        }
    }

    internal class FunctionPtr<T> : IDisposable
        //whish I could: where T : Delegate
        where T : class, ICloneable, ISerializable
    {
        private T _delegate;
        public IntPtr Handle;

        public FunctionPtr(T data)
        {
            _delegate = data;
            Handle = Marshal.GetFunctionPointerForDelegate((Delegate) (object) data);
        }
         
        public void Dispose()
        {
            _delegate = null;
            Handle = IntPtr.Zero; 
        }
    }
}