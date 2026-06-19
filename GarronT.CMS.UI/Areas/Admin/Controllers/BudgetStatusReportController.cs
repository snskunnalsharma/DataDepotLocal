using GarronT.CMS.Model.BO;
using GarronT.CMS.Model.DAL;
using KSPL.Indus.MSTS.UI.Filters;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GarronT.CMS.UI.Areas.Admin.Controllers
{
    [OutputCache(Duration = 0, VaryByParam = "none")]
    [SessionExpire]
    [Authorize]
    [RBAC]
    public class BudgetStatusReportController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BudgetStatusReportController));

        public ActionResult Index()
        {
            try
            {
                using (BudgetStatusReportDAL objDAL = new BudgetStatusReportDAL())
                {
                    var utilityList = objDAL.GetUtilityTypeList();

                    if (utilityList == null)
                    {
                        utilityList = new List<BudgetUtilityBO>();
                    }

                    utilityList.Insert(0, new BudgetUtilityBO
                    {
                        Id = 0,
                        UtilityName = ""
                    });

                    IEnumerable<SelectListItem> utility = utilityList
                        .Select(value => new SelectListItem
                        {
                            Text = value.UtilityName,
                            Value = value.Id == 0 ? "" : value.Id.ToString()
                        })
                        .OrderBy(o => o.Text)
                        .ToList();

                    ViewBag.UtilityListItem = utility;
                }

                return View();
            }
            catch (Exception ex)
            {
                log.Error("BudgetStatusReport Index error", ex);

                ViewBag.UtilityListItem = new List<SelectListItem>
                {
                    new SelectListItem { Text = "", Value = "" }
                };

                return View();
            }
        }

        [HttpGet]
        public JsonResult GetProject(int UtilityId)
        {
            try
            {
                using (BudgetStatusReportDAL objDAL = new BudgetStatusReportDAL())
                {
                    var data = objDAL.GetProjectByUtility(UtilityId);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error("BudgetStatusReport GetProject error", ex);

                return Json(new
                {
                    success = false,
                    returnMessage = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetInstallerList(long projectId)
        {
            try
            {
                using (BudgetStatusReportDAL objDAL = new BudgetStatusReportDAL())
                {
                    var data = objDAL.GetInstallerByProject(projectId);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error("BudgetStatusReport GetInstallerList error", ex);

                return Json(new
                {
                    success = false,
                    returnMessage = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetBudgetStatusReport(long? projectId, long? installerId, string fromDate, string toDate)
        {
            try
            {
                using (BudgetStatusReportDAL objDAL = new BudgetStatusReportDAL())
                {
                    BudgetStatusReportResultBO result = objDAL.GetBudgetStatusReport(projectId, installerId, fromDate, toDate);

                    var jsonResult = Json(new
                    {
                        success = true,
                        data = result.ReportRows,
                        ApprovedProjectBudget = result.ApprovedProjectBudget,
                        UsedBudget = result.UsedBudget,
                        BalanceBudget = result.BalanceBudget,
                        ServiceLineBudget = result.ServiceLineBudget,
                        ServiceLineUsedBudget = result.ServiceLineUsedBudget,
                        ServiceLineBalanceBudget = result.ServiceLineBalanceBudget,
                        ServiceLineData = result.ServiceLineReportRows
                    }, JsonRequestBehavior.AllowGet);

                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                }
            }
            catch (Exception ex)
            {
                log.Error("BudgetStatusReport GetBudgetStatusReport error", ex);

                return Json(new
                {
                    success = false,
                    returnMessage = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult ExportBudgetStatusReport(long? projectId, long? installerId, string fromDate, string toDate)
        {
            try
            {
                using (BudgetStatusReportDAL objDAL = new BudgetStatusReportDAL())
                {
                    string exportedBy = string.Empty;

                    if (installerId.HasValue && installerId.Value > 0)
                    {
                        using (var db = new GarronT.CMS.Model.CompassEntities())
                        {
                            var activeUser = db.tblUsers.Where(t => t.UserID == installerId.Value).FirstOrDefault();
                            if (activeUser != null)
                            {
                                exportedBy = (activeUser.FirstName + " " + activeUser.LastName).Trim();
                                if (string.IsNullOrEmpty(exportedBy))
                                {
                                    exportedBy = activeUser.UserName;
                                }
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(exportedBy))
                    {
                        var userName = User.Identity.Name;
                        if (!string.IsNullOrEmpty(userName))
                        {
                            using (var db = new GarronT.CMS.Model.CompassEntities())
                            {
                                var activeUser = db.tblUsers.Where(t => t.UserName == userName).FirstOrDefault();
                                if (activeUser != null)
                                {
                                    exportedBy = (activeUser.FirstName + " " + activeUser.LastName).Trim();
                                }
                            }
                            if (string.IsNullOrEmpty(exportedBy))
                            {
                                exportedBy = userName;
                            }
                        }
                    }

                    byte[] fileBytes = objDAL.ExportBudgetStatusReport(projectId, installerId, fromDate, toDate, exportedBy);
                    return File(fileBytes, "application/pdf", "BudgetStatusReport.pdf");
                }
            }
            catch (Exception ex)
            {
                log.Error("BudgetStatusReport ExportBudgetStatusReport error", ex);
                return Content("Export failed: " + ex.Message);
            }
        }
    }
}