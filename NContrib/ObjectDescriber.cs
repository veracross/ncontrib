using System;
using System.Collections.Generic;
using System.Linq;
using NContrib.Extensions;

namespace NContrib {

    /// <summary>
    /// TODO: Build a nested list of keys and values instead of returning strings when it suits us, then format it
    /// TODO: Setup a clean system for resolving custom Describe methods, such as for arrays, dictionaries, and anything else
    /// </summary>
    public class ObjectDescriber {

        private int _previousNestLevel;

        private HashSet<string> _describedObjects;

        public int NestLimit { get; set; }

        public int IndentationSize { get; set; }

        public static object Defaults = new {NestLimit = 4, IndentationSize = 2, Redescribe = false};

        /// <summary>
        /// When true, previously described objects will be described again.
        /// When false, the object id will be printed instead
        /// </summary>
        public bool Redescribe { get; set; }

        public ObjectDescriber() {
            this.SetDefaults(Defaults);
        }

        public string Describe(object obj) {
            _previousNestLevel = 0;
            _describedObjects = new HashSet<string>();
            return Describe(obj, 0);
        }

        private string Describe(object obj, int nestLevel) {

            if (Redescribe && nestLevel < _previousNestLevel)
                _describedObjects = new HashSet<string>();

            if (_describedObjects == null)
                _describedObjects = new HashSet<string>();

            // record what nest level we're at so we can detect when we've started moving back UP the chain
            // this is needed so we can optionally re-print previously seen objects
            _previousNestLevel = nestLevel;

            if (obj == null)
                return "(null)";

            var type = obj.GetType();
            var objectId = type.Name + " <0x" + obj.GetHashCode().ToString("X") + ">";

            if (_describedObjects.Contains(objectId))
                return objectId;

            if (type == typeof(string))
                return obj.ToString();

            if (type.IsValueType)
                return obj.ToString();

            if (type.IsArray)
                return Describe(((object[])obj));

            var describer = type.GetMethod("Describe");

            if (describer != null)
                return describer.Invoke(obj, null) as string;

            _describedObjects.Add(objectId);

            if (nestLevel > NestLimit)
                return "(nest level exceeded)";

            var props = type
                .GetProperties()
                .Where(p => p.GetIndexParameters().Length == 0)
                .Select(p => new { p.Name, Value = p.GetValue(obj, null) })
                .ToDictionary(p => p.Name, p => Describe(p.Value, nestLevel + 1));

            // figure out how much space the property names take up so we can pad them properly
            var padLen = props.Keys.Select(k => k.ToString()).Max(x => x.Length);

            // format as Property => Value
            var formatted = props.Select(x => x.Key.ToString().PadRight(padLen) + " => " + x.Value).ToArray();

            // indent nested object descriptions
            var spacer = new String(' ', nestLevel * IndentationSize);

            return objectId + "\n" + (nestLevel > 0 ? spacer : "") + formatted.Join("\n" + spacer);
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
