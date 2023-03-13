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
            ".model", ".data", ".code", "end", "db", "dw", "dd"
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
        public List<List<string>> _words { get; set; }
        public bool isExist { get; }

        public RowsData()
        {
            isExist = false;
        }
        public RowsData(List<string> rows)
        {
            _rows = rows;
            _words = new List<List<string>>();
            isExist = true;
            int i = 0;
            foreach (var row in _rows)
            {
                if(row.Trim(new []{' ', '\t'}).StartsWith(";") || row.Trim(new []{' ', '\t'}) == "")
                    continue;
                _words.Add(new List<string>());
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
                                    _words[i].Add(newWord.Substring(offset, j - offset));
                                _words[i].Add(newWord.Substring(j, 1));
                                offset = j + 1;
                            }
                        }
                        if(offset != newWord.Length)
                            _words[i].Add(newWord.Substring(offset, newWord.Length - offset));
                    }
                }

                i++;
            }
        }

        public string CreateSymbolTable()
        {
            string result = "Таблиця лексем\n";
            foreach (var wordsLine in _words)
            {
                int i = 0;
                result += "№\t\tЛексема\t\tДовжина лексеми у символах\t\tТип лексеми\n";

                foreach (var word in wordsLine)
                {
                    string offset = "\t";
                    if (word.Length < 8) offset += "\t";
                    string description = "Ідентифікатор користувача або не визначений";
                    if (word.Length == 1) description = "односимвольна";
                    else if (_directives.Contains(word))
                        description = "Ідентифікатор директиви";
                    else if (_instructions.Contains(word.ToLower()))
                        description = "Ідентифікатор мнемокоду машиної інструкції";
                    else if (_bit32.Contains(word.ToLower()))
                        description = "Ідентифікатор 32-розрядного регестра даних";
                    else if (_bit16.Contains(word.ToLower()))
                        description = "Ідентифікатор 16-розрядного регестра даних";
                    else if (_bit8.Contains(word.ToLower()))
                        description = "Ідентифікатор 8-розрядного регестра даних";
                    else if (_modelType.Contains(word.ToLower()))
                        description = "Модель пам'яті";
                    else if (word.ToUpper() == "BYTE")
                        description = "Ідентифікатор тип 1";
                    else if (word.ToUpper() == "WORD")
                        description = "Ідентифікатор тип 2";
                    else if (word.ToUpper() == "DWORD")
                        description = "Ідентифікатор тип 4";
                    else if (word.ToUpper() == "PTR")
                        description = "Ідентифікатор оператора визначення типу";
                    else if (int.TryParse(word, out _))
                        description = "Десяткова константа";
                    else if (Regex.IsMatch(word, @"[0-f]+h"))
                        description = "Шістнадцяткова константа";
                    else if (Regex.IsMatch(word, "[0-1]+b"))
                        description = "Двійкова константа";
                    result += i + "\t\t" + word + offset + word.Length + "\t\t\t\t\t" + description + "\n";
                    i++;
                }
                
                result += "\n";
            }

            return result;
        }
    }
}