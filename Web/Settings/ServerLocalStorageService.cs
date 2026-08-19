using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blazored.LocalStorage;

namespace Web.Settings;

public class ServerLocalStorageService : ILocalStorageService
{
    public ValueTask ClearAsync(CancellationToken cancellationToken = default)
    {
         return ValueTask.CompletedTask;
    }

    public ValueTask<T?> GetItemAsync<T>(string key, CancellationToken cancellationToken = default)
    {              
        return ValueTask.FromResult<T?>(default);
    }

    public ValueTask<string?> GetItemAsStringAsync(string key, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult<string?>(null);
    }

    public ValueTask<string?> KeyAsync(int index, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult<string?>(null);
    }

    public ValueTask<IEnumerable<string>> KeysAsync(CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult<IEnumerable<string>>(Array.Empty<string>());
    }

    public ValueTask<bool> ContainKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(false);
    }

    public ValueTask<int> LengthAsync(CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(0);
    }

    public ValueTask RemoveItemAsync(string key, CancellationToken cancellationToken = default)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask RemoveItemsAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask SetItemAsync<T>(string key, T data, CancellationToken cancellationToken = default)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask SetItemAsStringAsync(string key, string data, CancellationToken cancellationToken = default)
    {
        return ValueTask.CompletedTask;
    }

    public event EventHandler<ChangingEventArgs>? Changing;
    public event EventHandler<ChangedEventArgs>? Changed;
}
