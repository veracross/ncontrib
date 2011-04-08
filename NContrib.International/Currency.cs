using System;
using System.Globalization;
using System.Text.RegularExpressions;
using NContrib.Extensions;

namespace NContrib.International {

    public class Currency {

        public static readonly CurrencyCollection BuiltInCurrencies = new CurrencyCollection {
            {"AED", "784", 2, "United Arab Emirates Dirham", "ar-AE"},
            {"AFN", "971", 2, "Afghani", "ps-AF"},
            {"ALL", "008", 2, "Albanian Lek", "sq-AL"},
            {"AMD", "051", 2, "Armenian Dram", "hy-AM"},
            {"ANG", "532", 2, "Netherlands Antillean Guilder"},
            {"AOA", "973", 2, "Angolan Kwanza"},
            {"ARS", "032", 2, "Argentine Peso", "es-AR"},
            {"AUD", "036", 2, "Australian Dollar", "en-AU"},
            {"AWG", "533", 2, "Aruban Guilder"},
            {"AZN", "944", 2, "Azerbaijanian Manat", "az-Latn-AZ"},
            {"BAM", "977", 2, "Bosnia and Herzegovina convertible mark", "bs-Latn-BA"},
            {"BBD", "052", 2, "Barbados Dollar", "en-029"},
            {"BDT", "050", 2, "Bangladeshi Taka", "bn-BD"},
            {"BGN", "975", 2, "Bulgarian Lev", "bg-BG"},
            {"BHD", "048", 3, "Bahraini Dinar", "ar-BH"},
            {"BIF", "108", 0, "Burundi Franc"},
            {"BMD", "060", 2, "Bermudian Dollar"},
            {"BND", "096", 2, "Brunei Dollar", "ms-BN"},
            {"BOB", "068", 2, "Boliviano", "es-BO"},
            {"BRL", "986", 2, "Brazilian Real", "pt-BR"},
            {"BSD", "044", 2, "Bahamian Dollar"},
            {"BTN", "064", 2, "Bhutanese Ngultrum"},
            {"BWP", "072", 2, "Botswana Pula"},
            {"BYR", "974", 0, "Belarussian Ruble", "be-BY"},
            {"BZD", "084", 2, "Belize Dollar", "en-BZ"},
            {"CAD", "124", 2, "Canadian Dollar", "en-CA"},
            {"CDF", "976", 2, "Congolese Franc"},
            {"CHF", "756", 2, "Swiss Franc", "de-CH"},
            {"CLP", "152", 0, "Chilean Peso", "es-CL"},
            {"CNY", "156", 2, "Yuan Renminbi", "zh-CN"},
            {"COP", "170", 2, "Colombian Peso", "es-CO"},
            {"CRC", "188", 2, "Costa Rican Colon", "es-CR"},
            {"CUC", "931", 2, "Cuban convertible peso"},
            {"CUP", "192", 2, "Cuban Peso"},
            {"CVE", "132", 2, "Cape Verde Escudo"},
            {"CZK", "203", 2, "Czech Koruna", "cs-CZ"},
            {"DJF", "262", 0, "Djibouti Franc"},
            {"DKK", "208", 2, "Danish Krone", "da-DK"},
            {"DOP", "214", 2, "Dominican Peso", "es-DO"},
            {"DZD", "012", 2, "Algerian Dinar", "tzm-Latn-DZ"},
            {"EGP", "818", 2, "Egyptian Pound", "ar-EG"},
            {"ERN", "232", 2, "Eritrean Nakfa"},
            {"ETB", "230", 2, "Ethiopian Birr", "am-ET"},
            {"EUR", "978", 2, "Euro", "de-DE"},
            {"FJD", "242", 2, "Fiji Dollar", "en-029"},
            {"FKP", "238", 2, "Falkland Islands Pound"},
            {"GBP", "826", 2, "Pound Sterling", "en-GB"},
            {"GEL", "981", 2, "Georgian Lari", "ka-GE"},
            {"GHS", "936", 2, "Ghanaian Cedi"},
            {"GIP", "292", 2, "Gibraltar Pound", "en-GB"},
            {"GMD", "270", 2, "Gambian Dalasi"},
            {"GNF", "324", 0, "Guinea Franc"},
            {"GTQ", "320", 2, "Guatemalan Quetzal"},
            {"GYD", "328", 2, "Guyana Dollar", "en-029"},
            {"HKD", "344", 2, "Hong Kong Dollar", "zh-HK"},
            {"HNL", "340", 2, "Honduran Lempira", "es-HN"},
            {"HRK", "191", 2, "Croatian Kuna", "hr-HR"},
            {"HTG", "332", 2, "Haitian Gourde"},
            {"HUF", "348", 2, "Hungarian Forint", "hu-HU"},
            {"IDR", "360", 2, "Indonesian Rupiah", "id-ID"},
            {"ILS", "376", 2, "New Israeli Sheqel", "he-IL"},
            {"INR", "356", 2, "Indian Rupee", "en-IN"},
            {"IQD", "368", 3, "Iraqi Dinar", "ar-IQ"},
            {"IRR", "364", 2, "Iranian Rial", "fa-IR"},
            {"ISK", "352", 0, "Iceland Krona", "is-IS"},
            {"JMD", "388", 2, "Jamaican Dollar", "en-JM"},
            {"JOD", "400", 3, "Jordanian Dinar", "ar-JO"},
            {"JPY", "392", 0, "Japanese Yen", "ja-JP"},
            {"KES", "404", 2, "Kenyan Shilling", "sw-KE"},
            {"KGS", "417", 2, "Kyrgyzstani Som", "ky-KG"},
            {"KHR", "116", 2, "Cambodian Riel", "km-KH"},
            {"KMF", "174", 0, "Comoro Franc"},
            {"KPW", "408", 2, "North Korean Won"},
            {"KRW", "410", 0, "South Korean Won", "ko-KR"},
            {"KWD", "414", 3, "Kuwaiti Dinar", "ar-KW"},
            {"KYD", "136", 2, "Cayman Islands Dollar", "en-029"},
            {"KZT", "398", 2, "Kazakhstani Tenge", "kk-KZ"},
            {"LAK", "418", 2, "Lao Kip", "lo-LA"},
            {"LBP", "422", 2, "Lebanese Pound", "ar-LB"},
            {"LKR", "144", 2, "Sri Lanka Rupee", "si-LK"},
            {"LRD", "430", 2, "Liberian Dollar"},
            {"LSL", "426", 2, "Lesotho Loti"},
            {"LTL", "440", 2, "Lithuanian Litas", "lt-LT"},
            {"LVL", "428", 2, "Latvian Lats", "lv-LV"},
            {"LYD", "434", 3, "Libyan Dinar", "ar-LY"},
            {"MAD", "504", 2, "Moroccan Dirham", "ar-MA"},
            {"MDL", "498", 2, "Moldovan Leu"},
            {"MGA", "969", 2, "Malagasy Ariary"},
            {"MKD", "807", 2, "Macedonian Denar", "mk-MK"},
            {"MMK", "104", 2, "Myanama Kyat"},
            {"MNT", "496", 2, "Mongolian Tugrik", "mn-MN"},
            {"MOP", "446", 2, "Macanese Pataca", "zh-MO"},
            {"MRO", "478", 2, "Mauritanian Ouguiya"},
            {"MUR", "480", 2, "Mauritius Rupee"},
            {"MWK", "454", 2, "Malawian Kwacha"},
            {"MVR", "462", 2, "Maldavian Rufiyaa", "dv-MV"},
            {"MXN", "484", 2, "Mexican Peso", "es-MX"},
            {"MYR", "458", 2, "Malaysian Ringgit", "en-MY"},
            {"MZN", "943", 2, "Mozambican Metical"},
            {"NAD", "516", 2, "Namibia Dollar"},
            {"NGN", "566", 2, "Nigerian Naira", "yo-NG"},
            {"NIO", "558", 2, "Nicaraguan Cordoba Oro", "es-NI"},
            {"NOK", "578", 2, "Norwegian Krone", "nb-NO"},
            {"NPR", "524", 2, "Nepalese Rupee", "ne-NP"},
            {"NZD", "554", 2, "New Zealand Dollar", "en-NZ"},
            {"OMR", "512", 3, "Omani Rial", "ar-OM"},
            {"PAB", "590", 2, "Panamanian Balboa", "es-PA"},
            {"PEN", "604", 2, "Peruvian Nuevo Sol", "es-PE"},
            {"PGK", "598", 2, "Papua New Guinean Kina"},
            {"PHP", "608", 2, "Philippine Peso", "en-PH"},
            {"PKR", "586", 2, "Pakistan Rupee", "ur-PK"},
            {"PLN", "985", 2, "Polish Zloty", "pl-PL"},
            {"PYG", "600", 0, "Paraguayan guaraní", "es-PY"},
            {"QAR", "634", 2, "Qatari Rial", "ar-QA"},
            {"RON", "946", 2, "Romanian New Leu", "ro-RO"},
            {"RSD", "941", 2, "Serbian Dinar", "sr-Latn-RS"},
            {"RUB", "643", 2, "Russian Ruble", "ru-RU"},
            {"RWF", "646", 0, "Rwanda Franc", "rw-RW"},
            {"SAR", "682", 2, "Saudi Riyal", "ar-SA"},
            {"SBD", "090", 2, "Solomon Islands Dollar"},
            {"SCR", "690", 2, "Seychelles Rupee"},
            {"SDG", "938", 2, "Sudanese Pound"},
            {"SEK", "752", 2, "Swedish Krona", "sv-SE"},
            {"SGD", "702", 2, "Singapore Dollar", "en-SG"},
            {"SHP", "654", 2, "Saint Helena Pound", "en-GB"},
            {"SLL", "694", 2, "Sierra Leonean Leone"},
            {"SOS", "706", 2, "Somali Shilling"},
            {"SRD", "968", 2, "Surinam Dollar"},
            {"STD", "678", 2, "São Tomé and Príncipe dobra"},
            {"SYP", "760", 2, "Syrian Pound", "ar-SY"},
            {"SZL", "748", 2, "Swazi Lilangeni"},
            {"THB", "764", 2, "Thai Baht", "th-TH"},
            {"TJS", "972", 2, "Tajikistani Somoni"},
            {"TMT", "934", 2, "Turkmenistani New Manat"},
            {"TND", "788", 3, "Tunisian Dinar", "ar-TN"},
            {"TOP", "776", 2, "Tongan Pa’anga"},
            {"TRY", "949", 2, "Turkish Lira", "tr-TR"},
            {"TTD", "780", 2, "Trinidad and Tobago Dollar", "en-TT"},
            {"TWD", "901", 2, "New Taiwan Dollar", "zh-TW"},
            {"TZS", "834", 2, "Tanzanian Shilling"},
            {"UAH", "980", 2, "Ukranian Hryvnia", "uk-UA"},
            {"UGX", "800", 2, "Uganda Shilling"},
            {"USD", "840", 2, "United States Dollar", "en-US"},
            {"UYU", "858", 2, "Uruguayan Peso", "es-UY"},
            {"UZS", "860", 2, "Uzbekistan Sum", "uz-Latn-UZ"},
            {"VEF", "937", 2, "Venezuelan bolívar fuerte", "es-VE"},
            {"VND", "704", 0, "Vietnamese Dong", "vi-VN"},
            {"WST", "882", 2, "Samoan Tala"},
            {"VUV", "548", 0, "Vanuatu Vatu"},
            {"XAF", "950", 0, "CFA Franc BEAC"},
            {"XCD", "951", 2, "East Caribbean Dollar"},
            {"XOF", "952", 0, "CFA Franc BCEAO "},
            {"XPF", "953", 0, "CFP Franc"},
            {"YER", "886", 2, "Yemeni Rial", "ar-YE"},
            {"ZAR", "710", 2, "South African Rand", "en-ZA"},
            {"ZMK", "894", 2, "Zambian Kwacha"},
            {"ZWL", "932", 2, "Zimbabwe Dollar", "en-ZW"},
        };

