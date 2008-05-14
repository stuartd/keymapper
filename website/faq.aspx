<%@ Page Language="C#" AutoEventWireup="true" CodeFile="faq.aspx.cs" Inherits="faq" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
    <link href="keymapper.css" rel="stylesheet" type="text/css" />
    <script src="faq.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <h3>
            What does Key Mapper do?</h3>
        <p>
            Key Mapper allows you to tell Windows you want one key to act as another, or not
            to work at all. It converts your key mappings into codes which it stores in a specific
            registry key: when Windows starts, it looks in the registry and maps and disables
            the key.</p>
        <p>
            Unlike some other programs which do this, Key Mapper lets you set mappings for each
            user on the computer (as well as mappings which apply to all users). Because of
            this, you can log off and on again to set and cancel mappings rather than having
            to restart your computer.</p>
        <h3>
            Why should I use Key Mapper?</h3>
        <p>
            Here's a list of possible reasons why people would use Key Mapper. Click here to show how they would accomplish their needs.</p>
        <ul id="ScenarioList">
            <li>Mike is fed up of accidentally pressing Num Lock or Insert - keys which he never
                uses otherwise, but which mess up his typing. He needs to have Num Lock set On,
                though. <span class="kmusage">Mike can use Key Mapper to disable Num Lock and Insert</span>
            </li>
            <li>Sue hates accidentally pressing Caps Lock when she wants Shift. She hardly ever
                uses Caps Lock.<span class="kmusage">Sue can use Key Mapper to remap Caps Lock to Left
                    Shift, and to remap Scroll Lock to Caps Lock</span></li>
            <li>Mark in annoyed that his work keyboard doesn&#39;t have hardware volume up/down
                buttons like his Mac keyboard at home.<span class="kmusage">Mark uses Key Mapper to
                    his F10 Key to Play/Pause, F11 key to Volume Down and F12 to Volume Up.</span></li>
            <li>Mary is always looking for her email program and browser in the taskbar. She wishes
                she could just press a button to make them appear whether they are open or not.<span
                    class="kmusage">Mary remaps Insert to Email, and Num Lock to Browser.</span></li>
            <li>Sandy is frustrated that when using Boot Camp or virtualization to run Windows on
                her Mac that there&#39;s no Print Screen or Scroll Lock keys (she uses Excel all
                the time, and it happens to be the only program on Earth that actually takes notice
                of Scroll Lock). She also gets fed up with pressing Command - expecting it to behave
                like Control - when in fact it&#39;s the Windows key and opens the Start Menu.<span
                    class="kmusage">Sandy uses Key Mapper to remap F3 to Print Screen, and F4 to Scroll Lock.</span></li>
            <li>Andy keeps pressing &quot;Power Off&quot; on his Extended Keyboard because it&#39;s
                right next to the Delete key, causing much annoyance and inconvenience.</li>
            <li>Kevin has broken the 6 key on his laptop. He&#39;d like to remap another key to
                it, like F6, rather than have to buy and fit a new keyboard.<span class=kmusage>Kevin remaps F6 to 6</span></li>
            <li>George hates Sarah and wants to mess with her head, so he wants to remap half her
                keys and disable the other half.<span class=kmusage>George remaps and disables keys like the crazy man he is, ignoring the warnings about remapping Ctrl, ALt or Delete.</span></li>
        </ul>
    </form>
    <h3>
        How do I remap or disable keys?</h3>
    <p>
        If you want to map a key to another and they&#39;re both on the keyboard, you can
        drag and drop the action key onto the target key. For example, to remap Caps Lock
        to act like Left Shift, drag and drop the Left Shift key onto the Caps Lock key.
    </p>
    <p>
        To disable a key on the virtual keyboard, drag and drop it off the keyboard.</p>
    If you want to remap a key to a key or action that isn&#39;t on the virtual keyboard,
    double-click the key to bring up the Add Mapping screen. From here, you can choose
    what you want the key to do from the lists presented.
    <p>
        The key lists are divided into three groups:</p>
    <ul>
        <li>Useful Keys - these are keys which are commonly used in mappings.</li>
        <li>All Working Keys - these are all the keys which have worked in mappings in testing</li>
        <li>All Keys - these are all the keys which are available. Some of these require special
            keyboard drivers to work, and some cannot be used as mappings because of the way
            Windows processes keys.</li>
    </ul>
    <p>
        If you want to remap or disable a key that itself isn&#39;t on the virtual keyboard,
        you can either select it from a list of keys or use Key Capture.</p>
    <ul id="sublist">
        <li>To select from the lists, choose Create New Mapping from the Mappings menu, and
            then the Select From Lists option. The keys are arranged into groups, with &#39;All
            keys&#39; as the first group. Find the key you want to map, then click Set.</li>
        <li>To use Key Capture, choose Create New Mapping from the Mappings menu, and then the
            Use Key Capture option. You&#39;ll see a blank key: press the key you want to map.
            If it isn&#39;t shown, it may be an extended key which your keyboard handles itself
            - some keyboards have extended keys which the keyboard driver processes and which
            aren&#39;t received by Windows: these keys can&#39;t be remapped or disabled. (A
            common annoyance is a Sleep or Shut Down key inconveniently placed near a commonly
            used key. A friend of mine glued his keyboard power key so it couldn&#39;t be pressed
            accidentally)</li>
    </ul>
    <p>
        Once you've selected the key, you again choose from the list of actions, use Key
        Capture to grab a key that's on your keyboard, or click Disable to disable the key.</p>
    <h3>
        How do I unmap or enable keys?</h3>
    <p>
        If the mapped or disabled key is shown on the virtual keyboard, simply drag it off
        the keyboard to restore or enable it.</p>
    <p>
        If the key isn&#39;t on the virtual keyboard, you can delete the mapping from the
        Mapping List screen. Click on Mapping List in the keyboard&#39;s Windows Menu to
        start the form: it shows all the current and pending mappings, and allows you to
        delete the current mappings.</p>
    <p>
        You can also clear all your mappings by choosing Clear All Mappings from the Mappings
        menu.</p>
    <h3>
        How do I undo my changes?</h3>
    <p>
        The virtual keyboard supports Undo and Redo from the Edit menu: you can also restore
        your mappings to their current effective state by choosing Revert To Saved Mappings
        from the Mappings menu (this restores the mappings shown to those which are currently
        in effect, discarding any new mappings and restoring any cleared mappings)</p>
    <p>
        You can also clear all your mappings by choosing Clear All Mappings from the Mappings
        menu.</p>
    <p>
        &nbsp;</p>
    <h3>
        How do I create mappings which apply to all users?</h3>
    <p>
        By default, mapped and disabled keys only apply to the current user (except in Windows
        2000, which only allows mappings which apply to all users). To set mappings which
        apply to all users and which take effect before a user has logged in (i.e. at the
        Windows Login screen) click Show and then Boot Mappings from the Mappings menu,
        and create the required mappings. You can have both boot mappings and user mappings:
        if the same key is mapped in each, then user mappings override boot mappings. If
        you use Fast User Switching, then you will need to use Boot Mappings as user mappings
        are discarded when using Fast User Switching to switch to an account that&#39;s
        already logged on.</p>
    <p>
        Using Boot Mappings requires you to be an Administrator on your PC. It&#39;s also
        problematic on Windows Vista, as UAC prevents access from
    </p>
    <h3>
        How do I change the key colours?</h3>
    <p>
        Show the Colour Map form from the keyborad&#39;s Windows menu. You can double-click
        on a button to invoke the Colour Editor, which lets you tweak individual colour
        components and set the font colour. Alternatively, click the Random key until you
        find a colour you like! The Colour Map form has a right-click menu which allows
        you to show all the possible buttons instead of just the ones in use, to close all
        editor forms, and to reset all the colours back to their defaults.</p>
    <h3>
        How do I get back a window which is off the screen?</h3>
    <p>
        You can reset the positions of the child windows by choosing Arrange All in the
        Windows menu.</p>
    <h3>
        What does the Toggle Key menu do?</h3>
    <p>
        The Toggle Key menu allows you to turn Num, Caps and Scroll lock on and off: this
        can be useful if you have disabled one of those keys but a rogue application then
        switches the value.
    </p>
    <h3>
        Why is my keyboard wrong?</h3>
    <p>
        If your Enter key is shown in the wrong orientation (i.e. horizontal when it should
        be vertical) you can switch it using the virtual keyboard&#39;s Keyboard menu.
        <br />
        If the displayed keyboard is a different language from your current keyboard and
        you haven't changed your keyboard since Key Mapper started, then that's a bug. The
        Keyboard Menu is also the place to specify you have a Mac keyboard (where the Alt
        and Windows (Command) keys are reversed), switch the Number Pad off if you have
        a laptop, or switch to Typewriter Keys Only view for the best view when browsing
        through the different keyboards installed on your PC.
    </p>
    <h3>
        How do I see all the mappings I've created?</h3>
    <p>
        The Mapping List screen shows you all your user and boot mappings, whether current,
        pending or cleared.</p>
    <h3>
        Why don't the Chinese keyboards work?</h3>
    <p>
        I think it's because Chinese keyboards require an Input Method Editor: the Korean
        keyboard doesn't work either. The Japanese one does, though.</p>
    <h3>
        How do you get all those exotic keyboards in Windows XP?</h3>
    <p>
        You need your Windows installation disk, or at least an i386 folder with the Windows
        installation files. A recovery disk from a PC manufacturer might not work though,
        it depends how it's configured.
        <br />
        Then you need to open "Regional and Language Options" in Windows' Control Panel,
        select the Languages tab, and check both "Install files for complex script and right-to-left
        languages ..." and "Install files for East Asian languages". The required files
        will be installed when you click OK or Apply, and you'll need to restart your computer.
        <h3>
            Can I remap SHIFT-4 or CTRL-Q to another character?</h3>
        <p>
            No. Scancode mappings apply to the physical key - Shift-4 is two keypresses, Shift
            and then Four. If you want to remap these kinds of keypresses, there are a lot of
            programs which will enable you to do it - they run in the background, detect your
            keypresses, and can then launch programs or substitute other characters (and that&#39;s
            why they don&#39;t require you to restart your computer or log off)<br />
            There are some key combinations which can be mapped though, due to the way Windows
            processes keypresses - you can map Alt-PrtScr and Ctrl-Break, for example (Alt PrtScr
            puts a screenshot of the program currently running on your clipboard, and Ctrl-Break
            cancels some dialogs, e.g. the Open File dialog.)</p>
    <h3>
        Can Key Mapper create Microsoft Keyboard Layout files?</h3>
    <p>
        No. You can use <a href="http://www.microsoft.com/globaldev/tools/msklc.mspx">Microsoft's
            own tool</a> for this, or there are alternatives like <a href="http://www.kbdedit.com/">
                KBEdit</a></p>
    <h3>
        How does remapping keys work?</h3>
    <p>
        Key Mapper writes your key mappings to the system registry in a format that Windows
        understands. The format is documented <a href="http://www.microsoft.com/whdc/archive/w2kscan-map.mspx">
            here</a>, although the fact that you can have User mappings in Windows XP or
        later isn't documented anywhere.</p>
</body>
</html>
