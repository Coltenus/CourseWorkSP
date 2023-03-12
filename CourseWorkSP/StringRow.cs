using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CourseWorkSP
{
    public class StringRow
    {
        public int _number { get; set; }
        private List<string> _words { get; set; }

        private static readonly Dictionary<string, int> directsCommands = new Dictionary<string, int>() {
            {".model", 1}
        };

        public List<string> GetList()
        {
            return _words;
        }
        public StringRow(string line, int number)
        {
            _number = number;
            var inWords = line.Split(' ').ToList();
            _words = new List<string>();
            if(inWords.Count > 1)
                inWords.RemoveAll(EmptyString);
            int i = 0;
            foreach (var word in inWords)
            {
                var buffer = word.Trim(new char[] { '\t', ' ' });
                if(i > 0 && buffer.Contains(";"))
                    break;
                _words.Add(buffer);
                i++;
            }
        }

        private static bool EmptyString(string str)
        {
            return str == "";
        }
    }
}