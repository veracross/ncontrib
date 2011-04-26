using System.Linq;
using System.Text.RegularExpressions;

namespace NContrib.Culture {

    public class EnglishInflector : IInflector {

        protected static readonly string[,] Plural = new[,] {
            { @"(quiz)$",                                           "$1zes"  },
            { @"^(ox)$",                                            "$1en"   },
            { @"([m|l])ouse$",                                      "$1ice"  },
            { @"(matr|vert|ind)ix|ex$",                             "$1ices" },
            { @"(x|ch|ss|sh)$",                                     "$1es"   },
            { @"([^aeiouy]|qu)y$",                                  "$1ies"  },
            { @"(hive)$",                                           "$1s"    },
            { @"(?:([^f])fe|([lr])f)$",                             "$1$2ves"},
            { @"sis$",                                              "ses"    },
            { @"([ti])um$",                                         "$1a"    },
            { @"(buffal|tomat)o$",                                  "$1oes"  },
            { @"(bu)s$",                                            "$1ses"  },
            { @"(alias|status)$",                                   "$1es"   },
            { @"(octop|vir|alumn|cact|nucle|rad|stimul|fung)us$",   "$1i"    },
            { @"(antenn|foruml|nebul|vertebr|vit)a$",               "$1ae"   },
            { @"(ax|test)is$",                                      "$1es"   },
            { @"s$",                                                "s"      },
            { @"$",                                                 "s"      },
        };

        protected static readonly string[,] Singular = new[,] {
            { @"(quiz)zes$",                                                    "$1"     },
            { @"(matr)ices$",                                                   "$1ix"   },
            { @"(vert|ind)ices$",                                               "$1ex"   },
            { @"^(ox)en",                                                       "$1"     },
            { @"(alias|status)es$",                                             "$1"     },
            { @"(octop|vir|alumn|cact|nucle|rad|stimul|fung)i$",                "$1us"   },
            { @"(cris|ax|test)es$",                                             "$1is"   },
            { @"(shoe)s$",                                                      "$1"     },
            { @"(o)es$",                                                        "$1"     },
            { @"(bus)es$",                                                      "$1"     },
            { @"([m|l])ice$",                                                   "$1ouse" },
            { @"(x|ch|ss|sh)es$",                                               "$1"     },
            { @"(m)ovies$",                                                     "$1ovie" },
            { @"(s)eries$",                                                     "$1eries"},
            { @"([^aeiouy]|qu)ies$",                                            "$1y"    },
            { @"([lr])ves$",                                                    "$1f"    },
            { @"(tive)s$",                                                      "$1"     },
            { @"(hive)s$",                                                      "$1"     },
            { @"([^f])ves$",                                                    "$1fe"   },
            { @"(^analy)ses$",                                                  "$1sis"  },
            { @"((a)naly|(b)a|(d)iagno|(p)arenthe|(p)rogno|(s)ynop|(t)he)ses$", "$1$2sis"},
            { @"([ti])a$",                                                      "$1um"   },
            { @"(n)ews$",                                                       "$1ews"  },
            { @"(antenn|foruml|nebul|vertebr|vit)ae$",                          "$1a"    },
            { @"s$",                                                            ""       },
        };

        protected static readonly string[,] Irregular = new[,] {
            { "move", "moved", },
            { "sex", "sexes", },
            { "child", "children" },
            { "man", "men" },
            { "woman", "women" },
            { "person", "people" },
            { "goose", "geese" },
            { "criterion", "criteria" },
            { "datum", "data" },
            { "leaf", "leaves" },
            { "loaf", "loaves" },
            { "elf", "elves" },
            { "hoof", "hooves" },
            { "dwarf", "dwarfs", },
            { "tooth", "teeth" },
            { "foot", "feet" },
        };

        protected static readonly string[] Uncountable = new[] {
            "sheep",
            "fish",
            "series",
            "species",
            "money",
            "rice",
            "information",
            "equipment",
            "moose",
            "deer",
        };
        
        public string ToPlural(string word) {
            if (Uncountable.Contains(word.ToLower()))
                return word;

            for (var i = 0; i <= Irregular.GetUpperBound(0); i++) {
                var singular = Irregular[i, 0];
                var plural = Irregular[i, 1];
                if (word.ToLower() == singular || word.ToLower() == plural)
                    return plural;
            }

            for (var i = 0; i <= Plural.GetUpperBound(0); i++) {
                var regex = Plural[i, 0];
                var replace = Plural[i, 1];
                if (Regex.IsMatch(word.ToLower(), regex, RegexOptions.IgnoreCase))
                    return Regex.Replace(word, regex, replace);
            }

            return word;
        }

        public string ToPlural(string word, int number) {
            return number == 1 ? word : ToPlural(word);
        }

        public string ToSingular(string word) {
            if (Uncountable.Contains(word.ToLower()))
                return word;

            for (var i = 0; i <= Irregular.GetUpperBound(0); i++) {
                var singular = Irregular[i, 0];
                var plural = Irregular[i, 1];
                if (word.ToLower() == singular || word.ToLower() == plural)
                    return singular;
            }

            for (var i = 0; i <= Singular.GetUpperBound(0); i++) {
                var regex = Singular[i, 0];
                var replace = Singular[i, 1];
                if (Regex.IsMatch(word.ToLower(), regex, RegexOptions.IgnoreCase))
                    return Regex.Replace(word, regex, replace);
            }

            return word;
        }

        public string ToSingular(string word, int number) {
            return number != 1 ? word : ToSingular(word);
        }
    }
}
