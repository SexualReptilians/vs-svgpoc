using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace SVGPoc
{
    public class GuiSvgSamples : GuiDialog
    {
        public override string ToggleKeyCombinationCode => "SVG PoC window toggle";
        private LoadedTexture ownTexture;
        private readonly IntPtr rasterizer;

        public GuiSvgSamples(ICoreClientAPI capi) : base(capi)
        {
            this.rasterizer = NanoSvg.NativeMethods.nsvgCreateRasterizer();
            if (this.rasterizer == IntPtr.Zero)
            {
                System.Diagnostics.Debug.WriteLine("RASTER INIT ERROR");
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            
            SetupDialog();
            
            try
            {
                SetupGui();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"bruh: ${e}" );
            }
        }

        // todo get system dpi
        // if scale == 0.0f, compute scale based on passed w/h and svg w/h 
        private void SetupDialog(int width = 25, int height = 25, float scale = 1.0f, float offX = 0, float offY = 0, float dpi = 96)
        {
            IAsset svg = this.capi.Assets.Get(new AssetLocation("testdomain", "textures/test.svg"));
            System.Diagnostics.Debug.WriteLine(svg.Location.Path);

            IntPtr image = NanoSvg.NativeMethods.nsvgParse(svg.ToText(), "px", dpi);
            if (image == IntPtr.Zero)
            {
                System.Diagnostics.Debug.WriteLine("SVG READ FAILED");
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
                    NanoSvg.NativeMethods.nsvgRasterize(this.rasterizer, image, offX,offY,scale, ptr, width, height, width*4);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, ptr);
                }
            }

            NanoSvg.NativeMethods.nsvgDelete(image);

            this.ownTexture = new LoadedTexture(this.capi, num, width, height);
        }

        public override void OnGuiClosed()
        {
            base.OnGuiClosed();
            NanoSvg.NativeMethods.nsvgDeleteRasterizer(this.rasterizer);
        }

        public override void OnRenderGUI(float deltaTime)
        {
            this.SingleComposer = this.mainWindow;
            base.OnRenderGUI(deltaTime);
        }

        private List<GuiHandbookPage> sections;
        private GuiComposer mainWindow;
        
        private void SetupGui()
        {
            // Texture is preview icon, LargeTexture is the actual preview when clicked.
            // Title appears on the list (Text does nothing rn)
            this.sections = new List<GuiHandbookPage> 
            {
                new GuiHandbookTextIconPage { Title = "Aaa", Texture = this.ownTexture, LargeTexture = this.ownTexture},
                new GuiHandbookTextIconPage { Title = "Bbb", Texture = this.ownTexture, LargeTexture = this.ownTexture},
                new GuiHandbookTextIconPage { Title = "Ccc", Texture = this.ownTexture, LargeTexture = this.ownTexture},
                new GuiHandbookTextIconPage { Title = "Ddd", Texture = this.ownTexture, LargeTexture = this.ownTexture},
                new GuiHandbookTextIconPage { Title = "Eee", Texture = this.ownTexture, LargeTexture = this.ownTexture},
            };

            var count = 0;
            foreach (var page in this.sections)
            {
                if (!(page is GuiHandbookTextIconPage textPage)) continue;
                textPage.Init(this.capi);
                textPage.PageNumber = count;
                textPage.pageCode = $"{count}";
                count++;
            }

            var mainBounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.CenterFixed).WithFixedPosition(0.0, 100.0);  // 8
            var bkgrBounds = ElementBounds.Fill.WithFixedPadding(GuiStyle.ElementToDialogPadding);  // 7
            var listBounds = ElementBounds.Fixed(GuiStyle.ElementToDialogPadding - 2.0, 50.0, 500.0, 580);  // 2
            var clipBounds = listBounds.ForkBoundingParent();   // 3
            var sideBounds = listBounds.FlatCopy().FixedGrow(6.0).WithFixedOffset(-3.0, -3.0);  // 4
            var scrlBounds = sideBounds.CopyOffsetedSibling(3.0 + listBounds.fixedWidth + 7.0).WithFixedWidth(20.0);    // 5
            var quitBounds = ElementBounds.FixedSize(0.0, 0.0).FixedUnder(clipBounds, 18.0).WithAlignment(EnumDialogArea.RightFixed).WithFixedPadding(20.0, 4.0).WithFixedAlignmentOffset(2.0, 0.0);    // 6 

            bkgrBounds.BothSizing = ElementSizing.FitToChildren;
            bkgrBounds.WithChildren(sideBounds, listBounds, scrlBounds, quitBounds);

            this.mainWindow = this.capi.Gui.CreateCompo("svgTest", mainBounds)
                .AddShadedDialogBG(bkgrBounds)
                .AddDialogTitleBar("SVG Sample", OnTitleBarClose)
                .BeginChildElements(bkgrBounds)
                .BeginClip(clipBounds)
                .AddInset(sideBounds, 3)
                .AddHandbookStackList(listBounds, OnLeftClickListElement, this.sections, "stackList")
                .EndClip()
                .AddVerticalScrollbar(OnNewScrollbarValueOverviewPage, scrlBounds, "scrollbar")
                .AddSmallButton(Lang.Get("general-close"), TryClose, quitBounds)
                .EndChildElements()
                .Compose();
            
            this.mainWindow.GetScrollbar("scrollbar")
                .SetHeights(580, (float) this.mainWindow.GetHandbookStackList("stackList").insideBounds.fixedHeight);
        }

        private void OnTitleBarClose() => TryClose();

        private void OnLeftClickListElement(int index)
        {
            var page = this.sections[index];
            if (!(page is GuiHandbookTextIconPage textPage)) return;
            if (textPage.LargeTexture != null)
            {
                new GuiTexturePreview(this.capi) { DisplayTexture = textPage.LargeTexture }.TryOpen();
            }
        }

        private void OnNewScrollbarValueOverviewPage(float value)
        {
            var handbookStackList = this.mainWindow.GetHandbookStackList("stackList");
            handbookStackList.insideBounds.fixedY = 3.0 - value;
            handbookStackList.insideBounds.CalcWorldBounds();
        }
    }
}
