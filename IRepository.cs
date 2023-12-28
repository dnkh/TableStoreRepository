using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dnkh.TableStoreRepository;
public interface IRepository<T>
{
    Task<IEnumerable<T>?> GetAsync(int? skip, int? take);
    Task<T?> GetByGuidAsync(Guid id);
    Task<IEnumerable<T?>> GetByGuidPropertyAsync(string popertyName, Guid value);
    Task<IEnumerable<T?>> GetByIntPropertyAsync(string popertyName, int value);
    Task<IEnumerable<T?>> GetByStringPropertyAsync(string popertyName, string value);
    Task<T?> UpsertAsync(T entity, string? rowKey = null, string? partitionKey = null);
    Task<T?> DeleteAsync(Guid id);
}
