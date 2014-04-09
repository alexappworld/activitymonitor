<%@ Page Language="C#" AutoEventWireup="true" CodeFile="activitylog.aspx.cs" Inherits="MyCalender" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Activity Log</title>
    <link rel="shortcut icon" href="images/favicon.ico" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Calendar ID="Calendar1" runat="server" BackColor="White" 
            BorderColor="Black" DayNameFormat="Shortest" Font-Names="Century Schoolbook"
            Font-Size="Medium" ForeColor="Black" Height="220px" 
            NextPrevFormat="ShortMonth" Width="400px" ondayrender="Calendar1_DayRender" 
            onselectionchanged="Calendar1_SelectionChanged" EnableViewState="True" 
            onvisiblemonthchanged="Calendar1_VisibleMonthChanged" 
            ViewStateMode="Enabled" onload="Calendar1_Load">
            <DayHeaderStyle BackColor="#CCCCCC" Font-Bold="True" Font-Size="7pt" 
                ForeColor="#333333" Height="10pt" />
            <DayStyle Width="14%" />
            <NextPrevStyle Font-Size="8pt" ForeColor="White" />
            <OtherMonthDayStyle ForeColor="#999999" />
            <SelectedDayStyle BackColor="#CC3333" ForeColor="White" />
            <TitleStyle BackColor="#666666" Font-Bold="True" Font-Size="13pt" 
                ForeColor="White" Height="14pt" />
            <TodayDayStyle BackColor="#FFD5AA" BorderColor="White" />
            
        </asp:Calendar>
    </div>
    </form>
</body>
</html>
