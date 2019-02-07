# **To use KeyMapper in modern versions of Windows (i.e. 7 or later), you will have to initially switch to 'Boot' mappings from the Mappings menu.**

## **If you don't do this, your mappings won't work**

![Capture image](https://github.com/stuartd/keymapper/blob/develop/Capture.PNG)

* * *

Exported from [code.google.com/p/keymapper](http://code.google.com/p/keymapper)

![header](http://justkeepswimming.net/keymapper/images/kmheader.png)

# Key Mapper

## Make your keyboard work the way you want it to

### Remap and disable keyboard keys using a virtual keyboard

* * *

~~[Downloads still available](https://code.google.com/p/keymapper/downloads/list)~~

[Download .exe file](https://github.com/stuartd/keymapper/blob/develop/KeyMapper.exe)


You will also need to have Administrative access to your computer.

This is because both Windows XP and Vista supported per-user key mappings written to `HKEY_CURRENT_USER\Keyboard Layout` and when KeyMapper was released, those were the latest versions.

Then Windows 7 came along and dropped the unofficial or accidental support, and it doesn't look like it's coming back.

If you're interested, my original blog post from 2008 on per-user scancode mappings is reproduced below.

I did the work required to port it in 2012 and that's what the current master branch represents.

### Features

Key Mapper uses a virtual keyboard to create and show mappings. It will change the keyboard to reflect whatever keyboard the user currently has active. You can browse the keyboards installed on your PC if you like.

As a convenience for people who have disabled the Caps Lock, Num Lock or Scroll Lock keys, Key Mapper has a facility to toggle the value of these keys, in case - for example - an application sets Caps Lock on but the key is disabled.

### ~~~[Available downloads](https://code.google.com/p/keymapper/downloads/list)~~~

~~~Key Mapper is a Windows Form application written in C#, targeting the .NET Framework 2.0\. Most people download the MSI installer, but there's also a Setup package (these use [NGEN](http://msdn.microsoft.com/en-us/library/6t9t5wcf(VS.80).aspx) to optimise the application at install) but there's a plain .exe version as well.~~~

Project home page: [https://github.com/stuartd/keymapper](https://github.com/stuartd/keymapper)

Support: [mailto:keymappersupport@gmail.com](mailto:keymappersupport@gmail.com)

Discussions, Suggestions and Bug Reports: [http://keymapper.uservoice.com](http://keymapper.uservoice.com)

#### Per-User Scancode Mappings [From 2008, Windows 7 was released in 2009]

One thing that distinguishes Key Mapper from other scancode mapping programs is that it lets you map or disable keys on a per-user basis: when Microsoft [originally implemented scancode mappings in Windows 2000](http://www.microsoft.com/whdc/archive/w2kscan-map.mspx), they stated in the "disadvantages" section:

> The mappings stored in the registry work at system level and apply to all users. These mappings cannot be set to work differently depending on the current user.

This is because the mappings are stored in `HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Keyboard Layout` which needs Administrative access to change and is only loaded at boot time.

In Windows XP, though, per-user mappings were quietly introduced, with no fanfare or documentation: scancode mappings set in the `HKEY_CURRENT_USER\Keyboard Layout` key are recognised, and apply to an individual user profile. This means that mappings can be added or removed by logging off and logging back on again - still inconvenient, but less so than a full reboot: it also means that mappings can be set up users without Administrative rights. (Mappings set in `HKEY_LOCAL_MACHINE` are overridden by those in `HKEY_CURRENT_USER`).

It''s possible that Microsoft kept this quiet because user mappings are incompatible with Fast User Switching: when you switch to an account that''s already logged on, the mappings are not reloaded. It''s also possible that because they kept it quiet, the Fast User Switching development team didn''t realise that user mappings should be reloaded when switching users. Boot mappings persist through Fast User Switching.

While this is a possible disadvantage to using user mappings, most people probably don''t use more than one account on their computer anyway, and in computers attached to a domain (i.e. corporate PCs) which may often be used by different people Fast User Switching isn%27t available anyway.

There are some other advantages to user mappings:

*   They don''t require Administrative rights to be set or removed.
*   Different users can have different mappings - one can have Caps Lock disabled but Num Lock enabled, another can have them the other way round
*   Keys can be mapped on shared computers without affecting all users

There is yet another place scancode mappings can be set - in the `HKEY_USERS\.DEFAULT\Keyboard Layout` key. These apply at the login prompt, but are then removed when logged in.
