namespace NContrib.Web.MultiTenancy {

    using System;    

    /// <summary>
    /// Creates a TenantNotFoundException
    /// </summary>
    public class TenantNotFoundException : Exception {

        public TenantNotFoundException(string tenantName)
            : base("A tenant named '" + tenantName + "' could not be found.") {}

    }

}
