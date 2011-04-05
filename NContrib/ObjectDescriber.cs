using System;
using System.Collections.Generic;
using System.Linq;
using NContrib.Extensions;

namespace NContrib {

    public class ObjectDescriber {

        private readonly object _targetObject;

        private readonly HashSet<int> _describedObjects;

        private readonly int _nestLevel;

        private int _nestLimit;

        public static int DefaultNestLimit = 8;

        public static int NestIndentationSize = 2;

        public int NestLimit {
            get { return _nestLimit <= 0 ? DefaultNestLimit : _nestLimit; }
            set { _nestLimit = value; }
        }

        public ObjectDescriber(object obj, HashSet<int> describedObjects = null, int nestLevel = 0) {
            _targetObject = obj;
            _describedObjects = describedObjects ?? new HashSet<int>();
            _nestLevel = nestLevel;
        }

        public static string Describe(object obj) {
            return Describe(obj, null, 0);
        }

        private static string Describe(object obj, HashSet<int> describedObjects = null, int nestLevel = 0) {
            var od = new ObjectDescriber(obj, describedObjects, nestLevel);
            return od.Describe();
        }

        public string Describe() {

            if (_targetObject == null)
                return "(null)";

            var type = _targetObject.GetType();

            var desc = type.Name + " <0x" + _targetObject.GetHashCode().ToString("X") + ">";

            if (_describedObjects.Contains(_targetObject.GetHashCode()))
                return desc;

            if (_nestLevel > NestLimit)
                return "(nest level exceeded)";

            if (type == typeof(string))
                return _targetObject.ToString();

            if (type.IsValueType)
                return _targetObject.ToString();

            if (type.IsArray)
                return Describe(((object[]) _targetObject));
            
            var describer = type.GetMethod("Describe");

            if (describer != null)
                return describer.Invoke(_targetObject, null) as string;

            // log that we've seen this object so we don't describe it again
            _describedObjects.Add(_targetObject.GetHashCode());

            var props = type
                .GetProperties()
                .Where(p => p.GetIndexParameters().Length == 0)
                .Select(p => new { p.Name, Value = p.GetValue(_targetObject, null) })
                .ToDictionary(p => p.Name, p => Describe(p.Value, _describedObjects, _nestLevel + 1));

            // figure out how much space the property names take up so we can pad them properly
            var padLen = props.Keys.Select(k => k.ToString()).Max(x => x.Length);

            // format as Property => Value
            var formatted = props.Select(x => x.Key.ToString().PadRight(padLen) + " => " + x.Value);

            // indent nested object descriptions
            var spacer = new String(' ', _nestLevel * NestIndentationSize);

            return desc + "\n" + (_nestLevel > 0 ? spacer : "") + formatted.Join("\n" + spacer);
        }

        public static string Describe<T>(T[] source) {
            if (source == null)
                return "(null)";

            if (source.Length == 0)
                return "(empty)";

            return "[" + source.Select(x => x.ToString()).Join(", ") + "]";
        }

        /// <summary>
        /// Returns a formatted string showing the keys and values in this Dictionary for easy reading
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static string Describe<T1, T2>(IDictionary<T1, T2> dict) {
            if (dict == null)
                return "(null)";

            if (dict.Count == 0)
                return "(empty)";
            
            var padLen = dict.Keys.Select(k => k.ToString()).Max(x => x.Length);
            return dict.Select(x => x.Key.ToString().PadRight(padLen) + " => " + x.Value).Join("\n");
        }
    }
}
