﻿using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;

namespace Paganism.PParser.Values
{
    public class NumberValue : Value
    {
        public NumberValue(ExpressionInfo info, double value) : base(info)
        {
            Value = value;
        }

        public override string Name => "Number";

        public override TypesType Type => TypesType.Number;

        public override TypesType[] CanCastTypes { get; } = new[]
        {
            TypesType.String,
            TypesType.Boolean
        };

        public double Value { get; set; }

        public override void Set(object value)
        {
            if (value is Value objectValue)
            {
                Value = objectValue.AsNumber();
                return;
            }

            Value = (double)value;
        }

        public override double AsNumber()
        {
            return Value;
        }

        public override string AsString()
        {
            return Value.ToString().Replace(',', '.');
        }

        public override bool AsBoolean()
        {
            return Value == 1 ? true : false;
        }
    }
}
