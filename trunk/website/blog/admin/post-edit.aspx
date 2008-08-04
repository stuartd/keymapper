<%@ Page Language="C#" MasterPageFile="~/KMBlogAdmin.Master" AutoEventWireup="true" CodeBehind="post-edit.aspx.cs"
    Inherits="KMBlog.post_edit" Title="Post Editor" ValidateRequest="false" %>

<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <h3 id="header">
        Blog Post Editor</h3>
    <a href="admin.aspx">Back to Admin page</a>
    <div id="editarea" runat="server">
        <div id="edit_title">
            Title:
            <asp:TextBox ID="posttitle" runat="server" Width="30em" TabIndex="1"></asp:TextBox>
            <asp:RequiredFieldValidator ControlToValidate="posttitle" runat="server" ErrorMessage="The title can't be blank" />
        </div>
        <div id="edit_categories">
            <div class="subheader">
                Categories</div>
            <asp:CheckBoxList runat="server" ID="CatList">
            </asp:CheckBoxList>
        </div>
        <div id="edit_body">
            <asp:TextBox runat="server" TextMode="MultiLine" ID="blogpost" Width="79.5%" Height="150px"
                TabIndex="2"></asp:TextBox>
        </div>
        <div id="controls">
            <div id="slugdiv" runat="server">
                <asp:Label AssociatedControlID="postslug" ID="sluglabel" runat="server">Slug</asp:Label>
                <asp:TextBox runat="server" ID="postslug" ReadOnly="True"></asp:TextBox>
                <asp:Button ID="btnRegenerateSlug" runat="server" Text="Regenerate" OnClick="RegenerateSlug" />
            </div>
            <div id="post_timestamp">
                Datestamp:
                <asp:TextBox ID="postday" runat="server" Width="2em" TabIndex="5">
                </asp:TextBox>
                <asp:DropDownList ID="postmonth" runat="server" Width="10em" TabIndex="6">
                </asp:DropDownList>
                <asp:TextBox ID="postyear" runat="server" Width="4em" TabIndex="7">
                </asp:TextBox>
                <asp:Label ID="date_error" runat="server" CssClass="dateerrortext"></asp:Label>
                <br />
            </div>
            <input type="hidden" id="hiddenPostID" runat="server" />
            <asp:Button ID="btnPublishPost" Text="Publish" CommandName="Publish" CausesValidation="true"
                runat="server" OnCommand="SavePost" TabIndex="3" />
            <asp:Button ID="btnSavePost" CommandName="Draft" Text="Save As Draft" CausesValidation="true"
                runat="server" OnCommand="SavePost" TabIndex="4" />
            <asp:Button ID="btnCancelEdit" Text="Cancel" CausesValidation="false" runat="server"
                OnClick="CancelEdit" />
        </div>
        <asp:Label ID="resultlabel" runat="server"></asp:Label>
    </div>
</asp:Content>
