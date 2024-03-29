﻿using Paganism.API.Attributes;
using Paganism.Interpreter.Data.Instances;
using Paganism.PParser;
using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;
using Paganism.PParser.Values;
using Paganism.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Paganism.API
{
    public static class PaganismFromCSharp
    {
        public static object Create(Type type, ref List<object> importedTypes)
        {
            var value = Value.Create(type);

            importedTypes ??= new List<object>();

            if (value is not NoneValue)
            {
                return value;
            }

            if (type.IsEnum)
            {
                var enumType = CreateEnum(type);

                importedTypes.Add(enumType);

                return enumType;
            }

            if ((IsStructure(type) || type.IsClass) && !type.IsValueType)
            {
                return CreateStructureOrClass(type, importedTypes);
            }

            return null;
        }

        public static StructureValue CreateStructureOrClass(Type type, List<object> importedTypes)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase).
                Where(field => field.GetCustomAttribute<PaganismSerializable>() is not null);

            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase).
                Where(method => method.GetCustomAttribute<PaganismSerializable>() is not null &&
                method.GetParameters()[0].ParameterType.IsArray && method.GetParameters()[0].ParameterType == typeof(Argument[]));

            var members = new Dictionary<string, StructureMemberExpression>(fields.Count());

            for (int i = 0; i < fields.Count();i++)
            {
                var field = fields.ElementAt(i);

                var csharpType = Create(field.FieldType, ref importedTypes);

                var typeValue = csharpType is Value value ? value.GetTypeValue() : new TypeValue(new ExpressionInfo(), TypesType.Enum, (csharpType as EnumInstance).Name);

                if (!importedTypes.Contains(csharpType) && csharpType is not Value)
                {
                    importedTypes.Add(csharpType);
                }

                members.Add(field.Name, new StructureMemberExpression(new ExpressionInfo(),
                    type.Name, typeValue, field.Name, 
                    new StructureMemberInfo(false, null, false, true, false, false)));
            }

            for (int i = 0; i < methods.Count();i++)
            {
                var method = methods.ElementAt(i);

                var array = method.GetParameters();

                var arguments = new Argument[array.Length];

                for (int j = 0; j < array.Length; j++)
                {
                    var paramater = array[j];

                    var createdType = Create(paramater.ParameterType, ref importedTypes);

                    var argumentType = createdType is Value value ? value.GetTypeValue() : new TypeValue(new ExpressionInfo(), TypesType.Enum, (createdType as EnumInstance).Name);

                    arguments[j] = new Argument(paramater.Name, argumentType, null, true, paramater.ParameterType.IsArray);
                }

                var createdReturnType = Create(method.ReturnType, ref importedTypes);
                var returnType = createdReturnType is Value value2 ? value2.GetTypeValue() : new TypeValue(new ExpressionInfo(), TypesType.Enum, (createdReturnType as EnumInstance).Name);

                members.Add(method.Name, new StructureMemberExpression(new ExpressionInfo(),
                    type.Name, returnType, method.Name,
                    new StructureMemberInfo(true, arguments.ToArray(), true, true, false, false)));
            }

            var structure = new StructureValue(new ExpressionInfo(), type.Name, members);

            foreach (var method in methods)
            {
                var member = structure.Structure.Members[method.Name];

                var action = (Action) Delegate.CreateDelegate(typeof(Action), (object)type.GetType(), method.Name);

                structure.Values[method.Name] = new FunctionValue(new ExpressionInfo(), method.Name, member.Info.Arguments, member.Type,
                (Argument[] arguments) =>
                {
                    var types = new List<object>();

                    return Create(action.DynamicInvoke(arguments) as Type, ref types) as Value;
                });
            }

            return structure; 
        }

        public static EnumInstance CreateEnum(Type type)
        {
            if (!type.IsEnum)
            {
                return null;
            }

            var values = type.GetEnumValues();

            var members = new EnumMemberExpression[values.Length];

            System.Collections.IList list = values;
            for (int i = 0; i < list.Count; i++)
            {
                var value = list[i];
                var name = type.GetEnumName(value);

                var memberValue = new NumberValue(new ExpressionInfo(), (int)value);

                members[i] = new EnumMemberExpression(new ExpressionInfo(), name, memberValue, type.Name);
            }

            return new EnumInstance(new EnumDeclarateExpression(new ExpressionInfo(), type.Name, members, true));
        }

        public static bool IsStructure(Type source)
        {
            return source.IsValueType && !source.IsPrimitive && !source.IsEnum;
        }

        public static object ValueToObject(Value value)
        {
            switch (value)
            {
                case StringValue stringValue:
                    return stringValue.Value;
                case NumberValue numberValue:
                    return numberValue.Value;
                case BooleanValue booleanValue:
                    return booleanValue.Value;
                case ArrayValue arrayValue:
                    var array = new object[arrayValue.Elements.Length];

                    for (int i = 0; i < array.Length;i++)
                    {
                        array[i] = ValueToObject(value);
                    }

                    return array;
                case EnumValue enumValue:
                    return enumValue.Member.Enum;
            }

            return null;
        }
    }
}
