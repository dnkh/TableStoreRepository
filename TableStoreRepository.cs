using Microsoft.Azure.Cosmos.Table;

namespace dnkh.TableStoreRepository;

public class TableStoreRepository<T> : IRepository<T>
{
    private CloudTable? _cloudTable = null;
    private readonly TableStoreConfiguration _config;
    public TableStoreRepository(TableStoreConfiguration configuration)
    {
        _config = configuration;
    }

    private async Task<CloudTable> GetCloudTable()
    {
        if (_cloudTable is null)
        {
            CloudTableClient tableClient = new(new Uri(_config.StorageUri),
                new StorageCredentials(_config.SASToken));
            _cloudTable = tableClient.GetTableReference(_config.TableName);
            await _cloudTable.CreateIfNotExistsAsync();
        }

        return _cloudTable;
    }
    public virtual async Task<IEnumerable<T>?> GetAsync(int? skip = 0, int? take = 100)
    {
        var cloudTable = await GetCloudTable();

        var query = new TableQuery<TableEntityAdapter<T>>();
        int localTake = take ?? 100;
        int localSkip = skip ?? 0;
        query.TakeCount = localTake >= 1000 ? 1000 : localTake;
        var result = new List<TableEntityAdapter<T>>();

        TableContinuationToken token = null;
        do
        {
            var queryResult = await cloudTable.ExecuteQuerySegmentedAsync<TableEntityAdapter<T>>(query, token);
            result.AddRange(queryResult.Results);
            token = queryResult.ContinuationToken;
        } while (token != null);

        return result.Select(x => x.OriginalEntity).Skip(localSkip).Take(localTake);
    }

    public virtual async Task<T?> GetByGuidAsync(Guid id)
    {
        return (await GetByStringPropertyAsync("RowKey", id.ToString())).FirstOrDefault();
    }

    public virtual async Task<IEnumerable<T?>> GetByGuidPropertyAsync(string popertyName, Guid value)
    {
        var cloudTable = await GetCloudTable();
        var queryResult = cloudTable.ExecuteQuery(new TableQuery<TableEntityAdapter<T>>().Where(TableQuery.GenerateFilterConditionForGuid(popertyName, QueryComparisons.Equal, value)));

        return queryResult.Select(x => x.OriginalEntity);
    }
    public virtual async Task<IEnumerable<T?>> GetByIntPropertyAsync(string popertyName, int value)
    {
        var cloudTable = await GetCloudTable();
        var queryResult = cloudTable.ExecuteQuery(new TableQuery<TableEntityAdapter<T>>().Where(TableQuery.GenerateFilterConditionForInt(popertyName, QueryComparisons.Equal, value)));

        return queryResult.Select(x => x.OriginalEntity);
    }
    public virtual async Task<IEnumerable<T?>> GetByStringPropertyAsync(string popertyName, string value)
    {
        var cloudTable = await GetCloudTable();
        var queryResult = cloudTable.ExecuteQuery(new TableQuery<TableEntityAdapter<T>>().Where(TableQuery.GenerateFilterCondition(popertyName, QueryComparisons.Equal, value)));

        return queryResult.Select(x => x.OriginalEntity);
    }

    
    public virtual async Task<T?> UpsertAsync(T entity, string? rowKey = null, string? partitionKey = null)
    {
        var cloudTable = await GetCloudTable();
        var tableEntity = new TableEntityAdapter<T>(entity);
        if (partitionKey is null)
        {
            partitionKey = _config.PartitionKey;
        }
        if (rowKey is null)
        {
            rowKey = Guid.NewGuid().ToString();
        }
        tableEntity.PartitionKey = partitionKey;
        tableEntity.RowKey = rowKey;
        await cloudTable.ExecuteAsync(TableOperation.InsertOrReplace(tableEntity));

        return entity;
    }
    public virtual async Task<T?> DeleteAsync(Guid id)
    {
        var cloudTable = await GetCloudTable();
        var queryResult = cloudTable.ExecuteQuery(new TableQuery<TableEntityAdapter<T>>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, id.ToString())));
        var entity = queryResult.FirstOrDefault();
        if (entity is null)
        {
            return default(T?);
        }

        await cloudTable.ExecuteAsync(TableOperation.Delete(entity));

        return entity.OriginalEntity;
    }
}