<%@ Page Language="C#" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="Login"
    MasterPageFile="blog/KMBlog.Master" Title="Administrative Login" %>

<asp:Content ID="default" ContentPlaceHolderID="body" runat="server">
    You can use the username: demo and password: demo to see the admin page. You won't
    be able to save any changes.<br />
    <a href="blog/default.aspx">Back to blog</a>
        <asp:Login ID="KMLogin" runat="server" OnAuthenticate="Login1_Authenticate" 
            DestinationPageUrl="~/blog/admin/admin.aspx" BackColor="#EEEEEE" 
            DisplayRememberMe="False" PasswordLabelText="Password" 
            UserNameLabelText="User Name" CssClass="loginform" >
            <TextBoxStyle CssClass="input_textbox" Width="10em" />
            <LabelStyle CssClass="loginLabel" HorizontalAlign="Left" />
            <TitleTextStyle CssClass="loginTitle" />
        </asp:Login>
</asp:Content>
