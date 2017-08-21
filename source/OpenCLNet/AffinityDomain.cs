namespace OpenCLNet {
	public enum AffinityDomain {
		L1_CACHE = 0x1,
		L2_CACHE = 0x2,
		L3_CACHE = 0x3,
		L4_CACHE = 0x4,
		NUMA = 0x10,
		NEXT_FISSIONABLE = 0x100
	}
}