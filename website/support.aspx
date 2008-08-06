<%@ Page Language="C#" Title="Key Mapper Support" MasterPageFile="keymapper.master"
    AutoEventWireup="true" CodeFile="support.aspx.cs" Inherits="KeyMapperSupport" %>

<asp:Content ID="default" ContentPlaceHolderID="body" runat="server">
    <form id="form1" runat="server">
    <div>
        <ul class="basiclist">
            <li>Drag a key off the keyboard to disable it.</li>
            <li>To remap a key, double-click it.</li>
            <li>To delete a key mapping or re-enable a key, drag it off the keyboard.</li>
            <li>You need to log off and on again for the remapping or disabling to take effect</li>
            <li>You can capture the key you want to map or choose it from a list by choosing Create
                New Mapping from the Mappings menu.</li>
            <li>If the key you want to map and the one you want to map it to are both visible, drag
                and drop the action key onto the target key. Otherwise, choose "Create new Mapping"
                from them Mappings menu and choose from the key lists or use key capture.</li>
            <li>You need to log off and on again for the mappings to take effect.</li>
            <li>If you're using Windows 2000 or you want to create mappings that apply to all users,
                you need to restart your PC before the mappings take effect</li>
        </ul>
        <h3>
            <a href="mailto://key%6dappersupport@%67%6dail%2Ecom?subject=Key%20Mapper%20Support">
                Email Support</a></h3>
    </div>
    </form>
</asp:Content>
