using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebInquirer.Models;
using WebInquirer.Classes;


namespace WebInquirer.Controllers
{
    public class HomeController : Controller
    {
        private DatabaseEntities db = new DatabaseEntities();

        public ActionResult Index()
        {
            
            return View(DBHelpers.GetNewTests());
        }

        public ActionResult View(int id)
        {
            Test test = DBHelpers.GetPassedTestById(id);
            if (test == null)
            {
                return RedirectToAction("Index");
            }

            return View(test);
        }


        public ActionResult Pass(int id)
        {
            Test test = DBHelpers.GetNewTestById(id);

            if (test == null)
            {
                return RedirectToAction("Index");
            }

            return View(test);
        }


        [HttpPost]
        public ActionResult Pass(Test test)
        {
            DBHelpers.PassTest(test);

            return RedirectToAction("Index");
        }

        public ActionResult Results()
        {

            return View(DBHelpers.GetPassedTests());
        }

        public ActionResult Remove(int id)
        {
            DBHelpers.RemoveTestById(id);

            return RedirectToAction("Index");
        }

    }
}