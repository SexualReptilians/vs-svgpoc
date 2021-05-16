
using System;
using System.Runtime.InteropServices;

namespace RSvg
{
    internal struct RsvgDimensionData {
        int width;  // current svg width
        int height; // current svg width
        double em;  // original svg width
        double ex;  // original svg height
    };

    internal struct RsvgPositionData
    {
        int x;
        int y;
    };

    internal enum RsvgUnit
    {
        RSVG_UNIT_PERCENT,
        RSVG_UNIT_PX,
        RSVG_UNIT_EM,
        RSVG_UNIT_EX,
        RSVG_UNIT_IN,
        RSVG_UNIT_CM,
        RSVG_UNIT_MM,
        RSVG_UNIT_PT,
        RSVG_UNIT_PC,
    }
    
    internal struct RsvgLength {
        double   length;
        RsvgUnit unit;
    }

    // a struct is being passed
    internal class RsvgRectangle {
        double x;
        double y;
        double width;
        double height;
    };

    
    internal static class NativeMethods
    { 
        //   disable resharper warnings for names   //
        //// ReSharper disable InconsistentNaming ////
        
        
        // Windows: "librsvg-2-2"
        // Linux:   "librsvg-2"
        private const string rsvg = "librsvg-2-2";
        
        /*/
         * Current librsvg Version: 2.50.3-1
         * Windows:
         *    Binaries: https://packages.msys2.org/base/mingw-w64-librsvg
         *      - 32-bit: mingw-w64-i686-librsvg or mingw-w64-ucrt-x86_64-librsvg for UCRT
         *      - 64-bit: mingw-w64-x86_64-librsvg
         * 
        /*/
        
        /*/
         * Binding notes:
         *
         * void *<ret>          - <ret> IntPtr
         * typ_t *handle        - IntPtr handle
         * int *ints            - int[] ints
         * int *ints            - [out] IntPtr ints
         * int *anInt           - [out] out int anInt
         * int *anInt           - [in/out] ref int anInt
         * my_enum_t <ret>      - <ret> MyEnum
         * my_enum_t anEnum     - MyEnum anEnum
         * my_class_t *aClass   - IntPtr          (if C type)
         * my_class_t *aClass   - MyClass aClass  (if data type)    (ICloneable with public object Clone() => this.MemberwiseClone(); ?)
         * my_class_t *aClass   - [out] MyClass aClass              (ICloneable with public object Clone() => this.MemberwiseClone(); ?)
         * my_struct_t *<ret>   - <ret> IntPtr
         * my_struct_t *aStruct - IntPtr/"Declare my_struct_t as class instead of struct and don't pass it by ref"
         * my_struct_t *aStruct - [out] out MyStruct aStruct        (c# struct field ordering should match)
         * const char *<ret>    - <ret> IntPtr                      (use PtrToStringAuto(IntPtr ptr) to obtain the string)
        /*/
        

        /* Creation */
        [DllImport(rsvg, CallingConvention = CallingConvention.Cdecl)]  // since 2.14
        internal static extern IntPtr rsvg_handle_new_from_file([MarshalAs(UnmanagedType.LPStr)]string file_name, out IntPtr error);

        [DllImport(rsvg, CallingConvention = CallingConvention.Cdecl)]  // since 2.14
        internal static extern IntPtr rsvg_handle_new_from_data(byte[] data, int len, out IntPtr error);
        
        
        /* Rendering */
        [DllImport(rsvg, CallingConvention = CallingConvention.Cdecl)]  // since 2.14
        internal static extern int rsvg_handle_render_cairo(IntPtr handle, IntPtr cr);
        
        [DllImport(rsvg, CallingConvention = CallingConvention.Cdecl)]  // since 2.14
        internal static extern int rsvg_handle_render_cairo_sub(IntPtr handle, IntPtr cr, [CanBeNull] [MarshalAs(UnmanagedType.LPStr)]string id);
        
        [DllImport(rsvg, CallingConvention = CallingConvention.Cdecl)]  // since 2.46
        internal static extern int rsvg_handle_render_document(IntPtr handle, IntPtr cr, RsvgRectangle viewport, out IntPtr error);
        
