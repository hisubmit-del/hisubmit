using Blazored.LocalStorage;

namespace ServerClientShared.Interfaces;

public class PrerenderLocalStorageService:ILocalStorageService
{
    public async ValueTask ClearAsync(CancellationToken? cancellationToken = null)
    {
        
    }

    public async ValueTask<T> GetItemAsync<T>(string key, CancellationToken? cancellationToken = null)
    {
        return default(T);
    }

    public ValueTask<string> GetItemAsStringAsync(string key, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public async ValueTask<string> KeyAsync(int index, CancellationToken? cancellationToken = null)
    {
        return string.Empty;
    }

    public ValueTask<IEnumerable<string>> KeysAsync(CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public async ValueTask<bool> ContainKeyAsync(string key, CancellationToken? cancellationToken = null)
    {
        return false;
    }

    public ValueTask<int> LengthAsync(CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public async ValueTask RemoveItemAsync(string key, CancellationToken? cancellationToken = null)
    {
        
    }

    public async ValueTask RemoveItemsAsync(IEnumerable<string> keys, CancellationToken? cancellationToken = null)
    {
       
    }

    public ValueTask SetItemAsync<T>(string key, T data, CancellationToken? cancellationToken = null)
    {
        var s = key;
        return ValueTask.CompletedTask;
    }

    public async ValueTask SetItemAsStringAsync(string key, string data, CancellationToken? cancellationToken = null)
    {
        
    }

    public event EventHandler<ChangingEventArgs>? Changing;
    public event EventHandler<ChangedEventArgs>? Changed;
}