using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Util;

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
            bool a = dialog.TryOpen();
            System.Diagnostics.Debug.WriteLine($"was opened?: ${a}" );
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
            const int width = 25;
            const int height = 25;
            const float scale = 0.025f;
            
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
                byte[] buffer = new byte[width*height*4]; // w*h*4
                fixed (byte* p = buffer)
                {
                    IntPtr ptr = (IntPtr) p;
                    NanoSvg.NativeMethods.nsvgRasterize(ras, image, 0,0,scale, ptr, width, height, width*4);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, ptr);
                }
            }

            // Close the image at least.
            // TODO: create and keep the rasterizer object somewhere, best used only once
            NanoSvg.NativeMethods.nsvgDelete(image);
            NanoSvg.NativeMethods.nsvgDeleteRasterizer(ras);

            ownTexture = new LoadedTexture(capi, num, width, height);

            try
            {
                SetupGui();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"bruh: ${e}" );
            }
        }
        
        public override void OnRenderGUI(float deltaTime)
        {
            // Render2DLoadedTexture nicely takes the texture itself without the need to specify width and height
            // capi.Render.Render2DLoadedTexture(ownTexture, 100, 100, 9999);
            this.SingleComposer = this.mainWindow;
            base.OnRenderGUI(deltaTime);
        }


        private GuiComposer mainWindow;
        
        private void SetupGui()     // todo wip, no touchie you wyvern >:(
        {
            var mainBounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.CenterFixed).WithFixedPosition(0.0, 100.0);  // 8
            var bkgrBounds = ElementBounds.Fill.WithFixedPadding(GuiStyle.ElementToDialogPadding);  // 7
            var listBounds = ElementBounds.Fixed(GuiStyle.ElementToDialogPadding - 2.0, 50.0, 500.0, 580);  // 2
            var clipBounds = listBounds.ForkBoundingParent();   // 3
            var sideBounds = listBounds.FlatCopy().FixedGrow(6.0).WithFixedOffset(-3.0, -3.0);  // 4
            var scrlBounds = sideBounds.CopyOffsetedSibling(3.0 + listBounds.fixedWidth + 7.0).WithFixedWidth(20.0);    // 5
            var quitBounds = ElementBounds.FixedSize(0.0, 0.0).FixedUnder(clipBounds, 18.0).WithAlignment(EnumDialogArea.RightFixed).WithFixedPadding(20.0, 4.0).WithFixedAlignmentOffset(2.0, 0.0);    // 6 
            
            bkgrBounds.BothSizing = ElementSizing.FitToChildren;
            bkgrBounds.WithChildren(sideBounds, listBounds, scrlBounds, quitBounds);
            
            System.Diagnostics.Debug.WriteLine("aAA");
            
            var sections = new List<GuiHandbookPage>
            {
                new GuiHandbookTextIconPage { pageCode = "a", Title = "Aaa", Text = "Aaaaa", Texture = ownTexture},
                new GuiHandbookTextIconPage { pageCode = "b", Title = "Bbb", Text = "Bbbbbbb", Texture = ownTexture},
                new GuiHandbookTextIconPage { pageCode = "c", Title = "Ccc", Text = "Cccc", Texture = ownTexture},
                new GuiHandbookTextIconPage { pageCode = "d", Title = "Ddd", Text = "Dragon", Texture = ownTexture},
                new GuiHandbookTextIconPage { pageCode = "e", Title = "Eee", Text = "Eeeeeeeeeee", Texture = ownTexture},
            };

            int count = 0;
            foreach (var page in sections)
            {
                if (page is GuiHandbookTextIconPage textPage)
                {
                    System.Diagnostics.Debug.WriteLine("Init ");
                    textPage.Init(capi);
                    textPage.PageNumber = count;
                    count++;
                }
            }

            System.Diagnostics.Debug.WriteLine("bBB");
            this.mainWindow = this.capi.Gui.CreateCompo("svgTest", mainBounds)
                .AddShadedDialogBG(bkgrBounds)
                .AddDialogTitleBar("SVG Sample", OnTitleBarClose)
                .BeginChildElements(bkgrBounds)
                .BeginClip(clipBounds)
                .AddInset(sideBounds, 3)
                .AddHandbookStackList(listBounds, OnLeftClickListElement, sections, "stackList")
                .EndClip()
                .AddVerticalScrollbar(OnNewScrollbarvalueOverviewPage, scrlBounds, "scrollbar")
                .AddSmallButton(Lang.Get("general-close"), TryClose, quitBounds)
                .EndChildElements()
                .Compose();
            
            System.Diagnostics.Debug.WriteLine("C");
            
            this.mainWindow.GetScrollbar("scrollbar")
                .SetHeights(580, (float) this.mainWindow.GetHandbookStackList("stackList").insideBounds.fixedHeight);
        }

        private void OnTitleBarClose() => this.TryClose();

        private void OnLeftClickListElement(int index)
        {
            
        }
        
        private void OnNewScrollbarvalueOverviewPage(float value)
        {
            GuiElementHandbookList handbookStackList = this.mainWindow.GetHandbookStackList("stackList");
            handbookStackList.insideBounds.fixedY = 3.0 - value;
            handbookStackList.insideBounds.CalcWorldBounds();
        }
    }
    
    
    public class GuiHandbookTextIconPage : GuiHandbookPage
  {
    public string pageCode;
    public string Title;
    public string Text;
    public string categoryCode = "guide";
    public LoadedTexture Texture;
    private LoadedTexture TextTexture;
    private RichTextComponentBase[] comps;
    public int PageNumber;
    private string titleCached;

    public override string PageCode => this.pageCode;

    public override string CategoryCode => this.categoryCode;

    public override void Dispose()
    {
      this.Texture?.Dispose();
      this.Texture = (LoadedTexture) null;
    }

    public void Init(ICoreClientAPI capi)
    {
      if (this.Text.Length < (int) byte.MaxValue)
        this.Text = Lang.Get(this.Text);
      this.comps = VtmlUtil.Richtextify(capi, this.Text, CairoFont.WhiteSmallText().WithLineHeightMultiplier(1.2));
      this.titleCached = Lang.Get(this.Title);
    }

    public override RichTextComponentBase[] GetPageText(
      ICoreClientAPI capi,
      ItemStack[] allStacks,
      ActionConsumable<string> openDetailPageFor)
    {
      return this.comps;
    }

    public void Recompose(ICoreClientAPI capi)
    {
      this.TextTexture?.Dispose();
      this.TextTexture = new TextTextureUtil(capi).GenTextTexture(Lang.Get(this.Title), CairoFont.WhiteSmallText());
    }

    public override float TextMatchWeight(string searchText)
    {
      if (this.titleCached.Equals(searchText, StringComparison.InvariantCultureIgnoreCase))
        return 3f;
      if (this.titleCached.StartsWith(searchText, StringComparison.InvariantCultureIgnoreCase))
        return 2.5f;
      if (this.titleCached.CaseInsensitiveContains(searchText))
        return 2f;
      return this.Text.CaseInsensitiveContains(searchText) ? 1f : 0.0f;
    }

    public override void RenderTo(ICoreClientAPI capi, double x, double y)
    {
      float num1 = (float) GuiElement.scaled(25.0);
      float num2 = (float) GuiElement.scaled(10.0);
      if (this.TextTexture == null)
        this.Recompose(capi);
      capi.Render.Render2DTexturePremultipliedAlpha(this.Texture.TextureId, x + (double) num2, y + (double) num1 / 4.0 - 3.0, (double) this.Texture.Width, (double) this.Texture.Height);
      capi.Render.Render2DTexturePremultipliedAlpha(this.TextTexture.TextureId, x + (double) num2 + Texture.Width + 5, y + (double) num1 / 4.0 - 3.0, (double) this.Texture.Width, (double) this.Texture.Height);
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