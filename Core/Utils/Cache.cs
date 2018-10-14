/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode) */

using Agrishare.Core.Caching;
using Agrishare.Core.Entities;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Collections.Generic;

namespace Agrishare.Core.Utils
{

    public class Cache
    {
        #region Singleton

        private static Cache instance = null;
        private static readonly object padlock = new object();

        public static Cache Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                        instance = new Cache();
                    return instance;
                }
            }
        }

        #endregion
        
        #region Key Value

        public void Add(string Key, object Value)
        {
            Redis.Add(Key, JsonConvert.SerializeObject(Value));
        }

        public T Get<T>(string Key)
        {
            try
            {
                var json = Redis.Get(Key);
                if (!json.IsEmpty())
                    return JsonConvert.DeserializeObject<T>(json);
            }
            catch { }
            return default(T);
        }

        public bool Remove(string Key)
        {
            return Redis.Remove(Key);
        }

        #endregion

        #region List

        public void AddSet(string Key, List<IEntity> Values)
        {
            var entries = new HashEntry[] { };

            for (int i = 0; i < Values.Count; i++)
                entries[i] = new HashEntry(Values[i].Id, JsonConvert.SerializeObject(Values[i]));

            Redis.SetHash(Key, entries);
        }

        public bool RemoveSet(string Key)
        {
            return Redis.RemoveHash(Key);
        }

        public bool AddToSet(string Key, object Id, object Value)
        {
            return Redis.SetHashEntry(Key, Id.ToString(), JsonConvert.SerializeObject(Value));
        }

        public bool RemoveFromSet(string Key, object Id)
        {
            return Redis.RemoveHashEntry(Key, Id.ToString());
        }

        public List<T> GetSet<T>(string Key)
        {
            var objectList = new List<T>();

            var set = Redis.GetHash(Key);
            foreach(var item in set)
                objectList.Add(JsonConvert.DeserializeObject<T>(item.Value));
            return objectList;
        }

        public T GetFromSet<T>(string Key, object Id)
        {
            try
            {
                var json = Redis.GetHashEntry(Key, Id.ToString());
                if (!json.IsEmpty())
                    return JsonConvert.DeserializeObject<T>(json);
            }
            catch { }

            return default(T);
        }

        #endregion

        public void Clear()
        {
            Redis.Clear();
        }
    }
}
