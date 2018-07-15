using System.Collections.Generic;

namespace IpcChannel
{
    /// <summary>
    /// Interface to provide a means of channel member registration and cross-process serialization
    /// of arguments for specific events.  Implementations must be thread-safe even across process
    /// boundaries.
    /// </summary>
    public interface IIpcChannelRegistrar
    {
        /// <summary> Registers a member (instanceId) for the provided channel name </summary>
        void RegisterInstance(string channelName, string instanceId, string instanceName);
        /// <summary> Unregisters a member (instanceId) from the provided channel name </summary>
        void UnregisterInstance(string channelName, string instanceId);

        /// <summary> Enumerates the registered instanceIds for the provided channel name </summary>
        IEnumerable<string> GetRegisteredInstances(string channelName);
        /// <summary> Enumerates the registered instanceIds who's name is instanceName for the provided channel name </summary>
        IEnumerable<string> GetRegisteredInstances(string channelName, string instanceName);
        /// <summary> Enumerates the registered instanceIds who's name is instanceName for the provided channel name </summary>
        IEnumerable<string> GetRegisteredInstances(string channelName, IEnumerable<string> instanceNames);

        /// <summary> Serializes the arguments for the event being sent to the specified instance </summary>
        bool WriteParameters(string channelName, string instanceId, string eventName, string[] arguments);
        /// <summary> Retreives the arguments for the event being sent to the specified instance </summary>
        string[] ReadParameters(string channelName, string instanceId, string eventName);
    }
}