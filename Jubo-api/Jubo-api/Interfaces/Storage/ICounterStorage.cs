namespace jubo_api.Interfaces.Storage;

public interface ICounterStorage
{
    ValueTask<int> GetNextSequenceValueAsync(string name);
}