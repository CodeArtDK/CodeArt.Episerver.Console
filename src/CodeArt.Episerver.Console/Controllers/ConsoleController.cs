﻿using CodeArt.Episerver.DevConsole;
using CodeArt.Episerver.DevConsole.Core;
using CodeArt.Episerver.DevConsole.Interfaces;
using CodeArt.Episerver.DevConsole.Models;
using EPiServer.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CodeArt.Episerver.Controllers
{
    
    public class ConsoleController : Controller
    {
        private readonly CommandManager _manager;

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            
            if (!PrincipalInfo.HasAdminAccess)
            {
                throw new UnauthorizedAccessException();
            }

            base.OnAuthorization(filterContext);
        }


        public ConsoleController(CommandManager cman)
        {
            _manager = cman;
        }


        public ActionResult Index()
        {
            var model = new ConsoleModel();
            var cmdlist=_manager.Commands.Select(kvp => 
                new { Keyword = kvp.Value.Keyword, Description=kvp.Value.Info.Description,CanPipeIn= typeof(IInputCommand).IsAssignableFrom(kvp.Value.CommandType), CanPipeOut= typeof(IOutputCommand).IsAssignableFrom(kvp.Value.CommandType), Syntax=kvp.Value.Info.Syntax, Group=kvp.Value.Info.Group, Parameters = kvp.Value.Parameters.Keys }).OrderBy(m => m.Keyword).ToList();
            model.Commands = JsonConvert.SerializeObject(cmdlist);
            
            return View(model);
        }

        public ActionResult FetchLog(int LastLogNo, string session=null)
        {
            if (string.IsNullOrEmpty(session)) session = Guid.NewGuid().ToString();
            var lst = _manager.GetLogs(session).Skip(LastLogNo).Take(100).ToList();
            return Json(new { LastNo = LastLogNo + lst.Count, LogItems = lst.Select(l => l.ToString().Replace("\t","&nbsp;&nbsp;")).ToList(), Session=session }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult RunCommand(string command, string session=null)
        {
            var rf=_manager.ExecuteCommand(command,session);
            if (rf != null)
            {
                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = rf.FileName,
                    Inline = false
                };
                Response.AppendHeader("Content-Disposition", cd.ToString());
                return File(rf.Data, "application/octet-stream" /*rf.Mimetype*/);
            }
            return Json(new { Success = true });
        }
    }
}