        /// <summary>Three-letter ISO 4217 currency code</summary>
        public string Code { get; set; }

        /// <summary>Three-digit ISO 4217 currency code</summary>
        /// <example>GBP for Great Britan Pounds Sterling</example>
        public string NumericCode { get; set; }

        /// <summary>Length of the minor unit. typically 2</summary>
        public int MinorUnit { get; set; }

        /// <summary>Currency name in English</summary>
        public string EnglishName { get; set; }

        public string FormatCulture { get; set; }

        public Currency(string code, string numericCode, int minorUnit = 2, string englishName = null, string formatCulture = null) {
            Code = code;
            NumericCode = numericCode;
            MinorUnit = minorUnit;
            EnglishName = englishName;
            FormatCulture = formatCulture;
        }

        /// <summary>
        /// Formats an amount in an appropriate way for this currency's culture
        /// When no culture is available for the currency, format with invariant decimal formating plus the currency code
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="overrideCulture">Optional override culture formatting. For example using en-IE for formatting Euros instead of the default de-DE</param>
        /// <returns></returns>
        /// <example>GBP 1234.56m => £1,245.56</example>
        /// <example>SEK 1234.56m => 1.234,56 kr</example>
        public string FormatAmount(decimal amount, string overrideCulture = null) {
            var culture = overrideCulture ?? FormatCulture;

            return culture.IsNotBlank()
                       ? amount.ToString("C", CultureInfo.GetCultureInfo(culture))
                       : amount.ToString("N", CultureInfo.InvariantCulture) + " " + Code;
        }

