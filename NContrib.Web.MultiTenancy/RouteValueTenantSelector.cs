using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;

namespace NContrib.Web.MultiTenancy {

    /// <summary>
    /// Selects the application tenant based on a route value
    /// </summary>
    public class RouteValueTenantSelector<T> : ITenantSelector<T> where T : IApplicationTenant {

        /// <summary>
        /// MVC route value used to identify the tenant
        /// </summary>
        public const string RouteValueTenantIdentifier = "tenant";

        /// <summary>
        /// Gets the tenants used by the application
        /// </summary>
        public IEnumerable<T> Tenants { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the RouteTenantSelector. This will
        /// select the tenant to use based on an MVC route value
        /// </summary>
        /// <param name="tenants"></param>
        public RouteValueTenantSelector(IEnumerable<T> tenants) {
            
            Ensure.Argument.NotNull(tenants, "tenants");

            Tenants = tenants;
        }

        public T Select(RequestContext context) {

            Ensure.Argument.NotNull(context, "context");

            if (!context.RouteData.Values.ContainsKey(RouteValueTenantIdentifier))
                throw new Exception("The route does not contain a value for '" + RouteValueTenantIdentifier + "'. Make sure your route contains {" + RouteValueTenantIdentifier + "}");

            var tenantId = context.RouteData.Values[RouteValueTenantIdentifier].ToString();
            
            Ensure.NotNullOrEmpty(tenantId, "Could not find a route value '" + RouteValueTenantIdentifier + "' in the current requst.");
            
            var tenant = Tenants.SingleOrDefault(t => t.RouteValues.Contains(tenantId, StringComparer.InvariantCultureIgnoreCase));

            if (tenant == null)
                throw new TenantNotFoundException(tenantId);

            return tenant;
        }
    }
}
