using GarronT.CMS.Model.BO;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
namespace GarronT.CMS.Model.DAL
{
    public class InstallerDAL : IDisposable
    {

        #region Create Object ILog for Logger write
        private static readonly ILog log = LogManager.GetLogger(typeof(InstallerDAL));
        #endregion

        // Add static list
        //public static List<Proc_GetInstallerFormStatusData_Result> objTempSkipList = new List<Proc_GetInstallerFormStatusData_Result>();

        bool disposed;
        protected virtual void Dispose(bool disposing)
        {
            //if (!disposed)
            //{
            //    if (disposing)
            //    {
            //        //dispose managed ressources

            //    }
            //}
            //dispose unmanaged ressources
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public List<string> GetCycleByProjectID(long ProjectID)
        {
            CompassEntities dbcontext = new CompassEntities();

            List<string> GetCycle = dbcontext.tblUploadedDatas.AsNoTracking().Where(x => x.ProjectId == ProjectID && x.Active == 1).Select(a => a.Cycle).Distinct().OrderBy(a => a).ToList();

            return GetCycle;
        }

        public List<string> GetFreshCycleByProjectID(long projectID)
        {
            CompassEntities dbcontext = new CompassEntities();

            List<string> GetCycle = dbcontext.PROC_PendingCycle(projectID).OrderBy(o => o).ToList();

            return GetCycle;
        }

        public List<PROC_ExistingCycle_Result> GetExistingMappedCycle(long projectID, long installerId, DateTime fromDate, DateTime toDate)
        {
            CompassEntities dbcontext = new CompassEntities();

            var GetCycle = dbcontext.PROC_ExistingCycle(projectID, installerId, fromDate, toDate).ToList();

            return GetCycle;
        }

        public List<string> GetDynamicFreshCycleByProjectID(long projectID)
        {
            CompassEntities dbcontext = new CompassEntities();
            
            var Param = new SqlParameter { ParameterName = "@ProjectId", Value = projectID };

            //var GetCycle1 = dbcontext.Database.SqlQuery<DataTable>("exec  PROC_DynamicPendingCycle @ProjectId", Param);


            List<string> GetCycle = new List<string>();



            SqlConnection sqlConnection1 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            cmd.CommandText = "PROC_DynamicPendingCycle";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = sqlConnection1;
            cmd.Parameters.Add(Param);
            cmd.CommandTimeout = sqlConnection1.ConnectionTimeout;
            sqlConnection1.Open();

            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                GetCycle.Add(reader["cycle"].ToString());
            }
            // Data is accessible through the DataReader object here.

            sqlConnection1.Close();
            reader.Close();
            reader.Dispose();
            cmd.Dispose();
            sqlConnection1.Dispose();
            return GetCycle;
        }

        public List<PROC_ExistingCycle_Result> GetDynamicExistingMappedCycle(long projectID, long installerId, DateTime fromDate, DateTime toDate)
        {
            CompassEntities dbcontext = new CompassEntities();


            //var GetCycle = dbcontext.Database.SqlQuery<PROC_ExistingCycle_Result>("PROC_DynamicExistingCycle", parameters).ToList();   
            List<PROC_ExistingCycle_Result> objListPROC_ExistingCycle_Result = new List<PROC_ExistingCycle_Result>();
            SqlConnection sqlConnection1 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            cmd.CommandText = "PROC_DynamicExistingCycle";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = sqlConnection1;
            cmd.Parameters.Add("@ProjectId", projectID);
            cmd.Parameters.Add("@InstallerId", installerId);
            cmd.Parameters.Add("@fromDate", fromDate);
            cmd.Parameters.Add("@toDate", toDate);
            sqlConnection1.Open();

            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                objListPROC_ExistingCycle_Result.Add(new PROC_ExistingCycle_Result { Cycle = reader["cycle"].ToString(), Disabled = int.Parse(reader["Disabled"].ToString()) });

            }
            // Data is accessible through the DataReader object here.

            sqlConnection1.Close();
            reader.Close();
            reader.Dispose();
            cmd.Dispose();
            sqlConnection1.Dispose();


            return objListPROC_ExistingCycle_Result;
        }

        public List<string> GetRootByCycle(string[] Cycle, long ProjectId)
        {
            CompassEntities dbcontext = new CompassEntities();

            List<string> GetRoot = dbcontext.tblUploadedDatas.AsNoTracking().Where(x => Cycle.Contains(x.Cycle) && x.ProjectId == ProjectId && x.Active == 1).Select(a => a.Route).Distinct().OrderBy(a => a).ToList();

            return GetRoot;
        }

        public List<string> GetFreshRootByCycle(string[] Cycle, long ProjectId)
        {
            CompassEntities dbcontext = new CompassEntities();

            List<string> GetRoot = dbcontext.PROC_PendingRoute(ProjectId).Where(x => Cycle.Contains(x.Cycle)).Select(a => a.Route).Distinct().OrderBy(a => a).ToList();

            return GetRoot;
        }

        public List<PROC_ExistingRoute_Result> GetExistingRootByCycle(string[] Cycle, long ProjectId, long installerId, DateTime fromDate, DateTime toDate)
        {
            CompassEntities dbcontext = new CompassEntities();

            List<PROC_ExistingRoute_Result> GetRoot = dbcontext.PROC_ExistingRoute(ProjectId, installerId, fromDate, toDate).
                Where(x => Cycle.Contains(x.Cycle)).Select(a => a).Distinct().ToList();

            return GetRoot;
        }

        public List<string> GetAllRouteByCycle(string[] Cycle, long ProjectId)
        {
            string cycleVal = "";
            foreach(var item in Cycle)
            {
                if (cycleVal.Length == 0)
                {
                    cycleVal = item;
                }
                else
                {
                    cycleVal += "," + item + "";
                }
            }
            CompassEntities dbcontext = new CompassEntities();
            List<PROC_ExistingRoute_Result> objListPROC_ExistingRoute_Result = new List<PROC_ExistingRoute_Result>();
            List<string> GetRoot = new List<string>();
            SqlConnection sqlConnection1 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            cmd.CommandText = "PROC_AllRouteByCycle";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = sqlConnection1;
            cmd.Parameters.Add("@ProjectId", ProjectId);
            cmd.Parameters.Add("@Cycle", cycleVal);

            sqlConnection1.Open();

            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                objListPROC_ExistingRoute_Result.Add(new PROC_ExistingRoute_Result { Route = reader["Route"].ToString(), Cycle = reader["Cycle"].ToString() });
            }

            sqlConnection1.Close();
            reader.Close();
            reader.Dispose();
            cmd.Dispose();
            sqlConnection1.Dispose();
            GetRoot = objListPROC_ExistingRoute_Result.Where(x => Cycle.Contains(x.Cycle)).Select(a => a.Route).Distinct().OrderBy(a => a).ToList();

