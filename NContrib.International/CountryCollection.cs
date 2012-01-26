using System.Collections.Generic;
using System.Linq;

namespace NContrib.International {

    public class CountryCollection : HashSet<Country> {

        public void Add(int id, string twoChar, string threeChar, string name) {
            Add(new Country(id, twoChar, threeChar, name));
        }

        /// <summary>
        /// Get a country base
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Country this[int id] {
            get { return this.Single(x => x.CodeNumeric == id); }
        }

        public Country this[string id] {
            get {
                if (id.Length == 2)
                    return this.SingleOrDefault(x => x.CodeAlpha2 == id);

                if (id.Length == 3)
                    return this.SingleOrDefault(x => x.CodeAlpha3 == id);

                return this.SingleOrDefault(x => x.EnglishName == id);
            }
        }
    }

}
