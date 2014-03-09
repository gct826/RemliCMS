﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using RemliCMS.WebData.Entities;
using RemliCMS.WebData.Services;

namespace RemliCMS.WebData.Services
{
    public abstract class EntityService<T> : IEntityService<T> where T : IMongoEntity
    {
        protected readonly MongoConnectionHandler<T> MongoConnectionHandler;

        public virtual void Create(T entity)
        {
            //// Save the entity with safe mode (WriteConcern.Acknowledged)
            var result = this.MongoConnectionHandler.MongoCollection.Save(
                entity,
                new MongoInsertOptions
                {
                    WriteConcern = WriteConcern.Acknowledged
                });

            if (!result.Ok)
            {
                //// Something went wrong
            }
        }

        public virtual void Delete(string id)
        {
            var result = this.MongoConnectionHandler.MongoCollection.Remove(
                Query<T>.EQ(e => e.Id, new ObjectId(id)),
                RemoveFlags.None,
                WriteConcern.Acknowledged);

            if (!result.Ok)
            {
                //// Something went wrong
            }
        }

        protected EntityService()
        {
            MongoConnectionHandler = new MongoConnectionHandler<T>();
        }

        public virtual T GetById(string id)
        {
            var entityQuery = Query<T>.EQ(e => e.Id, new ObjectId(id));
            return this.MongoConnectionHandler.MongoCollection.FindOne(entityQuery);
        }

        public virtual void Update(T entity)
        {

            // Update not required.

            //// Save the entity with safe mode (WriteConcern.Acknowledged)
            //var query = Query<T>.EQ(e => e.Id, entity.Id);
            //var sortBy = SortBy.Null;

            var result = this.MongoConnectionHandler.MongoCollection.Save(
                entity,
                new MongoInsertOptions
                {
                    WriteConcern = WriteConcern.Acknowledged
                });

            if (!result.Ok)
            {
                //// Something went wrong
            }


        }
    }
}
