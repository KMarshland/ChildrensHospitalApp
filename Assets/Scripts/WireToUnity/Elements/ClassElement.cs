using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClassElement : WireElement {

	public static List<WireElement> classes = new List<WireElement>();

	public ClassElement(WireElement parent) : base(parent) {
		classes.Add(this);
	}
	
	public override void drawSelf(){
		//Debug.Log("class " + name);
		base.drawSelf();
	}

	public static WireElement GetClassByName(string name){
		foreach (WireElement w in classes){
			if (w.Name == name || w.Alias == name){
				if (w is ClassElement){
					return w;
				} else {
					return GetClassByName(w.parent.Name);
				}
			}
		}
		return null;
	}

}
