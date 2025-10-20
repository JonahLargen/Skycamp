using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Skycamp.Web.Constants;
using Skycamp.Web.Models;

namespace Skycamp.Web.Services;

public class WorkspaceStateService
{
    private readonly ProtectedLocalStorage _localStorage;

    public WorkspaceStateService(ProtectedLocalStorage localStorage)
    {
        _localStorage = localStorage;
    }

    public event Action<WorkspaceState?>? OnChange;

    public async Task<WorkspaceState?> GetSelectedWorkspaceAsync()
    {
        var result = await _localStorage.GetAsync<WorkspaceState>(LocalStorageKeys.ActiveWorkspace);

        return result.Success ? result.Value : null;
    }

    public async Task SetSelectedWorkspaceAsync(WorkspaceState workspace)
    {
        ArgumentNullException.ThrowIfNull(workspace);

        await _localStorage.SetAsync(LocalStorageKeys.ActiveWorkspace, workspace);

        OnChange?.Invoke(workspace);
    }

    public async Task RemoveSelectedWorkspaceAsync()
    {
        await _localStorage.DeleteAsync(LocalStorageKeys.ActiveWorkspace);

        OnChange?.Invoke(null);
    }
}