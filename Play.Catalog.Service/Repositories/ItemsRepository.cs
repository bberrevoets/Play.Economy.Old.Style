﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Play.Catalog.Service.Entities;
using Play.Economy.ServiceDefaults;

namespace Play.Catalog.Service.Repositories;

public class ItemsRepository
{
    private const string CollectionName = "items";
    private readonly IMongoCollection<Item> _dbCollection;
    private readonly FilterDefinitionBuilder<Item> _filterBuilder = Builders<Item>.Filter;

    public ItemsRepository()
    {
        var credential = MongoCredential.CreateCredential("admin", "admin", Constants.MongoDbPassword);
        var settings = MongoClientSettings.FromConnectionString("mongodb://localhost:27017");
        settings.Credential = credential;
        var mongoClient = new MongoClient(settings);
        var database = mongoClient.GetDatabase("Catalog");
        _dbCollection = database.GetCollection<Item>(CollectionName);
    }

    public async Task<IReadOnlyCollection<Item>> GetAllAsync()
    {
        return await _dbCollection.Find(_filterBuilder.Empty).ToListAsync();
    }

    public async Task<Item> GetAsync(Guid id)
    {
        var filter = _filterBuilder.Eq(entity => entity.Id, id);
        return await _dbCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Item entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        await _dbCollection.InsertOneAsync(entity);
    }
    
    public async Task UpdateAsync(Item entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        var filter = _filterBuilder.Eq(existingEntity => existingEntity.Id, entity.Id);
        await _dbCollection.ReplaceOneAsync(filter, entity);
    }

    public async Task RemoveAsync(Guid id)
    {
        var filter = _filterBuilder.Eq(entity => entity.Id, id);
        await _dbCollection.DeleteOneAsync(filter);
    }
}