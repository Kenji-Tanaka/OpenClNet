using System;
using System.Runtime.InteropServices;

namespace OpenCLNet {
	#region Using statements
	using cl_char = SByte;
	using cl_uchar = Byte;
	using cl_short = Byte;
	using cl_ushort = Byte;
	using cl_int = Int32;
	using cl_uint = UInt32;
	using cl_long = Int64;
	using cl_ulong = UInt64;
	using cl_half = UInt16;
	using cl_float = Single;
	using cl_double = Double;
	using cl_platform_id = IntPtr;
	using cl_device_id = IntPtr;
	using cl_context = IntPtr;
	using cl_command_queue = IntPtr;
	using cl_mem = IntPtr;
	using cl_program = IntPtr;
	using cl_kernel = IntPtr;
	using cl_event = IntPtr;
	using cl_sampler = IntPtr;
	using cl_bool = UInt32;
	using cl_bitfield = UInt64;
	using cl_device_type = UInt64;
	using cl_platform_info = UInt32;
	using cl_device_info = UInt32;
	using cl_device_address_info = UInt64;
	using cl_device_fp_config = UInt64;
	using cl_device_mem_cache_type = UInt32;
	using cl_device_local_mem_type = UInt32;
	using cl_device_exec_capabilities = UInt64;
	using cl_command_queue_properties = UInt64;
	using cl_context_properties = IntPtr;
	using cl_context_info = UInt32;
	using cl_command_queue_info = UInt32;
	using cl_channel_order = UInt32;
	using cl_channel_type = UInt32;
	using cl_mem_flags = UInt64;
	using cl_mem_object_type = UInt32;
	using cl_mem_info = UInt32;
	using cl_image_info = UInt32;
	using cl_addressing_mode = UInt32;
	using cl_filter_mode = UInt32;
	using cl_sampler_info = UInt32;
	using cl_map_flags = UInt64;
	using cl_program_info = UInt32;
	using cl_program_build_info = UInt32;
	using cl_build_status = Int32;
	using cl_kernel_info = UInt32;
	using cl_kernel_work_group_info = UInt32;
	using cl_event_info = UInt32;
	using cl_command_type = UInt32;
	using cl_profiling_info = UInt32;
	#endregion

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ImageFormat {
		internal cl_channel_order image_channel_order;
		internal cl_channel_type image_channel_data_type;

		#region Predefined static Image formats
		public static readonly ImageFormat RGB8U = new ImageFormat(ChannelOrder.RGB, ChannelType.UNSIGNED_INT8);
		public static readonly ImageFormat RGB8S = new ImageFormat(ChannelOrder.RGB, ChannelType.SIGNED_INT8);
		public static readonly ImageFormat RGB16U = new ImageFormat(ChannelOrder.RGB, ChannelType.UNSIGNED_INT16);
		public static readonly ImageFormat RGB16S = new ImageFormat(ChannelOrder.RGB, ChannelType.SIGNED_INT16);
		public static readonly ImageFormat RGB32U = new ImageFormat(ChannelOrder.RGB, ChannelType.UNSIGNED_INT32);
		public static readonly ImageFormat RGB32S = new ImageFormat(ChannelOrder.RGB, ChannelType.SIGNED_INT32);
		public static readonly ImageFormat RGBFloat = new ImageFormat(ChannelOrder.RGB, ChannelType.FLOAT);
		public static readonly ImageFormat RGBHalf = new ImageFormat(ChannelOrder.RGB, ChannelType.HALF_FLOAT);

		public static readonly ImageFormat RG8U = new ImageFormat(ChannelOrder.RG, ChannelType.UNSIGNED_INT8);
		public static readonly ImageFormat RG8S = new ImageFormat(ChannelOrder.RG, ChannelType.SIGNED_INT8);
		public static readonly ImageFormat RG16U = new ImageFormat(ChannelOrder.RG, ChannelType.UNSIGNED_INT16);
		public static readonly ImageFormat RG16S = new ImageFormat(ChannelOrder.RG, ChannelType.SIGNED_INT16);
		public static readonly ImageFormat RG32U = new ImageFormat(ChannelOrder.RG, ChannelType.UNSIGNED_INT32);
		public static readonly ImageFormat RG32S = new ImageFormat(ChannelOrder.RG, ChannelType.SIGNED_INT32);
		public static readonly ImageFormat RGFloat = new ImageFormat(ChannelOrder.RG, ChannelType.FLOAT);
		public static readonly ImageFormat RGHalf = new ImageFormat(ChannelOrder.RG, ChannelType.HALF_FLOAT);

		public static readonly ImageFormat R8U = new ImageFormat(ChannelOrder.R, ChannelType.UNSIGNED_INT8);
		public static readonly ImageFormat R8S = new ImageFormat(ChannelOrder.R, ChannelType.SIGNED_INT8);
		public static readonly ImageFormat R16U = new ImageFormat(ChannelOrder.R, ChannelType.UNSIGNED_INT16);
		public static readonly ImageFormat R16S = new ImageFormat(ChannelOrder.R, ChannelType.SIGNED_INT16);
		public static readonly ImageFormat R32U = new ImageFormat(ChannelOrder.R, ChannelType.UNSIGNED_INT32);
		public static readonly ImageFormat R32S = new ImageFormat(ChannelOrder.R, ChannelType.SIGNED_INT32);
		public static readonly ImageFormat RFloat = new ImageFormat(ChannelOrder.R, ChannelType.FLOAT);
		public static readonly ImageFormat RHalf = new ImageFormat(ChannelOrder.R, ChannelType.HALF_FLOAT);

		public static readonly ImageFormat RA8U = new ImageFormat(ChannelOrder.RA, ChannelType.UNSIGNED_INT8);
		public static readonly ImageFormat RA8S = new ImageFormat(ChannelOrder.RA, ChannelType.SIGNED_INT8);
		public static readonly ImageFormat RA16U = new ImageFormat(ChannelOrder.RA, ChannelType.UNSIGNED_INT16);
		public static readonly ImageFormat RA16S = new ImageFormat(ChannelOrder.RA, ChannelType.SIGNED_INT16);
		public static readonly ImageFormat RA32U = new ImageFormat(ChannelOrder.RA, ChannelType.UNSIGNED_INT32);
		public static readonly ImageFormat RA32S = new ImageFormat(ChannelOrder.RA, ChannelType.SIGNED_INT32);
		public static readonly ImageFormat RAFloat = new ImageFormat(ChannelOrder.RA, ChannelType.FLOAT);
		public static readonly ImageFormat RAHalf = new ImageFormat(ChannelOrder.RA, ChannelType.HALF_FLOAT);

		public static readonly ImageFormat RGBA8U = new ImageFormat(ChannelOrder.RGBA, ChannelType.UNSIGNED_INT8);
		public static readonly ImageFormat RGBA8S = new ImageFormat(ChannelOrder.RGBA, ChannelType.SIGNED_INT8);
		public static readonly ImageFormat RGBA16U = new ImageFormat(ChannelOrder.RGBA, ChannelType.UNSIGNED_INT16);
		public static readonly ImageFormat RGBA16S = new ImageFormat(ChannelOrder.RGBA, ChannelType.SIGNED_INT16);
		public static readonly ImageFormat RGBA32U = new ImageFormat(ChannelOrder.RGBA, ChannelType.UNSIGNED_INT32);
		public static readonly ImageFormat RGBA32S = new ImageFormat(ChannelOrder.RGBA, ChannelType.SIGNED_INT32);
		public static readonly ImageFormat RGBAFloat = new ImageFormat(ChannelOrder.RGBA, ChannelType.FLOAT);
		public static readonly ImageFormat RGBAHalf = new ImageFormat(ChannelOrder.RGBA, ChannelType.HALF_FLOAT);

		public static readonly ImageFormat BGRA8U = new ImageFormat(ChannelOrder.BGRA, ChannelType.UNSIGNED_INT8);
		public static readonly ImageFormat BGRA8S = new ImageFormat(ChannelOrder.BGRA, ChannelType.SIGNED_INT8);
		public static readonly ImageFormat BGRA16U = new ImageFormat(ChannelOrder.BGRA, ChannelType.UNSIGNED_INT16);
		public static readonly ImageFormat BGRA16S = new ImageFormat(ChannelOrder.BGRA, ChannelType.SIGNED_INT16);
		public static readonly ImageFormat BGRA32U = new ImageFormat(ChannelOrder.BGRA, ChannelType.UNSIGNED_INT32);
		public static readonly ImageFormat BGRA32S = new ImageFormat(ChannelOrder.BGRA, ChannelType.SIGNED_INT32);
		public static readonly ImageFormat BGRAFloat = new ImageFormat(ChannelOrder.BGRA, ChannelType.FLOAT);
		public static readonly ImageFormat BGRAHalf = new ImageFormat(ChannelOrder.BGRA, ChannelType.HALF_FLOAT);

		public static readonly ImageFormat ARGB8U = new ImageFormat(ChannelOrder.ARGB, ChannelType.UNSIGNED_INT8);
		public static readonly ImageFormat ARGB8S = new ImageFormat(ChannelOrder.ARGB, ChannelType.SIGNED_INT8);
		public static readonly ImageFormat ARGB16U = new ImageFormat(ChannelOrder.ARGB, ChannelType.UNSIGNED_INT16);
		public static readonly ImageFormat ARGB16S = new ImageFormat(ChannelOrder.ARGB, ChannelType.SIGNED_INT16);
		public static readonly ImageFormat ARGB32U = new ImageFormat(ChannelOrder.ARGB, ChannelType.UNSIGNED_INT32);
		public static readonly ImageFormat ARGB32S = new ImageFormat(ChannelOrder.ARGB, ChannelType.SIGNED_INT32);
		public static readonly ImageFormat ARGBFloat = new ImageFormat(ChannelOrder.ARGB, ChannelType.FLOAT);
		public static readonly ImageFormat ARGBHalf = new ImageFormat(ChannelOrder.ARGB, ChannelType.HALF_FLOAT);

		public static readonly ImageFormat A8U = new ImageFormat(ChannelOrder.A, ChannelType.UNSIGNED_INT8);
		public static readonly ImageFormat A8S = new ImageFormat(ChannelOrder.A, ChannelType.SIGNED_INT8);
		public static readonly ImageFormat A16U = new ImageFormat(ChannelOrder.A, ChannelType.UNSIGNED_INT16);
		public static readonly ImageFormat A16S = new ImageFormat(ChannelOrder.A, ChannelType.SIGNED_INT16);
		public static readonly ImageFormat A32U = new ImageFormat(ChannelOrder.A, ChannelType.UNSIGNED_INT32);
		public static readonly ImageFormat A32S = new ImageFormat(ChannelOrder.A, ChannelType.SIGNED_INT32);
		public static readonly ImageFormat AFloat = new ImageFormat(ChannelOrder.A, ChannelType.FLOAT);
		public static readonly ImageFormat AHalf = new ImageFormat(ChannelOrder.A, ChannelType.HALF_FLOAT);

		public static readonly ImageFormat INTENSITY8U = new ImageFormat(ChannelOrder.INTENSITY, ChannelType.UNSIGNED_INT8);
		public static readonly ImageFormat INTENSITY8S = new ImageFormat(ChannelOrder.INTENSITY, ChannelType.SIGNED_INT8);
		public static readonly ImageFormat INTENSITY16U = new ImageFormat(ChannelOrder.INTENSITY, ChannelType.UNSIGNED_INT16);
		public static readonly ImageFormat INTENSITY16S = new ImageFormat(ChannelOrder.INTENSITY, ChannelType.SIGNED_INT16);
		public static readonly ImageFormat INTENSITY32U = new ImageFormat(ChannelOrder.INTENSITY, ChannelType.UNSIGNED_INT32);
		public static readonly ImageFormat INTENSITY32S = new ImageFormat(ChannelOrder.INTENSITY, ChannelType.SIGNED_INT32);
		public static readonly ImageFormat INTENSITYFloat = new ImageFormat(ChannelOrder.INTENSITY, ChannelType.FLOAT);
		public static readonly ImageFormat INTENSITYHalf = new ImageFormat(ChannelOrder.INTENSITY, ChannelType.HALF_FLOAT);

		public static readonly ImageFormat LUMINANCE8U = new ImageFormat(ChannelOrder.LUMINANCE, ChannelType.UNSIGNED_INT8);
		public static readonly ImageFormat LUMINANCE8S = new ImageFormat(ChannelOrder.LUMINANCE, ChannelType.SIGNED_INT8);
		public static readonly ImageFormat LUMINANCE16U = new ImageFormat(ChannelOrder.LUMINANCE, ChannelType.UNSIGNED_INT16);
		public static readonly ImageFormat LUMINANCE16S = new ImageFormat(ChannelOrder.LUMINANCE, ChannelType.SIGNED_INT16);
		public static readonly ImageFormat LUMINANCE32U = new ImageFormat(ChannelOrder.LUMINANCE, ChannelType.UNSIGNED_INT32);
		public static readonly ImageFormat LUMINANCE32S = new ImageFormat(ChannelOrder.LUMINANCE, ChannelType.SIGNED_INT32);
		public static readonly ImageFormat LUMINANCEFloat = new ImageFormat(ChannelOrder.LUMINANCE, ChannelType.FLOAT);
		public static readonly ImageFormat LUMINANCEHalf = new ImageFormat(ChannelOrder.LUMINANCE, ChannelType.HALF_FLOAT);
		#endregion

		public ImageFormat(ChannelOrder channelOrder, ChannelType channelType) {
			this.image_channel_order = (cl_channel_order)channelOrder;
			this.image_channel_data_type = (cl_channel_type)channelType;
		}

		public ChannelOrder ChannelOrder {
			get => (ChannelOrder)this.image_channel_order;
			set => this.image_channel_order = (cl_channel_order)value;
		}

		public ChannelType ChannelType {
			get => (ChannelType)this.image_channel_data_type;
			set => this.image_channel_data_type = (cl_channel_type)value;
		}
	}

