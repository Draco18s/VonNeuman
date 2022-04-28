using UnityEngine;

namespace Assets.draco18s.util {
	public interface ITickable {
		Transform transform {get;}
		void Tick(float deltaTime);
	}
}