using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using log4net;

/// <summary>
/// Summary description for clsMail
/// </summary>

namespace GarronT.CMS.Model
{
    /// <summary>
    /// Summary description for tblManufacture
    /// </summary>
    /// 
    [Serializable]
    public class clsMail
    {
        #region Create Object ILog for Logger write
        private static readonly ILog log = LogManager.GetLogger(typeof(clsMail));

        public static object bcc { get; set; }


        #endregion
        /// <summary>
        /// Initializes a new instance of the <see cref="clsMail"/> class.
        /// </summary>
        public clsMail()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        /// <summary>
        /// Sends the mail.
        /// </summary>
        /// <param name="mailIdsList">The mail ids list.</param>
        /// <param name="message">The message.</param>
        /// <param name="subject">The subject.</param>
        /// <returns></returns>
        //public string SendMail(List<string> mailIdsList, string message, string subject)
        //{
        //    string bRet = string.Empty;
        //    //string message = string.Empty;
        //    //string subject = string.Empty;
        //    string webMasterMailId = ConfigurationManager.AppSettings["WebMasterMailId"].ToString();
        //    //string mailIds = ConfigurationManager.AppSettings["SendToMailId"].ToString();
        //    //mailIdsList.Add(mailIds);

        //    SmtpClient mailClient = new SmtpClient();
        //    mailClient.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["WebMasterMailId"].ToString(), ConfigurationManager.AppSettings["WebMasterMailPassword"].ToString());
        //    mailClient.Host = ConfigurationManager.AppSettings["WebMasterSMTPServer"].ToString();
        //    mailClient.Port = int.Parse(ConfigurationManager.AppSettings["WebMasterPort"].ToString());
        //    mailClient.EnableSsl = bool.Parse(ConfigurationManager.AppSettings["WebMasterEnableSSL"].ToString());
        //    MailMessage myMail = new MailMessage();
        //    myMail.From = new MailAddress(webMasterMailId);
        //    foreach (string mailID in mailIdsList)
        //    {
        //        myMail.To.Add(new MailAddress(mailID));
        //    }
        //    myMail.Subject = subject;
        //    myMail.IsBodyHtml = true;
        //    myMail.Body = message;
        //    myMail.Priority = MailPriority.Normal;

        //    try
        //    {
        //        mailClient.Send(myMail);
        //        bRet = "Password Reset Successfull and mailed to Email ID";
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Password Reset Unsuccessfull");
        //    }
        //    return bRet;
        //}
        public string SendMail(List<string> mailIdsList, List<string> CClist, string message, string subject)
        {
            try
            {
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();

                if (mailIdsList.Count > 0)
                {
                    foreach (string mailID in mailIdsList)
                    {
                        mail.To.Add(new MailAddress(mailID));
                    }
                }


                //List<string> CClist = new EmployeeDAL().GetActiveEmployees().Where(p => p.RoleId == "CEO" || p.RoleId == "HR-Manager").Select(p => p.Email).ToList();
                if (CClist != null)
                {
                    foreach (string CCID in CClist)
                    {
                        mail.Bcc.Add(new MailAddress(CCID));
                    }
                }

                mail.From = new MailAddress(ConfigurationManager.AppSettings["WebMasterMailId"]);

                mail.Subject = subject;

                mail.Body = message;

                mail.IsBodyHtml = true;
                //MailAddress bcc = new MailAddress("gayatri@osplabs.com");
                //MailAddress bcc1 = new MailAddress("garron@garront.com");


                //mail.Bcc.Add(bcc);
                //mail.Bcc.Add(bcc1);

                string Host = ConfigurationManager.AppSettings["WebMasterSMTPServer"].ToString();
                int HostPort = int.Parse(ConfigurationManager.AppSettings["WebMasterPort"].ToString());
                SmtpClient smtp = new SmtpClient(Host, HostPort);
                smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings["WebMasterPort"]);
                smtp.Host = ConfigurationManager.AppSettings["WebMasterSMTPServer"]; //"smtp.gmail.com"; //Or Your SMTP Server Address
                smtp.UseDefaultCredentials = true;


                string user = ConfigurationManager.AppSettings["WebMasterMailId"].ToString();
                string password = ConfigurationManager.AppSettings["WebMasterMailPassword"].ToString();
                var enableSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["WebMasterEnableSSL"]);


                smtp.Credentials = new System.Net.NetworkCredential(user, password);
                smtp.EnableSsl = enableSSL;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(mail);
                return "Email sent successfully.";

            }
            catch (SmtpException ex)
            {
                log.Info("Error in  Mail send method at-" + DateTime.Now.ToString());
                log.Error(ex.Message, ex);
                return "Email sending fail";
                throw ex;
            }
            catch (Exception ex)
            {
                log.Info("Error in  Mail send method at-" + DateTime.Now.ToString());
                log.Error(ex.Message, ex);
                return "Email sending fail";
                throw ex;
            }
        }

        public string GetFileContents(string FullPath)
        {
            WebClient webClient = new WebClient();
            return webClient.DownloadString(FullPath);
        }

        //string MailFormat = emailhelper.GetFileContents(HttpContext.Server.MapPath("~/EmailTemplate/RegisterMailTemplate.txt"));
        //string MailBody = String.Format(MailFormat, @ResumeBuilder.Helper.SitePathHelper.BaseAddress, person.FirstName);
        //string IsSent = emailhelper.SendMail(user.UserName, "Welcome to Accenture Infographic Resume Builder", MailBody, null, null);
    }
}
