using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using MediaBrowser.Model.Dto;
using Microsoft.AspNetCore.Http;

namespace MediaBrowser.Model.Services
{
    // Remove this garbage class, it's just a bastard copy of NameValueCollection
    public class QueryParamCollection : List<NameValuePair>
    {
        public QueryParamCollection()
        {

        }

        public QueryParamCollection(IHeaderDictionary headers)
        {
            foreach (var pair in headers)
            {
                Add(pair.Key, pair.Value);
            }
        }

        public QueryParamCollection(IQueryCollection queryCollection)
        {
            foreach (var pair in queryCollection)
            {
                Add(pair.Key, pair.Value);
            }
        }

        private static StringComparison GetStringComparison()
        {
            return StringComparison.OrdinalIgnoreCase;
        }

        private static StringComparer GetStringComparer()
        {
            return StringComparer.OrdinalIgnoreCase;
        }

        /// <summary>
        /// Adds a new query parameter.
        /// </summary>
        public virtual void Add(string key, string value)
        {
            if (string.Equals(key, "content-length", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            Add(new NameValuePair(key, value));
        }

        public virtual void Set(string key, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                var parameters = GetItems(key);

                foreach (var p in parameters)
                {
                    Remove(p);
                }

                return;
            }

            foreach (var pair in this)
            {
                var stringComparison = GetStringComparison();

                if (string.Equals(key, pair.Name, stringComparison))
                {
                    pair.Value = value;
                    return;
                }
            }

            Add(key, value);
        }

        public string Get(string name)
        {
            var stringComparison = GetStringComparison();

            foreach (var pair in this)
            {
                if (string.Equals(pair.Name, name, stringComparison))
                {
                    return pair.Value;
                }
            }

            return null;
        }

        public virtual List<NameValuePair> GetItems(string name)
        {
            var stringComparison = GetStringComparison();

            var list = new List<NameValuePair>();

            foreach (var pair in this)
            {
                if (string.Equals(pair.Name, name, stringComparison))
                {
                    list.Add(pair);
                }
            }

            return list;
        }

        public virtual List<string> GetValues(string name)
        {
            var stringComparison = GetStringComparison();

            var list = new List<string>();

            foreach (var pair in this)
            {
                if (string.Equals(pair.Name, name, stringComparison))
                {
                    list.Add(pair.Value);
                }
            }

            return list;
        }

        public Dictionary<string, string> ToDictionary()
        {
            var stringComparer = GetStringComparer();

            var headers = new Dictionary<string, string>(stringComparer);

            foreach (var pair in this)
            {
                headers[pair.Name] = pair.Value;
            }

            return headers;
        }

        public IEnumerable<string> Keys
        {
            get
            {
                var keys = new string[this.Count];

                for (var i = 0; i < keys.Length; i++)
                {
                    keys[i] = this[i].Name;
                }

                return keys;
            }
        }

        /// <summary>
        /// Gets or sets a query parameter value by name. A query may contain multiple values of the same name
        /// (i.e. "x=1&amp;x=2"), in which case the value is an array, which works for both getting and setting.
        /// </summary>
        /// <param name="name">The query parameter name</param>
        /// <returns>The query parameter value or array of values</returns>
        public string this[string name]
        {
            get => Get(name);
            set => Set(name, value);
        }

        private string GetQueryStringValue(NameValuePair pair)
        {
            return pair.Name + "=" + pair.Value;
        }

        public override string ToString()
        {
            var vals = this.Select(GetQueryStringValue).ToArray();

            return string.Join("&", vals);
        }
    }
}
