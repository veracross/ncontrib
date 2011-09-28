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
        /// <returns></returns>
        private static string ToPropertyName(string s) {
            return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(s).Replace("_", "");
        }

        public DynamicDataRecord(IDataRecord dr) {
            _fields = Enumerable.Range(0, dr.FieldCount)
                .ToDictionary(i => ToPropertyName(dr.GetName(i)), dr.GetValue);
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
    }
}