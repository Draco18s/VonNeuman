using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Assets.draco18s.ui;
using Assets.draco18s.translation;
using Assets.draco18s.space.stellar;
using Assets.draco18s.space.planetary;
using Assets.draco18s.util;
using Assets.draco18s.gameAssets;
using Assets.draco18s.serialization;

namespace Assets.draco18s.space.ui {
	public static class StellarUIBuilder {
		public static void GalaxyStarGraphic(this StarSystem system, GameObject go, Action<StarSystem> ShowSystem, Action ShowGalaxy, Action<ITickable> addTickable) {
			Image m = go.GetComponent<Image>();
			m.sprite = system.spriteData.sprite;
			m.color = ColorExtensions.bv2rgb(system.Info.colorIndex);
			RectTransform rt = ((RectTransform)go.transform);
			rt.localScale = Vector3.one;
			rt.localPosition = system.uiposition;
			
			Button btn = go.GetComponent<Button>();
			btn.AddHover(pos => {
				ITranslatable tt = new RawString($"{system.Info.properName}")
				.Append(
					new RawString("\n")
				).Append(
					new TranslateText("escapeVelocity.text", system.escapeVelocity.ToString("#.##"))
				).Append(
					new RawString("\n")
				).Append(
					new TranslateText("brightness.text", system.Info.brightnessMagnitude.ToString("+#.##;-#.##"))
				);
				Tooltip.ShowTooltip(go.transform.position + Vector3.right * 12 + Vector3.up * 15, tt, 4);
			});
			btn.onClick.AddListener(() => {
				BacktrackStack.Navigate(new GuiAction(() => {
					ShowSystem(system);
				}, ShowGalaxy));
			});
			StarParalax paralax = go.AddComponent<StarParalax>();
			//paralax.follow = go.transform.parent.parent;
			addTickable(paralax);
		}

		public static void PopulateSystemUI(this StarSystem sys, GameObject star, Vector3 angle) {
			star.name = sys.Info.properName;
			Image m = star.GetComponent<Image>();
			m.sprite = ScriptableObjectRegistry.GetRegistry<SpriteMap>().First(x => x.spriteId == sys.spriteData.spriteId.Replace("star","star-large")).sprite;
			RectTransform rt = ((RectTransform)star.transform);
			rt.localScale = Vector3.one*4;
			rt.localPosition = new Vector3(1,1,0)*95;
			rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,m.sprite.rect.width);
			rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,m.sprite.rect.height);
			float mult = 32/sys.titiusBodeK0;
			char planet='a';
			foreach(OrbitalBody b in sys.GetPlanets()) {
				OrbitalBody body = b;
				GameObject go = GameObject.Instantiate(star, rt.parent);
				go.name = $"{sys.Info.properName}-{planet}";
				planet++;
				body.PopulateUI(go, angle, mult);
			}
			m.color = ColorExtensions.bv2rgb(sys.Info.colorIndex);
		}

		public static void PopulateUI(this OrbitalBody ob, GameObject go, Vector3 angle, float mult) {
			if(ob is Planet) 
				((Planet)ob).PopulateUI(go, angle, mult);
		}

		public static void PopulateUI(this Planet pl, GameObject go, Vector3 angle, float mult) {
			Image m = go.GetComponent<Image>();
			m.sprite = pl.spriteData.sprite;
			RectTransform rt = ((RectTransform)go.transform);
			go.transform.localScale = Vector3.one;
			go.transform.localPosition = angle*pl.orbitalDistance*mult*2+angle*24;
			rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,m.sprite.rect.width);
			rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,m.sprite.rect.height);
			
			Button btn = go.GetComponent<Button>();
			btn.AddHover(pos => {
				ITranslatable tt = new RawString($"{go.name}").Append(
					new RawString(" ")
				).Append(
					new TranslateText("escapeVelocity.short", pl.escapeVelocity.ToString("#.##"))
				).Append(
					new RawString("\n")
				).Append(
					new TranslateText("averageSurfaceTemp.text", pl.averageSurfaceTemp.ToString("#.##"))
				).Append(
					new RawString("\n")
				).Append(
					new TranslateText("atmosphere.text")
				).Append(
					new RawString("\n")
				).Append(
					pl.GetAtmosphere().Length > 0 ?
					(ITranslatable)new RawString(string.Join("\n", pl.GetAtmosphere().Take(2).Select(x => $" •{x.name}"))) :
					(ITranslatable)new TranslateText("none.text")
				).Append(
					new RawString("\n")
				).Append(
					pl.GetOcean() != null ?
					(ITranslatable)new TranslateText("ocean.text",pl.GetOcean().name) :
					(ITranslatable)new TranslateText("no_ocean.text")
				);
				Tooltip.ShowTooltip(go.transform.position + Vector3.right * 12 + Vector3.up * 15, tt, 6);
			});
		}
	}
}