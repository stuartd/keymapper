<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="KMBlog.login"
    MasterPageFile="~/KMBlog.Master" Title="Administrative Login" %>

<asp:Content ID="default" ContentPlaceHolderID="body" runat="server">
    <div id="maindiv">
        <h1 id="header">
            <a href="/">Key Mapper Blog Admin Login</a></h1>
        <form id="form1" runat="server" class="loginform">
            <asp:Login ID="KMLogin" runat="server" OnAuthenticate="Login1_Authenticate">
                <TextBoxStyle CssClass="input_textbox" />
            </asp:Login>
        </form>
    </div>
</asp:Content>
