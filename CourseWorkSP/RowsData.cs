using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CourseWorkSP
{
    public class RowsData
    {
        private List<string> _directives = new List<string>()
        {
            ".model", ".data", ".code", "end"
        };
        private List<string> _varTypes = new List<string>()
        {
            "db", "dw", "dd"
        };
        private List<string> _instructions = new List<string>()
        {
            "jmp", "sahf", "sal", "rcr", "sbb", "test", "bts", "mov", "adc", "jnb"
        };
        private List<string> _bit32 = new List<string>()
        {
            "eax", "ebx", "ecx", "edx", "edi", "esi", "ebp"
        };
        private List<string> _bit16 = new List<string>()
        {
            "ax", "bx", "cx", "dx", "di", "si", "bp"
        };
        private List<string> _bit8 = new List<string>()
        {
            "ah", "al", "bh", "bl", "ch", "cl", "dh", "dl"
        };
        private List<string> _modelType = new List<string>()
        {
            "tiny", "small"
        };
        public List<string> _rows { get; set; }
        public List<Tuple<int, List<string>>> _words { get; set; }
        public Dictionary<string, int> _labels { get; set; }
        public bool isExist { get; }

        public RowsData()
        {
            isExist = false;
        }
        public RowsData(List<string> rows)
        {
            _rows = rows;
            _words = new List<Tuple<int, List<string>>>();
            _labels = new Dictionary<string, int>();
            isExist = true;
            int i = 0;
            int rowOffset = 0;
            foreach (var row in _rows)
            {
                if (row.Trim(new[] { ' ', '\t' }).StartsWith(";") || row.Trim(new[] { ' ', '\t' }) == "")
                {
                    rowOffset++;
                    continue;
                }
                _words.Add(new Tuple<int, List<string>>(rowOffset+i+1, new List<string>()));
                foreach (var word in row.Split(' '))
                {
                    if (word != "")
                    {
                        int offset = 0;
                        var newWord = word.Trim('\t');
                        for (int j = 0; j < newWord.Length; j++)
                        {
                            if (!(newWord[j] >= 'a' && newWord[j] <= 'z' || newWord[j] >= 'A' && newWord[j] <= 'Z' 
                            || newWord[j] == '.' || newWord[j] >= 48 && newWord[j] <= 57 ))
                            {
                                if(j - offset != 0)
                                    _words[i].Item2.Add(newWord.Substring(offset, j - offset));
                                _words[i].Item2.Add(newWord.Substring(j, 1));
                                offset = j + 1;
                            }
                        }
                        if(offset != newWord.Length)
                            _words[i].Item2.Add(newWord.Substring(offset, newWord.Length - offset));
                    }
                }

                i++;
            }
        }

        public Dictionary<int, string> CreateSymbolTable()
        {
            Dictionary<int, string> result = new Dictionary<int, string>();
            foreach (var wordsLine in _words)
            {
                int i = 0;
                result.Add(wordsLine.Item1, "");
                foreach (var word in wordsLine.Item2)
                {
                    string offset = "\t";
                    string wLen = word.Length.ToString();
                    if (word.Length+wLen.Length+3 < 8) offset += "\t";
                    string description = "user identifier or undefined";
                    if (word.Length == 1 && !int.TryParse(word, out _)) description = "symbol";
                    else if (_directives.Contains(word))
                        description = "directive";
                    else if (_varTypes.Contains(word))
                        description = "variable type";
                    else if (_instructions.Contains(word.ToLower()))
                        description = "mnemocode";
                    else if (_bit32.Contains(word.ToLower()))
                        description = "register 32";
                    else if (_bit16.Contains(word.ToLower()))
                        description = "register 16";
                    else if (_bit8.Contains(word.ToLower()))
                        description = "register 8";
                    else if (_modelType.Contains(word.ToLower()))
                        description = "model type";
                    else if (word.ToUpper() == "BYTE")
                        description = "size directive 1";
                    else if (word.ToUpper() == "WORD")
                        description = "size directive 2";
                    else if (word.ToUpper() == "DWORD")
                        description = "size directive 4";
                    else if (word.ToUpper() == "PTR")
                        description = "pointer directive";
                    else if (int.TryParse(word, out _) || Regex.IsMatch(word, @"[0-f]+d"))
                        description = "decimal constant";
                    else if (Regex.IsMatch(word, @"[0-f]+h"))
                        description = "hex constant";
                    else if (Regex.IsMatch(word, "[0-1]+b"))
                        description = "binary constant";
                    else if(!_labels.ContainsKey(word) && i+1 < wordsLine.Item2.Count && (wordsLine.Item2[i+1] == ":"
                                || _varTypes.Contains(wordsLine.Item2[i + 1])))
                        _labels.Add(word, wordsLine.Item1+1);
                    result[wordsLine.Item1] += "\t" + (i+1) + ")\t" + word + " (" + word.Length + ")" + offset + description + "\n";
                    i++;
                }
            }

            return result;
        }

        public Dictionary<int, string> CreateSentenceStructureTable()
        {
            Dictionary<int, string> result = new Dictionary<int, string>();
                

            foreach (var wordsLine in _words)
            {
                int i = 0;
                int lekMC = -1;
                int lekMCCount = 0;
                int lekOp1 = -1;
                int lekOp1Count = 0;
                int lekOp2 = -1;
                int lekOp2Count = 0;
                bool isFirst = true;
                bool isException = false;
                string labels = "";
                
                result.Add(wordsLine.Item1, "Sentence struct:\n");

                foreach (var word in wordsLine.Item2)
                {
                    if (_directives.Contains(word.ToLower()) || _varTypes.Contains(word.ToLower()) || word == ":")
                        isException = true;
                    if (_instructions.Contains(word.ToLower()))
                    {
                        if (lekMC == -1)
                            lekMC = i;
                        lekMCCount++;
                    }
                    else if(word == ",")
                    {
                        isFirst = false;
                    }
                    else
                    {
                        if (isFirst)
                        {
                            if (lekOp1 == -1)
                                lekOp1 = i;
                            lekOp1Count++;
                        }
                        else {
                            if (lekOp2 == -1)
                                lekOp2 = i;
                            lekOp2Count++;
                        }
                    }

                    if (_labels.ContainsKey(word))
                    {
                        labels += _labels[word] + ", ";
                    }
                    i++;
                }

                if (!isException && lekMC == -1 && wordsLine.Item2.Count > 0)
                {
                    result[wordsLine.Item1] += "wrong instruction\n";
                    return result;
                }
                {
                    int row = -1;
                    var buffer = FindElement(_words, wordsLine.Item2);
                    if (buffer != null)
                        row = buffer.Item1;
                    result[wordsLine.Item1] += "labels: " + labels + "\t| mnem(first, count): " + (lekMC+1) + ", " + lekMCCount
                    + "\t| operand 1(first, count): " + (lekOp1+1) + ", " + lekOp1Count + "\t| operand 2(first, count): " + (lekOp2+1)
                    + ", " + lekOp2Count + "\n";
                }
            }

            return result;
        }

        public static Tuple<int, List<string>> FindElement(List<Tuple<int, List<string>>> list, List<string> value)
        {
            foreach (var item in list)
            {
                if (item.Item2 == value)
                    return item;
            }

            return null;
        }
    }
}