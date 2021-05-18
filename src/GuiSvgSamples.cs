using System;
using System.Collections.Generic;
using NanoSvg;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace SVGPoc
{
    public class GuiSvgSamples : GuiDialog
    {
        public override string ToggleKeyCombinationCode => "SVG PoC window toggle";
        private readonly SvgLoader svgLoader;

        private List<GuiHandbookPage> sections;
        private GuiComposer mainWindow;
        
        public GuiSvgSamples(ICoreClientAPI capi) : base(capi)
        {
            try
            {
                this.svgLoader = new SvgLoader(capi);
                SetupPages();
                SetupGui();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"bruh: ${e}" );
            }
        }

        private void SetupPages()
        {
            IAsset tiger = this.capi.Assets.Get(new AssetLocation("testdomain", "textures/tiger.svg"));
            IAsset cam = this.capi.Assets.Get(new AssetLocation("testdomain", "textures/AJ_Digital_Camera.svg"));
            IAsset acid = this.capi.Assets.Get(new AssetLocation("testdomain", "textures/acid.svg"));
            IAsset alpha = this.capi.Assets.Get(new AssetLocation("testdomain", "textures/alphachannel.svg"));
            IAsset car = this.capi.Assets.Get(new AssetLocation("testdomain", "textures/car.svg"));
            IAsset clip = this.capi.Assets.Get(new AssetLocation("testdomain", "textures/clippath.svg"));
            IAsset dash = this.capi.Assets.Get(new AssetLocation("testdomain", "textures/dashes.svg"));
            IAsset car2 = this.capi.Assets.Get(new AssetLocation("testdomain", "textures/gallardo.svg"));
            IAsset g1 = this.capi.Assets.Get(new AssetLocation("testdomain", "textures/gaussian1.svg"));
            IAsset g2 = this.capi.Assets.Get(new AssetLocation("testdomain", "textures/gaussian3.svg"));
            IAsset grad = this.capi.Assets.Get(new AssetLocation("testdomain", "textures/lineargradient2.svg"));
            IAsset rgrad = this.capi.Assets.Get(new AssetLocation("testdomain", "textures/radialgradient2.svg"));
            IAsset polyl = this.capi.Assets.Get(new AssetLocation("testdomain", "textures/shapes-polyline-01-t.svg"));
            
            // Texture is preview icon, LargeTexture is the actual preview when clicked.
            // Title appears on the list (Text does nothing rn)
            this.sections = new List<GuiHandbookPage> 
            {
                new GuiHandbookTextIconPage
                {
                    Title = "Tiger", 
                    Texture = this.svgLoader.LoadSvg(tiger, 32, 32), 
                    LargeTexture = this.svgLoader.LoadSvg(tiger)
                },
                new GuiHandbookTextIconPage
                {
                    Title = "Acid Warning", 
                    Texture = this.svgLoader.LoadSvg(acid, 32, 32), 
                    LargeTexture = this.svgLoader.LoadSvg(acid)
                },
                new GuiHandbookTextIconPage
                {
                    Title = "Camera", 
                    Texture = this.svgLoader.LoadSvg(cam, 32, 32), 
                    LargeTexture = this.svgLoader.LoadSvg(cam)
                },
                new GuiHandbookTextIconPage
                {
                    Title = "Car 1", 
                    Texture = this.svgLoader.LoadSvg(car, 32, 32), 
                    LargeTexture = this.svgLoader.LoadSvg(car)
                },
                new GuiHandbookTextIconPage
                {
                    Title = "Car 2", 
                    Texture = this.svgLoader.LoadSvg(car2, 32, 32), 
                    LargeTexture = this.svgLoader.LoadSvg(car2)
                },
                new GuiHandbookTextIconPage
                {
                    Title = "Alpha Channel Test", 
                    Texture = this.svgLoader.LoadSvg(alpha, 32, 32), 
                    LargeTexture = this.svgLoader.LoadSvg(alpha)
                },
                new GuiHandbookTextIconPage
                {
                    Title = "Clipping Path Test", 
                    Texture = this.svgLoader.LoadSvg(clip, 32, 32), 
                    LargeTexture = this.svgLoader.LoadSvg(clip)
                },
                new GuiHandbookTextIconPage
                {
                    Title = "Dashes Test", 
                    Texture = this.svgLoader.LoadSvg(dash, 32, 32), 
                    LargeTexture = this.svgLoader.LoadSvg(dash)
                },
                new GuiHandbookTextIconPage
                {
                    Title = "Polyline Test", 
                    Texture = this.svgLoader.LoadSvg(polyl, 32, 32), 
                    LargeTexture = this.svgLoader.LoadSvg(polyl)
                },
                new GuiHandbookTextIconPage
                {
                    Title = "Gaussian Blur Test 1", 
                    Texture = this.svgLoader.LoadSvg(g1, 32, 32), 
                    LargeTexture = this.svgLoader.LoadSvg(g1)
                },
                new GuiHandbookTextIconPage
                {
                    Title = "Gaussian Blur Test 2", 
                    Texture = this.svgLoader.LoadSvg(g2, 32, 32), 
                    LargeTexture = this.svgLoader.LoadSvg(g2)
                },
                new GuiHandbookTextIconPage
                {
                    Title = "Linear Gradient Test", 
                    Texture = this.svgLoader.LoadSvg(grad, 32, 32), 
                    LargeTexture = this.svgLoader.LoadSvg(grad)
                },
                new GuiHandbookTextIconPage
                {
                    Title = "Radial Gradient Test", 
                    Texture = this.svgLoader.LoadSvg(rgrad, 32, 32), 
                    LargeTexture = this.svgLoader.LoadSvg(rgrad)
                },
            };

            var count = 0;
            foreach (var page in this.sections)
            {
                if (!(page is GuiHandbookTextIconPage textPage)) continue;
                textPage.pageCode = $"{count}";
                textPage.PageNumber = count;
                textPage.Text = "";
                textPage.Init(this.capi);
                count++;
            }
        }


        private void SetupGui()
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

        public override void OnRenderGUI(float deltaTime)
        {
            this.SingleComposer = this.mainWindow;
            base.OnRenderGUI(deltaTime);
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
