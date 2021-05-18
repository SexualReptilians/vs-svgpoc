using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace SVGPoc
{
    public class Wyvern : ModSystem
    {
        private GuiDialog dialog;

        public override void StartClientSide(ICoreClientAPI capi)
        {
            System.Diagnostics.Debug.WriteLine("Wyverns? Wyverns.");
            this.dialog = new GuiSvgSamples(capi);
            capi.Event.IsPlayerReady += Event_IsPlayerReady;
        }

        private bool Event_IsPlayerReady(ref EnumHandling handling)
        {
            this.dialog?.TryOpen();
            return true;
        }
    }
}
