using System.Diagnostics.CodeAnalysis;

namespace Skycamp.Web.Api;

public class ApiResult
{
    [MemberNotNullWhen(false, nameof(ErrorMessage))]
    public bool IsSuccess { get; init; }

    public string? ErrorMessage { get; init; }
}

public class ApiDataResult<T>
{
    [MemberNotNullWhen(true, nameof(Data))]
    [MemberNotNullWhen(false, nameof(ErrorMessage))]
    public bool IsSuccess { get; init; }

    public T? Data { get; init; }

    public string? ErrorMessage { get; init; }
}