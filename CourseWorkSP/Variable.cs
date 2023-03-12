using System.Collections.Generic;

namespace CourseWorkSP
{
    public enum VarType
    {
        db = 1,
        dw = 2,
        dd = 4
    }
    
    public class Variable
    {
        public string Name { get; }
        public VarType Type { get; }
        public int Offset { get; }
        public int InitPos { get; }
        public List<int> UsagePos { get; set; }

        public Variable(string name, VarType type, int offset, int initPos)
        {
            Name = name;
            Type = type;
            Offset = offset;
            InitPos = initPos;
            UsagePos = new List<int>();
        }
    }
}