<%@ Page Language="C#" MasterPageFile="~/KMBlog.master" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="login" Title="Untitled Page" %>

<asp:Content ID="default" ContentPlaceHolderID="body" runat="server">
    <form id="form1" runat="server">
        <div id="maindiv">
            <h1 id="header">
                <a href="/">Key Mapper Blog Admin Login</a></h1>
        </div>
        <asp:Login ID="KMLogin" runat="server" onauthenticate="Login1_Authenticate">
        </asp:Login>
    </form>
</asp:Content>