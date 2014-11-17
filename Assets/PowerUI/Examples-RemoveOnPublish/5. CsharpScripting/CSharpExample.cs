using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PowerUI;

/// <summary>
/// This example shows an alternative way of accepting mousedowns other than with Nitro by using C# delegates and anonymous delegates.
/// </summary>

public class CSharpExample : MonoBehaviour {
	
	/// <summary>A File containing the html/css/nitro of your UI.</summary>
	public TextAsset HtmlFile;
	
	
	// Use this for initialization
	void Start () {
		
		// Load the UI from the above HtmlFile:
		if(HtmlFile!=null){
			UI.Html=HtmlFile.text;
		}else{
			Debug.Log("HTML file missing! Please set one up in the inspector.");
			return;
		}
		
		// Next, let's hook up our mouse methods to an element with an ID of 'illBeClickable':
		
		Element myElement=UI.document.getElementById("illBeClickable");
		
		if(myElement==null){
			// Usually you won't need to do this if, but this is just incase!
			Debug.Log("Whoops - It looks like the clickable element got removed!");
		}else{
			// Great, let's setup the events:
			myElement.OnMouseDown+=OnElementMouseDown;
		}
		
		// In many cases there is a group of elements that, when clicked, all do similar things.
		// If this applies to you too, then you can grab all elements that have a certain tag (such as all 'div' elements)
		// See the OnHeaderMouseDown function for then distinguishing one from another.
		// Or all elements with a certain attribute (class of "button"):
		
		// First, all h4 that are kids of body:
		List<Element> allHeaders=UI.document.body.getElementsByTagName("h4");
		
		foreach(Element element in allHeaders){
			element.OnMouseDown+=OnHeaderMouseDown;
		}
		
		// Second, all elements that are kids of body and have a class of "button":
		List<Element> allButtons=UI.document.body.getElementsWithProperty("class","button");
		
		foreach(Element element in allButtons){
			
			// This also shows how to create an "anonymous delegate" - that's one not declared as a seperate function.
			// These are more useful if you have a significant amount of callbacks:
			element.OnMouseDown += delegate(UIEvent mouseEvent) {
				// mouseEvent.target is the element that actually got clicked:
				mouseEvent.target.innerHTML="You clicked this!";
			};
			
		}
		
	}
	
	/// <summary>Direct is called directly with onmousedown="CSharpExample.Direct".</summary>
	public static void Direct(UIEvent mouseEvent){
		mouseEvent.target.style.border="2px solid #0000ff";
	}
	
	/// <summary>Called when any span gets clicked. This is linked up in Start as a delegate.</summary>
	private void OnElementMouseDown(UIEvent mouseEvent){
		// mouseEvent.target is the element that actually got clicked:
		mouseEvent.target.innerHTML="You clicked it!";
	}
	
	/// <summary>Called when any h1 gets clicked. This is linked up in Start as a delegate.</summary>
	private void OnHeaderMouseDown(UIEvent mouseEvent){
		// mouseEvent.target is the element that actually got clicked:
		mouseEvent.target.style.color="#ff00ff";
		// Sometimes the element itself must store something unique.
		// For that, we reccommend using attributes (<h1 thisIsAn="attribute"... note that they are always lowercase from C#!), or the element Data property:
		
		// Grab it's category=".." value:
		Debug.Log("It's 'category' attribute is "+mouseEvent.target["category"]);
	}
	
}
