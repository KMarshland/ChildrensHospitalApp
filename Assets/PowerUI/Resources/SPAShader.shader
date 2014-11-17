Shader "PowerUI Animation Shader" {
	Properties {
		_Sprite  ("Graphical Sprite", 2D) = "white" {}
	}

	SubShader {

		Tags{"RenderType"="Transparent" Queue=Transparent}
		
		Lighting Off 
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Back 
		ZWrite On 
		Fog { Mode Off }  
		
		BindChannels {
			Bind "Color", color
			Bind "Vertex", vertex
			Bind "TexCoord", texcoord
		}
		
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};
			
			sampler2D _Sprite;
			uniform float4 _Sprite_ST;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_Sprite);
				return o;
			}

			fixed4 frag (v2f i) : COLOR
			{
				return i.color*  tex2D(_Sprite, i.texcoord);
			}
			ENDCG 
		}
	} 	

}
