<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="KMBlog.login"
    MasterPageFile="~/KMBlog.Master" Title="Administrative Login"%>

<asp:Content ID="default" ContentPlaceHolderID="body" runat="server">
    <form id="form1" runat="server">
    <div id="maindiv">
        <h1 id="header"><a href="/">
            Key Mapper Blog Admin Login</a></h1>
        <asp:Login ID="Login1" runat="server" BorderStyle="Solid" 
            TitleText="Please Log In" onauthenticate="Login1_Authenticate">
        </asp:Login>
        </div>
    </form>
</asp:Content>
