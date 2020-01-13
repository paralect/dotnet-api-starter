using System.Security.Principal;

namespace Api.Security
{
    public class Principal : GenericPrincipal
    {
        public Principal(IIdentity identity, string[] roles) : base(identity, roles)
        {
        }
    }
}
