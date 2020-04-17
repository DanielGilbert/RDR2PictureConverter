using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace RDR2PicConv
{
    class Program
    {
        static void Main(string[] args)
        {
            Welcome();

            string currentDirectory = Directory.GetCurrentDirectory();

            Console.WriteLine($"Searching in: \"{currentDirectory}\"");

            string[] files = Directory.GetFiles(currentDirectory, "PRDR*");
            //FF D8 FF E0 00 10
            byte[] magicJpgBytes = new byte[] { 255, 216, 255, 224, 0, 16, };

            if (files.Length == 0)
            {
                Goodbye(files.Length);
                return;
            }

            foreach(string file in files)
            {
                if (Path.HasExtension(file))
                {
                    Console.WriteLine($"File \"{Path.GetFileName(file)}\" matches the first four letters, but has a file extension.");
                    continue;
                }
                try
                {
                    byte[] data = File.ReadAllBytes(file);
                    int idx = SearchBytes(data, magicJpgBytes);
                    if (idx == -1)
                    {
                        Console.WriteLine($"File \"{Path.GetFileName(file)}\" doesn't contain JPG data.");
                        continue;
                    }

                    Console.WriteLine($"Found JPG data in \"{Path.GetFileName(file)}\"");

                    byte[] result = data.Skip(idx).Take(data.Length - (idx + 1)).ToArray();

                    File.WriteAllBytes(file + ".jpg", result);

                    Console.WriteLine($"Wrote \"{Path.GetFileName(file)}.jpg\"");
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error while reading \"{file}\": {ex}");
                }


            }
        }

        /// <summary>
        /// Kudos to https://stackoverflow.com/a/26880541
        /// </summary>
        private static int SearchBytes(byte[] haystack, byte[] needle)
        {
            var len = needle.Length;
            var limit = haystack.Length - len;
            for (var i = 0; i <= limit; i++)
            {
                var k = 0;
                for (; k < len; k++)
                {
                    if (needle[k] != haystack[i + k]) break;
                }
                if (k == len) return i;
            }
            return -1;
        }

        private static void Goodbye(int filesFound)
        {
            Console.WriteLine($"Converted {filesFound} file(s).");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static void Welcome()
        {
            Console.WriteLine("********************************************");
            Console.WriteLine("*                                          *");
            Console.WriteLine("*        RDR 2 Screenshot Patcher          *");
            Console.WriteLine("*                  by                      *");
            Console.WriteLine("*     Daniel \"HerrGilbert\" Gilbert       *");
            Console.WriteLine("*                                          *");
            Console.WriteLine("*             https://g5t.de               *");
            Console.WriteLine("*                                          *");
            Console.WriteLine("********************************************");
        }
    }
}