﻿using IO.Milvus.Client.REST;
using IO.Milvus.Diagnostics;
using System.Net.Http;
using System.Text.Json.Serialization;

namespace IO.Milvus.ApiSchema;

/// <summary>
/// Create a partition.
/// </summary>
internal sealed class CreatePartitionRequest
{
    /// <summary>
    /// Collection name.
    /// </summary>
    [JsonPropertyName("collection_name")]
    public string CollectionName { get; set; }

    /// <summary>
    /// The partition name you want to create.
    /// </summary>
    [JsonPropertyName("partition_name")]
    public string PartitionName { get; set; }

    /// <summary>
    /// Database name
    /// </summary>
    /// <remarks>
    /// available in <c>Milvus 2.2.9</c>
    /// </remarks>
    [JsonPropertyName("db_name")]
    public string DbName { get; set; }

    internal static CreatePartitionRequest Create(
        string collectionName, 
        string partitionName,
        string dbName)
    {
        return new CreatePartitionRequest(collectionName, partitionName,dbName);
    }

    public Grpc.CreatePartitionRequest BuildGrpc()
    {
        return new Grpc.CreatePartitionRequest()
        {
            CollectionName = this.CollectionName,
            PartitionName = this.PartitionName,
            DbName = this.DbName,
        };
    }

    public HttpRequestMessage BuildRest()
    {
        return HttpRequest.CreatePostRequest(
            $"{ApiVersion.V1}/partition",
            payload: this
            );
    }

    public void Validate()
    {
        Verify.ArgNotNullOrEmpty(CollectionName, "Milvus collection name cannot be null or empty.");
        Verify.ArgNotNullOrEmpty(PartitionName, "Milvus partition name cannot be null or empty.");
        Verify.NotNullOrEmpty(DbName, "DbName cannot be null or empty");
    }

    #region Private ====================================================================
    private CreatePartitionRequest(string collectionName, string partitionName, string dbName)
    {
        this.CollectionName = collectionName;
        this.PartitionName = partitionName;
        this.DbName = dbName;
    }
    #endregion
}
