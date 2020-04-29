using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using Microsoft.Reporting.WebForms;
using System.Web.Services;
using System.Web.Script.Serialization;

 
using GarronT.CMS.Model.DAL;
using GarronT.CMS.Model.BO;

namespace GarronT.CMS.UI.ReportViewer
{
    public partial class Report : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var ID = 4155;
            var frmID = 7;
            var list = new ReportDAL().GetReportsRecordsID(ID, frmID);

            DataSet dsreport = new ReportDAL().getListReport(ID, frmID);
            var listitem = list.GroupBy(o => o.sectionName).ToList();

            List<sectionfiledval> sectionfieldlist = new System.Collections.Generic.List<sectionfiledval>();
            sectionfieldlist.Add(new sectionfiledval
            {
                IDS = 1,
                sectionName = "section 1",
                FieldId=1,
                FieldName="account",
                FieldValues="11111"
            });
            sectionfieldlist.Add(new sectionfiledval
            {
                IDS = 2,
                sectionName = "section 2",
                FieldId = 1,
                FieldName = "account",
                FieldValues = "11111"
            });

            var reportDataSource = new Microsoft.Reporting.WebForms.ReportDataSource { Name = "DataSet1", Value = dsreport.Tables[0] };

            ReportViewerTSPPerformance.ProcessingMode = ProcessingMode.Local;
            //var report = new Microsoft.Reporting.WebForms.LocalReport();
            //report.DataSources.Add(reportDataSource);
           
            ReportViewerTSPPerformance.LocalReport.DataSources.Clear();

            ReportViewerTSPPerformance.LocalReport.DataSources.Add(reportDataSource);
            ReportViewerTSPPerformance.LocalReport.EnableExternalImages = true;
            ReportViewerTSPPerformance.LocalReport.ReportPath = Server.MapPath("~/Reports/ListReport.rdlc");
            ReportViewerTSPPerformance.LocalReport.SubreportProcessing += LocalReport_SubreportProcessing;
            ReportViewerTSPPerformance.DataBind();
            ReportViewerTSPPerformance.LocalReport.Refresh();

        }

        void LocalReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            string FieldValues = (e.Parameters["FieldValues"].Values.First()).ToString();
            string IDS = (e.Parameters["IDS"].Values.First()).ToString();

         //   e.DataSources.Add(new ReportDataSource("Refs_SubDs", SqlDs_RefsReportsSub));


        }
    }
}