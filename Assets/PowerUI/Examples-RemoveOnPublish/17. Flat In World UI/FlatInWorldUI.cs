using UnityEngine;
using System.Collections;
using PowerUI;

public class FlatInWorldUI : MonoBehaviour {
	
	/// <summary>Is input enabled for WorldUI's?</summary>
	public bool InputEnabled;
	/// <summary>A File containing the html/css/nitro for the screen.</summary>
	public TextAsset HtmlFile;

		
	// Use this for initialization
	void Start () {
		
		// Next, generate a new UI:
		FlatWorldUI ui=new FlatWorldUI("BillboardContent");
		
		// It's representing a TV, so lets use some standard TV screen dimensions:
		ui.SetDimensions(800,600);
		
		// As this example only uses a WorldUI, we'll set the filter mode to bilinear so it looks smoother.
		ui.TextFilterMode=FilterMode.Bilinear;
		
		// Give it some content:
		if(HtmlFile!=null){
			ui.document.innerHTML=HtmlFile.text;
		}
		
		if(InputEnabled){
			// World UI's should accept input:
			UI.WorldInputMode=InputMode.Screen;
		}
		
		// Grab the texture:
		Texture texture=ui.Texture;
		
		// Assign it to the parents material:
		Material material=gameObject.GetComponent<MeshRenderer>().sharedMaterial;
		
		// Apply it:
		material.mainTexture=texture;
		
	}
	
}
