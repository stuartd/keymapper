<%@ Page Language="C#" AutoEventWireup="true" CodeFile="faq.aspx.cs" Inherits="faq"
    MasterPageFile="keymapper.master" Title="Key Mapper FAQ" %>

<asp:Content ID="faqhead" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.2.6/jquery.min.js">
     
    </script>

    <script type="text/javascript" src="keymapper.js"></script>

    <link rel="stylesheet" type="text/css" href="css/faq.css" />
</asp:Content>
<asp:Content ID="default" ContentPlaceHolderID="body" runat="server">
    <!-- Need a div that can act as the top of the page -->
    <div id="faqtop">
    </div>
    <form id="form1" runat="server">
        <div id="contents">
            <h3>
                Table Of Contents</h3>
            <a id="toggletoc" href="#">Hide</a>
        </div>
        <!-- As toc ul runs on server, can't predict it's ID, so wrap it in a div for easy hiding. -->
        <div id="toc_container">
            <ul id="toc" runat="server">
            </ul>
        </div>
        <h3 class="question" runat="server">
            What does Key Mapper do?</h3>
        <p>
            Key Mapper allows you to disable a keyboard key or to remap it to act as another
            key which isn&#39;t on your keyboard. In this faq, the term &quot;key mapping&quot;
            is used to describe disabled and remapped keys.</p>
        <br />
        <p>
            Unlike other programs which do this, Key Mapper lets you set key mappings for each
            person who on your computer (as well as mappings which apply to all users). These
            don&#39;t require Administrative rights, and you can log off and on again to set
            and cancel key mappings rather than having to restart your computer. This functionality
            was introduced in Windows XP, so Windows 2000 users can't have per-user key mappings.</p>
        <h3 class="question" runat="server">
            Why would I want to disable or remap my keys?</h3>
        <p>
            The most common uses are disabling the Num Lock or Insert key - keys which most
            people never press except accidentally, and which cause confusion and inconvenience
            when they are pressed.
        </p>
        <br />
        <p>
            Another common use is disabling Caps Lock, or perhaps remapping it to Shift, so
            accidentally pressing Caps Lock doesn't turn on Caps Lock, but it's functionality
            is still partially preserved. An option would be to remap another key to Caps Lock
            - perhaps Scroll Lock, which only Excel power-users ever use.
        </p>
        <br />
        <p>
            You can also remap keys you don't use to keys which aren't on your keyboard - Volume
            Up and Down, Play and Pause, Email.
        </p>
        <br />
        <p>
            If you using virtualisation to run Windows on a Mac, you will have noticed that
            Mac keyboards don't have many of the keys Windows uses, for example Print Screen
            and Scroll Lock, and Key Mapper lets you map keys to these, as well as remapping
            the Command key to Control and choosing which keys activate the context menu. This
            also applies when using Remote Desktop For Mac to administer a Windows computer
            remotely. (If you use Remote Desktop for Mac Beta 3 on a Mac with a UK keyboard
            layout, disabling both Alt keys stops the annoying problem with the Alt key &#39;sticking&#39;
            when pressed)</p>
        <br />
        <p>
            If your laptop keyboard has a broken key, you can use Key Mapper to map another
            key to the broken one rather than buy and fit a new keyboard.
        </p>
        <h3 class="question" runat="server">
            How do I disable or remap keys with Key Mapper?</h3>
        <p>
            To disable a key you can see on the virtual keyboard, drag and drop it off the virtual
            keyboard.</p>
        <br />
        <p>
            Also, if you want to map a key to another and they&#39;re both on the virtual keyboard,
            you can drag and drop the action key onto the target key. For example, to map Caps
            Lock to act like Left Shift, drag and drop the Left Shift key onto the Caps Lock
            key.
        </p>
        <br />
        <p>
            If you want to remap a key on the virtual keyboard to a key that isn&#39;t on the
            virtual keyboard, double-click the key to bring up the Add Mapping screen. From
            here, you can choose what you want the key to do from the lists presented.</p>
        <br />
        <p>
            The key lists are divided into three groups:</p>
        <ul class="sublist">
            <li>Useful Keys - these are keys which are commonly used - and most useful - in key
                mappings.</li>
            <li>All Working Keys - these are all the keys which have worked in key mappings in testing</li>
            <li>All Keys - these are all the keys which are available. Some of these require special
                keyboard drivers to work, and some cannot be used in key mappings because of the
                way Windows processes keys.</li>
        </ul>
        <br />
        <p>
            If you want to remap or disable a key that itself isn&#39;t on the virtual keyboard,
            you can either select it from a list of keys or use Key Capture.</p>
        <br />
        <ul class="sublist">
            <li>To select from the lists, choose &quot;Create New Mapping&quot; from the Mappings
                menu, and then the &quot;Select From Lists&quot; option. The keys are arranged into
                groups, with &#39;All keys&#39; as the first group. Find the key you want to remap,
                then click Set.</li>
            <li>To use Key Capture, choose "Create New Mapping" from the Mappings menu, and then
                the "Use Key Capture" option. You&#39;ll see a blank key: press the key you want
                to remap. If it isn&#39;t shown, it may be an extended key which your keyboard handles
                itself - some keyboards have extended keys which the keyboard driver processes and
                which aren&#39;t received by Windows: these keys can&#39;t be remapped or disabled.</li>
        </ul>
        <br />
        <p>
            Once you've selected the key, you again choose from the list of actions, use Key
            Capture to grab a key that's on your keyboard, or click Disable to disable the key.</p>
        <h3 class="question" runat="server">
            How do I enable or unmap keys?</h3>
        <p>
            If the remapped or disabled key is shown on the virtual keyboard, simply drag it
            off the keyboard to restore or enable it.</p><br />
        <p>
            If the remapped or disabled key isn&#39;t on the virtual keyboard, you can delete
            the key mapping from the Mapping List screen. Click on Mapping List in the keyboard&#39;s
            Windows Menu to show the screen: it shows all your current and pending key mappings,
            and allows you to delete key mappings by clicking the Delete column.</p><br />
        <p>
            You can also clear all your key mappings by choosing "Clear All Mappings" from the
            Mappings menu.</p>
        <h3 class="question" runat="server">
            How do I undo my changes?</h3>
        <p>
            Key Mapper supports Undo and Redo from the Edit menu: you can also restore your
            key mappings to their current effective state by choosing Revert To Saved Mappings
            from the Mappings menu (this restores the key mappings shown to those which are
            currently in effect, discarding any new key mappings and restoring any cleared key
            mappings)</p><br />
        <p>
            You can also clear all your key mappings by choosing Clear All Mappings from the
            Mappings menu.</p>
        <h3 class="question" runat="server">
            What are the extra keys I can use?</h3>
        <p>
            If there are keys you never use - some function keys, perhaps - you can remap them
            to something more useful.</p>
        <p>
            There are some key mappings to non-standard keyboard keys that can be useful: how
            well they work depends on which program you use for music and web browsing.</p>
        <ul class="sublist">
            <li>Volume Keys - you can remap keys to Volume Up, Volume Down, and Mute. These are
                controlled by Windows and will work whatever program you use.</li>
            <li>Media Keys - Next Track, Previous Track, Play / Pause, and Stop are available, These
                <a href="http://msdn.microsoft.com/en-us/library/ms997498.aspx#mshrdwre_topic4">should</a>
                be recognised by any mainstream music player: some players - iTunes, for example
                - will only react to keys when it is the frontmost application; and some - like
                VLC - ignore them altogether, while Windows Media Player can be controlled whether
                active or even minimised. </li>
            <li>Browser Keys - Back, Forward, Stop Loading, Refresh, and Home can be remapped to
                (if you use Internet Explorer, you can also remap a key to your &#39;Favourites&#39;).
                A key remapped to Home will start your browser if isn&#39;t already running, and
                start a new browser window if it is. If you press it while your browser is the current
                window, you will return to your home page.</li>
            <li>Email - a key remapped to Email will start your email program if it isn&#39;t running,
                and bring it to the foreground if it is.</li>
        </ul>
        <h3 class="question" runat="server">
            I remapped a function key and now I can&#39;t use it anymore in another program
            where I need it!</h3>
        <p>
            You need to unmap the key and choose another: key key mappings apply to all programs
            or none.</p>
        <h3 class="question" runat="server">
            Can I disable my keyboard's Sleep (or Shutdown) key? I keep pressing it my mistake.</h3>
        <p>
            Probably not. Most keyboard drivers act on special keys before they reach Windows,
            which means they can't be remapped or disabled.<br />
            If you want to try, you can select the key from the key lists or try using Key Capture
            (if you do that, close any open documents as it may well cause your computer to
            sleep or shut down).</p>
        <h3 class="question" runat="server">
            How do I create key mappings which apply to all users?</h3>
        <p>
            By default, remapped and disabled keys only apply to the current user (except in
            Windows 2000, which only allows key mappings which apply to all users). To set key
            mappings which apply to all users and which take effect before a user has logged
            in (i.e. at the Windows Login screen) click Show and then Boot Mappings from the
            Mappings menu, and create the required key mappings. You can have both boot key
            mappings and user key mappings: if the same key is remapped in each, then user key
            mappings override boot key mappings. If you use Fast User Switching, then you will
            need to use Boot Mappings as User Mappings are discarded when using Fast User Switching
            to switch to an account that is already logged on.</p>
        <br />
        <p>
            Using Boot Mappings requires you to be an Administrator on your PC.
        </p>
        <h3 class="question" runat="server">
            How do I change the key colours?</h3>
        <p>
            Show the Colour Map form from the virtual keyboard&#39;s Windows menu. You can double-click
            on a button to invoke the Colour Editor, which lets you tweak individual colour
            components and set the font colour. Alternatively, click the Random key until you
            find a colour you like! The Colour Map form has a right-click menu which allows
            you to show all the possible buttons instead of just the ones in use, to close all
            editor forms, and to reset all the colours back to their defaults.</p>
        <h3 class="question" runat="server">
            How do I get back a window which is off the screen?</h3>
        <p>
            You can reset the positions of the child windows by choosing Arrange All in the
            Windows menu.</p>
        <h3 class="question" runat="server">
            What does the Toggle Key menu do?</h3>
        <p>
            The Toggle Key menu allows you to turn Num, Caps and Scroll lock on and off: this
            can be useful if you have disabled one of those keys but another application then
            switches the value.
        </p>
        <h3 class="question" runat="server">
            Why is the virtual keyboard's Enter key the wrong way up?</h3>
        <p>
            If your Enter key is shown in the wrong orientation (i.e. horizontal when it should
            be vertical) you can switch it by clicking "Change Enter Key@ on the Keyboard menu,
            and you can click "Always Use This Enter Key Orientation For This Layout" on the
            Keyboard menu to save the change.</p>
            <br />
            <p>If you have multiple keyboards installed, the displayed keyboard should be the same
            language as your current keyboard unless you've changed your keyboard since Key
            Mapper started.</p><br />
        <p>
            The Keyboard Menu is also the place to specify you have a Mac keyboard (where the
            Alt and Windows (Command) keys are reversed), or switch the Number Pad off if you
            have a laptop.</p>
        <h3 class="question" runat="server">
            How do I see all the key mappings I've created?</h3>
        <p>
            The Mapping List screen shows you all your user and boot key mappings, whether current,
            pending or cleared.</p>
        <h3 class="question" runat="server">
            Why don't the Chinese or Korean keyboard layouts work?</h3>
        <p>
            Chinese and Korean keyboards require an Input Method Editor, which use multiple
            keypresses to construct a <a href="http://en.wikipedia.org/wiki/Logogram">logogram</a>
            representing a word. The way Key Mapper gets the layouts for other languages doesn&#39;t
            work on Chinese or Korean keyboards.</p>
        <h3 class="question" runat="server">
            How do you get exotic keyboards in Windows XP?</h3>
        <p>
            You need your Windows installation disk, or at least an i386 folder with the Windows
            installation files. A recovery disk from a PC manufacturer might not work though,
            it depends how it's configured.</p><br />
        <p>
            To install them, first you need to open "Regional and Language Options" in Windows' Control Panel,
            select the Languages tab, and check both "Install files for complex script and right-to-left
            languages ..." and "Install files for East Asian languages". The required files
            will be installed when you click OK or Apply so have your CD (or the path to your
            i386 folder) handy, and you'll need to restart your computer.</p>
        <h3 class="question" runat="server">
            Can I remap SHIFT-4 or CTRL-Q to another character?</h3>
        <p>
            No. Scancode key mappings apply to the physical key - Shift-4 is two keypresses,
            Shift and then Four. If you want to remap these kinds of keypresses, there are a
            lot of programs which will enable you to do it - they run in the background, detect
            your keypresses, and can then launch programs or substitute other characters (and
            that&#39;s why they don&#39;t require you to restart your computer or log off)<br />
            There are some quirky key combinations which can be remapped though, due to the
            way Windows processes keypresses - you can remap Alt-PrtScr and Ctrl-Break, for
            example (Alt PrtScr puts a screenshot of the program currently running on your clipboard,
            and Ctrl-Break cancels some dialogs, e.g. the &#39;Open File&#39; dialog.)</p>
        <h3 class="question" runat="server">
            I&#39;ve disabled Num Lock / Scroll Lock / Caps Lock and now it&#39;s stuck on the
            wrong value. How can I change it?</h3>
        <p>
            You can toggle the value of the Num Lock, Caps Lock and Scroll Lock keys from Key
            mapper&#39;s Toggle Keys menu.</p>
        <h3 class="question" runat="server">
            I&#39;ve set up a boot key mapping to disable Num Lock but when I need to log in
            Num Lock is off. How can I set it to come on?</h3>
        <p>
            Use the "Set Current Toggle Keys As Default" item on the Toggle Keys menu to set
            the keys at login.</p>
        <h3 class="question" runat="server">
            Can Key Mapper create Microsoft Keyboard Layout files?</h3>
        <p>
            No. You can use <a href="http://www.microsoft.com/globaldev/tools/msklc.mspx">Microsoft's
                own tool</a> for this, or there are alternatives like <a href="http://www.kbdedit.com/">
                    KBEdit</a></p>
        <h3 class="question" runat="server">
            How does key mapping keys work?</h3>
        <p>
            Key Mapper writes your key mappings to the system registry in a format that Windows
            understands. The format is documented <a href="http://www.microsoft.com/whdc/archive/w2kscan-map.mspx">
                here</a>, although the fact that you can have User Mappings in Windows XP or
            later isn't documented anywhere (until now)</p>
        <h3 class="question" runat="server">
            Hey! My key mappings have disappeared! Where did they go?</h3>
        <p>
            If you set user mappings (which are the default in Windows XP and Vista) and then
            use Fast User Switching to switch to a user account that&#39;s already logged on,
            that user&#39;s key mappings will not be loaded.</p>
        <h3 class="question" runat="server">
            I&#39;ve disabled Num Lock (or Caps Lock) but when I press it, the light still comes
            on. What&#39;s up with that?</h3>
        <p>
            Your keyboard driver is acting on the keypress before it reaches Windows</p>
        <h3 class="question" runat="server">
            I've disabled Insert but now what I type is overwriting my text! How do I turn it
            off?</h3>
        <p>
            If your keyboard has a numeric keypad, Shift-NumericZero toggles Insert. Generally,
            Insert / Overtype is application-specific: if you're using Word, there's a small
            button marked OVR at the bottom of the Window which toggles between insert and Overtype
            modes (in Word 2007 it's hidden away in Word Options / Advanced / ' Use Overtype
            Mode')</p>
        <p>
            Microsoft have a knowledge-base article on the subject with more options, charmingly
            called "<a href="http://support.microsoft.com/default.aspx/kb/325719">Text that is to
                the right of the insertion point disappears as you type in Word</a>"
        </p>
        <h3 class="question" runat="server">
            Why does my Anti-Virus / Spyware / Malware program say Key Mapper is suspicious?</h3>
        <p>
            Key Mapper uses a keyboard hook for Key Capture, and also to detect if Num Lock
            or Caps Lock is pressed while the program is running. The source code is available
            to <a href="http://code.google.com/p/keymapper/source/browse/trunk/keymapper">browse</a>
            or <a href="http://code.google.com/p/keymapper/downloads/list">download</a>, if
            you want to make sure for yourself. If you want to build it, you can use Microsoft&#39;s
            free <a href="http://www.microsoft.com/express/vcsharp/">C# Express Edition</a>.</p>
        <h3 class="question" runat="server">
            Why does Key Mapper disable Num Lock when I remap my Pause key?</h3>
        <p>
            Key Mapper automatically disables Num Lock when you remap your Pause key, as otherwise
            Num Lock will toggle whenever you press the remaped Pause key. Even if you have
            Num Lock remapped to another key, it will still fire when you press the remapped
            Pause key, which could be very annoying: a useful side-effect of this though is
            that you can assign two keystrokes to the Pause key: the first is what you remap
            Pause to, and the second is what you remap Num Lock to.
            </p>
            <br />
<p>
            In order to do this, you have to remap Pause to the first key in the combination,
            let Key Mapper disable Num Lock, then re-enable Num Lock and map it to the second
            key in the key combination.</p>
            <br />
            <p>For example: remapping Pause to Alt-F4:<br />
            Map Pause => Alt<br />
            Num Lock: => 4</p>
            <br />
            <p>Pause is thus remapped to Alt-F4, the shortcut used to close programs (or if the
            desktop has the focus, restart Windows)</p>
            <br />
            <p>Another example: remapping Pause to the Euro symbol<br />
            Pause => Right Alt<br />
            Num Lock => 4 $</p>
            <br />
            Pause is remapped to Right-Alt-4, the shortcut for the Euro sign.
            <br />
            <p>
            Other possibilities include:<br />
            Shift-F10 (Context menu)<br />
            Shift 2 (or Shift ' depending on keyboard layout) to get @<br />
            Windows-L (Lock Workstation in Windows XP or later)</p>
            <br />
        </p>
        <h3 class="question" runat="server">
            I've remapped some keys, but my keyboard hasn't been updated and still shows the
            original keys!</h3>
        <p>
            This functionality will have to wait for a future version. Alternatively, try the <a href="http://www.thinkgeek.com/computing/input/9836/">Optimus Maximus keyboard</a> (Thanks to Steve Ward
            for this question.)</p>
    </form>
</asp:Content>