        [DllImport(rsvg, CallingConvention = CallingConvention.Cdecl)]  // since 2.46
        internal static extern int rsvg_handle_render_element(
            IntPtr handle, 
            IntPtr cr, 
            [CanBeNull] [MarshalAs(UnmanagedType.LPStr)]string id,
            RsvgRectangle element_viewport, 
            out IntPtr error);
        
        [DllImport(rsvg, CallingConvention = CallingConvention.Cdecl)]  // since 2.46
        internal static extern int rsvg_handle_render_layer(
            IntPtr handle, 
            IntPtr cr, 
            [CanBeNull] [MarshalAs(UnmanagedType.LPStr)]string id,
            RsvgRectangle viewport, 
            out IntPtr error);
        
        
        /* Dimensions */
        [DllImport(rsvg, CallingConvention = CallingConvention.Cdecl)]  // since 2.14
        internal static extern void rsvg_handle_get_dimensions(IntPtr handle, out RsvgDimensionData dimension_data);
        
        [DllImport(rsvg, CallingConvention = CallingConvention.Cdecl)]  // since 2.22
        internal static extern int rsvg_handle_get_dimensions_sub(IntPtr handle, out RsvgDimensionData dimension_data, [CanBeNull] [MarshalAs(UnmanagedType.LPStr)]string id);

        [DllImport(rsvg, CallingConvention = CallingConvention.Cdecl)]  // since 2.22
        internal static extern int rsvg_handle_get_position_sub(IntPtr handle, out RsvgPositionData position_data, [CanBeNull] [MarshalAs(UnmanagedType.LPStr)]string id);
        
        [DllImport(rsvg, CallingConvention = CallingConvention.Cdecl)]  // since 2.14
        internal static extern void rsvg_handle_get_intrinsic_dimensions(
            IntPtr handle, 
            [CanBeNull] out int out_has_width,
            [CanBeNull] out RsvgLength out_width,
            [CanBeNull] out int out_has_height, 
            [CanBeNull] out RsvgLength out_height,
            [CanBeNull] out int out_has_viewbox,
            [CanBeNull] out RsvgRectangle out_viewbox);

        [DllImport(rsvg, CallingConvention = CallingConvention.Cdecl)]  // since 2.46
        internal static extern int rsvg_handle_get_geometry_for_layer(
            IntPtr handle, 
            [CanBeNull] [MarshalAs(UnmanagedType.LPStr)]string id,
            RsvgRectangle viewport,
            [CanBeNull] RsvgRectangle out_ink_rect,
            [CanBeNull] RsvgRectangle out_logical_rect,
            out IntPtr error);

        [DllImport(rsvg, CallingConvention = CallingConvention.Cdecl)]  // since 2.46
        internal static extern int rsvg_handle_get_geometry_for_element(
            IntPtr handle, 
            [CanBeNull] [MarshalAs(UnmanagedType.LPStr)]string id,
            [CanBeNull] RsvgRectangle out_ink_rect,
            [CanBeNull] RsvgRectangle out_logical_rect,
            out IntPtr error);
        
        [DllImport(rsvg, CallingConvention = CallingConvention.Cdecl)]  // since 2.8
        internal static extern void rsvg_handle_set_dpi(IntPtr handle, double dpi);

        [DllImport(rsvg, CallingConvention = CallingConvention.Cdecl)] // since 2.8
        internal static extern void rsvg_handle_set_dpi_x_y(IntPtr handle, double dpi_x, double dpi_y);
        

        /* Other */
        [DllImport(rsvg, CallingConvention = CallingConvention.Cdecl)]  // since 2.22
        internal static extern int rsvg_handle_has_sub(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)]string id);

        [DllImport(rsvg, CallingConvention = CallingConvention.Cdecl)]  // since 2.14
        internal static extern int rsvg_handle_set_stylesheet(IntPtr handle, byte[] css, int len, out IntPtr error);
    }

    internal class CanBeNullAttribute : Attribute
    {
    }
}