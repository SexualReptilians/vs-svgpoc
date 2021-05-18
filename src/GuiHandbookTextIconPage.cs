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
        
        public override void RenderTo(ICoreClientAPI capi, double x, double y)
        {
            var num1 = (float) GuiElement.scaled(25.0);
            var num2 = (float) GuiElement.scaled(10.0);
            if (this.Texture != null)
                capi.Render.Render2DTexturePremultipliedAlpha(this.Texture.TextureId, x + num2, y + num1 / 4.0 - 3.0, this.Texture.Width, this.Texture.Height);
            if (this.textTexture == null)
                Recompose(capi);
            capi.Render.Render2DTexturePremultipliedAlpha(this.textTexture.TextureId, x + num2 + 50, y + num1 / 4.0 - 3.0, this.textTexture.Width, this.textTexture.Height);
        }
    }
}
