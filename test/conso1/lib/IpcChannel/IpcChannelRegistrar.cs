using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;

namespace IpcChannel
{
    /// <summary>
    /// Provides a default implementation of the channel registrar usuing the system registry as the 
    /// storage facility.
    /// </summary>
    public class IpcChannelRegistrar : IIpcChannelRegistrar
    {
        private static readonly string[] EmptyList = new string[0];
        const string LockFormat = "{0}.RegistrarLock";
        readonly RegistryKey _rootHive;
        readonly string _rootKey;

        /// <summary>
        /// Creates a ChannelRegistrar baseed in the hive specified at the allChannelsRoot path provided
        /// </summary>
        /// <param name="rootHive">One of the registry hives ex: Registry.CurrentUser</param>
        /// <param name="allChannelsRoot">The path to store the ex: @"Software\YourProduct\IpcChannels"</param>
        public IpcChannelRegistrar(RegistryKey rootHive, string allChannelsRoot)
        {
            _rootHive = Check.NotNull(rootHive);
            _rootKey = Check.NotEmpty(allChannelsRoot);

            using (RegistryKey key = OpenKey(true))
                Check.Assert<ArgumentException>(key != null);
        }

        RegistryKey OpenKey(bool writable, params string[] paths)
        {
            string path = _rootKey;
            foreach (string part in paths) path = Path.Combine(path, part);
            return writable
                ? _rootHive.CreateSubKey(path)
                : _rootHive.OpenSubKey(path, false);
        }

        /// <summary> Registers a member (instanceId) for the provided channel name </summary>
        public void RegisterInstance(string channelName, string instanceId, string name)
        {
            using (new MutexLock(LockFormat, channelName))
            using (RegistryKey key = OpenKey(true, channelName, instanceId))
            {
                if (key != null && !String.IsNullOrEmpty(name))
                    key.SetValue(null, name);
            }
        }

        /// <summary> Unregisters a member (instanceId) from the provided channel name </summary>
        public void UnregisterInstance(string channelName, string instanceId)
        {
            using (new MutexLock(LockFormat, channelName))
            using (RegistryKey key = OpenKey(true, channelName))
            {
                try 
                {
                    if (key != null)
                    {
                        RegistryKey test = key.OpenSubKey(instanceId, false);
                        if (test != null)
                        {
                            test.Close();
                            key.DeleteSubKeyTree(instanceId);
                        }
                    }
                }
                catch (ArgumentException) { }
                catch (IOException) { }
            }
        }

        /// <summary> Enumerates the registered instanceIds for the provided channel name </summary>
        public IEnumerable<string> GetRegisteredInstances(string channelName) { return GetRegisteredInstances(channelName, (IEnumerable<string>)null); }

        /// <summary> Enumerates the registered instanceIds who's name is instanceName for the provided channel name </summary>
        public IEnumerable<string> GetRegisteredInstances(string channelName, string nameFilter)
        {
            IEnumerable<string> filter = String.IsNullOrEmpty(nameFilter) ? (IEnumerable<string>)null : new string[] { nameFilter };
            return GetRegisteredInstances(channelName, filter);
        }

        /// <summary> Enumerates the registered instanceIds who's name is instanceName for the provided channel name </summary>
        public IEnumerable<string> GetRegisteredInstances(string channelName, IEnumerable<string> nameFilterIn)
        {
            List<string> nameFilter = new List<string>();
            if (nameFilterIn != null)
                foreach (string name in nameFilterIn)
                    if (!String.IsNullOrEmpty(name))
                        nameFilter.Add(name);
            nameFilter.Sort();

            string[] instances;
            using (new MutexLock(LockFormat, channelName))
            {
                using (RegistryKey key = OpenKey(false, channelName))
                    instances = key != null ? key.GetSubKeyNames() : new string[0];
            }
            List<string> found = new List<string>();
            foreach (string instance in instances)
            {
                if (nameFilter.Count == 0 || nameFilter.BinarySearch(instance, StringComparer.Ordinal) >= 0)
                    found.Add(instance);
                else
                {
                    try
                    {
                        using (RegistryKey key = OpenKey(false, channelName, instance))
                        {
                            string tmpName = key.GetValue(null, null) as string;
                            if (!String.IsNullOrEmpty(tmpName) && nameFilter.BinarySearch(tmpName, StringComparer.OrdinalIgnoreCase) >= 0)
                                found.Add(instance);
                        }
                    }
                    catch (IOException) { }
                }
            }
            return found;
        }

        /// <summary> Serializes the arguments for the event being sent to the specified instance </summary>
        public bool WriteParameters(string channelName, string instanceId, string eventName, string[] arguments)
        {
            int attempt = 0;
            while (true)
            {
                try
                {
                    using (RegistryKey key = OpenKey(true, channelName, instanceId))
                    {
                        if (key == null)
                            return false;
                        if (arguments != null && arguments.Length > 0)
                            key.SetValue(eventName, arguments, RegistryValueKind.MultiString);
                        else
                            key.DeleteValue(eventName, false);
                    }
                    return true;
                }
                catch (IOException)
                {
                    if(++attempt > 5)
                        return false;
                }
            }
        }

        /// <summary> Retreives the arguments for the event being sent to the specified instance </summary>
        public string[] ReadParameters(string channelName, string instanceId, string eventName)
        {
            string[] args;
            using (RegistryKey key = OpenKey(true, channelName, instanceId))
            {
                if (key == null)
                    return null;
                try
                {
                    args = key.GetValue(eventName, null, RegistryValueOptions.DoNotExpandEnvironmentNames) as string[];
                    if (args != null)
                        key.DeleteValue(eventName, false);
                }
                catch (IOException) { return null; }
            }
            return args ?? EmptyList;
        }
    }
}
