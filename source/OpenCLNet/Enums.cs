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
			get { return (ChannelOrder)this.image_channel_order; }
			set { this.image_channel_order = (cl_channel_order)value; }
		}

		public ChannelType ChannelType {
			get { return (ChannelType)this.image_channel_data_type; }
			set { this.image_channel_data_type = (cl_channel_type)value; }
		}
	}

	public enum ErrorCode {
		SUCCESS = 0,
		DEVICE_NOT_FOUND = -1,
		DEVICE_NOT_AVAILABLE = -2,
		COMPILER_NOT_AVAILABLE = -3,
		MEM_OBJECT_ALLOCATION_FAILURE = -4,
		OUT_OF_RESOURCES = -5,
		OUT_OF_HOST_MEMORY = -6,
		PROFILING_INFO_NOT_AVAILABLE = -7,
		MEM_COPY_OVERLAP = -8,
		IMAGE_FORMAT_MISMATCH = -9,
		IMAGE_FORMAT_NOT_SUPPORTED = -10,
		BUILD_PROGRAM_FAILURE = -11,
		MAP_FAILURE = -12,
		MISALIGNED_SUB_BUFFER_OFFSET = -13,
		EXEC_STATUS_ERROR_FOR_EVENTS_IN_WAIT_LIST = -14,

		INVALID_VALUE = -30,
		INVALID_DEVICE_TYPE = -31,
		INVALID_PLATFORM = -32,
		INVALID_DEVICE = -33,
		INVALID_CONTEXT = -34,
		INVALID_QUEUE_PROPERTIES = -35,
		INVALID_COMMAND_QUEUE = -36,
		INVALID_HOST_PTR = -37,
		INVALID_MEM_OBJECT = -38,
		INVALID_IMAGE_FORMAT_DESCRIPTOR = -39,
		INVALID_IMAGE_SIZE = -40,
		INVALID_SAMPLER = -41,
		INVALID_BINARY = -42,
		INVALID_BUILD_OPTIONS = -43,
		INVALID_PROGRAM = -44,
		INVALID_PROGRAM_EXECUTABLE = -45,
		INVALID_KERNEL_NAME = -46,
		INVALID_KERNEL_DEFINITION = -47,
		INVALID_KERNEL = -48,
		INVALID_ARG_INDEX = -49,
		INVALID_ARG_VALUE = -50,
		INVALID_ARG_SIZE = -51,
		INVALID_KERNEL_ARGS = -52,
		INVALID_WORK_DIMENSION = -53,
		INVALID_WORK_GROUP_SIZE = -54,
		INVALID_WORK_ITEM_SIZE = -55,
		INVALID_GLOBAL_OFFSET = -56,
		INVALID_EVENT_WAIT_LIST = -57,
		INVALID_EVENT = -58,
		INVALID_OPERATION = -59,
		INVALID_GL_OBJECT = -60,
		INVALID_BUFFER_SIZE = -61,
		INVALID_MIP_LEVEL = -62,
		INVALID_GLOBAL_WORK_SIZE = -63,
		INVALID_PROPERTY = -64,

		// CL_GL Error Codes
		INVALID_GL_SHAREGROUP_REFERENCE_KHR = -1000,

		// cl_khr_icd extension
		PLATFORM_NOT_FOUND_KHR = -1001,

		// D3D10 extension Error Codes
		INVALID_D3D10_DEVICE_KHR = -1002,
		INVALID_D3D10_RESOURCE_KHR = -1003,
		D3D10_RESOURCE_ALREADY_ACQUIRED_KHR = -1004,
		D3D10_RESOURCE_NOT_ACQUIRED_KHR = -1005,

		// cl_ext_device_fission
		DEVICE_PARTITION_FAILED_EXT = -1057,
		INVALID_PARTITION_COUNT_EXT = -1058,
		INVALID_PARTITION_NAME_EXT = -1059
	}

	public sealed class OpenCLConstants {
		public static readonly Int32 CHAR_BIT = 8;
		public static readonly Int32 SCHAR_MAX = 127;
		public static readonly Int32 CHAR_MAX = OpenCLConstants.SCHAR_MAX;
		public static readonly Int32 SCHAR_MIN = (-127 - 1);
		public static readonly Int32 CHAR_MIN = OpenCLConstants.SCHAR_MIN;

		public static readonly Int32 DBL_DIG = 15;
		public static readonly Double DBL_EPSILON = Double.Epsilon;
		public static readonly Int32 DBL_MANT_DIG = 53;
		public static readonly Double DBL_MAX = Double.MaxValue;
		public static readonly Int32 DBL_MAX_10_EXP = +308;
		public static readonly Int32 DBL_MAX_EXP = +1024;
		public static readonly Double DBL_MIN = Double.MinValue;
		public static readonly Int32 DBL_MIN_10_EXP = -307;
		public static readonly Int32 DBL_MIN_EXP = -1021;
		public static readonly Int32 DBL_RADIX = 2;

		public static readonly Int32 FLT_DIG = 6;
		public static readonly Single FLT_EPSILON = Single.Epsilon;
		public static readonly Int32 FLT_MANT_DIG = 24;
		public static readonly Single FLT_MAX = Single.MaxValue;
		public static readonly Int32 FLT_MAX_10_EXP = +38;
		public static readonly Int32 FLT_MAX_EXP = +128;
		public static readonly Single FLT_MIN = Single.MinValue;
		public static readonly Int32 FLT_MIN_10_EXP = -37;
		public static readonly Int32 FLT_MIN_EXP = -125;
		public static readonly Int32 FLT_RADIX = 2;
		public static readonly Double HUGE_VAL = Double.PositiveInfinity;
		public static readonly Single HUGE_VALF = Single.PositiveInfinity;
		public static readonly Single INFINITY = OpenCLConstants.HUGE_VALF;
		public static readonly Int32 INT_MAX = 2147483647;
		public static readonly Int32 INT_MIN = (-2147483647 - 1);
		public static readonly Int64 LONG_MAX = 0x7FFFFFFFFFFFFFFFL;
		public static readonly Int64 LONG_MIN = -0x7FFFFFFFFFFFFFFFL - 1L;
		public static readonly Double M_1_PI = 0.318309886183790691216;
		public static readonly Single M_1_PI_F = 0.31830987334251f;
		public static readonly Double M_2_PI = 0.636619772367581382433;
		public static readonly Single M_2_PI_F = 0.63661974668503f;
		public static readonly Double M_2_SQRTPI = 1.128379167095512558561;
		public static readonly Single M_2_SQRTPI_F = 1.12837922573090f;

		public static readonly Double M_E = 2.718281828459045090796;

		public static readonly Single M_E_F = 2.71828174591064f;
		public static readonly Double M_LN10 = 2.302585092994045901094;
		public static readonly Single M_LN10_F = 2.30258512496948f;
		public static readonly Double M_LN2 = 0.693147180559945286227;
		public static readonly Single M_LN2_F = 0.69314718246460f;
		public static readonly Double M_LOG10E = 0.434294481903251816668;
		public static readonly Single M_LOG10E_F = 0.43429449200630f;
		public static readonly Double M_LOG2E = 1.442695040888963387005;
		public static readonly Single M_LOG2E_F = 1.44269502162933f;
		public static readonly Double M_PI = 3.141592653589793115998;
		public static readonly Double M_PI_2 = 1.570796326794896557999;
		public static readonly Single M_PI_2_F = 1.57079637050629f;
		public static readonly Double M_PI_4 = 0.785398163397448278999;
		public static readonly Single M_PI_4_F = 0.78539818525314f;
		public static readonly Single M_PI_F = 3.14159274101257f;
		public static readonly Double M_SQRT1_2 = 0.707106781186547572737;
		public static readonly Single M_SQRT1_2_F = 0.70710676908493f;
		public static readonly Double M_SQRT2 = 1.414213562373095145475;
		public static readonly Single M_SQRT2_F = 1.41421353816986f;
		public static readonly Single MAXFLOAT = OpenCLConstants.FLT_MAX;

		public static readonly Single NAN = Single.NaN;

		public static readonly Int32 SHRT_MAX = 32767;
		public static readonly Int32 SHRT_MIN = (-32767 - 1);
		public static readonly Int32 UCHAR_MAX = 255;
		public static readonly UInt32 UINT_MAX = 0xffffffffU;
		public static readonly UInt64 ULONG_MAX = 0xFFFFFFFFFFFFFFFFUL;
		public static readonly Int32 USHRT_MAX = 65535;
	}

	public enum Bool {
		FALSE = 0,
		TRUE = 1
	}

	public enum PlatformInfo {
		PROFILE = 0x0900,
		VERSION = 0x0901,
		NAME = 0x0902,
		VENDOR = 0x0903,
		EXTENSIONS = 0x0904,

		// cl_khr_icd extension
		PLATFORM_ICD_SUFFIX_KHR = 0x0920
	}

	// cl_device_type - bitfield
	public enum DeviceType : ulong {
		DEFAULT = (1 << 0),
		CPU = (1 << 1),
		GPU = (1 << 2),
		ACCELERATOR = (1 << 3),
		ALL = 0xFFFFFFFF
	}

	// cl_device_info
	public enum DeviceInfo {
		TYPE = 0x1000,
		VENDOR_ID = 0x1001,
		MAX_COMPUTE_UNITS = 0x1002,
		MAX_WORK_ITEM_DIMENSIONS = 0x1003,
		MAX_WORK_GROUP_SIZE = 0x1004,
		MAX_WORK_ITEM_SIZES = 0x1005,
		PREFERRED_VECTOR_WIDTH_CHAR = 0x1006,
		PREFERRED_VECTOR_WIDTH_SHORT = 0x1007,
		PREFERRED_VECTOR_WIDTH_INT = 0x1008,
		PREFERRED_VECTOR_WIDTH_LONG = 0x1009,
		PREFERRED_VECTOR_WIDTH_FLOAT = 0x100A,
		PREFERRED_VECTOR_WIDTH_DOUBLE = 0x100B,
		MAX_CLOCK_FREQUENCY = 0x100C,
		ADDRESS_BITS = 0x100D,
		MAX_READ_IMAGE_ARGS = 0x100E,
		MAX_WRITE_IMAGE_ARGS = 0x100F,
		MAX_MEM_ALLOC_SIZE = 0x1010,
		IMAGE2D_MAX_WIDTH = 0x1011,
		IMAGE2D_MAX_HEIGHT = 0x1012,
		IMAGE3D_MAX_WIDTH = 0x1013,
		IMAGE3D_MAX_HEIGHT = 0x1014,
		IMAGE3D_MAX_DEPTH = 0x1015,
		IMAGE_SUPPORT = 0x1016,
		MAX_PARAMETER_SIZE = 0x1017,
		MAX_SAMPLERS = 0x1018,
		MEM_BASE_ADDR_ALIGN = 0x1019,
		MIN_DATA_TYPE_ALIGN_SIZE = 0x101A,
		SINGLE_FP_CONFIG = 0x101B,
		GLOBAL_MEM_CACHE_TYPE = 0x101C,
		GLOBAL_MEM_CACHELINE_SIZE = 0x101D,
		GLOBAL_MEM_CACHE_SIZE = 0x101E,
		GLOBAL_MEM_SIZE = 0x101F,
		MAX_CONSTANT_BUFFER_SIZE = 0x1020,
		MAX_CONSTANT_ARGS = 0x1021,
		LOCAL_MEM_TYPE = 0x1022,
		LOCAL_MEM_SIZE = 0x1023,
		ERROR_CORRECTION_SUPPORT = 0x1024,
		PROFILING_TIMER_RESOLUTION = 0x1025,
		ENDIAN_LITTLE = 0x1026,
		AVAILABLE = 0x1027,
		COMPILER_AVAILABLE = 0x1028,
		EXECUTION_CAPABILITIES = 0x1029,
		QUEUE_PROPERTIES = 0x102A,
		NAME = 0x102B,
		VENDOR = 0x102C,
		DRIVER_VERSION = 0x102D,
		PROFILE = 0x102E,
		VERSION = 0x102F,
		EXTENSIONS = 0x1030,
		PLATFORM = 0x1031,
		DOUBLE_FP_CONFIG = 0x1032,
		HALF_FP_CONFIG = 0x1033,
		PREFERRED_VECTOR_WIDTH_HALF = 0x1034,
		HOST_UNIFIED_MEMORY = 0x1035,
		NATIVE_VECTOR_WIDTH_CHAR = 0x1036,
		NATIVE_VECTOR_WIDTH_SHORT = 0x1037,
		NATIVE_VECTOR_WIDTH_INT = 0x1038,
		NATIVE_VECTOR_WIDTH_LONG = 0x1039,
		NATIVE_VECTOR_WIDTH_FLOAT = 0x103A,
		NATIVE_VECTOR_WIDTH_DOUBLE = 0x103B,
		NATIVE_VECTOR_WIDTH_HALF = 0x103C,
		OPENCL_C_VERSION = 0x103D,

		// cl_nv_device_attribute_query
		COMPUTE_CAPABILITY_MAJOR_NV = 0x4000,
		COMPUTE_CAPABILITY_MINOR_NV = 0x4001,
		REGISTERS_PER_BLOCK_NV = 0x4002,
		WARP_SIZE_NV = 0x4003,
		GPU_OVERLAP_NV = 0x4004,
		KERNEL_EXEC_TIMEOUT_NV = 0x4005,
		INTEGRATED_MEMORY_NV = 0x4006,

		// cl_amd_device_attribute_query
		PROFILING_TIMER_OFFSET_AMD = 0x4036
	}

	// cl_device_fp_config - bitfield
	public enum FpConfig : ulong {
		DENORM = (1 << 0),
		INF_NAN = (1 << 1),
		ROUND_TO_NEAREST = (1 << 2),
		ROUND_TO_ZERO = (1 << 3),
		ROUND_TO_INF = (1 << 4),
		FMA = (1 << 5),
		SOFT_FLOAT = (1 << 6)
	}

	// cl_device_mem_cache_type
	public enum DeviceMemCacheType {
		NONE = 0x0,
		READ_ONLY_CACHE = 0x1,
		READ_WRITE_CACHE = 0x2
	}

	// cl_device_local_mem_type
	public enum DeviceLocalMemType {
		LOCAL = 0x1,
		GLOBAL = 0x2
	}

	// cl_device_exec_capabilities - bitfield
	public enum DeviceExecCapabilities : ulong {
		KERNEL = (1 << 0),
		NATIVE_KERNEL = (1 << 1)
	}

	// cl_command_queue_properties - bitfield
	public enum CommandQueueProperties : ulong {
		NONE = 0,
		OUT_OF_ORDER_EXEC_MODE_ENABLE = (1 << 0),
		PROFILING_ENABLE = (1 << 1)
	}

	// cl_context_info
	public enum ContextInfo {
		REFERENCE_COUNT = 0x1080,
		DEVICES = 0x1081,
		PROPERTIES = 0x1082,
		NUM_DEVICES = 0x1083,

		// D3D10 extension
		D3D10_DEVICE_KHR = 0x4014,
		D3D10_PREFER_SHARED_RESOURCES_KHR = 0x402C
	}

	// cl_gl_context_info
	public enum GLContextInfo {
		CURRENT_DEVICE_FOR_GL_CONTEXT_KHR = 0x2006,
		DEVICES_FOR_GL_CONTEXT_KHR = 0x2007
	}

	// cl_context_properties
	public enum ContextProperties : ulong {
		PLATFORM = 0x1084,

		// Additional cl_context_properties for GL support
		GL_CONTEXT_KHR = 0x2008,
		EGL_DISPLAY_KHR = 0x2009,
		GLX_DISPLAY_KHR = 0x200A,
		WGL_HDC_KHR = 0x200B,
		CGL_SHAREGROUP_KHR = 0x200C
	}

	// cl_command_queue_info
	public enum CommandQueueInfo {
		CONTEXT = 0x1090,
		DEVICE = 0x1091,
		REFERENCE_COUNT = 0x1092,
		PROPERTIES = 0x1093
	}

	// cl_mem_flags - bitfield
	public enum MemFlags : ulong {
		READ_WRITE = (1 << 0),
		WRITE_ONLY = (1 << 1),
		READ_ONLY = (1 << 2),
		USE_HOST_PTR = (1 << 3),
		ALLOC_HOST_PTR = (1 << 4),
		COPY_HOST_PTR = (1 << 5)
	}

	// cl_channel_order
	public enum ChannelOrder {
		R = 0x10B0,
		A = 0x10B1,
		RG = 0x10B2,
		RA = 0x10B3,
		RGB = 0x10B4,
		RGBA = 0x10B5,
		BGRA = 0x10B6,
		ARGB = 0x10B7,
		INTENSITY = 0x10B8,
		LUMINANCE = 0x10B9,
		Rx = 0x10BA,
		RGx = 0x10BB,
		RGBx = 0x10BC
	}

	// cl_channel_type
	public enum ChannelType {
		SNORM_INT8 = 0x10D0,
		SNORM_INT16 = 0x10D1,
		UNORM_INT8 = 0x10D2,
		UNORM_INT16 = 0x10D3,
		UNORM_SHORT_565 = 0x10D4,
		UNORM_SHORT_555 = 0x10D5,
		UNORM_INT_101010 = 0x10D6,
		SIGNED_INT8 = 0x10D7,
		SIGNED_INT16 = 0x10D8,
		SIGNED_INT32 = 0x10D9,
		UNSIGNED_INT8 = 0x10DA,
		UNSIGNED_INT16 = 0x10DB,
		UNSIGNED_INT32 = 0x10DC,
		HALF_FLOAT = 0x10DD,
		FLOAT = 0x10DE
	}

	// cl_mem_object_type
	public enum MemObjectType {
		BUFFER = 0x10F0,
		IMAGE2D = 0x10F1,
		IMAGE3D = 0x10F2
	}

	// cl_mem_info
	public enum MemInfo {
		TYPE = 0x1100,
		FLAGS = 0x1101,
		SIZE = 0x1102,
		HOST_PTR = 0x1103,
		MAP_COUNT = 0x1104,
		REFERENCE_COUNT = 0x1105,
		CONTEXT = 0x1106,
		ASSOCIATED_MEMOBJECT = 0x1107,
		OFFSET = 0x1108,

		// D3D10 extension
		D3D10_RESOURCE_KHR = 0x4015
	}

	// cl_image_info
	public enum ImageInfo {
		FORMAT = 0x1110,
		ELEMENT_SIZE = 0x1111,
		ROW_PITCH = 0x1112,
		SLICE_PITCH = 0x1113,
		WIDTH = 0x1114,
		HEIGHT = 0x1115,
		DEPTH = 0x1116,

		// D3D10 extension
		D3D10_SUBRESOURCE_KHR = 0x4016
	}

	// cl_addressing_mode
	public enum AddressingMode : uint {
		NONE = 0x1130,
		CLAMP_TO_EDGE = 0x1131,
		CLAMP = 0x1132,
		REPEAT = 0x1133,
		MIRRORED_REPEAT = 0x1134
	}

	// cl_filter_mode
	public enum FilterMode : uint {
		NEAREST = 0x1140,
		LINEAR = 0x1141
	}

	// cl_sampler_info
	public enum SamplerInfo : uint {
		REFERENCE_COUNT = 0x1150,
		CONTEXT = 0x1151,
		NORMALIZED_COORDS = 0x1152,
		ADDRESSING_MODE = 0x1153,
		FILTER_MODE = 0x1154
	}

	// cl_map_flags - bitfield
	public enum MapFlags : ulong {
		READ = (1 << 0),
		WRITE = (1 << 1),
		READ_WRITE = (MapFlags.READ + MapFlags.WRITE)
	}

	// cl_program_info
	public enum ProgramInfo {
		REFERENCE_COUNT = 0x1160,
		CONTEXT = 0x1161,
		NUM_DEVICES = 0x1162,
		DEVICES = 0x1163,
		SOURCE = 0x1164,
		BINARY_SIZES = 0x1165,
		BINARIES = 0x1166
	}

	// cl_program_build_info
	public enum ProgramBuildInfo {
		STATUS = 0x1181,
		OPTIONS = 0x1182,
		LOG = 0x1183
	}

	// cl_build_status
	public enum BuildStatus {
		SUCCESS = 0,
		NONE = -1,
		ERROR = -2,
		IN_PROGRESS = -3
	}

	// cl_kernel_info
	public enum KernelInfo {
		FUNCTION_NAME = 0x1190,
		NUM_ARGS = 0x1191,
		REFERENCE_COUNT = 0x1192,
		CONTEXT = 0x1193,
		PROGRAM = 0x1194
	}

	// cl_kernel_work_group_info
	public enum KernelWorkGroupInfo {
		WORK_GROUP_SIZE = 0x11B0,
		COMPILE_WORK_GROUP_SIZE = 0x11B1,
		LOCAL_MEM_SIZE = 0x11B2,
		PREFERRED_WORK_GROUP_SIZE_MULTIPLE = 0x11B3,
		PRIVATE_MEM_SIZE = 0x11B4
	}

	// cl_event_info
	public enum EventInfo {
		COMMAND_QUEUE = 0x11D0,
		COMMAND_TYPE = 0x11D1,
		REFERENCE_COUNT = 0x11D2,
		COMMAND_EXECUTION_STATUS = 0x11D3,
		CONTEXT = 0x11D4
	}

	// cl_command_type
	public enum CommandType {
		NDRANGE_KERNEL = 0x11F0,
		TASK = 0x11F1,
		NATIVE_KERNEL = 0x11F2,
		READ_BUFFER = 0x11F3,
		WRITE_BUFFER = 0x11F4,
		COPY_BUFFER = 0x11F5,
		READ_IMAGE = 0x11F6,
		WRITE_IMAGE = 0x11F7,
		COPY_IMAGE = 0x11F8,
		COPY_IMAGE_TO_BUFFER = 0x11F9,
		COPY_BUFFER_TO_IMAGE = 0x11FA,
		MAP_BUFFER = 0x11FB,
		MAP_IMAGE = 0x11FC,
		UNMAP_MEM_OBJECT = 0x11FD,
		MARKER = 0x11FE,
		ACQUIRE_GL_OBJECTS = 0x11FF,
		RELEASE_GL_OBJECTS = 0x1200,
		READ_BUFFER_RECT = 0x1201,
		WRITE_BUFFER_RECT = 0x1202,
		COPY_BUFFER_RECT = 0x1203,
		USER = 0x1204,

		// D3D10 extension
		ACQUIRE_D3D10_OBJECTS_KHR = 0x4017,
		RELEASE_D3D10_OBJECTS_KHR = 0x4018
	}

	// command execution status
	public enum ExecutionStatus {
		COMPLETE = 0x0,
		RUNNING = 0x1,
		SUBMITTED = 0x2,
		QUEUED = 0x3
	}

	public enum BufferCreateType {
		REGION = 0x1220
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct BufferRegion {
		public IntPtr Origin;
		public IntPtr Size;
	}

	// cl_profiling_info
	public enum ProfilingInfo {
		QUEUED = 0x1280,
		SUBMIT = 0x1281,
		START = 0x1282,
		END = 0x1283
	}

	// ********************************************
	// * CLGL enums
	// ********************************************
	public enum CLGLObjectType {
		BUFFER = 0x2000,
		TEXTURE2D = 0x2001,
		TEXTURE3D = 0x2002,
		RENDERBUFFER = 0x2003
	}

	public enum CLGLTextureInfo {
		TEXTURE_TARGET = 0x2004,
		MIPMAP_LEVEL = 0x2005
	}

	// ********************************************
	// * D3D10 enums
	// ********************************************
	public enum D3D10DeviceSource {
		D3D10_DEVICE_KHR = 0x4010,
		D3D10_DXGI_ADAPTER_KHR = 0x4011
	}

	public enum D3D10DeviceSet {
		PREFERRED_DEVICES_FOR_D3D10_KHR = 0x4012,
		ALL_DEVICES_FOR_D3D10_KHR = 0x4013
	}

	// ********************************************
	// * cl_ext_device_fission enums
	// ********************************************
	public enum DevicePartition : ulong {
		EQUALLY = 0x4050,
		BY_COUNTS = 0x4051,
		BY_NAMES = 0x4052,
		BY_AFFINITY_DOMAIN = 0x4053
	}

	public enum AffinityDomain {
		L1_CACHE = 0x1,
		L2_CACHE = 0x2,
		L3_CACHE = 0x3,
		L4_CACHE = 0x4,
		NUMA = 0x10,
		NEXT_FISSIONABLE = 0x100
	}

	public enum DeviceInfoPropertyNames {
		PARENT_DEVICE = 0x4054,
		PARITION_TYPES = 0x4055,
		AFFINITY_DOMAINS = 0x4056,
		REFERENCE_COUNT = 0x4057,
		PARTITION_STYLE = 0x4058
	}

	public enum ListTerminators {
		PROPERTIES_LIST_END = 0x0,
		PARTITION_BY_COUNTS_LIST_END = 0x0,
		PARTITION_BY_NAMES_LIST_END = -1
	}

	#region Vector2
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Char2 {
		public SByte S0;
		public SByte S1;

		public Char2(SByte s0, SByte s1) {
			this.S0 = s0;
			this.S1 = s1;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct UChar2 {
		public Byte S0;
		public Byte S1;

		public UChar2(Byte s0, Byte s1) {
			this.S0 = s0;
			this.S1 = s1;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Short2 {
		public System.Int16 S0;
		public System.Int16 S1;

		public Short2(System.Int16 s0, System.Int16 s1) {
			this.S0 = s0;
			this.S1 = s1;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct UShort2 {
		public System.UInt16 S0;
		public System.UInt16 S1;

		public UShort2(System.UInt16 s0, System.UInt16 s1) {
			this.S0 = s0;
			this.S1 = s1;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Int2 {
		public Int32 S0;
		public Int32 S1;

		public Int2(Int32 s0, Int32 s1) {
			this.S0 = s0;
			this.S1 = s1;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct UInt2 {
		public UInt32 S0;
		public UInt32 S1;

		public UInt2(UInt32 s0, UInt32 s1) {
			this.S0 = s0;
			this.S1 = s1;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Long2 {
		public Int64 S0;
		public Int64 S1;

		public Long2(Int64 s0, Int64 s1) {
			this.S0 = s0;
			this.S1 = s1;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ULong2 {
		public UInt64 S0;
		public UInt64 S1;

		public ULong2(UInt64 s0, UInt64 s1) {
			this.S0 = s0;
			this.S1 = s1;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Float2 {
		public Single S0;
		public Single S1;

		public Float2(Single s0, Single s1) {
			this.S0 = s0;
			this.S1 = s1;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Double2 {
		public Double S0;
		public Double S1;

		public Double2(Double s0, Double s1) {
			this.S0 = s0;
			this.S1 = s1;
		}
	}
	#endregion

	#region Vector3
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Char3 {
		public SByte S0;
		public SByte S1;
		public SByte S2;

		public Char3(SByte s0, SByte s1, SByte s2) {
			this.S0 = s0;
			this.S1 = s1;
			this.S2 = s2;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct UChar3 {
		public Byte S0;
		public Byte S1;
		public Byte S2;

		public UChar3(Byte s0, Byte s1, Byte s2) {
			this.S0 = s0;
			this.S1 = s1;
			this.S2 = s2;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Short3 {
		public System.Int16 S0;
		public System.Int16 S1;
		public System.Int16 S2;

		public Short3(System.Int16 s0, System.Int16 s1, System.Int16 s2) {
			this.S0 = s0;
			this.S1 = s1;
			this.S2 = s2;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct UShort3 {
		public System.UInt16 S0;
		public System.UInt16 S1;
		public System.UInt16 S2;

		public UShort3(System.UInt16 s0, System.UInt16 s1, System.UInt16 s2) {
			this.S0 = s0;
			this.S1 = s1;
			this.S2 = s2;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Int3 {
		public Int32 S0;
		public Int32 S1;
		public Int32 S2;

		public Int3(Int32 s0, Int32 s1, Int32 s2) {
			this.S0 = s0;
			this.S1 = s1;
			this.S2 = s2;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct UInt3 {
		public UInt32 S0;
		public UInt32 S1;
		public UInt32 S2;

		public UInt3(UInt32 s0, UInt32 s1, UInt32 s2) {
			this.S0 = s0;
			this.S1 = s1;
			this.S2 = s2;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Long3 {
		public Int64 S0;
		public Int64 S1;
		public Int64 S2;

		public Long3(Int64 s0, Int64 s1, Int64 s2) {
			this.S0 = s0;
			this.S1 = s1;
			this.S2 = s2;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ULong3 {
		public UInt64 S0;
		public UInt64 S1;
		public UInt64 S2;

		public ULong3(UInt64 s0, UInt64 s1, UInt64 s2) {
			this.S0 = s0;
			this.S1 = s1;
			this.S2 = s2;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Float3 {
		public Single S0;
		public Single S1;
		public Single S2;

		public Float3(Single s0, Single s1, Single s2) {
			this.S0 = s0;
			this.S1 = s1;
			this.S2 = s2;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Double3 {
		public Double S0;
		public Double S1;
		public Double S2;

		public Double3(Double s0, Double s1, Double s2) {
			this.S0 = s0;
			this.S1 = s1;
			this.S2 = s2;
		}
	}
	#endregion

	#region Vector4
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Char4 {
		public SByte S0;
		public SByte S1;
		public SByte S2;
		public SByte S3;

		public Char4(SByte s0, SByte s1, SByte s2, SByte s3) {
			this.S0 = s0;
			this.S1 = s1;
			this.S2 = s2;
			this.S3 = s3;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct UChar4 {
		public Byte S0;
		public Byte S1;
		public Byte S2;
		public Byte S3;

		public UChar4(Byte s0, Byte s1, Byte s2, Byte s3) {
			this.S0 = s0;
			this.S1 = s1;
			this.S2 = s2;
			this.S3 = s3;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Short4 {
		public System.Int16 S0;
		public System.Int16 S1;
		public System.Int16 S2;
		public System.Int16 S3;

		public Short4(System.Int16 s0, System.Int16 s1, System.Int16 s2, System.Int16 s3) {
			this.S0 = s0;
			this.S1 = s1;
			this.S2 = s2;
			this.S3 = s3;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct UShort4 {
		public System.UInt16 S0;
		public System.UInt16 S1;
		public System.UInt16 S2;
		public System.UInt16 S3;

		public UShort4(System.UInt16 s0, System.UInt16 s1, System.UInt16 s2, System.UInt16 s3) {
			this.S0 = s0;
			this.S1 = s1;
			this.S2 = s2;
			this.S3 = s3;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Int4 {
		public Int32 S0;
		public Int32 S1;
		public Int32 S2;
		public Int32 S3;

		public Int4(Int32 s0, Int32 s1, Int32 s2, Int32 s3) {
			this.S0 = s0;
			this.S1 = s1;
			this.S2 = s2;
			this.S3 = s3;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct UInt4 {
		public UInt32 S0;
		public UInt32 S1;
		public UInt32 S2;
		public UInt32 S3;

		public UInt4(UInt32 s0, UInt32 s1, UInt32 s2, UInt32 s3) {
			this.S0 = s0;
			this.S1 = s1;
			this.S2 = s2;
			this.S3 = s3;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Long4 {
		public Int64 S0;
		public Int64 S1;
		public Int64 S2;
		public Int64 S3;

		public Long4(Int64 s0, Int64 s1, Int64 s2, Int64 s3) {
			this.S0 = s0;
			this.S1 = s1;
			this.S2 = s2;
			this.S3 = s3;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ULong4 {
		public UInt64 S0;
		public UInt64 S1;
		public UInt64 S2;
		public UInt64 S3;

		public ULong4(UInt64 s0, UInt64 s1, UInt64 s2, UInt64 s3) {
			this.S0 = s0;
			this.S1 = s1;
			this.S2 = s2;
			this.S3 = s3;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Float4 {
		public Single S0;
		public Single S1;
		public Single S2;
		public Single S3;

		public Float4(Single s0, Single s1, Single s2, Single s3) {
			this.S0 = s0;
			this.S1 = s1;
			this.S2 = s2;
			this.S3 = s3;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Double4 {
		public Double S0;
		public Double S1;
		public Double S2;
		public Double S3;

		public Double4(Double s0, Double s1, Double s2, Double s3) {
			this.S0 = s0;
			this.S1 = s1;
			this.S2 = s2;
			this.S3 = s3;
		}
	}
	#endregion

	#region Vector8
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Char8 {
		public SByte S0;
		public SByte S1;
		public SByte S2;
		public SByte S3;
		public SByte S4;
		public SByte S5;
		public SByte S6;
		public SByte S7;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct UChar8 {
		public Byte S0;
		public Byte S1;
		public Byte S2;
		public Byte S3;
		public Byte S4;
		public Byte S5;
		public Byte S6;
		public Byte S7;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Short8 {
		public System.Int16 S0;
		public System.Int16 S1;
		public System.Int16 S2;
		public System.Int16 S3;
		public System.Int16 S4;
		public System.Int16 S5;
		public System.Int16 S6;
		public System.Int16 S7;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct UShort8 {
		public System.UInt16 S0;
		public System.UInt16 S1;
		public System.UInt16 S2;
		public System.UInt16 S3;
		public System.UInt16 S4;
		public System.UInt16 S5;
		public System.UInt16 S6;
		public System.UInt16 S7;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Int8 {
		public Int32 S0;
		public Int32 S1;
		public Int32 S2;
		public Int32 S3;
		public Int32 S4;
		public Int32 S5;
		public Int32 S6;
		public Int32 S7;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct UInt8 {
		public UInt32 S0;
		public UInt32 S1;
		public UInt32 S2;
		public UInt32 S3;
		public UInt32 S4;
		public UInt32 S5;
		public UInt32 S6;
		public UInt32 S7;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Long8 {
		public Int64 S0;
		public Int64 S1;
		public Int64 S2;
		public Int64 S3;
		public Int64 S4;
		public Int64 S5;
		public Int64 S6;
		public Int64 S7;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ULong8 {
		public UInt64 S0;
		public UInt64 S1;
		public UInt64 S2;
		public UInt64 S3;
		public UInt64 S4;
		public UInt64 S5;
		public UInt64 S6;
		public UInt64 S7;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Float8 {
		public Single S0;
		public Single S1;
		public Single S2;
		public Single S3;
		public Single S4;
		public Single S5;
		public Single S6;
		public Single S7;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Double8 {
		public Double S0;
		public Double S1;
		public Double S2;
		public Double S3;
		public Double S4;
		public Double S5;
		public Double S6;
		public Double S7;
	}
	#endregion

	#region Vector16
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Char16 {
		public SByte S0;
		public SByte S1;
		public SByte S2;
		public SByte S3;
		public SByte S4;
		public SByte S5;
		public SByte S6;
		public SByte S7;
		public SByte S8;
		public SByte S9;
		public SByte S10;
		public SByte S11;
		public SByte S12;
		public SByte S13;
		public SByte S14;
		public SByte S15;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct UChar16 {
		public Byte S0;
		public Byte S1;
		public Byte S2;
		public Byte S3;
		public Byte S4;
		public Byte S5;
		public Byte S6;
		public Byte S7;
		public Byte S8;
		public Byte S9;
		public Byte S10;
		public Byte S11;
		public Byte S12;
		public Byte S13;
		public Byte S14;
		public Byte S15;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Short16 {
		public System.Int16 S0;
		public System.Int16 S1;
		public System.Int16 S2;
		public System.Int16 S3;
		public System.Int16 S4;
		public System.Int16 S5;
		public System.Int16 S6;
		public System.Int16 S7;
		public System.Int16 S8;
		public System.Int16 S9;
		public System.Int16 S10;
		public System.Int16 S11;
		public System.Int16 S12;
		public System.Int16 S13;
		public System.Int16 S14;
		public System.Int16 S15;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct UShort16 {
		public System.UInt16 S0;
		public System.UInt16 S1;
		public System.UInt16 S2;
		public System.UInt16 S3;
		public System.UInt16 S4;
		public System.UInt16 S5;
		public System.UInt16 S6;
		public System.UInt16 S7;
		public System.UInt16 S8;
		public System.UInt16 S9;
		public System.UInt16 S10;
		public System.UInt16 S11;
		public System.UInt16 S12;
		public System.UInt16 S13;
		public System.UInt16 S14;
		public System.UInt16 S15;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Int16 {
		public Int32 S0;
		public Int32 S1;
		public Int32 S2;
		public Int32 S3;
		public Int32 S4;
		public Int32 S5;
		public Int32 S6;
		public Int32 S7;
		public Int32 S8;
		public Int32 S9;
		public Int32 S10;
		public Int32 S11;
		public Int32 S12;
		public Int32 S13;
		public Int32 S14;
		public Int32 S15;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct UInt16 {
		public UInt32 S0;
		public UInt32 S1;
		public UInt32 S2;
		public UInt32 S3;
		public UInt32 S4;
		public UInt32 S5;
		public UInt32 S6;
		public UInt32 S7;
		public UInt32 S8;
		public UInt32 S9;
		public UInt32 S10;
		public UInt32 S11;
		public UInt32 S12;
		public UInt32 S13;
		public UInt32 S14;
		public UInt32 S15;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Long16 {
		public Int64 S0;
		public Int64 S1;
		public Int64 S2;
		public Int64 S3;
		public Int64 S4;
		public Int64 S5;
		public Int64 S6;
		public Int64 S7;
		public Int64 S8;
		public Int64 S9;
		public Int64 S10;
		public Int64 S11;
		public Int64 S12;
		public Int64 S13;
		public Int64 S14;
		public Int64 S15;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ULong16 {
		public UInt64 S0;
		public UInt64 S1;
		public UInt64 S2;
		public UInt64 S3;
		public UInt64 S4;
		public UInt64 S5;
		public UInt64 S6;
		public UInt64 S7;
		public UInt64 S8;
		public UInt64 S9;
		public UInt64 S10;
		public UInt64 S11;
		public UInt64 S12;
		public UInt64 S13;
		public UInt64 S14;
		public UInt64 S15;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Float16 {
		public Single S0;
		public Single S1;
		public Single S2;
		public Single S3;
		public Single S4;
		public Single S5;
		public Single S6;
		public Single S7;
		public Single S8;
		public Single S9;
		public Single S10;
		public Single S11;
		public Single S12;
		public Single S13;
		public Single S14;
		public Single S15;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Double16 {
		public Double S0;
		public Double S1;
		public Double S2;
		public Double S3;
		public Double S4;
		public Double S5;
		public Double S6;
		public Double S7;
		public Double S8;
		public Double S9;
		public Double S10;
		public Double S11;
		public Double S12;
		public Double S13;
		public Double S14;
		public Double S15;
	}
	#endregion
}