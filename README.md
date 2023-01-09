# N1MM+ Commands for Touch Portal
### by Jason Frazier W7DM

(Stream Deck plugin planned in the future, see my other projects)

This TouchPortal plugin integrates the N1MM Logger+ app by Tom Wagner N1MM, 
allowing the user to create custom buttons in TouchPortal to control the popular
N1MM+ Logger ham radio contesting application for Windows.

Custom button actions can currently perform these functions:
- Execute macros directly in N1MM+, including CAT rig control behaviors
- Transmit CW content and SSB wav files, which can also contain macros
- Summon N1MM+ into the foreground and send arbitrary N1MM+ keystroke commands, including Ctrl/Alt/Shift


This is a work-in-progress plugin, created 
specifically with contesters in mind, in a way that eases multitasking on the 
radio operator's shack PC while quickly allowing to regain control over your contest activity.

This plugin accesses **experimental features** in N1MM+ to integrate external
actions, courtesy of Tom N1MM.  This software may perform unexpectedly or stop 
working if the experimental features are changed or withdrawn. Its behavior 
might be modified, expanded, or narrowed in scope during feature development,
in the interests of stability for the competitive community of amateur radio 
contesters.  It is not intended to be a general purpose radio control toolkit.
This plugin is not compatible with the earlier N1MM Logger Classic app.

## Installation instructions and User Guide
...are being written right now. Screenshots and walkthroughs are also to come. 
For now, here are some early notes for testers and developers.

**Using this software has a one-time cost.** TouchPortal for Windows is free, 
and the tablet/phone app is a free install on the Apple/Google store, but you must pay for the 
"Pro Upgrade" in the tablet application's in-app purchase in order to use plug-ins like this one. At the time of this plugin's debut, 
the Pro Upgrade went for about $14 USD. If you switch between Apple/Android, you have to buy it twice. 
I am not associated with TouchPortal, and I don't receive any funds from them.

Start by installing TouchPortal for Windows from:
https://www.touch-portal.com/#downloadstitle
All of the defaults are fine. There is a step to install ADB USB drivers, which
come from Google and only run when you connect Android over USB (not WiFi).

