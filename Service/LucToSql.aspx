<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LucToSql.aspx.cs" Inherits="_LucToSql" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>import</title>
</head>
<body>
    <form id="form1" runat="server">
        <%# GetType().Name %>
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <asp:Timer runat="server" ID="UpdateTimer" Interval="5000" OnTick="UpdateTimer_Tick" />
        <asp:UpdatePanel runat="server" ID="TimedPanel" UpdateMode="Conditional">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="UpdateTimer" EventName="Tick" />
            </Triggers>
            <ContentTemplate>
                <asp:BulletedList reversed ID="BulletedList1" runat="server" DisplayMode="HyperLink" DataSource="<%# List_ImporterLightDoc.ToArray() %>" DataTextField="DocTitle" DataValueField="DocSrc" BulletStyle="Numbered" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
