using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using C4Inventory2._0.Models;

namespace C4Inventory2._0.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ViewResult InventoryRequest()
        {
            return View();
        }

        public ActionResult Returns()
        {
            return View();
        }

        public ActionResult MaintainProductIds()
        {
            return View();
        }

        public ActionResult MaintainKits()
        {
            return View();
        }

        public ActionResult DeliveryHistory()
        {
            return View();
        }

        public ActionResult IrHistory()
        {
            return View();
        }

        public ActionResult ExportMacId()
        {
            var model = new ExportMacIdModel();
            return View(model);
        }

        public ActionResult Logout()
        {
            throw new NotImplementedException();
        }

        public ActionResult Deliveries()
        {
            var model = new DeliveriesModel(Request.QueryString["DeliveryNum"]);
            

            return View(model);
        }

        public ActionResult SearchMacId()
        { 
            var model = new SearchMacIdModel();

            return View(model);
        }

        public ActionResult VerifyRecord(string id)
        {
            //int.Parse()

            return null;
        }
    }
}
