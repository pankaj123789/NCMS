using System.Collections.Generic;
using System.Web;

namespace MyNaati.Ui.Common
{
    public class SessionStorage<TKey, TValue>
    {
        private static readonly string SessionKey = string.Format("{0}_{1}_{2}", typeof(TValue).Namespace, typeof(TValue).Name, typeof(TKey).Name);
        private Dictionary<TKey, TValue> InnerStore
        {
            get
            {
                var store = HttpContext.Current.Session[SessionKey] as Dictionary<TKey, TValue>;

                if (store != null)
                {
                    return store;
                }

                store = new Dictionary<TKey, TValue>();
                HttpContext.Current.Session[SessionKey] = store;

                return store;
            }
            set
            {
                HttpContext.Current.Session[SessionKey] = value;
            }
        }

        public void Put(TValue val, TKey key)
        {
            if (InnerStore == null)
            {
                InnerStore = new Dictionary<TKey, TValue>();
            }

            InnerStore[key] = val;
        }

        public bool ContainsKey(TKey key)
        {        
            return InnerStore.ContainsKey(key);
        }
        public TValue Get(TKey key)
        {
            if (InnerStore == null)
            {
                return default(TValue);
            }

            return InnerStore.ContainsKey(key) ? InnerStore[key] : default(TValue);
        }

        public void Delete(TKey key)
        {
            if (InnerStore != null && InnerStore.ContainsKey(key))
            {
                InnerStore.Remove(key);
            }
        }
    }
}