using System.Collections.Generic;

namespace NContrib.Culture {

    public interface ITextCaseTransformer {

        string ToTitleCase(string input, IEnumerable<string> specials = null);
    }
}
