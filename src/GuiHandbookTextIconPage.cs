using Vintagestory.API.Client;
using Vintagestory.API.Config;

namespace SVGPoc
{
    public class GuiHandbookTextIconPage : GuiHandbookTextPage
    {
        public LoadedTexture LargeTexture;
        private LoadedTexture textTexture;

        private new void Recompose(ICoreClientAPI capi)
        {
            this.textTexture?.Dispose();
            this.textTexture = new TextTextureUtil(capi).GenTextTexture(Lang.Get(this.Title), CairoFont.WhiteSmallText());
        }
        
        // override class in order to allow the rendering of two textures
        public override void RenderTo(ICoreClientAPI capi, double x, double y)
        {
            double posX = GuiElement.scaled(10.0);
            double posY = GuiElement.scaled(25.0);

            if (this.Texture != null)
            {
                capi.Render.Render2DTexturePremultipliedAlpha(this.Texture.TextureId, x + posX, y + posY / 4.0 - 3.0, this.Texture.Width, this.Texture.Height);
            }

            if (this.textTexture == null)
            {
                Recompose(capi);
            }

            capi.Render.Render2DTexturePremultipliedAlpha(this.textTexture.TextureId, x + posX + 50, y + posY / 4.0 - 3.0, this.textTexture.Width, this.textTexture.Height);
        }
    }
}
