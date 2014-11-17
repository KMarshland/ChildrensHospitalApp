using UnityEngine;
using System.Collections;
using PowerUI;


public class SimpleHealthPoints : MonoBehaviour {
	
	// My current points:
	public int Points;
	
	
	// Update is called once per frame
	void Update () {
		
		// Point counter.
		// We're going to be extra generous and give a point every frame!
		AddPoints(1);
		
		
		// Basic health bar.
		// Note that this could all be done in C#/ UnityJS if you wish; just use UI.document.
		// Let's talk to Nitro!
		// A function called "IncreaseBasicBarHealth" has been defined in the document.
		// It sets the basic bar, and we call it like this:
		UI.document.Run("IncreaseBasicBarHealth",0.2f*Time.deltaTime);
		
	}
	
	/// <summary>Adds the given points to Points, then tells the UI it changed.</summary>
	public void AddPoints(int pointsToAdd){
	
		// Bump it up by the points to add:
		Points+=pointsToAdd;
		
		// Write it out to the UI:
		UI.Variables["Points"]=Points.ToString();
		
	}
	
}
