using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CourseWorkSP
{
    public class Parser
    {
        private static readonly Dictionary<string, InstructionData> Keywords = new Dictionary<string, InstructionData>()
        {
            {".model", new InstructionData(InstructionType.model, 0, OperandType.str)},
            {".data", new InstructionData(InstructionType.data)},
            {".code", new InstructionData(InstructionType.code)},
            {"end", new InstructionData(InstructionType.code, 0, OperandType.str, OperandType.none, false)},
            {"jmp", new InstructionData(InstructionType.jmp, 2, OperandType.str)},
            {"sahf", new InstructionData(InstructionType.sahf, 1)},
            {"sal", new InstructionData(InstructionType.sal, 1, OperandType.reg, OperandType.imm)},
            {"rcr", new InstructionData(InstructionType.rcr, 1, OperandType.mem, OperandType.imm)},
            {"sbb", new InstructionData(InstructionType.sbb, 1, OperandType.reg, OperandType.reg)},
            {"test", new InstructionData(InstructionType.test, 1, OperandType.reg, OperandType.mem)},
            {"bts", new InstructionData(InstructionType.bts, 2, OperandType.mem, OperandType.reg)},
            {"mov", new InstructionData(InstructionType.mov, 1, OperandType.reg, OperandType.imm)},
            {"adc", new InstructionData(InstructionType.adc, 1, OperandType.mem, OperandType.imm)},
            {"jnb", new InstructionData(InstructionType.jnb, 2, OperandType.str)},
        };

        private static List<string> tokens32 = new List<string>()
        {
            "dd", "eax", "ebx", "ecx", "edx", "edi", "esi", "ebp"
        };

        private bool is32 = false;
        public bool Is32()
        {
            return is32;
        }


        private List<string> Rows;
        private List<RowType> RowTypes;
        private List<Variable> Variables;

        public Parser(ref RowsData rowsData)
        {
            Variables = new List<Variable>();
            Rows = rowsData.ToStringList();
            RowTypes = new List<RowType>();
            int i = 0;
            int sizeOfSegment = 0;
            foreach (var row in Rows)
            {
                rowsData._rows[i]._number = sizeOfSegment;
                foreach (var token in tokens32)
                {
                    if (row.Contains(token.ToString()))
                        is32 = true;
                }
                InstructionData? id = null;
                string keyStr = "";
                int ercode = 0;
                foreach (var key in Keywords)
                {
                    if (row.StartsWith(key.Key))
                    {
                        id = key.Value;
                        keyStr = key.Key;
                        sizeOfSegment += key.Value._size;
                    }
                    if(id != null)
                        break;
                }

                if (!IsRowValid(row, id, ref ercode))
                {
                    if(row.Length > 0 && row[0] == ';')
                        RowTypes.Add(RowType.comment);
                    else if (ercode != 0)
                    {
                        RowTypes.Add(RowType.error);
                    }
                    else
                    {
                        if(row == "") RowTypes.Add(RowType.empty);
                        else
                        {
                            if (row.Contains("db") || row.Contains("dw") || row.Contains("dd"))
                            {
                                RowTypes.Add(RowType.variable);
                                var words = row.Split(' ').ToList();
                                words.RemoveAll(EmptyString);
                                if (row.Contains("db"))
                                {
                                    Variables.Add(new Variable(words[0]
                                        , VarType.db, sizeOfSegment, i));
                                    sizeOfSegment += 1;
                                }
                                else if (row.Contains("dw"))
                                {
                                    Variables.Add(new Variable(words[0],
                                        VarType.dw, sizeOfSegment, i));
                                    sizeOfSegment += 2;
                                }
                                else if (row.Contains("dd"))
                                {
                                    Variables.Add(new Variable(words[0],
                                        VarType.dd, sizeOfSegment, i));
                                    sizeOfSegment += 4;
                                }
                            }
                            else if (row.Contains(":"))
                            {
                                RowTypes.Add(RowType.label);
                            }
                            else if (row == "")
                            {
                                RowTypes.Add(RowType.empty);
                            }
                        }
                    }
                }
                else
                {
                    if(keyStr == row.Split(' ')[0] && keyStr.Contains(".") || keyStr == "end")
                        RowTypes.Add(RowType.directive);
                    else if(row.Length > 0 && row[0] != ';')
                        RowTypes.Add(RowType.instruction);
                }

                if (RowTypes[i] == RowType.directive)
                {
                    sizeOfSegment = 0;
                }

                foreach (var variable in Variables)
                {
                    if(row.Contains(variable.Name) && i != variable.InitPos)
                        variable.UsagePos.Add(i+1);
                }

                i++;
            }
        }

        private static bool IsRowValid(string row, InstructionData? id, ref int ercode)
        {
            bool result = id != null;
            List<string> list = row.Split(' ').ToList();
            int oppos1 = 2, oppos2 = 3;
            list.RemoveAll(EmptyString);

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
                    catch (ArgumentOutOfRangeException e)
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

        private static bool EmptyString(string str)
        {
            return str == "";
        }
    }
}