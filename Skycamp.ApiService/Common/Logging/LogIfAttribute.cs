namespace Skycamp.ApiService.Common.Logging;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class LogIfAttribute : Attribute
{
    /// <summary>
    /// Name of a boolean property on the containing class/struct.
    /// If this property is true at runtime, the attributed property should be logged.
    /// </summary>
    public string ConditionPropertyName { get; }

    public LogIfAttribute(string conditionPropertyName)
    {
        ConditionPropertyName = conditionPropertyName;
    }
}