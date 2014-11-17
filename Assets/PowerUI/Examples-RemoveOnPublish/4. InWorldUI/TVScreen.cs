using UnityEngine;
using System.Collections;
using PowerUI;

public class TVScreen : MonoBehaviour {
	
	/// <summary>Is input enabled for WorldUI's?</summary>
	public bool InputEnabled;
	/// <summary>A File containing the html/css/nitro for the screen.</summary>
	public TextAsset HtmlFile;

		
	// Use this for initialization
	void Start () {
		
		// Next, generate a new UI:
		WorldUI ui=new WorldUI("TVScreenContent");
		
		// It's representing a TV, so lets use some standard TV screen dimensions:
		ui.SetDimensions(800,600);
		
		// But we need to define the resolution - that's how many pixels per world unit.
		// This of course entirely depends on the model and usage.
		ui.SetResolution(540,610);
		
		// As this example only uses a WorldUI, we'll set the filter mode to bilinear so it looks smoother.
		ui.TextFilterMode=FilterMode.Bilinear;
		
		// Give it some content:
		if(HtmlFile!=null){
			ui.document.innerHTML=HtmlFile.text;
		}
		
		// Parent it to the TV:
		ui.transform.parent=transform;
		
		// Let's move it around a little too:
		ui.transform.localPosition=new Vector3(0f,1.02f,0.03f);
		
		// And spin it around - the TV mesh is facing the other way:
		ui.transform.localRotation=Quaternion.AngleAxis(180f,Vector3.up);
		
		if(InputEnabled){
			// World UI's should accept input:
			UI.WorldInputMode=InputMode.Screen;
		}
		
	}
	
}
