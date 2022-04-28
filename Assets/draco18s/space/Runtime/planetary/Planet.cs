using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Assets.draco18s.space.stellar;
using Assets.draco18s.util;
using Assets.draco18s.gameAssets;
using Assets.draco18s.serialization;
using Assets.draco18s.crafting;
using Assets.draco18s.crafting.properties;

using Material = Assets.draco18s.crafting.Material;

namespace Assets.draco18s.space.planetary {
	[JsonResolver(typeof(PlanetConverter))]
	public class Planet : OrbitalBody
	{
		protected Material[] atmosphere;
		protected Material ocean;
		protected Material surfaceDeposits;

		public static Planet Generate(StarSystem star, int tbk) {
			float r = GetRandomRadius();
			/*if(tbk <= 3 && r < 5 && RandomExtensions.Shared.NextDouble() < 0.333) {
				r *= (float)Math.Max(RandomExtensions.Shared.NextDouble(),RandomExtensions.Shared.NextDouble());
			}*/
			float mass = GetRandomMass(r);
			//Debug.Log($"Planet mass: {mass}");
			double orbit = star.titiusBodeK0 * Math.Pow(1.7275, tbk);
			List<ElementData> atmo = new List<ElementData>();
			ElementMap elems = ScriptableObjectRegistry.GetRegistry<ElementMap>();
			ElementData primaryAtmoGas;
			ElementData secondaryAtmoGas;
			ElementData traceAtmoGas;
			ElementData primaryMetal;
			ElementData secondaryMetal;
			ElementData traceElement;
			
			if(r > 7)
				primaryAtmoGas = elems.GetRandom(ElementType.HALOGEN_GAS|ElementType.NOBLE_GAS, 5).OrderByDescending(x => x.Key.id < 10 ? Mathf.Sqrt(x.Key.id) * x.Value : 0).First().Key;
			else if(r > 3)
				primaryAtmoGas = elems.GetRandom(ElementType.HALOGEN_GAS|ElementType.NOBLE_GAS, 5).OrderByDescending(x => x.Key.id > 1 ? Mathf.Sqrt(x.Key.id) * x.Value : 0).First().Key;
			else
				primaryAtmoGas = elems.GetRandom(ElementType.HALOGEN_GAS|ElementType.NOBLE_GAS, 5).OrderByDescending(x => x.Key.id > 4 ? Mathf.Sqrt(x.Key.id) * x.Value : 0).First().Key;
			secondaryAtmoGas = elems.GetRandom(ElementType.HALOGEN_GAS|ElementType.NOBLE_GAS|ElementType.NONMETAL, 5).OrderByDescending(x => Mathf.Sqrt(x.Key.id) * x.Value /** (x.Key.id == primaryAtmoGas.id ? 0 : 1)*/).First().Key;
			
			primaryMetal = elems.GetRandom(ElementType.METAL|ElementType.METALOID, 5).OrderByDescending(x => x.Key.id < 54 ? Mathf.Sqrt(x.Key.id) * (x.Value+2) : 0).First().Key;
			secondaryMetal = elems.GetRandom(ElementType.METAL|ElementType.METALOID, 5).OrderByDescending(x => Mathf.Sqrt(x.Key.id) * (x.Value+2) /** (x.Key.id == primaryMetal.id ? 0 : 1)*/).First().Key;
			traceElement = elems.GetRandom(ElementType.METAL|ElementType.ALKALI, 5).OrderBy(x => x.Key.relativeQuant < 4 ? Mathf.Sqrt(x.Key.id) * (x.Value+2) /** (x.Key.id == primaryMetal.id ? 0 : 1) * (x.Key.id == secondaryMetal.id ? 0 : 1)*/ : 999).First().Key;
			traceAtmoGas = elems.GetRandom(ElementType.HALOGEN_GAS|ElementType.NOBLE_GAS, 5).OrderBy(x => x.Key.relativeQuant > 4 ? Mathf.Sqrt(x.Key.id) * x.Value /** (x.Key.id == primaryAtmoGas.id ? 0 : 1) * (x.Key.id == secondaryAtmoGas.id ? 0 : 1)*/ : 999).First().Key;

			ElementData[] composites = new ElementData[]{primaryAtmoGas, primaryMetal, secondaryAtmoGas, secondaryMetal, traceElement, traceAtmoGas};
			Material[] matComposites = ElementsToMaterials(composites);
			float greenhouse = EstimateGreenhouseEffect(matComposites);
			float albedo = EstimateAlbedo(matComposites);
			int r1 = (int)Mathf.Clamp(Mathf.Floor(Mathf.Sqrt(r*4)),0,8);
			//Debug.Log($"Planet radius: {r} -> {r1}");
			string sprite = "planet-sprite_" + r1.ToString();
			float temp = (float)SurfaceTemperature((float)orbit, star.Info.brightnessMagnitude,albedo,greenhouse);
			matComposites = CombineElements(matComposites, temp);
			matComposites = EvaporateGasses(ref temp, matComposites, Mathf.Sqrt(2*G*mass/(R*r)), (float)orbit, star.Info.brightnessMagnitude);
			Material[] resour = matComposites.Where(x => temp < x.GetProperty<AtomicProperties>().meltingPoint).ToArray();
			float f = RandomExtensions.Shared.NextSingle();

			Planet p = new Planet() {
				orbitalDistance = (float)orbit,
				radius = r,
				mass = mass,
				averageSurfaceTemp = temp,
				atmosphere = matComposites.Where(x => temp > x.GetProperty<AtomicProperties>().boilingPoint).ToArray(),
				ocean = matComposites.Where(x => temp > x.GetProperty<AtomicProperties>().meltingPoint && temp < x.GetProperty<AtomicProperties>().boilingPoint).OrderBy(x => x.GetProperty<AtomicProperties>().boilingPoint).FirstOrDefault(),
				resources = resour,
				surfaceDeposits = resour.Length > 0 ? resour[(int)(f * resour.Length)] : default,
				spriteData = ScriptableObjectRegistry.GetRegistry<SpriteMap>().First(x => x.spriteId == sprite)
			};

			return p;
		}

