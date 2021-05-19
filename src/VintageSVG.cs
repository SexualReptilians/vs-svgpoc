﻿using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace SVGPoc
{
    public class VintageSVG : ModSystem
    {
        private GuiDialog dialog;

        public override void StartClientSide(ICoreClientAPI capi)
        {
            dialog = new GuiSvgSamples(capi);
            capi.Input.RegisterHotKey("svgtoggle", "svgtoggle", GlKeys.U, HotkeyType.GUIOrOtherControls);
            capi.Input.SetHotKeyHandler("svgtoggle", OpenGui);

            capi.Event.IsPlayerReady += Event_IsPlayerReady;
        }

        private bool OpenGui(KeyCombination t1)
        {
            dialog?.TryOpen();
            return true;
        }

        private bool Event_IsPlayerReady(ref EnumHandling handling)
        {
            return OpenGui(null);
        }
    }
}
