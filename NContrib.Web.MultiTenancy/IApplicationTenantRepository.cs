namespace NContrib.Web.MultiTenancy {

    using System.Collections.Generic;

    public interface IApplicationTenantRepository<T> where T : IApplicationTenant {

        IList<T> Tenants();
        void Refresh();
    }
}
