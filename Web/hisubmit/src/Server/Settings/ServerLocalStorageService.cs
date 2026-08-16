using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blazored.LocalStorage;

namespace HiSubmit.Server.Settings;

public class ServerLocalStorageService : ILocalStorageService
{
    public ValueTask ClearAsync(CancellationToken cancellationToken = default)
    {
         return ValueTask.CompletedTask;
    }

    public async ValueTask<T> GetItemAsync<T>(string key, CancellationToken cancellationToken = default)
    {              
        return default(T);
    }

    public async ValueTask<string> GetItemAsStringAsync(string key, CancellationToken cancellationToken = default)
    {
        return string.Empty;
    }

    public async ValueTask<string> KeyAsync(int index, CancellationToken cancellationToken = default)
    {
        return string.Empty;
    }

    public async ValueTask<IEnumerable<string>> KeysAsync(CancellationToken cancellationToken = default)
    {
        return new List<string>();
    }

    public async ValueTask<bool> ContainKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        return false;
    }

    public async ValueTask<int> LengthAsync(CancellationToken cancellationToken = default)
    {
        return 0;
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

    public event EventHandler<ChangingEventArgs> Changing;
    public event EventHandler<ChangedEventArgs> Changed;
}