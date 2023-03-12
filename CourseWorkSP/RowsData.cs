using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CourseWorkSP
{
    public class RowsData
    {
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
                    if(word != "")
                        _words[i].Add(word.Trim('\t'));
                }

                i++;
            }
        }
    }
}