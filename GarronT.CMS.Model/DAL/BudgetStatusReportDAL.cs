using GarronT.CMS.Model.BO;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Globalization;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace GarronT.CMS.Model.DAL
{
    public class BudgetStatusReportDAL : IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BudgetStatusReportDAL));
        private bool disposed;

        public List<BudgetUtilityBO> GetUtilityTypeList()
        {
            using (CompassEntities db = new CompassEntities())
            {
                return db.UtilityMasters
                    .Where(o => o.Active == 1)
                    .Select(o => new BudgetUtilityBO
                    {
                        Id = o.Id,
                        UtilityName = o.UtilityName
                    })
                    .OrderBy(o => o.UtilityName)
                    .ToList();
            }
        }

        public List<BudgetProjectBO> GetProjectByUtility(int utilityId)
        {
            using (CompassEntities db = new CompassEntities())
            {
                string utilityName = db.UtilityMasters
                    .Where(o => o.Id == utilityId && o.Active == 1)
                    .Select(o => o.UtilityName)
                    .FirstOrDefault();

                if (string.IsNullOrWhiteSpace(utilityName))
                {
                    return new List<BudgetProjectBO>();
                }

                return db.STP_GetProjectCityState()
                    .Where(o => o.Active == 1 && o.Utilitytype == utilityName)
                    .Select(o => new BudgetProjectBO
                    {
                        ProjectId = o.ProjectId,
                        ProjectCityState = o.ProjectCityName,
                        ProjectName = o.ProjectCityName
                    })
                    .OrderBy(o => o.ProjectCityState)
                    .ToList();
            }
        }

        public List<BudgetInstallerBO> GetInstallerByProject(long projectId)
        {
            using (UserRoleDAL userRoleDAL = new UserRoleDAL())
            {
                return userRoleDAL.GeInstallerListCurrentProject(projectId)
                    .Select(o => new BudgetInstallerBO
                    {
                        UserId = o.UserId,
                        UserName = o.UserName
                    })
                    .OrderBy(o => o.UserName)
                    .ToList();
            }
        }

        public BudgetStatusReportResultBO GetBudgetStatusReport(long? projectId, long? installerId, string fromDate, string toDate)
        {
            BudgetStatusReportResultBO result = new BudgetStatusReportResultBO();

            if (!projectId.HasValue || projectId.Value <= 0)
                return result;

            DateTime parsedFromDate;
            DateTime parsedToDate;

            if (!DateTime.TryParse(fromDate, out parsedFromDate))
                throw new Exception("From Date is required.");
            if (!DateTime.TryParse(toDate, out parsedToDate))
                throw new Exception("To Date is required.");

            using (CompassEntities db = new CompassEntities())
            {
                db.Database.CommandTimeout = 180;
                List<BudgetStatusReportBO> allRows = new List<BudgetStatusReportBO>();

                // Check if project has service line budget configured
                var serviceLineBudgetRecord = db.ProjectBudgetMaster
                    .Where(o => o.ProjectId == projectId.Value && o.BudgetCode == "SERVICE_LINE_BUDGET" && o.IsActive == true)
                    .FirstOrDefault();

                bool hasServiceLineBudget = serviceLineBudgetRecord != null || projectId.Value == 10134;

                // === Project 10134 (Beaumont) logic ===
                if (projectId.Value == 10134)
                {
                    using (SqlConnection con = new SqlConnection(db.Database.Connection.ConnectionString))
                    {
                        con.Open();

                        var serviceConfig = GetTimeClockServiceConfig(projectId.Value, con);

                        var installedMeters = GetBeaumont21InstalledMetersDetails(
                            projectId.Value,
                            installerId,
                            parsedFromDate.Date,
                            parsedToDate.Date.AddDays(1),
                            con
                        );

                        foreach (var service in serviceConfig.Values.OrderBy(x => x.DisplayOrder))
                        {
                            int count = installedMeters.Count(x => x.MeterDisplayName == service.ServiceDisplayName);
                            allRows.Add(new BudgetStatusReportBO
                            {
                                ServiceDescription = service.ServiceDisplayName,
                                UnitCost = service.Cost,
                                TotalCount = count,
                                TotalCost = service.Cost * count
                            });
                        }
                    }
                }
                else
                {
                    // Other projects use the default stored procedure
                    allRows = db.Database.SqlQuery<BudgetStatusReportBO>(
                        "EXEC dbo.PROC_GetBudgetStatusReport @ProjectId, @InstallerId, @FromDate, @ToDate",
                        new SqlParameter("@ProjectId", projectId.Value),
                        new SqlParameter("@InstallerId", installerId.HasValue ? (object)installerId.Value : DBNull.Value),
                        new SqlParameter("@FromDate", parsedFromDate.Date),
                        new SqlParameter("@ToDate", parsedToDate.Date)
                    ).ToList();
                }

                if (hasServiceLineBudget)
                {
                    foreach (var row in allRows)
                    {
                        bool isServiceLineRow = !string.IsNullOrEmpty(row.ServiceDescription) &&
                            (row.ServiceDescription.IndexOf("Line Inspection", StringComparison.OrdinalIgnoreCase) >= 0 ||
                             row.ServiceDescription.IndexOf("Lead Line", StringComparison.OrdinalIgnoreCase) >= 0);

                        if (isServiceLineRow)
                        {
                            bool keepOriginalName = string.Equals(row.ServiceDescription, "Line Inspection Dig Outside Box", StringComparison.OrdinalIgnoreCase) ||
                                                    string.Equals(row.ServiceDescription, "Line Inspection Inside Meter Box", StringComparison.OrdinalIgnoreCase) ||
                                                    string.Equals(row.ServiceDescription, "Lead Line Inspection (Outside the Box) with ERT Mitigation", StringComparison.OrdinalIgnoreCase);
                            if (!keepOriginalName)
                            {
                                row.ServiceDescription = "Service Line";
                            }
                            result.ServiceLineReportRows.Add(row);
                        }
                        else
                        {
                            result.ReportRows.Add(row);
                        }
                    }

                    // Fetch project budget from ProjectBudgetMaster
                    var projectBudgetRecord = db.ProjectBudgetMaster
                        .Where(o => o.ProjectId == projectId.Value && o.BudgetCode == "PROJECT_BUDGET" && o.IsActive == true)
                        .FirstOrDefault();

                    result.ApprovedProjectBudget = projectBudgetRecord != null
                        ? projectBudgetRecord.BudgetAmount
                        : GetApprovedProjectBudget(db, projectId.Value);

                    result.ServiceLineBudget = serviceLineBudgetRecord != null
                        ? serviceLineBudgetRecord.BudgetAmount
                        : 500000.00m; // fallback for 10134
                }
                else
                {
                    result.ReportRows = allRows;
                    result.ApprovedProjectBudget = GetApprovedProjectBudget(db, projectId.Value);
                    result.ServiceLineBudget = 0;
                }

                result.UsedBudget = result.ReportRows.Sum(o => o.TotalCost);
                result.BalanceBudget = result.ApprovedProjectBudget - result.UsedBudget;

                result.ServiceLineUsedBudget = result.ServiceLineReportRows.Sum(o => o.TotalCost);
                result.ServiceLineBalanceBudget = result.ServiceLineBudget - result.ServiceLineUsedBudget;
            }

            return result;
        }
        private decimal GetApprovedProjectBudget(CompassEntities db, long projectId)
        {
            /*
                Option 1:
                If ProjectBudget column exists in tblProjects entity after EDMX update,
                use this block.
            */

            var projectBudget = db.tblProjects
                .Where(o => o.ProjectId == projectId)
                .Select(o => o.ProjectBudget)
                .FirstOrDefault();

            if (projectBudget == null)
            {
                return 0;
            }

            return Convert.ToDecimal(projectBudget);
        }

        public byte[] ExportBudgetStatusReport(long? projectId, long? installerId, string fromDate, string toDate, string exportedBy = "")
        {
            BudgetStatusReportResultBO reportResult = GetBudgetStatusReport(projectId, installerId, fromDate, toDate);

            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.LETTER, 36f, 36f, 36f, 36f);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 13f, BaseColor.BLACK);

                Font infoLabelFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9f, BaseColor.BLACK);
                Font infoValueFont = FontFactory.GetFont(FontFactory.HELVETICA, 9f, BaseColor.BLACK);

                Font tableFont = FontFactory.GetFont(FontFactory.HELVETICA, 8.5f, BaseColor.BLACK);
                Font tableHeaderFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9.5f, BaseColor.BLACK);
                Font tableBoldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9.0f, BaseColor.BLACK);

                Font summaryLabelFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9.5f, BaseColor.BLACK);
                Font summaryAmountFont = FontFactory.GetFont(FontFactory.HELVETICA, 9.5f, BaseColor.BLACK);

                string projectName = string.Empty;
                if (projectId.HasValue)
                {
                    using (CompassEntities db = new CompassEntities())
                    {
                        var pd = db.tblProjects.Where(o => o.ProjectId == projectId.Value).FirstOrDefault();
                        if (pd != null)
                        {
                            var projectCityStateResult = db.STP_GetProjectCityState()
                                .Where(o => o.ProjectId == projectId.Value)
                                .FirstOrDefault();
                            if (projectCityStateResult != null)
                            {
                                projectName = projectCityStateResult.ProjectCityName;
                            }
                            else
                            {
                                projectName = pd.ProjectName;
                            }
                        }
                    }
                }

                // Title Section
                Paragraph title = new Paragraph();
                title.Alignment = Element.ALIGN_CENTER;
                title.SetLeading(0f, 1.15f);
                title.SpacingBefore = 0f;
                title.SpacingAfter = 12f;

                title.Add(new Chunk("Budget Status Report\n", titleFont));

                if (!string.IsNullOrEmpty(projectName))
                {
                    title.Add(new Chunk("Project Name: ", infoLabelFont));
                    title.Add(new Chunk(projectName + "\n", infoValueFont));
                }

                title.Add(new Chunk("Date Range: ", infoLabelFont));
                title.Add(new Chunk(fromDate + " ", infoValueFont));
                title.Add(new Chunk("To ", infoLabelFont));
                title.Add(new Chunk(toDate + "\n", infoValueFont));

                document.Add(title);

                // Create Table
                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;
                table.HorizontalAlignment = Element.ALIGN_CENTER;
                table.SpacingBefore = 5f;
                table.SpacingAfter = 15f;

                // Added Sr. No column
                table.SetWidths(new float[] { 0.45f, 3f, 1f, 1f, 1f });

                // Header Cells
                table.AddCell(CreateBorderPdfCell("Sr. No", tableHeaderFont, Element.ALIGN_CENTER));
                table.AddCell(CreateBorderPdfCell("Service Description", tableHeaderFont, Element.ALIGN_LEFT));
                table.AddCell(CreateBorderPdfCell("Unit Cost ($)", tableHeaderFont, Element.ALIGN_RIGHT));
                table.AddCell(CreateBorderPdfCell("Total Count", tableHeaderFont, Element.ALIGN_RIGHT));
                table.AddCell(CreateBorderPdfCell("Total Cost ($)", tableHeaderFont, Element.ALIGN_RIGHT));

                CultureInfo usCulture = CultureInfo.GetCultureInfo("en-US");

                // Data Rows
                int srNo = 1;

                foreach (var item in reportResult.ReportRows)
                {
                    table.AddCell(CreateBorderPdfCell(srNo.ToString(), tableFont, Element.ALIGN_CENTER));
                    table.AddCell(CreateBorderPdfCell(item.ServiceDescription ?? "", tableFont, Element.ALIGN_LEFT));
                    table.AddCell(CreateBorderPdfCell("$" + item.UnitCost.ToString("N2", usCulture), tableFont, Element.ALIGN_RIGHT));
                    table.AddCell(CreateBorderPdfCell(item.TotalCount.ToString("N0", usCulture), tableFont, Element.ALIGN_RIGHT));
                    table.AddCell(CreateBorderPdfCell("$" + item.TotalCost.ToString("N2", usCulture), tableFont, Element.ALIGN_RIGHT));

                    srNo++;
                }

                // Table Footer / Total Row
                decimal totalUnitCost = reportResult.ReportRows.Sum(o => o.UnitCost);
                int totalCount = reportResult.ReportRows.Sum(o => o.TotalCount);
                decimal totalCost = reportResult.ReportRows.Sum(o => o.TotalCost);

                table.AddCell(CreateBorderPdfCell("", tableBoldFont, Element.ALIGN_CENTER));
                table.AddCell(CreateBorderPdfCell("Total", tableBoldFont, Element.ALIGN_LEFT));
                table.AddCell(CreateBorderPdfCell("$" + totalUnitCost.ToString("N2", usCulture), tableBoldFont, Element.ALIGN_RIGHT));
                table.AddCell(CreateBorderPdfCell(totalCount.ToString("N0", usCulture), tableBoldFont, Element.ALIGN_RIGHT));
                table.AddCell(CreateBorderPdfCell("$" + totalCost.ToString("N2", usCulture), tableBoldFont, Element.ALIGN_RIGHT));

                document.Add(table);

                bool hasServiceLine = reportResult.ServiceLineReportRows != null && reportResult.ServiceLineReportRows.Any();

                if (hasServiceLine)
                {
                    // Under Table 1, render budget summary for Table 1
                    PdfPTable t1SummaryTable = new PdfPTable(2);
                    t1SummaryTable.WidthPercentage = 100;
                    t1SummaryTable.SetWidths(new float[] { 1f, 1f });
                    t1SummaryTable.SpacingBefore = 5f;
                    t1SummaryTable.SpacingAfter = 20f;
                    t1SummaryTable.KeepTogether = true;

                    t1SummaryTable.AddCell(CreateNoBorderPdfCell(
                        CreateSummaryPhrase("APPROVED PROJECT BUDGET", reportResult.ApprovedProjectBudget, usCulture, summaryLabelFont, summaryAmountFont),
                        Element.ALIGN_LEFT
                    ));

                    t1SummaryTable.AddCell(CreateNoBorderPdfCell(
                        CreateSummaryPhrase("BALANCE BUDGET", reportResult.BalanceBudget, usCulture, summaryLabelFont, summaryAmountFont),
                        Element.ALIGN_LEFT
                    ));

                    document.Add(t1SummaryTable);

                    // Render Table 2
                    PdfPTable table2 = new PdfPTable(5);
                    table2.WidthPercentage = 100;
                    table2.HorizontalAlignment = Element.ALIGN_CENTER;
                    table2.SpacingBefore = 5f;
                    table2.SpacingAfter = 15f;
                    table2.SetWidths(new float[] { 0.45f, 3f, 1f, 1f, 1f });

                    // Header Cells
                    table2.AddCell(CreateBorderPdfCell("Sr. No", tableHeaderFont, Element.ALIGN_CENTER));
                    table2.AddCell(CreateBorderPdfCell("Service Description", tableHeaderFont, Element.ALIGN_LEFT));
                    table2.AddCell(CreateBorderPdfCell("Unit Cost ($)", tableHeaderFont, Element.ALIGN_RIGHT));
                    table2.AddCell(CreateBorderPdfCell("Total Count", tableHeaderFont, Element.ALIGN_RIGHT));
                    table2.AddCell(CreateBorderPdfCell("Total Cost ($)", tableHeaderFont, Element.ALIGN_RIGHT));

                    // Data Rows Table 2
                    int slSrNo = 1;
                    foreach (var item in reportResult.ServiceLineReportRows)
                    {
                        table2.AddCell(CreateBorderPdfCell(slSrNo.ToString(), tableFont, Element.ALIGN_CENTER));
                        table2.AddCell(CreateBorderPdfCell(item.ServiceDescription ?? "", tableFont, Element.ALIGN_LEFT));
                        table2.AddCell(CreateBorderPdfCell("$" + item.UnitCost.ToString("N2", usCulture), tableFont, Element.ALIGN_RIGHT));
                        table2.AddCell(CreateBorderPdfCell(item.TotalCount.ToString("N0", usCulture), tableFont, Element.ALIGN_RIGHT));
                        table2.AddCell(CreateBorderPdfCell("$" + item.TotalCost.ToString("N2", usCulture), tableFont, Element.ALIGN_RIGHT));
                        slSrNo++;
                    }

                    document.Add(table2);

                    // Under Table 2, render Service Line budgets
                    PdfPTable t2SummaryTable = new PdfPTable(2);
                    t2SummaryTable.WidthPercentage = 100;
                    t2SummaryTable.SetWidths(new float[] { 1f, 1f });
                    t2SummaryTable.SpacingBefore = 5f;
                    t2SummaryTable.KeepTogether = true;

                    t2SummaryTable.AddCell(CreateNoBorderPdfCell(
                        CreateSummaryPhrase("SERVICE LINE BUDGET", reportResult.ServiceLineBudget, usCulture, summaryLabelFont, summaryAmountFont),
                        Element.ALIGN_LEFT
                    ));

                    t2SummaryTable.AddCell(CreateNoBorderPdfCell(
                        CreateSummaryPhrase("BALANCE SERVICE LINE BUDGET", reportResult.ServiceLineBalanceBudget, usCulture, summaryLabelFont, summaryAmountFont),
                        Element.ALIGN_LEFT
                    ));

                    document.Add(t2SummaryTable);
                }
                else
                {
                    // Bottom Summary Table
                    PdfPTable bottomTable = new PdfPTable(3);
                    bottomTable.WidthPercentage = 100;
                    bottomTable.SetWidths(new float[] { 1f, 1f, 1f });
                    bottomTable.SpacingBefore = 10f;
                    bottomTable.KeepTogether = true;

                    // Approved Project Budget
                    bottomTable.AddCell(CreateNoBorderPdfCell(
                        CreateSummaryPhrase(
                            "APPROVED PROJECT BUDGET",
                            reportResult.ApprovedProjectBudget,
                            usCulture,
                            summaryLabelFont,
                            summaryAmountFont
                        ),
                        Element.ALIGN_CENTER
                    ));

                    bottomTable.AddCell(CreateNoBorderPdfCell(
                        CreateSummaryPhrase(
                            "TOTAL USED BUDGET",
                            reportResult.UsedBudget,
                            usCulture,
                            summaryLabelFont,
                            summaryAmountFont
                        ),
                        Element.ALIGN_CENTER
                    ));

                    bottomTable.AddCell(CreateNoBorderPdfCell(
                        CreateSummaryPhrase(
                            "BALANCE BUDGET",
                            reportResult.BalanceBudget,
                            usCulture,
                            summaryLabelFont,
                            summaryAmountFont
                        ),
                        Element.ALIGN_CENTER
                    ));

                    document.Add(bottomTable);
                }

                document.Close();
                return ms.ToArray();
            }
        }

        private PdfPCell CreateBorderPdfCell(string text, Font font, int alignment)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text ?? "", font));
            cell.HorizontalAlignment = alignment;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;

            cell.PaddingTop = 4f;
            cell.PaddingBottom = 4f;
            cell.PaddingLeft = 5f;
            cell.PaddingRight = 5f;

            cell.MinimumHeight = 18f;
            cell.NoWrap = false;
            cell.BorderWidth = 0.7f;

            return cell;
        }

        private PdfPCell CreateNoBorderPdfCell(string text, Font font, int alignment)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text ?? "", font));
            cell.Border = Rectangle.NO_BORDER;
            cell.HorizontalAlignment = alignment;
            cell.VerticalAlignment = Element.ALIGN_TOP;

            cell.PaddingTop = 2f;
            cell.PaddingBottom = 2f;
            cell.PaddingLeft = 4f;
            cell.PaddingRight = 4f;

            cell.NoWrap = false;

            return cell;
        }

        private class TimeClockServiceConfig
        {
            public string ServiceCode { get; set; }
            public string ServiceDisplayName { get; set; }
            public decimal Cost { get; set; }
            public int DisplayOrder { get; set; }
        }

        private List<Model_InstalledMeterDetailPayRate> GetBeaumont21InstalledMetersDetails(
            long projectId,
            long? installerId,
            DateTime fromDate,
            DateTime toDate,
            SqlConnection con)
        {
            List<Model_InstalledMeterDetailPayRate> result =
                new List<Model_InstalledMeterDetailPayRate>();

            Dictionary<string, TimeClockServiceConfig> serviceConfig =
                GetTimeClockServiceConfig(projectId, con);

            string sqlQuery = @"
        SELECT
            pf.Id AS ProjectFieldDataId,
            CONVERT(varchar, pf.VisitDateTime, 101) AS CreatedOn,
            DATENAME(weekday, pf.VisitDateTime) AS DayName,
            FORMAT(pf.VisitDateTime, 'HH:mm') AS [Time],
            dm.InitialFieldName,
            dm.FieldValue
        FROM uploadedexceldata up
        INNER JOIN TblProjectFieldData pf
            ON up.Id = pf.fk_uploadedexceldata_id
        INNER JOIN tblProjectFieldDataMaster dm
            ON pf.Id = dm.TblProjectFieldData_Id
        WHERE up.FKProjectId = @ProjectId
          AND pf.VisitDateTime >= @FromDate
          AND pf.VisitDateTime < @ToDate
          AND (@InstallerId IS NULL OR pf.CreatedBy = @InstallerId)
          AND pf.Active = 1
          AND dm.Active = 1
        ORDER BY
            pf.VisitDateTime,
            pf.Id,
            dm.Id;
    ";

            DataTable dt = new DataTable();

            using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
            {
                cmd.CommandTimeout = 180;
                cmd.Parameters.AddWithValue("@ProjectId", projectId);
                cmd.Parameters.AddWithValue("@InstallerId", installerId.HasValue ? (object)installerId.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
            }

            if (dt.Rows.Count == 0)
            {
                return result;
            }

            var groupedRecords = dt.AsEnumerable()
                .GroupBy(r => Convert.ToInt64(r["ProjectFieldDataId"]));

            foreach (var recordGroup in groupedRecords)
            {
                DataRow firstRow = recordGroup.First();

                string createdOn = Convert.ToString(firstRow["CreatedOn"]);
                string dayName = Convert.ToString(firstRow["DayName"]);
                string time = Convert.ToString(firstRow["Time"]);

                string serviceType = "";              // SERVICE = Residential / Commercial
                string endPointReading = "";          // END POINT READING
                string fRegRead = "";                 // OLD FINAL REGISTER READ
                string fMeterRead = "";               // OLD METER FINAL READ
                string oldMeterSize = "";             // OLD METER SIZE

                // Lead Line Inspection logic fields
                string lcrInvestigation = "";         // LCR INVESTIGATION (Y/N)
                string citySidePipeType = "";         // CITY SIDE PIPE TYPE
                string customerSidePipeType = "";     // CUSTOMER SIDE PIPE TYPE
                string verified = "";                 // VERIFIED

                bool isWorkCompleted = false;

                List<int> selectedServiceIds = new List<int>();

                foreach (DataRow row in recordGroup)
                {
                    string fieldName = Convert.ToString(row["InitialFieldName"]);
                    string fieldValue = Convert.ToString(row["FieldValue"]);

                    if (string.IsNullOrWhiteSpace(fieldValue))
                    {
                        continue;
                    }

                    string normalizedFieldName = NormalizeBeaumontText(fieldName);
                    string normalizedFieldValue = NormalizeBeaumontText(fieldValue);

                    if (normalizedFieldName == "SERVICE")
                    {
                        serviceType = fieldValue.Trim();
                    }

                    if (normalizedFieldName == "SERVICES")
                    {
                        selectedServiceIds.AddRange(ParseBeaumontServiceIds(fieldValue));
                    }

                    if (normalizedFieldName == "WORK COMPLETED")
                    {
                        if (normalizedFieldValue == "TRUE"
                            || normalizedFieldValue == "YES"
                            || normalizedFieldValue == "Y"
                            || normalizedFieldValue == "1"
                            || normalizedFieldValue == "DONE")
                        {
                            isWorkCompleted = true;
                        }
                    }

                    if (normalizedFieldName == "END POINT READING")
                    {
                        if (string.IsNullOrWhiteSpace(endPointReading))
                        {
                            endPointReading = fieldValue.Trim();
                        }
                    }

                    if (normalizedFieldName == "OLD FINAL REGISTER READ")
                    {
                        if (string.IsNullOrWhiteSpace(fRegRead))
                        {
                            fRegRead = fieldValue.Trim();
                        }
                    }

                    if (normalizedFieldName == "OLD METER FINAL READ")
                    {
                        if (string.IsNullOrWhiteSpace(fMeterRead))
                        {
                            fMeterRead = fieldValue.Trim();
                        }
                    }

                    if (normalizedFieldName == "OLD METER SIZE")
                    {
                        if (string.IsNullOrWhiteSpace(oldMeterSize))
                        {
                            oldMeterSize = fieldValue.Trim();
                        }
                    }
                    if (normalizedFieldName == NormalizeBeaumontText("LCR INVESTIGATION (Y/N)")
                        || normalizedFieldName.Contains("LCR INVESTIGATION"))
                    {
                        if (string.IsNullOrWhiteSpace(lcrInvestigation))
                        {
                            lcrInvestigation = fieldValue.Trim();
                        }
                    }

                    if (normalizedFieldName == NormalizeBeaumontText("CITY SIDE PIPE TYPE")
                        || normalizedFieldName.Contains("CITY SIDE PIPE TYPE"))
                    {
                        if (string.IsNullOrWhiteSpace(citySidePipeType))
                        {
                            citySidePipeType = fieldValue.Trim();
                        }
                    }

                    if (normalizedFieldName == NormalizeBeaumontText("CUSTOMER SIDE PIPE TYPE")
                        || normalizedFieldName.Contains("CUSTOMER SIDE PIPE TYPE"))
                    {
                        if (string.IsNullOrWhiteSpace(customerSidePipeType))
                        {
                            customerSidePipeType = fieldValue.Trim();
                        }
                    }

                    if (normalizedFieldName == "VERIFIED"
                        || normalizedFieldName.Contains("VERIFIED"))
                    {
                        if (string.IsNullOrWhiteSpace(verified))
                        {
                            verified = fieldValue.Trim();
                        }
                    }
                }
                
                if (!isWorkCompleted)
                {
                    continue;
                }

                bool hasEndPointReading =
                    !string.IsNullOrWhiteSpace(endPointReading);

                bool hasFRegRead =
                    !string.IsNullOrWhiteSpace(fRegRead);

                bool hasFMeterRead =
                    !string.IsNullOrWhiteSpace(fMeterRead);

                bool isResidential =
                    NormalizeBeaumontText(serviceType).Contains("RESIDENTIAL");

                bool isCommercial =
                    NormalizeBeaumontText(serviceType).Contains("COMMERCIAL");

                const int BEAUMONT_SERVICE_RESET_METER_BOX = 7;
                const int BEAUMONT_SERVICE_METER_BOX_INSTALL_58_TO_1 = 10;
                const int BEAUMONT_SERVICE_METER_BOX_INSTALL_15_TO_2 = 61;
                const int BEAUMONT_SERVICE_REG_EXCH_STAND_ALONE = -1;

                bool hasResetMeterBox =
                    selectedServiceIds.Contains(BEAUMONT_SERVICE_RESET_METER_BOX);

                bool hasMeterBoxInstall58To1 =
                    selectedServiceIds.Contains(BEAUMONT_SERVICE_METER_BOX_INSTALL_58_TO_1);

                bool hasMeterBoxInstall15To2 =
                    selectedServiceIds.Contains(BEAUMONT_SERVICE_METER_BOX_INSTALL_15_TO_2);

                bool hasLineInspectionOutsideBox =
                    IsValidBeaumontLeadLineInspection(
                        lcrInvestigation,
                        citySidePipeType,
                        customerSidePipeType,
                        verified
                    );

                bool hasRegExchangeStandAlone =
                    selectedServiceIds.Contains(BEAUMONT_SERVICE_REG_EXCH_STAND_ALONE);

                if (hasEndPointReading && !hasFRegRead)
                {
                    AddBeaumont21RowByCode(
                        result,
                        serviceConfig,
                        projectId,
                        "ERT_MITIGATION",
                        createdOn,
                        dayName,
                        time
                    );
                }

                if (isResidential && hasFRegRead)
                {
                    AddBeaumont21RowByCode(
                        result,
                        serviceConfig,
                        projectId,
                        "REG_EX_RES",
                        createdOn,
                        dayName,
                        time
                    );
                }

                if (isCommercial && hasFRegRead)
                {
                    AddBeaumont21RowByCode(
                        result,
                        serviceConfig,
                        projectId,
                        "REG_EX_COM",
                        createdOn,
                        dayName,
                        time
                    );
                }

                if (hasRegExchangeStandAlone)
                {
                    AddBeaumont21RowByCode(
                        result,
                        serviceConfig,
                        projectId,
                        "REG_EX_STANDALONE",
                        createdOn,
                        dayName,
                        time
                    );
                }

                if (hasFMeterRead)
                {
                    if (BeaumontSizeMatches(oldMeterSize, "5/8")
                        || BeaumontSizeMatches(oldMeterSize, "5/8 x 3/4")
                        || BeaumontSizeMatches(oldMeterSize, "5/8\" x 3/4"))
                    {
                        AddBeaumont21RowByCode(
                            result,
                            serviceConfig,
                            projectId,
                            "METER_58",
                            createdOn,
                            dayName,
                            time
                        );
                    }
                    else if (BeaumontSizeMatches(oldMeterSize, "1"))
                    {
                        AddBeaumont21RowByCode(
                            result,
                            serviceConfig,
                            projectId,
                            "METER_1",
                            createdOn,
                            dayName,
                            time
                        );
                    }
                    else if (BeaumontSizeMatches(oldMeterSize, "1.5")
                             || BeaumontSizeMatches(oldMeterSize, "1 1/2")
                             || BeaumontSizeMatches(oldMeterSize, "1-1/2"))
                    {
                        AddBeaumont21RowByCode(
                            result,
                            serviceConfig,
                            projectId,
                            "METER_15",
                            createdOn,
                            dayName,
                            time
                        );
                    }
                    else if (BeaumontSizeMatches(oldMeterSize, "2"))
                    {
                        AddBeaumont21RowByCode(
                            result,
                            serviceConfig,
                            projectId,
                            "METER_2",
                            createdOn,
                            dayName,
                            time
                        );
                    }
                }

                if (hasLineInspectionOutsideBox)
                {
                    AddBeaumont21RowByCode(
                        result,
                        serviceConfig,
                        projectId,
                        "LEAD_LINE",
                        createdOn,
                        dayName,
                        time
                    );
                }

                if (hasResetMeterBox)
                {
                    AddBeaumont21RowByCode(
                        result,
                        serviceConfig,
                        projectId,
                        "BOX_REPLACEMENT",
                        createdOn,
                        dayName,
                        time
                    );
                }

                if (hasMeterBoxInstall58To1)
                {
                    AddBeaumont21RowByCode(
                        result,
                        serviceConfig,
                        projectId,
                        "BOX_REPLACEMENT",
                        createdOn,
                        dayName,
                        time
                    );
                }

                if (hasMeterBoxInstall15To2)
                {
                    AddBeaumont21RowByCode(
                        result,
                        serviceConfig,
                        projectId,
                        "BOX_REPLACEMENT",
                        createdOn,
                        dayName,
                        time
                    );
                }
            }

            return result;
        }

        private string NormalizeBeaumontText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "";
            }

            string normalized = value
                .Trim()
                .ToUpper()
                .Replace("\"", "")
                .Replace("'", "")
                .Replace("&", " AND ")
                .Replace("/", " / ")
                .Replace("_", " ")
                .Replace("-", " ");

            while (normalized.Contains("  "))
            {
                normalized = normalized.Replace("  ", " ");
            }

            return normalized.Trim();
        }

        private bool BeaumontSizeMatches(string actualSize, string expectedSize)
        {
            string actual = NormalizeBeaumontSize(actualSize);
            string expected = NormalizeBeaumontSize(expectedSize);

            if (string.IsNullOrWhiteSpace(actual) || string.IsNullOrWhiteSpace(expected))
            {
                return false;
            }

            return actual == expected || actual.Contains(expected);
        }

        private string NormalizeBeaumontSize(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "";
            }

            return value
                .Trim()
                .ToUpper()
                .Replace("\"", "")
                .Replace("'", "")
                .Replace(" ", "")
                .Replace("-", "")
                .Replace("X", "x")
                .Replace("INCH", "")
                .Replace("IN", "");
        }

        private Dictionary<string, TimeClockServiceConfig> GetTimeClockServiceConfig(
            long projectId,
            SqlConnection con)
        {
            Dictionary<string, TimeClockServiceConfig> config =
                GetDefaultBeaumont21ServiceConfig();

            string sql = @"
        SELECT
            ServiceCode,
            ServiceDisplayName,
            Cost,
            DisplayOrder
        FROM TimeClockServiceMaster
        WHERE ProjectId = @ProjectId
          AND IsActive = 1
        ORDER BY DisplayOrder;
    ";

            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@ProjectId", projectId);

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    string serviceCode = Convert.ToString(row["ServiceCode"]).Trim();

                    if (string.IsNullOrWhiteSpace(serviceCode))
                    {
                        continue;
                    }

                    config[serviceCode] = new TimeClockServiceConfig
                    {
                        ServiceCode = serviceCode,
                        ServiceDisplayName = Convert.ToString(row["ServiceDisplayName"]).Trim(),
                        Cost = Convert.ToDecimal(row["Cost"]),
                        DisplayOrder = Convert.ToInt32(row["DisplayOrder"])
                    };
                }
            }

            return config;
        }

        private Dictionary<string, TimeClockServiceConfig> GetDefaultBeaumont21ServiceConfig()
        {
            return new Dictionary<string, TimeClockServiceConfig>
            {
                {
                    "ERT_MITIGATION",
                    new TimeClockServiceConfig
                    {
                        ServiceCode = "ERT_MITIGATION",
                        ServiceDisplayName = "ERT Mitigation",
                        Cost = 9.00m,
                        DisplayOrder = 1
                    }
                },
                {
                    "METER_58",
                    new TimeClockServiceConfig
                    {
                        ServiceCode = "METER_58",
                        ServiceDisplayName = "5/8\" x 3/4 Meter Change Out & ERT Installation",
                        Cost = 15.00m,
                        DisplayOrder = 2
                    }
                },
                {
                    "REG_EX_RES",
                    new TimeClockServiceConfig
                    {
                        ServiceCode = "REG_EX_RES",
                        ServiceDisplayName = "Register Exchange with ERT Mitigation (Residential)",
                        Cost = 12.00m,
                        DisplayOrder = 3
                    }
                },
                {
                    "REG_EX_COM",
                    new TimeClockServiceConfig
                    {
                        ServiceCode = "REG_EX_COM",
                        ServiceDisplayName = "Register Exchange with ERT Mitigation (Commercial)",
                        Cost = 18.00m,
                        DisplayOrder = 4
                    }
                },
                {
                    "REG_EX_STANDALONE",
                    new TimeClockServiceConfig
                    {
                        ServiceCode = "REG_EX_STANDALONE",
                        ServiceDisplayName = "Register Exchange (Stand Alone)",
                        Cost = 8.00m,
                        DisplayOrder = 5
                    }
                },
                {
                    "LEAD_LINE",
                    new TimeClockServiceConfig
                    {
                        ServiceCode = "LEAD_LINE",
                        ServiceDisplayName = "Lead Line Inspection (Outside the Box) with ERT Mitigation",
                        Cost = 12.00m,
                        DisplayOrder = 6
                    }
                },
                {
                    "METER_1",
                    new TimeClockServiceConfig
                    {
                        ServiceCode = "METER_1",
                        ServiceDisplayName = "1\" Meter Change Out & ERT Installation",
                        Cost = 15.00m,
                        DisplayOrder = 7
                    }
                },
                {
                    "METER_15",
                    new TimeClockServiceConfig
                    {
                        ServiceCode = "METER_15",
                        ServiceDisplayName = "1.5\" Meter Change Out & ERT Installation",
                        Cost = 65.00m,
                        DisplayOrder = 8
                    }
                },
                {
                    "METER_2",
                    new TimeClockServiceConfig
                    {
                        ServiceCode = "METER_2",
                        ServiceDisplayName = "2\" Meter Change Out & ERT Installation",
                        Cost = 70.00m,
                        DisplayOrder = 9
                    }
                },
                {
                    "BOX_REPLACEMENT",
                    new TimeClockServiceConfig
                    {
                        ServiceCode = "BOX_REPLACEMENT",
                        ServiceDisplayName = "Residential Box Replacement or Adjustment",
                        Cost = 15.75m,
                        DisplayOrder = 10
                    }
                }
            };
        }

        private void AddBeaumont21RowByCode(
            List<Model_InstalledMeterDetailPayRate> result,
            Dictionary<string, TimeClockServiceConfig> serviceConfig,
            long projectId,
            string serviceCode,
            string createdOn,
            string dayName,
            string time)
        {
            if (!serviceConfig.ContainsKey(serviceCode))
            {
                log.Info("Beaumont 2.1 service config missing for ServiceCode: " + serviceCode);
                return;
            }

            TimeClockServiceConfig config = serviceConfig[serviceCode];

            result.Add(new Model_InstalledMeterDetailPayRate
            {
                ProjectId = projectId,
                MeterSize = "",
                MeterType = "",
                MeterDisplayName = config.ServiceDisplayName,
                CreatedOn = createdOn,
                DayName = dayName,
                Time = time,
                Pay = config.Cost.ToString("0.00")
            });
        }

        private List<int> ParseBeaumontServiceIds(string value)
        {
            List<int> serviceIds = new List<int>();

            if (string.IsNullOrWhiteSpace(value))
            {
                return serviceIds;
            }

            string[] parts = value.Split('|');

            foreach (string part in parts)
            {
                int serviceId;

                if (int.TryParse(part.Trim(), out serviceId))
                {
                    serviceIds.Add(serviceId);
                }
            }

            return serviceIds;
        }

        private bool IsValidBeaumontLeadLineInspection(
            string lcrInvestigation,
            string citySidePipeType,
            string customerSidePipeType,
            string verified)
        {
            bool isLcrYes =
                NormalizeBeaumontText(lcrInvestigation) == "Y"
                || NormalizeBeaumontText(lcrInvestigation) == "YES";

            bool isVerifiedBoth =
                NormalizeBeaumontText(verified) == "BOTH";

            bool isValidCitySidePipe =
                IsAllowedBeaumontCitySidePipeType(citySidePipeType);

            bool isValidCustomerSidePipe =
                IsAllowedBeaumontCustomerSidePipeType(customerSidePipeType);

            return isLcrYes
                && isValidCitySidePipe
                && isValidCustomerSidePipe
                && isVerifiedBoth;
        }

        private bool IsAllowedBeaumontCitySidePipeType(string pipeType)
        {
            string value = NormalizeBeaumontText(pipeType);

            return value == "COPPER"
                || value == "POLY"
                || value == "GALVANIZED"
                || value == "GALVANIZES"
                || value == "GALVANIZE"
                || value == "CAST IRON"
                || value == "PEX";
        }

        private bool IsAllowedBeaumontCustomerSidePipeType(string pipeType)
        {
            string value = NormalizeBeaumontText(pipeType);

            return value == "COPPER"
                || value == "POLY"
                || value == "GALVANIZED"
                || value == "GALVANIZES"
                || value == "GALVANIZE"
                || value == "CAST IRON"
                || value == "PEX"
                || value == "PVC"
                || value == "LEAD";
        }

        protected virtual void Dispose(bool disposing)
        {
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private Phrase CreateSummaryPhrase(
    string label,
    decimal amount,
    CultureInfo cultureInfo,
    Font labelFont,
    Font amountFont)
        {
            Phrase phrase = new Phrase();

            phrase.Add(new Chunk(label + "\n", labelFont));

            // Amount and dollar sign both normal/unbold
            phrase.Add(new Chunk("$" + amount.ToString("N2", cultureInfo), amountFont));

            return phrase;
        }

        private PdfPCell CreateNoBorderPdfCell(Phrase phrase, int alignment)
        {
            PdfPCell cell = new PdfPCell(phrase);
            cell.Border = Rectangle.NO_BORDER;
            cell.HorizontalAlignment = alignment;
            cell.VerticalAlignment = Element.ALIGN_TOP;

            cell.PaddingTop = 2f;
            cell.PaddingBottom = 2f;
            cell.PaddingLeft = 4f;
            cell.PaddingRight = 4f;

            cell.NoWrap = false;

            return cell;
        }
    }
}