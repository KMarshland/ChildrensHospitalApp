Shader "StandardUI Unlit" {
	Properties {
		_Font ("Font Texture", 2D) = "white" {}
		_Atlas  ("Graphical Atlas", 2D) = "white" {}
		BottomFontAlias ("Lower Alias",Float)=0.24
		TopFontAlias ("Upper Alias",Float)=1.24
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
			#pragma glsl_no_auto_normalization
			
			#include "UnityCG.cginc"
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				half2 texcoord : TEXCOORD;
				half2 texcoord1 : TEXCOORD1;
			};
			
			sampler2D _Font;
			uniform float4 _Font_ST;
			
			sampler2D _Atlas;
			uniform float4 _Atlas_ST;
			
			uniform float TopFontAlias;
			uniform float BottomFontAlias;
			
			appdata_t vert (appdata_t v)
			{
				v.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				return v;
			}

			fixed4 frag (appdata_t i) : COLOR
			{
				fixed4 col = i.color;
				
				if(i.texcoord.y<=1){
					col *= tex2D(_Atlas, i.texcoord);
				}
				
				if(i.texcoord1.y<=1){
					col.a *= smoothstep(BottomFontAlias,TopFontAlias,tex2D(_Font,i.texcoord1).a);
				}
				
				return col;
			}
			ENDCG 
		}
	} 	

}
