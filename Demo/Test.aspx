<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Test.aspx.cs" Inherits="Demo.Test" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Button ID="btnAdd" runat="server" Text="Add" OnClick="btnAdd_Click" Width="77px" />
        <asp:Button ID="btnUpdate" runat="server" OnClick="btnUpdate_Click" Text="Update" />
        <asp:Button ID="btnUpdateNull" runat="server" Text="UpdateNull" OnClick="btnUpdateNull_Click" />
        <asp:Button ID="btnDelete" runat="server" Text="Delete" OnClick="btnDelete_Click" />
        <asp:Button ID="btnGet" runat="server" Text="Get" OnClick="btnGet_Click" Width="84px" />
        <asp:Button ID="btnGetList" runat="server" Text="GetList" Width="84px" OnClick="btnGetList_Click" />
        <br /><br />
        <asp:Literal ID="litPerson" runat="server"></asp:Literal>
    </form>
</body>
</html>
