using System.Collections.Generic;
using System.Linq;

namespace NContrib.International {

    public class CountrySubdivisionCollection : HashSet<CountrySubdivision> {

        public void Add(string id, string name, string type) {
            Add(new CountrySubdivision(id, name, type));
        }

        public IEnumerable<CountrySubdivision> ForCountry(Country c) {
            return ForCountry(c.CodeAlpha2);
        }

        public IEnumerable<CountrySubdivision> ForCountry(string code) {
            return this.Where(cs => cs.CountryCode == code);
        }
    }
}
