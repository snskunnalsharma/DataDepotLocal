using GarronT.CMS.Model.BO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.DAL
{
    public class DuplicateReportDAL : IDisposable
    {

        bool disposed;
        protected virtual void Dispose(bool disposing)
        {
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Returns project list for given status. 
        /// </summary>
        /// <param name="proStatus">Active/Complete</param>
        /// <returns>Project List</returns>
        public List<ProjectModel> GetProjectName(string proStatus)
        {
            CompassEntities CompassEntities = new CompassEntities();

            List<ProjectModel> data = CompassEntities.STP_GetProjectCityState().Where(a => a.Active == 1 && a.ProjectStatus == proStatus).Select(o =>
                                       new ProjectModel
                                       {
                                           ProjectId = o.ProjectId,
                                           ProjectCityState = o.ProjectCityName
                                       }).OrderBy(a => a.ProjectCityState).ToList();
            return data;
        }




        /// <summary>
        /// get Installer list for the current project
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <returns></returns>
        public List<UsersSmallModel> GeInstallerListCurrentProject(long ProjectId)
        {
            try
            {
                CompassEntities CompassEntities = new CompassEntities();
                List<UsersSmallModel> InstallerList = (from b in CompassEntities.TblProjectInstallers
                                                       where b.Active == 1 && b.ProjectID == ProjectId
                                                       select new UsersSmallModel
                                                       {
                                                           UserId = b.tblUsers_UserID,
                                                           UserName = CompassEntities.tblUsers.Where(a => a.UserID == b.tblUsers_UserID).Select(m => m.FirstName).FirstOrDefault()
                                                       }).ToList();



                return InstallerList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// get Installer list for the current project
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <returns></returns>
        public List<DuplicateReportBO> GetDuplicateRecords(long projectId, long installerId, int SearchFieldId)
        {
            try
            {
                CompassEntities objCompassEntities = new CompassEntities();
                List<DuplicateReportBO> objDuplicateList = new List<DuplicateReportBO>();


                List<PROC_GetDuplicateRadio_Result> list = new List<PROC_GetDuplicateRadio_Result>();

                SqlConnection sqlConnection1 = new SqlConnection(objCompassEntities.Database.Connection.ConnectionString);
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;

                cmd.CommandText = "PROC_ReportHome_GetDuplicateRadio";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = sqlConnection1;
                cmd.CommandTimeout = 180;
                cmd.Parameters.Add("@projectId", projectId);


                sqlConnection1.Open();
                DateTime? dt1 = null;
                int? s1 = null;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    //Route = reader["Route"].ToString(), Cycle = reader["Cycle"].ToString() 
                    list.Add(new PROC_GetDuplicateRadio_Result
                    {

                        Account = Convert.ToString(reader["Account"]),

                        Customer = Convert.ToString(reader["Customer"]),
                        FirstName = Convert.ToString(reader["FirstName"]),
                        ID = long.Parse(reader["Id"].ToString()),
                        NEWMETERRADIO = Convert.ToString(reader["NEWMETERRADIO"]),
                        NoRecords = !string.IsNullOrEmpty(Convert.ToString(reader["NoRecords"])) ? int.Parse(reader["NoRecords"].ToString()) : s1,
                        OldMeterNo = Convert.ToString(reader["OldMeterNo"]),
                        OldMeterRadioNo = Convert.ToString(reader["OldMeterRadioNo"]),
                        Street = Convert.ToString(reader["Street"]),
                        UserID = long.Parse(Convert.ToString(reader["UserID"])),
                        visitdatetime = !string.IsNullOrEmpty(Convert.ToString(reader["visitdatetime"])) ? DateTime.Parse(reader["visitdatetime"].ToString()) : dt1
                    });
                }

                sqlConnection1.Close();
                reader.Close();
                reader.Dispose();
                cmd.Dispose();
                sqlConnection1.Dispose();




                List<PROC_GetDuplicateMeterNo_Result> list1 = new List<PROC_GetDuplicateMeterNo_Result>();
                sqlConnection1 = new SqlConnection(objCompassEntities.Database.Connection.ConnectionString);
                cmd = new SqlCommand();
                reader = null;

                cmd.CommandText = "PROC_ReportHome_GetDuplicateMeterNo";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = sqlConnection1;
                cmd.CommandTimeout = 180;
                cmd.Parameters.Add("@projectId", projectId);


                sqlConnection1.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    //Route = reader["Route"].ToString(), Cycle = reader["Cycle"].ToString() 
                    list1.Add(new PROC_GetDuplicateMeterNo_Result
                    {

                        Account = Convert.ToString(reader["Account"]),
                        Customer = Convert.ToString(reader["Customer"]),
                        FirstName = Convert.ToString(reader["FirstName"]),
                        ID = long.Parse(reader["Id"].ToString()),
                        NEWMETERNUMBER = Convert.ToString(reader["NEWMETERNUMBER"]),
                        NoRecords = !string.IsNullOrEmpty(Convert.ToString(reader["NoRecords"])) ? int.Parse(reader["NoRecords"].ToString()) : s1,
                        OldMeterNo = Convert.ToString(reader["OldMeterNo"]),
                        OldMeterRadioNo = Convert.ToString(reader["OldMeterRadioNo"]),
                        Street = Convert.ToString(reader["Street"]),
                        UserID = long.Parse(Convert.ToString(reader["UserID"])),
                        visitdatetime = !string.IsNullOrEmpty(Convert.ToString(reader["visitdatetime"])) ? DateTime.Parse(reader["visitdatetime"].ToString()) : dt1
                    });
                }

                sqlConnection1.Close();
                reader.Close();
                reader.Dispose();
                cmd.Dispose();
                sqlConnection1.Dispose();

                if (installerId > 0)
                {

                    if (SearchFieldId == 0 || SearchFieldId == 2)
                    {
                        // if search field is not selected or new meter radio number selected
                        var dataList = list.Where(o => o.UserID == installerId).ToList();
                        dataList.ForEach(o => objDuplicateList.Add(new DuplicateReportBO
                        {
                            Account = o.Account,
                            Customer = (!string.IsNullOrEmpty(o.Customer) ? o.Customer : "-"),
                            FirstName = o.FirstName,
                            ID = o.ID,
                            NEWMETERNUMBER = "-",
                            NEWMETERRADIO = (!string.IsNullOrEmpty(o.NEWMETERRADIO) ? o.NEWMETERRADIO : "-"),
                            OldMeterNo = (!string.IsNullOrEmpty(o.OldMeterNo) ? o.OldMeterNo : "-"),
                            OldMeterRadioNo = (!string.IsNullOrEmpty(o.OldMeterRadioNo) ? o.OldMeterRadioNo : "-"),
                            Street = o.Street,
                            visitdatetime = (o.visitdatetime != null ? DateTime.Parse(o.visitdatetime.ToString()).ToString("MMM-dd-yyyy hh:mm tt") : "-")
                        }));
                    }

                    if (SearchFieldId == 0 || SearchFieldId == 1)
                    {
                        // if search field is not selected or new meter number selected
                        var dataList1 = list1.Where(o => o.UserID == installerId).ToList();
                        dataList1.ForEach(o => objDuplicateList.Add(new DuplicateReportBO
                        {
                            Account = o.Account,
                            Customer = (!string.IsNullOrEmpty(o.Customer) ? o.Customer : "-"),
                            FirstName = o.FirstName,
                            ID = o.ID,
                            NEWMETERNUMBER = (!string.IsNullOrEmpty(o.NEWMETERNUMBER) ? o.NEWMETERNUMBER : "-"),
                            NEWMETERRADIO = "-",
                            OldMeterNo = (!string.IsNullOrEmpty(o.OldMeterNo) ? o.OldMeterNo : "-"),
                            OldMeterRadioNo = (!string.IsNullOrEmpty(o.OldMeterRadioNo) ? o.OldMeterRadioNo : "-"),
                            Street = o.Street,
                            visitdatetime = (o.visitdatetime != null ? DateTime.Parse(o.visitdatetime.ToString()).ToString("MMM-dd-yyyy hh:mm tt") : "-")
                        }));
                    }
                }
                else
                {
                    if (SearchFieldId == 0 || SearchFieldId == 2)
                    {
                        // if search field is not selected or new meter radio number selected

                        list.ForEach(o => objDuplicateList.Add(new DuplicateReportBO
                        {
                            Account = o.Account,
                            Customer = (!string.IsNullOrEmpty(o.Customer) ? o.Customer : "-"),
                            FirstName = o.FirstName,
                            ID = o.ID,
                            NEWMETERNUMBER = "-",
                            NEWMETERRADIO = (!string.IsNullOrEmpty(o.NEWMETERRADIO) ? o.NEWMETERRADIO : "-"),
                            OldMeterNo = (!string.IsNullOrEmpty(o.OldMeterNo) ? o.OldMeterNo : "-"),
                            OldMeterRadioNo = (!string.IsNullOrEmpty(o.OldMeterRadioNo) ? o.OldMeterRadioNo : "-"),
                            Street = o.Street,
                            visitdatetime = (o.visitdatetime != null ? DateTime.Parse(o.visitdatetime.ToString()).ToString("MMM-dd-yyyy hh:mm tt") : "-")
                        }));
                    }

                    if (SearchFieldId == 0 || SearchFieldId == 1)
                    {
                        // if search field is not selected or new meter number selected
                        list1.ForEach(o => objDuplicateList.Add(new DuplicateReportBO
                        {
                            Account = o.Account,
                            Customer = (!string.IsNullOrEmpty(o.Customer) ? o.Customer : "-"),
                            FirstName = o.FirstName,
                            ID = o.ID,
                            NEWMETERNUMBER = (!string.IsNullOrEmpty(o.NEWMETERNUMBER) ? o.NEWMETERNUMBER : "-"),
                            NEWMETERRADIO = "-",
                            OldMeterNo = (!string.IsNullOrEmpty(o.OldMeterNo) ? o.OldMeterNo : "-"),
                            OldMeterRadioNo = (!string.IsNullOrEmpty(o.OldMeterRadioNo) ? o.OldMeterRadioNo : "-"),
                            Street = o.Street,
                            visitdatetime = (o.visitdatetime != null ? DateTime.Parse(o.visitdatetime.ToString()).ToString("MMM-dd-yyyy hh:mm tt") : "-")
                        }));
                    }
                }

                return objDuplicateList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
