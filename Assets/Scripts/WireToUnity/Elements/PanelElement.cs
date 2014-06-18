using UnityEngine;
using System;
using System.Collections;

public class PanelElement : WireElement {

	//<panel name="gen" alias = "GEN" width="[param:w]" height="[param:h]" background="[param:color]" target = "CONTAINER" >
	
	Color backgroundColor;
	string onclickup;
	Action onClick;

	public PanelElement(WireElement parent) : base(parent) {
		ClassElement.classes.Add(this);
	}

	public override void drawSelf(){

		GUI.backgroundColor = backgroundColor;

		if (onclickup != "" && onclickup != null){
			if (GUI.Button(PositionRect, "")){
				if (onClick == null){
					generateOnClick();
				}
				//Debug.Log("panel " + name);
				onClick();
			}
		} else {
			GUI.Box(PositionRect, "");
		}

		base.drawSelf();
	}

	void generateOnClick(){
		onClick = WireAction.GetActionByName(onclickup).Run;
	}

	public Color BackgroundColor {
				get {
						return backgroundColor;
				}
				set {
						backgroundColor = value;
				}
		}
	
	public string Onclickup {
				get {
						return onclickup;
				}
				set {
						onclickup = value;
				}
		}
}
