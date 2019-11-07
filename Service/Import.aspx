<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Import.aspx.cs" Inherits="_Import" %>

<%@ Import Namespace="dCForm" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>import</title>
</head>
<body>
    <form id="form1" runat="server">
    import
    <asp:ScriptManager runat="server" />
    <asp:Timer runat="server" ID="UpdateTimer" Interval="10000" OnTick="UpdateTimer_Tick" />
    <asp:UpdatePanel runat="server" ID="TimedPanel" UpdateMode="Conditional">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="UpdateTimer" EventName="Tick" />
        </Triggers>
        <ContentTemplate>
            <asp:BulletedList reversed Width="40%" Style="float: left;" ID="BulletedList1" runat="server"
                DisplayMode="HyperLink" DataSource="<%# ImporterController.GetImporterLogByFolder().Where(m=>string.IsNullOrWhiteSpace(m.ExceptionMessage)).OrderByDescending(m=>m.LightDoc.DocSubmitDate)  %>"
                DataTextField="DocTitleLi" DataValueField="DocSrc" BulletStyle="Numbered" />
            <asp:BulletedList reversed Width="40%" Style="float: right;" ID="BulletedList2" runat="server"
                DisplayMode="HyperLink" DataSource="<%# ImporterController.GetImporterLogByFolder().Where(m=>!string.IsNullOrWhiteSpace(m.ExceptionMessage)).OrderByDescending(m=>m.LightDoc.DocSubmitDate)  %>"
                DataTextField="DocTitleLi" DataValueField="DocSrc" BulletStyle="Numbered" />
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
