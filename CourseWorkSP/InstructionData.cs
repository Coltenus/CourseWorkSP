namespace CourseWorkSP
{
    public enum InstructionType
    {
        none = 0,
        model,
        data,
        code,
        end,
        jmp,
        sahf,
        sal,
        rcr,
        sbb,
        test,
        bts,
        mov,
        adc,
        jnb
    }

    public enum OperandType
    {
        none = 0,
        reg,
        mem,
        imm,
        str
    }

    public struct InstructionData
    {
        public InstructionType _instruction { get; }
        public OperandType _operand1 { get; }
        public OperandType _operand2 { get; }
        public bool _isFOpExist { get; }
        public bool _isSOpExist { get; }
        public bool _isFOpRequired { get; }
        public bool _isSOpRequired { get; }

        public InstructionData(InstructionType instruction = InstructionType.none,
            OperandType op1 = OperandType.none, OperandType op2 = OperandType.none,
            bool isFOpRequired = true, bool isSOpRequired = true)
        {
            _instruction = instruction;
            _operand1 = op1;
            _operand2 = op2;
            if (op1 != OperandType.none)
            {
                _isFOpExist = true;
                _isFOpRequired = true;
                if (op2 != OperandType.none)
                {
                    _isSOpExist = true;
                    _isSOpRequired = true;
                }
                else
                {
                    _isSOpExist = false;
                    _isSOpRequired = false;
                }
            }
            else
            {
                _isFOpExist = false;
                _isSOpExist = false;
                _isFOpRequired = false;
                _isSOpRequired = false;
            }

            if (_isSOpExist && !isSOpRequired)
            {
                _isSOpRequired = false;
                if (!isFOpRequired)
                {
                    _isFOpRequired = false;
                }
            }
            else if (_isFOpExist && !isFOpRequired)
            {
                _isFOpRequired = false;
            }
        }
    }
}