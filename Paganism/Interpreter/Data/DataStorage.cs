﻿using Paganism.Exceptions;
using Paganism.PParser.AST;
using System.Collections.Generic;

namespace Paganism.Interpreter.Data
{
    public abstract class DataStorage<T>
    {
        public abstract string Name { get; }

        private Dictionary<BlockStatementExpression, Dictionary<string, T>> Declarated { get; } = new Dictionary<BlockStatementExpression, Dictionary<string, T>>();

        private Dictionary<string, T> GlobalDeclarated { get; } = new Dictionary<string, T>();

        protected virtual IReadOnlyDictionary<string, T> Language { get; } = new Dictionary<string, T>();

        public void Add(BlockStatementExpression expression, string name, T value)
        {
            if (expression is null)
            {
                GlobalDeclarated[name] = value;
                return;
            }

            if (!Declarated.TryGetValue(expression, out _))
            {
                Declarated.Add(expression, new Dictionary<string, T>());
            }

            if (Declarated[expression].ContainsKey(name))
            {
                Declarated[expression][name] = value;
            }
            else
            {
                Declarated[expression].Add(name, value);
            }
        }

        public void Set(BlockStatementExpression expression, string name, T value)
        {
            if (expression is null)
            {
                GlobalDeclarated[name] = value;
                return;
            }

            if (!Declarated.TryGetValue(expression, out _))
            {
                Declarated.Add(expression, new Dictionary<string, T>());
                return;
            }

            Declarated[expression][name] = value;
        }

        public void Remove(BlockStatementExpression expression, string name)
        {
            if (expression is null)
            {
                _ = GlobalDeclarated.Remove(name);
                return;
            }

            if (!Declarated.TryGetValue(expression, out _))
            {
                Declarated.Add(expression, new Dictionary<string, T>());
            }

            _ = Declarated[expression].Remove(name);
        }

        public void Clear()
        {
            Declarated.Clear();
            GlobalDeclarated.Clear();
        }

        public void Clear(BlockStatementExpression expression)
        {
            if (expression is null)
            {
                GlobalDeclarated.Clear();
                return;
            }

            if (!Declarated.ContainsKey(expression))
            {
                return;
            }

            Declarated[expression].Clear();
        }

        public bool IsLanguage(string name)
        {
            return Language.ContainsKey(name);
        }

        public T Get(BlockStatementExpression expression, string name)
        {
            if (expression is null)
            {
                return Language.ContainsKey(name) ? Language[name] : GlobalDeclarated[name];
            }

            if (!Declarated.TryGetValue(expression, out _))
            {
                Declarated.Add(expression, new Dictionary<string, T>());
            }

            if (!Language.TryGetValue(name, out T result) && !Declarated[expression].TryGetValue(name, out _))
            {
                T value = Get(expression.Parent, name);

                return value != null ? value : throw new InterpreterException($"{Name} with '{name}' name not found");
            }

            return Declarated[expression].TryGetValue(name, out T result1) ? result1 : result;
        }
    }
}
