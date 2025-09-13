namespace Skycamp.ApiService.Common.Logging;

[AttributeUsage(AttributeTargets.Property)]
public class TruncateLogAttribute : Attribute
{
    public int MaxLength { get; }

    public TruncateLogAttribute(int maxLength)
    {
        MaxLength = maxLength;
    }
}