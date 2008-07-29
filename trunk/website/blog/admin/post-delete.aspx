<%@ Page Language="C#" MasterPageFile="~/KMBlog.Master" 
AutoEventWireup="true" CodeBehind="post-delete.aspx.cs" 
Inherits="KMBlog.post_delete" Title="Confirm Deletion" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <form id="form1" runat="server">
    <p>
        Are you sure you want to delete the post?<br />
        <asp:Button ID="btnYes" 
            runat="server" onclick="DeletePost" Text="Yes" Width="5em"/>
        <asp:Button ID="btnNo" runat="server" onclick="CancelDelete" Text="No" Width="5em"/>
    </p>
    </form>
</asp:Content>
