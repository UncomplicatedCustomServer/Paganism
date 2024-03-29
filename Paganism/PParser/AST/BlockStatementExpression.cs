﻿using Paganism.Interpreter.Data;
using Paganism.PParser.AST.Interfaces;
using Paganism.PParser.Values;

namespace Paganism.PParser.AST
{
    public class BlockStatementExpression : EvaluableExpression, IStatement, IExecutable
    {
        public BlockStatementExpression(ExpressionInfo info, IStatement[] statements, bool isLoop = false, bool isClearing = true) : base(info)
        {
            Statements = statements;
            IsLoop = isLoop;
            IsClearing = isClearing;
        }

        public IStatement[] Statements { get; set; }

        public bool IsLoop { get; }

        public bool IsClearing { get; set; }

        public bool IsBreaked { get; private set; }

        public void Execute(params Argument[] arguments)
        {
            Eval(arguments);
        }

        public override Value Eval(params Argument[] arguments)
        {
            if (Statements == null)
            {
                return null;
            }

            IsBreaked = false;

            Value result = null;

            for (int i = 0; i < Statements.Length; i++)
            {
                if (IsBreaked)
                {
                    break;
                }

                var statement = Statements[i];

                switch (statement)
                {
                    case ReturnExpression returnExpression:
                        result = returnExpression.Value.Eval();
                        IsBreaked = true;
                        break;
                    case BreakExpression breakExpression:
                        breakExpression.IsLoop = IsLoop;
                        breakExpression.Execute();

                        if (breakExpression.IsBreaked)
                        {
                            i = Statements.Length;
                        }

                        break;
                    case AwaitExpression awaitExpression:
                        awaitExpression.Execute();
                        break;
                    case AssignExpression assignExpression:
                        assignExpression.Eval();
                        break;
                    case BinaryOperatorExpression binaryOperatorExpression:
                        binaryOperatorExpression.Eval();
                        break;
                    case IDeclaratable declaratable:
                        declaratable.Declarate();
                        break;
                    case TryCatchExpression tryCatchExpression:
                        var value2 = tryCatchExpression.Eval();

                        if (IsLoop && (tryCatchExpression.TryExpression.IsBreaked || tryCatchExpression.CatchExpression.IsBreaked))
                        {
                            IsBreaked = true;
                            break;
                        }

                        if (value2 != null)
                        {
                            IsBreaked = true;
                            return value2;
                        }

                        break;
                    case IfExpression ifExpression:
                        var value = ifExpression.Eval();

                        if (IsLoop && (ifExpression.BlockStatement.IsBreaked || ifExpression.ElseBlockStatement.IsBreaked))
                        {
                            IsBreaked = true;
                            break;
                        }

                        if (value != null)
                        {
                            IsBreaked = true;
                            return value;
                        }

                        break;
                    case FunctionCallExpression functionCallExpression:
                        functionCallExpression.Execute();
                        break;
                    case ForExpression forExpression:
                        var variable = forExpression.Variable as AssignExpression;

                        if (variable != null)
                        {
                            Variables.Instance.Value.Set(forExpression.ExpressionInfo.Parent, (variable.Left as VariableExpression).Name, variable.Right.Eval());
                        }

                        var result2 = forExpression.Eval();

                        Variables.Instance.Value.Remove(forExpression.ExpressionInfo.Parent, (variable.Left as VariableExpression).Name);

                        if (result2 is not NoneValue)
                        {
                            IsBreaked = true;
                            return result2;
                        }

                        break;
                }
            }

            if (IsClearing)
            {
                Variables.Instance.Value.Clear(this);
                Functions.Instance.Value.Clear(this);
                Interpreter.Data.Enums.Instance.Value.Clear(this);
                Interpreter.Data.Structures.Instance.Value.Clear(this);
            }

            return result;
        }
    }
}
