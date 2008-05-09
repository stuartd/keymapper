<%@ Page Language="C#" AutoEventWireup="true" CodeFile="faq.aspx.cs" Inherits="faq" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
    <link href="keymapper.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <h3>
            Why should I use Key Mapper</h3>
        <p>
            Here's a list of possible reasons why people would use Key Mapper.</p>
        <div>
            <asp:BulletedList ID="ScenarioList" runat="server" CssClass="ScenarioList">
                <asp:ListItem>Mike is fed up of accidentally pressing Num Lock or Insert - keys 
                which he never uses otherwise, but which mess up his typing. He needs to have 
                Num Lock set On, though.</asp:ListItem>
                <asp:ListItem>Sue hates accidentally pressing Caps Lock when she wants Shift. 
                She hardly ever uses Caps Lock.</asp:ListItem>
                <asp:ListItem>Mark in annoyed that his work keyboard doesn&#39;t have hardware 
                volume up/down buttons like his Mac keyboard at home.</asp:ListItem>
                <asp:ListItem>Mary is always looking for her email program and browser in the 
                taskbar. She wishes she could just press a button to make them appear whether 
                they are open or not.</asp:ListItem>
                <asp:ListItem>Sandy is frustrated that when using Boot Camp or virtualization to 
                run Windows on her Mac that there&#39;s no Print Screen or Scroll Lock keys (she 
                uses Excel all the time, and it happens to be the only program on Earth that 
                actually takes notice of Scroll Lock). She also gets fed up with pressing 
                Command - expecting it to behave like Control - when in fact it&#39;s the Windows 
                key and opens the Start Menu.</asp:ListItem>
                <asp:ListItem>Andy keeps pressing &quot;Power Off&quot; on his Extended Keyboard because 
                it&#39;s right next to the Delete key, causing much annoyance and inconvenience.</asp:ListItem>
                <asp:ListItem>Kevin has broken the 6 key on his laptop. He&#39;d like to remap 
                another key to it, like F6, rather than have to buy and fit a new keyboard.</asp:ListItem>
                <asp:ListItem>George hates Sarah and wants to mess with her head, so he wants to 
                remap half her keys and disable the other half.</asp:ListItem>
            </asp:BulletedList>
        </div>
    </form>
    <h3>
        How do I remap keys?</h3>
    <p>
        If you want to map a key to another and they&#39;re both on the keyboard, you can
        drag and drop the action key onto the target key. For example, to remap Caps Lock
        to act lke Left Shift, drag and drop the Left Shift key onto the Caps Lock key.</p>
    <p>
        If you want to remap a key to one that isn&#39;t on the keyboard, double-click the
        key to bring up the Add Mapping screen. From here, you can
    </p>
    <p>
        If you want to remap a key that isn&#39;t on the virtual keyboard, you can either
        select it from a list of keys or use Key Capture.</p>
    <ul id="sublist">
        <li>To select from the lists, choose Create New Mapping from the Mappings menu, and
            then the Select From Lists option. The keys are arranged into groups, with &#39;All
            keys&#39; as the first group. Find the key you want to map, then click Set.</li>
        <li>To use Key Capture, choose Create New Mapping from the Mappings menu, and then the
            Use Key Capture option. You&#39;ll see a blank key: press the key you want to map.
            If it doesn&#39;t show up, it may be an extended key which your keyboard handles
            itself - some keyboards have extended keys which the keyboard driver processes and
            which aren&#39;t received by Windows: these keys can&#39;t be remapped or disabled.
            (A common annoyance is a Sleep or Shut Down key inconveniently placed near a commonly
            used key.)</li>
    </ul>
</body>
</html>
