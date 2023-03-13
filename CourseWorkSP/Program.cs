using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CourseWorkSP
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            RowsData rows = null;
            int in_param = -1;
            if(args.Length > 1)
                in_param = FindParameterPosition("-i", args);

            List<string> lines = null;
            try
            {
                if (in_param > 0 && CheckParametersValidity(in_param, args)
                                 && File.Exists(args[in_param + 1]))
                {
                    lines = File.ReadLines(args[in_param + 1], Encoding.Default).ToList();
                }
                else lines = File.ReadLines("test1.asm", Encoding.Default).ToList();

                rows = new RowsData(lines);

                WriteListInFile("result.txt", rows.CreateSymbolTable(), FileMode.Create);
                WriteListInFile("result.txt", rows.CreateSentenceStructureTable(), FileMode.Append);
                Console.WriteLine("Файл з даними було створено(result.txt).");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static int FindParameterPosition(string param, string[] args)
        {
            int result = -1;
            int counter = 0;
            
            foreach (var arg in args)
            {
                if (arg == param) {
                    result = counter;
                    break;
                }
                counter++;
            }

            return result;
        }

        private static bool CheckParametersValidity(int paramPos, string[] args)
        {
            return paramPos != -1 && paramPos + 1 < args.Length && args[paramPos + 1] != "";
        }

        private static void WriteListInFile(string path, List<string> lines, FileMode mode)
        {
            var output = File.Open(path, mode);
            foreach (var line in lines)
            {
                var bytes = Encoding.UTF8.GetBytes(line);
                output.Write(bytes, 0, bytes.Length);
            }

            var nl = Encoding.UTF8.GetBytes("\n\n");
            output.Write(nl, 0, nl.Length);
            output.Close();
        }
    }
}