using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Google.ProtocolBuffers
{
   public class _test
    {
        public static void RUN_2()
        {


            /////////////////////////////////////////
            /// FREE RESOURCE

            Console.WriteLine("Enter to exit...");
            Console.ReadLine();
        }

        public static void RUN_1()
        {

            string addressBookFile = "addressbook.data";

            bool stopping = false;
            while (!stopping)
            {
                Console.WriteLine("Options:");
                Console.WriteLine("  L: List contents");
                Console.WriteLine("  A: Add new person");
                Console.WriteLine("  Q: Quit");
                Console.Write("Action? ");
                Console.Out.Flush();
                char choice = Console.ReadKey().KeyChar;
                Console.WriteLine();
                try
                {
                    switch (choice)
                    {
                        case 'A':
                        case 'a':
                            AddPerson.RUN(new string[] { addressBookFile });
                            break;
                        case 'L':
                        case 'l':
                            ListPeople.RUN(new string[] { addressBookFile });
                            break;
                        case 'Q':
                        case 'q':
                            stopping = true;
                            break;
                        default:
                            Console.WriteLine("Unknown option: {0}", choice);
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception executing action: {0}", e);
                }
                Console.WriteLine();
            }

            /////////////////////////////////////////
            /// FREE RESOURCE

            Console.WriteLine("Enter to exit...");
            Console.ReadLine(); 
        }
    }
}
