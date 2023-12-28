using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dnkh.TableStoreRepository;

public record TableStoreConfiguration
{
    public  string StorageUri { get; set; }
    public  string SASToken { get; set; }
    public  string TableName { get; set; }
    public  string PartitionKey { get; set; }
}
