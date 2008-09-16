<%@ Control Language="C#" AutoEventWireup="true" CodeFile="comment-editor.ascx.cs"
    Inherits="KMBlog.CommentEditor" ClassName="CtrlCommentEditor" %>
<%@ Register TagPrefix="tinymce" Namespace="Moxiecode.TinyMCE.Web" Assembly="Moxiecode.TinyMCE" %>
<fieldset id="comment-editor">
    <ol>
        <li>
            <asp:Label ID="labelName" AssociatedControlID="txtName" runat="server" Text="Name" />
            <asp:TextBox ID="txtName" runat="server" TabIndex="10" />
        </li>
        <li>
            <asp:Label ID="labelWebsite" AssociatedControlID="txtURL" runat="server" Text="Website" />
            <asp:TextBox ID="txtURL" runat="server" Text="" TabIndex="20" />
        </li>
    </ol>
    <asp:Label ID="labelText" AssociatedControlID="comment_text" runat="server" Text="Text" />
    <br />
    <tinymce:TextArea ID="comment_text" theme="advanced" plugins="safari" theme_advanced_buttons1="bold,italic,underline,fontsizeselect,bullist,numlist,undo,redo,link,unlink,image,cleanup,formatselect"
        theme_advanced_buttons2="" theme_advanced_buttons3="" theme_advanced_buttons4=""
        theme_advanced_toolbar_location="top" theme_advanced_toolbar_align="left" theme_advanced_path_location="bottom"
        theme_advanced_resizing="true" theme_advanced_blockformats="blockquote,code,samp"
        valid_elements="a[class|href|id|lang|name|onclick|rel|style|title],b,i,strong,em,ol,ul,li,blockquote,cite,img,div,span,font,p,code,samp"
        runat="server" InstallPath="/keymapper/tiny_mce" init_instance_callback="setTabIndex" />
    <br />
</fieldset>

<script type="text/javascript">
    function setTabIndex() {

        $('iframe').attr("tabIndex", "30");

    }
</script>

