namespace NContrib.Culture {

    public static class TextCaseTrasnformers {

        private static ITextCaseTransformer _english;

        public static ITextCaseTransformer English = _english ?? (_english = new EnglishTextCaseTransformer());
    }
}
