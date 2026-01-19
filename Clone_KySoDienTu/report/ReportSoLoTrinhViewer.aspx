<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportSoLoTrinhViewer.aspx.cs" Inherits="Clone_KySoDienTu.Report.ReportSoLoTrinhViewer" %>

<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.4000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" Namespace="CrystalDecisions.Web" TagPrefix="CR" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form3" runat="server">
        <div>
            <CR:CrystalReportViewer ID="CrystalReportViewer3" runat="server" AutoDataBind="true" />
        </div>
    </form>
</body>
</html>
