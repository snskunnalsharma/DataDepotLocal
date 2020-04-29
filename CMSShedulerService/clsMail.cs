using GarronT.CMS.Model;
using GarronT.CMS.Model.BO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CMSShedulerService
{
    [Serializable]
    public class clsMail
    {


        public void SendMail(long projectId, long InstallerId)
        {
            try
            {
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();

                //if (mailIdsList.Count > 0)
                //{
                //    foreach (string mailID in mailIdsList)
                //    {
                //        mail.To.Add(new MailAddress(mailID));
                //    }
                //}


                //if (CClist != null)
                //{
                //    foreach (string CCID in CClist)
                //    {
                //        mail.Bcc.Add(new MailAddress(CCID));
                //    }
                //}


                StringBuilder sb = new StringBuilder();

                CompassEntities objCompassEntities = new CompassEntities();

                var ProjectModel = (from b in objCompassEntities.tblProjects
                                    join c in objCompassEntities.tblStateMasters on b.StateId equals c.StateId
                                    join d in objCompassEntities.TblCityMasters on b.CityId equals d.CityId
                                    where b.ProjectId == projectId
                                    select new ProjectModel
                                    {
                                        ProjectName = b.ProjectName,
                                        StateName = c.StateName,
                                        CityName = d.CityName,
                                        Utilitytype = b.Utilitytype
                                    }).FirstOrDefault();
                var tbluser = objCompassEntities.tblUsers.Where(o => o.UserID == InstallerId).FirstOrDefault();

                string Message = "Please check below details of project & installer who do not have audit percent present.";


                sb.Append("<body   style='padding: 10px 0px 0px 10px; background: #f8f8fb; font-family:Times New Roman; font-size:14px;'>");
                sb.Append("Dear " + " Administrator " + ",");
                sb.Append("  <p style='font-size:15px;'>Greetings from CMS....!</p>");

                sb.Append("<div style='background: #e6e1e1; width:100%;'>" + Message + "</div><br/>");

                sb.Append("<div style='background: #e6e1e1; width:100%;'><table style='width:100%;background: #e6e1e1;border:none;padding: 0px 0px 0px 10px;'>" +
                             "<tr>" +
                                  "<td style='border:none;'><b>Project Name </b></td>" +
                                  "<td style='border:none;'><b>:</b></td>" +
                                  "<td style='border:none;' colspan='3'>" + ProjectModel.ProjectName.ToUpper() + "</td>" +
                              "</tr>" +
                               "<tr>" +
                                  "<td style='border:none;'><b>Utility Type</b></td>" +
                                  "<td style='border:none;'><b>:</b></td>" +
                                  "<td style='border:none;' >" + ProjectModel.Utilitytype + "</td>" +
                              "</tr>" +
                               "<tr>" +
                                  "<td style='border:none;'><b>City:</b></td>" +
                                  "<td style='border:none;'><b>:</b></td>" +
                                 " <td style='border:none;' >" + ProjectModel.CityName.ToUpper() + " &nbsp;&nbsp;</td>" +

                                 " <td style='border:none;'><b>State</b></td>" +
                                 " <td style='border:none;'><b>:</b></td>" +
                                  "<td style='border:none;' >" + ProjectModel.StateName.ToUpper() + "</td>" +
                             " </tr>" +
                              "<tr>" +
                                 " <td style='border:none;'><b>Installer Name</b></td>" +
                                 " <td style='border:none;'><b>:</b></td>" +
                                  "<td style='border:none;' colspan='3'>" + tbluser.FirstName + "   &nbsp;&nbsp;</td>" +
                             "</tr>" +
                            "</table></div>");



                sb.Append("<br/><div></div>");

                sb.Append("Thank You,<br/>Data Depot Team.<br>" +
                            "<p style='text-align:center;'> <a href='https://www.datadepotonline.com/' title='' rel='home'>" +
                            "<img src='https://www.datadepotonline.com/images/logo_03_2.png' title='Compass Metering Solutions' alt='' style='height:100px; width:200px;'>" +
                            "</a> </p>" +
                            "<p style='text-align:center; font-size:13px;'>You are receiving this notification because you are registered with Data Depot<br/>" +
                            " ©2018 Data Depot</p> <br>");
                sb.Append("<p style='text-align:center; color:red; font-size:15px;'>*** This Email is system generated. Please do not reply to this email ID." +
                            "For any queries please contact Data Depot Team. ***</p>");
                sb.Append("<p style='text-align:center; color:red; font-size:13px;'>This message (including any attachments) contains confidential information" +
                            "intended for a specific individual and purpose and is protected by law." +
                            "If you are not the intended recipient, you should delete this message." +
                            "Any disclosure, copying or distribution of this message, or the taking of any action based on it, is strictly prohibited.</p>");
                sb.Append("</body>");
                string MailBody = sb.ToString();




                //mail.To.Add(new MailAddress("bharat.magdum@kairee.in"));

                mail.From = new MailAddress(ConfigurationManager.AppSettings["WebMasterMailId"]);

                mail.Subject = "Data Depot Audit Percentage Not Present For Installer";

                mail.Body = MailBody;

                mail.IsBodyHtml = true;
                MailAddress bcc = new MailAddress("gayari@osplabs.com");
                mail.Bcc.Add(bcc);

                MailAddress bcc1 = new MailAddress("garron@garront.com");
                mail.Bcc.Add(bcc1);

                string Host = ConfigurationManager.AppSettings["WebMasterSMTPServer"].ToString();
                int HostPort = int.Parse(ConfigurationManager.AppSettings["WebMasterPort"].ToString());
                SmtpClient smtp = new SmtpClient(Host, HostPort);



                string user = ConfigurationManager.AppSettings["WebMasterMailId"].ToString();
                string password = ConfigurationManager.AppSettings["WebMasterMailPassword"].ToString();
                var enableSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["WebMasterEnableSSL"]);


                smtp.Credentials = new System.Net.NetworkCredential(user, password);
                smtp.EnableSsl = enableSSL;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(mail);


            }
            catch (SmtpException ex)
            {
                clsErrorLogFile.writeStatus("Error in  Mail send method at-" + DateTime.Now.ToString());
                clsErrorLogFile.writeStatus(ex.Message);

            }
            catch (Exception ex)
            {
                clsErrorLogFile.writeStatus("Error in  Mail send method at-" + DateTime.Now.ToString());
                clsErrorLogFile.writeStatus(ex.Message);

            }
        }



        public void SendMail(long projectId)
        {
            try
            {
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();

                //if (mailIdsList.Count > 0)
                //{
                //    foreach (string mailID in mailIdsList)
                //    {
                //        mail.To.Add(new MailAddress(mailID));
                //    }
                //}


                //if (CClist != null)
                //{
                //    foreach (string CCID in CClist)
                //    {
                //        mail.Bcc.Add(new MailAddress(CCID));
                //    }
                //}


                StringBuilder sb = new StringBuilder();

                CompassEntities objCompassEntities = new CompassEntities();

                var ProjectModel = (from b in objCompassEntities.tblProjects
                                    join c in objCompassEntities.tblStateMasters on b.StateId equals c.StateId
                                    join d in objCompassEntities.TblCityMasters on b.CityId equals d.CityId
                                    where b.ProjectId == projectId
                                    select new ProjectModel
                                    {
                                        ProjectName = b.ProjectName,
                                        StateName = c.StateName,
                                        CityName = d.CityName,
                                        Utilitytype = b.Utilitytype
                                    }).FirstOrDefault();


                string Message = "Please check below details of project not having audit percent present for the customer.";


                sb.Append("<body   style='padding: 10px 0px 0px 10px; background: #f8f8fb; font-family:Times New Roman; font-size:14px;'>");
                sb.Append("Dear " + " Administrator " + ",");
                sb.Append("  <p style='font-size:15px;'>Greetings from Data Depot....!</p>");

                sb.Append("<div style='background: #e6e1e1; width:100%;'>" + Message + "</div><br/>");

                sb.Append("<div style='background: #e6e1e1; width:100%;'><table style='width:100%;background: #e6e1e1;border:none;padding: 0px 0px 0px 10px;'>" +
                             "<tr>" +
                                  "<td style='border:none;'><b>Project Name </b></td>" +
                                  "<td style='border:none;'><b>:</b></td>" +
                                  "<td style='border:none;' colspan='3'>" + ProjectModel.ProjectName.ToUpper() + "</td>" +
                              "</tr>" +
                               "<tr>" +
                                  "<td style='border:none;'><b>Utility Type</b></td>" +
                                  "<td style='border:none;'><b>:</b></td>" +
                                  "<td style='border:none;' >" + ProjectModel.Utilitytype + "</td>" +
                              "</tr>" +
                               "<tr>" +
                                  "<td style='border:none;'><b>City:</b></td>" +
                                  "<td style='border:none;'><b>:</b></td>" +
                                 " <td style='border:none;' >" + ProjectModel.CityName.ToUpper() + " &nbsp;&nbsp;</td>" +

                                 " <td style='border:none;'><b>State</b></td>" +
                                 " <td style='border:none;'><b>:</b></td>" +
                                  "<td style='border:none;' >" + ProjectModel.StateName.ToUpper() + "</td>" +
                             " </tr>" +

                            "</table></div>");



                sb.Append("<br/><div></div>");

                sb.Append("Thank You,<br/>Data Depot Team.<br>" +
                            "<p style='text-align:center;'> <a href='https://www.datadepotonline.com' title='' rel='home'>" +
                            "<img src='https://www.datadepotonline.com/images/logo_03_2.png' title='Data Depot' alt='' style='height:100px; width:200px;'>" +
                            "</a> </p>" +
                            "<p style='text-align:center; font-size:13px;'>You are receiving this notification because you are registered with Data Depot <br/>" +
                            " ©2018 Data Depot</p> <br>");
                sb.Append("<p style='text-align:center; color:red; font-size:15px;'>*** This Email is system generated. Please do not reply to this email ID." +
                            " For any queries please contact Data Depot Team. ***</p>");
                sb.Append("<p style='text-align:center; color:red; font-size:13px;'>This message (including any attachments) contains confidential information" +
                            " intended for a specific individual and purpose and is protected by law." +
                            " If you are not the intended recipient, you should delete this message." +
                            " Any disclosure, copying or distribution of this message, or the taking of any action based on it, is strictly prohibited.</p>");
                sb.Append("</body>");
                string MailBody = sb.ToString();




                //mail.To.Add(new MailAddress("bharat.magdum@kairee.in"));

                mail.From = new MailAddress(ConfigurationManager.AppSettings["WebMasterMailId"]);

                mail.Subject = "Data Depot Audit Percentage Not Present For Customer";

                mail.Body = MailBody;

                mail.IsBodyHtml = true;
                //MailAddress bcc = new MailAddress("gayatri@osplabs.com");
                //mail.Bcc.Add(bcc);

                //MailAddress bcc1 = new MailAddress("garron@garront.com");
                //mail.Bcc.Add(bcc1);

                string Host = ConfigurationManager.AppSettings["WebMasterSMTPServer"].ToString();
                int HostPort = int.Parse(ConfigurationManager.AppSettings["WebMasterPort"].ToString());
                SmtpClient smtp = new SmtpClient(Host, HostPort);



                string user = ConfigurationManager.AppSettings["WebMasterMailId"].ToString();
                string password = ConfigurationManager.AppSettings["WebMasterMailPassword"].ToString();
                var enableSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["WebMasterEnableSSL"]);


                smtp.Credentials = new System.Net.NetworkCredential(user, password);
                smtp.EnableSsl = enableSSL;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(mail);


            }
            catch (SmtpException ex)
            {
                clsErrorLogFile.writeStatus("Error in  Mail send method at-" + DateTime.Now.ToString());
                clsErrorLogFile.writeStatus(ex.Message);

            }
            catch (Exception ex)
            {
                clsErrorLogFile.writeStatus("Error in  Mail send method at-" + DateTime.Now.ToString());
                clsErrorLogFile.writeStatus(ex.Message);

            }
        }

    }
}
