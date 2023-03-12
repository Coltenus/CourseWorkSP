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
            RowsData rows;
            int in_param = FindParameterPosition("-i", args);

            if(CheckParametersValidity(in_param, args)
               && File.Exists(args[in_param+1])){
                var lines = File.ReadLines(args[in_param+1], Encoding.Default).ToList();
                
                rows = new RowsData(lines);
            }
            
            Console.WriteLine();
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
    }
}