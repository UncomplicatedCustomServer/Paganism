﻿using Paganism.PParser.AST;

namespace Paganism.Interpreter
{
    public class Interpreter
    {
        public Interpreter(BlockStatementExpression blockStatement)
        {
            BlockStatement = blockStatement;
        }

        public BlockStatementExpression BlockStatement { get; }

        public void Run(bool isClearing = true)
        {
            BlockStatement.IsClearing = isClearing;
            BlockStatement.Execute();
        }
    }
}
