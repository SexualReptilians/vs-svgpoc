using System;

namespace NanoSvg
{
    public class Rasterizer : IDisposable
    {
        private IntPtr handle = IntPtr.Zero;
        
        // TODO wip //
        
        ~Rasterizer() => Dispose();
        
        public void Dispose()
        {
            if (this.handle != IntPtr.Zero)
            {
                NativeMethods.nsvgDeleteRasterizer(this.handle);
                this.handle = IntPtr.Zero;
            }
            GC.SuppressFinalize( this);
        }
    }
}
