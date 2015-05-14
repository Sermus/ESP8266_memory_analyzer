using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace MemAnalyzer
{
    class Program
    {
        private static string[] sections = new []
        {
	        "data",
	        "rodata",
	        "bss",
	        "text",
	        "irom0_text"
        };

        private static string[] sectionsDesc = new string[]
        {
	        "Initialized Data (RAM)",
	        "ReadOnly Data (RAM)",
	        "Uninitialized Data (RAM)",
	        "Cached Code (IRAM)",
	        "Uncached Code (SPI)"
        };

        private const int availableDRAM = 32786;
        private const int availableIRAM = 81920;

        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("USAGE:");
                Console.WriteLine("memanalyzer.exe path_to_objdump path_to_app_out");
                return;
            }

            string objDump = args[0].Replace('/', Path.DirectorySeparatorChar);
            string appOut = args[1].Replace('/', Path.DirectorySeparatorChar);
            if (!File.Exists(appOut))
            {
                Console.WriteLine("Please ensure app.out file location is correct: {0}", appOut);
                return;
            }

            Process process = new Process();
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = objDump;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.Arguments = "-t " + appOut;
            process.StartInfo.RedirectStandardOutput = true;
            string objDumpOutput = string.Empty;
            try
            {
                process.Start();
                objDumpOutput = process.StandardOutput.ReadToEnd();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Caught Exception " + ex.Message);
                return;
            }

            Console.WriteLine("{0, 10}|{1,30}|{2,12}|{3,12}|{4, 8}", "Section", "Description", "Start (hex)", "End (hex)", "Used space");
            Console.WriteLine("------------------------------------------------------------------------------");

            string[] lines = objDumpOutput.Split(new char[]{'\n'});
            long totalRamUsed = 0L;
            long totalIRamUsed = 0L;
            for (int i = 0; i < sections.Length; i++)
            {
                string sectionStartToken = string.Format(" _{0}_start", sections[i]);
                string sectionEndToken = string.Format(" _{0}_end", sections[i]);
                long sectionStart = -1L;
                long sectionEnd = -1L;
                string[] array2 = lines;
                for (int j = 0; j < array2.Length; j++)
                {
                    string currentLine = array2[j];
                    if (currentLine.IndexOf(sectionStartToken) != -1)
                    {
                        string[] array3 = currentLine.Split(new []{' '});
                        sectionStart = Convert.ToInt64(array3[0], 16);
                    }
                    if (currentLine.IndexOf(sectionEndToken) != -1)
                    {
                        string[] array4 = currentLine.Split(new []{' '});
                        sectionEnd = Convert.ToInt64(array4[0], 16);
                    }
                    if (sectionStart != -1L && sectionEnd != -1L)
                    {
                        break;
                    }
                }
                long sectionLength = sectionEnd - sectionStart;
                if (i < 3)
                {
                    totalRamUsed += sectionLength;
                }
                if (i == 3)
                {
                    totalIRamUsed = availableDRAM - sectionLength;
                }

                Console.WriteLine("{0, 10}|{1,30}|{2,12:X}|{3,12:X}|{4, 8}", sections[i], sectionsDesc[i], sectionStart, sectionEnd, sectionLength);
            }
            Console.WriteLine("Total Used RAM : {0}", totalRamUsed);
            Console.WriteLine("Free RAM : {0}", availableIRAM - totalRamUsed);
            Console.WriteLine("Free IRam : {0}", totalIRamUsed);
        }
    }
}
