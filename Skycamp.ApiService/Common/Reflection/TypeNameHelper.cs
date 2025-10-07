using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skycamp.ApiService.Common.Reflection;

public static class TypeNameHelper
{
    private static readonly Dictionary<Type, string> BuiltInTypeNames = new()
    {
        { typeof(bool), "bool" },
        { typeof(byte), "byte" },
        { typeof(sbyte), "sbyte" },
        { typeof(char), "char" },
        { typeof(decimal), "decimal" },
        { typeof(double), "double" },
        { typeof(float), "float" },
        { typeof(int), "int" },
        { typeof(uint), "uint" },
        { typeof(long), "long" },
        { typeof(ulong), "ulong" },
        { typeof(object), "object" },
        { typeof(short), "short" },
        { typeof(ushort), "ushort" },
        { typeof(string), "string" },
        { typeof(void), "void" }
    };

    public static string GetFriendlyName(Type? type)
    {
        if (type == null)
        {
            return "null";
        }

        // Handle built-in types with C# keywords
        if (BuiltInTypeNames.TryGetValue(type, out var builtInName))
        {
            return builtInName;
        }

        // Handle nullable types
        if (IsNullableValueType(type))
        {
            var underlyingType = Nullable.GetUnderlyingType(type)!;
            return $"{GetFriendlyName(underlyingType)}?";
        }

        // Handle arrays
        if (type.IsArray)
        {
            var elementType = type.GetElementType()!;
            var rankSpecifier = new string(',', type.GetArrayRank() - 1);
            return $"{GetFriendlyName(elementType)}[{rankSpecifier}]";
        }

        // Handle generic types
        if (type.IsGenericType)
        {
            return FormatGenericType(type);
        }

        // Handle nested types
        if (type.IsNested)
        {
            return $"{GetFriendlyName(type.DeclaringType!)}.{type.Name}";
        }

        return type.Name;
    }

    private static string FormatGenericType(Type type)
    {
        var genericTypeDefinition = type.GetGenericTypeDefinition();
        var typeName = genericTypeDefinition.Name;

        // Remove the backtick and number (e.g., "List`1" -> "List")
        var backtickIndex = typeName.IndexOf('`');
        if (backtickIndex > 0)
        {
            typeName = typeName.Substring(0, backtickIndex);
        }

        var genericArguments = type.GetGenericArguments();

        // Handle nested generic types by only taking the arguments that belong to this level
        var ownGenericArgumentCount = genericTypeDefinition.GetGenericArguments().Length;
        var ownGenericArguments = genericArguments.TakeLast(ownGenericArgumentCount);

        var argumentNames = ownGenericArguments.Select(GetFriendlyName);
        var argumentsString = string.Join(", ", argumentNames);

        // Handle nested types in generic context
        var declaringTypeName = string.Empty;
        if (type.IsNested && type.DeclaringType != null)
        {
            declaringTypeName = $"{GetFriendlyName(type.DeclaringType)}.";
        }

        return $"{declaringTypeName}{typeName}<{argumentsString}>";
    }

    private static bool IsNullableValueType(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }
}