		private static List<Recipe> naturalRecipes;
		private static List<Recipe> smeltingRecipes;
		private static Material[] CombineElements(Material[] materials, float temp) {
			if(naturalRecipes == null) naturalRecipes = ScriptableObjectRegistry.GetRegistry2<Recipe>().Where(x => x.building == default && x.outputs.Count() == 1 && !x.name.Contains("Melt")).ToList();
			if(smeltingRecipes == null) smeltingRecipes = ScriptableObjectRegistry.GetRegistry2<Recipe>().Where(x => x.name.Contains("Melt") || x.name.Contains("Liquid")).ToList();
			List<Recipe> valid = naturalRecipes.Where(x => x.inputs.All(r => materials.Contains(r.mat))).ToList();
			List<Material> outputs = new List<Material>();
			outputs.AddRange(materials);
			outputs.AddRange(valid.Select(x => x.outputs.First().mat));
			outputs = outputs.Select(x => {
				if(x.GetProperty<AtomicProperties>().meltingPoint <= temp) {
					try {
						return smeltingRecipes.First(recipe => recipe.inputs.Select(input => input.mat).First() == x).outputs.First().mat;
					} catch(Exception _) {
						if(x.name.Contains("Ore")) {
							Debug.Log($"{x.name} : {x.GetHashCode()}");
							Debug.Log($"allRecipes.Count: {ScriptableObjectRegistry.GetRegistry2<Recipe>().Count()}");
							foreach(var sr in smeltingRecipes) {
								//Debug.Log($"    {sr.name}");
								var f = sr.inputs.Select(input => input.mat).First();
								Debug.Log($"    {f.name} {f.GetHashCode()}");
								Debug.Log($"    {f == x}");
							}
						}
						return x;
					}
				}
				return x;
			}).ToList();
			return outputs.ToArray();
		}

		private static Material[] ElementsToMaterials(ElementData[] composites) {
			return composites.Select(x => x.material).Distinct().Cast<Material>().ToArray();
		}

		private static Material[] EvaporateGasses(ref float tempC, Material[] elements, float escapeVel, float orbit, float brigthness) {
			float tempK = tempC+273;
			var result = elements.Where(x => GetEscapeVelocity(x.GetProperty<AtomicProperties>().molarMass, tempK) < escapeVel/6).ToArray();
			float greenhouse = EstimateGreenhouseEffect(elements);
			float albedo = EstimateAlbedo(elements);
			tempC = (float)SurfaceTemperature(orbit, brigthness, albedo, greenhouse);
			return result;
		}

		private static float GetEscapeVelocity(float molMass, float tempK) => (float)(Math.Sqrt(3*K*tempK*mol/molMass)/100);

