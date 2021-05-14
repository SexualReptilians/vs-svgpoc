using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace RSvg
{
    public class Wyvern : ModSystem
    {
        private GuiDialogAnnoyingText dialog;
        private ICoreClientAPI capi;

        public override void StartClientSide(ICoreClientAPI api)
        {
            System.Diagnostics.Debug.WriteLine("What???");
            capi = api;
            dialog = new GuiDialogAnnoyingText(capi);
            capi.Event.IsPlayerReady += Event_IsPlayerReady;
        }

        private bool Event_IsPlayerReady(ref EnumHandling handling)
        {
            dialog.TryOpen();
            return true;
        }
    }
    
    public class GuiDialogAnnoyingText : GuiDialog
    {
        public override string ToggleKeyCombinationCode => "annoyingtextgui";
        private LoadedTexture ownTexture;

        public GuiDialogAnnoyingText(ICoreClientAPI capi) : base(capi)
        {
            SetupDialog();
        }

        private void SetupDialog()
        {
            Cairo.ImageSurface imageSurface = new Cairo.ImageSurface(Cairo.Format.Argb32, 200, 200);
            Cairo.Context ctx = new Cairo.Context(imageSurface);

            IAsset svg = capi.Assets.Get(new AssetLocation("testdomain", "textures/test.svg"));
            System.Diagnostics.Debug.WriteLine(svg.Location.Path);

            IntPtr error;
            IntPtr handle = NativeMethods.rsvg_handle_new_from_data(svg.Data, svg.Data.Length, out error);
            
            if (error != IntPtr.Zero)
            {
                System.Diagnostics.Debug.WriteLine("FAILED");
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            NativeMethods.rsvg_handle_render_cairo(handle, ctx.Handle);

            System.Diagnostics.Debug.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");

            ownTexture = new LoadedTexture(capi, capi.Gui.LoadCairoTexture(imageSurface, true), 200, 200);
        }

        public override void OnRenderGUI(float deltaTime)
        {
            // Render2DLoadedTexture nicely takes the texture itself without the need to specify width and height
            capi.Render.Render2DLoadedTexture(ownTexture, 100, 100, 9999);
        }
    }
}