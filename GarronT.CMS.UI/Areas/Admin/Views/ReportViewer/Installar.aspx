<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Installar.aspx.cs" Inherits="GarronT.CMS.UI.Areas.Admin.Views.ReportViewer.Installar" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>


<!DOCTYPE html>

<html xmlns="https://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
         <div id="Div_Report" runat="server" class="Report">
                    
                  <rsweb:ReportViewer ID="ReportViewer1" CssClass="col-sm-8"  runat="server" Height="400px" Width="100%"></rsweb:ReportViewer>
              <div class="col-sm-2">

                    </div>
              </div>
    </form>
</body>
</html>
