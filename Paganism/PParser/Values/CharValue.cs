﻿using Paganism.PParser.AST.Enums;
using Paganism.PParser.Values.Interfaces;

namespace Paganism.PParser.Values
{
    public class CharValue : Value, ISettable
    {
        public CharValue(ExpressionInfo info, char value) : base(info)
        {
            Value = value;
        }

        public override string Name => "Char";

        public override TypesType Type => TypesType.Char;

        public override TypesType[] CanCastTypes { get; } = new[]
        {
            TypesType.Char
        };

        public char Value { get; private set; }

        public override string AsString()
        {
            return Value.ToString();
        }

        public override bool Is(TypeValue typeValue)
        {
            return Type == typeValue.Value;
        }

        public override bool Is(Value value)
        {
            if (value is not CharValue charValue)
            {
                return false;
            }

            return charValue.Value == Value;
        }

        public void Set(Value value)
        {
            if (value is not CharValue charValue)
            {
                return;
            }

            Value = charValue.Value;
        }
    }
}
