using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using LiteDB;

namespace Kontur.GameStats.Server.Data
{
    public static class LiteDatabaseExtensions
    {
        private static LiteCollection<TModel> GetCollection<TModel>(this LiteDatabase db)
        {
            var collectionName = typeof(TModel).Name;
            return db.GetCollection<TModel>(collectionName);
        }

        public static IEnumerable<TModel> Find<TModel>(this LiteDatabase db, Expression<Func<TModel, bool>> predicate)
        {
            return db.GetCollection<TModel>().Find(predicate);
        }

        public static TModel FindOne<TModel>(this LiteDatabase db, Expression<Func<TModel, bool>> predicate)
        {
            return db.GetCollection<TModel>().FindOne(predicate);
        }

        public static IEnumerable<TModel> FindAll<TModel>(this LiteDatabase db)
        {
            return db.GetCollection<TModel>().FindAll();
        }

        public static TModel FindById<TModel>(this LiteDatabase db, BsonValue id)
        {
            return db.GetCollection<TModel>().FindById(id);
        }

        public static void Insert<TModel>(this LiteDatabase db, TModel model)
        {
            db.GetCollection<TModel>().Insert(model);
        }

        public static void InsertOrUpdateById<TModel>(this LiteDatabase db, BsonValue id, TModel model)
        {
            var modelCollection = db.GetCollection<TModel>();
            if (!modelCollection.Update(id, model))
                modelCollection.Insert(id, model);
        }
    }
}
