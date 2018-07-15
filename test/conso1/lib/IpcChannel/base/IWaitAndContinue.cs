using System;
using System.Threading;

namespace IpcChannel
{
    /// <summary> Describes a set of WaitHandles that, when signaled, trigger a process to continue </summary>
    public interface IWaitAndContinue : IDisposable
    {
        /// <summary>
        /// Returns true when the task is complete, this value may change between calls; however, the
        /// HandleCount can not change except inside a call to ContinueProcessing.
        /// </summary>
        bool Completed { get; }
        /// <summary>
        /// Returns the number of handles that will be copied when CopyHandles is called, this value
        /// is invariant except inside a call to ContinueProcessing.  Must not return 0 unless Completed
        /// is also true.
        /// </summary>
        int HandleCount { get; }
        /// <summary>
        /// Copies the wait handles that will signal that this object is ready to continue processing
        /// </summary>
        void CopyHandles(WaitHandle[] array, int offset);
        /// <summary> 
        /// Called after one of the wait handles is signaled, providing the wait handle that was signaled.
        /// For a Mutex, this may also occur when AbandonedMutexException is raised. 
        /// </summary>
        void ContinueProcessing(WaitHandle handleSignaled);
    }
}
