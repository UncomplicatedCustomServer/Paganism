﻿using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.AST
{
    public class StatementExpression : Expression, IStatement, IExecutable
    {
        public StatementExpression(Expression[] expressions)
        {
            Expressions = expressions;
        }

        public Expression[] Expressions { get; }

        public void Execute(params Argument[] arguments)
        {
            throw new NotImplementedException();
        }
    }
}
