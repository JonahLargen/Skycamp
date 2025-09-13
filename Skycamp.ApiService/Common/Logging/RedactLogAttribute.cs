namespace Skycamp.ApiService.Common.Logging;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class RedactLogAttribute : MaskLogAttribute
{
    public RedactLogAttribute() : base("[REDACTED]")
    {
        
    }
}