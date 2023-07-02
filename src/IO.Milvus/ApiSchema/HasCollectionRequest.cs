﻿using IO.Milvus.Client.REST;
using IO.Milvus.Diagnostics;
using IO.Milvus.Utils;
using System;
using System.Net.Http;
using System.Text.Json.Serialization;

namespace IO.Milvus.ApiSchema;

/// <summary>
/// Get if a collection's existence
/// </summary>
internal sealed class HasCollectionRequest
{
    /// <summary>
    /// Collection Name
    /// </summary>
    /// <remarks>
    /// The unique collection name in milvus.(Required)
    /// </remarks>
    [JsonPropertyName("collection_name")]
    public string CollectionName { get; set; }

    /// <summary>
    /// Database name
    /// </summary>
    /// <remarks>
    /// available in <c>Milvus 2.2.9</c>
    /// </remarks>
    [JsonPropertyName("db_name")]
    public string DbName { get; set; }

    /// <summary>
    /// TimeStamp
    /// </summary>
    /// <remarks>
    /// If time_stamp is not zero, will return true when time_stamp >= created collection timestamp, otherwise will return false.
    /// </remarks>
    [JsonPropertyName("time_stamp")]
    public long Timestamp { get; set; }

    public static HasCollectionRequest Create(string collectionName, string dbName)
    {
        return new HasCollectionRequest(collectionName, dbName);
    }

    public HasCollectionRequest WithTimestamp(DateTime? dateTime)
    {
        Timestamp = dateTime == null ? 0 : dateTime.Value.ToUtcTimestamp();
        return this;
    }

    public Grpc.HasCollectionRequest BuildGrpc()
    {
        this.Validate();

        return new Grpc.HasCollectionRequest()
        {
            CollectionName = this.CollectionName,
            TimeStamp = (ulong)this.Timestamp,
            DbName = this.DbName
        };
    }

    public HttpRequestMessage BuildRest()
    {
        this.Validate();

        return HttpRequest.CreateGetRequest(
            $"{ApiVersion.V1}/collection/existence",
            this);
    }

    public void Validate()
    {
        Verify.ArgNotNullOrEmpty(CollectionName, "Milvus collection name cannot be null or empty");
        Verify.NotNullOrEmpty(DbName, "DbName cannot be null or empty");
    }

    #region Private =========================================================================================================
    private HasCollectionRequest(string collectionName, string dbName)
    {
        CollectionName = collectionName;
        this.DbName = dbName;
    }
    #endregion
}
