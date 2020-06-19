using CodeArt.Episerver.DevConsole.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CodeArt.Episerver.DevConsole.Controllers
{


    //[Authorize(Roles ="CLIUsers")]
    public class CLIController : Controller
    {
        private readonly CommandManager _manager;

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (!User.IsInRole("CLIUsers")) throw (new UnauthorizedAccessException("Only available with proper access token"));
        }

        public CLIController(CommandManager cman)
        {
            _manager = cman;
        }

        public ActionResult FetchLog(int LastLogNo)
        {
            _manager.UpdateJobs();
            var lst = _manager.Log.Skip(LastLogNo).Take(100).ToList();
            return Json(new { LastNo = LastLogNo + lst.Count, LogItems = lst }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RunCommand(string command)
        {
            _manager.ExecuteCommand(command);
            return Json(new { });
        }

        public ActionResult Index()
        {
            return Content("Test");
        }
    }
}
