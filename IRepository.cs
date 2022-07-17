using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dnkh.TableStoreRepository;
public interface IRepository<T>
{
    Task<IEnumerable<T>?> GetAsync(int? skip, int? take);
    Task<T?> GetByGuidAsync(Guid id);
    Task<T?> UpsertAsync(T entity, string? rowKey = null, string? partitionKey = null);
    Task<T?> DeleteAsync(Guid id);
}
