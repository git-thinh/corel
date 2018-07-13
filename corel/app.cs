using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading; 

namespace corel
{
    public class app
    {
        static app()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (se, ev) =>
            {
                Assembly asm = null;
                string comName = ev.Name.Split(',')[0];

                string resourceName = @"dll\" + comName + ".dll";
                var assembly = Assembly.GetExecutingAssembly();
                resourceName = typeof(app).Namespace + "." + resourceName.Replace(" ", "_").Replace("\\", ".").Replace("/", ".");
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                //using (Stream stream = File.OpenRead("bin/" + comName + ".dll"))
                {
                    if (stream == null)
                    {
                        //Debug.WriteLine(resourceName);
                    }
                    else
                    {
                        byte[] buffer = new byte[stream.Length];
                        using (MemoryStream ms = new MemoryStream())
                        {
                            int read;
                            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                                ms.Write(buffer, 0, read);
                            buffer = ms.ToArray();
                        }
                        asm = Assembly.Load(buffer);
                    }
                }
                return asm;
            };
        }

        //static JobMonitor jom; 

        public static void f_INIT()
        {
            ThreadPool.SetMaxThreads(25, 25);

            System.Net.ServicePointManager.DefaultConnectionLimit = 1000;
            try
            {
                // active SSL 1.1, 1.2, 1.3 for WebClient request HTTPS
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3
                    | (SecurityProtocolType)3072
                    | (SecurityProtocolType)0x00000C00
                    | SecurityProtocolType.Tls;
            }
            catch
            {
                // active SSL 1.1, 1.2, 1.3 for WebClient request HTTPS
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3
                    | SecurityProtocolType.Tls;
            }
        }

        public static void f_RUN()
        { 
            //jom = new JobMonitor();
            //Application.EnableVisualStyles();


            //Application.Run(new fMedia());
            //Application.Run(new fMain());
            //Application.Run(new fEdit());
            //Application.Run(new fBrowser());
            //Application.Run(new fGeckFX());
            //main = new fApp(jobs);
            //main = new fNone(jom);
            //Application.Run(main);
            f_Exit();
        }


        //public static IFORM get_Main() {
        //    return null;
        //}

        static void f_Exit()
        {
            //jom.f_removeAll();

            //if (Xpcom.IsInitialized)
            //    Xpcom.Shutdown();
            //Application.ExitThread();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            //Application.Exit();
        }
         
    }

    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            app.f_INIT();
            //app.f_RUN();

            test_job.f_handle_HTTP_FILE();
            //test_job.f_jobTest_Handle();
            //test_job.f_jobTest_Factory();
        }
    }
}
