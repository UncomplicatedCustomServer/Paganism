﻿using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;

namespace Paganism.PParser.Values
{
    public class NoneValue : Value
    {
        public NoneValue(ExpressionInfo info) : base(info)
        {
        }

        public override string Name => "None";

        public override TypesType Type => TypesType.None;

        public override TypesType[] CanCastTypes { get; } = new[]
        {
            TypesType.String,
        };

        public override string AsString()
        {
            return "None";
        }
    }
}
