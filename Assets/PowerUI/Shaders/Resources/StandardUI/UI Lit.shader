Shader "StandardUI Lit" {
	Properties {
		_Font ("Font Texture", 2D) = "white" {}
		_Atlas  ("Graphical Atlas", 2D) = "white" {}
		BottomFontAlias ("Lower Alias",Float)=0.2
		TopFontAlias ("Upper Alias",Float)=0.8
	}

	SubShader {
		
		Tags{"RenderType"="Transparent" Queue=Transparent}
		
		Lighting On
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Back 
		ZWrite On
		
		UsePass "StandardUI Unlit/BASE"
		
	} 	

}