using SiraTakipSistemi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiraTakipSistemi.Controllers
{
    public class Auth2 : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (CurrentSession.giris2 == 0)
            {
                filterContext.Result = new RedirectResult("/Anasayfa/Giris");
            }
           
        }
    }
}