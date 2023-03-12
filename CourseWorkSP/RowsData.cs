using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CourseWorkSP
{
    public enum RowType
    {
        none = 0,
        empty,
        directive,
        instruction,
        variable,
        label,
        error
    }
    public class RowsData
    {
        public List<StringRow> _rows { get; }
        public bool isExist { get; }

        public RowsData()
        {
            isExist = false;
        }
        public RowsData(List<string> rawData)
        {
            _rows = new List<StringRow>();
            isExist = true;
            foreach (var line in rawData)
            {
                // if(line == "")
                //     continue;
                var row = new StringRow(line, 1);
                _rows.Add(row);
            }
        }

        public List<string> ToStringList()
        {
            List<string> result = new List<string>();

            foreach (var row in _rows)
            {
                var words = row.GetList();
                string str;
                if (words.Count > 0)
                    str = words[0];
                else str = "";
                for (int i = 1; i < words.Count; i++)
                {
                    str += " " + words[i];
                }
                result.Add(str);
            }

            return result;
        }

        public void WriteFile(string file)
        {
            if (isExist)
            {
                var output = File.Open(file, FileMode.Create);
                foreach (var line in ToStringList())
                {
                    output.Write(Encoding.Default.GetBytes(line + "\n"), 0, line.Length+1);
                }
            }
        }

        public void WriteConsole()
        {
            if (isExist)
            {
                foreach (var line in ToStringList())
                {
                    Console.WriteLine(line);
                }
            }
        }

        public void ProcessData()
        {
            StringRow._number = 0;
            foreach (var row in _rows)
            {
                row.ProcessRow();
            }

            for (int i = 0; i < _rows.Count;)
            {
                if (_rows[i].GetList().Count == 0)
                {
                    _rows.RemoveAt(i);
                }
                else i++;
            }
        }
    }
}