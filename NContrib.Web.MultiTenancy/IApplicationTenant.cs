namespace NContrib.Web.MultiTenancy {

    using System.Collections.Generic;

    public interface IApplicationTenant {

        /// <summary>
        /// Client name, for example
        /// </summary>
        string Name { get; }

        /// <summary>
        /// List of 'tenant' route values that apply to this tenant
        /// </summary>
        IEnumerable<string> RouteValues { get; }
    }
}