using System.Diagnostics;

namespace System
{
    public class Tracer
    {
        const string _fix = "[LOG] - ";
        
        [Conditional("DEBUG")]
        public static void WriteLine(string message)
        {
            Debug.WriteLine(_fix + message);
        }

        [Conditional("DEBUG")]
        public static void WriteLine(string format, object arg0)
        {
            if (!format.Contains("{0}")) format += "\r\n\t\t[1] {0}";
            Debug.WriteLine(string.Format(_fix + format, arg0));
        }

        [Conditional("DEBUG")]
        public static void WriteLine(string format, object arg0, object arg1)
        {
            if (!format.Contains("{0}")) format += "\r\n\t\t[1] {0}";
            if (!format.Contains("{1}")) format += "\r\n\t\t[2] {1}";
            Debug.WriteLine(string.Format(_fix + format, arg0, arg1));
        }

        [Conditional("DEBUG")]
        public static void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            if (!format.Contains("{0}")) format += "\r\n\t\t[1] {0}";
            if (!format.Contains("{1}")) format += "\r\n\t\t[2] {1}";
            if (!format.Contains("{2}")) format += "\r\n\t\t[3] {2}";
            Debug.WriteLine(string.Format(_fix + format, arg0, arg1, arg2));
        }

        [Conditional("DEBUG")]
        public static void WriteLine(string format, object arg0, object arg1, object arg2, object arg3)
        {
            if (!format.Contains("{0}")) format += "\r\n\t\t[1] {0}";
            if (!format.Contains("{1}")) format += "\r\n\t\t[2] {1}";
            if (!format.Contains("{2}")) format += "\r\n\t\t[3] {2}";
            if (!format.Contains("{3}")) format += "\r\n\t\t[4] {3}";
            Debug.WriteLine(string.Format(_fix + format, arg0, arg1, arg2, arg3));
        }

        [Conditional("DEBUG")]
        public static void WriteLine(string format, object arg0, object arg1, object arg2, object arg3, object arg4)
        {
            if (!format.Contains("{0}")) format += "\r\n\t\t[1] {0}";
            if (!format.Contains("{1}")) format += "\r\n\t\t[2] {1}";
            if (!format.Contains("{2}")) format += "\r\n\t\t[3] {2}";
            if (!format.Contains("{3}")) format += "\r\n\t\t[4] {3}";
            if (!format.Contains("{4}")) format += "\r\n\t\t[5] {4}";
            Debug.WriteLine(string.Format(_fix + format, arg0, arg1, arg2, arg3));
        }
    }
}
