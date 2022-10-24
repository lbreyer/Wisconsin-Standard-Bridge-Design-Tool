using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WisDot.Bos.StandardPlans.Repositories;

namespace WisDot.Bos.StandardPlans.Controllers
{

    public class StandardPlanController : Controller
    {
        private IStandardPlanRepository standardPlanRepository;

        public StandardPlanController()
        {
            this.standardPlanRepository = new StandardPlanRepository();
        }

        // GET: StandardPlan
        public ActionResult Index()
        {
            return View();
        }

        // GET: StandardPlan/Details/5
        public ActionResult Details(int id = 2)
        {
            var standardPlan = standardPlanRepository.GetStandardPlan(id);
            
            /*
            if (standardPlan == null)
            {
                return DllNotFoundException()
            }*/
            return View(standardPlan);
        }

        // GET: StandardPlan/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StandardPlan/Create
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

        // GET: StandardPlan/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: StandardPlan/Edit/5
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

        // GET: StandardPlan/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: StandardPlan/Delete/5
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
