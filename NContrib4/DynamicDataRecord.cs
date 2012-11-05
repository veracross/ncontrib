using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using NContrib.Extensions;

namespace NContrib4 {

    public class DynamicDataRecord : DynamicObject {

        private readonly Dictionary<string, object> _fields;

        public DynamicDataRecord(IDataRecord dr, bool convertDbNull) {
            _fields = Enumerable.Range(0, dr.FieldCount)
                .Select(i => GetRecord(i, dr, convertDbNull))
                .ToDictionary(r => r.Key, r => r.Value);
        }

        public string[] GetFieldNames() {
            return _fields.Keys.ToArray();
        }

        public override IEnumerable<string> GetDynamicMemberNames() {
            return GetFieldNames().Select(ToPropertyName);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result) {
            return _fields.TryGetValue(binder.Name.ToSnakeCase().ToLower(), out result);
        }

        // properties are not updatable
        public override bool TrySetMember(SetMemberBinder binder, object value) {
            return false;
        }

        public object Get(string fieldName) {
            return _fields[fieldName];
        }

        public dynamic ToExpandoObject() {

            dynamic x = new ExpandoObject();
            var d = x as IDictionary<string, object>;

            foreach (var p in _fields)
                d[ToPropertyName(p.Key)] = p.Value;

            return x;
        }

        protected static string ToPropertyName(string name) {
            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(name).Replace("_", "");
        }

        protected static KeyValuePair<string, object> GetRecord(int i, IDataRecord dr, bool convertDbNull) {

            var value = dr.GetValue(i, convertDbNull);
            var name = dr.GetName(i);

            const string autoXmlSuffix = "_xml";

            // turn xml fields into DynamicXElement
            if (name.EndsWith(autoXmlSuffix) && value.ToString().StartsWith("<")) {
                name = name.Substring(0, name.Length - autoXmlSuffix.Length);
                return new KeyValuePair<string, object>(name, new DynamicXElement(value.ToString()));
            }

            return new KeyValuePair<string, object>(name, value);
        }
    }
}
