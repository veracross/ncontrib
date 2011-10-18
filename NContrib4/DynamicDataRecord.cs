using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;

namespace NContrib4 {

    public class DynamicDataRecord : DynamicObject {

        private readonly Dictionary<string, object> _fields;

        /// <summary>Turn the field names into standard format property names.</summary>
        /// <param name="s">field name</param>
        /// <example>field_name => FieldName</example>
        /// <remarks>Field names ending in _xml and that begin with a &gt; are converted to <see cref="DynamicXElement"/></remarks>
        /// <returns></returns>
        private static string ToPropertyName(string s) {
            return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(s).Replace("_", "");
        }

        public DynamicDataRecord(IDataRecord dr) {
            _fields = Enumerable.Range(0, dr.FieldCount)
                .Select(i => GetRecord(i, dr))
                .ToDictionary(r => r.Key, r => r.Value);
        }

        public string[] GetFieldNames() {
            return _fields.Keys.ToArray();
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result) {
            return _fields.TryGetValue(binder.Name, out result);
        }

        // properties are not updatable
        public override bool TrySetMember(SetMemberBinder binder, object value) {
            return false;
        }

        public dynamic ToExpandoObject() {

            dynamic x = new ExpandoObject();
            var d = x as IDictionary<string, object>;

            foreach (var p in _fields)
                d[p.Key] = p.Value;

            return x;
        }

        protected static KeyValuePair<string, object> GetRecord(int i, IDataRecord dr) {

            var value = dr.GetValue(i);
            var name = dr.GetName(i);

            const string autoXmlSuffix = "_xml";

            if (name.EndsWith(autoXmlSuffix) && value.ToString().StartsWith("<")) {
                name = name.Substring(0, name.Length - autoXmlSuffix.Length);
                name = ToPropertyName(name);
                return new KeyValuePair<string, object>(name, new DynamicXElement(value.ToString()));
            }

            return new KeyValuePair<string, object>(ToPropertyName(name), value);
        }
    }
}