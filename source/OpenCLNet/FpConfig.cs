namespace OpenCLNet {
	public enum FpConfig : ulong {
		DENORM = (1 << 0),
		INF_NAN = (1 << 1),
		ROUND_TO_NEAREST = (1 << 2),
		ROUND_TO_ZERO = (1 << 3),
		ROUND_TO_INF = (1 << 4),
		FMA = (1 << 5),
		SOFT_FLOAT = (1 << 6)
	}
}