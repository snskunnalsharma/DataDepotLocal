using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class UserDetailsDAL
    {
        public List<UserDetailsModel> GetUserDeatils(int id)
        {
            CompassEntities CompassEntities = new CompassEntities();
            var list = new List<UserDetailsModel>();
            int i = 1;
            List<UserDetailsModel> objSubMenuesBo = (from b in CompassEntities.tblUsers
                                                     where b.Active == 1 && b.UserID == id
                                                     select new UserDetailsModel
                                                     {
                                                         //ID = b.UserName,
                                                         FirstName = b.FirstName,
                                                         LastName = b.LastName,
                                                         MobileNumber = Convert.ToString(b.MobileNo),
                                                         EmailId = b.Email
                                                     }).ToList();


            foreach (var obj in objSubMenuesBo)
            {

                UserDetailsModel myobj = new UserDetailsModel();
                myobj.TempID = i;
                myobj.FirstName = obj.FirstName;
                myobj.LastName = obj.LastName;
                myobj.MobileNumber = obj.MobileNumber;
                myobj.EmailId = obj.EmailId;
                i = i + 1;
                list.Add(myobj);
            }
            return list;
        }

    }
}
