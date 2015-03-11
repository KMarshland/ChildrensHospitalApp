using UnityEngine;
using System.Collections;
using PowerUI;

/// <summary>
/// This class is the default PowerUI Manager Monobehaviour. To use it, it needs to be attached to 
/// a gameobject in the scene.
/// </summary>

public class PoweruiManager : UnityEngine.MonoBehaviour {
	
	/// <summary>A File containing the html/css/nitro of your UI.</summary>
	public TextAsset HtmlFile;
	/// <summary>The UI screen Resolution. See UI.Resolution for more info. Note that this is a very basic usage!
	/// PowerUI can also select different images too without affecting your html.</summary>
	public float SimpleResolution=1f;
	
	
	// OnEnable is called when the game starts, or when the manager script component is enabled.
	void OnEnable () {
		
		// Optional. This allows PowerUI to easily work with screens of varying resolution.
		if(SimpleResolution!=1f){
			UI.Resolution=new ResolutionInfo(SimpleResolution);
		}
		
		// Load the UI from the above HtmlFile. Note that UI's don't have to be loaded like this! You
		// can also just set a string of text if needed.
		if(HtmlFile==null){
		
			Debug.Log("Please provide a HTML file for your UI. "+
				"If you're stuck, please see the Getting Started guide on the website (http://powerUI.kulestar.com/)"+
				", in the GettingStarted folder, or feel free to contact us! We're always happy to help :)");
			
		}else{
			
			// Write the html:
			UI.Html=HtmlFile.text;
		
		}
		
	}
	
	// OnDisable is called when the manager script component is disabled. You don't need this.
	void OnDisable () {
		UI.Destroy();
	}
	
}