        /// <summary>
        /// Takes an amount and converts it to an implied decimal integer.
        /// </summary>
        /// <example>Converting €123.45 to implied decimal yields 12345 since EUR is an exponent 2 currency</example>
        /// <example>Converting ¥123 to implied decimal yields 123 since JPY is an exponent 0 currency</example>
        /// <param name="amount"></param>
        /// <returns></returns>
        public int AmountToImpliedDecimal(decimal amount) {
            var multiplier = (int)Math.Pow(10d, MinorUnit);
            return (int)(amount * multiplier);
        }

        /// <summary>
        /// Get a Currency object by the 3-char or 3-digit currency code
        /// </summary>
        /// <param name="currencyId"></param>
        /// <returns></returns>
        public static Currency GetById(string currencyId) {
            return BuiltInCurrencies[currencyId];
        }

        /// <summary>
        /// Test if the given identifier is valid. Must be 3 letters or 3 numbers
        /// </summary>
        /// <param name="currencyId"></param>
        /// <returns></returns>
        public static bool IsValidIdentifier(string currencyId) {
            return currencyId != null && currencyId.Length == 3 && Regex.IsMatch(currencyId, @"^(?:[A-Z]{3}|[0-9]{3})$");
        }

        /// <summary>
        /// Returns the currency 3-character code
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return Code;
        }

        /// <summary>Allow creating currencies by ID</summary>
        /// <param name="currencyId"></param>
        /// <returns></returns>
        public static implicit operator Currency(string currencyId) {
            return GetById(currencyId);
        }
    }
}