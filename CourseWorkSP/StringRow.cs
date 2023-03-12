using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CourseWorkSP
{
    public class StringRow
    {
        public static int _number;
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
            foreach (var word in inWords)
            {
                _words.Add(word.Trim(new char[]{'\t', ' '}));
            }
            _words.RemoveAll(EmptyString);
        }

        private static bool EmptyString(string str)
        {
            return str == "";
        }

        public void ProcessRow()
        {
            // bool isData = false;
            // int size = 1;
            // int baseFormat = 10;
            var nwords = new List<string>();
            // if(_words.Count > 0 && _words[0] != ";")
                // nwords.Add(_number.ToString("X8"));
            // foreach (var word in _words)
            // {
                // if (word == "db" || word == "dw" || word == "dd")
                // {
                //     isData = true;
                //     if (word == "db") size = 2;
                //     else if (word == "dw") size = 4;
                //     else if (word == "dd") size = 8;
                //     _number += size / 2;
                // }
                // else if (isData)
                // {
                    // if (word.EndsWith("h")) baseFormat = 16;
                    // else if (word.EndsWith("b")) baseFormat = 2;
                    // if (word.EndsWith("d") || word.EndsWith("h") || word.EndsWith("b"))
                    // {
                        // int buf = Convert.ToInt32(word.Trim(new char[]{'d', 'h', 'b'}), baseFormat);
                        // var str = "X" + size.ToString();
                        // var tab = "\t";
                        // if (size < 8) tab += "\t";
                        // nwords.Add(buf.ToString(str)+tab);
                    // }
                // }
                // else if (word == ".code") _number = 0;
            // }

            if(_words.Count > 0 && !_words[0].StartsWith(";")){
                foreach (var word in _words)
                {
                    nwords.Add(word);
                }
            }

            _words = nwords;
        }
    }
}