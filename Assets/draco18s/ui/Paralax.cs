using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.draco18s.util;

namespace Assets.draco18s.ui {
	public class Paralax : MonoBehaviour,ITickable
	{
		public Transform referenceObj;
		public Vector3 origin;
		public Vector3 maxOffset = new Vector3(128,128,0);
		public float rate;

		public virtual void Start() {
			origin = transform.localPosition;
			referenceObj = GameObject.Find("ScreenCenter").transform;
		}
		
		public virtual void Tick(float deltaTime)
		{
			if(referenceObj == null)
				return;
			transform.localPosition = origin;
			Vector3 p = ((transform.position - referenceObj.position) * rate)/64;
			p.x = Mathf.Clamp(p.x, -maxOffset.x, maxOffset.x);
			p.y = Mathf.Clamp(p.y, -maxOffset.y, maxOffset.y);
			p.z = 0;
			transform.localPosition = origin + p;
		}
	}
}