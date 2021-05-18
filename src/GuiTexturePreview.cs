using Vintagestory.API.Client;

namespace SVGPoc
{
    public class GuiTexturePreview : GuiDialog
    {
        public override string ToggleKeyCombinationCode => null;
        private float counter = 0;

        public LoadedTexture DisplayTexture { set; private get;  }

        public GuiTexturePreview(ICoreClientAPI capi) : base(capi) {}

        public override void OnRenderGUI(float deltaTime)
        {
            this.counter += deltaTime;
            if (this.DisplayTexture != null)
                this.capi.Render.Render2DLoadedTexture(this.DisplayTexture, (int)(100 + this.counter*2), (int)(100 + this.counter*2), 9999);
        }

        public override void OnMouseDown(MouseEvent args)
        {
            base.OnMouseDown(args);
            TryClose();
        }
    }
}
