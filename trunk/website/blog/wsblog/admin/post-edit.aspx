<%@ Page Language="C#" MasterPageFile="~/KMBlog.master" AutoEventWireup="true" CodeFile="post-edit.aspx.cs" Inherits="EditPost" 
Title="Post Editor" ValidateRequest="false"%>

<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <h3 id="header">
        Blog Post Editor</h3>
    <form id="form1" runat="server" method="post" action="post-edit.aspx">
        <div id="editarea">
            <div id="edit_title">
                Title:
                <asp:TextBox ID="blogtitle" runat="server" Width="30em">
                </asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="blogtitle" runat="server" ErrorMessage="Title can't be blank"
                    CssClass="errortext" />
            </div>
            <div id="edit_body">
                <textarea id="blogpost" cols="60" rows="20" runat="server" style="width: 80%;"></textarea>
            </div>
            <div id="controls">
                <input type="hidden" id="hiddenPostID" runat="server" />
                <asp:Button ID="submitpost" Text="Save" CausesValidation="true" runat="server" />
                <asp:Button ID="canceledit" Text="Cancel" CausesValidation="false" runat="server"
                    OnClick="CancelEdit" />
              
                <span id="post_timestamp">
                    Datestamp:
                    <asp:TextBox ID="postday" runat="server" Width="2em">
                    </asp:TextBox>
                    <asp:DropDownList ID="postmonth" runat="server" Width="10em">
                    </asp:DropDownList>
                    <asp:TextBox ID="postyear" runat="server" Width="4em">
                    </asp:TextBox>
                    <asp:Label ID="date_error" runat="server" CssClass="dateerrortext"></asp:Label>
                </span>
            </div>
            <asp:Label ID="resultlabel" runat="server"></asp:Label>
        </div>
    </form>
</asp:Content>

