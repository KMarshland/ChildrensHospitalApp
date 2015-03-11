using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;

public class UnityFromWire : MonoBehaviour {

	public TextAsset wire;

	WireElement first;
	public static volatile List<WireElement> bufferedDisplay;
	WireElement[] displaying;
	GUISkin skin;

	// Use this for initialization
	void Start () {
		//load the wire as an xml file
		//TextAsset wire = Resources.Load("Wires/Wire1") as TextAsset;

		first = buildFromXML(wire.text);
		bufferedDisplay = new List<WireElement>();
		bufferedDisplay.Add(first);
		displaying = bufferedDisplay.ToArray();

		skin = Resources.Load("GUISkins/MainSkin") as GUISkin;
		//Debug.Log(first.GetChild(0).GetChild(0).GetChild(0).Ypos);

	}
	
	// Update is called once per frame
	void Update () {
		displaying = bufferedDisplay.ToArray();
	}

	void OnGUI(){
		GUI.skin = skin;

		displaying[displaying.Length - 1].drawSelf();

		//ClassElement.GetClassByName("menuitem2").drawSelf();
		//Debug.Log (ClassElement.GetClassByName("scrollmenu").GetChild(2).ChildNumber);
		//ClassElement.GetClassByName("scrollmenu").GetChild(2).GetChild(2).drawSelf();
		//ClassElement.GetClassByName("scrollmenu").GetChild(2).drawSelf();
	}

	WireElement buildFromXML(string xmlString){

		WireElement firstelement = null;
		WireElement currentParent = null;
		WireAction currentAction = null;

		//parse the xml file
		using (XmlReader reader = XmlReader.Create(new StringReader(xmlString)))
		{
			// Parse the file and display each of the nodes.
			while (reader.Read())
			{
				switch (reader.NodeType)
				{
				case XmlNodeType.Element:
					//this is the type of the open tag. eg panel, class
					string ename = reader.Name;


					WireElement thisElement = null;

					//load the attributes. It might not have all of them
					string aname = reader.GetAttribute("name");
					string alias = reader.GetAttribute("alias");
					string width = reader.GetAttribute("width");
					string height = reader.GetAttribute("height");
					string text  = reader.GetAttribute("text");
					string xpos = reader.GetAttribute("xpos");
					string ypos = reader.GetAttribute("ypos");
					string align = reader.GetAttribute("align");
					string valign = reader.GetAttribute("valign");
					//string font = reader.GetAttribute("font");
					string background = reader.GetAttribute("background");
					string color = reader.GetAttribute("color");
					string size = reader.GetAttribute("size");
					string onclickup = reader.GetAttribute("onclickup");
					string aclass = reader.GetAttribute("class");
					string target = reader.GetAttribute("target");

					//figure out what type of element it is and create it
					switch (ename){

					case "main":
						ClassElement mel = new ClassElement(currentParent);
						
						mel.Name = "MainScreen___";
						mel.Alias = alias;
						mel.Width = nicefyWidth(width, currentParent);
						mel.Height = nicefyHeight(height, currentParent);
						mel.Xpos = xpos == null || xpos == "" ? 0 : nicefyWidth(xpos, currentParent);
						mel.Ypos = ypos == null || ypos == "" ? 0 : nicefyHeight(ypos, currentParent);
						
						thisElement = mel;
						firstelement = thisElement;

						break;
					case "class":
						ClassElement cel = new ClassElement(currentParent);

						cel.Name = aname;
						cel.Alias = alias;
						cel.Width = nicefyWidth(width, currentParent);
						cel.Height = nicefyHeight(height, currentParent);
						cel.Xpos = xpos == null || xpos == "" ? 0 : nicefyWidth(xpos, currentParent);
						cel.Ypos = ypos == null || ypos == "" ? 0 : nicefyHeight(ypos, currentParent);

						thisElement = cel;

						break;
					case "text":
						TextElement tel = new TextElement(currentParent);

						tel.Name = aname;
						tel.Alias = alias;

						tel.Text = text;
						tel.Width = nicefyWidth(width, currentParent);
						tel.Height = nicefyHeight(height, currentParent);
						tel.Xpos = xpos == null || xpos == "" ? 0 : nicefyWidth(xpos, currentParent);
						tel.Ypos = ypos == null || ypos == "" ? 0 : nicefyHeight(ypos, currentParent);
						tel.Align = align;
						tel.Valign = valign;
						tel.Colour = HexToRGB(color);
						tel.Size = int.Parse(size);

						//Debug.Log(text);

						thisElement = tel;
						break;
					case "panel":
						PanelElement pel = new PanelElement(currentParent);

						pel.Name = aname;
						pel.Alias = alias;

						pel.Width = nicefyWidth(width, currentParent);
						pel.Height = nicefyHeight(height, currentParent);
						pel.Xpos = xpos == null || xpos == "" ? 0 : nicefyWidth(xpos, currentParent);
						pel.Ypos = ypos == null || ypos == "" ? 0 : nicefyHeight(ypos, currentParent);
						pel.BackgroundColor = HexToRGB(background);
						pel.Onclickup = onclickup;

						thisElement = pel;
						break;

					case "action":
						WireAction wac = new WireAction(currentAction);

						wac.Name = aname;
						currentAction = wac;

						break;

					case "create":
						WireAction cac = new WireAction(currentAction);

						cac.Name = name;
						cac.Target = aclass;
						cac.Run = cac.create;

						break;

					case "delete":
						WireAction dac = new WireAction(currentAction);
						
						dac.Name = name;
						dac.Target = target;
						dac.Run = dac.delete;
						
						break;
					
					}

					//check to see if this is the first element
					if (firstelement == null){
						firstelement = thisElement;
					}

					//set next elements to have this one as a parent if it isn't a text element
					if (thisElement != null && 
					    thisElement.GetType().ToString() != "TextElement" && 
					    thisElement.GetType().ToString() != "WireAction"
					){
						currentParent = thisElement;
					}

					break;
				case XmlNodeType.Text:
					//Debug.Log(reader.Value);
					break;
				case XmlNodeType.ProcessingInstruction:
					break;
				case XmlNodeType.Comment:
					break;
				case XmlNodeType.EndElement:
					//it's a closing tag, so go up a level

					if (reader.Name == "action"){
						currentAction = currentAction.parent;
					}

					if (currentParent != null){
						if (currentParent.GetType().ToString() == "WireAction"){
							//currentAction = currentAction.parent;
						} else if (currentParent.GetType().ToString() != "TextElement"){
							currentParent = currentParent.parent;
						}
					}

					break;
				}
			}
		}

		return firstelement;
	}

