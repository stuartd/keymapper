<%@ Page Language="C#" MasterPageFile="~/KMBlog.Master" AutoEventWireup="true" CodeBehind="admin.aspx.cs"
    Inherits="KMBlog.admin.admin" Title="Key Mapper Blog Admin" ValidateRequest ="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        @import url('../kmblog.css');
    </style>

    <script language="javascript" type="text/javascript" src="../scripts/tiny_mce/tiny_mce.js"></script>

    <script language="javascript" type="text/javascript">
tinyMCE.init({
    mode : "textareas",
    theme : "simple",
auto_reset_designmode : true 
});
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <form id="form1" runat="server">
        <asp:TextBox ID="MyTextBox" runat="server" Rows="10" TextMode="MultiLine" Width="557px"
            Height="267px"></asp:TextBox>
        <asp:Button ID="Submit" runat="server" UseSubmitBehavior="false" Text="Submit" />
                
    </form>
</asp:Content>
