﻿using System.Linq;
using System.Web.Mvc;
using YangKai.BlogEngine.Domain;
using YangKai.BlogEngine.Service;

namespace YangKai.BlogEngine.Web.Mvc.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var channels = Proxy.Repository<Channel>().GetAll(p => !p.IsDeleted).ToList();
            return View(channels);
        }

        public ActionResult Config()
        {
            return Json(new {a = "1"});
        }
    }
}