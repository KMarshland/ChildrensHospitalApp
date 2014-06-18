using UnityEngine;
using System.Collections;

public struct MenuItem {

	Texture2D image;
	string header;
	string content;

	public MenuItem(string imagepath, string nheader, string ncontent){
		image = Resources.Load(imagepath) as Texture2D;
		header = nheader;
		content = ncontent;
	}

	public MenuItem(Texture2D nimage, string nheader, string ncontent){
		image = nimage;
		header = nheader;
		content = ncontent;
	}

	public Texture2D Image {
		get {
			return image;
		} set {
			image = value;
		}
	}

	public string Header {
		get {
			return header;
		} set {
			header = value;
		}
	}

	public string Content {
		get {
			return content;
		} set {
			content = value;
		}
	}

}
