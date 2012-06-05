namespace NContrib {

    public static class RegexLibrary {

        public static class Dates {

            /// <summary>Restricted to the rather modern era. 1000 - 2999</summary>
            public const string Year4Digit = @"(?<year4digit>[12][0-9]{3})";

            /// <summary>Any 2 digits from 00 to 99</summary>
            public const string Year2Digit = @"(?<year2digit>[0-9]{2})";

            /// <summary>Two digits 01 to 12. Zero required.</summary>
            public const string Month = @"(?<month>0[1-9]|1[0-2])";

            /// <summary>Two digits 01 to 31. Zero required.</summary>
            public const string Day = @"(?<day>0[1-9]|[12][0-9]|3[01])";

            /// <summary>
            /// yyyy-MM-dd: Constrainted to year 1000 - 2999
            /// </summary>
            public const string Iso8601 = Year4Digit + "[-]" + Month + "[-]" + Day;
        }

        public static class NationalId {

            public const string DanishCprNumber = @"(?x)
                ^
                " + Dates.Day + Dates.Month + Dates.Year2Digit + @"
                (?<separator>[-])
                (?<sequence>[0-9]{4})
                $";

            public const string SwedishPersonNumber = @"(?x)
                ^
                (?<century>    18|19|2[0-9])?           # optional century. restrict to 1800 - 2999
                " + Dates.Year2Digit + Dates.Month + Dates.Day + @"
                (?<separator>  [-+]?)                   # separator. - for under 100 years old, + for over
                (?<sequence>   [0-9]{4})
                $";
        }

        /// <summary>
        /// A forgiving email address regex. Let email software handle real validation.
        /// </summary>
        public const string EmailAddress = @"\b[\w._%+-]+@[\w.-]+\.[a-z]{2,4}\b";

        public const string Url = @"((https?|ftp|gopher|telnet|file|notes|ms-help):((//)|(\\\\))+[\w\d:#@%/;$()~_?\+-=\\\.&]*)";

        public const string MimeType = @"(?ix)
            ^
            # List of known types and x-* customs
            (?<type>
                application|audio|chemical|example|image|message|model|multipart|text|video
                |
                x- \w+ (?:[\-+\._]{0,2} \w)?
            )
            [/]                         # Type/Subtype separator
            (?<subtype>
                \w+                     # Starts with a word char     
                (?:[\-+\._]{0,2} \w)+   # Continues with 0 - 2 non-word chars, followed by a word char
                                        # Allowing two non-word chars was needed for the likes of text/x-c++hdr
                [+]?                    # Optionally ends with a + (audio/amr-wb+)
            )$";

        
    }
}
