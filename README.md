![header](kmheader800w.png)

# Key Mapper

## Make your keyboard work the way you want it to

### Features

Key Mapper uses a virtual keyboard to create and show mappings. It will change the keyboard to reflect whatever keyboard you have active. You can browse the keyboards installed on your PC if you like.

You can view a 'slideshow' of all installed keyboards - it's in the 'Advanced' menu 

* * *

[Download .exe file (Windows 10/11 or .Net 4.8)](https://github.com/stuartd/keymapper/releases/download/1.4/KeyMapper.exe)
)

You will need to have Administrative access to your computer.

Support: [mailto:keymappersupport@gmail.com](mailto:keymappersupport@gmail.com)

* * *

### History

I created KeyMapper in 2007 when my employer decided to replace the in-house finance systems we had built with off-the shelf systems, but retained us for support only, so I had time on my hands. 

My intention was to use the (then) new language C# and also to solve a problem I had - **I never wanted caps lock on or num lock off!** - with a friendly user interface, using scancode mappings held in the registry rather than remapping them at runtime.

I ended out taking a deep dive into keyboard layouts, localization and internationalization, and distributed source control.

The documentation for scancode mappings stated:

> The mappings stored in the registry work at system level and apply to all users. These mappings cannot be set to work differently depending on the current user.

But I discovered that both Windows XP and Vista supported per-user key mappings written to `HKEY_CURRENT_USER\Keyboard Layout` and when KeyMapper was written, those were the latest Windows versions.

Because this was never documented, it was an opportunity to build something that would be of real utility - let people set keyboard mappings for their own user account, which no other programs at the time seemed to support.

But then Windows 7 came along and dropped the unofficial or accidental support, and it doesn't look like it's ever coming back: I took out all the code that used user mappings, but other than that the code is essentially as it was in 2008: I am happy with the code, but needless to say this is not how I would write it now..

If you're interested, my original blog post from 2008 on per-user scancode mappings is reproduced below.

* * *

### Credits

The image used for the keyboard keys is based on the blank icon in the keyboard icon set released by Alan Who?, used with permission
http://alanwho.com/ 

The late Michael Kaplan's [blog](https://web.archive.org/web/20250000000000*/%20http://blogs.msdn.com/b/michkap) was invaluable in understanding the minutiae of how Windows actually handles internationalization, localization, collations and keyboard usage

* * *

### Per-User Scancode Mappings [From 2008, Windows 7 was released in 2009]

One thing that distinguishes Key Mapper from other scancode mapping programs is that it lets you map or disable keys on a per-user basis: when Microsoft [originally implemented scancode mappings in Windows 2000](http://www.microsoft.com/whdc/archive/w2kscan-map.mspx), they stated in the "disadvantages" section:

> The mappings stored in the registry work at system level and apply to all users. These mappings cannot be set to work differently depending on the current user.

This is because the mappings are stored in `HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Keyboard Layout` which needs Administrative access to change and is only loaded at boot time.

In Windows XP, though, per-user mappings were quietly introduced, with no fanfare or documentation: scancode mappings set in the `HKEY_CURRENT_USER\Keyboard Layout` key were recognised, and applied to an individual user profile. This meant that mappings can be added or removed by logging off and logging back on again - still inconvenient, but less so than a full reboot: it also meant that mappings can be set up users without Administrative rights (and mappings set in `HKEY_LOCAL_MACHINE` were overridden by those in `HKEY_CURRENT_USER`).

It's possible that Microsoft kept this quiet because user mappings are incompatible with Fast User Switching: when you switch to an account that's already logged on, the mappings are not reloaded. It's also possible that because they kept it quiet, the Fast User Switching development team didn't realise that user mappings should be reloaded when switching users.

While this was a possible disadvantage to using user mappings, most people probably don't use more than one account on their computer anyway, and in computers attached to a domain (i.e. corporate PCs) which may often be used by different people Fast User Switching isn't available anyway.

There are some other advantages to user mappings:

*   They don't require Administrative rights to be set or removed.
*   Different users can have different mappings - one can have Caps Lock disabled but Num Lock enabled, another can have them the other way round
*   Keys can be mapped on shared computers without affecting all users

Originally developed at http://code.google.com/p/keymapper where it was popular with the shareware sites of the time (2008!)

![numbers](km_numbers_2008.png)

* * *

### Trivia
There was (is?) another place scancode mappings could be set - in the `HKEY_USERS\.DEFAULT\Keyboard Layout` key. These apply at the login prompt, but are then removed when logged in.


