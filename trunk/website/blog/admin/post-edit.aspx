<%@ Page Language="C#" MasterPageFile="../KMBlogAdmin.Master" AutoEventWireup="true"
    CodeFile="post-edit.aspx.cs" Inherits="KMBlog.PostEdit" Title="Post Editor"
    ValidateRequest="false" %>

<%@ Register TagPrefix="tinymce" Namespace="Moxiecode.TinyMCE.Web" Assembly="Moxiecode.TinyMCE" %>

<asp:Content ID="editPageBody" ContentPlaceHolderID="body" runat="server">
    <div id="editarea" runat="server">
        <div id="edit_title">
            Title
            <asp:TextBox ID="posttitle" runat="server" Width="30em" TabIndex="10"></asp:TextBox>
            <asp:RequiredFieldValidator ControlToValidate="posttitle" ID="titleValidator" runat="server" ErrorMessage="The title can't be blank" />
        </div>
        <div id="edit_post_categories">
            <div class="subheader">
                Categories</div>
            <asp:CheckBoxList runat="server" ID="CatList">
            </asp:CheckBoxList>
        </div>
        <div id="edit_body">
               
           <tinymce:TextArea ID="blogpost" Width="79.5%" theme="advanced" 
           plugins="safari" 
           theme_advanced_buttons1="bold,italic,underline,fontsizeselect,bullist,numlist,undo,redo,link,unlink,image,cleanup,code,formatselect"
                theme_advanced_buttons2="" theme_advanced_buttons3="" theme_advanced_buttons4=""
                theme_advanced_toolbar_location="top" theme_advanced_toolbar_align="left" theme_advanced_path_location="bottom"
                theme_advanced_resizing="true" theme_advanced_blockformats="blockquote,code,samp"  TabIndex="20" 
                runat="server" InstallPath="/keymapper/tiny_mce" init_instance_callback="setTabIndex"  />
                
        </div>

            <script type="text/javascript">
                function setTabIndex() {

                    $('iframe').attr("tabIndex", "20");
                    
            }
        
        </script>
        <div id="controls">
            <div id="slugContainer" runat="server">
                <a id="editslug" href="#">Edit Slug</a>
                <div id="slugdiv">
                    <asp:Label AssociatedControlID="postslug" ID="sluglabel" runat="server">Post slug</asp:Label>
                    <asp:TextBox runat="server" ID="postslug" Width="50%"></asp:TextBox>
                    <asp:CustomValidator runat="server" ErrorMessage="That slug is already used: slugs must be unique."
                        ControlToValidate="postslug" OnServerValidate="ValidateSlug" ID="slugValidator"></asp:CustomValidator>
                    <p>
                        The post slug will be regenerated automatically from the title if left blank. It
                        can only contain letters, numbers, and hyphens.</p>
                </div>
            </div>
            <div id="post_timestamp">
                Datestamp:<asp:TextBox ID="postday" runat="server" Width="2em" TabIndex="50">
                </asp:TextBox>
                <asp:DropDownList ID="postmonth" runat="server" Width="8em" TabIndex="60">
                </asp:DropDownList>
                <asp:TextBox ID="postyear" runat="server" Width="4em" TabIndex="70">
                </asp:TextBox>
                <asp:Label ID="date_error" runat="server" CssClass="dateerrortext"></asp:Label>
                <br />
            </div>
            <input type="hidden" id="hiddenPostId" runat="server" />
            <asp:Button ID="btnPublishPost" Text="Publish" CommandName="Publish" CausesValidation="true"
                runat="server" OnCommand="SavePost" TabIndex="30" />
            <asp:Button ID="btnSavePost" CommandName="Draft" Text="Save As Draft" CausesValidation="true"
                runat="server" OnCommand="SavePost" TabIndex="40" />
            <asp:Button ID="btnCancelEdit" Text="Cancel" CausesValidation="false" runat="server"
                OnClick="CancelEdit" />
        </div>
        <asp:Label ID="resultlabel" runat="server"></asp:Label>
    </div>
</asp:Content>
