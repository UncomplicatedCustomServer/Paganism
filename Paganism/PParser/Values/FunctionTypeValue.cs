﻿using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;
using Paganism.PParser.Values.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.PParser.Values
{
    public class FunctionTypeValue : TypeValue, ISettable
    {
        public FunctionTypeValue(ExpressionInfo info, TypesType value, string typeName, Argument[] arguments, bool isAsync) : base(info, value, typeName)
        {
            Arguments = arguments;

            ReturnType = new TypeValue(info, value, typeName);
            IsAsync = isAsync;
        }

        public FunctionTypeValue(ExpressionInfo info, TypeValue typeValue, Argument[] arguments, bool isAsync) : this(info, typeValue.Value, typeValue.TypeName, arguments, isAsync) { }

        public FunctionTypeValue(ExpressionInfo info, FunctionDeclarateExpression functionDeclarateExpression, bool isAsync) : this(info, functionDeclarateExpression.ReturnType, functionDeclarateExpression.Arguments, isAsync) { }

        public bool IsAsync { get; private set; }

        public TypeValue ReturnType { get; private set; }

        public Argument[] Arguments { get; private set; }

        public bool CheckArguments(Argument[] arguments)
        {
            for (var i = 0; i < arguments.Length; i++)
            {
                var argument = Arguments[i];

                if (i > Arguments.Length - 1)
                {
                    return false;
                }

                if (!argument.Is(arguments[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public new string AsString()
        {
            var message = $"Function ({(IsAsync ? "async" : "not async")}) ({ReturnType}) (";

            foreach (var argument in Arguments)
            {
                message += argument.Type.ToString() + ", ";
            }

            return message + ")";
        }

        public new string ToString() => AsString();

        public new bool Is(TypeValue typeValue)
        {
            if (typeValue is not FunctionTypeValue functionTypeValue)
            {
                return false;
            }

            return ReturnType.Is(functionTypeValue.ReturnType) && CheckArguments(functionTypeValue.Arguments) && IsAsync == functionTypeValue.IsAsync;
        }

        public void Set(Value value)
        {
            if (value is not FunctionTypeValue functionTypeValue)
            {
                return;
            }

            IsAsync = functionTypeValue.IsAsync;
            ReturnType = functionTypeValue.ReturnType;
            Arguments = functionTypeValue.Arguments;
            Value = functionTypeValue.Value;
            TypeName = functionTypeValue.TypeName;
        }
    }
}
