using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace OpenCLNet {
	#region Using statements
	using cl_addressing_mode = UInt32;
	using cl_bool = UInt32;
	using cl_command_queue = IntPtr;
	using cl_context = IntPtr;
	using cl_device_id = IntPtr;
	using cl_event = IntPtr;
	using cl_event_info = UInt32;
	using cl_filter_mode = UInt32;
	using cl_gl_object_type = UInt32;
	using cl_gl_texture_info = UInt32;
	using cl_int = Int32;
	using cl_kernel = IntPtr;
	using cl_mem = IntPtr;
	using cl_mem_flags = UInt64;
	using cl_sampler = IntPtr;
	using cl_sampler_info = UInt32;
	using cl_uint = UInt32;
	using GLenum = Int32;
	using GLint = Int32;
	using GLuint = UInt32;
	#endregion

	public static unsafe class OpenCL {
		static readonly Dictionary<IntPtr, Platform> _Platforms = new Dictionary<IntPtr, Platform>();
		static IntPtr[] PlatformIDs;
		static Platform[] Platforms;

		public static Int32 NumberOfPlatforms => OpenCL._Platforms.Count;

		static OpenCL() {
			try {
				OpenCL.Initialize();
			}
			catch (Exception) {
				OpenCL._Platforms.Clear();
				OpenCL.PlatformIDs = new IntPtr[0];
				OpenCL.Platforms = new Platform[0];
			}
		}

		private static IntPtr[] GetPlatformIDs() {
			IntPtr[] platformIDs;
			ErrorCode result;
			UInt32 returnedPlatforms;

			result = OpenCL.GetPlatformIDs(0, null, out returnedPlatforms);
			if (result == ErrorCode.INVALID_VALUE)
				return null;

			platformIDs = new IntPtr[returnedPlatforms];
			result = OpenCL.GetPlatformIDs((UInt32)platformIDs.Length, platformIDs, out returnedPlatforms);
			if (result == ErrorCode.INVALID_VALUE)
				return null;
			return platformIDs;
		}

		private static void Initialize() {
			OpenCL.PlatformIDs = OpenCL.GetPlatformIDs();
			if (OpenCL.PlatformIDs == null)
				return;

			OpenCL.Platforms = new Platform[OpenCL.PlatformIDs.Length];
			for (var i = 0; i < OpenCL.PlatformIDs.Length; i++) {
				Platform p;

				p = new Platform(OpenCL.PlatformIDs[i]);
				OpenCL.Platforms[i] = p;
				OpenCL._Platforms[OpenCL.PlatformIDs[i]] = p;
			}
		}

		public static ErrorCode GetEventProfilingInfo(cl_event _event, ProfilingInfo param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret) {
			return (ErrorCode)OpenCLAPI.clGetEventProfilingInfo(_event, (UInt32)param_name, param_value_size, param_value, out param_value_size_ret);
		}

		// Extension function access
		public static IntPtr GetExtensionFunctionAddress(String func_name) {
			return OpenCLAPI.clGetExtensionFunctionAddress(func_name);
		}

		public static Platform GetPlatform(Int32 index) {
			return OpenCL._Platforms[OpenCL.PlatformIDs[index]];
		}

		public static Platform GetPlatform(IntPtr platformID) {
			return OpenCL._Platforms[platformID];
		}

		public static Platform[] GetPlatforms() {
			return (Platform[])OpenCL.Platforms.Clone();
		}

		#region Platform API
		public static ErrorCode GetPlatformIDs(UInt32 num_entries, IntPtr[] platforms, out UInt32 num_platforms) {
			return (ErrorCode)OpenCLAPI.clGetPlatformIDs(num_entries, platforms, out num_platforms);
		}

		public static ErrorCode GetPlatformInfo(IntPtr platform, UInt32 param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret) {
			return (ErrorCode)OpenCLAPI.clGetPlatformInfo(platform, param_name, param_value_size, param_value, out param_value_size_ret);
		}
		#endregion

		#region Device API
		public static ErrorCode GetDeviceIDs(IntPtr platform, DeviceType device_type, UInt32 num_entries, IntPtr[] devices, out UInt32 num_devices) {
			return (ErrorCode)OpenCLAPI.clGetDeviceIDs(platform, device_type, num_entries, devices, out num_devices);
		}

		public static ErrorCode GetDeviceInfo(IntPtr device, UInt32 param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret) {
			return (ErrorCode)OpenCLAPI.clGetDeviceInfo(device, param_name, param_value_size, param_value, out param_value_size_ret);
		}
		#endregion

		#region Context API
		public static IntPtr CreateContext(IntPtr[] properties, UInt32 num_devices, IntPtr[] devices, ContextNotify pfn_notify, IntPtr user_data, out ErrorCode errcode_ret) {
			return OpenCLAPI.clCreateContext(properties, num_devices, devices, pfn_notify, user_data, out errcode_ret);
		}

		public static IntPtr CreateContextFromType(IntPtr[] properties, DeviceType device_type, ContextNotify pfn_notify, IntPtr user_data, out ErrorCode errcode_ret) {
			return OpenCLAPI.clCreateContextFromType(properties, device_type, pfn_notify, user_data, out errcode_ret);
		}

		public static ErrorCode RetainContext(IntPtr context) {
			return (ErrorCode)OpenCLAPI.clRetainContext(context);
		}

		public static ErrorCode ReleaseContext(IntPtr context) {
			return (ErrorCode)OpenCLAPI.clReleaseContext(context);
		}

		public static ErrorCode GetContextInfo(IntPtr context, UInt32 param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret) {
			return (ErrorCode)OpenCLAPI.clGetContextInfo(context, param_name, param_value_size, param_value, out param_value_size_ret);
		}
		#endregion

		#region Program Object API
		public static IntPtr CreateProgramWithSource(IntPtr context, UInt32 count, String[] strings, IntPtr[] lengths, out ErrorCode errcode_ret) {
			return OpenCLAPI.clCreateProgramWithSource(context, count, strings, lengths, out errcode_ret);
		}

		public static IntPtr CreateProgramWithBinary(IntPtr context, UInt32 num_devices, IntPtr[] device_list, IntPtr[] lengths, Byte[][] binaries, Int32[] binary_status, out ErrorCode errcode_ret) {
			IntPtr program;

			var pinnedArrays = new GCHandle[binaries.Length];
			// Pin arrays
			for (var i = 0; i < binaries.Length; i++)
				pinnedArrays[i] = GCHandle.Alloc(binaries[i], GCHandleType.Pinned);

			var pointerArray = new IntPtr[binaries.Length];
			for (var i = 0; i < binaries.Length; i++)
				pointerArray[i] = pinnedArrays[i].AddrOfPinnedObject();

			program = OpenCLAPI.clCreateProgramWithBinary(context, num_devices, device_list, lengths, pointerArray, binary_status, out errcode_ret);

			for (var i = 0; i < binaries.Length; i++)
				pinnedArrays[i].Free();

			return program;
		}

		public static ErrorCode RetainProgram(IntPtr program) {
			return (ErrorCode)OpenCLAPI.clRetainProgram(program);
		}

		public static ErrorCode ReleaseProgram(IntPtr program) {
			return (ErrorCode)OpenCLAPI.clReleaseProgram(program);
		}

		public static ErrorCode BuildProgram(IntPtr program, UInt32 num_devices, IntPtr[] device_list, String options, ProgramNotify pfn_notify, IntPtr user_data) {
			return (ErrorCode)OpenCLAPI.clBuildProgram(program, num_devices, device_list, options, pfn_notify, user_data);
		}

		public static ErrorCode UnloadCompiler() {
			return (ErrorCode)OpenCLAPI.clUnloadCompiler();
		}

		public static ErrorCode GetProgramInfo(IntPtr program, UInt32 param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret) {
			return (ErrorCode)OpenCLAPI.clGetProgramInfo(program, param_name, param_value_size, param_value, out param_value_size_ret);
		}

		public static ErrorCode GetProgramBuildInfo(IntPtr program, IntPtr device, UInt32 param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret) {
			return (ErrorCode)OpenCLAPI.clGetProgramBuildInfo(program, device, param_name, param_value_size, param_value, out param_value_size_ret);
		}
		#endregion

		#region Command Queue API
		public static IntPtr CreateCommandQueue(IntPtr context, IntPtr device, UInt64 properties, out ErrorCode errcode_ret) {
			return OpenCLAPI.clCreateCommandQueue(context, device, properties, out errcode_ret);
		}

		public static ErrorCode RetainCommandQueue(IntPtr command_queue) {
			return OpenCLAPI.clRetainCommandQueue(command_queue);
		}

		public static ErrorCode ReleaseCommandQueue(IntPtr command_queue) {
			return OpenCLAPI.clReleaseCommandQueue(command_queue);
		}

		public static ErrorCode GetCommandQueueInfo(IntPtr command_queue, UInt32 param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret) {
			return OpenCLAPI.clGetCommandQueueInfo(command_queue, param_name, param_value_size, param_value, out param_value_size_ret);
		}

		[Obsolete("Function deprecated in OpenCL 1.1 due to being inherently unsafe", false)]
		public static ErrorCode SetCommandQueueProperty(IntPtr command_queue, UInt64 properties, Boolean enable, out UInt64 old_properties) {
#pragma warning disable 618
			return OpenCLAPI.clSetCommandQueueProperty(command_queue, properties, enable, out old_properties);
#pragma warning restore 618
		}
		#endregion

		#region Memory Object API
		public static IntPtr CreateBuffer(IntPtr context, UInt64 flags, IntPtr size, void* host_ptr, out ErrorCode errcode_ret) {
			return OpenCLAPI.clCreateBuffer(context, flags, size, host_ptr, out errcode_ret);
		}

		public static IntPtr CreateImage2D(IntPtr context, UInt64 flags, ImageFormat image_format, IntPtr image_width, IntPtr image_height, IntPtr image_row_pitch, void* host_ptr, out ErrorCode errcode_ret) {
			return OpenCLAPI.clCreateImage2D(context, flags, &image_format, image_width, image_height, image_row_pitch, host_ptr, out errcode_ret);
		}

		public static IntPtr CreateImage3D(IntPtr context, UInt64 flags, ImageFormat image_format, IntPtr image_width, IntPtr image_height, IntPtr image_depth, IntPtr image_row_pitch, IntPtr image_slice_pitch, void* host_ptr, out ErrorCode errcode_ret) {
			return OpenCLAPI.clCreateImage3D(context, flags, &image_format, image_width, image_height, image_depth, image_row_pitch, image_slice_pitch, host_ptr, out errcode_ret);
		}

		public static ErrorCode RetainMemObject(IntPtr memobj) {
			return OpenCLAPI.clRetainMemObject(memobj);
		}

		public static ErrorCode ReleaseMemObject(IntPtr memobj) {
			return OpenCLAPI.clReleaseMemObject(memobj);
		}

		public static ErrorCode GetSupportedImageFormats(IntPtr context, UInt64 flags, UInt32 image_type, UInt32 num_entries, ImageFormat[] image_formats, out UInt32 num_image_formats) {
			return OpenCLAPI.clGetSupportedImageFormats(context, flags, image_type, num_entries, image_formats, out num_image_formats);
		}

		public static ErrorCode GetMemObjectInfo(IntPtr memobj, UInt32 param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret) {
			return OpenCLAPI.clGetMemObjectInfo(memobj, param_name, param_value_size, param_value, out param_value_size_ret);
		}

		public static ErrorCode GetImageInfo(IntPtr image, UInt32 param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret) {
			return OpenCLAPI.clGetImageInfo(image, param_name, param_value_size, param_value, out param_value_size_ret);
		}

		public static IntPtr CreateSubBuffer(IntPtr memobj, MemFlags flags, BufferRegion buffer_create_info, out ErrorCode errcode_ret) {
			return OpenCLAPI.clCreateSubBuffer(memobj, (cl_mem_flags)flags, BufferCreateType.REGION, &buffer_create_info, out errcode_ret);
		}
		#endregion

		#region Kernel Object API
		public static cl_kernel CreateKernel(IntPtr program, String kernel_name, out ErrorCode errcode_ret) {
			return OpenCLAPI.clCreateKernel(program, kernel_name, out errcode_ret);
		}

		public static ErrorCode CreateKernelsInProgram(IntPtr program, UInt32 num_kernels, IntPtr[] kernels, out UInt32 num_kernels_ret) {
			return OpenCLAPI.clCreateKernelsInProgram(program, num_kernels, kernels, out num_kernels_ret);
		}

		public static ErrorCode RetainKernel(IntPtr kernel) {
			return OpenCLAPI.clRetainKernel(kernel);
		}

		public static ErrorCode ReleaseKernel(IntPtr kernel) {
			return OpenCLAPI.clReleaseKernel(kernel);
		}

		public static ErrorCode SetKernelArg(IntPtr kernel, UInt32 arg_index, IntPtr arg_size, void* arg_value) {
			return OpenCLAPI.clSetKernelArg(kernel, arg_index, arg_size, arg_value);
		}

		public static ErrorCode GetKernelInfo(IntPtr kernel, UInt32 param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret) {
			return OpenCLAPI.clGetKernelInfo(kernel, param_name, param_value_size, param_value, out param_value_size_ret);
		}

		public static ErrorCode GetKernelWorkGroupInfo(IntPtr kernel, IntPtr device, UInt32 param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret) {
			return OpenCLAPI.clGetKernelWorkGroupInfo(kernel, device, param_name, param_value_size, param_value, out param_value_size_ret);
		}
		#endregion

		#region Enqueued Commands API
		public static ErrorCode EnqueueReadBuffer(IntPtr command_queue, IntPtr buffer, UInt32 blocking_read, IntPtr offset, IntPtr cb, void* ptr, UInt32 num_events_in_wait_list, IntPtr[] event_wait_list, IntPtr* _event) {
			return OpenCLAPI.clEnqueueReadBuffer(command_queue, buffer, blocking_read, offset, cb, ptr, num_events_in_wait_list, event_wait_list, _event);
		}

		public static ErrorCode EnqueueReadBuffer(IntPtr command_queue, IntPtr buffer, UInt32 blocking_read, Int32 offset, Int32 cb, IntPtr ptr, Int32 num_events_in_wait_list, IntPtr* event_wait_list, IntPtr* _event) {
			return OpenCLAPI.clEnqueueReadBuffer(command_queue, buffer, blocking_read, (IntPtr)offset, (IntPtr)cb, ptr.ToPointer(), (UInt32)num_events_in_wait_list, event_wait_list, _event);
		}

		public static ErrorCode EnqueueReadBuffer(IntPtr command_queue, IntPtr buffer, UInt32 blocking_read, Int64 offset, Int64 cb, IntPtr ptr, Int32 num_events_in_wait_list, IntPtr* event_wait_list, IntPtr* _event) {
			return OpenCLAPI.clEnqueueReadBuffer(command_queue, buffer, blocking_read, (IntPtr)offset, (IntPtr)cb, ptr.ToPointer(), (UInt32)num_events_in_wait_list, event_wait_list, _event);
		}

		public static ErrorCode EnqueueWriteBuffer(IntPtr command_queue, IntPtr buffer, UInt32 blocking_write, IntPtr offset, IntPtr cb, void* ptr, UInt32 num_events_in_wait_list, IntPtr[] event_wait_list, IntPtr* _event) {
			return OpenCLAPI.clEnqueueWriteBuffer(command_queue, buffer, blocking_write, offset, cb, ptr, num_events_in_wait_list, event_wait_list, _event);
		}

		public static ErrorCode EnqueueWriteBuffer(IntPtr command_queue, IntPtr buffer, UInt32 blocking_write, Int32 offset, Int32 cb, IntPtr ptr, Int32 num_events_in_wait_list, IntPtr* event_wait_list, IntPtr* _event) {
			return OpenCLAPI.clEnqueueWriteBuffer(command_queue, buffer, blocking_write, (IntPtr)offset, (IntPtr)cb, ptr.ToPointer(), (UInt32)num_events_in_wait_list, event_wait_list, _event);
		}

		public static ErrorCode EnqueueWriteBuffer(IntPtr command_queue, IntPtr buffer, UInt32 blocking_write, Int64 offset, Int64 cb, IntPtr ptr, Int32 num_events_in_wait_list, IntPtr* event_wait_list, IntPtr* _event) {
			return OpenCLAPI.clEnqueueWriteBuffer(command_queue, buffer, blocking_write, (IntPtr)offset, (IntPtr)cb, ptr.ToPointer(), (UInt32)num_events_in_wait_list, event_wait_list, _event);
		}

		public static ErrorCode EnqueueCopyBuffer(IntPtr command_queue, IntPtr src_buffer, IntPtr dst_buffer, IntPtr src_offset, IntPtr dst_offset, IntPtr cb, UInt32 num_events_in_wait_list, IntPtr[] event_wait_list, IntPtr* _event) {
			return OpenCLAPI.clEnqueueCopyBuffer(command_queue, src_buffer, dst_buffer, src_offset, dst_offset, cb, num_events_in_wait_list, event_wait_list, _event);
		}

		public static ErrorCode EnqueueCopyBuffer(IntPtr command_queue, IntPtr src_buffer, IntPtr dst_buffer, Int32 src_offset, Int32 dst_offset, Int32 cb, Int32 num_events_in_wait_list, IntPtr* event_wait_list, IntPtr* _event) {
			return OpenCLAPI.clEnqueueCopyBuffer(command_queue, src_buffer, dst_buffer, (IntPtr)src_offset, (IntPtr)dst_offset, (IntPtr)cb, (UInt32)num_events_in_wait_list, event_wait_list, _event);
		}

		public static ErrorCode EnqueueCopyBuffer(IntPtr command_queue, IntPtr src_buffer, IntPtr dst_buffer, Int64 src_offset, Int64 dst_offset, Int64 cb, Int32 num_events_in_wait_list, IntPtr* event_wait_list, IntPtr* _event) {
			return OpenCLAPI.clEnqueueCopyBuffer(command_queue, src_buffer, dst_buffer, (IntPtr)src_offset, (IntPtr)dst_offset, (IntPtr)cb, (UInt32)num_events_in_wait_list, event_wait_list, _event);
		}

		public static ErrorCode EnqueueReadImage(IntPtr command_queue, IntPtr image, UInt32 blocking_read, IntPtr[] origin, IntPtr[] region, IntPtr row_pitch, IntPtr slice_pitch, void* ptr, UInt32 num_events_in_wait_list, IntPtr[] event_wait_list, IntPtr* _event) {
			return OpenCLAPI.clEnqueueReadImage(command_queue, image, blocking_read, origin, region, row_pitch, slice_pitch, ptr, num_events_in_wait_list, event_wait_list, _event);
		}

		public static ErrorCode EnqueueReadImage(IntPtr command_queue, IntPtr image, UInt32 blocking_read, IntPtr* origin, IntPtr* region, Int32 row_pitch, Int32 slice_pitch, IntPtr ptr, Int32 num_events_in_wait_list, IntPtr* event_wait_list, IntPtr* _event) {
			return OpenCLAPI.clEnqueueReadImage(command_queue, image, blocking_read, origin, region, (IntPtr)row_pitch, (IntPtr)slice_pitch, ptr.ToPointer(), (UInt32)num_events_in_wait_list, event_wait_list, _event);
		}

		public static ErrorCode EnqueueReadImage(IntPtr command_queue, IntPtr image, UInt32 blocking_read, IntPtr* origin, IntPtr* region, Int64 row_pitch, Int64 slice_pitch, IntPtr ptr, Int32 num_events_in_wait_list, IntPtr* event_wait_list, IntPtr* _event) {
			return OpenCLAPI.clEnqueueReadImage(command_queue, image, blocking_read, origin, region, (IntPtr)row_pitch, (IntPtr)slice_pitch, ptr.ToPointer(), (UInt32)num_events_in_wait_list, event_wait_list, _event);
		}

		public static ErrorCode EnqueueWriteImage(IntPtr command_queue, IntPtr image, UInt32 blocking_write, IntPtr[] origin, IntPtr[] region, IntPtr input_row_pitch, IntPtr input_slice_pitch, void* ptr, UInt32 num_events_in_wait_list, IntPtr[] event_wait_list, IntPtr* _event) {
			return OpenCLAPI.clEnqueueWriteImage(command_queue, image, blocking_write, origin, region, input_row_pitch, input_slice_pitch, ptr, num_events_in_wait_list, event_wait_list, _event);
		}

		public static ErrorCode EnqueueWriteImage(IntPtr command_queue, IntPtr image, UInt32 blocking_write, IntPtr* origin, IntPtr* region, Int32 input_row_pitch, Int32 input_slice_pitch, IntPtr ptr, Int32 num_events_in_wait_list, IntPtr* event_wait_list, IntPtr* _event) {
			return OpenCLAPI.clEnqueueWriteImage(command_queue, image, blocking_write, origin, region, (IntPtr)input_row_pitch, (IntPtr)input_slice_pitch, ptr.ToPointer(), (UInt32)num_events_in_wait_list, event_wait_list, _event);
		}

		public static ErrorCode EnqueueWriteImage(IntPtr command_queue, IntPtr image, UInt32 blocking_write, IntPtr* origin, IntPtr* region, Int64 input_row_pitch, Int64 input_slice_pitch, IntPtr ptr, Int32 num_events_in_wait_list, IntPtr* event_wait_list, IntPtr* _event) {
			return OpenCLAPI.clEnqueueWriteImage(command_queue, image, blocking_write, origin, region, (IntPtr)input_row_pitch, (IntPtr)input_slice_pitch, ptr.ToPointer(), (UInt32)num_events_in_wait_list, event_wait_list, _event);
		}

		public static ErrorCode EnqueueCopyImage(IntPtr command_queue, IntPtr src_image, IntPtr dst_image, IntPtr[] src_origin, IntPtr[] dst_origin, IntPtr[] region, UInt32 num_events_in_wait_list, IntPtr[] event_wait_list, IntPtr* _event) {
			return OpenCLAPI.clEnqueueCopyImage(command_queue, src_image, dst_image, src_origin, dst_origin, region, num_events_in_wait_list, event_wait_list, _event);
		}

		public static ErrorCode EnqueueCopyImage(IntPtr command_queue, IntPtr src_image, IntPtr dst_image, IntPtr* src_origin, IntPtr* dst_origin, IntPtr* region, Int32 num_events_in_wait_list, IntPtr* event_wait_list, IntPtr* _event) {
			return OpenCLAPI.clEnqueueCopyImage(command_queue, src_image, dst_image, src_origin, dst_origin, region, (UInt32)num_events_in_wait_list, event_wait_list, _event);
		}

		public static ErrorCode EnqueueCopyImageToBuffer(IntPtr command_queue, IntPtr src_image, IntPtr dst_buffer, IntPtr[] src_origin, IntPtr[] region, IntPtr dst_offset, UInt32 num_events_in_wait_list, IntPtr[] event_wait_list, IntPtr* _event) {
			return OpenCLAPI.clEnqueueCopyImageToBuffer(command_queue, src_image, dst_buffer, src_origin, region, dst_offset, num_events_in_wait_list, event_wait_list, _event);
		}

		public static ErrorCode EnqueueCopyImageToBuffer(IntPtr command_queue, IntPtr src_image, IntPtr dst_buffer, IntPtr* src_origin, IntPtr* region, Int32 dst_offset, Int32 num_events_in_wait_list, IntPtr* event_wait_list, IntPtr* _event) {
			return OpenCLAPI.clEnqueueCopyImageToBuffer(command_queue, src_image, dst_buffer, src_origin, region, (IntPtr)dst_offset, (UInt32)num_events_in_wait_list, event_wait_list, _event);
		}

		public static ErrorCode EnqueueCopyImageToBuffer(IntPtr command_queue, IntPtr src_image, IntPtr dst_buffer, IntPtr* src_origin, IntPtr* region, Int64 dst_offset, Int32 num_events_in_wait_list, IntPtr* event_wait_list, IntPtr* _event) {
			return OpenCLAPI.clEnqueueCopyImageToBuffer(command_queue, src_image, dst_buffer, src_origin, region, (IntPtr)dst_offset, (UInt32)num_events_in_wait_list, event_wait_list, _event);
		}

		public static ErrorCode EnqueueCopyBufferToImage(IntPtr command_queue, IntPtr src_buffer, IntPtr dst_image, IntPtr src_offset, IntPtr[] dst_origin, IntPtr[] region, UInt32 num_events_in_wait_list, IntPtr[] event_wait_list, IntPtr* _event) {
			return OpenCLAPI.clEnqueueCopyBufferToImage(command_queue, src_buffer, dst_image, src_offset, dst_origin, region, num_events_in_wait_list, event_wait_list, _event);
		}

		public static ErrorCode EnqueueCopyBufferToImage(IntPtr command_queue, IntPtr src_buffer, IntPtr dst_image, Int32 src_offset, IntPtr* dst_origin, IntPtr* region, Int32 num_events_in_wait_list, IntPtr* event_wait_list, IntPtr* _event) {
			return OpenCLAPI.clEnqueueCopyBufferToImage(command_queue, src_buffer, dst_image, (IntPtr)src_offset, dst_origin, region, (UInt32)num_events_in_wait_list, event_wait_list, _event);
		}

		public static ErrorCode EnqueueCopyBufferToImage(IntPtr command_queue, IntPtr src_buffer, IntPtr dst_image, Int64 src_offset, IntPtr* dst_origin, IntPtr* region, Int32 num_events_in_wait_list, IntPtr* event_wait_list, IntPtr* _event) {
			return OpenCLAPI.clEnqueueCopyBufferToImage(command_queue, src_buffer, dst_image, (IntPtr)src_offset, dst_origin, region, (UInt32)num_events_in_wait_list, event_wait_list, _event);
		}

		public static void* EnqueueMapBuffer(IntPtr command_queue, IntPtr buffer, UInt32 blocking_map, UInt64 map_flags, IntPtr offset, IntPtr cb, UInt32 num_events_in_wait_list, IntPtr[] event_wait_list, IntPtr* _event, out ErrorCode errcode_ret) {
			return OpenCLAPI.clEnqueueMapBuffer(command_queue, buffer, blocking_map, map_flags, offset, cb, num_events_in_wait_list, event_wait_list, _event, out errcode_ret);
		}

		public static void* EnqueueMapBuffer(IntPtr command_queue, IntPtr buffer, UInt32 blocking_map, UInt64 map_flags, Int32 offset, Int32 cb, Int32 num_events_in_wait_list, IntPtr* event_wait_list, IntPtr* _event, out ErrorCode errcode_ret) {
			return OpenCLAPI.clEnqueueMapBuffer(command_queue, buffer, blocking_map, map_flags, (IntPtr)offset, (IntPtr)cb, (UInt32)num_events_in_wait_list, event_wait_list, _event, out errcode_ret);
		}

		public static void* EnqueueMapBuffer(IntPtr command_queue, IntPtr buffer, UInt32 blocking_map, UInt64 map_flags, Int64 offset, Int64 cb, Int32 num_events_in_wait_list, IntPtr* event_wait_list, IntPtr* _event, out ErrorCode errcode_ret) {
			return OpenCLAPI.clEnqueueMapBuffer(command_queue, buffer, blocking_map, map_flags, (IntPtr)offset, (IntPtr)cb, (UInt32)num_events_in_wait_list, event_wait_list, _event, out errcode_ret);
		}

		public static void* EnqueueMapImage(IntPtr command_queue, IntPtr image, UInt32 blocking_map, UInt64 map_flags, IntPtr[] origin, IntPtr[] region, out IntPtr image_row_pitch, out IntPtr image_slice_pitch, UInt32 num_events_in_wait_list, IntPtr[] event_wait_list, IntPtr* _event, out ErrorCode errcode_ret) {
			return OpenCLAPI.clEnqueueMapImage(command_queue, image, blocking_map, map_flags, origin, region, out image_row_pitch, out image_slice_pitch, num_events_in_wait_list, event_wait_list, _event, out errcode_ret);
		}

		public static void* EnqueueMapImage(IntPtr command_queue, IntPtr image, UInt32 blocking_map, UInt64 map_flags, IntPtr* origin, IntPtr* region, out Int32 image_row_pitch, out Int32 image_slice_pitch, Int32 num_events_in_wait_list, IntPtr* event_wait_list, IntPtr* _event, out ErrorCode errcode_ret) {
			IntPtr rowPitch;
			IntPtr slicePitch;
			void* p;
			p = OpenCLAPI.clEnqueueMapImage(command_queue, image, blocking_map, map_flags, origin, region, out rowPitch, out slicePitch, (UInt32)num_events_in_wait_list, event_wait_list, _event, out errcode_ret);
			image_row_pitch = rowPitch.ToInt32();
			image_slice_pitch = slicePitch.ToInt32();
			return p;
		}

		public static void* EnqueueMapImage(IntPtr command_queue, IntPtr image, UInt32 blocking_map, UInt64 map_flags, IntPtr* origin, IntPtr* region, out Int64 image_row_pitch, out Int64 image_slice_pitch, Int32 num_events_in_wait_list, IntPtr* event_wait_list, IntPtr* _event, out ErrorCode errcode_ret) {
			IntPtr rowPitch;
			IntPtr slicePitch;
			void* p;
			p = OpenCLAPI.clEnqueueMapImage(command_queue, image, blocking_map, map_flags, origin, region, out rowPitch, out slicePitch, (UInt32)num_events_in_wait_list, event_wait_list, _event, out errcode_ret);
			image_row_pitch = rowPitch.ToInt64();
			image_slice_pitch = slicePitch.ToInt64();
			return p;
		}

		public static ErrorCode EnqueueUnmapMemObject(IntPtr command_queue, IntPtr memobj, void* mapped_ptr, UInt32 num_events_in_wait_list, IntPtr[] event_wait_list, IntPtr* _event) {
			return OpenCLAPI.clEnqueueUnmapMemObject(command_queue, memobj, mapped_ptr, num_events_in_wait_list, event_wait_list, _event);
		}

		public static ErrorCode EnqueueNDRangeKernel(IntPtr command_queue, IntPtr kernel, UInt32 work_dim, IntPtr[] global_work_offset, IntPtr[] global_work_size, IntPtr[] local_work_size, UInt32 num_events_in_wait_list, IntPtr[] event_wait_list, IntPtr* _event) {
			return OpenCLAPI.clEnqueueNDRangeKernel(command_queue, kernel, work_dim, global_work_offset, global_work_size, local_work_size, num_events_in_wait_list, event_wait_list, _event);
		}

		public static ErrorCode EnqueueNDRangeKernel(IntPtr command_queue, IntPtr kernel, Int32 work_dim, IntPtr* global_work_offset, IntPtr* global_work_size, IntPtr* local_work_size, Int32 num_events_in_wait_list, IntPtr* event_wait_list, IntPtr* _event) {
			return OpenCLAPI.clEnqueueNDRangeKernel(command_queue, kernel, (UInt32)work_dim, global_work_offset, global_work_size, local_work_size, (UInt32)num_events_in_wait_list, event_wait_list, _event);
		}

		public static ErrorCode EnqueueTask(IntPtr command_queue, IntPtr kernel, UInt32 num_events_in_wait_list, IntPtr[] event_wait_list, IntPtr* _event) {
			return OpenCLAPI.clEnqueueTask(command_queue, kernel, num_events_in_wait_list, event_wait_list, _event);
		}

		public static ErrorCode EnqueueTask(IntPtr command_queue, IntPtr kernel, Int32 num_events_in_wait_list, IntPtr* event_wait_list, IntPtr* _event) {
			return OpenCLAPI.clEnqueueTask(command_queue, kernel, (UInt32)num_events_in_wait_list, event_wait_list, _event);
		}

		public static ErrorCode EnqueueNativeKernel(
			cl_command_queue command_queue,
			NativeKernelInternal user_func,
			void* args,
			IntPtr cb_args,
			cl_uint num_mem_objects,
			cl_mem[] mem_list,
			IntPtr[] args_mem_loc,
			cl_uint num_events_in_wait_list,
			cl_event[] event_wait_list,
			cl_event* _event) {
			return OpenCLAPI.clEnqueueNativeKernel(command_queue,
				user_func,
				args,
				cb_args,
				num_mem_objects,
				mem_list,
				args_mem_loc,
				num_events_in_wait_list,
				event_wait_list,
				_event);
		}

		public static ErrorCode EnqueueMarker(IntPtr command_queue, IntPtr* _event) {
			return OpenCLAPI.clEnqueueMarker(command_queue, _event);
		}

		public static ErrorCode EnqueueWaitForEvents(IntPtr command_queue, UInt32 num_events, IntPtr[] _event_list) {
			return OpenCLAPI.clEnqueueWaitForEvents(command_queue, num_events, _event_list);
		}

		public static ErrorCode EnqueueWaitForEvents(IntPtr command_queue, Int32 num_events, IntPtr* _event_list) {
			return OpenCLAPI.clEnqueueWaitForEvents(command_queue, (UInt32)num_events, _event_list);
		}

		public static ErrorCode EnqueueBarrier(IntPtr command_queue) {
			return OpenCLAPI.clEnqueueBarrier(command_queue);
		}

		#region EnqueueReadBufferRect
		/// <summary>
		///     OpenCL 1.1
		/// </summary>
		/// <param name="command_queue"></param>
		/// <param name="buffer"></param>
		/// <param name="blocking_read"></param>
		/// <param name="buffer_offset"></param>
		/// <param name="host_offset"></param>
		/// <param name="region"></param>
		/// <param name="buffer_row_pitch"></param>
		/// <param name="buffer_slice_pitch"></param>
		/// <param name="host_row_pitch"></param>
		/// <param name="host_slice_pitch"></param>
		/// <param name="ptr"></param>
		/// <param name="num_events_in_wait_list"></param>
		/// <param name="event_wait_list"></param>
		/// <param name="_event"></param>
		/// <returns></returns>
		public static ErrorCode EnqueueReadBufferRect(cl_command_queue command_queue, cl_mem buffer, cl_bool blocking_read, IntPtr[] buffer_offset, IntPtr[] host_offset, IntPtr[] region, IntPtr buffer_row_pitch, IntPtr buffer_slice_pitch, IntPtr host_row_pitch, IntPtr host_slice_pitch, void* ptr, cl_uint num_events_in_wait_list, IntPtr[] event_wait_list, IntPtr* _event) {
			return OpenCLAPI.clEnqueueReadBufferRect(command_queue, buffer, blocking_read, buffer_offset, host_offset, region, buffer_row_pitch, buffer_slice_pitch, host_row_pitch, host_slice_pitch, ptr, num_events_in_wait_list, event_wait_list, _event);
		}

		public static ErrorCode EnqueueReadBufferRect(cl_command_queue command_queue, cl_mem buffer, cl_bool blocking_read, IntPtr* buffer_offset, IntPtr* host_offset, IntPtr* region, Int32 buffer_row_pitch, Int32 buffer_slice_pitch, Int32 host_row_pitch, Int32 host_slice_pitch, void* ptr, Int32 num_events_in_wait_list, IntPtr* event_wait_list, IntPtr* _event) {
			return OpenCLAPI.clEnqueueReadBufferRect(command_queue, buffer, blocking_read, buffer_offset, host_offset, region, (IntPtr)buffer_row_pitch, (IntPtr)buffer_slice_pitch, (IntPtr)host_row_pitch, (IntPtr)host_slice_pitch, ptr, (UInt32)num_events_in_wait_list, event_wait_list, _event);
		}

		public static ErrorCode EnqueueReadBufferRect(cl_command_queue command_queue, cl_mem buffer, cl_bool blocking_read, IntPtr* buffer_offset, IntPtr* host_offset, IntPtr* region, Int64 buffer_row_pitch, Int64 buffer_slice_pitch, Int64 host_row_pitch, Int64 host_slice_pitch, void* ptr, Int32 num_events_in_wait_list, IntPtr* event_wait_list, IntPtr* _event) {
			return OpenCLAPI.clEnqueueReadBufferRect(command_queue, buffer, blocking_read, buffer_offset, host_offset, region, (IntPtr)buffer_row_pitch, (IntPtr)buffer_slice_pitch, (IntPtr)host_row_pitch, (IntPtr)host_slice_pitch, ptr, (UInt32)num_events_in_wait_list, event_wait_list, _event);
		}
		#endregion

		#region EnqueueWriteBufferRect
		/// <summary>
		///     OpenCL 1.1
		/// </summary>
		/// <param name="command_queue"></param>
		/// <param name="buffer"></param>
		/// <param name="blocking_read"></param>
		/// <param name="buffer_offset"></param>
		/// <param name="host_offset"></param>
		/// <param name="region"></param>
		/// <param name="buffer_row_pitch"></param>
		/// <param name="buffer_slice_pitch"></param>
		/// <param name="host_row_pitch"></param>
		/// <param name="host_slice_pitch"></param>
		/// <param name="ptr"></param>
		/// <param name="num_events_in_wait_list"></param>
		/// <param name="_event_wait_list"></param>
		/// <param name="_event"></param>
		/// <returns></returns>
		public static ErrorCode EnqueueWriteBufferRect(cl_command_queue command_queue, cl_mem buffer, cl_bool blocking_write, IntPtr[] buffer_offset, IntPtr[] host_offset, IntPtr[] region, IntPtr buffer_row_pitch, IntPtr buffer_slice_pitch, IntPtr host_row_pitch, IntPtr host_slice_pitch, void* ptr, cl_uint num_events_in_wait_list, cl_event[] _event_wait_list, cl_event* _event) {
			return OpenCLAPI.clEnqueueWriteBufferRect(command_queue, buffer, blocking_write, buffer_offset, host_offset, region, buffer_row_pitch, buffer_slice_pitch, host_row_pitch, host_slice_pitch, ptr, num_events_in_wait_list, _event_wait_list, _event);
		}

		public static ErrorCode EnqueueWriteBufferRect(cl_command_queue command_queue, cl_mem buffer, cl_bool blocking_write, IntPtr* buffer_offset, IntPtr* host_offset, IntPtr* region, Int32 buffer_row_pitch, Int32 buffer_slice_pitch, Int32 host_row_pitch, Int32 host_slice_pitch, void* ptr, Int32 num_events_in_wait_list, cl_event* _event_wait_list, cl_event* _event) {
			return OpenCLAPI.clEnqueueWriteBufferRect(command_queue, buffer, blocking_write, buffer_offset, host_offset, region, (IntPtr)buffer_row_pitch, (IntPtr)buffer_slice_pitch, (IntPtr)host_row_pitch, (IntPtr)host_slice_pitch, ptr, (UInt32)num_events_in_wait_list, _event_wait_list, _event);
		}

		public static ErrorCode EnqueueWriteBufferRect(cl_command_queue command_queue, cl_mem buffer, cl_bool blocking_write, IntPtr* buffer_offset, IntPtr* host_offset, IntPtr* region, Int64 buffer_row_pitch, Int64 buffer_slice_pitch, Int64 host_row_pitch, Int64 host_slice_pitch, void* ptr, Int32 num_events_in_wait_list, cl_event* _event_wait_list, cl_event* _event) {
			return OpenCLAPI.clEnqueueWriteBufferRect(command_queue, buffer, blocking_write, buffer_offset, host_offset, region, (IntPtr)buffer_row_pitch, (IntPtr)buffer_slice_pitch, (IntPtr)host_row_pitch, (IntPtr)host_slice_pitch, ptr, (UInt32)num_events_in_wait_list, _event_wait_list, _event);
		}
		#endregion

		#region EnqueueCopyBufferRect
		/// <summary>
		///     OpenCL 1.1
		/// </summary>
		/// <param name="command_queue"></param>
		/// <param name="src_buffer"></param>
		/// <param name="dst_buffer"></param>
		/// <param name="src_origin"></param>
		/// <param name="dst_origin"></param>
		/// <param name="region"></param>
		/// <param name="src_row_pitch"></param>
		/// <param name="src_slice_pitch"></param>
		/// <param name="dst_row_pitch"></param>
		/// <param name="dst_slice_pitch"></param>
		/// <param name="num_events_in_wait_list"></param>
		/// <param name="_event_wait_list"></param>
		/// <param name="_event"></param>
		/// <returns></returns>
		public static ErrorCode EnqueueCopyBufferRect(cl_command_queue command_queue, cl_mem src_buffer, cl_mem dst_buffer, IntPtr[] src_origin, IntPtr[] dst_origin, IntPtr[] region, IntPtr src_row_pitch, IntPtr src_slice_pitch, IntPtr dst_row_pitch, IntPtr dst_slice_pitch, cl_uint num_events_in_wait_list, cl_event[] _event_wait_list, cl_event* _event) {
			return OpenCLAPI.clEnqueueCopyBufferRect(command_queue, src_buffer, dst_buffer, src_origin, dst_origin, region, src_row_pitch, src_slice_pitch, dst_row_pitch, dst_slice_pitch, num_events_in_wait_list, _event_wait_list, _event);
		}

		public static ErrorCode EnqueueCopyBufferRect(cl_command_queue command_queue, cl_mem src_buffer, cl_mem dst_buffer, IntPtr* src_origin, IntPtr* dst_origin, IntPtr* region, Int32 src_row_pitch, Int32 src_slice_pitch, Int32 dst_row_pitch, Int32 dst_slice_pitch, Int32 num_events_in_wait_list, cl_event* _event_wait_list, cl_event* _event) {
			return OpenCLAPI.clEnqueueCopyBufferRect(command_queue, src_buffer, dst_buffer, src_origin, dst_origin, region, (IntPtr)src_row_pitch, (IntPtr)src_slice_pitch, (IntPtr)dst_row_pitch, (IntPtr)dst_slice_pitch, (UInt32)num_events_in_wait_list, _event_wait_list, _event);
		}

		public static ErrorCode EnqueueCopyBufferRect(cl_command_queue command_queue, cl_mem src_buffer, cl_mem dst_buffer, IntPtr* src_origin, IntPtr* dst_origin, IntPtr* region, Int64 src_row_pitch, Int64 src_slice_pitch, Int64 dst_row_pitch, Int64 dst_slice_pitch, Int32 num_events_in_wait_list, cl_event* _event_wait_list, cl_event* _event) {
			return OpenCLAPI.clEnqueueCopyBufferRect(command_queue, src_buffer, dst_buffer, src_origin, dst_origin, region, (IntPtr)src_row_pitch, (IntPtr)src_slice_pitch, (IntPtr)dst_row_pitch, (IntPtr)dst_slice_pitch, (UInt32)num_events_in_wait_list, _event_wait_list, _event);
		}
		#endregion
		#endregion

		#region Flush and Finish API
		public static ErrorCode Flush(cl_command_queue command_queue) {
			return OpenCLAPI.clFlush(command_queue);
		}

		public static ErrorCode Finish(cl_command_queue command_queue) {
			return OpenCLAPI.clFinish(command_queue);
		}
		#endregion

		#region Event Object API
		public static ErrorCode WaitForEvents(cl_uint num_events, cl_event[] _event_list) {
			return OpenCLAPI.clWaitForEvents(num_events, _event_list);
		}

		public static ErrorCode GetEventInfo(cl_event _event, cl_event_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret) {
			return OpenCLAPI.clGetEventInfo(_event, param_name, param_value_size, param_value, out param_value_size_ret);
		}

		public static ErrorCode RetainEvent(cl_event _event) {
			return OpenCLAPI.clRetainEvent(_event);
		}

		public static ErrorCode ReleaseEvent(cl_event _event) {
			return OpenCLAPI.clReleaseEvent(_event);
		}

		/// <summary>
		///     OpenCL 1.1
		/// </summary>
		/// <param name="context"></param>
		/// <param name="errcode_ret"></param>
		/// <returns></returns>
		public static cl_event CreateUserEvent(cl_context context, out ErrorCode errcode_ret) {
			return OpenCLAPI.clCreateUserEvent(context, out errcode_ret);
		}

		/// <summary>
		///     OpenCL 1.1
		/// </summary>
		/// <param name="_event"></param>
		/// <param name="execution_status"></param>
		/// <returns></returns>
		public static ErrorCode SetUserEventStatus(cl_event _event, ExecutionStatus execution_status) {
			return OpenCLAPI.clSetUserEventStatus(_event, execution_status);
		}

		/// <summary>
		///     OpenCL 1.1
		/// </summary>
		/// <param name="_event"></param>
		/// <param name="command_exec_callback_type"></param>
		/// <param name="pfn_notify"></param>
		/// <param name="user_data"></param>
		/// <returns></returns>
		public static ErrorCode SetEventCallback(cl_event _event, cl_int command_exec_callback_type, EventNotifyInternal pfn_notify, IntPtr user_data) {
			return OpenCLAPI.clSetEventCallback(_event, command_exec_callback_type, pfn_notify, user_data);
		}
		#endregion

		#region Sampler API
		public static cl_sampler CreateSampler(cl_context context, Boolean normalized_coords, cl_addressing_mode addressing_mode, cl_filter_mode filter_mode, out ErrorCode errcode_ret) {
			return OpenCLAPI.clCreateSampler(context, normalized_coords ? (cl_bool)Bool.TRUE : (cl_bool)Bool.FALSE, addressing_mode, filter_mode, out errcode_ret);
		}

		public static ErrorCode RetainSampler(cl_sampler sampler) {
			return OpenCLAPI.clRetainSampler(sampler);
		}

		public static ErrorCode ReleaseSampler(cl_sampler sampler) {
			return OpenCLAPI.clReleaseSampler(sampler);
		}

		public static ErrorCode GetSamplerInfo(cl_sampler sampler, cl_sampler_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret) {
			return OpenCLAPI.clGetSamplerInfo(sampler, param_name, param_value_size, param_value, out param_value_size_ret);
		}
		#endregion

		#region GLObject API
		public static cl_mem CreateFromGLBuffer(cl_context context, cl_mem_flags flags, GLuint bufobj, out ErrorCode errcode_ret) {
			return OpenCLAPI.clCreateFromGLBuffer(context, flags, bufobj, out errcode_ret);
		}

		public static cl_mem CreateFromGLTexture2D(cl_context context, cl_mem_flags flags, GLenum target, GLint mipLevel, GLuint texture, out ErrorCode errcode_ret) {
			return OpenCLAPI.clCreateFromGLTexture2D(context, flags, target, mipLevel, texture, out errcode_ret);
		}

		public static cl_mem CreateFromGLTexture3D(cl_context context, cl_mem_flags flags, GLenum target, GLint mipLevel, GLuint texture, out ErrorCode errcode_ret) {
			return OpenCLAPI.clCreateFromGLTexture3D(context, flags, target, mipLevel, texture, out errcode_ret);
		}

		public static cl_mem CreateFromGLRenderbuffer(cl_context context, cl_mem_flags flags, GLuint renderbuffer, out ErrorCode errcode_ret) {
			return OpenCLAPI.clCreateFromGLRenderbuffer(context, flags, renderbuffer, out errcode_ret);
		}

		public static ErrorCode GetGLObjectInfo(cl_mem memobj, out cl_gl_object_type gl_object_type, out GLuint gl_object_name) {
			return OpenCLAPI.clGetGLObjectInfo(memobj, out gl_object_type, out gl_object_name);
		}

		public static ErrorCode GetGLTextureInfo(cl_mem memobj, cl_gl_texture_info param_name, IntPtr param_value_size, void* param_value, out IntPtr param_value_size_ret) {
			return OpenCLAPI.clGetGLTextureInfo(memobj, param_name, param_value_size, param_value, out param_value_size_ret);
		}

		public static ErrorCode EnqueueAcquireGLObjects(cl_command_queue command_queue, cl_uint num_objects, cl_mem[] mem_objects, cl_uint num_events_in_wait_list, cl_event[] event_wait_list, cl_event* _event) {
			return OpenCLAPI.clEnqueueAcquireGLObjects(command_queue, num_objects, mem_objects, num_events_in_wait_list, event_wait_list, _event);
		}

		public static ErrorCode EnqueueReleaseGLObjects(cl_command_queue command_queue, cl_uint num_objects, cl_mem[] mem_objects, cl_uint num_events_in_wait_list, cl_event[] event_wait_list, cl_event* _event) {
			return OpenCLAPI.clEnqueueAcquireGLObjects(command_queue, num_objects, mem_objects, num_events_in_wait_list, event_wait_list, _event);
		}
		#endregion

		#region Device Fission API (Extension)
		public static ErrorCode ReleaseDeviceEXT(cl_device_id device) {
			return OpenCLAPI.clReleaseDeviceEXT(device);
		}

		public static ErrorCode RetainDeviceEXT(cl_device_id device) {
			return OpenCLAPI.clRetainDeviceEXT(device);
		}

		public static ErrorCode CreateSubDevicesEXT(cl_device_id in_device, Byte[] properties, cl_uint num_entries, cl_device_id[] out_devices, [Out] cl_uint* num_devices) {
			return OpenCLAPI.clCreateSubDevicesEXT(in_device, properties, num_entries, out_devices, num_devices);
		}
		#endregion
	}
}