using System;

namespace IpcChannel
{
    /// <summary>
    /// Represents an event raised by the IpcEventChannel to subscribers.
    /// </summary>
    public class IpcSignalEventArgs : EventArgs
    {
        private readonly IpcEventChannel _channel;
        private readonly string _name;
        private readonly string[] _arguments;

        /// <summary> Creates the event </summary>
        internal IpcSignalEventArgs(IpcEventChannel channel, string name, string[] args)
        {
            _channel = channel;
            _name = name;
            _arguments = (string[])args.Clone();
        }

        /// <summary> Gets the channel rasing the event </summary>
        public IpcEventChannel EventChannel { get { return _channel; } }
        /// <summary> Gets the name of the event </summary>
        public string EventName { get { return _name; } }
        /// <summary> Gets any arguments sent with the event </summary>
        public string[] Arguments { get { return (string[])_arguments.Clone(); } }
    }
}