            return GetRoot;
        }

        public List<string> GetDynamicFreshRootByCycle(string[] Cycle, long ProjectId)
        {
            CompassEntities dbcontext = new CompassEntities();
            List<PROC_ExistingRoute_Result> objListPROC_ExistingRoute_Result = new List<PROC_ExistingRoute_Result>();
            List<string> GetRoot = new List<string>();
            SqlConnection sqlConnection1 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            string cycleVal = "";
            foreach (var item in Cycle)
            {
                if (cycleVal.Length == 0)
                {
                    cycleVal = item;
                }
                else
                {
                    cycleVal += "," + item + "";
                }
            }

            cmd.CommandText = "PROC_DynamicPendingRoute_ByCycle";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = sqlConnection1;
            cmd.Parameters.Add("@ProjectId", ProjectId);
            cmd.Parameters.Add("@Cycle", cycleVal);
            sqlConnection1.Open();

            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                objListPROC_ExistingRoute_Result.Add(new PROC_ExistingRoute_Result { Route = reader["Route"].ToString(), Cycle = reader["Cycle"].ToString() });
            }

            sqlConnection1.Close();
            reader.Close();
            reader.Dispose();
            cmd.Dispose();
            sqlConnection1.Dispose();
            GetRoot = objListPROC_ExistingRoute_Result.Where(x => Cycle.Contains(x.Cycle)).Select(a => a.Route).Distinct().OrderBy(a => a).ToList();

            return GetRoot;
        }

        public List<PROC_ExistingRoute_Result> GetDynamicExistingRootByCycle(string[] Cycle, long ProjectId, long installerId, DateTime fromDate, DateTime toDate)
        {
            CompassEntities dbcontext = new CompassEntities();
            string cycleVal = "";
            foreach (var item in Cycle)
            {
                if (cycleVal.Length == 0)
                {
                    cycleVal = item;
                }
                else
                {
                    cycleVal += "," + item + "";
                }
            }

            List<PROC_ExistingRoute_Result> objListPROC_ExistingRoute_Result = new List<PROC_ExistingRoute_Result>();
            SqlConnection sqlConnection1 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            cmd.CommandText = "PROC_DynamicExistingRoute_ByCycle";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = sqlConnection1;
            cmd.Parameters.Add("@ProjectId", ProjectId);
            cmd.Parameters.Add("@InstallerId", installerId);
            cmd.Parameters.Add("@fromDate", fromDate);
            cmd.Parameters.Add("@toDate", toDate);
            cmd.Parameters.Add("@Cycle", cycleVal);
            sqlConnection1.Open();

            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                objListPROC_ExistingRoute_Result.Add(new PROC_ExistingRoute_Result { Route = reader["Route"].ToString(), Cycle = reader["Cycle"].ToString(), Disabled = int.Parse(reader["Disabled"].ToString()) });

            }
            sqlConnection1.Close();
            reader.Close();
            reader.Dispose();
            cmd.Dispose();
            sqlConnection1.Dispose();
            var GetRoot = objListPROC_ExistingRoute_Result.
                Where(x => Cycle.Contains(x.Cycle)).Select(a => a).Distinct().ToList();
            return GetRoot;
        }

        public List<string> GetVisitedRootByCycle(string[] Cycle, long ProjectId)
        {
            CompassEntities dbcontext = new CompassEntities();

            List<string> GetRoot = dbcontext.tblUploadedDatas.AsNoTracking().Where(x => Cycle.Contains(x.Cycle) && x.ProjectId == ProjectId && x.Active == 1).Select(a => a.Route).Distinct().OrderBy(a => a).ToList();

            return GetRoot;
        }

        public List<tblUploadedData> GetAddressByCycle_Root(string cycle, string route, long ProjectId)
        {
            CompassEntities dbcontext = new CompassEntities();
            List<tblUploadedData> GetAddress = dbcontext.tblUploadedDatas.AsNoTracking().Where(x => x.Cycle == cycle && x.Route == route && x.ProjectId == ProjectId && x.Active == 1).ToList();
            return GetAddress;
        }

        public List<tblUploadedDataBO> GetLat_Lag(List<long> Address)//string[] Address
        {
            CompassEntities dbcontext = new CompassEntities();
            List<tblUploadedDataBO> Get_lat_lag = new List<tblUploadedDataBO>();
            List<tblUploadedDataBO> Get_lat_lagnew = new List<tblUploadedDataBO>();
            if (Address != null)
            {
                Get_lat_lag = (from a in dbcontext.tblUploadedDatas.AsNoTracking()
                               where Address.Contains(a.ID) && a.Active == 1
                               select new tblUploadedDataBO { Latitude = a.Latitude, Longitude = a.Longitude, Street = a.Street, ID = a.ID }).ToList();


                //  var results = from p in persons
                //group p by p.PersonID into g
                //select new { PersonID = g.Key, 

                Get_lat_lagnew = (from a in dbcontext.STP_GetAddress()
                                  where Address.Contains(a.Id)
                                  select new tblUploadedDataBO { Latitude = a.Latitude, Longitude = a.Longitude, Street = a.Street, ID = a.Id }).ToList();


                //dbcontext.STP_GetAddress().Where(a => Address.Contains(a.Id)).ToList();

                //Get_lat_lag = (from a in dbcontext.tblUploadedDatas
                //               where Address.Contains(a.ID)
                //               select new tblUploadedDataBO { Latitude = a.Latitude, Longitude = a.Longitude, Street = a.Street, ID = a.ID }).ToList();



                //foreach (var item in Address)
                //{
                //    long a1 = long.Parse(item);
                //    var a = dbcontext.tblUploadedDatas.Where(c => c.ID == a1).FirstOrDefault();
                //    if (a != null)
                //    {
                //        Get_lat_lag.Add(new tblUploadedData { Latitude = a.Latitude, Longitude = a.Longitude, Street = a.Street, ID = a.ID });
                //    }
                // }


                //var newlst = (from c in Address
                //              where !dbcontext.tblInstallerMapDatas.ToList().Any(w => w.tblUploadedData_ID == c)
                //              select c).ToList();

                //if (newlst.Count > 0)
                //{
                //    Get_lat_lag = (from a in dbcontext.tblUploadedDatas
                //                   where newlst.ToList().Any(w => w == a.ID)
                //                   select new tblUploadedDataBO { Latitude = a.Latitude, Longitude = a.Longitude, Street = a.Street, ID = a.ID }).ToList();
                //}
            }
            return Get_lat_lagnew;
        }

        public List<tblUploadedDataBO> GetLat_LagNew(List<long> Address)//string[] Address
        {
            CompassEntities dbcontext = new CompassEntities();
            List<tblUploadedDataBO> Get_lat_lagnew = new List<tblUploadedDataBO>();
            if (Address != null)
            {

                //var result = dbcontext.tblUploadedDatas.AsNoTracking().AsParallel().
                //    Where(x => Address.Contains(x.ID) && x.Active == 1).
                //Select(a => new tblUploadedDataBO
                //{
                //    Latitude = a.Latitude,
                //    Longitude = a.Longitude,
                //    Street = a.Street,
                //    ID = a.ID,

                //}).ToList();

                long firstRecord = Address[0];
                var projectId = dbcontext.UploadedExcelDatas.Where(o => o.Id == firstRecord).FirstOrDefault();
                List<tblUploadedDataBO> result = new List<tblUploadedDataBO>();

                SqlConnection sqlConnection1 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;

                cmd.CommandText = "PROC_InstallerAllocation_AllData";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = sqlConnection1;
                cmd.Parameters.Add("@projectId", projectId.FKProjectId);


                sqlConnection1.Open();

                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new tblUploadedDataBO
                    {
                        Latitude = Convert.ToString(reader["Latitude"]),
                        Longitude = Convert.ToString(reader["Longitude"]),
                        Street = reader["Street"].ToString(),
                        ID = long.Parse(reader["ID"].ToString()),
                    });

                }

                sqlConnection1.Close();
                reader.Close();
                reader.Dispose();
                cmd.Dispose();
                sqlConnection1.Dispose();

                result = result.Where(x => Address.Contains(x.ID)).ToList();

                foreach (var item in result)
                {
                    var streeties = new string[] { "1717 CASTLE DR", "1717 CASTLE DR  RE: SPRINK", "1709 CASTLE DR  RE: PARSON" };
                    if (streeties.Contains(item.Street))
                    {
                        var itemId = item.ID;
                        
                    }
                    var checkExistStreet = Get_lat_lagnew.Where(o => o.Street.Contains(item.Street)).FirstOrDefault();
                    if (checkExistStreet == null)
                    {

                        var checkExistLatLon = Get_lat_lagnew.Where(o => o.Latitude == item.Latitude && o.Longitude == o.Longitude).FirstOrDefault();
                        if (checkExistLatLon == null)
                        {
                            Get_lat_lagnew.Add(item);
                        }
                        else
                        {
                            checkExistLatLon.Street = checkExistLatLon.Street + ",<br/>" + item.Street;
                        }

                    }
                }


            }
            return Get_lat_lagnew;
        }



        public List<UsersModel> GetInstallerList()
        {

            CompassEntities dbcontext = new CompassEntities();
            List<UsersModel> UsersModels = (from u in dbcontext.tblUsers.AsNoTracking()
                                            select new UsersModel()
                                            {

                                                UserId = u.UserID,
                                                UserFullName = u.UserName

                                            }).ToList();
            return UsersModels;
        }

        //public List<InstallerMapModel> GetInstallermapDatabyInstaller(DateTime strDate)
        public List<InstallerMapModel> GetInstallermapDatabyInstaller(DateTime strDate)
        {
            CompassEntities dbcontext = new CompassEntities();
            List<InstallerMapModel> UsersModels = (from u in dbcontext.tblInstallerMapDatas.AsNoTracking()
                                                   where u.Active == 1 && u.FromDate == strDate
                                                   select new InstallerMapModel()
                                                   {
                                                       InstallerId = u.InstallerId,
                                                       tblUploadedData_ID = u.tblUploadedData_ID

                                                   }).ToList();
            return UsersModels;
        }



        /// <summary>
        /// Returns existing mapped cycle list with enable disable status for dropdown
        /// </summary>
        /// <param name="projectID">current project</param>
        /// <param name="installerID">current installer</param>
        /// <param name="fromDate">selected start date</param>
        /// <param name="toDate">selected end date</param>
        /// <returns></returns>
        public List<CycleMaster> GetExistingCycleList(long projectID, long installerID, string fromDate, string toDate)
        {
            DateTime? _frmDate = Convert.ToDateTime(fromDate);
            DateTime? _toDate = Convert.ToDateTime(toDate);

            using (CompassEntities dbcontext = new CompassEntities())
            {

                //var quickResult = dbcontext.STP_GetVisitForUserInstallation(frmDate, toDate, InstallerID, ProjectID).OrderBy(a => a.Latitude).ThenBy(a => a.Longitude).ThenBy(a => a.Street);
                long formId = dbcontext.tblFormMasters.AsNoTracking().Where(o => o.Active == 1 && o.FormTypeID == 2
                   && o.ProjectId == projectID).Select(o => o.FormId).FirstOrDefault();

                var quickResult = dbcontext.Proc_GetInstallerFormStatusData(formId, installerID, _frmDate, _toDate).ToList();

                var pendingCycle = quickResult.Where(o => o.VisitStatus.ToLower() == "pending").
                    Select(o => o.Cycle).Distinct().ToList();

                var skipCycle = quickResult.Where(o => o.VisitStatus.ToLower() == "skip")
                    .Select(o => o.Cycle).Distinct().ToList();

                pendingCycle = pendingCycle.Except(skipCycle).ToList();
                var allCycle = new List<CycleMaster>();

                foreach (var item in pendingCycle)
                {
                    allCycle.Add(new CycleMaster { CycleId = item, CycleDisabled = false });
                }
                foreach (var item in skipCycle)
                {
                    allCycle.Add(new CycleMaster { CycleId = item, CycleDisabled = true });
                }

                return allCycle.OrderBy(o => o.CycleId).ToList();
            }
        }



        /// <summary>
        /// Returns existing mapped route list with enable disable status for dropdown
        /// </summary>
        /// <param name="projectID">current project</param>
        /// <param name="installerID">current installer</param>
        /// <param name="fromDate">selected start date</param>
        /// <param name="toDate">selected end date</param>
        /// <returns></returns>
        public List<RouteMaster> GetExistingRouteList(long projectID, long installerID, string fromDate, string toDate)
        {
            DateTime? _frmDate = Convert.ToDateTime(fromDate);
            DateTime? _toDate = Convert.ToDateTime(toDate);

            using (CompassEntities dbcontext = new CompassEntities())
            {

                //var quickResult = dbcontext.STP_GetVisitForUserInstallation(frmDate, toDate, InstallerID, ProjectID).OrderBy(a => a.Latitude).ThenBy(a => a.Longitude).ThenBy(a => a.Street);
                long formId = dbcontext.tblFormMasters.AsNoTracking().Where(o => o.Active == 1 && o.FormTypeID == 2
                   && o.ProjectId == projectID).Select(o => o.FormId).FirstOrDefault();

                var quickResult = dbcontext.Proc_GetInstallerFormStatusData(formId, installerID, _frmDate, _toDate).ToList();

                var pendingRoute = quickResult.Where(o => o.VisitStatus.ToLower() == "pending").
                    Select(o => o.Route).Distinct().ToList();

                var skipRoute = quickResult.Where(o => o.VisitStatus.ToLower() == "skip")
                    .Select(o => o.Route).Distinct().ToList();

                pendingRoute = pendingRoute.Except(skipRoute).ToList();
                var allRoute = new List<RouteMaster>();

                foreach (var item in pendingRoute)
                {
                    allRoute.Add(new RouteMaster { RouteId = item, RouteDisabled = false });
                }
                foreach (var item in skipRoute)
                {
                    allRoute.Add(new RouteMaster { RouteId = item, RouteDisabled = true });
                }

                return allRoute.OrderBy(o => o.RouteId).ToList();
            }
        }



        public List<AddressMaster> GetInstallerExistingAddress(long projectId, string fromDate, string toDate, long installerId)
        {

            CompassEntities CompassEntities = new CompassEntities();

            //List<AddressMaster> data = CompassEntities.STP_GetVisitForUserInstallation(fromDate, toDate, installerId, projectid).ToList().
            //      Where(x => Root.Contains(x.Route) && Cycle.Contains(x.Cycle)).
            //    Select(o => new AddressMaster { ID = o.id, Street = o.Street, Latitude = o.Latitude, Longitude = o.Longitude }).ToList();

            DateTime? _frmDate = Convert.ToDateTime(fromDate);
            DateTime? _toDate = Convert.ToDateTime(toDate);

            using (CompassEntities dbcontext = new CompassEntities())
            {

                long formId = dbcontext.tblFormMasters.AsNoTracking().Where(o => o.Active == 1 && o.FormTypeID == 2
                   && o.ProjectId == projectId).Select(o => o.FormId).FirstOrDefault();
                dbcontext.Database.CommandTimeout = 0;
                var quickResult = dbcontext.Proc_GetInstallerFormStatusData(formId, installerId, _frmDate, _toDate).ToList();

                var pendingAddress = quickResult.Where(o => o.VisitStatus.ToLower() == "pending").
                    Select(o => o).Distinct().ToList();

                var skipAddress = quickResult.Where(o => o.VisitStatus.ToLower() == "skip")
                    .Select(o => o).Distinct().ToList();


                var allExistingAddress = new List<AddressMaster>();

                foreach (var item in pendingAddress)
                {
                    allExistingAddress.Add(new AddressMaster { ID = item.ID, Latitude = item.Latitude, Longitude = item.Longitude, Street = item.Street, StreetDisabled = false });
                }
                foreach (var item in skipAddress)
                {
                    allExistingAddress.Add(new AddressMaster { ID = item.ID, Latitude = item.Latitude, Longitude = item.Longitude, Street = item.Street, StreetDisabled = true });
                }

                return allExistingAddress.OrderBy(a => a.Latitude).ThenBy(a => a.Longitude).ThenBy(a => a.Street).ToList();

            }


        }

        public List<AddressMaster> GetDynamicInstallerExistingAddress(long projectId, List<string> Root, List<string> Cycle, string fromDate, string toDate, long installerId, string includeSkip)
        {
            //List<AddressMaster> data = CompassEntities.STP_GetVisitForUserInstallation(fromDate, toDate, installerId, projectid).ToList().
            //      Where(x => Root.Contains(x.Route) && Cycle.Contains(x.Cycle)).
            //    Select(o => new AddressMaster { ID = o.id, Street = o.Street, Latitude = o.Latitude, Longitude = o.Longitude }).ToList();

            DateTime _frmDate = Convert.ToDateTime(fromDate);
            DateTime _toDate = Convert.ToDateTime(toDate);
            string routeVal = "";
            string cycleVal = "";
            /*routeVal = string.Join(",", Root.Select(x => $"'{x}'"));
            cycleVal = string.Join(",", Cycle.Select(x => $"'{x}'"));*/
            foreach (var item in Cycle)
            {
                if (cycleVal.Length == 0)
                {
                    cycleVal = item.Trim();
                }
                else
                {
                    cycleVal += "," + item.Trim() + "";
                }
            }
            foreach (var item in Root)
            {
                if (routeVal.Length == 0)
                {
                    routeVal = item.Trim();
                }
                else
                {
                    routeVal += "," + item.Trim();
                }
            }

            using (CompassEntities dbcontext = new CompassEntities())
            {

                var formId = dbcontext.tblFormMasters.AsNoTracking().Where(o => o.Active == 1 && o.FormTypeID == 2
                   && o.ProjectId == projectId).Select(o => o.FormId).FirstOrDefault();
                if (formId != null && formId != 0)
                {

                    var procName = "";
                    if (includeSkip != null && includeSkip.Equals("1"))
                    {
                        procName = "PROC_InstallerAllocation_GetInstallerData_ByCycleRoute_WithSkipped";
                    }
                    else
                    {
                        procName = "PROC_InstallerAllocation_GetInstallerData_ByCycleRoute";
                    }
                    List<Proc_GetInstallerFormStatusData_Result> objList = new List<Proc_GetInstallerFormStatusData_Result>();
                    SqlConnection sqlConnection1 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;

                    try
                    {
                        //cmd.CommandText = "PROC_InstallerAllocation_GetInstallerData";
                        cmd.CommandText = procName;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = sqlConnection1;
                        cmd.Parameters.Add("@formId", formId);
                        cmd.Parameters.Add("@InstallerId", installerId);
                        cmd.Parameters.Add("@FromDate", _frmDate);
                        cmd.Parameters.Add("@ToDate", _toDate);
                        cmd.Parameters.Add("@Cycle", cycleVal);
                        cmd.Parameters.Add("@Route", routeVal);
                        cmd.CommandTimeout = 600;
                        //cmd.CommandTimeout = 120;
                        sqlConnection1.Open();

                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            objList.Add(new Proc_GetInstallerFormStatusData_Result
                            {
                                ID = long.Parse(reader["ID"].ToString()),
                                Street = reader["Street"].ToString(),
                                Latitude = reader["Latitude"].ToString(),
                                Longitude = reader["Longitude"].ToString(),
                                Cycle = reader["Cycle"].ToString(),
                                Route = reader["Route"].ToString(),
                                VisitStatus = reader["VisitStatus"].ToString(),

                                Appointmentflag = reader["Appointmentflag"].ToString()
                            });

                        }


                        sqlConnection1.Close();
                        reader.Close();
                        reader.Dispose();
                        cmd.Dispose();
                        sqlConnection1.Dispose();
                    }
                    catch (Exception ex)
                    { string df = ex.Message.ToString(); }

                    // var quickResult = dbcontext.Proc_GetInstallerFormStatusData(formId, installerId, _frmDate, _toDate).ToList();

                    var pendingAddress = objList.Where(o => o.VisitStatus.ToLower() == "pending").
                        Select(o => o).Distinct().ToList();

                    var skipAddress = objList.Where(o => o.VisitStatus.ToLower() == "skip")
                        .Select(o => o).Distinct().ToList();

                    /*objTempSkipList = new List<Proc_GetInstallerFormStatusData_Result>();
                    objTempSkipList = skipAddress;*/  // This code do not use

                    var allExistingAddress = new List<AddressMaster>();

                    foreach (var item in pendingAddress)
                    {
                        allExistingAddress.Add(new AddressMaster { ID = item.ID, Cycle = item.Cycle, Route = item.Route, Latitude = item.Latitude, Longitude = item.Longitude, Street = item.Street, StreetDisabled = false, Appointmentflag = item.Appointmentflag });
                    }


                    allExistingAddress = allExistingAddress.Where(x => Root.Contains(x.Route) && Cycle.Contains(x.Cycle)).
                                          Select(o => new AddressMaster
                                          {
                                              Latitude = o.Latitude,
                                              Longitude = o.Longitude,
                                              ID = o.ID,
                                              Street = o.Street,
                                              Appointmentflag = o.Appointmentflag
                                          }).ToList();
                    if(includeSkip != null && includeSkip.Equals("1"))
                    {
                        foreach (var item in skipAddress)
                        {
                            allExistingAddress.Add(new AddressMaster { ID = item.ID, Cycle = item.Cycle, Route = item.Route, Latitude = item.Latitude, Longitude = item.Longitude, Street = item.Street, StreetDisabled = true, Appointmentflag = item.Appointmentflag });
                        }
                    }
                    
                    // return allExistingAddress.OrderBy(a => a.Latitude).ThenBy(a => a.Longitude).ThenBy(a => a.Street).ToList();
                    allExistingAddress.OrderBy(a => a.Latitude).ThenBy(a => a.Longitude).ThenBy(a => a.Street).ToList();
                    return allExistingAddress;
                }
                else
                {
                    var allExistingAddress = new List<AddressMaster>();
                    return allExistingAddress;
                }
            }


        }


        public List<AddressMaster> GetInstallerAddress(long projectId, string fromDate, string toDate, long installerId, string status)
        {

            CompassEntities CompassEntities = new CompassEntities();

            //List<AddressMaster> data = CompassEntities.STP_GetVisitForUserInstallation(fromDate, toDate, installerId, projectid).ToList().
            //      Where(x => Root.Contains(x.Route) && Cycle.Contains(x.Cycle)).
            //    Select(o => new AddressMaster { ID = o.id, Street = o.Street, Latitude = o.Latitude, Longitude = o.Longitude }).ToList();

            DateTime? _frmDate = Convert.ToDateTime(fromDate);
            DateTime? _toDate = Convert.ToDateTime(toDate);

            using (CompassEntities dbcontext = new CompassEntities())
            {

                long formId = dbcontext.tblFormMasters.AsNoTracking().Where(o => o.Active == 1 && o.FormTypeID == 2
                   && o.ProjectId == projectId).Select(o => o.FormId).FirstOrDefault();

                var quickResult = dbcontext.Proc_GetInstallerFormStatusData(formId, installerId, _frmDate, _toDate).ToList();

                var pendingAddress = quickResult.Where(o => o.VisitStatus.ToLower() == "pending").
                    Select(o => o).Distinct().ToList();

                var skipAddress = quickResult.Where(o => o.VisitStatus.ToLower() == "skip")
                    .Select(o => o).Distinct().ToList();


                var allExistingAddress = new List<AddressMaster>();

                if (status.ToLower() != "skip")
                {
                    foreach (var item in pendingAddress)
                    {
                        allExistingAddress.Add(new AddressMaster { ID = item.ID, Latitude = item.Latitude, Longitude = item.Longitude, Street = item.Street, StreetDisabled = false });
                    }
                }


                if (status.ToLower() != "pending")
                {
                    foreach (var item in skipAddress)
                    {
                        allExistingAddress.Add(new AddressMaster { ID = item.ID, Latitude = item.Latitude, Longitude = item.Longitude, Street = item.Street, StreetDisabled = true });
                    }
                }

                return allExistingAddress.OrderBy(a => a.Latitude).ThenBy(a => a.Longitude).ThenBy(a => a.Street).ToList();

            }


        }

        public List<AddressMaster> GetDynamicInstallerAddress(long projectId, string fromDate, string toDate, long installerId, string status)
        {

            CompassEntities CompassEntities = new CompassEntities();



            DateTime _frmDate = Convert.ToDateTime(fromDate);
            DateTime _toDate = Convert.ToDateTime(toDate);

            using (CompassEntities dbcontext = new CompassEntities())
            {

                long formId = dbcontext.tblFormMasters.AsNoTracking().Where(o => o.Active == 1 && o.FormTypeID == 2
                   && o.ProjectId == projectId).Select(o => o.FormId).FirstOrDefault();

                List<Proc_GetInstallerFormStatusData_Result> objList = new List<Proc_GetInstallerFormStatusData_Result>();
                SqlConnection sqlConnection1 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;

                cmd.CommandText = "PROC_InstallerAllocation_GetInstallerData";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = sqlConnection1;
                cmd.Parameters.Add("@formId", formId);
                cmd.Parameters.Add("@InstallerId", installerId);
                cmd.Parameters.Add("@FromDate", _frmDate);
                cmd.Parameters.Add("@ToDate", _toDate);
                cmd.CommandTimeout = 600;
                sqlConnection1.Open();

                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    objList.Add(new Proc_GetInstallerFormStatusData_Result
                    {
                        ID = long.Parse(reader["ID"].ToString()),
                        Street = reader["Street"].ToString(),
                        Latitude = reader["Latitude"].ToString(),
                        Longitude = reader["Longitude"].ToString(),
                        Cycle = reader["Cycle"].ToString(),
                        Route = reader["Route"].ToString(),
                        VisitStatus = reader["VisitStatus"].ToString(),

                        Appointmentflag = reader["Appointmentflag"].ToString()
                    });

                }

                sqlConnection1.Close();
                reader.Close();
                reader.Dispose();
                cmd.Dispose();
                sqlConnection1.Dispose();


                var pendingAddress = objList.Where(o => o.VisitStatus.ToLower() == "pending" && o.Appointmentflag != "1").
                    Select(o => o).Distinct().ToList();

                var skipAddress = objList.Where(o => o.VisitStatus.ToLower() == "skip" && o.Appointmentflag != "1")
                    .Select(o => o).Distinct().ToList();


                var allExistingAddress = new List<AddressMaster>();

                if (status.ToLower() != "skip")
                {
                    foreach (var item in pendingAddress)
                    {
                        allExistingAddress.Add(new AddressMaster { ID = item.ID, Latitude = item.Latitude, Longitude = item.Longitude, Street = item.Street, StreetDisabled = false });
                    }
                }


                if (status.ToLower() != "pending")
                {
                    foreach (var item in skipAddress)
                    {
                        allExistingAddress.Add(new AddressMaster { ID = item.ID, Latitude = item.Latitude, Longitude = item.Longitude, Street = item.Street, StreetDisabled = true });
                    }
                }

                return allExistingAddress.OrderBy(a => a.Latitude).ThenBy(a => a.Longitude).ThenBy(a => a.Street).ToList();

            }


        }

        public List<AddressMaster> GetAllAddress(List<string> Root, List<string> Cycle, long Projectid)
        {

            CompassEntities CompassEntities = new CompassEntities();
            List<AddressMaster> objaddress = new List<AddressMaster>();
            if (Root != null)
            {

                objaddress = CompassEntities.STP_GetVisitForProjectInstallation(Projectid).AsParallel().
                    Where(x => Root.Contains(x.Route) && Cycle.Contains(x.Cycle)).
                Select(o => new AddressMaster
                {
                    Latitude = o.Latitude,
                    Longitude = o.Longitude,
                    ID = o.id,
                    Street = o.Street,
                }).OrderBy(a => a.Latitude).ThenBy(a => a.Longitude).ThenBy(a => a.Street).ToList();



            }

            return objaddress;

        }



        public List<AddressMaster> GetDynamicAllAddress(List<string> Root, List<string> Cycle, long projectId)
        {

            CompassEntities dbcontext = new CompassEntities();
            List<AddressMaster> objaddress = new List<AddressMaster>();
            string procName = "PROC_InstallerAllocation_GetAllVisits_ByCycleRoute";
            string routeVal = "";
            string cycleVal = "";
            foreach(var item in Cycle)
            {
                if (cycleVal.Length == 0)
                {
                    cycleVal = item.Trim();
                }
                else
                {
                    cycleVal += "," + item.Trim() + "";
                }
            }
            foreach(var item in Root)
            {
                if(routeVal.Length == 0)
                {
                    routeVal = item.Trim();
                }
                else
                {
                    routeVal += "," + item.Trim();
                }
            }

            if (Root != null)
            {

                SqlConnection sqlConnection1 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;

                //cmd.CommandText = "PROC_InstallerAllocation_GetAllVisits";
                cmd.CommandText = "PROC_InstallerAllocation_GetAllVisits_ByCycleRoute";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = sqlConnection1;
                cmd.Parameters.Add("@ProjectId", projectId);
                cmd.Parameters.Add("@Cycle", cycleVal);
                cmd.Parameters.Add("@Route", routeVal);

                sqlConnection1.Open();

                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    objaddress.Add(new AddressMaster
                    {
                        ID = long.Parse(reader["id"].ToString()),
                        Street = reader["Street"].ToString(),
                        Latitude = reader["AddressLatitude"].ToString(),
                        Longitude = reader["AddressLongitude"].ToString(),
                        Cycle = reader["Cycle"].ToString(),
                        Route = reader["Route"].ToString()
                    });

                }

                sqlConnection1.Close();
                reader.Close();
                reader.Dispose();
                cmd.Dispose();
                sqlConnection1.Dispose();

                /*objaddress = objaddress.Where(x => Root.Contains(x.Route) && Cycle.Contains(x.Cycle)).
                Select(o => new AddressMaster
                {
                    Latitude = o.Latitude,
                    Longitude = o.Longitude,
                    ID = o.ID,
                    Street = o.Street,
                }).OrderBy(a => a.Latitude).ThenBy(a => a.Longitude).ThenBy(a => a.Street).ToList();*/
                objaddress = objaddress.OrderBy(a => a.Latitude).ThenBy(a => a.Longitude).ThenBy(a => a.Street).ToList();
            }

            return objaddress;

        }


        public List<AddressMaster> GetUnAllocatedSkipAllAddress(List<string> Root, List<string> Cycle, long projectId)
        {

            CompassEntities dbcontext = new CompassEntities();
            List<AddressMaster> objaddress = new List<AddressMaster>();
            string routeVal = "";
            string cycleVal = "";
            foreach (var item in Cycle)
            {
                if (cycleVal.Length == 0)
                {
                    cycleVal = item.Trim();
                }
                else
                {
                    cycleVal += "," + item.Trim() + "";
                }
            }
            foreach (var item in Root)
            {
                if (routeVal.Length == 0)
                {
                    routeVal = item.Trim();
                }
                else
                {
                    routeVal += "," + item.Trim();
                }
            }

            if (Root != null)
            {
                SqlConnection sqlConnection1 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;
                cmd.CommandText = "PROC_InstallerAllocation_GetAllUnallocatedSkipVisits_ByCycleRoute";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = sqlConnection1;
                cmd.Parameters.AddWithValue("@ProjectId", projectId);
                cmd.Parameters.AddWithValue("@Cycle", cycleVal);
                cmd.Parameters.AddWithValue("@Route", routeVal);
                sqlConnection1.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    objaddress.Add(new AddressMaster
                    {
                        ID = long.Parse(reader["id"].ToString()),
                        Street = reader["Street"].ToString(),
                        Latitude = reader["AddressLatitude"].ToString(),
                        Longitude = reader["AddressLongitude"].ToString(),
                        Cycle = reader["Cycle"].ToString(),
                        Route = reader["Route"].ToString()
                    });

                }
                sqlConnection1.Close();
                reader.Close();
                reader.Dispose();
                cmd.Dispose();
                sqlConnection1.Dispose();
                objaddress = objaddress.OrderBy(a => a.Latitude).ThenBy(a => a.Longitude).ThenBy(a => a.Street).ToList();
            }

            return objaddress;

        }
        /// <summary>
        /// Save mapped data in database for given installer.
        /// </summary>
        /// <param name="InstallerMap"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        public bool create(InstallerMap InstallerMap, out string Message, out List<string> DeallocatedItem, out bool Flag)
        {
            bool result = false;
            Flag = false;
            Message = "Unable to Save";
            DeallocatedItem = new List<string>();
            DateTime dt = new CommonFunctions().ServerDate();
            try
            {
                string routeValues = string.Join(",", InstallerMap.RouteValues.Select(x => $"'{x}'"));
                string cycleValues = string.Join(",", InstallerMap.CycleValues.Select(x => $"'{x}'"));

                CompassEntities dbcontext = new CompassEntities();
                tblInstallerMapData objtblInstallerMapData = new tblInstallerMapData();

                List<CurrentVisitRecord> objCurrentVisitList = new List<CurrentVisitRecord>();
                if (InstallerMap.AddressID == null)
                {

                    DateTime fromDate = DateTime.Parse(InstallerMap.FromDate);
                    DateTime toDate = DateTime.Parse(InstallerMap.ToDate);

                    //gets the old record deactivatelsist
                    //objDeactiveList = dbcontext.PROC_ListOfDeactivatesVisits(InstallerMap.ProjectID, fromDate, toDate, InstallerMap.InstallerID).ToList();

                    SqlConnection sqlConnection1 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;

                    cmd.CommandText = "PROC_InstallerAllocation_ViewCurrentVisits";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = sqlConnection1;
                    cmd.Parameters.Add("@ProjectId", InstallerMap.ProjectID);
                    cmd.Parameters.Add("@fromDate", fromDate);
                    cmd.Parameters.Add("@toDate", toDate);
                    cmd.Parameters.Add("@InstallerId", InstallerMap.InstallerID);
                    cmd.Parameters.Add("@RouteList", routeValues);
                    cmd.Parameters.Add("@CycleList", cycleValues);

                    sqlConnection1.Open();

                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        objCurrentVisitList.Add(new CurrentVisitRecord
                        {
                            ID = long.Parse(reader["ID"].ToString()),
                            Account = reader["Account"].ToString(),
                            Customer = reader["Customer"].ToString(),
                            FromDate = DateTime.Parse(reader["FromDate"].ToString()),
                            InstallerId = long.Parse(reader["InstallerId"].ToString()),
                            OldMeterNo = reader["OldMeterNo"].ToString(),
                            OldMeterRadioNo = reader["OldMeterRadioNo"].ToString(),
                            FK_UploadedExcelData_Id = long.Parse(reader["FK_UploadedExcelData_Id"].ToString()),
                            ToDate = DateTime.Parse(reader["ToDate"].ToString())
                        });

                    }

                    sqlConnection1.Close();
                    reader.Close();
                    reader.Dispose();
                    cmd.Dispose();
                    sqlConnection1.Dispose();



                    ///Stored procedure used to get existing visit info for the same date & deactivates only pending records.
                    //CompassEntities.PROC_DeactivateVisits(InstallerMap.ProjectID, fromDate, toDate, InstallerMap.InstallerID);


                    SqlConnection sqlConnection2 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
                    SqlCommand cmd2 = new SqlCommand();


                    cmd2.CommandText = "PROC_InstallerAllocation_DeactivateCurrentVisits";
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Connection = sqlConnection2;
                    cmd2.Parameters.Add("@ProjectId", InstallerMap.ProjectID);
                    cmd2.Parameters.Add("@fromDate", fromDate);
                    cmd2.Parameters.Add("@toDate", toDate);
                    cmd2.Parameters.Add("@InstallerId", InstallerMap.InstallerID);
                    cmd2.Parameters.Add("@RouteList", routeValues);
                    cmd2.Parameters.Add("@CycleList", cycleValues);

                    sqlConnection2.Open();
                    cmd2.ExecuteNonQuery();

                    sqlConnection2.Close();
                    cmd2.Dispose();
                    sqlConnection2.Dispose();
                    DeallocatedItem = objCurrentVisitList.Select(o => o.FK_UploadedExcelData_Id.ToString()).ToList();
                }

                if (InstallerMap != null && InstallerMap.AddressID != null && InstallerMap.AddressID.Count > 0)
                {
                    

                    DateTime fromDate = InstallerMap.OldFromDate == null ? DateTime.Parse(InstallerMap.FromDate) : DateTime.Parse(InstallerMap.OldFromDate);
                    DateTime toDate = InstallerMap.OldToDate == null ? DateTime.Parse(InstallerMap.ToDate) : DateTime.Parse(InstallerMap.OldToDate);


                    //gets the old record deactivatelsist
                    //objDeactiveList = CompassEntities.PROC_ListOfDeactivatesVisits(InstallerMap.ProjectID, fromDate, toDate, InstallerMap.InstallerID).ToList();

                    SqlConnection sqlConnection1 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;

                    cmd.CommandText = "PROC_InstallerAllocation_ViewCurrentVisits";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = sqlConnection1;
                    cmd.Parameters.Add("@ProjectId", InstallerMap.ProjectID);
                    cmd.Parameters.Add("@fromDate", fromDate);
                    cmd.Parameters.Add("@toDate", toDate);
                    cmd.Parameters.Add("@InstallerId", InstallerMap.InstallerID);
                    cmd.Parameters.Add("@RouteList", routeValues);
                    cmd.Parameters.Add("@CycleList", cycleValues);

                    sqlConnection1.Open();

                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        objCurrentVisitList.Add(new CurrentVisitRecord
                        {
                            ID = long.Parse(reader["ID"].ToString()),
                            Account = reader["Account"].ToString(),
                            Customer = reader["Customer"].ToString(),
                            FromDate = DateTime.Parse(reader["FromDate"].ToString()),
                            InstallerId = long.Parse(reader["InstallerId"].ToString()),
                            OldMeterNo = reader["OldMeterNo"].ToString(),
                            OldMeterRadioNo = reader["OldMeterRadioNo"].ToString(),
                            FK_UploadedExcelData_Id = long.Parse(reader["FK_UploadedExcelData_Id"].ToString()),
                            ToDate = DateTime.Parse(reader["ToDate"].ToString())
                        });

                    }

                    sqlConnection1.Close();
                    reader.Close();
                    reader.Dispose();
                    cmd.Dispose();
                    sqlConnection1.Dispose();


                    ///Stored procedure used to get existing visit info for the same date & deactivates only pending records.
                    //CompassEntities.PROC_DeactivateVisits(InstallerMap.ProjectID, fromDate, toDate, InstallerMap.InstallerID);


                    SqlConnection sqlConnection2 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
                    SqlCommand cmd2 = new SqlCommand();


                    cmd2.CommandText = "PROC_InstallerAllocation_DeactivateCurrentVisits";
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Connection = sqlConnection2;
                    cmd2.Parameters.Add("@ProjectId", InstallerMap.ProjectID);
                    cmd2.Parameters.Add("@fromDate", fromDate);
                    cmd2.Parameters.Add("@toDate", toDate);
                    cmd2.Parameters.Add("@InstallerId", InstallerMap.InstallerID);
                    cmd2.Parameters.Add("@RouteList", routeValues);
                    cmd2.Parameters.Add("@CycleList", cycleValues);

                    sqlConnection2.Open();
                    cmd2.ExecuteNonQuery();

                    sqlConnection2.Close();
                    cmd2.Dispose();
                    sqlConnection2.Dispose();



                    ///iterate new mapped address list.
                    foreach (var item in InstallerMap.AddressID)
                    {
                        var ID = Convert.ToInt64(item);
                        ///find if address already mapped to current installer.
                        var installerdata = dbcontext.tblInstallerMapDatas.Where(a => a.FK_UploadedExcelData_Id == ID
                            && a.InstallerId == InstallerMap.InstallerID).OrderByDescending(a => a.ID).FirstOrDefault();


                        if (installerdata != null)
                        {
                            /// if record present
                            /// Update existing record.
                            if (installerdata.Active == 0)
                            {
                                installerdata.MobileResponse = null;
                                installerdata.BatchNumber = null;
                                installerdata.SynchCheckSumString = null;
                                installerdata.SynchWithMobileDate = null;
                                installerdata.SynchWithMobileFlag = null;
                                installerdata.FromDate = Convert.ToDateTime(InstallerMap.FromDate);
                                installerdata.ToDate = Convert.ToDateTime(InstallerMap.ToDate);
                                installerdata.Active = 1;
                            }
                            //installerdata.Active = 1;
                            dbcontext.SaveChanges();
                        }
                        else
                        {
                            ///if record not present
                            ///Insert new record
                            objtblInstallerMapData = new tblInstallerMapData();
                            objtblInstallerMapData.InstallerId = InstallerMap.InstallerID;
                            //objtblInstallerMapData.tblUploadedData_ID = Convert.ToInt64(item);
                            objtblInstallerMapData.FK_UploadedExcelData_Id = Convert.ToInt64(item);

                            if (!string.IsNullOrEmpty(InstallerMap.FromDate) && !string.IsNullOrEmpty(InstallerMap.ToDate))
                            {
                                objtblInstallerMapData.FromDate = Convert.ToDateTime(InstallerMap.FromDate);
                                objtblInstallerMapData.ToDate = Convert.ToDateTime(InstallerMap.ToDate);
                            }
                            objtblInstallerMapData.Active = 1;
                            objtblInstallerMapData.CreatedBy = 1;
                            objtblInstallerMapData.CreatedOn = dt;
                            dbcontext.tblInstallerMapDatas.Add(objtblInstallerMapData);
                            dbcontext.SaveChanges();
                        }
                    }



                    if (objCurrentVisitList != null && objCurrentVisitList.Count > 0)
                    {
                        var ObjList = objCurrentVisitList.Select(o => o.FK_UploadedExcelData_Id.ToString()).ToList();
                        var ObjdeletedList = ObjList.Except(InstallerMap.AddressID).ToList();
                        DeallocatedItem = ObjdeletedList;

                        // updated existing data
                        Flag = false;
                    }
                    else
                    {
                        //new entry for current date range 
                        Flag = true;
                        DeallocatedItem = objCurrentVisitList.Select(o => o.FK_UploadedExcelData_Id.ToString()).ToList();
                    }
                }
                // This code comment because this code already wirte on the DD TNMP Utlity
                /*if (InstallerMap.InstallerID != null && InstallerMap.InstallerID > 0)
                {
                    //Skip account move to unallocate
                    var objTempList = objTempSkipList.Select(o => o.ID.ToString()).ToList();
                    var SkipDeleteList = objTempList.Except(InstallerMap.AddressID).ToList();
                    if (SkipDeleteList != null)
                    { SkipAccountMovedToUnAllocate(SkipDeleteList, InstallerMap.InstallerID); }
                }*/
                Message = "Installer mapped to address successfully";
                result = true;
            }
            catch (Exception ex)
            {

                Message += "<br/>" + ex.Message;
            }
            return result;
        }

        public void SkipAccountMovedToUnAllocate(List<string> objSkipDeleteList,long InstallerId)
        {
            try
            {
                CompassEntities compassEntities = new CompassEntities();
                foreach (var item in objSkipDeleteList)
                {
                    bool flag = true;
                    var ID = Convert.ToInt64(item);
                    var objTPFD = compassEntities.TblProjectFieldDatas.Where(o => o.FK_UploadedExcelData_Id == ID && o.Active == 1 && o.InstallerId == InstallerId && o.IsSkipped == true).OrderByDescending(o => o.ID).ToList();
                    foreach (var itemRow in objTPFD)
                    {
                        itemRow.Active = 0;
                        itemRow.ModifiedOn = new CommonFunctions().ServerDate();
                        compassEntities.SaveChanges();

                        #region history inserted
                        if (flag)       // Last record insert in History  Table
                        {
                            TblProjectFieldDataHistory objTblProjectFieldDataHistory = new TblProjectFieldDataHistory();
                            objTblProjectFieldDataHistory.Active = 1;
                            objTblProjectFieldDataHistory.CreatedBy = 1;
                            objTblProjectFieldDataHistory.CreatedOn = new CommonFunctions().ServerDate();
                            objTblProjectFieldDataHistory.FormId = itemRow.FormId;
                            objTblProjectFieldDataHistory.InstallerId = itemRow.InstallerId;
                            objTblProjectFieldDataHistory.InstallerMapId = itemRow.InstallerId;
                            objTblProjectFieldDataHistory.IsReAssign = itemRow.IsReAssign;
                            objTblProjectFieldDataHistory.IsSkipped = itemRow.IsSkipped;
                            objTblProjectFieldDataHistory.JsonDataVal = itemRow.JsonDataVal;
                            objTblProjectFieldDataHistory.SkipComment = itemRow.SkipComment;
                            objTblProjectFieldDataHistory.SkipId = itemRow.SkipId;
                            objTblProjectFieldDataHistory.SkippedDatetime = itemRow.SkippedDatetime;
                            objTblProjectFieldDataHistory.SkippedReason = itemRow.SkippedReason;
                            // objTblProjectFieldDataHistory.tblUploadedData_Id = objTblProjectFieldData.tblUploadedData_Id;
                            objTblProjectFieldDataHistory.FK_UploadedExcelData_Id = itemRow.FK_UploadedExcelData_Id;
                            objTblProjectFieldDataHistory.visitdatetime = itemRow.visitdatetime;

                            compassEntities.TblProjectFieldDataHistories.Add(objTblProjectFieldDataHistory);
                            compassEntities.SaveChanges();

                            TblUnAllocatedSkipHistory objTblUnAllocatedSkipHistory = new TblUnAllocatedSkipHistory();
                            objTblUnAllocatedSkipHistory.FK_UploadedExcelData_Id = itemRow.FK_UploadedExcelData_Id;
                            objTblUnAllocatedSkipHistory.TblProjectFieldData_Id = itemRow.ID;
                            objTblUnAllocatedSkipHistory.InstallerId = itemRow.InstallerId;
                            objTblUnAllocatedSkipHistory.InstallerName = compassEntities.tblUsers.Where(o => o.UserID == itemRow.InstallerId).Select(o => o.FirstName).FirstOrDefault();
                            objTblUnAllocatedSkipHistory.visitdatetime = itemRow.visitdatetime;
                            objTblUnAllocatedSkipHistory.SkippedReason = itemRow.SkippedReason;
                            objTblUnAllocatedSkipHistory.SkipComment = itemRow.SkipComment;
                            objTblUnAllocatedSkipHistory.Active = 1;
                            objTblUnAllocatedSkipHistory.CreatedBy = 1;
                            objTblUnAllocatedSkipHistory.CreatedOn = new CommonFunctions().ServerDate();
                            compassEntities.TblUnAllocatedSkipHistories.Add(objTblUnAllocatedSkipHistory);
                            compassEntities.SaveChanges();

                            flag = false;
                        }
                        #endregion
                    }
                    var objIMD = compassEntities.tblInstallerMapDatas.Where(o => o.FK_UploadedExcelData_Id == ID && o.Active == 1 && o.InstallerId == InstallerId).ToList();
                    foreach (var objItemRow in objIMD)
                    {
                        objItemRow.Active = 0;
                        objItemRow.BatchNumber = null;
                        objItemRow.MobileResponse = null;
                        objItemRow.ModifiedOn = new CommonFunctions().ServerDate();
                        compassEntities.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

        }

        //public List<projectStatus> ProjectStatus(List<long> Address, long ProjectID,
        //    long InstallerID, string InstallerName, string FromDate, string ToDate, out string Message, ref bool status)
        //{
        //    status = false;
        //    Message = "";
        //    List<projectStatus> Get_lat_lagnew = new List<projectStatus>();
        //    try
        //    {
        //        CompassEntities dbcontext = new CompassEntities();

        //        if (Address != null && ProjectID != 0 && InstallerID != 0 && InstallerName != "" && FromDate != "" && ToDate != "")
        //        {


        //            //Get_lat_lagnew = (from m in dbcontext.tblUploadedDatas.AsParallel()
        //            //                  where Address.Contains(m.ID)
        //            Get_lat_lagnew = (from m in dbcontext.STP_GetInstaAllocExportData(ProjectID, InstallerID)
        //                              where Address.Contains(m.id)

        //                              select new projectStatus
        //                              {
        //                                  ClientName = m.clientname,

        //                                  ProjectName = m.projectname,
        //                                  InstallerName = InstallerName,
        //                                  StartDate = FromDate,
        //                                  EndDate = ToDate,
        //                                  Cycle = m.cycle,
        //                                  Route = m.route,
        //                                  City = m.cityname,
        //                                  Latitude = m.Latitude,
        //                                  Longitude = m.Longitude,
        //                                  Street = m.Street,
        //                                  Status = "",
        //                                  UploadID = m.id
        //                              }).ToList();


        //            //#region installer export status
        //            //foreach (var item in Get_lat_lagnew)
        //            //{
        //            //    item.Status = "Pending";
        //            //    var objFieldData = dbcontext.TblProjectFieldDatas.Where(o => o.tblUploadedData_Id == item.UploadID && o.Active == 1
        //            //        ).FirstOrDefault();


        //            //    if (objFieldData != null)
        //            //    {
        //            //        var result2 = dbcontext.tblProjectFieldDataMasters.Where(o => o.Active == 1
        //            //            && o.InitialFieldName == "WORK COMPLETED" && o.TblProjectFieldData_Id == objFieldData.ID).FirstOrDefault();
        //            //        if (result2 != null && result2.FieldValue == "true")
        //            //        {
        //            //            item.Status = "Completed";
        //            //        }
        //            //        else
        //            //        {
        //            //            item.Status = "Pending";
        //            //        }

        //            //        var objFieldData1 = dbcontext.TblProjectFieldDatas.Where(o => o.tblUploadedData_Id == item.UploadID && o.Active == 1
        //            //       && o.IsSkipped == true).FirstOrDefault();
        //            //        if (objFieldData1 != null)
        //            //        {
        //            //            item.Status = "Skipped";
        //            //        }
        //            //    }

        //            //}
        //            //#endregion

        //            //4-completed
        //            //3-skipped

        //            //2-pending
        //            //1-not mapped
        //            //5-partially done
        //            #region Project Status
        //            List<addressVisitInfo> objVisitList = new List<addressVisitInfo>();
        //            var activeFormList = dbcontext.tblFormMasters.Where(o => o.ProjectId == ProjectID && o.Active == 1).ToList();
        //            foreach (var item1 in Get_lat_lagnew)
        //            {
        //                var totalaccount = Get_lat_lagnew.Where(o => o.Latitude == item1.Latitude && o.Longitude == item1.Longitude).ToList();
        //                if (totalaccount.Count == 1)
        //                {
        //                    #region Single address for latlong  logic
        //                    addressVisitInfo obj2 = new addressVisitInfo();
        //                    obj2.latitude = Convert.ToString(item1.Latitude);
        //                    obj2.longitude = Convert.ToString(item1.Longitude);

        //                    int totalVisitCount = activeFormList.Count;
        //                    int visitedCount = 0;

        //                    foreach (var formName in activeFormList)
        //                    {
        //                        var objFieldData = dbcontext.TblProjectFieldDatas.Where(o => o.tblUploadedData_Id == item1.UploadID && o.Active == 1
        //                            && o.FormId == formName.FormId).FirstOrDefault();


        //                        if (objFieldData != null)
        //                        {
        //                            var result2 = dbcontext.tblProjectFieldDataMasters.Where(o => o.Active == 1
        //                                && o.InitialFieldName == "WORK COMPLETED" && o.TblProjectFieldData_Id == objFieldData.ID).FirstOrDefault();
        //                            if (result2 != null && result2.FieldValue == "true")
        //                            {
        //                                visitedCount = visitedCount + 1;
        //                            }
        //                        }
        //                    }
        //                    //int totalVisit = totalVisitCount == visitedCount ? totalVisit + 1 : totalVisit;
        //                    if (totalVisitCount != 0)
        //                    {
        //                        obj2.colorFlag = totalVisitCount == visitedCount ? 4 : (visitedCount > 0 ? 2 : 2);
        //                    }
        //                    else
        //                    {
        //                        item1.Status = "Pending";
        //                    }
        //                    if (obj2.colorFlag == 2)
        //                    {

        //                        var objFieldData = dbcontext.TblProjectFieldDatas.Where(o => o.tblUploadedData_Id == item1.UploadID && o.Active == 1
        //                           && o.IsSkipped == true).FirstOrDefault();
        //                        if (objFieldData != null)
        //                        {
        //                            item1.Status = "Skipped";
        //                        }
        //                        else
        //                        {
        //                            item1.Status = "Pending";
        //                        }
        //                    }
        //                    else if (obj2.colorFlag == 4)
        //                    {
        //                        item1.Status = "Completed";
        //                    }



        //                    objVisitList.Add(obj2);

        //                    #endregion
        //                }

        //                else
        //                {
        //                    #region Multiple Addresses for latlong  logic
        //                    var result = objVisitList.Where(o => o.latitude == Convert.ToString(item1.Latitude) && o.longitude == Convert.ToString(item1.Longitude)).FirstOrDefault();


        //                    if (result == null)
        //                    {
        //                        addressVisitInfo obj2 = new addressVisitInfo();
        //                        obj2.latitude = Convert.ToString(item1.Latitude);
        //                        obj2.longitude = Convert.ToString(item1.Longitude);


        //                        #region uncomment
        //                        var AccountList = dbcontext.STP_UserVisitsProjectStatus(InstallerID, ProjectID).ToList();
        //                        var objAccountsList = AccountList.Where(o => o.Street == item1.Street && o.Latitude == item1.Latitude
        //                            && o.Longitude == item1.Longitude && o.ProjectId == ProjectID).ToList();


        //                        int totalVisit = 0;
        //                        int totalVisitCount = activeFormList.Count;
        //                        int totalAccount = objAccountsList.Count * totalVisitCount;

        //                        int skippedCount = 0;
        //                        foreach (var account in objAccountsList)
        //                        {
        //                            int visitedCount = 0;

        //                            foreach (var formName in activeFormList)
        //                            {
        //                                var objFieldData = dbcontext.TblProjectFieldDatas.Where(o => o.tblUploadedData_Id == account.ID && o.Active == 1
        //                                    && o.FormId == formName.FormId).FirstOrDefault();

        //                                if (objFieldData != null)
        //                                {
        //                                    var result2 = dbcontext.tblProjectFieldDataMasters.Where(o => o.Active == 1
        //                                        && o.InitialFieldName == "WORK COMPLETED" && o.TblProjectFieldData_Id == objFieldData.ID).FirstOrDefault();
        //                                    if (result2 != null && result2.FieldValue == "true")
        //                                    {
        //                                        visitedCount = visitedCount + 1;
        //                                    }
        //                                }

        //                                //skipped records
        //                                var skippedrecords = dbcontext.TblProjectFieldDatas.Where(o => o.tblUploadedData_Id == account.ID && o.Active == 1
        //                                        && o.IsSkipped == true).FirstOrDefault();
        //                                if (skippedrecords != null)
        //                                {
        //                                    skippedCount = skippedCount + 1;
        //                                    //obj2.colorFlag = 3;
        //                                }
        //                            }

        //                            totalVisit = totalVisit + visitedCount;
        //                        }

        //                        //  obj2.colorFlag = totalVisit == totalAccount ? 4 : (totalVisit > 0 ? 2 : 2);

        //                        if (totalVisit == 0 && skippedCount == 0)
        //                        {
        //                            item1.Status = "Pending";
        //                        }
        //                        else if (totalVisit > 0 && totalVisit != totalAccount && skippedCount == 0)
        //                        {
        //                            item1.Status = "Partially done";
        //                        }
        //                        else if (totalVisit == 0 && skippedCount > 0)
        //                        {
        //                            item1.Status = "Skipped";
        //                        }
        //                        else if (totalVisit > 0 && totalVisit == totalAccount)
        //                        {
        //                            item1.Status = "Completed";
        //                        }
        //                        else if (totalVisit > 0 && skippedCount > 0 && (totalVisit + skippedCount) != totalAccount)
        //                        {
        //                            item1.Status = "Partially done";
        //                        }
        //                        else if (totalVisit > 0 && skippedCount > 0 && (totalVisit + skippedCount) == totalAccount)
        //                        {
        //                            item1.Status = "Skipped";
        //                        }

        //                        #endregion

        //                        // objVisitList.Add(obj2);
        //                    }
        //                    else
        //                    {

        //                        //string[] checkAddressList = result.street.Split(',');

        //                        //var result1 = checkAddressList.Where(o => o.Trim().ToLower() == item1.Street.Trim().ToLower()).FirstOrDefault();
        //                        //if (string.IsNullOrEmpty(result1))
        //                        //{
        //                        //    result.street = result.street + ",<br/>" + item1.Street;
        //                        //}
        //                        //if (item1.IsVisited == "false")
        //                        //{
        //                        //    result.isVisited = false;
        //                        //}



        //                    }
        //                    #endregion
        //                }
        //            }
        //            #endregion


        //            status = true;
        //        }
        //        else
        //        {
        //            status = false;
        //            Message = "Please select required fields";
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        status = false;
        //        Message = ex.Message;
        //    }
        //    return Get_lat_lagnew;
        //}


        #region All


        /// <summary>
        /// 
        /// </summary>
        /// <param name="formId"></param>
        /// <param name="latlongRequired"></param>
        /// <param name="mediaRequired"></param>
        /// <param name="objProject"></param>
        /// <param name="FormName"></param>
        /// <param name="columnNameList1"></param>
        /// <returns></returns>
        public List<InstallerDataStatus> GetDynamicAllRecordDataTable(DateTime fromDate, DateTime toDate, long installerId, long projectId)
        {



            CompassEntities dbcontext = new CompassEntities();

            long formId = dbcontext.tblFormMasters.Where(o => o.Active == 1 && o.FormTypeID == 2
                && o.ProjectId == projectId).Select(o => o.FormId).FirstOrDefault();



            List<InstallerDataStatus> objList = new List<InstallerDataStatus>();
            SqlConnection sqlConnection1 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            cmd.CommandText = "PROC_InstallerAllocation_GetInstallerData";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = sqlConnection1;
            cmd.Parameters.Add("@formId", formId);
            cmd.Parameters.Add("@InstallerId", installerId);
            cmd.Parameters.Add("@FromDate", fromDate);
            cmd.Parameters.Add("@ToDate", toDate);
            cmd.CommandTimeout = 600;

            sqlConnection1.Open();

            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                objList.Add(new InstallerDataStatus
                {

                    CustomerName = reader["Client Name"].ToString(),
                    ProjectName = reader["Project Name"].ToString(),
                    City = reader["City"].ToString(),
                    StartDate = reader["Visit Start Date"].ToString(),
                    EndDate = reader["Visit End Date"].ToString(),
                    InstallerName = reader["Installer"].ToString(),
                    Cycle = reader["Cycle"].ToString(),
                    Route = reader["Route"].ToString(),
                    Street = reader["Street"].ToString(),
                    Latitude = reader["Latitude"].ToString(),
                    Longitude = reader["Longitude"].ToString(),
                    AccountNumber = reader["Account"].ToString(),
                    OldMeterNumber = reader["OldMeterNo"].ToString(),
                    OldRadioNumber = reader["OldMeterRadioNo"].ToString(),
                    VisitedDate = Convert.ToString(reader["Visit DateTime"]) != "1/1/1900 12:00:00 AM" ? Convert.ToString(reader["Visit DateTime"]) : "-",
                    VisitStatus = reader["VisitStatus"].ToString(),
                    Reason = reader["Reason"].ToString(),
                    Comment = reader["Comment"].ToString(),
                    UploadID = long.Parse(reader["Id"].ToString())

                });

            }

            sqlConnection1.Close();
            reader.Close();
            reader.Dispose();
            cmd.Dispose();
            sqlConnection1.Dispose();


            return objList;
        }






        public List<InstallerDataStatus> GetAllRecordDataTable(DateTime fromDate, DateTime toDate, long installerId, long projectId)
        {


            List<InstallerDataStatus> objInstallerDataStatusList = new List<InstallerDataStatus>();

            CompassEntities objCompassEntities = new CompassEntities();

            long formId = objCompassEntities.tblFormMasters.Where(o => o.Active == 1 && o.FormTypeID == 2
                && o.ProjectId == projectId).Select(o => o.FormId).FirstOrDefault();

            objInstallerDataStatusList = objCompassEntities.Proc_GetInstallerFormStatusData(formId, installerId, fromDate, toDate).Select
                (o => new InstallerDataStatus
                {
                    UploadID = o.ID,
                    CustomerName = o.Client_Name,
                    ProjectName = o.Project_Name,
                    City = o.City,
                    StartDate = o.Visit_Start_Date.ToString(),
                    EndDate = o.Visit_End_Date.ToString(),
                    InstallerName = o.Installer,
                    Cycle = o.Cycle,
                    Route = o.Route,
                    Street = o.Street,
                    Latitude = o.Latitude,
                    Longitude = o.Longitude,
                    AccountNumber = o.Account,
                    OldMeterNumber = o.OldMeterNo,
                    OldRadioNumber = o.OldMeterRadioNo,
                    VisitedDate = o.Visit_DateTime.Value.ToString() != "1/1/1900 12:00:00 AM" ? o.Visit_DateTime.Value.ToString() : "-",
                    VisitStatus = o.VisitStatus,
                    Reason = o.Reason,
                    Comment = o.Comment
                }).ToList();
            return objInstallerDataStatusList;
        }




        #endregion


        #region Added code by vishal to send mail to installer

        public string GetSubject()
        {
            string subject = "";
            CompassEntities dbcontext = new CompassEntities();
            var ObjTblEmailConfigure = (from b in dbcontext.TblAlertMasters
                                        join c in dbcontext.TblEmailConfigures on b.AlertId equals c.AlertTableID
                                        where b.AlertId == 1
                                        select new EmailConfigureBO
                                        {
                                            Subject = c.Subject
                                        }).FirstOrDefault();

            if (ObjTblEmailConfigure != null)
            {
                subject = ObjTblEmailConfigure.Subject;
            }
            else
            {
                subject = null;
            }
            return subject;
        }

        public string GetMessage()
        {
            string Message = "";
            CompassEntities dbcontext = new CompassEntities();
            var ObjTblEmailConfigure = (from b in dbcontext.TblAlertMasters
                                        join c in dbcontext.TblEmailConfigures on b.AlertId equals c.AlertTableID
                                        where b.AlertId == 1
                                        select new EmailConfigureBO
                                        {
                                            Message = c.Message
                                        }).FirstOrDefault();

            if (ObjTblEmailConfigure != null)
            {
                Message = ObjTblEmailConfigure.Message;
            }
            else
            {
                Message = null;
            }
            return Message;
        }

        public List<TblEmailConfigureUserDetail> GetUserList()
        {
            List<TblEmailConfigureUserDetail> UserList = new List<TblEmailConfigureUserDetail>();
            CompassEntities dbcontext = new CompassEntities();
            var CCEmailId = (from b in dbcontext.TblAlertMasters
                             join c in dbcontext.TblEmailConfigures on b.AlertId equals c.AlertTableID
                             join d in dbcontext.TblEmailConfigureUserDetails on c.ID equals d.TblEmailConfigureID
                             where b.AlertId == 1
                             select new TblEmailConfigureUserDetail
                             {
                                 UserID = d.UserID
                             }).ToList();


            UserList = CCEmailId.ToList();


            return UserList;
        }
        public string SendMalil(InstallerMap InstallerMap, List<string> DeallocatedItem, bool Flag)
        {

            log.Info("Entry to SendMalil in  Mail send method at-" + DateTime.Now.ToString());


            DateTime dtserverdate = new CommonFunctions().ServerDate();
            StringBuilder sb = new StringBuilder();
            using (CompassEntities dbcontext = new CompassEntities())
            {


                /*var ProjectModel = (from b in dbcontext.tblProjects
                                    join c in dbcontext.tblStateMasters on b.StateId equals c.StateId
                                    join d in dbcontext.TblCityMasters on b.CityId equals d.CityId
                                    where b.ProjectId == InstallerMap.ProjectID
                                    select new ProjectModel
                                    {
                                        ProjectName = b.ProjectName,
                                        StateName = c.StateName,
                                        CityName = d.CityName,
                                        Utilitytype = b.Utilitytype
                                    }).FirstOrDefault();*/

                var ProjectModel = (from b in dbcontext.tblProjects
                                    join c in dbcontext.tblStateMasters on b.StateId equals c.StateId
                                    into s
                                    from st in s.DefaultIfEmpty()                 // Changes query because project creation (State or City) and (NationWide)
                                    join d in dbcontext.TblCityMasters on b.CityId equals d.CityId
                                    into c
                                    from ct in c.DefaultIfEmpty()
                                    where b.ProjectId == InstallerMap.ProjectID
                                    select new ProjectModel
                                    {
                                        ProjectName = b.ProjectName,
                                        StateName = st.StateName,
                                        CityName = ct.CityName,
                                        Utilitytype = b.Utilitytype
                                    }).FirstOrDefault();

                var Message = GetMessage();
                var InstallorName = dbcontext.tblUsers.Where(n => n.UserID == InstallerMap.InstallerID).FirstOrDefault();
                sb.Append("<body   style='padding: 10px 0px 0px 10px; background: #f8f8fb; font-family:Times New Roman; font-size:14px;'>");
                sb.Append("Dear " + InstallorName.FirstName + ",");

                sb.Append("  <p style='font-size:15px;'>Greetings from Data Depot....!!!</p>");
                if (Flag)
                    sb.Append("<br/> <p >This is to inform you that new work is allocated to you by Data Depot Team. Please check below details.</p><br/>");
                else
                    sb.Append("<br/><p >This is to inform you that your current allocated work is changed by Data Depot Team. Please check below details.</p><br/>");


                sb.Append("<div style='background: #e6e1e1; width:100%;'>" + Message + "</div><br/>");


                sb.Append("<div style='background: #e6e1e1; width:100%;'><table  style='border:none;padding: 0px 0px 0px 10px;'>" +
                              "<tr>" +
                                  "<td style='border:none;'><b>Project Name </b></td>" +
                                  "<td style='border:none;'><b>:</b></td>" +
                                  "<td style='border:none;' colspan='2'>" + ProjectModel.ProjectName.ToUpper() + "</td>" +
                              "</tr>" +

                              "<tr>" +
                                  "<td style='border:none;'><b>Utility Type</b></td>" +
                                  "<td style='border:none;'><b>:</b></td>" +
                                  "<td style='border:none;' colspan='4'>" + ProjectModel.Utilitytype + "</td>" +
                              "</tr>");

                if (!String.IsNullOrEmpty(ProjectModel.CityName) && !String.IsNullOrEmpty(ProjectModel.StateName))
                {
                    sb.Append("<tr>" +
                                       "<td style='border:none;'><b>City:</b></td>" +
                                       "<td style='border:none;'><b>:</b></td>" +
                                       " <td style='border:none;'>" + ProjectModel.CityName.ToUpper() + " &nbsp;&nbsp;</td>" +

                                       " <td style='border:none;'><b>State</b></td>" +
                                       " <td style='border:none;'><b>:</b></td>" +
                                       "<td style='border:none;'>" + ProjectModel.StateName.ToUpper() + "</td>" +
                                       " </tr>");
                }

                sb.Append("<tr>" +
                                 " <td style='border:none;'><b>Visit Start Date</b></td>" +
                                 " <td style='border:none;'><b>:</b></td>" +
                                  "<td style='border:none;'>" + InstallerMap.FromDate + "   &nbsp;&nbsp;</td>" +


                                  "<td style='border:none;'><b>Visit End Date</b></td>" +
                                  "<td style='border:none;'><b>:</b></td>" +
                                  "<td style='border:none;'>" + InstallerMap.ToDate + "   </td>" +
                             "</tr>" +
                             "<tr>" +
                                  "<td style='border:none;'><b>Allocated Accounts </b></td>" +
                                  "<td style='border:none;'><b>:</b></td>" +
                                  "<td style='border:none;'> " + (InstallerMap.AddressID != null ? InstallerMap.AddressID.Count : 0) + "</td>" +
                             "</tr>" +
                             "<tr>" +
                                  "<td style='border:none;'><b>Deallocated Accounts </b></td>" +
                                  "<td style='border:none;'><b>:</b></td>" +
                                  "<td style='border:none;' >" + (DeallocatedItem != null ? DeallocatedItem.Count : 0) + "</td>" +
                             "</tr>" +
                            "</table></div>");





                List<CurrentVisitRecordRow> objCurrentVisitList = new List<CurrentVisitRecordRow>();
                SqlConnection sqlConnection1 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;

                cmd.CommandText = "PROC_DynamicRow_ByProject";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = sqlConnection1;
                cmd.Parameters.Add("@ProjectId", InstallerMap.ProjectID);


                sqlConnection1.Open();

                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    objCurrentVisitList.Add(new CurrentVisitRecordRow
                    {
                        ID = long.Parse(reader["Id"].ToString()),
                        OldMeterNo = Convert.ToString(reader["OldMeterNo"]),
                        OldMeterRadioNo = Convert.ToString(reader["OldMeterRadioNo"]),
                        //Account = Convert.ToString(reader["Account"]),    // Remove deepak for Client Requirement
                        Customer = Convert.ToString(reader["Customer"]),
                        //FromDate = DateTime.Parse(reader["FromDate"].ToString()),
                        // ToDate = DateTime.Parse(reader["ToDate"].ToString()),
                        Cycle = Convert.ToString(reader["Cycle"]),
                        Route = Convert.ToString(reader["Route"]),
                        Street = Convert.ToString(reader["Street"]),
                        OldMeterSize= Convert.ToString(reader["OldMeterSize"]), // Add new deepak for Client Requirement
                    });

                }

                sqlConnection1.Close();
                reader.Close();
                reader.Dispose();
                cmd.Dispose();
                sqlConnection1.Dispose();
                if (InstallerMap.AddressID != null && InstallerMap.AddressID.Count > 0)
                {

                    sb.Append("<br /><b>Allocated Accounts</b><br /><br />");



                    sb.Append("<table style='border-collapse: collapse;padding: 0px 0px 0px 10px;'>");
                    sb.Append("<thead>" +
                        "<tr style='background-color:#80B2DF;'> " +
                            "<th style='border: 1px solid black;padding: 0px 8px;'>Sr.No</th> " +
                            "<th style='border: 1px solid black;padding: 0px 8px;'>Cycle</th> " +
                            "<th style='border: 1px solid black;padding: 0px 10px;'>Route</th> " +
                            //"<th style='border: 1px solid black;padding: 0px 10px;'>Account Number</th> " +
                            "<th style='border: 1px solid black;padding: 0px 20px;'>Customer Name</th> " +
                            "<th style='border: 1px solid black;padding: 0px 34px;'>Street</th> " +
                            "<th style='border: 1px solid black;padding: 0px 10px;'>Old Meter Size</th> " +
                            "<th style='border: 1px solid black;padding: 0px 10px;'>Old Meter Number</th> " +
                            "<th style='border: 1px solid black;padding: 0px 10px;'>Old Meter Radio Number</th> " +
                        "</tr>" +
                    "</thead>");


                    int i = 0;
                    int j = 1;
                    if (InstallerMap.AddressID.Count > 0)
                    {
                        i = InstallerMap.AddressID.Count;
                        sb.Append("<tbody style='font-size:13px;'>");

                        foreach (var item in InstallerMap.AddressID)
                        {
                            var ID = Convert.ToInt64(item);

                            var installerdata = objCurrentVisitList.Where(a => a.ID == ID).FirstOrDefault();



                            if (installerdata != null)
                            {

                                sb.Append("<tr" + ((i % 2 == 0) ? " style='font-size:13px; background-color: #c1dffa; text-align: center;' >" :
                                   " style='font-size:13px; text-align: center;'>"));
                                sb.Append("<td style='border: 1px solid black;font-weight:500; text-align: center; width:5%;'>" + j + "</td>");
                                sb.Append("<td style='border: 1px solid black;font-weight:500; text-align: center; width:5%;'>" + installerdata.Cycle + "</td>");
                                sb.Append("<td style='border: 1px solid black;font-weight:500; text-align: center; width:5%;'>" + installerdata.Route + "</td>");
                                //sb.Append("<td style='border: 1px solid black;font-weight:500; text-align: center; width:10%;'>" + installerdata.Account + "</td>");
                                sb.Append("<td style='border: 1px solid black;font-weight:500; text-align: center; width:25%;'>" + (!string.IsNullOrEmpty(installerdata.Customer) ? installerdata.Customer : "-") + "</td>");
                                sb.Append("<td style='border: 1px solid black;font-weight:500; text-align: center; width:30%;'>" + installerdata.Street + "</td>");
                                sb.Append("<td style='border: 1px solid black;font-weight:500; text-align: center; width:10%;'>" + (!string.IsNullOrEmpty(installerdata.OldMeterSize) ? installerdata.OldMeterSize : "-") + "</td>");
                                sb.Append("<td style='border: 1px solid black;font-weight:500; text-align: center; width:10%;'>" + (!string.IsNullOrEmpty(installerdata.OldMeterNo) ? installerdata.OldMeterNo : "-") + "</td>");
                                sb.Append("<td style='border: 1px solid black;font-weight:500; text-align: center; width:10%;'>" + (!string.IsNullOrEmpty(installerdata.OldMeterRadioNo) ? installerdata.OldMeterRadioNo : "-") + "</td>");
                                sb.Append("</tr>");

                            }
                            i = i - 1;
                            j++;
                        }

                        sb.Append("</tbody>");
                    }

                    sb.Append("</table><br/>");
                }
                if (DeallocatedItem.Count > 0)
                {
                    sb.Append(" <p><b> Deallocated Accounts </b></p><br />");


                    sb.Append("<table style='border-collapse: collapse;padding: 0px 0px 0px 10px;'>");
                    sb.Append("<thead>" +
                        "<tr style='font-size:13px; background-color:#80B2DF;'> " +
                            "<th style='border: 1px solid black;padding: 0px 8px;'>Sr.No</th> " +
                            "<th style='border: 1px solid black;padding: 0px 8px;'>Cycle</th> " +
                            "<th style='border: 1px solid black;padding: 0px 10px;'>Route</th> " +
                            //"<th style='border: 1px solid black;padding: 0px 10px;'>Account Number</th> " +
                            "<th style='border: 1px solid black;padding: 0px 20px;'>Customer Name</th> " +
                            "<th style='border: 1px solid black;padding: 0px 34px;'>Street</th> " +
                            "<th style='border: 1px solid black;padding: 0px 10px;'>Old Meter Size</th> " +
                            "<th style='border: 1px solid black;padding: 0px 10px;'>Old Meter Number</th> " +
                            "<th style='border: 1px solid black;padding: 0px 10px;'>Old Meter Radio Number</th> " +
                        "</tr>" +
                    "</thead>");
                    int k = 0;
                    int l = 1;
                    if (DeallocatedItem.Count > 0)
                    {
                        k = DeallocatedItem.Count;
                        sb.Append("<tbody style='font-size:13px;'>");
                        foreach (var item in DeallocatedItem)
                        {
                            var ID = Convert.ToInt64(item);
                            var Deinstallerdata = objCurrentVisitList.Where(a => a.ID == ID).FirstOrDefault();
                            if (Deinstallerdata != null)
                            {
                                sb.Append("<tr" + ((l % 2 == 0) ? " style='font-size:13px; background-color: #c1dffa; text-align: center;' >" :
                                    " style='font-size:13px; text-align: center;'>"));
                                sb.Append("<td style='border: 1px solid black;font-weight:500; text-align: center; width:5%;'>" + l + "</th>");
                                sb.Append("<td style='border: 1px solid black;font-weight:500; text-align: center; width:5%;'>" + Deinstallerdata.Cycle + "</td>");
                                sb.Append("<td style='border: 1px solid black;font-weight:500; text-align: center; width:5%;'>" + Deinstallerdata.Route + "</td>");
                                //sb.Append("<td style='border: 1px solid black;font-weight:500; text-align: center; width:10%;'>" + Deinstallerdata.Account + "</td>");
                                sb.Append("<td style='border: 1px solid black;font-weight:500; text-align: center; width:25%;'>" + (!string.IsNullOrEmpty(Deinstallerdata.Customer) ? Deinstallerdata.Customer : "-") + "</td>");
                                sb.Append("<td style='border: 1px solid black;font-weight:500; text-align: center; width:30%;'>" + Deinstallerdata.Street + "</td>");
                                sb.Append("<td style='border: 1px solid black;font-weight:500; text-align: center; width:10%;'>" + (!string.IsNullOrEmpty(Deinstallerdata.OldMeterSize) ? Deinstallerdata.OldMeterSize : "-") + "</td>");
                                sb.Append("<td style='border: 1px solid black;font-weight:500; text-align: center; width:10%;'>" + (!string.IsNullOrEmpty(Deinstallerdata.OldMeterNo) ? Deinstallerdata.OldMeterNo : "-") + "</td>");
                                sb.Append("<td style='border: 1px solid black;font-weight:500; text-align: center; width:10%;'>" + (!string.IsNullOrEmpty(Deinstallerdata.OldMeterRadioNo) ? Deinstallerdata.OldMeterRadioNo : "-") + "</td>");
                                sb.Append("</tr>");


                            }
                            k = k - 1;
                            l++;
                        }
                        sb.Append("</tbody>");
                    }

                    sb.Append("</table><br/>");
                }
                sb.Append("Thank You,<br/>Data Depot Team.<br>" +
               "<p style='text-align:center;'> <a href='https://www.datadepotonline.com/' title='' rel='home'>" +
               "<img src='https://www.datadepotonline.com/images/logo_03_2.png' title='Data Depot' alt='' style='height:100px; width:200px;'>" +
               "</a> </p>" +
               "<p style='text-align:center; font-size:13px;'>You are receiving this notification because you are registered with Data Depot <br/>" +
              " ©" + DateTime.Now.ToString("yyyy") + " Data Depot</p> <br>");
                sb.Append("<p style='text-align:center; color:red; font-size:15px;'>*** This Email is system generated. Please do not reply to this email ID." +
                    "For any queries please contact Data Depot Team. ***</p>");

                sb.Append("<p style='text-align:center; color:red; font-size:13px;'>This message (including any attachments) contains confidential information" +
                    " intended for a specific individual and purpose and is protected by law. " +
                "If you are not the intended recipient, you should delete this message." +
                " Any disclosure, copying or distribution of this message, or the taking of any action based on it, is strictly prohibited.</p>");


                sb.Append("</body>");
                //  return sb.ToString();


                //string MailFormat = new CommonFunctions().GetFileContents((HostingEnvironment.MapPath("~/EmailTemplate/RegisterMailTemplate.txt")));
                // string MailBody = String.Format(null, Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["WebURL"]), sb.ToString());
                string MailBody = sb.ToString();

                return MailBody;
            }
        }

        public List<String> CCEmailTo()
        {
            List<String> List = new List<String>();
            CompassEntities db = new CompassEntities();
            var CCEmailId = (from b in db.TblAlertMasters
                             join c in db.TblEmailConfigures on b.AlertId equals c.AlertTableID
                             join d in db.TblEmailConfigureUserDetails on c.ID equals d.TblEmailConfigureID
                             where b.AlertId == 1
                             //select new TblEmailConfigureUserDetail
                             //{
                             //    UserID = d.UserID
                             //}
                             select d.UserID
                             )
                             .ToList();
            if (CCEmailId.Count > 0)
            {
                foreach (var item in CCEmailId)
                {
                    var UserEmailId = db.tblUsers.Where(n => n.UserID == item).FirstOrDefault();
                    if (UserEmailId != null)
                    {
                        List.Add(UserEmailId.Email);
                    }
                }
            }
            return List;
        }

        #endregion


        //Assign Addresses from one installer to another installer

        #region -- Added by Aniket on 24-Aug--


        /// <summary>
        /// Get AddressList to Assign Installer
        /// </summary>
        /// <param name="ProjectID"></param>
        /// <param name="InstallerID"></param>
        /// <returns></returns>
        public List<STP_GetAddress_AssignToInstaller_Result> GetAddressListToAssignInstaller(long ProjectID, long InstallerID)
        {
            try
            {
                List<STP_GetAddress_AssignToInstaller_Result> addressList = new List<STP_GetAddress_AssignToInstaller_Result>();
                CompassEntities dbcontext = new CompassEntities();
                addressList = dbcontext.STP_GetAddress_AssignToInstaller(ProjectID, InstallerID).ToList();

                return addressList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Assign Addresses from one installer to another installer
        /// </summary>
        /// <param name="InstallerMap"></param>
        /// <param name="successMessage"></param>
        /// <returns></returns>
        public bool AssignAddressToInstaller(AddressInstallerMap InstallerMap, out string successMessage)
        {
            CompassEntities dbcontext = new CompassEntities();
            bool success = false;
            successMessage = "Street allocation failed";
            DateTime dt = new CommonFunctions().ServerDate();
            try
            {

                using (TransactionScope ts = new TransactionScope())
                {


                    List<tblInstallerMapData> InstallerMapDataList = (from inst in dbcontext.tblInstallerMapDatas
                                                                      join uplo in dbcontext.tblUploadedDatas on inst.tblUploadedData_ID equals uplo.ID
                                                                      where uplo.ProjectId == InstallerMap.ProjectID && inst.Active == 1 && uplo.Active == 1 &&
                                                                      inst.InstallerId == InstallerMap.oldInstallerID &&
                                                                      InstallerMap.AddressList.Contains(inst.tblUploadedData_ID)
                                                                      select inst).ToList();



                    if (InstallerMapDataList != null && InstallerMapDataList.Count > 0)
                    {

                        List<TblProjectFieldData> ProjectFieldDataList = (from pfd in dbcontext.TblProjectFieldDatas
                                                                          where InstallerMap.AddressList.Contains(pfd.tblUploadedData_Id)
                                                                          && pfd.InstallerId == InstallerMap.oldInstallerID && pfd.Active == 1 && pfd.IsSkipped == true
                                                                          select pfd).ToList();


                        foreach (var InstallerMapData in InstallerMapDataList)
                        {
                            if (ProjectFieldDataList != null && ProjectFieldDataList.Count > 0)
                            {
                                TblProjectFieldData TblProjectFieldData = ProjectFieldDataList.Where(a => a.tblUploadedData_Id == InstallerMapData.tblUploadedData_ID
                                                                          && a.InstallerId == InstallerMapData.InstallerId).FirstOrDefault();

                                if (TblProjectFieldData != null)
                                {

                                    TblProjectFieldData.Active = 2;

                                    #region --TblProjectFieldDataHistory--
                                    TblProjectFieldDataHistory TblProjectFieldDataHistory = new TblProjectFieldDataHistory();

                                    //TblProjectFieldDataHistory.ID = TblProjectFieldData.ID;
                                    TblProjectFieldDataHistory.FormId = TblProjectFieldData.FormId;
                                    TblProjectFieldDataHistory.tblUploadedData_Id = TblProjectFieldData.tblUploadedData_Id;
                                    TblProjectFieldDataHistory.InstallerId = TblProjectFieldData.InstallerId;
                                    TblProjectFieldDataHistory.Active = 1;
                                    TblProjectFieldDataHistory.CreatedBy = 1;
                                    TblProjectFieldDataHistory.CreatedOn = dt;
                                    TblProjectFieldDataHistory.IsReAssign = TblProjectFieldData.IsReAssign;
                                    TblProjectFieldDataHistory.ModifiedBy = null;
                                    TblProjectFieldDataHistory.ModifiedOn = null;
                                    TblProjectFieldDataHistory.IsSkipped = TblProjectFieldData.IsSkipped;
                                    TblProjectFieldDataHistory.SkippedReason = TblProjectFieldData.SkippedReason;
                                    TblProjectFieldDataHistory.SkipId = TblProjectFieldData.SkipId;
                                    TblProjectFieldDataHistory.InstallerMapId = TblProjectFieldData.InstallerMapId;
                                    TblProjectFieldDataHistory.SkipComment = TblProjectFieldData.SkipComment;
                                    TblProjectFieldDataHistory.SkippedDatetime = TblProjectFieldData.SkippedDatetime;
                                    TblProjectFieldDataHistory.visitdatetime = TblProjectFieldData.visitdatetime;
                                    TblProjectFieldDataHistory.JsonDataVal = TblProjectFieldData.JsonDataVal;
                                    dbcontext.TblProjectFieldDataHistories.Add(TblProjectFieldDataHistory);
                                    // dbcontext.SaveChanges();
                                    #endregion



                                    #region --create newTblProjectFieldData--
                                    TblProjectFieldData newTblProjectFieldData = new TblProjectFieldData();

                                    newTblProjectFieldData.FormId = TblProjectFieldData.FormId;
                                    newTblProjectFieldData.tblUploadedData_Id = TblProjectFieldData.tblUploadedData_Id;
                                    newTblProjectFieldData.InstallerId = InstallerMap.newInstallerID;
                                    newTblProjectFieldData.Active = 1;
                                    newTblProjectFieldData.IsReAssign = true;
                                    newTblProjectFieldData.CreatedBy = 1;
                                    newTblProjectFieldData.CreatedOn = dt;
                                    newTblProjectFieldData.ModifiedBy = null;
                                    newTblProjectFieldData.ModifiedOn = null;
                                    newTblProjectFieldData.IsSkipped = TblProjectFieldData.IsSkipped;
                                    newTblProjectFieldData.SkippedReason = TblProjectFieldData.SkippedReason;
                                    newTblProjectFieldData.SkipId = TblProjectFieldData.SkipId;
                                    newTblProjectFieldData.InstallerMapId = TblProjectFieldData.InstallerMapId;
                                    newTblProjectFieldData.SkipComment = TblProjectFieldData.SkipComment;
                                    newTblProjectFieldData.SkippedDatetime = TblProjectFieldData.SkippedDatetime;
                                    newTblProjectFieldData.visitdatetime = TblProjectFieldData.visitdatetime;
                                    newTblProjectFieldData.JsonDataVal = TblProjectFieldData.JsonDataVal;
                                    dbcontext.TblProjectFieldDatas.Add(newTblProjectFieldData);
                                    // dbcontext.SaveChanges();
                                    #endregion



                                    dbcontext.SaveChanges();

                                }
                            }

                            InstallerMapData.Active = 2;
                            // dbcontext.SaveChanges();

                            #region --InstallerMapDataHistory--

                            tblInstallerMapDataHistory tblInstallerMapDataHistory = new tblInstallerMapDataHistory();
                            // tblInstallerMapDataHistory.ID = InstallerMapData.ID;
                            tblInstallerMapDataHistory.InstallerId = InstallerMapData.InstallerId;
                            tblInstallerMapDataHistory.tblUploadedData_ID = InstallerMapData.tblUploadedData_ID;
                            tblInstallerMapDataHistory.FromDate = InstallerMapData.FromDate;
                            tblInstallerMapDataHistory.ToDate = InstallerMapData.ToDate;
                            tblInstallerMapDataHistory.Date = InstallerMapData.Date;
                            tblInstallerMapDataHistory.SynchWithMobileFlag = InstallerMapData.SynchWithMobileFlag;
                            tblInstallerMapDataHistory.SynchWithMobileDate = InstallerMapData.SynchWithMobileDate;
                            tblInstallerMapDataHistory.MobileResponse = InstallerMapData.MobileResponse;
                            tblInstallerMapDataHistory.MobileResponseDate = InstallerMapData.MobileResponseDate;
                            tblInstallerMapDataHistory.SynchCheckSumString = InstallerMapData.SynchCheckSumString;
                            tblInstallerMapDataHistory.BatchNumber = InstallerMapData.BatchNumber;
                            tblInstallerMapDataHistory.Active = 1;
                            tblInstallerMapDataHistory.CreatedBy = 1;
                            tblInstallerMapDataHistory.CreatedOn = dt;
                            tblInstallerMapDataHistory.ModifiedBy = null;
                            tblInstallerMapDataHistory.ModifiedOn = null;
                            tblInstallerMapDataHistory.ProjectID = InstallerMapData.ProjectID;
                            tblInstallerMapDataHistory.SkipReasonMasterId = InstallerMapData.SkipReasonMasterId;
                            tblInstallerMapDataHistory.SkipComment = InstallerMapData.SkipComment;
                            dbcontext.tblInstallerMapDataHistories.Add(tblInstallerMapDataHistory);
                            //dbcontext.SaveChanges();
                            #endregion

                            #region --create newInstallerMapData --
                            tblInstallerMapData newInstallerMapData = new tblInstallerMapData();
                            newInstallerMapData.tblUploadedData_ID = InstallerMapData.tblUploadedData_ID;
                            newInstallerMapData.FromDate = InstallerMapData.FromDate;
                            newInstallerMapData.ToDate = InstallerMapData.ToDate;
                            newInstallerMapData.Date = InstallerMapData.Date;
                            newInstallerMapData.ProjectID = InstallerMapData.ProjectID;

                            newInstallerMapData.SkipReasonMasterId = null;
                            newInstallerMapData.SkipComment = null;
                            newInstallerMapData.InstallerId = InstallerMap.newInstallerID;
                            newInstallerMapData.SynchWithMobileFlag = null;
                            newInstallerMapData.SynchWithMobileDate = null;
                            newInstallerMapData.MobileResponse = null;
                            newInstallerMapData.MobileResponseDate = null;
                            newInstallerMapData.SynchCheckSumString = null;
                            newInstallerMapData.BatchNumber = null;
                            newInstallerMapData.Active = 1;
                            newInstallerMapData.CreatedBy = 1;
                            newInstallerMapData.CreatedOn = dt;
                            newInstallerMapData.ModifiedBy = null;
                            newInstallerMapData.ModifiedOn = null;

                            dbcontext.tblInstallerMapDatas.Add(newInstallerMapData);
                            //dbcontext.SaveChanges();
                            #endregion

                            dbcontext.SaveChanges();
                        }
                        //Added by AniketJ on 16-Sep-2016
                        #region --Send Mail to New Installer--
                        List<String> mailtoNewInstaller = new List<String>();
                        List<String> mailtoOldInstaller = new List<String>();
                        List<String> CCEmailTo = new List<String>();
                        string installer = "";
                        UserDetailsModel newInstallerDetails = GetInstallerDetails(InstallerMap.newInstallerID);
                        UserDetailsModel oldInstallerDetails = GetInstallerDetails(InstallerMap.oldInstallerID);
                        List<InstallerAddressDetails> addressDetails = GetInstallerAddressDetails(InstallerMap.AddressList);
                        var mailSubjectNote = GetMailSubject();

                        installer = "New";
                        string MessageNewInstaller = GetMailBody_ReassignInstaller(newInstallerDetails, addressDetails, mailSubjectNote.Message, installer);
                        string Subject = mailSubjectNote.Subject;
                        Task.Factory.StartNew(() =>
                        {
                            //Send mail to old installer
                            mailtoNewInstaller.Add(newInstallerDetails.EmailId);
                            CCEmailTo.AddRange(mailSubjectNote.UserModel.Select(a => a.Email).ToList());
                            new GarronT.CMS.Model.clsMail().SendMail(mailtoNewInstaller, CCEmailTo, MessageNewInstaller, Subject);
                        });

                        installer = "Old";
                        string MessageOldInstaller = GetMailBody_ReassignInstaller(oldInstallerDetails, addressDetails, mailSubjectNote.Message, installer);
                        Task.Factory.StartNew(() =>
                        {
                            //Send mail to new installer
                            mailtoOldInstaller.Add(oldInstallerDetails.EmailId);
                            //mailtoOldInstaller.Add("aniket.jadhav@kairee.in");
                            new GarronT.CMS.Model.clsMail().SendMail(mailtoOldInstaller, null, MessageOldInstaller, Subject);
                        });

                        #endregion

                    }
                    ts.Complete();
                    success = true;
                    successMessage = "Street Allocated Successfully";
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                successMessage = ex.Message;
            }
            return success;
        }



        /// <summary>
        /// Assign Addresses from one installer to another installer
        /// </summary>
        /// <param name="InstallerMap"></param>
        /// <param name="successMessage"></param>
        /// <returns></returns>
        public bool AssignDynamicAddressToInstaller(AddressInstallerMap InstallerMap, out string successMessage)
        {
            CompassEntities dbcontext = new CompassEntities();
            bool success = false;
            successMessage = "Street allocation failed";
            DateTime dt = new CommonFunctions().ServerDate();
            try
            {
                List<tblInstallerMapData> InstallerMapDataList = new List<tblInstallerMapData>();
                using (TransactionScope ts = new TransactionScope())
                {

                    foreach (var item in InstallerMap.AddressList)
                    {
                        var data = dbcontext.tblInstallerMapDatas.Where(o => o.FK_UploadedExcelData_Id == item && o.Active == 1 && o.InstallerId == InstallerMap.oldInstallerID).FirstOrDefault();
                        if (data != null)
                        {
                            InstallerMapDataList.Add(data);
                        }
                    }


                    //List<tblInstallerMapData> InstallerMapDataList = (from inst in dbcontext.tblInstallerMapDatas
                    //                                                  join uplo in dbcontext.UploadedExcelDatas on inst.FK_UploadedExcelData_Id equals uplo.Id
                    //                                                  where uplo.FKProjectId == InstallerMap.ProjectID && inst.Active == 1 && uplo.Active == 1 &&
                    //                                                  inst.InstallerId == InstallerMap.oldInstallerID &&
                    //                                                  InstallerMap.AddressList.Contains(long.Parse(inst.FK_UploadedExcelData_Id.ToString()))
                    //                                                  select inst).ToList();



                    if (InstallerMapDataList != null && InstallerMapDataList.Count > 0)
                    {

                        //List<TblProjectFieldData> ProjectFieldDataList = (from pfd in dbcontext.TblProjectFieldDatas
                        //                                                  where InstallerMap.AddressList.Contains(long.Parse(pfd.FK_UploadedExcelData_Id.ToString()))
                        //                                                  && pfd.InstallerId == InstallerMap.oldInstallerID && pfd.Active == 1 && pfd.IsSkipped == true
                        //                                                  select pfd).ToList();
                        List<TblProjectFieldData> ProjectFieldDataList = new List<TblProjectFieldData>();

                        foreach (var item in InstallerMap.AddressList)
                        {
                            var data = dbcontext.TblProjectFieldDatas.Where(o => o.FK_UploadedExcelData_Id == item &&
                                o.Active == 1 && o.InstallerId == InstallerMap.oldInstallerID && o.IsSkipped == true).FirstOrDefault();
                            if (data != null)
                            {
                                ProjectFieldDataList.Add(data);
                            }
                        }


                        foreach (var InstallerMapData in InstallerMapDataList)
                        {
                            if (ProjectFieldDataList != null && ProjectFieldDataList.Count > 0)
                            {
                                TblProjectFieldData TblProjectFieldData = ProjectFieldDataList.Where(a => a.FK_UploadedExcelData_Id == InstallerMapData.FK_UploadedExcelData_Id
                                                                          && a.InstallerId == InstallerMapData.InstallerId).FirstOrDefault();

                                if (TblProjectFieldData != null)
                                {

                                    TblProjectFieldData.Active = 2;

                                    #region --TblProjectFieldDataHistory--
                                    TblProjectFieldDataHistory TblProjectFieldDataHistory = new TblProjectFieldDataHistory();

                                    //TblProjectFieldDataHistory.ID = TblProjectFieldData.ID;
                                    TblProjectFieldDataHistory.FormId = TblProjectFieldData.FormId;
                                    // TblProjectFieldDataHistory.tblUploadedData_Id = TblProjectFieldData.tblUploadedData_Id;
                                    TblProjectFieldDataHistory.FK_UploadedExcelData_Id = TblProjectFieldData.FK_UploadedExcelData_Id;
                                    TblProjectFieldDataHistory.InstallerId = TblProjectFieldData.InstallerId;
                                    TblProjectFieldDataHistory.Active = 1;
                                    TblProjectFieldDataHistory.CreatedBy = 1;
                                    TblProjectFieldDataHistory.CreatedOn = dt;
                                    TblProjectFieldDataHistory.IsReAssign = TblProjectFieldData.IsReAssign;
                                    TblProjectFieldDataHistory.ModifiedBy = null;
                                    TblProjectFieldDataHistory.ModifiedOn = null;
                                    TblProjectFieldDataHistory.IsSkipped = TblProjectFieldData.IsSkipped;
                                    TblProjectFieldDataHistory.SkippedReason = TblProjectFieldData.SkippedReason;
                                    TblProjectFieldDataHistory.SkipId = TblProjectFieldData.SkipId;
                                    TblProjectFieldDataHistory.InstallerMapId = TblProjectFieldData.InstallerMapId;
                                    TblProjectFieldDataHistory.SkipComment = TblProjectFieldData.SkipComment;
                                    TblProjectFieldDataHistory.SkippedDatetime = TblProjectFieldData.SkippedDatetime;
                                    TblProjectFieldDataHistory.visitdatetime = TblProjectFieldData.visitdatetime;
                                    TblProjectFieldDataHistory.JsonDataVal = TblProjectFieldData.JsonDataVal;
                                    dbcontext.TblProjectFieldDataHistories.Add(TblProjectFieldDataHistory);
                                    // dbcontext.SaveChanges();
                                    #endregion



                                    #region --create newTblProjectFieldData--
                                    TblProjectFieldData newTblProjectFieldData = new TblProjectFieldData();

                                    newTblProjectFieldData.FormId = TblProjectFieldData.FormId;
                                    // newTblProjectFieldData.tblUploadedData_Id = TblProjectFieldData.tblUploadedData_Id;
                                    newTblProjectFieldData.FK_UploadedExcelData_Id = TblProjectFieldData.FK_UploadedExcelData_Id;
                                    newTblProjectFieldData.InstallerId = InstallerMap.newInstallerID;
                                    newTblProjectFieldData.Active = 1;
                                    newTblProjectFieldData.IsReAssign = true;
                                    newTblProjectFieldData.CreatedBy = 1;
                                    newTblProjectFieldData.CreatedOn = dt;
                                    newTblProjectFieldData.ModifiedBy = null;
                                    newTblProjectFieldData.ModifiedOn = null;
                                    newTblProjectFieldData.IsSkipped = TblProjectFieldData.IsSkipped;
                                    newTblProjectFieldData.SkippedReason = TblProjectFieldData.SkippedReason;
                                    newTblProjectFieldData.SkipId = TblProjectFieldData.SkipId;
                                    newTblProjectFieldData.InstallerMapId = TblProjectFieldData.InstallerMapId;
                                    newTblProjectFieldData.SkipComment = TblProjectFieldData.SkipComment;
                                    newTblProjectFieldData.SkippedDatetime = TblProjectFieldData.SkippedDatetime;
                                    newTblProjectFieldData.visitdatetime = TblProjectFieldData.visitdatetime;
                                    newTblProjectFieldData.JsonDataVal = TblProjectFieldData.JsonDataVal;
                                    dbcontext.TblProjectFieldDatas.Add(newTblProjectFieldData);
                                    // dbcontext.SaveChanges();
                                    #endregion



                                    dbcontext.SaveChanges();
                                    InstallerMapData.Active = 2;
                                }
                                else
                                {
                                    InstallerMapData.Active = 0;
                                }
                            }
                            else
                            {
                                InstallerMapData.Active = 0;
                            }


                            // dbcontext.SaveChanges();

                            #region --InstallerMapDataHistory--

                            tblInstallerMapDataHistory tblInstallerMapDataHistory = new tblInstallerMapDataHistory();
                            // tblInstallerMapDataHistory.ID = InstallerMapData.ID;
                            tblInstallerMapDataHistory.InstallerId = InstallerMapData.InstallerId;
                            // tblInstallerMapDataHistory.tblUploadedData_ID = InstallerMapData.tblUploadedData_ID;
                            tblInstallerMapDataHistory.FK_UploadedExcelData_Id = InstallerMapData.FK_UploadedExcelData_Id;
                            tblInstallerMapDataHistory.FromDate = InstallerMapData.FromDate;
                            tblInstallerMapDataHistory.ToDate = InstallerMapData.ToDate;
                            tblInstallerMapDataHistory.Date = InstallerMapData.Date;
                            tblInstallerMapDataHistory.SynchWithMobileFlag = InstallerMapData.SynchWithMobileFlag;
                            tblInstallerMapDataHistory.SynchWithMobileDate = InstallerMapData.SynchWithMobileDate;
                            tblInstallerMapDataHistory.MobileResponse = InstallerMapData.MobileResponse;
                            tblInstallerMapDataHistory.MobileResponseDate = InstallerMapData.MobileResponseDate;
                            tblInstallerMapDataHistory.SynchCheckSumString = InstallerMapData.SynchCheckSumString;
                            tblInstallerMapDataHistory.BatchNumber = InstallerMapData.BatchNumber;
                            tblInstallerMapDataHistory.Active = 1;
                            tblInstallerMapDataHistory.CreatedBy = 1;
                            tblInstallerMapDataHistory.CreatedOn = dt;
                            tblInstallerMapDataHistory.ModifiedBy = null;
                            tblInstallerMapDataHistory.ModifiedOn = null;
                            tblInstallerMapDataHistory.ProjectID = InstallerMapData.ProjectID;
                            tblInstallerMapDataHistory.SkipReasonMasterId = InstallerMapData.SkipReasonMasterId;
                            tblInstallerMapDataHistory.SkipComment = InstallerMapData.SkipComment;
                            dbcontext.tblInstallerMapDataHistories.Add(tblInstallerMapDataHistory);
                            //dbcontext.SaveChanges();
                            #endregion

                            #region --create newInstallerMapData --
                            tblInstallerMapData newInstallerMapData = new tblInstallerMapData();
                            // newInstallerMapData.tblUploadedData_ID = InstallerMapData.tblUploadedData_ID;

                            newInstallerMapData.FK_UploadedExcelData_Id = InstallerMapData.FK_UploadedExcelData_Id;
                            newInstallerMapData.FromDate = InstallerMapData.FromDate;
                            newInstallerMapData.ToDate = InstallerMapData.ToDate;
                            newInstallerMapData.Date = InstallerMapData.Date;
                            newInstallerMapData.ProjectID = InstallerMapData.ProjectID;

                            newInstallerMapData.SkipReasonMasterId = null;
                            newInstallerMapData.SkipComment = null;
                            newInstallerMapData.InstallerId = InstallerMap.newInstallerID;
                            newInstallerMapData.SynchWithMobileFlag = null;
                            newInstallerMapData.SynchWithMobileDate = null;
                            newInstallerMapData.MobileResponse = null;
                            newInstallerMapData.MobileResponseDate = null;
                            newInstallerMapData.SynchCheckSumString = null;
                            newInstallerMapData.BatchNumber = null;
                            newInstallerMapData.Active = 1;
                            newInstallerMapData.CreatedBy = 1;
                            newInstallerMapData.CreatedOn = dt;
                            newInstallerMapData.ModifiedBy = null;
                            newInstallerMapData.ModifiedOn = null;

                            dbcontext.tblInstallerMapDatas.Add(newInstallerMapData);
                            //dbcontext.SaveChanges();
                            #endregion

                            dbcontext.SaveChanges();
                        }
                        //Added by AniketJ on 16-Sep-2016
                        #region --Send Mail to New Installer--
                        List<String> mailtoNewInstaller = new List<String>();
                        List<String> mailtoOldInstaller = new List<String>();
                        List<String> CCEmailTo = new List<String>();
                        string installer = "";
                        UserDetailsModel newInstallerDetails = GetInstallerDetails(InstallerMap.newInstallerID);
                        UserDetailsModel oldInstallerDetails = GetInstallerDetails(InstallerMap.oldInstallerID);
                        List<InstallerAddressDetails> addressDetails = GetDynamicInstallerAddressDetails(InstallerMap.AddressList, InstallerMap.ProjectID);
                        var mailSubjectNote = GetMailSubject();

                        installer = "New";
                        string MessageNewInstaller = GetMailBody_ReassignInstaller(newInstallerDetails, addressDetails, mailSubjectNote.Message, installer);
                        string Subject = mailSubjectNote.Subject;
                        Task.Factory.StartNew(() =>
                        {
                            //Send mail to old installer
                            mailtoNewInstaller.Add(newInstallerDetails.EmailId);
                            CCEmailTo.AddRange(mailSubjectNote.UserModel.Select(a => a.Email).ToList());
                            new GarronT.CMS.Model.clsMail().SendMail(mailtoNewInstaller, CCEmailTo, MessageNewInstaller, Subject);
                        });

                        installer = "Old";
                        string MessageOldInstaller = GetMailBody_ReassignInstaller(oldInstallerDetails, addressDetails, mailSubjectNote.Message, installer);
                        Task.Factory.StartNew(() =>
                        {
                            //Send mail to new installer
                            mailtoOldInstaller.Add(oldInstallerDetails.EmailId);
                            //mailtoOldInstaller.Add("aniket.jadhav@kairee.in");
                            new GarronT.CMS.Model.clsMail().SendMail(mailtoOldInstaller, null, MessageOldInstaller, Subject);
                        });

                        #endregion

                    }
                    ts.Complete();
                    success = true;
                    successMessage = "Street Allocated Successfully";
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                successMessage = ex.Message;
            }
            return success;
        }


        #endregion


        /// <summary>
        /// Created by Aniket on 16-Sep-2016
        /// To get Installer Details
        /// </summary>
        /// <param name="installerID"></param>
        /// <returns>InstallerMailID</returns>
        public UserDetailsModel GetInstallerDetails(long installerID)
        {
            UserDetailsModel installerDeatils = new UserDetailsModel();

            using (CompassEntities dbcontext = new CompassEntities())
            {
                try
                {
                    installerDeatils = dbcontext.tblUsers.Where(a => a.UserID == installerID).Select(a => new UserDetailsModel
                    {
                        EmailId = a.Email,
                        FirstName = a.FirstName
                    }).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message, ex);
                }
            }

            return installerDeatils;
        }

        /// <summary>
        /// Created by Aniket on 16-Sep-2016
        /// To get Installer Details List
        /// </summary>
        /// <param name="uploadIDList"></param>
        /// <returns>InstallerAddressDetailsList</returns>
        public List<InstallerAddressDetails> GetInstallerAddressDetails(List<long> uploadIDList)
        {
            List<InstallerAddressDetails> InstallerAddressDetails = new List<InstallerAddressDetails>();
            using (CompassEntities dbcontext = new CompassEntities())
            {
                try
                {
                    InstallerAddressDetails = dbcontext.PROC_Get_InstallerAddressDetails().Where(a => uploadIDList.Contains(a.id)).Select(a => new InstallerAddressDetails
                    {
                        id = a.id,
                        ProjectName = a.ProjectName,
                        Utilitytype = a.Utilitytype,
                        CityName = a.CityName,
                        StateName = a.StateName,
                        FromDate = a.FromDate,
                        ToDate = a.ToDate,
                        firstname = a.firstname,
                        Email = a.Email,
                        cycle = a.cycle,
                        Route = a.Route,
                        Account = a.Account,
                        Customer = a.Customer,
                        OldMeterNumber = a.OldMeterNo,
                        OldMeterRadioNumber = a.OldMeterRadioNo,
                        Street = a.street
                    }).ToList();
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                }
            }

            return InstallerAddressDetails;
        }




        /// <summary>
        /// Created by Aniket on 16-Sep-2016
        /// To get Installer Details List
        /// </summary>
        /// <param name="uploadIDList"></param>
        /// <returns>InstallerAddressDetailsList</returns>
        public List<InstallerAddressDetails> GetDynamicInstallerAddressDetails(List<long> uploadIDList, long projectId)
        {
            List<InstallerAddressDetails> InstallerAddressDetails = new List<InstallerAddressDetails>();
            using (CompassEntities dbcontext = new CompassEntities())
            {
                try
                {


                    SqlConnection sqlConnection1 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;

                    cmd.CommandText = "PROC_InstallerAllocation_GetAddressDetails";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = sqlConnection1;
                    cmd.Parameters.Add("@projectId", projectId);

                    sqlConnection1.Open();

                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        InstallerAddressDetails.Add(new InstallerAddressDetails
                        {
                            id = long.Parse(reader["id"].ToString()),
                            ProjectName = reader["ProjectName"].ToString(),
                            Utilitytype = reader["Utilitytype"].ToString(),
                            CityName = reader["CityName"].ToString(),
                            StateName = reader["StateName"].ToString(),
                            FromDate = DateTime.Parse(reader["FromDate"].ToString()),
                            ToDate = DateTime.Parse(reader["ToDate"].ToString()),
                            firstname = reader["firstname"].ToString(),
                            Email = reader["Email"].ToString(),
                            cycle = reader["cycle"].ToString(),
                            Route = reader["Route"].ToString(),
                            Account = reader["Account"].ToString(),
                            Customer = reader["Customer"].ToString(),
                            OldMeterNumber = reader["OldMeterNo"].ToString(),
                            OldMeterRadioNumber = reader["OldMeterRadioNo"].ToString(),
                            Street = reader["street"].ToString(),
                            OldMeterSize = reader["OldMeterSize"].ToString()
                        });

                    }

                    sqlConnection1.Close();
                    reader.Close();
                    reader.Dispose();
                    cmd.Dispose();
                    sqlConnection1.Dispose();


                    InstallerAddressDetails = InstallerAddressDetails.Where(a => uploadIDList.Contains(a.id)).Select(a => a).ToList();
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                }
            }

            return InstallerAddressDetails;
        }





        /// <summary>
        /// Created by Aniket on 16-Sep-2016
        /// To get Mail body
        /// </summary>
        /// <param name="InstallerDetails"></param>
        /// <param name="addressDetails"></param>
        /// <param name="noteMessage"></param>
        /// <param name="installer"></param>
        /// <returns>Mail Body</returns>
        public string GetMailBody_ReassignInstaller(UserDetailsModel InstallerDetails, List<InstallerAddressDetails> addressDetails, string noteMessage, string installer)
        {

            StringBuilder mailbody = new StringBuilder();
            mailbody.Append("<body  style='padding: 10px 0px 0px 10px; background: #f8f8fb; font-family:Times New Roman; font-size:14px;'>");
            mailbody.Append("<p style='padding: 10px 0px 0px 10px'>Dear " + InstallerDetails.FirstName.ToUpper() + ",</p>");
            mailbody.Append("<br/> <p style='font-size:15px;'>Greetings from Data Depot....!!!</p>");

            mailbody.Append("<br/> <p >This is to inform you that" +
                (installer == "New" ? "new visits are allocated to you" : " visits which are allocated to you are removed from you") +
                "by  CMS Management. Please check below details.</p><br/>");


            mailbody.Append("<br/> <div style='background: #e6e1e1; width:100%;'>" + noteMessage + "</div><br/>");
            mailbody.Append("<div style='background: #e6e1e1; width:100%;'><table style='border:none;padding: 0px 0px 0px 10px;'>" +
            "<tr>" +
                "<td style='border:none;'><b>Project Name </b></td>" +
                "<td style='border:none;'><b>:</b></td>" +
                "<td style='border:none;' colspan='2'>" + addressDetails.Select(a => a.ProjectName).FirstOrDefault().ToUpper() + "</td>" +
            "</tr>" +

            "<tr>" +
                "<td style='border:none;'><b>Utility Type</b></td>" +
                "<td style='border:none;'><b>:</b></td>" +
                "<td style='border:none;' colspan='4'>" + addressDetails.Select(a => a.Utilitytype).FirstOrDefault().ToUpper() + "</td>" +
            "</tr>" +

            "<tr>" +
                "<td style='border:none;'><b>City:</b></td>" +
                "<td style='border:none;'><b>:</b></td>" +
               " <td style='border:none;'>" + addressDetails.Select(a => a.CityName).FirstOrDefault().ToUpper() + " &nbsp;&nbsp;</td>" +

               " <td style='border:none;'><b>State</b></td>" +
               " <td style='border:none;'><b>:</b></td>" +
                "<td style='border:none;'>" + addressDetails.Select(a => a.StateName).FirstOrDefault().ToUpper() + "</td>" +
           " </tr>" +
            "<tr>" +
               " <td style='border:none;'><b>Visit Start Date</b></td>" +
               " <td style='border:none;'><b>:</b></td>" +
                "<td style='border:none;'>" + addressDetails.Select(a => a.FromDate).FirstOrDefault().ToString("MMM-dd-yyyy") + "   &nbsp;&nbsp;</td>" +


                 "<td style='border:none;'><b>Visit End Date</b></td>" +
               " <td style='border:none;'><b>:</b></td>" +
                "<td style='border:none;'>" + addressDetails.Select(a => a.ToDate).FirstOrDefault().ToString("MMM-dd-yyyy") + "   </td>" +
            "</tr>" +
             "<tr>" +
                "<td style='border:none;'><b>Total Records </b></td>" +
                "<td style='border:none;'><b>:</b></td>" +
                "<td style='border:none;' colspan='2'>" + addressDetails.Count + "</td>" +
            "</tr>" +
        "</table></div><br />");


            mailbody.Append("<table style='border:none;padding: 0px 0px 0px 10px;'>");
            mailbody.Append("<thead>" +
                "<tr style='font-size:13px;background-color:#80B2DF;'> " +
                    "<th style='border: 1px solid black;padding: 0px 8px;'>Sr.No</th> " +
                    "<th style='border: 1px solid black;padding: 0px 8px;'>Cycle</th> " +
                    "<th style='border: 1px solid black;padding: 0px 10px;'>Route</th> " +
                    //"<th style='border: 1px solid black;padding: 0px 10px;'>Account Number</th> " +
                    "<th style='border: 1px solid black;padding: 0px 20px;'>Customer Name</th> " +
                    "<th style='border: 1px solid black;padding: 0px 34px;'>Street</th> " +
                    "<th style='border: 1px solid black;padding: 0px 10px;'>Old Meter Size</th> " +
                    "<th style='border: 1px solid black;padding: 0px 10px;'>Old Meter Number</th> " +
                    "<th style='border: 1px solid black;padding: 0px 10px;'>Old Meter Radio Number</th> " +
                "</tr>" +
            "</thead><tbody style='font-size:13px;'>");

            int i = 0;
            int j = 1;

            i = addressDetails.Count;

            foreach (var item in addressDetails)
            {
                mailbody.Append("<tr " + ((i % 2 == 0) ? " style='font-size:13px; background-color: #c1dffa; text-align: center;' >" :
                    " style='font-size:13px; text-align: center;'>"));
                mailbody.Append("<td style='border: 1px solid black;font-weight:500'>" + j + "</td>");
                mailbody.Append("<td style='border: 1px solid black;font-weight:500'>" + item.cycle + "</td>");
                mailbody.Append("<td style='border: 1px solid black;font-weight:500'>" + item.Route + "</td>");
                //mailbody.Append("<td style='border: 1px solid black;font-weight:500'>" + item.Account + "</td>");
                mailbody.Append("<td style='border: 1px solid black;font-weight:500'>" + (item.Customer != null ? item.Customer : " - ") + "</td>");
                mailbody.Append("<td style='border: 1px solid black;font-weight:500'>" + item.Street + "</td>");
                mailbody.Append("<td style='border: 1px solid black;font-weight:500'>" + item.OldMeterSize + "</td>");
                mailbody.Append("<td style='border: 1px solid black;font-weight:500'>" + item.OldMeterNumber + "</td>");
                mailbody.Append("<td style='border: 1px solid black;font-weight:500'>" + item.OldMeterRadioNumber + "</td>");
                mailbody.Append("</tr>");

                i = i - 1;
                j++;
            }
            mailbody.Append("</tbody></table><br/>");
            mailbody.Append("Thank You,<br/>Data Depot Team.<br>" +
               "<p style='text-align:center;'> <a href='https://www.datadepotonline.com/' title='' rel='home'>" +
               "<img src='https://www.datadepotonline.com/images/logo_03_2.png' title='Data Depot' alt='' style='height:100px; width:200px;'>" +
               "</a> </p>" +
               "<p style='text-align:center; font-size:13px;'>You are receiving this notification because you are registered with Data Depot<br/>" +
              " ©" + DateTime.Now.ToString("yyyy") + " Data Depot</p> <br>");
            mailbody.Append("<p style='text-align:center; color:red; font-size:15px;'>*** This Email is system generated. Please do not reply to this email ID." +
                "For any queries please contact Data Depot Team. ***</p>");



            mailbody.Append("<p style='text-align:center; color:red; font-size:13px;'>This message (including any attachments) contains confidential information" +
                " intended for a specific individual and purpose and is protected by law." +
            " If you are not the intended recipient, you should delete this message." +
            " Any disclosure, copying or distribution of this message, or the taking of any action based on it, is strictly prohibited.</p>");


            mailbody.Append("</body>");
            string stringMailBody = mailbody.ToString();
            return stringMailBody;
        }

        /// <summary>
        /// Created by Aniket on 17-Sep-2016
        /// To get Mail Subject
        /// </summary>
        /// <returns></returns>
        public EmailConfigureBO GetMailSubject()
        {
            EmailConfigureBO EmailConfigureBO = new EmailConfigureBO();
            using (CompassEntities dbcontext = new CompassEntities())
            {
                try
                {
                    EmailConfigureBO = (from b in dbcontext.TblAlertMasters
                                        join c in dbcontext.TblEmailConfigures on b.AlertId equals c.AlertTableID
                                        join d in dbcontext.TblEmailConfigureUserDetails on c.ID equals d.TblEmailConfigureID

                                        where b.AlertId == 5 //Installer ReAssign
                                        select new EmailConfigureBO
                                        {
                                            Subject = c.Subject,
                                            Message = c.Message,
                                            UserModel = (from u in dbcontext.tblUsers where u.UserID == d.UserID select new UsersModel { Email = u.Email }).ToList()
                                        }).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message, ex);
                }
            }
            return EmailConfigureBO;
        }

        /// <summary>
        /// Created by Aniket on 23-Sep-2016
        /// To Change date of existing records
        /// </summary>
        /// <returns></returns>
        public bool changeDateofExistingRecords(InstallerMap InstallerMap)
        {
            bool success = false;
            using (CompassEntities dbcontext = new CompassEntities())
            {
                long formId = dbcontext.tblFormMasters.AsNoTracking().Where(o => o.Active == 1 && o.FormTypeID == 2
                      && o.ProjectId == InstallerMap.ProjectID).Select(o => o.FormId).FirstOrDefault();


                // var quickResult = dbcontext.Proc_GetInstallerFormStatusData(formId, InstallerMap.InstallerID, Convert.ToDateTime(InstallerMap.OldFromDate), Convert.ToDateTime(InstallerMap.OldToDate)).ToList();

                List<Proc_GetInstallerFormStatusData_Result> objList = new List<Proc_GetInstallerFormStatusData_Result>();
                SqlConnection sqlConnection1 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;

                cmd.CommandText = "PROC_InstallerAllocation_GetInstallerData";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = sqlConnection1;
                cmd.Parameters.Add("@formId", formId);
                cmd.Parameters.Add("@InstallerId", InstallerMap.InstallerID);
                cmd.Parameters.Add("@FromDate", DateTime.Parse(InstallerMap.FromDate));
                cmd.Parameters.Add("@ToDate", DateTime.Parse(InstallerMap.ToDate));
                cmd.CommandTimeout = 600;
                sqlConnection1.Open();

                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    objList.Add(new Proc_GetInstallerFormStatusData_Result
                    {
                        ID = long.Parse(reader["ID"].ToString()),
                        Street = reader["Street"].ToString(),
                        Latitude = reader["Latitude"].ToString(),
                        Longitude = reader["Longitude"].ToString(),
                        Cycle = reader["Cycle"].ToString(),
                        Route = reader["Route"].ToString(),
                        VisitStatus = reader["VisitStatus"].ToString()
                    });

                }

                sqlConnection1.Close();
                reader.Close();
                reader.Dispose();
                cmd.Dispose();
                sqlConnection1.Dispose();

                List<Proc_GetInstallerFormStatusData_Result> addressList = new List<Proc_GetInstallerFormStatusData_Result>();

                addressList.AddRange(objList.Where(a => a.VisitStatus.ToLower() == "pending" || a.VisitStatus.ToLower() == "skip"));
                foreach (var item in addressList)
                {
                    var installerdata = dbcontext.tblInstallerMapDatas.Where(a => a.FK_UploadedExcelData_Id == item.ID
                          && a.InstallerId == InstallerMap.InstallerID).FirstOrDefault();
                    if (installerdata != null)
                    {
                        installerdata.FromDate = Convert.ToDateTime(InstallerMap.FromDate);
                        installerdata.ToDate = Convert.ToDateTime(InstallerMap.ToDate);
                        dbcontext.SaveChanges();
                    }
                }
            }
            return success;
        }


        // Get Dynamic Col name
        public string getDynamicColumnName(string temColName, string ProjectID)
        {
            try
            {
                string dynamicColumnName = "";
                CompassEntities dbcontext = new CompassEntities();
                SqlConnection _sqlCon = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
                _sqlCon.Open();
                SqlDataAdapter sda = new SqlDataAdapter("Select DynamicColumnName=[dbo].[FN_GetColumnName]('"+ temColName + "','" + ProjectID + "')", _sqlCon);
                DataTable _dt = new DataTable();
                sda.Fill(_dt);
                if (_dt.Rows.Count > 0)
                {
                    dynamicColumnName = _dt.Rows[0]["DynamicColumnName"].ToString();
                }
                _sqlCon.Close();
                return dynamicColumnName;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //end ahre

        public List<STP_GetExistedUserInstallationbyDate_Result> CheckAddressExistby_ProjectInstallerDate(long ProjectID, long InstallerID, DateTime fromDate, DateTime toDate)
        {

            using (CompassEntities dbcontext = new CompassEntities())
            {
                var ExcludeSkip = dbcontext.tblUsers.Where(n => n.UserID == InstallerID).FirstOrDefault().ExcludeSkip;
                string procName = "";
                if(ExcludeSkip == 1)
                {
                    procName = "PROC_InstallerAllocation_CurrentDataInstaller_ByDate_New";

                }
                else
                {
                    procName = "PROC_InstallerAllocation_CurrentDataInstaller_WithSkipped_ByDate";
                }
                List<STP_GetExistedUserInstallationbyDate_Result> objList = new List<STP_GetExistedUserInstallationbyDate_Result>();
                SqlConnection sqlConnection1 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;

                //cmd.CommandText = "PROC_InstallerAllocation_CurrentDataInstaller_ByDate";
                cmd.CommandText = procName;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = sqlConnection1;
                cmd.Parameters.Add("@projectId", ProjectID);
                cmd.Parameters.Add("@installerId", InstallerID);
                cmd.Parameters.Add("@FromDate", fromDate);
                cmd.Parameters.Add("@ToDate", toDate);
                //cmd.CommandTimeout = 300;
                cmd.CommandTimeout = sqlConnection1.ConnectionTimeout;
                sqlConnection1.Open();

                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string DynamicRouteColumnName = getDynamicColumnName("ROUTE",reader["FKProjectId"].ToString());
                    string DynamicCycleColumnName = getDynamicColumnName("CYCLE", reader["FKProjectId"].ToString());

                    long tempInstallerId = long.Parse(reader["installerid"].ToString());
                    long tempProjectId = long.Parse(reader["FKProjectId"].ToString());
                    string tempCycle = reader["Cycle"].ToString();
                    string tempFromDate = DateTime.Parse(reader["FromDate"].ToString()).ToString("yyyy-MM-dd");
                    string tempToDate = DateTime.Parse(reader["ToDate"].ToString()).ToString("yyyy-MM-dd");


                    string strQuery = "select Distinct(u." + DynamicRouteColumnName + ") as Route,u." + DynamicCycleColumnName + " as Cycle from tblInstallerMapData as i join uploadedexceldata as u on i.fk_uploadedexceldata_id=u.id where i.InstallerId='" + tempInstallerId + "' and u.FKProjectId='" + tempProjectId + "' and u." + DynamicCycleColumnName + "='" + tempCycle + "' and (i.FromDate between '" + tempFromDate + "' and '" + tempFromDate + "' or i.ToDate between '" + tempToDate + "' and '" + tempToDate + "') order by Route";
                    SqlCommand _cmdRoutes = new SqlCommand(strQuery, sqlConnection1);
                    SqlDataAdapter _sda = new SqlDataAdapter(_cmdRoutes);
                    DataTable _dt = new DataTable();
                    _sda.Fill(_dt);
                    string finalRoute = string.Empty;
                    if (_dt.Rows.Count > 0)
                    {
                        /*string[] values = reader["Route"].ToString().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        foreach (string item in values)
                        {
                            string sd = item.ToString();
                        }*/

                        string strValue = reader["Route"].ToString();
                        string[] strArray = strValue.Split(',');

                        foreach (object obj in strArray)
                        {
                            string cc = obj.ToString();
                            foreach (DataRow item in _dt.Rows)
                            {
                                string ff = item["Route"].ToString();
                                if (cc == ff)
                                {
                                    finalRoute += ff + ",";
                                }
                            }
                        }
                    }

                    finalRoute = finalRoute.Remove(finalRoute.Length - 1, 1);

                    objList.Add(new STP_GetExistedUserInstallationbyDate_Result
                    {
                        FromDate = DateTime.Parse(reader["FromDate"].ToString()),
                        ToDate = DateTime.Parse(reader["ToDate"].ToString()),
                        installerid = long.Parse(reader["installerid"].ToString()),
                        ProjectId = long.Parse(reader["FKProjectId"].ToString()),
                        ProjectName = reader["ProjectName"].ToString(),
                        strFormDate = DateTime.Parse(reader["FromDate"].ToString()).ToString("MMM-dd-yyyy"),
                        strToDate = DateTime.Parse(reader["ToDate"].ToString()).ToString("MMM-dd-yyyy"),
                        UserName = reader["UserName"].ToString(),
                        /*to be uncommented later*/
                        Cycle = reader["Cycle"].ToString(),
                        //Route = reader["Route"].ToString(),
                        Route = finalRoute,
                    });

                }

                sqlConnection1.Close();
                reader.Close();
                reader.Dispose();
                cmd.Dispose();
                sqlConnection1.Dispose();



                //var lat_by_ProjectInstaller = compassEntities.STP_GetExistedUserInstallationbyDate(fromDate, toDate, InstallerID, ProjectID).ToList();

                //foreach (var item in lat_by_ProjectInstaller)
                //{
                //    item.strFormDate = item.FromDate.Value.ToString("MMM-dd-yyyy");
                //    item.strToDate = item.ToDate.Value.ToString("MMM-dd-yyyy");
                //}

                return objList;
            }
        }
    }
}