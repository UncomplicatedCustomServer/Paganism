﻿using Paganism.Interpreter.Data;
using Paganism.Interpreter.Data.Instances;
using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;
using System;

namespace Paganism.PParser.AST
{
    public class TryCatchExpression : EvaluableExpression, IStatement, IExecutable
    {
        public TryCatchExpression(ExpressionInfo info, BlockStatementExpression tryExpression, BlockStatementExpression catchExpression) : base(info)
        {
            TryExpression = tryExpression;
            CatchExpression = catchExpression;
        }

        public BlockStatementExpression TryExpression { get; }

        public BlockStatementExpression CatchExpression { get; }

        public override Value Evaluate(params Argument[] arguments)
        {
            try
            {
                var result = TryExpression.Evaluate(arguments);

                return result;
            }
            catch (Exception exception)
            {
                var structure = new StructureValue(ExpressionInfo, Interpreter.Data.Structures.Instance.Get(CatchExpression, "exception", ExpressionInfo));
                structure.Set("name", new StringValue(ExpressionInfo, exception.GetType().Name), ExpressionInfo.Filepath);
                structure.Set("description", new StringValue(ExpressionInfo, exception.Message), ExpressionInfo.Filepath);

                Variables.Instance.Set(ExpressionInfo, CatchExpression, "exception", new VariableInstance(
                    new InstanceInfo(false, false, ExpressionInfo.Filepath), structure));

                return CatchExpression.Evaluate();
            }
        }

        public void Execute(params Argument[] arguments)
        {
            Evaluate(arguments);
        }
    }
}
