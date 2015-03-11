Shader "StandardUI SFX Unlit" {
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
		
		Pass {
			Name "BASE"
			
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma glsl_no_auto_normalization
			
			struct appdata_t {
				float4 vertex : POSITION;
				half2 texcoord : TEXCOORD;
				half2 texcoord1 : TEXCOORD1;
				half4 tangent : TANGENT;
				fixed4 color : COLOR;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				half2 texcoord : TEXCOORD;
				half2 texcoord1 : TEXCOORD1;
				fixed4 color : COLOR;
				half2 antialias : TEXCOORD2;
			};
			
			sampler2D _Font;
			uniform float4 _Font_ST;
			
			sampler2D _Atlas;
			uniform float4 _Atlas_ST;
			
			v2f vert (appdata_t v) {
				v2f o;
				o.pos = mul( UNITY_MATRIX_MVP, v.vertex );
				o.color=v.color;
				o.texcoord=v.texcoord;
				o.texcoord1=v.texcoord1;
				
				half4 tan=v.tangent;
				
				o.antialias= half2(tan.x,tan.y);
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR {
				fixed4 col = i.color;
				
				if(i.texcoord.y<=1){
					col *= tex2D(_Atlas, i.texcoord);
				}
				
				if(i.texcoord1.y<=1){
					col.a *= smoothstep(i.antialias.x,i.antialias.y,tex2D(_Font,i.texcoord1).a);
				}
				
				return col;
			}
			
			ENDCG
		}
	}
}