using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Dfn.Etl.Core
{
    public interface IMetadata : IEnumerable<KeyValuePair<string, object>>
    {
        void Set(string key, object value);
        object Get(string key);
        IEnumerable<KeyValuePair<string, object>> GetAll();
        IMetadata Clone();
        void SetAll(IMetadata metadata);
    }

    public class DictionaryMetadata : IMetadata
    {
        protected Dictionary<string, object> m_Values;
        public Dictionary<string, object> Values { get { return m_Values; }} 

        public DictionaryMetadata()
        {
            m_Values = new Dictionary<string, object>();
        }

        public DictionaryMetadata(IDictionary<string, object> dictionary)
        {
            m_Values = new Dictionary<string, object>(dictionary);
        }

        public void Set(string key, object value)
        {
            m_Values[key] = value;
        }

        public object Get(string key)
        {
            object value = null;
            if (!m_Values.TryGetValue(key, out value))
            {
                return null;
            }
            return value;
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            return Values.ToList();
        }

        public IMetadata Clone()
        {
            return new DictionaryMetadata(Values);
        }

        public void SetAll(IMetadata metadata)
        {
            if(metadata == null)
                return;

            foreach (var p in metadata.GetAll())
            {
                Set(p.Key, p.Value);
            }
        }

        public virtual IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return m_Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}