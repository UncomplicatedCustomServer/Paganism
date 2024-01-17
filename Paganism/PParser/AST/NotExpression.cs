﻿using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public class NotExpression : EvaluableExpression
    {
        public EvaluableExpression Expression { get; }

        public NotExpression(BlockStatementExpression parent, int line, int position, string filepath, EvaluableExpression expression) : base(parent, line, position, filepath)
        {
            Expression = expression;
        }

        public override Value Eval(params Argument[] arguments)
        {
            return new BooleanValue(!Expression.Eval().AsBoolean());
        }
    }
}