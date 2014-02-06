using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RemliCMS.Routes;

namespace RemliCMS.Controllers
{
    public class SharedController : BaseController
    {
        public SharedController(IRouteService routeService) : base(routeService)
        {
        }

        //
        // GET: /Shared/Pong

        public ActionResult Pong()
        {
            return View();
        }

        //
        // GET: /Shared/Error
        public ActionResult Error(int errorCode=400)
        {

            ViewBag.ErrorCode = errorCode;
            Response.StatusCode = errorCode;

            return View();
        }

    }
}
