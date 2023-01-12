# N1MM+ Commands for Touch Portal
### by Jason Frazier W7DM

(Stream Deck plugin planned in the future, see my other projects)

This TouchPortal plugin integrates the N1MM Logger+ app by Tom Wagner N1MM, 
allowing the user to create custom buttons in TouchPortal to control the popular
N1MM+ Logger amateur radio contesting application for Windows.

Custom button actions can currently perform these functions:
- Execute macros directly in N1MM+, including CAT rig control behaviors
- Transmit CW content and SSB wav files, which can also contain macros
- Summon N1MM+ into the foreground and send arbitrary N1MM+ keystroke commands, 
  including Ctrl/Alt/Shift

This is a work-in-progress plugin, created specifically with contesters in mind,
in a way that eases multitasking on the radio operator's shack PC while quickly 
allowing to regain control over your contest activity.  

DISCLAIMER:  This plugin author takes no responsibility for anything you do with
the plugin, including but not limited to PC issues, radio issues, equipment 
damage, etc.  It is simply an extension of what you can already do with N1MM+ to
drive your radio station's automated controls.

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

#### Prerequisites:
- Windows 64-bit PC installation (no 32 bit) that already runs N1MM+ well enough
  to control your rig in successful contesting activities
- An iPad, Android tablet that has an account signed into its App/Play store and
  can make in-app purchases (iPhone/Android phones work too, w/ smaller buttons)
- Good WiFi connection shared by both your PC and your tablet, or optionally 
  use a USB cable for Android without any WiFi
- Willingness to make a one-time purchase in Apple App store or Google Play 
  store, about $14 USD in January 2023, for TouchPortal's Pro Upgrade that 
  enables plugin usage
  - Buy it twice if you want both iPad and Android tablet
  - I don't get any of this money, it goes to TouchPortal

Start by first enabling a Broadcast Data feature in N1MM+ application:
- In N1MM+, go to the Config menu and select "Configure Ports, Mode Control, Winkey, etc ..."
- Wait for the config window to appear. Click on the Broadcast Data tab near the far right.
- Ensure to check the box called "Radio"
  - If there is no text in the textbox to the right, set it to contain the phrase 
    127.0.0.1:12060 with no spaces in it. This should already be the default though.
- If the checkbox was ALREADY enabled when you got there:
  - Consider why this is the case. Are you an advanced user? Do you already have any other 
    apps integrated with N1MM+?  If so, you need to add into the right hand text box:  a 
    space, followed by another phrase like 127.0.0.1:12061, or change the last digit or two 
    a bit higher in numeric value.  Write this higher number down that you chose.
- Click OK to save.  We'll come back to N1MM+ later if needed.

Next, we'll install TouchPortal for Windows from:

https://www.touch-portal.com/#downloadstitle

The installer defaults are fine. There is a step to install ADB USB drivers, 
which come from Google and only run if you connect Android over USB (not WiFi).

