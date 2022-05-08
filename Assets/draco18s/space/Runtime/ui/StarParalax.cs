using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.draco18s.ui;

namespace Assets.draco18s.space.ui {
	public class StarParalax : Paralax {
		public override void Start() {
			base.Start();
			//RectTransform galaxyRT = (RectTransform)transform.parent.parent;
			//follow = galaxyRT;
			float wid = Screen.width;
			float p = (transform.localPosition.z*102.4f);// + (wid/2);
			if(p < 0) {
				//p = 1/Mathf.Abs(p);
				p = -Mathf.Log(Mathf.Abs(p+1))*3.3f;
			}
			else {
				p = Mathf.Log(p+1)*1f;
				//p /= (wid/64);
			}
			rate = p;
			maxOffset = new Vector3(Screen.width, Screen.height, 0);
			//if(gameObject.name == "Sol") {
			//	Debug.Log(rate);
			//}
		}

		/*public override void Tick(float deltaTime) {
			base.Tick(deltaTime);
			transform.localPosition = new Vector3(Mathf.RoundToInt(transform.localPosition.x), Mathf.RoundToInt(transform.localPosition.y), Mathf.RoundToInt(transform.localPosition.z));
		}*/
	}
}