<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="KMBlog.login"
    MasterPageFile="~/KMBlog.Master" Title="Administrative Login" %>

<asp:Content ID="default" ContentPlaceHolderID="body" runat="server">
    <form id="form1" runat="server">
        <div id="maindiv">
            <h1 id="header">
                <a href="/">Key Mapper Blog Admin Login</a></h1>
            <p>
                Please Log In</p>
            <asp:Label ID="lblUserName" runat="server" AssociatedControlID="txtUserName" Text="User Name:"></asp:Label>
            <asp:TextBox ID="txtUserName" runat="server"></asp:TextBox>
            <br />
            <asp:Label ID="lblPassword" runat="server" AssociatedControlID="txtPassword" Text="Password:" ></asp:Label>
            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox>
            <br />
            <asp:Button ID="btnLogin" runat="server" Text="Log In" />
        </div>
    </form>
</asp:Content>
