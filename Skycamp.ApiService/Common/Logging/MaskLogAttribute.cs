namespace Skycamp.ApiService.Common.Logging;

[AttributeUsage(AttributeTargets.Property)]
public class MaskLogAttribute : Attribute
{
    public string Mask { get; }

    public MaskLogAttribute(string mask = "***")
    {
        Mask = mask;
    }
}