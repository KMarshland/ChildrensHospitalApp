using UnityEngine;
using System.Collections;
using PowerUI;
using PowerUI.Css.Properties;

public class Text3DHandler : MonoBehaviour {
	
	/// <summary>The 3D text.</summary>
	public TextAsset Html;

	
	void Start(){
		
		// The text-extrude API is experimental and therefore unstable and unoptimised.
		// This example is provided straight from our test bench as a preview of what PowerUI 2 is capable of.
		
		// We're going to increase the curve accuracy - this results in smoother curves.
		// This of course means more triangles, but this text is the only thing here anyway.
		// TextExtrude.CurveAccuracy/=10f;
		
		
		// Create a WorldUI:
		WorldUI ui=new WorldUI();
		
		// Let's give it some space - this will affect the wrapping of the text:
		ui.SetDimensions(500,100);
		
		// Define the resolution - that's how many pixels per world unit.
		// This of course entirely depends on the usage.
		ui.SetResolution(10);
		
		if(Html==null){
			Debug.Log("No HTML file defined for 3D text!");
			return;
		}
		
		// Apply the content:
		ui.document.innerHTML="<div style='text-extrude:4;font-size:30px;height:100%;vertical-align:middle;text-align:center;font-family:Hardwood;'>"+Html.text+"</div>";
		
	}
	
}
