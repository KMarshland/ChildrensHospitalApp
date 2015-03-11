Shader "StandardUI Isolated Lit" {
	Properties {
		_Sprite  ("Graphical Sprite", 2D) = "white" {}
	}

	SubShader {

		Tags{"RenderType"="Transparent" Queue=Transparent}
		
		Lighting On
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Back 
		ZWrite On 
		
		UsePass "StandardUI Isolated Unlit/BASE"
		
	} 	

}
