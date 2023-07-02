﻿using IO.Milvus.Client.REST;
using IO.Milvus.Diagnostics;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Serialization;

namespace IO.Milvus.ApiSchema;

internal sealed class LoadPartitionsRequest
{
    /// <summary>
    /// Collection name in milvus
    /// </summary>
    [JsonPropertyName("collection_name")]
    public string CollectionName { get; set; }

    /// <summary>
    /// The partition names you want to load
    /// </summary>
    [JsonPropertyName("partition_names")]
    public IList<string> PartitionNames { get; set; }

    /// <summary>
    /// The replicas number you would load, 1 by default
    /// </summary>
    [JsonPropertyName("replica_number")]
    public int ReplicaNumber { get; set; } = 1;

    /// <summary>
    /// Database name
    /// </summary>
    /// <remarks>
    /// available in <c>Milvus 2.2.9</c>
    /// </remarks>
    [JsonPropertyName("db_name")]
    public string DbName { get; set; }

    public Grpc.LoadPartitionsRequest BuildGrpc()
    {
        var request = new Grpc.LoadPartitionsRequest()
        {
            CollectionName = this.CollectionName,
            ReplicaNumber = this.ReplicaNumber,
            DbName = this.DbName
        };
        request.PartitionNames.AddRange(PartitionNames);

        return request;
    }

    public void Validate()
    {
        Verify.ArgNotNullOrEmpty(CollectionName, "Milvus collection name cannot be null or empty.");
        Verify.NotNullOrEmpty(PartitionNames, "Partition names count must be greater than 1");
        Verify.True(ReplicaNumber >= 1, "Replica number must be greater than 1.");
        Verify.NotNullOrEmpty(DbName, "DbName cannot be null or empty");
    }

    public static LoadPartitionsRequest Create(string collectionName, string dbName)
    {
        return new LoadPartitionsRequest(collectionName, dbName);
    }

    public LoadPartitionsRequest WithPartitionNames(IList<string> partitionNames)
    {
        PartitionNames = partitionNames;
        return this;
    }

    public LoadPartitionsRequest WithReplicaNumber(int replicaNumber)
    {
        ReplicaNumber = replicaNumber;
        return this;
    }

    public HttpRequestMessage BuildRest()
    {
        return HttpRequest.CreatePostRequest(
            $"{ApiVersion.V1}/partitions/load",
            payload: this);
    }

    #region Private ==================================================
    public LoadPartitionsRequest(string collectionName, string dbName)
    {
        this.CollectionName = collectionName;
        this.DbName = dbName;
    }
    #endregion
}