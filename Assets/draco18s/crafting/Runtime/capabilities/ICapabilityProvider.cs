using Assets.draco18s.util;

namespace Assets.draco18s.crafting.capabilities {
	public interface ICapabilityProvider {
		LazyOptional<T> GetCapability<T>(Capability<T> cap);
	}
}