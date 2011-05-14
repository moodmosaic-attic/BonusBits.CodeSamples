using System;
using System.Collections.Generic;
using System.Dynamic;

    public sealed class DynamicEntity : DynamicObject
    {
        private readonly IDictionary<String, Object> m_dictionary
            = new Dictionary<String, Object>();

        private readonly String m_entityName;

        public DynamicEntity(String entityName)
        {
            m_entityName = entityName;
        }

        public String Name
        {
            get
            {
                return m_entityName;
            }
        }

        public IDictionary<String, Object> Map
        {
            get
            {
                return m_dictionary;
            }
        }

        public override Boolean TryGetMember(
            GetMemberBinder binder, out Object result)
        {
            if (!m_dictionary.TryGetValue(binder.Name, out result))
            {
                return false;
            }

            return true;
        }

        public override Boolean TrySetMember(
            SetMemberBinder binder, Object value)
        {
            String key = binder.Name;

            if (m_dictionary.ContainsKey(key))
            {
                m_dictionary.Remove(key);
            }

            m_dictionary.Add(key, value);

            return true;
        }
    }
