using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcTerminalService.Models
{
    /// <summary>
    /// Предоставляет функционал работы с БД
    /// </summary>
    public class DbHelper // TODO Generic
    {
        public DbHelper()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath));
        }

        readonly string collectionName = "InvoiceCollection";
        readonly string dbPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "data", "litedb.db");


        public T QueryEntryByNumber<T>(string id) where T : INumber, IDate, IId
        {
            using var db = new LiteDatabase(dbPath);
            var col = db.GetCollection<T>(collectionName);
            var results = col.Find(x => x.Number == id);
            var sorted = results.OrderByDescending(x => x.Id);

            if (sorted.Count() == 0)
                return default(T);
            else
                return sorted.First();
        }

        public void UpdateEntry<T>(T entry)
        {
            using var db = new LiteDatabase(dbPath);
            var col = db.GetCollection<T>(collectionName);
            col.Update(entry);
            db.Commit();
        }

        public string QueryMaxNumber(GroupName groupName)
        {
            using var db = new LiteDatabase(dbPath);
            var col = db.GetCollection<Invoice>(collectionName);
            var filtered = col.Find(x => x.Group == groupName);
            var sorted = filtered.OrderByDescending(x => x.Date);

            if (sorted.Count() == 0)
                return groupName + "0";
            else
                return sorted.First().Number;
        }

        public void SendEntry<T>(T data)
        {
            using var db = new LiteDatabase(dbPath);
            var col = db.GetCollection<T>(collectionName);
            col.Insert(data);
            db.Commit();
        }

        public List<T> QueryEntryesByGroup<T>(GroupName gropup) where T : IGroup, IId
        {
            using var db = new LiteDatabase(dbPath);
            var col = db.GetCollection<T>(collectionName);
            var filtered = col.Find(x => x.Group == gropup);
            var sorted = filtered.OrderByDescending(x => x.Id);

            if (sorted.Count() == 0)
                return null;
            else
                return sorted.ToList();
        }
    }
}
