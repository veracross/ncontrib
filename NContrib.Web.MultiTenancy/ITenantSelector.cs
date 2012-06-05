using System.Collections.Generic;
using System.Web.Routing;

namespace NContrib.Web.MultiTenancy {

    public interface ITenantSelector<T> where T : IApplicationTenant {

        /// <summary>
        /// Gets all available tenants for this application
        /// </summary>
        IEnumerable<T> Tenants { get; }
            
        /// <summary>
        /// Selects an application tenant based on the request
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        T Select(RequestContext context);
    }
}