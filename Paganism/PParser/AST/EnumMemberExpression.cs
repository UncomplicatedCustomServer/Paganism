﻿using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class EnumMemberExpression : Expression
    {
        public EnumMemberExpression(ExpressionInfo info, string name, NumberValue value, string enumParent) : base(info)
        {
            Name = name;
            Value = value;
            Enum = enumParent;
        }

        public string Name { get; }

        public NumberValue Value { get; }

        public string Enum { get; }
    }
}
