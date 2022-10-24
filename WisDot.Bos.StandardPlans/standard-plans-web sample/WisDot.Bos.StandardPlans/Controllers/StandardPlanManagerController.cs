using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WisDot.Bos.StandardPlans.Controllers
{
    public class StandardPlanManagerController : Controller
    {
        // GET: StandardPlanManager
        public ActionResult Index()
        {
            return View();
        }

        // GET: StandardPlanManager/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: StandardPlanManager/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StandardPlanManager/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: StandardPlanManager/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: StandardPlanManager/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: StandardPlanManager/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: StandardPlanManager/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
