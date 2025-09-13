using System.Collections;
using System.Collections.Concurrent;
using System.Dynamic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Skycamp.ApiService.Common.Logging;

/// <summary>
/// Helper class for converting objects to loggable property dictionaries
/// with support for property masking and exclusion.
/// </summary>
public static class LoggablePropertyHelper
{
    // Cache for property info to improve reflection performance
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertyCache = new();

    // Default max depth to prevent stack overflow on circular references
    private const int DefaultMaxDepth = 10;

    /// <summary>
    /// Gets a friendly type name, handling generics
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string GetFriendlyTypeName(Type type)
    {
        if (type == null)
            return "null";

        if (!type.IsGenericType)
            return type.Name;

        var genericTypeName = type.Name.Substring(0, type.Name.IndexOf('`'));
        var genericArgs = string.Join(", ", type.GetGenericArguments().Select(GetFriendlyTypeName));
        return $"{genericTypeName}<{genericArgs}>";
    }

    /// <summary>
    /// Gets a dictionary of loggable properties from an object
    /// </summary>
    public static object? GetLoggableProperties(object? obj)
    {
        return GetLoggableProperties(obj, DefaultMaxDepth);
    }

    /// <summary>
    /// Gets a dictionary of loggable properties with specified max recursion depth
    /// </summary>
    public static object? GetLoggableProperties(object? obj, int maxDepth)
    {
        // Use HashSet to track object references for circular reference detection
        return GetLoggablePropertiesInternal(obj, obj?.GetType(), maxDepth, new HashSet<object>(new ReferenceEqualityComparer()));
    }

    private static object? GetLoggablePropertiesInternal(object? obj, Type? type, int remainingDepth, HashSet<object> visitedObjects)
    {
        // Handle null objects or when max depth is reached
        if (obj == null || type == null || remainingDepth <= 0)
        {
            return obj == null ? null : "[Max depth exceeded]";
        }

        // Handle circular references
        if (!IsSimpleType(type) && obj is not string && !visitedObjects.Add(obj))
        {
            return "[Circular reference]";
        }

