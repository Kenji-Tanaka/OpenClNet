using System;

namespace OpenCLNet {
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
}