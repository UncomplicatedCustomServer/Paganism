﻿using Paganism.PParser.AST.Enums;

namespace Paganism.PParser.Values
{
    public class CharValue : Value
    {
        public CharValue(char value)
        {
            Value = value;
        }

        public override string Name => "Char";

        public override TypesType Type => TypesType.Char;

        public char Value { get; }

        public override string AsString()
        {
            return Value.ToString();
        }
    }
}