		public Planet() { }

		public override Material[] GetAtmosphere() {
			return atmosphere;
		}

		public Material GetOcean() {
			return ocean;
		}

		public List<Material> GetAvailableResources(float scanDepth) {
			List<Material> list = new List<Material>();
			list.AddRange(atmosphere);
			if(scanDepth > 1f)
				list.Add(ocean);
			if(scanDepth > 2f) { }
				//todo others
			if(scanDepth > 3f)
				list.Add(surfaceDeposits);
			return list;
		}

		private static float EstimateGreenhouseEffect(Material[] components) {
			float factor = 8;
			return Mathf.Clamp(components.Sum(x => {
				if(x == default) return 0;
				factor /= 2;
				return x.GetProperty<AtomicProperties>().greenhousePotential*factor;
			}), 0, 500);
		}

		private static float EstimateAlbedo(Material[] components) {
			float v = 0.12f;
			components.ForEach(x=> {
				v+=x.GetProperty<AtomicProperties>().albedo;
			});
			return v/components.Length*100;
		}

		public static float GetRandomRadius() {
			//Inverse gaussian; most values around 2.5, ranging from 0 to 30
			//https://courses.lumenlearning.com/astronomy/chapter/exoplanets-everywhere-what-we-are-learning/
			double r = RandomExtensions.Shared.NextInverseGaussian(4,8);
			//double r2 = 1;
			if(r > 2.4) r = RandomExtensions.Shared.NextInverseGaussian(4,8);
			if(r > 3.4 && r < 12 && RandomExtensions.Shared.NextDouble() > 0.35) {
				r = RandomExtensions.Shared.NextDouble()*1.4+.2;
				if(r < 0.5 && RandomExtensions.Shared.NextDouble() > 0.35) {
					r = RandomExtensions.Shared.NextDouble()*0.8+.7;
				}
			}
			//if(r > 4) r2 *= RandomExtensions.Shared.NextDouble();
			if(r < min) {
				Debug.Log($"Radius: {r}");
				min = (float)r;
			}
			if(r > max) {
				Debug.Log($"Radius: {r}");
				max = (float)r;
			}
			return (float)(r);
		}
		private static float max = 42.9122374265652f;//31.663700f; //19.532506; 17.602069
		private static float min = 0.200124171003757f;//0.200331f;//0.200764f; //0.3649f; 0.469505f; 1.281956; 1.431849f; 2.693742f;
		private static float GetRandomMass(float radius) {
			/*
			Approximated from fig.1 at https://www.pnas.org/doi/10.1073/pnas.1812905116
			*/
			if(radius < 2) {
				//Debug.Log($"Radius check: {radius}");
				double mr3 = (radius <= 1.85) ? (Math.Max(radius-0.4, 0.00001)*2+0.00001) : radius*3;
				double mr5 = (radius <= 1.587401052) ? ((radius <= 0.887481503) ? (radius/4)+0.02 : ((radius-0.887481503)*12.43+1.3)) : Math.Pow(radius,1.5)*5;
				double range = mr5-mr3;
				return (float)(RandomExtensions.Shared.NextDouble()*range + mr3);
			}
			else if(radius < 10) {
				double mr3 = radius*3;
				double mr5 = Math.Pow(radius,1.5)*5;
				double range = mr5-mr3;
				return (float)(RandomExtensions.Shared.NextDouble()*range + mr3);
			}
			else {
				double max=Math.Pow(Math.E,(radius-15)*-0.299573227355399+8.29404964010203);
				double min=radius*6;
				double range = max-min;
				return (float)(RandomExtensions.Shared.NextDouble()*range + min);
			}
		}

