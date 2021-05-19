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

        public SvgLoader(ICoreClientAPI _capi)
        {
            capi = _capi;

            // Attempt to create the rasterizer
            rasterizer = NativeMethods.nsvgCreateRasterizer();
            if (rasterizer == IntPtr.Zero)
            {
                System.Diagnostics.Debug.WriteLine("RASTER INIT ERROR");
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
        
        // if width and height = 0 (unspecified), render at the svg's default size
        // if one of w or h is 0, scale down to the supplied dimension
        // scale param is only considered when width and height is both 0
        public LoadedTexture LoadSvg(IAsset svgAsset, int width = 0, int height = 0)
        {
            float scale = 1.0f;
            float offX = 0;
            float offY = 0;
            float dpi = 96;

            // Rasterizer doesnt exist
            if (rasterizer == IntPtr.Zero) throw new ObjectDisposedException("SvgLoader is already disposed!");
            
            // Attempt to parse SVG file from string
            IntPtr image = NativeMethods.nsvgParse(svgAsset.ToText(), "px", dpi);
            if (image == IntPtr.Zero)
            {
                System.Diagnostics.Debug.WriteLine("SVG READ FAILED");
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            // Attempt to obtain parsed size
            IntPtr sizeptr = NativeMethods.nsvgImageGetSize(image);
            if (sizeptr == IntPtr.Zero)
            {
                System.Diagnostics.Debug.WriteLine("Size read failed");
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            // Put parsed size into object
            NsvgSize size = Marshal.PtrToStructure<NsvgSize>(NativeMethods.nsvgImageGetSize(image));

            // calc scale
            // none supplied, use original size
            if (width == 0 && height == 0)
            {
                width = (int)(size.width * scale);
                height = (int)(size.height * scale);
            }
            // Width auto
            else if (width == 0)
            {
                scale = height / size.height;
                width = (int)(size.width * scale);
            }
            // Height auto
            else if (height == 0)
            {
                scale = width / size.width;
                height = (int)(size.height * scale);
            }
            // Auto aspect ratio
            else
            {
                var scaleX = width / size.width;
                var scaleY = height / size.height;

                scale = scaleX < scaleY ? scaleX : scaleY;
            }
                
            // create GL texture
            // Stolen from base implementation
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            // We need to get a pointer to our byte array to store our pixel data.
            // Using an unsafe context + pointers is better in terms of performance
            // as there is no need to copy memory, compared to marshalling values.
            // This is probably necessary because Marshal wont allocate large buffers to begin with.
            unsafe
            {
                byte[] buffer = new byte[width * height * 4]; // rgba
                fixed (byte* p = buffer)
                {
                    // Rasterize
                    NativeMethods.nsvgRasterize(rasterizer, image, offX,offY,scale, (IntPtr)p, width, height, width*4);
                    // Make texture out of rasterised buffer
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, (IntPtr)p);
                }
            }

            // dispose
            NativeMethods.nsvgDelete(image);

            return new LoadedTexture(capi, id, width, height);
        }

        ~SvgLoader()
        {
            if (rasterizer != IntPtr.Zero)
            {
                NativeMethods.nsvgDeleteRasterizer(rasterizer);
                rasterizer = IntPtr.Zero;
            }
        } 
    }
}