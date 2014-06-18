using UnityEngine;
using System.Collections;

public class TextElement : WireElement {

	string text;
	Color color;
	string align;
	string valign;
	int fontSize;

	bool calculated;

	public TextElement(WireElement parent) : base(parent) {
		
	}

	public override void drawSelf(){
		//Debug.Log("text " + name + " " + text);
		if (!calculated){
			calcSize();
			calcXAndY();
			calculated = true;
		}

		GUI.contentColor = color;
		GUI.skin.label.fontSize = fontSize;

		GUI.Label(PositionRect, text);

		base.drawSelf();
	}

	public void calcSize(){
		GUI.skin.label.fontSize = fontSize;
		Vector2 ns = GUI.skin.label.CalcSize(new GUIContent(text));
		width = (int)ns.x;
		height = (int)ns.y;

		if (width + xpos > parent.Width + parent.Xpos){
			width = parent.Width + parent.Xpos - xpos;
			height = (int)GUI.skin.label.CalcHeight(new GUIContent(text), width);
		}

		if (height > parent.Height){
			height = parent.Height;
		}
	}

	public void calcXAndY(){
		if (parent != null){
			if (xpos == 0){
				if (align == "center"){
					int left = parent.Xpos;
					int right = parent.Xpos + parent.Width;
					xpos = (left + right - width)/2;
				} else {
					Debug.Log(align);
				}
			}

			if (ypos == 0){
				if (valign == "center"){
					int top = parent.Ypos;
					int bottom = parent.Ypos + parent.Height;
					ypos = (top + bottom - height)/2;
				} else {
					Debug.Log(valign);
				}
			}

			//Debug.Log(name + ": " + xpos + ", " + ypos);
		}

		//Debug.Log(name + " - No parent: " + xpos + ", " + ypos);
	}

	public string Text {
				get {
						return text;
				}
				set {
						text = value;
				}
		}

	public Color Colour {
				get {
						return color;
				}
				set {
						color = value;
				}
		}

	public string Align {
				get {
						return align;
				}
				set {
						align = value == "" || value == null ? "center":value;
				}
		}

	public string Valign {
		get {
				return valign;
		}
		set {
			valign = value == "" || value == null ? "center":value;
		}
	}


	public int Size {
				get {
						return fontSize;
				}
				set {
						fontSize = value;
				}
		}
}
