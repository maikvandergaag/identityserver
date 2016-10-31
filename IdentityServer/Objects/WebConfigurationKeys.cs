using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IdentityServer.Objects {
    internal static class WebConfigurationKeys {

        internal static readonly string Location = "IS:Location";

        internal static readonly string Thumbprint = "IS:Certificate";

        internal static readonly string Authority = "IS:Authority";

        internal static readonly string ClientId = "IS:ClientId";

        internal static readonly string RedirectUrl = "IS:RedirectUrl";
    }
}