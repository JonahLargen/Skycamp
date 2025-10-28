namespace Skycamp.Web.Api;

public abstract class BaseApiClient
{
    protected static async Task<ApiResult> CreateApiResultAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return new ApiResult { IsSuccess = true };
        }
        else
        {
            var errorMessage = await response.Content.ReadAsStringAsync();

            return new ApiResult { IsSuccess = false, ErrorMessage = errorMessage ?? "An unknown error has occured." };
        }
    }

    protected static async Task<ApiDataResult<T>> CreateApiDataResultAsync<T>(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<T>();

            if (data == null)
            {
                return new ApiDataResult<T> { IsSuccess = false, ErrorMessage = "Failed to deserialize response data." };
            }

            return new ApiDataResult<T> { IsSuccess = true, Data = data };
        }
        else
        {
            var errorMessage = await response.Content.ReadAsStringAsync();

            return new ApiDataResult<T> { IsSuccess = false, ErrorMessage = errorMessage ?? "An unknown error has occured." };
        }
    }
}