	// cl_device_type - bitfield

	// cl_device_info

	// cl_device_fp_config - bitfield

	// cl_device_mem_cache_type

	// cl_device_local_mem_type

	// cl_device_exec_capabilities - bitfield

	// cl_command_queue_properties - bitfield

	// cl_context_info

	// cl_gl_context_info

	// cl_context_properties

	// cl_command_queue_info

	// cl_mem_flags - bitfield

	// cl_channel_order

	// cl_channel_type

	// cl_mem_object_type

	// cl_mem_info

	// cl_image_info

	// cl_addressing_mode

	// cl_filter_mode

	// cl_sampler_info

	// cl_map_flags - bitfield

	// cl_program_info

	// cl_program_build_info

	// cl_build_status

	// cl_kernel_info

	// cl_kernel_work_group_info

	// cl_event_info

	// cl_command_type

	// command execution status

	// cl_profiling_info

	// ********************************************
	// * CLGL enums
	// ********************************************

	// ********************************************
	// * D3D10 enums
	// ********************************************

	// ********************************************
	// * cl_ext_device_fission enums
	// ********************************************

	#region Vector2
	#endregion

	#region Vector3
	#endregion

	#region Vector4
	#endregion

	#region Vector8
	#endregion

	#region Vector16
	#endregion
}