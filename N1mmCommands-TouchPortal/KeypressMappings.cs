﻿/* This project published at https://github.com/frazierjason/N1mmCommands-TouchPortal
 * under the MIT license.
 */

using System;
using System.Collections.Generic;
using InputSimulatorEx.Native;

namespace N1mmCommands.Touchportal
{
	public class KeypressMappings
	{
        // these must be matched in value and sequence to the following list im Entry.tp under:
        //  categories.id -> n1mm.commands.tp.sendCommand
        //      actions.id -> n1mm.commands.tp.sendKeys
        //          data.id -> n1mm.commands.tp.sendKeys.press.Data.vk

        // InputSimulatorEx is based on these mappings:
        // https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes
        public static readonly Dictionary<string, VirtualKeyCode> VIRTUALKEY = new()
        {
            //{"NO KEY (just show N1MM+)", VirtualKeyCode.VK_ },  LOOKUP FALLS THROUGH IN CODE TO NOT SEND A KEY
            {"Escape",                   VirtualKeyCode.ESCAPE },
            {"Arrow Up",                 VirtualKeyCode.UP },
            {"Arrow Down",               VirtualKeyCode.DOWN },
            {"Back-quote or Grave `",    VirtualKeyCode.VK_3 },
            {"Backslash \\",             VirtualKeyCode.OEM_5 },
            {"Enter",                    VirtualKeyCode.RETURN },
            {"Equal =+",                    VirtualKeyCode.OEM_PLUS },
            {"Foward Slash /",           VirtualKeyCode.OEM_2 },
            {"Insert",                   VirtualKeyCode.INSERT },
            {"Minus -_",                    VirtualKeyCode.OEM_MINUS },
            {"Page Up",                  VirtualKeyCode.PRIOR },
            {"Page Down",                VirtualKeyCode.NEXT },
            {"Pause",                    VirtualKeyCode.PAUSE },
            {"Quote '",                    VirtualKeyCode.OEM_7 },
            {"Semicolon ;",                VirtualKeyCode.OEM_1 },
            {"Space Bar",                VirtualKeyCode.SPACE },
            {"Ten-Key Add (Plus)",        VirtualKeyCode.ADD },
            {"Ten-Key Subtract (Minus)",  VirtualKeyCode.SUBTRACT },
            {"A",                        VirtualKeyCode.VK_A },
            {"B",                        VirtualKeyCode.VK_B },
            {"C",                        VirtualKeyCode.VK_C },
            {"D",                        VirtualKeyCode.VK_D },
            {"E",                        VirtualKeyCode.VK_E },
            {"F",                        VirtualKeyCode.VK_F },
            {"G",                        VirtualKeyCode.VK_G },
            {"H",                        VirtualKeyCode.VK_H },
            {"I",                        VirtualKeyCode.VK_I },
            {"J",                        VirtualKeyCode.VK_J },
            {"K",                        VirtualKeyCode.VK_K },
            {"L",                        VirtualKeyCode.VK_L },
            {"M",                        VirtualKeyCode.VK_M },
            {"N",                        VirtualKeyCode.VK_N },
            {"O",                        VirtualKeyCode.VK_O },
            {"P",                        VirtualKeyCode.VK_P },
            {"Q",                        VirtualKeyCode.VK_Q },
            {"R",                        VirtualKeyCode.VK_R },
            {"S",                        VirtualKeyCode.VK_S },
            {"T",                        VirtualKeyCode.VK_T },
            {"U",                        VirtualKeyCode.VK_U },
            {"V",                        VirtualKeyCode.VK_V },
            {"W",                        VirtualKeyCode.VK_W },
            {"X",                        VirtualKeyCode.VK_X },
            {"Y",                        VirtualKeyCode.VK_Y },
            {"Z",                        VirtualKeyCode.VK_Z },
            {"1",                        VirtualKeyCode.VK_1 },
            {"2",                        VirtualKeyCode.VK_2 },
            {"3",                        VirtualKeyCode.VK_3 },
            {"4",                        VirtualKeyCode.VK_4 },
            {"5",                        VirtualKeyCode.VK_5 },
            {"6",                        VirtualKeyCode.VK_6 },
            {"7",                        VirtualKeyCode.VK_7 },
            {"8",                        VirtualKeyCode.VK_8 },
            {"9",                        VirtualKeyCode.VK_9 },
            {"0",                        VirtualKeyCode.VK_0 },
            {"F1",                       VirtualKeyCode.F1 },
            {"F2",                       VirtualKeyCode.F2 },
            {"F3",                       VirtualKeyCode.F3 },
            {"F4",                       VirtualKeyCode.F4 },
            {"F5",                       VirtualKeyCode.F5 },
            {"F6",                       VirtualKeyCode.F6 },
            {"F7",                       VirtualKeyCode.F7 },
            {"F8",                       VirtualKeyCode.F8 },
            {"F9",                       VirtualKeyCode.F9 },
            {"F10",                      VirtualKeyCode.F10 },
            {"F11",                      VirtualKeyCode.F11 },
            {"F12",                      VirtualKeyCode.F12 },
            { "Scroll Lock",             VirtualKeyCode.SCROLL },
            { "Comma",                   VirtualKeyCode.OEM_COMMA },
            {"Period",                   VirtualKeyCode.OEM_PERIOD },
            {"Left Square Brace [",      VirtualKeyCode.OEM_4 },
            {"Right Square Brace ]",     VirtualKeyCode.OEM_6 }
        };
		public KeypressMappings()
		{
		}
    }
}