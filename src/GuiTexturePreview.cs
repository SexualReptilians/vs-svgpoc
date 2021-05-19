using Vintagestory.API.Client;

namespace SVGPoc
{
    public class GuiTexturePreview : GuiDialog
    {
        public override string ToggleKeyCombinationCode => null;
        private float counter = 0;

        public LoadedTexture DisplayTexture { set; private get;  }

        public GuiTexturePreview(ICoreClientAPI capi) : base(capi) {}

        // Overlay large texture, move it around
        public override void OnRenderGUI(float deltaTime)
        {
            counter += deltaTime;
            if (DisplayTexture != null)
                capi.Render.Render2DLoadedTexture(DisplayTexture, (int)(100 + counter*2), (int)(100 + counter*2), 9999);
        }

        public override void OnMouseDown(MouseEvent args)
        {
            base.OnMouseDown(args);
            TryClose();
        }
    }
}
