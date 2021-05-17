using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

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

    public class GuiShowPreview : GuiDialog
    {
        public override string ToggleKeyCombinationCode => "annoyingtextgui";
        private float counter = 0;

        public GuiShowPreview(ICoreClientAPI capi) : base(capi)
        {
        }

        public LoadedTexture ownTexture { set; private get;  }
        
        public override void OnRenderGUI(float deltaTime)
        {
            counter += deltaTime;
            // Render2DLoadedTexture nicely takes the texture itself without the need to specify width and height
            if (ownTexture != null)
                capi.Render.Render2DLoadedTexture(ownTexture, 100 + counter*2, 100 + counter*2, 9999);
        }

        public override void OnMouseDown(MouseEvent args)
        {
            base.OnMouseDown(args);
            this.TryClose();
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

        // todo pass width,height as params, compute
        private void SetupDialog()
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

        private List<GuiHandbookTextIconPage> sections;
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
            
            // Texture is preview icon, LargeTexture is the actual preview when clicked.
            // Title appears on the list (Text does nothing rn)
            sections = new List<GuiHandbookTextIconPage> 
            {
                new GuiHandbookTextIconPage { pageCode = "a", Title = "Aaa", Text = "Aaaaa", Texture = ownTexture, LargeTexture = ownTexture},
                new GuiHandbookTextIconPage { pageCode = "b", Title = "Bbb", Text = "Bbbbbbb", Texture = ownTexture, LargeTexture = ownTexture},
                new GuiHandbookTextIconPage { pageCode = "c", Title = "Ccc", Text = "Cccc", Texture = ownTexture, LargeTexture = ownTexture},
                new GuiHandbookTextIconPage { pageCode = "d", Title = "Ddd", Text = "Dragon", Texture = ownTexture, LargeTexture = ownTexture},
                new GuiHandbookTextIconPage { pageCode = "e", Title = "Eee", Text = "Eeeeeeeeeee", Texture = ownTexture, LargeTexture = ownTexture},
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
                .AddHandbookStackList(listBounds, OnLeftClickListElement, sections.Cast<GuiHandbookPage>().ToList(), "stackList")
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
            GuiHandbookTextIconPage page = sections[index];
            if (page?.LargeTexture != null)
            {
                new GuiShowPreview(capi) {ownTexture = page.LargeTexture}.TryOpen();
            }
        }

        private void OnNewScrollbarvalueOverviewPage(float value)
        {
            GuiElementHandbookList handbookStackList = this.mainWindow.GetHandbookStackList("stackList");
            handbookStackList.insideBounds.fixedY = 3.0 - value;
            handbookStackList.insideBounds.CalcWorldBounds();
        }
    }

    
    public class GuiHandbookTextIconPage : GuiHandbookTextPage
    {
        public LoadedTexture LargeTexture;
        private LoadedTexture TextTexture;

        new public void Recompose(ICoreClientAPI capi)
        {
            this.TextTexture?.Dispose();
            this.TextTexture = new TextTextureUtil(capi).GenTextTexture(Lang.Get(this.Title), CairoFont.WhiteSmallText());
        }
        
        public override void RenderTo(ICoreClientAPI capi, double x, double y)
        {
            float num1 = (float) GuiElement.scaled(25.0);
            float num2 = (float) GuiElement.scaled(10.0);
            if (this.Texture != null)
                capi.Render.Render2DTexturePremultipliedAlpha(this.Texture.TextureId, x + (double) num2, y + (double) num1 / 4.0 - 3.0, (double) this.Texture.Width, (double) this.Texture.Height);
            if (this.TextTexture == null)
                this.Recompose(capi);
            capi.Render.Render2DTexturePremultipliedAlpha(this.TextTexture.TextureId, x + (double) num2 + TextTexture.Width + 5, y + (double) num1 / 4.0 - 3.0, (double) this.TextTexture.Width, (double) this.TextTexture.Height);
        }
    }
}
