using SiraTakipSistemi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiraTakipSistemi.Controllers
{
    public class Auth : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (CurrentSession.giris == 0)
            {
                filterContext.Result = new RedirectResult("/SiraTakip/Giris");
            }
           
        }
    }
}