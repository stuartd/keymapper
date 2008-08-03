<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="KMBlog.login"
    MasterPageFile="~/KMBlog.Master" Title="Administrative Login" %>

<asp:Content ID="default" ContentPlaceHolderID="body" runat="server">
    <div id="maindiv">
        <h1 id="header">
            Key Mapper Blog Admin Login</h1>
            You can use the username: demo and password: demo to see the admin page. You won't be able to save any changes.<br />
        <div class="loginform">
            <asp:Login ID="KMLogin" runat="server" OnAuthenticate="Login1_Authenticate">
                <TextBoxStyle CssClass="input_textbox" />
            </asp:Login>
        </div>
    </div>
</asp:Content>
