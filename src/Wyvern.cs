using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace RSvg
{
    public class Wyvern : ModSystem
    {
        private GuiDialog dialog;
        private ICoreClientAPI capi;

        public override void StartClientSide(ICoreClientAPI api)
        {
            System.Diagnostics.Debug.WriteLine("What???");
            capi = api;
            dialog = new GuiNanosvg(capi);
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
    
    
    // AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA //
    public class GuiNanosvg : GuiDialog
    {
        public override string ToggleKeyCombinationCode => "annoyingtextgui";
        private LoadedTexture ownTexture;

        public GuiNanosvg(ICoreClientAPI capi) : base(capi)
        {
            SetupDialog();
        }

        private void SetupDialog()      // todo pass width,height as params, compute
        {
            IAsset svg = capi.Assets.Get(new AssetLocation("testdomain", "textures/test.svg"));
            System.Diagnostics.Debug.WriteLine(svg.Location.Path);

            IntPtr image = NanoSvg.NativeMethods.nsvgParse(svg.ToText(), "px", 96);     // todo get system dpi
            if (image == IntPtr.Zero)
            {
                System.Diagnostics.Debug.WriteLine("SVG READ FAILED");
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            IntPtr ras = NanoSvg.NativeMethods.nsvgCreateRasterizer();
            if (ras == IntPtr.Zero)
            {
                System.Diagnostics.Debug.WriteLine("RASTER INIT ERROR");
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            
            int num = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, num);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            
            // We need to get a pointer to our byte array to store our pixel data.
            // Using an unsafe context + pointers is better in terms of performance
            // as there is no need to copy memory, compared to marshalling values.
            unsafe
            {
                byte[] buffer = new byte[300*300*4]; // w*h*4
                fixed (byte* p = buffer)
                {
                    IntPtr ptr = (IntPtr) p;
                    NanoSvg.NativeMethods.nsvgRasterize(ras, image, 0,0,1, ptr, 300, 300, 300*4);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, 300, 300, 0, PixelFormat.Rgba, PixelType.UnsignedByte, ptr);
                }
            }

            // Close the image at least.
            // TODO: create and keep the rasterizer object somewhere, best used only once
            NanoSvg.NativeMethods.nsvgDelete(image);
            NanoSvg.NativeMethods.nsvgDeleteRasterizer(ras);

            ownTexture = new LoadedTexture(capi, num, 300, 300);
        }
        
        public override void OnRenderGUI(float deltaTime)
        {
            // Render2DLoadedTexture nicely takes the texture itself without the need to specify width and height
            capi.Render.Render2DLoadedTexture(ownTexture, 100, 100, 9999);
        }

        private void setupGui()     // todo wip, no touchie you wyvern >:(
        {
            var mainBounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.CenterFixed).WithFixedPosition(0.0, 100.0);  // 8
            var bkgrBounds = ElementBounds.Fill.WithFixedPadding(GuiStyle.ElementToDialogPadding);  // 7
            var listBounds = ElementBounds.Fixed(0.0, 0.0, 500.0, 580);  // 2
            var clipBounds = listBounds.ForkBoundingParent();   // 3
            var sideBounds = listBounds.FlatCopy().FixedGrow(6.0).WithFixedOffset(-3.0, -3.0);  // 4
            var scrlBounds = sideBounds.CopyOffsetedSibling(3.0 + listBounds.fixedWidth + 7.0).WithFixedWidth(20.0);    // 5
            var backBounds = ElementBounds.FixedSize(0.0, 0.0).FixedUnder(clipBounds, 15.0).WithAlignment(EnumDialogArea.LeftFixed).WithFixedPadding(20.0, 4.0).WithFixedAlignmentOffset(-6.0, 3.0);    // 10
            var exitBounds = ElementBounds.FixedSize(0.0, 0.0).FixedUnder(clipBounds, 18.0).WithAlignment(EnumDialogArea.RightFixed).WithFixedPadding(20.0, 4.0).WithFixedAlignmentOffset(2.0, 0.0);    // 6 
            
            //this.capi.Gui.CreateCompo("SVG Test", mainBounds)
            //    .AddHandbookStackList(); 
        }

    }
}


/*
this.capi.Gui.CreateCompo("handbook-overview", bounds8)
    .AddShadedDialogBG(bounds7)
    .AddDialogTitleBar(
        Lang.Get("Survival Handbook"), 
        Vintagestory.API.Common.Action(this.OnTitleBarClose))
    .AddVerticalTabs(
        this.genTabs(out curTab), 
        bounds9, 
        new Vintagestory.API.Common.Action<int, GuiTab>(this.OnTabClicked),
        "verticalTabs")
    .AddTextInput(
        bounds1, 
        new Vintagestory.API.Common.Action<string>(this.FilterItemsBySearchText), 
        CairoFont.WhiteSmallishText(), 
        "searchField")
    .BeginChildElements(bounds7)
    .BeginClip(bounds3)
    .AddInset(bounds4, 3)
    .AddHandbookStackList(
        bounds2, 
        new Vintagestory.API.Common.Action<int>(this.onLeftClickListElement), 
        this.shownHandbookPages, 
        "stacklist")
    .EndClip()
    .AddVerticalScrollbar(
        new Vintagestory.API.Common.Action<float>(this.OnNewScrollbarvalueOverviewPage), 
        bounds5, 
        "scrollbar")
    .AddSmallButton(
        Lang.Get("general-back"), 
        new ActionConsumable(this.OnButtonBack), 
        bounds10, 
        key: "backButton")
    .AddSmallButton(
        Lang.Get("general-close"), 
        new ActionConsumable(this.OnButtonClose), 
        bounds6)
    .EndChildElements()
    .Compose();
*/