		const double sigma = 0.000056703;
		/// <summary>https://www.astro.indiana.edu/ala/PlanetTemp/index.html
		/// <param name="auDist">Distance from the star in AU</param>
		/// <param name="brightness">Absolute brightness magnitude</param>
		/// <param name="albedoPercent">Percent of light reflected by the resources back into space</param>
		/// <param name="greenhouseRatio">Ratio of heat trapped by greenhouse gasses. 1 = Earth, higher hotter</param>
		/// </summary>
		public static float SurfaceTemperature(float auDist, float brightness, float albedoPercent, float greenhouseRatio) {
			double pi = Math.PI;
			double LO = 3.846*Math.Pow(10, 33)*Math.Pow(1, 3);
			double L0 = 3.0128*Math.Pow(10, 28+7);//convert from watts to ergs
			double C = -2.511886432;

			double L = L0*(Math.Pow(10,brightness/C));
			
			double D = auDist*1.496*Math.Pow(10, 13);
			double A = albedoPercent/100;
			double T = greenhouseRatio*0.438075;
			double X = Math.Sqrt((1-A)*L/(16*pi*sigma));
			double T_eff = Math.Sqrt(X)*(1/Math.Sqrt(D));
			double T_eq = (Math.Pow(T_eff, 4))*(1+T);
			double T_sur = T_eq/0.9;
			double T_kel = Math.Sqrt(Math.Sqrt(T_sur));
			T_kel = Math.Round(T_kel);
			double celsius = T_kel-273;
			//Debug.Log($"{(4.86/brightness).ToString("#.##")}+{auDist}+{albedoPercent}+{greenhouseRatio}: {celsius}");
			return (float)celsius;
		}

		private class PlanetConverter : JsonConverter {
			public override bool CanConvert(System.Type objectType) {
				return typeof(Planet).IsAssignableFrom(objectType);
			}

			public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer) {
				JObject jObject = (JObject)existingValue;
				float orbit = (float)jObject.GetValue("orbit");
				float rad = (float)jObject.GetValue("rad");
				float mass = (float)jObject.GetValue("mass");
				float temp = (float)jObject.GetValue("temp");
				string sprite = (string)jObject.GetValue("sprite");
				ElementMap data = ScriptableObjectRegistry.GetRegistry<ElementMap>();
				ScriptableObjectMap<Material> matMap = ScriptableObjectRegistry.GetRegistry2<Material>();

				JArray jatmo = (JArray)jObject.GetValue("atmo");
				string[] atmo = jatmo.ToObject<string[]>(serializer);
				Material[] atmoMats = atmo.Select(x => matMap[x]).ToArray();

				JArray jresource = (JArray)jObject.GetValue("resources");
				string[] resource = jresource.ToObject<string[]>(serializer);
				Material[] resourceMats = resource.Select(x => matMap[x]).ToArray();
				
				string jsd = (string)jObject.GetValue("surfdep");
				Material sd = string.IsNullOrEmpty(jsd) ? null : matMap[jsd];
				string jocen = (string)jObject.GetValue("ocean");
				Material ocen = string.IsNullOrEmpty(jocen) ? null : matMap[jocen];

				/*
				string[] atmo = v.atmosphere.Select(x => x.displayName).ToArray();
				o.Add(new JProperty("atmo", JArray.FromObject(atmo)));

				string[] resource = v.resources.Select(x => x.displayName).ToArray();
				o.Add(new JProperty("resources", JArray.FromObject(resource)));

				o.Add(new JProperty("surfdep", v.surfaceDeposits.displayName));
				o.Add(new JProperty("ocean", v.ocean.displayName));
				*/
				
				return new Planet() {
					orbitalDistance = orbit,
					radius = rad,
					mass = mass,
					averageSurfaceTemp = temp,
					atmosphere = atmoMats,
					resources = resourceMats,
					surfaceDeposits = sd,
					ocean = ocen,
					spriteData = ScriptableObjectRegistry.GetRegistry<SpriteMap>().First(x => x.spriteId == sprite)
				};
			}

			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
				Planet v = (Planet)value;
				JObject o = new JObject();
				o.Add(new JProperty("class", v.GetType().AssemblyQualifiedName));
				o.Add(new JProperty("orbit", v.orbitalDistance));
				o.Add(new JProperty("rad", v.radius));
				o.Add(new JProperty("mass", v.mass));
				o.Add(new JProperty("temp", v.averageSurfaceTemp));

				string[] atmo = v.atmosphere.Select(x => x.displayName).ToArray();
				o.Add(new JProperty("atmo", JArray.FromObject(atmo)));

				string[] resource = v.resources.Select(x => x.displayName).ToArray();
				o.Add(new JProperty("resources", JArray.FromObject(resource)));

				o.Add(new JProperty("surfdep", v.surfaceDeposits?.displayName));
				o.Add(new JProperty("ocean", v.ocean?.displayName));
				
				o.Add(new JProperty("sprite", v.spriteData.spriteId));
				o.WriteTo(writer);
			}

			public override bool CanRead {
				get { return true; }
			}

			public override bool CanWrite {
				get { return true; }
			}
		}
	}
}