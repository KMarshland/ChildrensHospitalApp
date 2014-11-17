using UnityEngine;
using System.Collections;
using PowerUI;

public class DynamicTexturesExample : MonoBehaviour {
	
	/// <summary>A File containing the html/css/nitro of your UI.</summary>
	public TextAsset HtmlFile;
	/// <summary>An instance of the power bar - a dynamic graphic.</summary>
	public PowerBar PowerBarGraphic;
	/// <summary>An instance of the health bar - a dynamic graphic.</summary>
	public HealthBar HealthBarGraphic;
	
	
	// Use this for initialization
	void Start () {
		
		// Create our dynamic textures:
		PowerBarGraphic=new PowerBar();
		HealthBarGraphic=new HealthBar();
		
		// Internally they have now setup the dynamic://healthbar and dynamic://powerbar links for easy access from the html.
		
		// Load the UI from the above HtmlFile:
		if(HtmlFile!=null){
			UI.Html=HtmlFile.text;
		}
	}
	
	// Update is called once per frame
	void Update () {
		// Increase health/power by 0.2 a second:
		PowerBarGraphic.IncreasePower(0.2f*Time.deltaTime);
		HealthBarGraphic.IncreaseHealth(0.2f*Time.deltaTime);
	}

}
