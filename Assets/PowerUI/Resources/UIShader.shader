Shader "Hybrid GUI Shader" {
	Properties {
		_Font ("Font Texture", 2D) = "white" {}
		_Atlas  ("Graphical Atlas", 2D) = "white" {}
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
			
			sampler2D _Font;
			uniform float4 _Font_ST;
			
			sampler2D _Atlas;
			uniform float4 _Atlas_ST;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.texcoord = v.texcoord;
				return o;
			}

			fixed4 frag (v2f i) : COLOR
			{
				fixed4 col = i.color;
				
				if(i.texcoord.x>=2){
					i.texcoord.x=i.texcoord.x-2;
					col.a *= UNITY_SAMPLE_1CHANNEL(_Font,i.texcoord);
				}else if(i.texcoord.y<=1){
					col *= tex2D(_Atlas, i.texcoord);
				}
				
				return col;
			}
			ENDCG 
		}
	} 	

}
