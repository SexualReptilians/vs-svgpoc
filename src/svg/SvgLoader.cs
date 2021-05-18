using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace NanoSvg
{
    public class SvgLoader
    {
        private readonly ICoreClientAPI capi;
        private IntPtr rasterizer;

        public SvgLoader(ICoreClientAPI capi)
        {
            this.capi = capi;
            this.rasterizer = NativeMethods.nsvgCreateRasterizer();
            if (this.rasterizer == IntPtr.Zero)
            {
                System.Diagnostics.Debug.WriteLine("RASTER INIT ERROR");
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
        
        // if width and height = 0 (unspecified), render at the svg's default size
        // if one of w or h is 0, scale down to the supplied dimension
        // scale param is only considered when width and height is both 0
        public LoadedTexture LoadSvg(IAsset svgAsset, int width = 0, int height = 0, float scale = 1.0f, float offX = 0, float offY = 0, float dpi = 96)
        {
            if (this.rasterizer == IntPtr.Zero) throw new ObjectDisposedException("SvgLoader is already disposed!");
            
            IntPtr image = NativeMethods.nsvgParse(svgAsset.ToText(), "px", dpi);
            if (image == IntPtr.Zero)
            {
                System.Diagnostics.Debug.WriteLine("SVG READ FAILED");
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            
            NsvgSize size;
            NativeMethods.nsvgImageGetSize(image, out size);
                
            // calc scale
            if (width == 0 && height == 0)  // none supplied, use original size
            {
                width = (int)(size.width * scale);
                height = (int)(size.height * scale);
            }
            else if (width == 0)    // height supplied, scale image down to height
            {
                scale = height / size.height;
                width = (int)(size.width * scale);
            }
            else if (height == 0)   // width supplied, scale image down to width
            {
                scale = width / size.width;
                height = (int)(size.height * scale);
            }
            else                    // all supplied, pick smaller
            {
                var scaleX = width / size.width;
                var scaleY = height / size.height;

                scale = scaleX < scaleY ? scaleX : scaleY;
            }
                
            // create mem texture
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            // We need to get a pointer to our byte array to store our pixel data.
            // Using an unsafe context + pointers is better in terms of performance
            // as there is no need to copy memory, compared to marshalling values.
            unsafe
            {
                byte[] buffer = new byte[width * height * 4];   // rgba
                fixed (byte* p = buffer)
                {
                    IntPtr ptr = (IntPtr) p;
                    NativeMethods.nsvgRasterize(this.rasterizer, image, offX,offY,scale, ptr, width, height, width*4);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, ptr);
                }
            }

            // dispose
            NativeMethods.nsvgDelete(image);

            return new LoadedTexture(this.capi, id, width, height);
        }

        ~SvgLoader()
        {
            if (this.rasterizer != IntPtr.Zero)
            {
                NativeMethods.nsvgDeleteRasterizer(this.rasterizer);
                this.rasterizer = IntPtr.Zero;
            }
        } 
    }
}