	int nicefyWidth(string w){
		return nicefyWidth(w, null);
	}
	
	int nicefyWidth(string w, WireElement we){//parses a string to make a nice width

		if (w == "" || w == null){
			return Screen.width;
		}

		if (w.EndsWith("%%")){
			w = w.Split(new char[]{'%'}, System.StringSplitOptions.RemoveEmptyEntries)[0];
			return (int)(float.Parse(w) * Screen.width / 100f);
		} else if (w.EndsWith("%")){
			w = w.Split(new char[]{'%'}, System.StringSplitOptions.RemoveEmptyEntries)[0];
			if (we == null){
				return (int)(float.Parse(w) * Screen.width / 100f);
			} else {
				//Debug.Log(we.Xpos);
				return (int)(float.Parse(w) * we.Width / 100f) + we.Xpos;
			}
		}

		if (w == "[param:w]"){
			return Screen.width;
		}

		return int.Parse(w);
	}

	int nicefyHeight(string w){//parses a string to make a nice height
		return nicefyHeight(w, null);
	}

	int nicefyHeight(string w, WireElement we){//parses a string to make a nice height

		if (w == "" || w == null){
			return Screen.height;
		}

		if (w.EndsWith("%%")){
			w = w.Split(new char[]{'%'}, System.StringSplitOptions.RemoveEmptyEntries)[0];
			return (int)(float.Parse(w) * Screen.height / 100f);
		} else if (w.EndsWith("%")){
			w = w.Split(new char[]{'%'}, System.StringSplitOptions.RemoveEmptyEntries)[0];
			if (we == null){
				return (int)(float.Parse(w) * Screen.height / 100f);
			} else {
				return (int)(float.Parse(w) * we.Height / 100f) + we.Ypos;
			}
		}

		if (w == "[param:h]"){
			return Screen.height;
		}
		
		return int.Parse(w);
	}

	string GetHex (int decima) {
		string alpha = "0123456789ABCDEF";
		string outp = "" + alpha[decima];
		return outp;
	}
	
	int HexToInt (char hexChar) {
		string hex = "" + hexChar;
		switch (hex) {
		case "0": return 0;
		case "1": return 1;
		case "2": return 2;
		case "3": return 3;
		case "4": return 4;
		case "5": return 5;
		case "6": return 6;
		case "7": return 7;
		case "8": return 8;
		case "9": return 9;
		case "A": return 10;
		case "B": return 11;
		case "C": return 12;
		case "D": return 13;
		case "E": return 14;
		case "F": return 15;
		}
		return 0;
	}
	
	string RGBToHex (Color color) {
		float red = color.r * 255;
		float green = color.g * 255;
		float blue = color.b * 255;
		
		string a = GetHex(Mathf.Floor(red/16f).ToString()[0]);
		string b = GetHex(Mathf.Round(red % 16f).ToString()[0]);
		string c = GetHex(Mathf.Floor(green / 16f).ToString()[0]);
		string d = GetHex(Mathf.Round(green % 16f).ToString()[0]);
		string e = GetHex(Mathf.Floor(blue / 16f).ToString()[0]);
		string f = GetHex(Mathf.Round(blue % 16f).ToString()[0]);
		
		string z = a + b + c + d + e + f;
		
		return z;
	}
	
	Color HexToRGB (string color) {
		if (color == "[param:color]"){
			return Color.white;
		}

		color = color.Split(new char[]{'#'}, System.StringSplitOptions.RemoveEmptyEntries)[0];
		color = color.ToUpper();

		float red = (HexToInt(color[1]) + HexToInt(color[0]) * 16.000f) / 255f;
		float green = (HexToInt(color[3]) + HexToInt(color[2]) * 16.000f) / 255f;
		float blue = (HexToInt(color[5]) + HexToInt(color[4]) * 16.000f) / 255f;
		Color finalColor = new Color();

		finalColor.r = red;
		finalColor.g = green;
		finalColor.b = blue;
		finalColor.a = 1f;

		return finalColor;
	}
	
}
