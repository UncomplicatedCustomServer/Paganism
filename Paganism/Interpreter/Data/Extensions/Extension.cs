﻿using Paganism.Exceptions;
using Paganism.PParser;
using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;
using Paganism.PParser.Values;
using System.Collections.Generic;

#nullable enable
namespace Paganism.Interpreter.Data.Extensions
{
    internal class Extension
    {
        public static List<string> AllowedExtensions { get; set; } = new()
        {
            "StringExtension"
        };
        public static Dictionary<TypesType, string> TypeExtensionAssociation { get; } = new()
        {
            { TypesType.String, "StringExtension" },
            { TypesType.Char, "StringExtension" },
            { TypesType.Number, "NumberExtension" },
            { TypesType.Array, "ArrayExtension" }
        };
        public static Dictionary<string, dynamic> StringExtension { get; set; } = new()
        {
            { "Replace", new ExtensionExecutor(TypesType.String, "Replace", new Argument[]
                {
                    new("original", TypesType.String),
                    new("replace", TypesType.String)
                }, (VariableExpression Original, Argument[] Arguments) =>
                {
                    var result = Original.Evaluate();

                    if (result is VoidValue)
                    {
                        throw new InterpreterException($"Variable {Original.Name} cannot be null while using Variable Extensions!", Original.ExpressionInfo);
                    }
                    else
                    {
                        if (result is null)
                        {
                            throw new InterpreterException($"Variable {Original.Name} cannot be null while using Variable Extensions!", Original.ExpressionInfo);
                        }
                        else
                        {
                            return Value.Create(result.AsString().Replace(Arguments[0].Value.Evaluate().AsString(), Arguments[1].Value.Evaluate().AsString()));
                        }
                    }
                })
            },
            { "Split", new ExtensionExecutor(TypesType.String, "Split", new Argument[]
                {
                    new("char", TypesType.Char)
                }, (VariableExpression Original, Argument[] Arguments) =>
                {
                    var result = Original.Evaluate();

                    if (result is VoidValue)
                    {
                        throw new InterpreterException($"Variable {Original.Name} cannot be null while using Variable Extensions!", Original.ExpressionInfo);
                    }
                    else
                    {
                        if (result is null)
                        {
                            throw new InterpreterException($"Variable {Original.Name} cannot be null while using Variable Extensions!", Original.ExpressionInfo);
                        }
                        else
                        {
                            return Value.Create(result.AsString().Split(char.Parse(Arguments[0].Value.Evaluate().AsString())));
                        }
                    }
                })
            }
        };

        public static Dictionary<string, dynamic> ArrayExtension { get; set; } = new();

        public static dynamic? Get(Dictionary<string, dynamic> Base, string Name)
        {
            return Base[Name] ?? null;
        }

        public static bool TryGet(Dictionary<string, dynamic> Base, string Name, out dynamic? executor)
        {
            executor = Get(Base, Name);

            if (executor is null)
            {
                executor = null;
                return false;
            }

            return true;
        }
    }
}
