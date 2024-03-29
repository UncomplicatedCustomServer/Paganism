﻿using Paganism.Exceptions;
using Paganism.PParser.AST.Enums;
using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public abstract class Expression
    {
        public Expression() 
        {
            ExpressionInfo = new ExpressionInfo();
        }

        protected Expression(ExpressionInfo info)
        {
            ExpressionInfo = info;
        }

        public ExpressionInfo ExpressionInfo { get; }
    }
}