![A cat](https://github.com/frazierjason/N1mmCommands-TouchPortal/blob/a4e312141209ae65b25b7cf9186aa80c9a13b962/tp-installing.jpg)

Reboot your PC after installation of TouchPortal, then start up TouchPortal.
Since N1MM+ isn't officially on Mac, this plugin doesn't support Mac right now.

After rebooting, go to Windows Start menu and start Touch Portal.  You should 
see a Windows Advanced Firewall dialog come up asking your permission to allow
Touch Portal to receive connections.  **CLICK ACCEPT**, or you will not be able
to make connections from your tablet to your PC.

Upon starting TouchPortal, complete the setup wizard as follows:

![A cat](https://github.com/frazierjason/N1mmCommands-TouchPortal/blob/a4e312141209ae65b25b7cf9186aa80c9a13b962/tp-setup-1.JPG)

![A cat](https://github.com/frazierjason/N1mmCommands-TouchPortal/blob/a4e312141209ae65b25b7cf9186aa80c9a13b962/tp-setup-2.JPG)

![A cat](https://github.com/frazierjason/N1mmCommands-TouchPortal/blob/a4e312141209ae65b25b7cf9186aa80c9a13b962/tp-setup-3.JPG)

![A cat](https://github.com/frazierjason/N1mmCommands-TouchPortal/blob/a4e312141209ae65b25b7cf9186aa80c9a13b962/tp-setup-4.JPG)

![A cat](https://github.com/frazierjason/N1mmCommands-TouchPortal/blob/a4e312141209ae65b25b7cf9186aa80c9a13b962/tp-setup-5.JPG)

![A cat](https://github.com/frazierjason/N1mmCommands-TouchPortal/blob/a4e312141209ae65b25b7cf9186aa80c9a13b962/tp-setup-6.JPG)

After starting up, you will have an empty "(main)" page with no button tiles.

![A cat](https://github.com/frazierjason/N1mmCommands-TouchPortal/blob/a4e312141209ae65b25b7cf9186aa80c9a13b962/tp-first-startup.jpg)

TouchPortal is now set up.  If you close it, it's still running in the system
tray next to the Windows clock.  You can reopen/restart/exit from there.

By default TouchPortal does not start automatically. If you want, you can enable
auto startup in the application's settings menus.

![A cat](https://github.com/frazierjason/N1mmCommands-TouchPortal/blob/a4e312141209ae65b25b7cf9186aa80c9a13b962/tp-open-settings.jpg)

![A cat](https://github.com/frazierjason/N1mmCommands-TouchPortal/blob/c44d19b48e7f023a12974f01dbf38f448391ed0b/tp-settings-scrolldown.jpg)

![A cat](https://github.com/frazierjason/N1mmCommands-TouchPortal/blob/c44d19b48e7f023a12974f01dbf38f448391ed0b/tp-settings-enable-autostart.jpg)

Now you need a spare iPad or Android tablet to use at your shack desk.  It can
even be somewhat old, this software supports some old versions of
these devices. Install TouchPortal from the Apple App or Android Play Store.
You can also use your smartphone, but the buttons will be pretty small.  Use 
the TouchPortal documentation, website, and Discord chat group if needed to get
your device connected to your PC, and confirm the default page buttons show up
on your tablet/smartphone device.  If your tablet is on the same WiFi network 
as your PC, the device app should automatically find your PC and connect after 
you install and start it up.  You should see the "(main)" page buttons show 
up on your device.

After TouchPortal is connected, download on your PC the 
N1MMCommands-TouchPortal-Release.tpp file from this GitHub project's page.
(Or download the sources and build yourself in VS 2019 or later if you want.)
https://github.com/frazierjason/N1mmCommands-TouchPortal/releases

Go to TouchPortal and click on the gear icon near the top right of the app. 
Select "Import plug-in...", then navigate to the .tpp file you just downloaded 
and load the .tpp file. Choose "Trust Always" when asked, then click OK to 
complete the plug-in installation.

*If you get a .NET error or command line error of some kind:*
You might need the obsolete .NET 5 Core libary, or a newer x86 version such as 
.NET 6 or 7 core.  Most modern Windows 10/11 installations already have it, so 
**Only install this if** you hit errors on the plugin's first start. Then you 
must quit and restart TouchPortal from the system tray icon and confirm the 
error doesn't reoccur.
https://aka.ms/dotnet-core-applaunch?framework=Microsoft.NETCore.App&framework_version=5.0.0&arch=x86&rid=win10-x86

After the plug-in is installed, download the Pages sample pack .zip file from 
the plugin releases page linked earlier above and unzip it on your PC. In 
TouchPortal, click on the "Pages" tab and then click the gear icon that is 
displayed just a few icons to the right of the page selection dropdown that 
probably says "(main)". In the Pages gear icon, select "Import page" and 
navigate to the location where you unzipped the Pages sample pack zip file.
Choose "N1MM Quick Buttons.tpz" to install this page. When TouchPortal asks if 
you want to see your new imported page, choose Yes.  Or you can select it from
the dropdown.  You'll see some new buttons added in this page.

*BEFORE YOU DO ANYTHING ELSE,* or add any more pages, you should right-click on
the "N1MM" logo'd button on this new page, and select Copy > Button.  Now click
on the Pages dropdown and select "(main)", then in any blank tile space on the 
(main) page, right click and select Paste > Button. This N1MM button you just 
pasted will allow you to get to the other pages.

Repeat the page import process for the other .tpz pages that you unzipped on 
your PC earlier.  Don't import the Rotor page if you don't need it. Also, 
the Radio Adjustment page contains CAT commands specific to certain rigs, so
you should select the indicated rig that most closely matches yours to get 
started, and then modify the CAT commands to your liking.  CAT command help 
is beyond the scope of this README file.  

At initial release, only one example Radio Adjustment Page is provided, which 
should work with most modern ICOM SDR transceivers. It is configurable for your
ICOM CI-V rig address by going to the "Values" tab and right-clicking the value 
named "N1MM CAT1 CIV ID (ICOM)".  By default it is set to "94" which is the 
usual value for an ICOM IC-7300. I don't have one of these, so I tested it 
with value "A4" on my IC-705.  More example pages matching other rigs will 
come as I get help from others.  I'll do one for Flex SmartSDR series as well.

You should have a bunch of buttons imported by now on several pages.  It should
be plenty enough to get started using it. Study the button implementations by 
clicking them on the desktop app and reviewing how they are made.  If you mess 
up a button, you can always delete and reimport a whole page if you don't mind 
losing your other changes. You can also create more pages and buttons yourself.

## PLEASE TEST THE FOLLOWING:

This alpha-build software offers two TouchPortal button actions at this time:

### Send any command via the Call Sign box

Ability to send an arbitrary message, expansion macro, or CAT sequence as if it
were typed into the Call Sign Entry window and pressed Enter.  This action 
is performed in the background, without affecting the Entry box, and the 
N1MM+ application is not brought to the foreground.  This should permit 
users to issue actions, send messages, use macros like {F8} to advance along
the QSO process, and process other macros like {WIPE}, {FREQUP}, {TURNROTOR}, 
{F8}, custom CAT rig commands, and digital mode macros. For more info, read:
https://n1mmwp.hamdocs.com/setup/function-keys

*Note about special non-macro action strings:*
CW, LSB, RTTY, COUNTYLINE, ROVERQTH, 14028, and similar non-macro words that 
invoke a special feature or action in N1MM+ are not supported by this feature.
You must type them yourself into the Entry window and hit Enter like usual,
or use the keypress simulator below to type in a sequence of keys for you.

### Simulate a keypress into the Entry Window

Ability to send a user-configured keystroke with Ctrl/Alt/Shift modifiers. 
This action will first call the correct N1MM+ Call Sign Entry window to the 
foreground, and then issue the keypress.  The plugin handles the keypress 
simulation directly, since TouchPortal does not know which of the N1MM+ 
windows should receive it.  The implementation eventually will take into 
account the two-radio scenario on the same workstation, but for now it is 
not yet fully implemented or tested, so please don't expect it to work. 
Any text box that has the cursor will receive the keys, not just the Entry
box. There is also a switch called "Allow in Background", which is not yet 
implemented and may never be, depending on how much can be accomplished 
by hams with the Send Message functionality (which works even if N1MM+ 
is backgrounded).  N1MM+ keystroke bindings are documented at:
https://n1mmwp.hamdocs.com/setup/keyboard-shortcuts/

### Sample pages are provided

An example set of pages are provided to accomplish the following:
- 

## Alpha build number 1 instructions and known issues:
- Only the happy path scenario should be expected to work:
  - Start N1MM+ and make sure your rig is connected and functioning
  - Start TouchPortal, install this plugin .tpp file if not already
  - Create button actions described further below and use them
  - Quit TouchPortal (TP) when finished, or stop this plugin in TP Settings
    if you have other TP needs and don't want to close TP.
  - N1MM+ can continue working, there is only loose integration via network
     messages.  This plugin does not share any files or resources with N1MM+.
- Plugin has no UI, only notifications delivered inside the TP application.
  Current notifications only tell if an invalid IP or port number is entered
  into this plugin's settings, located within the TP Settings menu.
- Plugin only does simple suppression of action attempts before N1MM+ has
  been started.
- Plugin is unware if the rig connection is busted, or if N1MM+ has quitted.
- Logic for handling dual radios (rigs) is very much alpha stage. Plugin can 
  track which rig is the active one and send commands there, but does not yet
  offer targeting a specific rig. Well-characterized bug reports are welcome.
- Plugin is unaware of any N1MM config states, it doesn't know your different 
  .ini files and can't detect what rigs are connected. It's up to you to make
  sure any rig-specific CAT commands are correct for your rig(s).  You can 
  create different TP pages for different rigs and switch between on your own.
- Plugin is unaware if N1MM+ is terminated and restarted with a different rig 
  setup.  Changing between single rig configs should not be an issue, but if 
  changing from a two-rig config to a one-rig config, the plugin will still 
  think there is a second rig until the plugin is restarted (or restart TP).
  There should not be any ill effect with this since the plugin doesn't
  offer rig selection or targeting an action to a specific rig, you simply 
  interact with whatever rig is in focus in N1MM+ already.
- Plugin comes configured by default for single-op single-rig setup using 
  UDP Broadcast messages enabled in N1MM.
- Occasionally N1MM+ will alert in the taskbar, but does not come to the 
  foreground as expected.  The keypress seems to be processed successfully.
  This was noticed when the TP app was in the foreground and being edited.
- You must set your broadcast IPs/ports adequately if you are in a multi-op  
  multi-PC network.  You're on your own for this, consult the N1MM+ forums.
- This plugin is designed for Windows only, using .NET 5 Core APIs. If there 
  is a large enough user base running N1MM+ in Wine or similar on Linux/Mac,
  efforts could be made to slightly adapt this plugin to start without issue.
- Plugin files are stored at this variable-expanding location
  `%APPDATA%\TouchPortal\plugins\N1mmCommands-TouchPortal\`
  (for example)
  C:\Users\Jason\AppData\Roaming\TouchPortal\plugins\N1mmCommands-TouchPortal\
  - Subfolder files are kept for one week in a "Log" subfolder here
  - If you want to increase logging, change line 12 "N1mmCommands" in the file 
    AppSettings.json from value "Warning" to instead use "Info" for more logs, 
    or "Debug" or "Verbose" for copious logs.  These are useful in bug reports.
- DEVELOPERS:  This solution was built on Visual Studio 2019 Community Edition,
  which is free to install and use from Microsoft if you select the Desktop 
  Applications option to build command line programs.  A future effort will 
  convert this app to VS2022 and a newer .NET Core, after confirming the user 
  base can handle using newer versions of .NET Core than the old version 5.0.
  DO NOT submit pull requests that convert the .sln or .csproj files to 
  VS2022 at this time.

## Contact and Support:
More to come, probably within the Github ticketing facilities once configured.
If you like my app, drop me a line by finding me on QRZ.com.

## Terms and licensing:

N1MM+ Logger logo image is adapted to TP UI with permission of Tom Wagner N1MM.

This free software is offered under terms of the MIT license (see included
"LICENSE.txt"), and also incorporates the following MIT-licensed softwares:

TouchPortal-CS-API, by Max Paperno
see license file "LICENSE-TouchPortal-CS-API.txt"
https://github.com/mpaperno/TouchPortal-CS-API

TouchPortalSDK.Extensions.Attributes, by Oddbjørn Bakke
see license file "LICENSE-TouchPortalSDKExtensions.txt"
https://github.com/oddbear/TouchPortalSDK.Extensions

MSFSTouchPortalPlugin, by Tim Lewis
see license file "LICENSE-MSFSTouchPortalPlugin.txt"
https://github.com/tlewis17/MSFSTouchPortalPlugin

InputSimulatorEx, by radj307 and michaelnoonan
see license file "LICENSE-InputSimulatorEx.txt"
https://github.com/radj307/inputsimulator

AssemblyAttribute, bu radj307 (used by InputSimulatorEx)
see license file "LICENSE-AssemblyAttribute.txt"
https://github.com/radj307/AssemblyAttribute
