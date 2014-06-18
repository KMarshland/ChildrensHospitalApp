using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WireElement {

	protected List<WireElement> children = new List<WireElement>();
	public WireElement parent = null;

	protected string name;
	protected string alias;
	protected int width;
	protected int height;
	protected int xpos;
	protected int ypos;
	protected Rect positionRect;

	public WireElement(WireElement nparent){
		if (nparent != null){
			nparent.children.Add(this);
		}
		parent = nparent;
	}

	public virtual void drawSelf(){
		//Debug.Log("element");
		foreach (WireElement w in children){
			w.drawSelf();
		}
	}

	public string Name {
				get {
						return name;
				}
				set {
						name = value;
				}
		}

	public string Alias {
				get {
						return alias;
				}
				set {
						alias = value;
				}
		}

	public int Width {
		get {
			if (width <= 0){
				width = Screen.width;
			}
			return width;
		}
		set {
			width = value;
			if (width <= 0){
				width = Screen.width;
			}
		}
	}
	
	public int Height {
		get {
			if (height <= 0){
				height = Screen.height;
			}
			return height;
		}
		set {
			height = value;
			if (height <= 0){
				height = Screen.height;
			}
		}
	}
	
	public int Xpos {
		get {
			return xpos;
		}
		set {
			xpos = value;
		}
	}
	
	public int Ypos {
		get {
			return ypos;
		}
		set {
			ypos = value;
		}
	}

	public Rect PositionRect {
				get {
						positionRect = new Rect (xpos, ypos, width, height);
						return positionRect;
				}
				set {
						positionRect = value;
				}
		}

	public int ChildNumber {
		get {
			return children.Count;
		}
	}

	public WireElement GetChild(int n){
		return children[n];
	}
}
