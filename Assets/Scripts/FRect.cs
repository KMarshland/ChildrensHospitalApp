using UnityEngine;
using System.Collections;

public struct FRect {

	float x, y, width, height;

	public FRect(float nx, float ny, float nwidth, float nheight){
		x = nx;
		y = ny;
		width = nwidth;
		height = nheight;
	}

	public float X {
		get {
			return x;
		} set {
			x = value;
		}
	}

	public float Y {
		get {
			return y;
		} set {
			y = value;
		}
	}

	public float Width {
		get {
			return width;
		} set {
			width = value;
		}
	}

	public float Height {
		get {
			return height;
		} set {
			height = value;
		}
	}

	public Rect toRect{
		get {
			return new Rect(x * Screen.width, y * Screen.height, width * Screen.width, height * Screen.height);
		}
	}

}
