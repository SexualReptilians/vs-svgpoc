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
        public override string ToggleKeyCombinationCode => "svgtoggle";

        private readonly SvgLoader svgLoader;
        private List<GuiHandbookPage> listItems;
        private GuiComposer mainWindow;
        
        public GuiSvgSamples(ICoreClientAPI capi) : base(capi)
        {
            try
            {
                // attempt creation of rasterizer
                svgLoader = new SvgLoader(capi);
                SetupPages();
                SetupGui();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        private void SetupPages()
        {
            // Some testing samples
            IAsset tiger = capi.Assets.Get(new AssetLocation("svgpoc", "textures/tiger.svg"));
            IAsset cam = capi.Assets.Get(new AssetLocation("svgpoc", "textures/AJ_Digital_Camera.svg"));
            IAsset acid = capi.Assets.Get(new AssetLocation("svgpoc", "textures/acid.svg"));
            IAsset alpha = capi.Assets.Get(new AssetLocation("svgpoc", "textures/alphachannel.svg"));
            IAsset car = capi.Assets.Get(new AssetLocation("svgpoc", "textures/car.svg"));
            IAsset clip = capi.Assets.Get(new AssetLocation("svgpoc", "textures/clippath.svg"));
            IAsset dash = capi.Assets.Get(new AssetLocation("svgpoc", "textures/dashes.svg"));
            IAsset car2 = capi.Assets.Get(new AssetLocation("svgpoc", "textures/gallardo.svg"));
            IAsset g1 = capi.Assets.Get(new AssetLocation("svgpoc", "textures/gaussian1.svg"));
            IAsset g2 = capi.Assets.Get(new AssetLocation("svgpoc", "textures/gaussian3.svg"));
            IAsset grad = capi.Assets.Get(new AssetLocation("svgpoc", "textures/lineargradient2.svg"));
            IAsset rgrad = capi.Assets.Get(new AssetLocation("svgpoc", "textures/radialgradient2.svg"));
            IAsset polyl = capi.Assets.Get(new AssetLocation("svgpoc", "textures/shapes-polyline-01-t.svg"));
            IAsset wyvern = capi.Assets.Get(new AssetLocation("svgpoc", "textures/wyvern.svg"));
            IAsset dragon = capi.Assets.Get(new AssetLocation("svgpoc", "textures/dragon.svg"));
            IAsset strokesandcaps = capi.Assets.Get(new AssetLocation("svgpoc", "textures/strokes_and_caps.svg"));
            IAsset dashes2 = capi.Assets.Get(new AssetLocation("svgpoc", "textures/dashes2.svg"));

            // Texture is preview icon, LargeTexture is the actual preview when clicked.
            // Title appears on the list (Text does nothing rn)
            listItems = new List<GuiHandbookPage>
            {
                new GuiHandbookTextIconPage
                {
                    Title = "Ghostscript Tiger",
                    Texture = svgLoader.LoadSvg(tiger, 32, 32),
                    LargeTexture = svgLoader.LoadSvg(tiger)
                },
                new GuiHandbookTextIconPage
                {
                    Title = "Acid Warning", 
                    Texture = svgLoader.LoadSvg(acid, 32, 32),
                    LargeTexture = svgLoader.LoadSvg(acid)
                },
                new GuiHandbookTextIconPage
                {
                    Title = "Camera", 
                    Texture = svgLoader.LoadSvg(cam, 32, 32),
                    LargeTexture = svgLoader.LoadSvg(cam)
                },
                new GuiHandbookTextIconPage
                {
                    Title = "Car 1", 
                    Texture = svgLoader.LoadSvg(car, 32, 32),
                    LargeTexture = svgLoader.LoadSvg(car)
                },
                new GuiHandbookTextIconPage
                {
                    Title = "Car 2", 
                    Texture = svgLoader.LoadSvg(car2, 32, 32),
                    LargeTexture = svgLoader.LoadSvg(car2)
                },
                new GuiHandbookTextIconPage
                {
                    Title = "Alpha Channel Test", 
                    Texture = svgLoader.LoadSvg(alpha, 32, 32),
                    LargeTexture = svgLoader.LoadSvg(alpha)
                },
                new GuiHandbookTextIconPage
                {
                    Title = "Clipping Path Test", 
                    Texture = svgLoader.LoadSvg(clip, 32, 32),
                    LargeTexture = svgLoader.LoadSvg(clip)
                },
                new GuiHandbookTextIconPage
                {
                    Title = "Dashes Test", 
                    Texture = svgLoader.LoadSvg(dash, 32, 32),
                    LargeTexture = svgLoader.LoadSvg(dash)
                },
                new GuiHandbookTextIconPage
                {
                    Title = "Dashes Test 2",
                    Texture = svgLoader.LoadSvg(dashes2, 32, 32),
                    LargeTexture = svgLoader.LoadSvg(dashes2)
                },
                new GuiHandbookTextIconPage
                {
                    Title = "Strokes and caps test",
                    Texture = svgLoader.LoadSvg(strokesandcaps, 32, 32),
                    LargeTexture = svgLoader.LoadSvg(strokesandcaps)
                },
                new GuiHandbookTextIconPage
                {
                    Title = "Polyline Test", 
                    Texture = svgLoader.LoadSvg(polyl, 32, 32),
                    LargeTexture = svgLoader.LoadSvg(polyl)
                },
                new GuiHandbookTextIconPage
                {
                    Title = "Gaussian Blur Test 1", 
                    Texture = svgLoader.LoadSvg(g1, 32, 32),
                    LargeTexture = svgLoader.LoadSvg(g1)
                },
                new GuiHandbookTextIconPage
                {
                    Title = "Gaussian Blur Test 2", 
                    Texture = svgLoader.LoadSvg(g2, 32, 32),
                    LargeTexture = svgLoader.LoadSvg(g2)
                },
                new GuiHandbookTextIconPage
                {
                    Title = "Linear Gradient Test", 
                    Texture = svgLoader.LoadSvg(grad, 32, 32),
                    LargeTexture = svgLoader.LoadSvg(grad)
                },
                new GuiHandbookTextIconPage
                {
                    Title = "Radial Gradient Test", 
                    Texture = svgLoader.LoadSvg(rgrad, 32, 32),
                    LargeTexture = svgLoader.LoadSvg(rgrad)
                },
                new GuiHandbookTextIconPage
                {
                    Title = "Wyvern",
                    Texture = svgLoader.LoadSvg(wyvern, 32, 32),
                    LargeTexture = svgLoader.LoadSvg(wyvern)
                },
                new GuiHandbookTextIconPage
                {
                    Title = "Dragon",
                    Texture = svgLoader.LoadSvg(dragon, 32, 32),
                    LargeTexture = svgLoader.LoadSvg(dragon)
                },
            };

            // initialize the pages
            int count = 0;
            foreach (var page in listItems)
            {
                if (!(page is GuiHandbookTextIconPage textPage)) continue;
                textPage.pageCode = $"{count}";
                textPage.PageNumber = count;
                textPage.Text = "";
                textPage.Init(capi);
                count++;
            }
        }


        private void SetupGui()
        {
            // Stolen from base implementation
            var mainBounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.CenterFixed).WithFixedPosition(0.0, 100.0);
            var bkgrBounds = ElementBounds.Fill.WithFixedPadding(GuiStyle.ElementToDialogPadding);
            var listBounds = ElementBounds.Fixed(GuiStyle.ElementToDialogPadding - 2.0, 50.0, 500.0, 580);
            var clipBounds = listBounds.ForkBoundingParent();
            var sideBounds = listBounds.FlatCopy().FixedGrow(6.0).WithFixedOffset(-3.0, -3.0);
            var scrlBounds = sideBounds.CopyOffsetedSibling(3.0 + listBounds.fixedWidth + 7.0).WithFixedWidth(20.0);
            var quitBounds = ElementBounds.FixedSize(0.0, 0.0).FixedUnder(clipBounds, 18.0).WithAlignment(EnumDialogArea.RightFixed).WithFixedPadding(20.0, 4.0).WithFixedAlignmentOffset(2.0, 0.0);

            bkgrBounds.BothSizing = ElementSizing.FitToChildren;
            bkgrBounds.WithChildren(sideBounds, listBounds, scrlBounds, quitBounds);

            // Compose the window
            mainWindow = capi.Gui.CreateCompo("svgTest", mainBounds)
                .AddShadedDialogBG(bkgrBounds)
                .AddDialogTitleBar("SVG Sample", OnTitleBarClose)
                .BeginChildElements(bkgrBounds)
                .BeginClip(clipBounds)
                .AddInset(sideBounds, 3)
                .AddHandbookStackList(listBounds, OnLeftClickListElement, listItems, "stackList")
                .EndClip()
                .AddVerticalScrollbar(OnNewScrollbarValueOverviewPage, scrlBounds, "scrollbar")
                .AddSmallButton(Lang.Get("general-close"), TryClose, quitBounds)
                .EndChildElements()
                .Compose();
            
            mainWindow.GetScrollbar("scrollbar")
                .SetHeights(580, (float) mainWindow.GetHandbookStackList("stackList").insideBounds.fixedHeight);
        }

        public override void OnRenderGUI(float deltaTime)
        {
            SingleComposer = mainWindow;
            base.OnRenderGUI(deltaTime);
        }
        
        private void OnTitleBarClose() => TryClose();

        // Try and show the large texture overlay
        private void OnLeftClickListElement(int index)
        {
            var page = listItems[index];
            if (!(page is GuiHandbookTextIconPage textPage)) return;
            if (textPage.LargeTexture != null)
            {
                new GuiTexturePreview(capi) { DisplayTexture = textPage.LargeTexture }.TryOpen();
            }
        }

        private void OnNewScrollbarValueOverviewPage(float value)
        {
            var handbookStackList = mainWindow.GetHandbookStackList("stackList");
            handbookStackList.insideBounds.fixedY = 3.0 - value;
            handbookStackList.insideBounds.CalcWorldBounds();
        }
    }
}
