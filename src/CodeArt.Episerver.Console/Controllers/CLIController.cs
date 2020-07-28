using CodeArt.Episerver.DevConsole.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CodeArt.Episerver.DevConsole.Controllers
{


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

        public ActionResult FetchLog(int LastLogNo, string session = null)
        {
            if (session == null) session = Guid.NewGuid().ToString();
            var log = _manager.GetLogs(session);
            var lst =log.Skip(LastLogNo).Take(100).ToList();
            return Json(new { LastNo = LastLogNo + lst.Count, LogItems = lst, Session = session }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult RunCommand(string command, string session = null)
        {
            if (session == null) session = Guid.NewGuid().ToString();
            var rf = _manager.ExecuteCommand(command, session);
            if (rf != null)
            {
                string data=Convert.ToBase64String(rf.Data);
                string filename = rf.FileName;
                return Json(new { Success = true, Session = session, Filename = filename, Data = data });
            } else return Json(new { Success = true, Session=session });
        }

        public ActionResult Index()
        {
            return Content("Test");
        }
    }
}
