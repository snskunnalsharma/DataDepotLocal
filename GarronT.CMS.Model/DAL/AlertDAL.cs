using GarronT.CMS.Model.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GarronT.CMS.Model;
namespace GarronT.CMS.Model.DAL
{
    public class AlertDAL
    {
        public List<AlertModel> GetAlertList(bool _val)
        {
            int id = 0;
            if (_val)
            {
                id = 1;
            }
            else
                id = 0;
            CompassEntities objCompassEntities = new CompassEntities();



            List<AlertModel> objAlertModelList = objCompassEntities.TblAlertMasters.Where(o => o.Active == id).
                Select(o => new AlertModel
                {
                    AlertId = o.AlertId,
                    AlertName = o.AlertName,
                    AdminEmailId = objCompassEntities.TblEmailConfigures.Where(x => x.AlertTableID == o.AlertId).Select(x => x.AdminEmailID).FirstOrDefault(),

                    ProjectName = objCompassEntities.tblProjects.Where(x => x.ProjectId == o.ProjectID).Select(x => x.ProjectName).FirstOrDefault(),

                }).ToList();

            return objAlertModelList;
        }

        public List<UserDetailsModel> GetUserDeatils(string[] id)
        {
            CompassEntities CompassEntities = new CompassEntities();
            var list = new List<UserDetailsModel>();
            int i = 1;
            int tempuserid = 0;
            try
            {
                if (id != null)
                {


                    foreach (var item in id)
                    {
                        tempuserid = Convert.ToInt32(item);

                        List<UserDetailsModel> objSubMenuesBo = (from b in CompassEntities.tblUsers
                                                                 where b.Active == 1 && b.UserID == tempuserid
                                                                 select new UserDetailsModel
                                                                 {
                                                                     //ID = b.UserName,
                                                                     FirstName = b.FirstName,

                                                                     MobileNumber = b.MobileNo.ToString(),
                                                                     EmailId = b.Email
                                                                 }).ToList();


                        foreach (var obj in objSubMenuesBo)
                        {

                            UserDetailsModel myobj = new UserDetailsModel();
                            myobj.TempID = i;
                            myobj.FirstName = obj.FirstName;

                            myobj.MobileNumber = obj.MobileNumber;
                            myobj.EmailId = obj.EmailId;
                            i = i + 1;
                            list.Add(myobj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return list;
        }
        public List<UserDetailsModel> GetUserList()
        {
            CompassEntities CompassEntities = new CompassEntities();
            var list = new List<UserDetailsModel>();
            //string role = "c01e6161-e020-4a0b-954e-c3d1b405bdc0";//c01e6161-e020-4a0b-954e-c3d1b405bdc0
            string role = "Admin";
            List<long?> objUserId = new UserDAL().GetUserRecords(role);
            List<UserDetailsModel> objSubMenuesBo = (from b in CompassEntities.tblUsers
                                                     where b.Active == 1 && objUserId.Contains(b.UserID)
                                                     select new UserDetailsModel
                                                     {
                                                         FullName = b.FirstName ,
                                                         UserID = b.UserID
                                                     }).ToList();

            int i = 1;
            foreach (var obj in objSubMenuesBo)
            {

                UserDetailsModel myobj = new UserDetailsModel();
                myobj.TempID = i;
                myobj.FullName = obj.FullName;
                myobj.UserID = obj.UserID;
                i = i + 1;
                list.Add(myobj);
            }
            return list;
        }
        public AlertModel GetEditRecord(long Id)
        {
            CompassEntities objCompassEntities = new CompassEntities();

            AlertModel result = (from tbl in objCompassEntities.TblAlertMasters
                                 where tbl.AlertId == Id
                                 select new AlertModel
                                 {
                                     AlertName = tbl.AlertName,
                                     AlertId = tbl.AlertId
                                 }).FirstOrDefault();



            return result;

        }

        public bool InsertRecord(EmailConfigureBO objEmailConfigureBO, List<string> UserID, out string _result)
        {
            CompassEntities objCompassEntities = new CompassEntities();

            var a = objCompassEntities.TblEmailConfigures.Where(x => x.AlertTableID == objEmailConfigureBO.AlertTableID).FirstOrDefault();

            if (a == null)
            {

                TblEmailConfigure obj1 = new TblEmailConfigure();
                obj1.Active = 1;
                obj1.Message = objEmailConfigureBO.Message;
                obj1.Subject = objEmailConfigureBO.Subject;
                obj1.AlertTableID = objEmailConfigureBO.AlertTableID;

                obj1.CreatedBy = 1;
                obj1.CreatedOn = new CommonFunctions().ServerDate();

                objCompassEntities.TblEmailConfigures.Add(obj1);
                objCompassEntities.SaveChanges();

                foreach (var item in UserID)
                {
                    long Userid = Convert.ToInt64(item);
                    TblEmailConfigureUserDetail obj = new TblEmailConfigureUserDetail();
                    obj.TblEmailConfigureID = obj1.ID;
                    obj.UserID = Userid;
                    obj.CreatedBy = 1;
                    obj.Active = 1;
                    obj.CreatedOn = new CommonFunctions().ServerDate();
                    objCompassEntities.TblEmailConfigureUserDetails.Add(obj);
                    objCompassEntities.SaveChanges();

                }

                _result = "Success";
                return true;
            }
            else
            {
                a.Active = 1;
                a.Message = objEmailConfigureBO.Message;
                a.Subject = objEmailConfigureBO.Subject;
                a.AlertTableID = objEmailConfigureBO.AlertTableID;

                a.CreatedBy = 1;
                a.CreatedOn = new CommonFunctions().ServerDate();
                objCompassEntities.SaveChanges();

                foreach (var item in UserID)
                {
                    long Userid = Convert.ToInt64(item);
                    var Checkpresent = objCompassEntities.TblEmailConfigureUserDetails.Where(x => x.UserID == Userid && x.TblEmailConfigureID == a.ID).FirstOrDefault();
                    if (Checkpresent == null)
                    {
                        TblEmailConfigureUserDetail obj = new TblEmailConfigureUserDetail();
                        obj.TblEmailConfigureID = a.ID;
                        obj.UserID = Userid;
                        obj.CreatedBy = 1;
                        obj.Active = 1;
                        obj.CreatedOn = new CommonFunctions().ServerDate();
                        objCompassEntities.TblEmailConfigureUserDetails.Add(obj);
                        objCompassEntities.SaveChanges();
                    }
                }
                _result = "Success";
                return true;
            }

        }

        public EmailConfigureBO GetEmailConfigureDetails(long AlertID)
        {
            EmailConfigureBO result = null;
            CompassEntities CompassEntities = new CompassEntities();
            EmailConfigureBO b = new EmailConfigureBO();
            try
            {
                var data = CompassEntities.TblEmailConfigures.Where(x => x.AlertTableID == AlertID).FirstOrDefault();
                if (data != null)
                {
                    result = (from Tbl in CompassEntities.TblEmailConfigureUserDetails
                              where Tbl.TblEmailConfigureID == data.ID
                              select new EmailConfigureBO
                              {
                                  AlertTableID = data.AlertTableID,
                                  Subject = data.Subject,
                                  Message = data.Message,
                                  Userid = CompassEntities.TblEmailConfigureUserDetails.Where(x => x.TblEmailConfigureID == data.ID).Select(x => x.UserID.ToString()).ToList(),

                              }).FirstOrDefault();



                }


            }
            catch (Exception ex)
            {

                throw ex;
            }
            return result;
        }

        public bool UpdateRecord(EmailConfigureBO objEmailConfigureBO, out string _result)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            bool b = false;
            _result = "";
            TblEmailConfigure a = objCompassEntities.TblEmailConfigures.Where(x => x.AlertTableID == objEmailConfigureBO.AlertTableID).FirstOrDefault();

            if (a == null)
            {


                a.Active = 1;
                a.Message = objEmailConfigureBO.Message;
                a.Subject = objEmailConfigureBO.Subject;
                a.AlertTableID = objEmailConfigureBO.AlertTableID;

                a.CreatedBy = 1;
                a.CreatedOn = new CommonFunctions().ServerDate();


                objCompassEntities.SaveChanges();
                _result = "Success";
                b = true;
            }
            return b;
        }
    }

}
