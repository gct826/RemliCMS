using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using RemliCMS.Models;
using RemliCMS.Routes;

namespace RemliCMS.Controllers
{
    [Authorize(Roles = "admin")]
    public class UserManagementController : BaseController
    {

        public UserManagementController(IRouteService routeService) : base(routeService)
        {
        }

        //
        // GET: /UserManagement/
        public ActionResult Index()
        {
            var allRolesList = Roles.GetAllRoles();

            var rolesList = new List<RoleModel>();

            foreach (var role in allRolesList)
            {
                var newRole = new RoleModel
                    {
                        RoleName = role
                    };

                rolesList.Add(newRole);
            }

            return View(rolesList.ToList());
        }



    }
}