        try
        {
            // Simple types can be returned directly
            if (IsSimpleType(type))
            {
                return obj;
            }

            // Handle dynamic objects
            if (obj is ExpandoObject expandoObj)
            {
                var dynamicDict = new Dictionary<string, object?>();
                foreach (var kvp in (IDictionary<string, object?>)expandoObj)
                {
                    dynamicDict[kvp.Key] = GetLoggablePropertiesInternal(kvp.Value, kvp.Value?.GetType(),
                        remainingDepth - 1, visitedObjects);
                }
                return dynamicDict;
            }

            // Handle dictionaries
            if (obj is IDictionary dictionary)
            {
                var dictResult = new Dictionary<object, object?>();

                foreach (DictionaryEntry entry in dictionary)
                {
                    dictResult[entry.Key] = GetLoggablePropertiesInternal(entry.Value,
                        entry.Value?.GetType(), remainingDepth - 1, visitedObjects);
                }

                return dictResult;
            }

            // Handle collections (except strings)
            if (obj is IEnumerable enumerable && !(obj is string))
            {
                var result = new List<object?>();
                var count = 0;
                var maxItems = 1000; // Limit large collections

                foreach (var item in enumerable)
                {
                    if (count >= maxItems)
                    {
                        result.Add($"[Collection truncated. {count} total items]");
                        break;
                    }

                    result.Add(GetLoggablePropertiesInternal(item, item?.GetType(),
                        remainingDepth - 1, visitedObjects));
                    count++;
                }
                return result;
            }

            // Process complex object's properties
            var dict = new Dictionary<string, object?>();

            // Get cached properties or retrieve and cache them
            var properties = PropertyCache.GetOrAdd(type, t =>
                t.GetProperties(BindingFlags.Public | BindingFlags.Instance));

            foreach (var prop in properties)
            {
                try
                {
                    // Skip indexed properties or write-only properties
                    if (prop.GetIndexParameters().Length > 0 || !prop.CanRead)
                    {
                        continue;
                    }

                    // Skip properties marked with DoNotLog attribute
                    if (Attribute.IsDefined(prop, typeof(DoNotLogAttribute)))
                    {
                        continue;
                    }
                    // Mask properties with MaskLog attribute
                    else if (Attribute.IsDefined(prop, typeof(MaskLogAttribute)))
                    {
                        var maskAttr = (MaskLogAttribute?)Attribute.GetCustomAttribute(prop, typeof(MaskLogAttribute));
                        dict[prop.Name] = maskAttr?.Mask ?? "***";
                    }
                    // LogIf: only log if the runtime condition property is true
                    else if (Attribute.IsDefined(prop, typeof(LogIfAttribute)))
                    {
                        var logIfAttr = (LogIfAttribute?)Attribute.GetCustomAttribute(prop, typeof(LogIfAttribute));
                        if (logIfAttr != null)
                        {
                            var conditionProp = properties.FirstOrDefault(p => p.Name == logIfAttr.ConditionPropertyName);
                            bool conditionMet = false;
                            if (conditionProp != null)
                            {
                                var conditionValue = conditionProp.GetValue(obj);
                                if (conditionValue is bool b && b)
                                    conditionMet = true;
                            }
                            if (!conditionMet)
                                continue;

                            object? value = null;
                            try
                            {
                                value = prop.GetValue(obj);
                            }
                            catch (Exception ex)
                            {
                                dict[prop.Name] = $"[Error reading property: {ex.Message}]";
                                continue;
                            }
                            dict[prop.Name] = GetLoggablePropertiesInternal(value, prop.PropertyType, remainingDepth - 1, visitedObjects);
                        }
                    }
                    // TruncateLog: truncate enumerable property value if needed
                    else if (Attribute.IsDefined(prop, typeof(TruncateLogAttribute)))
                    {
                        var truncateAttr = (TruncateLogAttribute?)Attribute.GetCustomAttribute(prop, typeof(TruncateLogAttribute));
                        object? value = null;
                        try
                        {
                            value = prop.GetValue(obj);
                        }
                        catch (Exception ex)
                        {
                            dict[prop.Name] = $"[Error reading property: {ex.Message}]";
                            continue;
                        }

                        if (value is string strVal && truncateAttr != null && strVal.Length > truncateAttr.MaxLength)
                        {
                            dict[prop.Name] = strVal.Substring(0, truncateAttr.MaxLength) + "...[truncated]";
                        }
                        else if (value is IEnumerable enumerableVal && truncateAttr != null)
                        {
                            var list = new List<object?>();
                            int count = 0;
                            foreach (var item in enumerableVal)
                            {
                                if (count >= truncateAttr.MaxLength)
                                {
                                    list.Add($"[Collection truncated. {count} total items]");
                                    break;
                                }
                                list.Add(item);
                                count++;
                            }
                            dict[prop.Name] = list;
                        }
                        else
                        {
                            dict[prop.Name] = GetLoggablePropertiesInternal(value, prop.PropertyType,
                                remainingDepth - 1, visitedObjects);
                        }
                    }
                    else
                    {
                        // Get property value safely
                        object? value = null;
                        try
                        {
                            value = prop.GetValue(obj);
                        }
                        catch (Exception ex)
                        {
                            dict[prop.Name] = $"[Error reading property: {ex.Message}]";
                            continue;
                        }

                        dict[prop.Name] = GetLoggablePropertiesInternal(value, prop.PropertyType,
                            remainingDepth - 1, visitedObjects);
                    }
                }
                catch (Exception ex)
                {
                    dict[prop.Name] = $"[Error processing property: {ex.Message}]";
                }
            }

            return dict;
        }
        catch (Exception ex)
        {
            return $"[Error: {ex.Message}]";
        }
        finally
        {
            // Remove the object from visited set when done processing it
            if (obj != null && !IsSimpleType(type) && obj is not string)
            {
                visitedObjects.Remove(obj);
            }
        }
    }

    private static bool IsSimpleType(Type type)
    {
        var underlying = Nullable.GetUnderlyingType(type) ?? type;
        return underlying.IsPrimitive
            || underlying.IsEnum
            || underlying == typeof(string)
            || underlying == typeof(decimal)
            || underlying == typeof(DateTime)
            || underlying == typeof(Guid)
            || underlying == typeof(DateTimeOffset)
            || underlying == typeof(TimeSpan)
            || underlying == typeof(DateOnly)
            || underlying == typeof(TimeOnly)
            || underlying == typeof(Uri)
            || underlying == typeof(Version);
    }

    /// <summary>
    /// Equality comparer that uses reference equality instead of value equality
    /// </summary>
    private class ReferenceEqualityComparer : IEqualityComparer<object>
    {
        public new bool Equals(object? x, object? y) => ReferenceEquals(x, y);
        public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
    }
}