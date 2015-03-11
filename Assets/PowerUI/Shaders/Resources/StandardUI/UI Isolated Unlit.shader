Shader "StandardUI Isolated Unlit" {
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
		
		Pass {
			Name "BASE"
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				half2 texcoord : TEXCOORD0;
			};
			
			sampler2D _Sprite;
			uniform float4 _Sprite_ST;
			
			appdata_t vert (appdata_t v)
			{
				v.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				v.texcoord = TRANSFORM_TEX(v.texcoord,_Sprite);
				return v;
			}

			fixed4 frag (appdata_t i) : COLOR
			{
				return i.color*tex2D(_Sprite, i.texcoord);
			}
			ENDCG 
		}
	} 	

}
