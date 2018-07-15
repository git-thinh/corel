using System;
using System.Diagnostics;

namespace Rpc
{
    internal static class Log
    {
        private static readonly string Category = "RpcInterop";
        internal static bool VerboseEnabled = false;

        [Conditional("DEBUG")]
        public static void Verbose(string message)
        {
            if (VerboseEnabled)
            {
                Trace.WriteLine(message, Category);
            }
        }

        [Conditional("DEBUG")]
        public static void Verbose(string message, params object[] arguments)
        {
            if (VerboseEnabled)
            {
                try
                {
                    Verbose(String.Format(message, arguments));
                }
                catch
                {
                    Verbose(message);
                }
            }
        }

        [Conditional("DEBUG")]
        public static void Warning(string message)
        {
            Trace.WriteLine(message, Category);
        }

        [Conditional("DEBUG")]
        public static void Warning(string message, params object[] arguments)
        {
            try
            {
                Warning(String.Format(message, arguments));
            }
            catch
            {
                Warning(message);
            }
        }

        [Conditional("DEBUG")]
        public static void Error(Exception error)
        {
            Error("{0}", error);
        }

        [Conditional("DEBUG")]
        public static void Error(string message, params object[] arguments)
        {
            //try
            //{
            //    Error(String.Format(message, arguments));
            //}
            //catch
            //{
            //    Error(message);
            //}
        }
    }
}