![A cat](https://github.com/frazierjason/N1mmCommands-TouchPortal/blob/a4e312141209ae65b25b7cf9186aa80c9a13b962/tp-installing.jpg)

Reboot your PC after installation of TouchPortal, then start up TouchPortal.

After rebooting, go to Windows Start menu and start Touch Portal.  You should 
see a Windows Advanced Firewall dialog come up asking your permission to allow
Touch Portal to receive connections.  **CLICK ACCEPT**, or you will not be able
to make connections from your tablet to your PC.

(screenshot of firewall prompt TBD)

Upon starting TouchPortal, complete the setup wizard as follows:

![A cat](https://github.com/frazierjason/N1mmCommands-TouchPortal/blob/a4e312141209ae65b25b7cf9186aa80c9a13b962/tp-setup-1.JPG)

![A cat](https://github.com/frazierjason/N1mmCommands-TouchPortal/blob/a4e312141209ae65b25b7cf9186aa80c9a13b962/tp-setup-2.JPG)

![A cat](https://github.com/frazierjason/N1mmCommands-TouchPortal/blob/a4e312141209ae65b25b7cf9186aa80c9a13b962/tp-setup-3.JPG)

![A cat](https://github.com/frazierjason/N1mmCommands-TouchPortal/blob/a4e312141209ae65b25b7cf9186aa80c9a13b962/tp-setup-4.JPG)

![A cat](https://github.com/frazierjason/N1mmCommands-TouchPortal/blob/a4e312141209ae65b25b7cf9186aa80c9a13b962/tp-setup-5.JPG)

![A cat](https://github.com/frazierjason/N1mmCommands-TouchPortal/blob/a4e312141209ae65b25b7cf9186aa80c9a13b962/tp-setup-6.JPG)

After starting up, you will have an empty "(main)" page with no button tiles.

<img src=https://github.com/frazierjason/N1mmCommands-TouchPortal/blob/a4e312141209ae65b25b7cf9186aa80c9a13b962/tp-first-startup.JPG width=00/>

TouchPortal is now set up.  If you close it, it's still running in the system
tray next to the Windows clock.  You can reopen/restart/exit from there.

By default TouchPortal does not start automatically. If you want, you can enable
auto startup in the application's settings menus.

![A cat](https://github.com/frazierjason/N1mmCommands-TouchPortal/blob/a4e312141209ae65b25b7cf9186aa80c9a13b962/tp-open-settings.JPG)

![A cat](https://github.com/frazierjason/N1mmCommands-TouchPortal/blob/e60efad44559207c8d1292b9de38d77ef9d70cba/tp-settings-scrolldown.jpg)

![A cat](https://github.com/frazierjason/N1mmCommands-TouchPortal/blob/e60efad44559207c8d1292b9de38d77ef9d70cba/tp-settings-enable-autostart.jpg)

On your tablet, double-check that you are connected to the same WiFi as your PC.

Search for "TouchPortal" in the Apple App or Android Play Store. Install the 
app onto your tablet.

(screenshot for store search result TBD)

If your tablet is on the same WiFi network as your PC, the device app should 
automatically find your PC and connect after you install and start it up.  You 
should see any "(main)" page button show up on your device (there are none set 
up so far, so the tablet app will look fairly empty).

(screenshot of empty looking tablet TP page)

If your tablet cannot connect to your PC, it will prompt an error. Please refer 
to the FAQ link below, or the "Guides & Help" materials on TouchPortal's 
website to fix the issue and get your tablet connected to your PC, before 
performing any further steps or buying the upgrade.

https://www.touch-portal.com/faq.php?faqId=touch-portal-cannot-connect

After TouchPortal is connected, look for the shopping cart icon near the top
right area of the app UI.  Tap that and look for the "Pro Upgrade", which you 
need to purchase in order to use any plugins with TouchPortal. Be ready to pay 
and sign into the App Store or Play Store for your purchase. The license is 
perpetual one-time, good for all your devices that use that same store account.

(screenshots of finding the Pro Upgrade button on the tablet)

On your PC, go to this Github project's Releases page and download the latest 
released version of "N1mmCommands-TouchPortal-win-x64-Release.tpp" file. 
You'll also want to download the latest "N1MM-Pages-Pack.zip" file. Remember 
where you downloaded these files, usually in your Downloads folder.

https://github.com/frazierjason/N1mmCommands-TouchPortal/releases

In TouchPortal on your PC, click the gear icon at the top right of the app. 
Select "Import plug-in...", then navigate to the .tpp file you just downloaded 
and load the .tpp file. Choose "Trust Always" when asked, then click OK to 
complete the plug-in installation.

![A cat](https://github.com/frazierjason/N1mmCommands-TouchPortal/blob/1d6f2d12794f7580d13c18ed9582d503ddcf0cf5/tp-open-import-plugin.jpg)

![A cat](https://github.com/frazierjason/N1mmCommands-TouchPortal/blob/1d6f2d12794f7580d13c18ed9582d503ddcf0cf5/tp-import-plugin.jpg)

![A cat](https://github.com/frazierjason/N1mmCommands-TouchPortal/blob/1d6f2d12794f7580d13c18ed9582d503ddcf0cf5/tp-import-plugin-tpp.jpg)

![A cat](https://github.com/frazierjason/N1mmCommands-TouchPortal/blob/1d6f2d12794f7580d13c18ed9582d503ddcf0cf5/tp-importing-plugin-tpz.jpg)

Immediately after installing the plugin, you will get another Firewall prompt,
this time allowing the plugin to communicate with both N1MM+ and TouchPortal.
You must accept this firewall prompt or the plugin will not function.

![A cat](https://github.com/frazierjason/N1mmCommands-TouchPortal/blob/1d6f2d12794f7580d13c18ed9582d503ddcf0cf5/tp-installing-plugin-trust-always.jpg)

![A cat](https://github.com/frazierjason/N1mmCommands-TouchPortal/blob/1d6f2d12794f7580d13c18ed9582d503ddcf0cf5/tp-import-plugin-success.jpg)

After the plug-in is installed, it's running but not doing much for you yet.

If at the start of these instructions, your N1MM+ Broadcast Data "Radio" text
value had the default of 127.0.0.1:12060, and you are not using any other kind 
of integrated app with N1MM+, you can skip the next one paragraph and three screenshots.

In this paragraph, we reconfigure the plugin for users that have multiple 
integrated apps with N1MM+ that receive Radio UDP data over the network. 
In TouchPortal for Windows, click the top right gear, then Settings, and in 
the Settings UI, click Plugins near bottom left. Select this plugin in the 
dropdown on right, and look for the setting named "N1MM+ RadioInfo Broadcast 
Port". Change its default value of 12060 to your alternate port 12061 (or 
whatever new number you added on earlier). Click Save.  You're done 
reconfiguring the network port.

![A cat](https://github.com/frazierjason/N1mmCommands-TouchPortal/blob/a4e312141209ae65b25b7cf9186aa80c9a13b962/tp-open-settings.JPG)

![A cat](https://github.com/frazierjason/N1mmCommands-TouchPortal/blob/1d6f2d12794f7580d13c18ed9582d503ddcf0cf5/tp-settings-reveal-plugin.jpg)

![A cat](https://github.com/frazierjason/N1mmCommands-TouchPortal/blob/1d6f2d12794f7580d13c18ed9582d503ddcf0cf5/tp-plugin-settings-defaults.jpg)

Open a Windows Explorer window and navigate to where your downloaded the plugin
files earlier (probably Downloads folder). Extract the "N1MM-Pages-Pack.zip" 
file that you downloaded.  If you don't have any extractor software, you can
double click on the .zip file to open it in Windows Explorer, then select all of
the .tpz files inside, right click, and select Copy.  Now nagivate back to your 
Downloads folder and press Ctrl-V to paste all of the .tpz files. *You have to
extract these files to a folder, TouchPortal cannot open the .zip file itself.*

(screenshot of a folder with some .tpz files)

In TouchPortal, click on the "Pages" tab and then click the gear icon that is 
displayed just a few icons to the right of the page selection dropdown that 
probably says "(main)". In the Pages gear icon, select "Import page".

(screenshot of opening the Page Import dialog)

Navigate to the location where you unzipped the Pages sample pack zip file.
Choose "N1MM-Quick-Buttons.tpz" to install this page. When TouchPortal asks if 
you want to see your new imported page, choose Yes.  Or you can select it from
the dropdown.  You'll see some new buttons added in this page.

(screenshots of importing the plugin and accepting dialogs)

(screenshot of the Quick Buttons page loaded up)

*BEFORE YOU DO ANYTHING ELSE,* or add any more pages, you should right-click on
the "N1MM" logo'd button on this new page, and select Copy > Button.  Now click
on the Pages dropdown and select "(main)", then in any blank tile space on the 
(main) page, right click and select Paste > Button. This N1MM button you just 
pasted will allow you to get to the other pages.

(screenshots of copy-pasting the N1MM button over to the empty (main page))

Repeat the page import process for these additional .tpz pages that you unzipped
on your PC earlier:
- N1MM-Function-Keys.tpz
- N1MM-Frequency-Control.tpz
- N1MM-Run-Page.tpz

You should now have all the basic sample button pages loaded, to control common
actions in N1MM+ that generally apply to most connected rigs.

(screenshot of the page list dropdown list)

(screenshots of the remaining pages)

IF YOU HAVE A NON-ICOM RIG that accepts CAT controls for N1MM+ to send commands 
to the radio:

It is possible to create buttons that control your rig via CAT commands. You can
review the below information and page on how it's done for an ICOM SDR rig, and
try to figure out how to create similar CAT actions for your own radio model. IF
the ham community has suitable examples tested and offered up to share, I can 
add them to this project for others' benefit and ease of use.

IF YOU HAVE AN ICOM SDR TRANSCEIVER on Radio 1 in N1MM+:

You can install the page "N1MM-Radio-Adjustments-RIG1-ICOM-SDRs.tpz" for more 
advanced direct control of your rig features through N1MM+.  After installing 
the page, change the ICOM CI-V rig address in the "Values" tab to match your 
rig. Right-click the value named "N1MM CAT1 CIV ID (ICOM)". Change it to the two
character code matching your ICOM rig settings.  Do not mess with any of the
other values, they are updated live by the page buttons as they are used.

(screenshot opening the Values tab in TouchPortal)

(screenshot of the CI-V value being changed)

The Radio Adjustment page contains CAT commands specific to certain rigs, so
you should select the indicated rig that most closely matches yours to get 
started, and then modify the CAT commands to your liking.  CAT command help 
is beyond this guide's scope. Initially, only an ICOM SDR CAT sample is created.

(screenshot of the ICOM radio adjustment page with all question marks)

You'll see that most of the buttons have pairs of question marks.  The plugin 
does not know the radio state for most of these buttons. What happens is, you 
press the button you want to toggle, and it tells the radio (via N1MM+) to go 
to the newly displayed status.  If you change the radio directly on its front 
panel, the page buttons will not update to match. The next time you use the page
button, it will again force the radio setting to match whatever shows after you 
press the desired page button.

(screenshot of half the radio adjustment page buttons populated)

(stacked screenshot of cycling a button through its different states)

You should have a bunch of buttons imported by now on several pages.  It should
be plenty enough to get started using it. Study the button implementations by 
clicking them on the desktop app and reviewing how they are made.  If you mess 
up a button, you can always delete and reimport a whole page if you don't mind 
losing your other changes. Or you can rename the messed-up page and reimport 
the original, then transfer over the buttons you want to recover. You can also 
create more pages and buttons yourself. It's a good idea to export your own page
changes once you have something working that you're happy with, in case you need
to recover your custom page changes later.

(screenshot opening the page settings, click export, name the export)

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



## Alpha builds instructions and known issues:
- Active and closed issues are reported on the project Issues page:
  https://github.com/frazierjason/N1mmCommands-TouchPortal/issues
- Only the happy path scenario should be expected to work:
  - Start N1MM+ and make sure your rig is connected and functioning
  - Start TouchPortal, install this plugin .tpp file if not already
  - Create button actions described further below and use them
  - Quit TouchPortal (TP) when finished, or stop this plugin in TP Settings
    if you have other TP needs and don't want to close TP.
  - N1MM+ can continue working, there is only loose integration via network
     messages.  This plugin does not share any files or resources with N1MM+.
- Plugin comes configured by default for single-op single-rig setup using 
  UDP Broadcast messages enabled in N1MM.
- You must set your broadcast IPs/ports adequately if you are in a multi-op  
  multi-PC network.  You're on your own for this, consult the N1MM+ forums.
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
