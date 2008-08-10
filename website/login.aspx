<%@ Page Language="C#" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="login"
    MasterPageFile="blog/KMBlog.Master" Title="Administrative Login" %>

<asp:Content ID="default" ContentPlaceHolderID="body" runat="server">
    You can use the username: demo and password: demo to see the admin page. You won't
    be able to save any changes.<br />
    <div class="loginform">
        <asp:Login ID="KMLogin" runat="server" OnAuthenticate="Login1_Authenticate" DestinationPageUrl="~/blog/admin/admin.aspx">
            <TextBoxStyle CssClass="input_textbox" />
        </asp:Login>
    </div>
</asp:Content>
