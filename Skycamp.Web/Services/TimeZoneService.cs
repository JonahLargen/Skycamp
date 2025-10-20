using Microsoft.JSInterop;
using TimeZoneConverter;

namespace Skycamp.Web.Services;

public class TimeZoneService
{
    private readonly IJSRuntime _jsRuntime;
    private TimeZoneInfo? _cachedTimeZone;
    private readonly SemaphoreSlim _initializationLock = new(1, 1);

    public TimeZoneService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task InitializeAsync()
    {
        // If already initialized, return immediately
        if (_cachedTimeZone != null)
            return;

        // Wait for the lock - only one caller proceeds at a time
        await _initializationLock.WaitAsync();
        try
        {
            // Double-check after acquiring lock (another thread may have initialized)
            if (_cachedTimeZone != null)
                return;

            try
            {
                var tzId = await _jsRuntime.InvokeAsync<string>(
                    "eval",
                    "Intl.DateTimeFormat().resolvedOptions().timeZone"
                );
                _cachedTimeZone = TZConvert.GetTimeZoneInfo(tzId);
            }
            catch
            {
                _cachedTimeZone = TimeZoneInfo.Utc;
            }
        }
        finally
        {
            _initializationLock.Release();
        }
    }


    public DateTime ConvertFromUtc(DateTime utcDateTime)
    {
        if (_cachedTimeZone == null)
            return utcDateTime; // Fallback if not initialized

        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, _cachedTimeZone);
    }

    public void Dispose()
    {
        _initializationLock?.Dispose();
    }
}