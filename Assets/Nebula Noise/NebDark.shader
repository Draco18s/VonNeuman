// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "FancyShaders/NebDark" {
	Properties {
		_Distort ("Distortion", Range(0.05,.5)) = .2
		_Color ("Color", Color) = (1.0,1.0,1.0,1.0)
		_HighLight ("HighLight", Color) = (1.0,1.0,1.0,1.0)
		_MainTex ("Texture", 2D) = "white" {}
		//_MaskTex ("Mask", 2D) = "white" {}
		_MaskTex ("Mask", 2D) = "white" {}
	}
	SubShader {
		Pass{
			Tags { 
				"LightMode"="ForwardBase"
			}
			
			ZWrite Off
			Cull Back
			
			Blend SrcAlpha OneMinusSrcAlpha
			
			CGPROGRAM
			#pragma glsl
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			
			//user vars
			uniform fixed _Distort;
			uniform fixed4 _Color;
			uniform fixed4 _HighLight;
			uniform sampler2D _MaskTex;
			//uniform sampler2D _MaskTex2;
			uniform float4 _MaskTex_ST;
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			
			//structs
			struct vin{
				half4 vertex : POSITION;
				half4 texcoord: TEXCOORD0;
			};
			
			struct v2f{
				half4 pos : SV_POSITION;
				half4 tex: TEXCOORD0;
				half4 posWorld : TEXCOORD1;
			};
			
			float rand(float3 co)
		    {
		    	return frac(sin( dot(co.xyz ,float3(12.9898,78.233,45.5432) )) * 43758.5453);
		    }
			
			//vertex shader
			v2f vert(vin v){
				v2f o;
				//float4 temp = tex2Dlod(_MainTex,float4(v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw,0,0));
				//o.pos= mul(UNITY_MATRIX_MVP,v.vertex);
				o.pos= UnityObjectToClipPos(v.vertex);
				o.posWorld = half4(mul(unity_ObjectToWorld,v.vertex).xyz,0);
				
				o.tex=v.texcoord;
				
				return o;
			}
			
			//fragment shader
			float4 frag(v2f i) : COLOR{
				half4 col = tex2D(_MainTex,i.tex.xy * _MainTex_ST.xy + _MainTex_ST.zw);
				half4 col2 = tex2D(_MainTex,i.tex.xy * _MainTex_ST.xy + _MainTex_ST.zw + float2(_Distort*(col.x-.5),_Distort*(col.y-.5)));
				fixed radA = max(1-max(length(half2(.5,.5)-i.tex.xy)-.25,0)/.25,0)+rand(i.posWorld)/30.0;
				
				half4 mask = tex2D(_MaskTex,i.tex.xy*_MaskTex_ST.xy + _MaskTex_ST.zw);
				return float4(lerp(_HighLight,_Color, col2.z*.5).rgb,
					saturate(1.5*col2.w*col2.y*col.y*(mask.g+.25*mask.r))*radA);
			}
			
			ENDCG
		}
	} 
	//FallBack "Diffuse"
}
