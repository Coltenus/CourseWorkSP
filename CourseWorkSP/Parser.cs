using System;
using System.Collections.Generic;
using System.Linq;

namespace CourseWorkSP
{
    public class Parser
    {
        private static readonly Dictionary<string, InstructionData> Keywords = new Dictionary<string, InstructionData>()
        {
            {".model", new InstructionData(InstructionType.model, OperandType.str)},
            {".data", new InstructionData(InstructionType.data)},
            {".code", new InstructionData(InstructionType.code)},
            {"end", new InstructionData(InstructionType.code, OperandType.str, OperandType.none, false)},
            {"jmp", new InstructionData(InstructionType.jmp, OperandType.str)},
            {"sahf", new InstructionData(InstructionType.sahf)},
            {"sal", new InstructionData(InstructionType.sal, OperandType.reg, OperandType.imm)},
            {"rcr", new InstructionData(InstructionType.rcr, OperandType.mem, OperandType.imm)},
            {"sbb", new InstructionData(InstructionType.sbb, OperandType.reg, OperandType.reg)},
            {"test", new InstructionData(InstructionType.test, OperandType.reg, OperandType.mem)},
            {"bts", new InstructionData(InstructionType.bts, OperandType.mem, OperandType.reg)},
            {"mov", new InstructionData(InstructionType.mov, OperandType.reg, OperandType.imm)},
            {"adc", new InstructionData(InstructionType.adc, OperandType.mem, OperandType.imm)},
            {"jnb", new InstructionData(InstructionType.jnb, OperandType.str)},
        };

        private List<string> Rows { get; }
        private List<RowType> RowTypes;

        public Parser(List<string> rows)
        {
            Rows = rows;
            RowTypes = new List<RowType>();
            foreach (var row in Rows)
            {
                InstructionData? id = null;
                string keyStr = "";
                int ercode = 0;
                foreach (var key in Keywords)
                {
                    if (row.Contains(key.Key))
                    {
                        id = key.Value;
                        keyStr = key.Key;
                    }
                    if(id != null)
                        break;
                }

                if (!IsRowValid(row, id, ref ercode))
                {
                    if (ercode != 0)
                    {
                        RowTypes.Add(RowType.error);
                    }
                    else
                    {
                        if(row == "") RowTypes.Add(RowType.empty);
                        else
                        {
                            if(row.Contains("db") || row.Contains("dw") || row.Contains("dd"))
                                RowTypes.Add(RowType.variable);
                            else if(row.Contains(":"))
                                RowTypes.Add(RowType.label);
                        }
                    }
                }
                else
                {
                    if(keyStr.Contains(".") || keyStr == "end")
                        RowTypes.Add(RowType.directive);
                    else RowTypes.Add(RowType.instruction);
                }
            }
        }

        private static bool IsRowValid(string row, InstructionData? id, ref int ercode)
        {
            bool result = id != null;
            List<string> list = row.Split(' ').ToList();
            int oppos1 = 2, oppos2 = 3;

            if (result)
            {
                if (list.Count >= oppos2 + 2 && list[oppos1] == "ptr")
                {
                    oppos1 += 2;
                    oppos2 += 2;
                    if (list.Count >= oppos2 + 2 && list[oppos2] == "ptr") oppos2 += 2;
                }
                if (id.Value._isFOpExist)
                {
                    if (list.Count >= oppos1)
                    {
                        CheckOperand(list[oppos1-1], id.Value._operand1, ref result, ref ercode);
                        if (id.Value._isSOpExist)
                        {
                            if (list.Count >= oppos2) CheckOperand(list[oppos2-1], id.Value._operand2, ref result, ref ercode);
                            else
                            {
                                result = false;
                                ercode = 1;
                            }
                        }
                    }
                    else
                    {
                        result = false;
                        ercode = 1;
                    }
                }
            }

            return result;
        }

        private static void CheckOperand(string operand, OperandType operandType, ref bool result, ref int ercode)
        {
            switch (operandType)
            {
                case OperandType.imm:
                    int baseFormat = 10;
                    if (operand.EndsWith("h")) baseFormat = 16;
                    else if (operand.EndsWith("b")) baseFormat = 2;
                    try
                    {
                        if (Convert.ToInt32(operand.Trim(new char[] { 'b', 'd', 'h' }), baseFormat) == 0
                            && operand.Trim(new char[] { 'b', 'd', 'h' }) != "0")
                        {
                            result = false;
                            ercode = 2;
                        }
                    }
                    catch (FormatException e)
                    {
                        result = false;
                        ercode = 2;
                    }
                    
                    break;
                case OperandType.reg:
                case OperandType.mem:
                case OperandType.str:
                    if (operand == "")
                    {
                        result = false;
                        ercode = 2;
                    }
                    break;
            }
        }

        public void WriteRowTypes()
        {
            foreach (var row in RowTypes)
            {
                Console.WriteLine("{0}", row.ToString());
            }
        }
    }
}