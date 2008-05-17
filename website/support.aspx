<%@ Page Language="C#" AutoEventWireup="true" CodeFile="support.aspx.cs" Inherits="httpdocs_KeyMapper_support" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Key Mapper Support Page :: justkeepswimming.net</title>
    <link href="keymapper.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableViewState="False" />
        <h2>
            Quick Start</h2>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <ul>
                    <li>Drag a key off the keyboard to disable it</li>
                    <li>To remap a key, double-click it.</li>
                    <li>To delete a key mapping or re-enable a key, drag it off the keyboard.</li>
                    <li>You can capture the key you want to map or choose it from a list by choosing Create
                        New Mapping from the Mappings menu.</li>
                    <li>If the key you want to map and the one you want to map it to are both visible, drag
                        and drop the action key onto the target key. Otherwise, choose "Create new Mapping"
                        from them Mappings menu and choose from the key lists or use key capture.</li>
                </ul>
            </ContentTemplate>
        </asp:UpdatePanel>
        <h2>
            Contact</h2>
        <h2>
            <a href="faq.aspx">FAQ</a></h2>
    </div>
    </form>
    <h3>
        <a href="/KeyMapper/install.html">Get latest version</a></h3>
</body>
</html>
