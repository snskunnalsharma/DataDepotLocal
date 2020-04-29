using GarronT.CMS.Model;
using KSPL.AIBC.ARM.Model.BO;
using KSPL.Indus.MSTS.UI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GarronT.CMS.UI.Areas.Admin.Controllers
{
    [OutputCache(Duration = 0, VaryByParam = "none")]
     [SessionExpire]
     [Authorize]
    [RBAC]
    public class CountryController : Controller
    {
      
            //
            // GET: /SuperAdmin/Country/
            public ActionResult Index()
            {
                // CountryModel a = new CountryModel();
                return View();
            }
            [HttpPost]
            public ActionResult GetCountryList()
            {
                try
                {

                    List<CountryModel> list = GetStaticCountylist();

                    return Json(list, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {

                    throw ex;
                }

            }

            public List<CountryModel> GetStaticCountylist()
            {
                List<CountryModel> list = new List<CountryModel>();
                CountryModel obj1 = new CountryModel();
                obj1.CountryName = "US";
                obj1.CountryID = 1;
                list.Add(obj1);

                CountryModel obj2 = new CountryModel();
                obj2.CountryName = "India";
                obj2.CountryID = 2;
                list.Add(obj2);

                return list;
            }

            [HttpPost]
            public ActionResult GetDeActiveRecords()
            {
                try
                {
                    var list = GetStaticCountylist(); ;//new CountryDAL().GetDeActiveRecords();
                    return Json(list, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {

                    throw ex;
                }

            }

            //
            // GET: /SuperAdmin/Country/Details/5
            public ActionResult Details(int id)
            {
                return View();
            }

            //
            // GET: /SuperAdmin/Country/Create
            public ActionResult Create()
            {
                return View();
            }

            //
            // POST: /SuperAdmin/Country/Create
            [HttpPost]
            public ActionResult Create(CountryModel CountryModel)
            {
                try
                {
                    // TODO: Add insert logic here
                    DateTime dt = DateTime.Now;// new CommonFunctions().ServerDate();
                    if (CountryModel.CountryID == 0)
                    {
                        ModelState["CountryID"].Errors.Clear();
                    }
                    string returnMessage = "";
                    bool success = false;

                    if (ModelState.IsValid)
                    {
                        if (CountryModel.CountryID == 0)
                        {
                            CountryModel.CreatedBy = null;
                            CountryModel.CreatedOn = dt;
                            CountryModel.Active = 1;
                        }
                        else
                        {
                            CountryModel.Active = 1;
                            CountryModel.ModifiedBy = null;
                            CountryModel.ModifiedOn = dt;
                        }
                        returnMessage = CommonFunctions.strRecordCreated;
                        success = true;// new CountryDAL().CreateOrUpdate(CountryModel, out returnMessage);

                    }
                    return Json(new { success = success, returnMessage = returnMessage }, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return View();
                }
                return RedirectToAction("Index");
            }

            //
            // GET: /SuperAdmin/Country/Edit/5
            public ActionResult Edit(int id)
            {
                DateTime dt = new CommonFunctions().ServerDate();

                CountryModel obj1 = new CountryModel();
                obj1.CountryName = "US";
                obj1.CountryID = 1;

                CountryModel modelData = obj1;// new CountryDAL().GetById(id);

                return Json(modelData, JsonRequestBehavior.AllowGet);
            }

            //
            // POST: /SuperAdmin/Country/Edit/5
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

            //
            // GET: /SuperAdmin/Country/Delete/5
            public ActionResult Delete(int id)
            {
                return View();
            }

            //
            // POST: /SuperAdmin/Country/Delete/5
            [HttpPost]
            public ActionResult Delete(string id, string msg)
            {
                try
                {
                    string returnMessage = ""; ;
                    bool success;
                    if (msg == "Activate")
                    {
                        // var model = new CountryDAL().GetByCountryById(Convert.ToInt64(id));
                        // model.Active = 1;

                        success = true;//new CountryDAL().ActivateRecord(Convert.ToInt32(model.CountryID), out returnMessage);

                        if (success)
                        {
                            returnMessage = CommonFunctions.strRecordActivated;
                        }
                        else
                        {
                            returnMessage = CommonFunctions.strRecordActivatingError;
                        }
                    }
                    else
                    {

                        //  var model = new CountryDAL().GetByCountryById(Convert.ToInt64(id));
                        // model.Active = 0;

                        success = true;//new CountryDAL().CreateOrUpdate(model, out returnMessage);

                        if (success)
                        {
                            returnMessage = CommonFunctions.strRecordDeactivated;
                        }
                        else
                        {
                            if (returnMessage == CommonFunctions.strRecordDeactivatingExist)
                                returnMessage = CommonFunctions.strRecordDeactivatingExist;
                            else
                                returnMessage = CommonFunctions.strRecordDeactivatingError;
                        }

                    }

                    return Json(new { success = success, returnMessage = returnMessage }, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return View();
                }
            }
 
        }
}