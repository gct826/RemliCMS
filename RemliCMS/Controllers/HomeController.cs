using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RemliCMS.Routes;

namespace RemliCMS.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IRouteService routeService) : base(routeService)
        {
        }

        //
        // GET: /Home/Permalink

        public ActionResult Index(string permalink)
        {
            RouteValues routeValues = RouteValue;

            ViewBag.Debug = true;
            ViewBag.Translation = routeValues.Translation;
            ViewBag.Controller = routeValues.Controller;
            ViewBag.Action = routeValues.Action;
            ViewBag.Permalink = routeValues.Permalink;

            return View();
        }

        //
        // GET: /Home/RouteDebug
        [ChildActionOnly]
        public ActionResult RouteDebug()
        {
            return View();
        }
    }
}
