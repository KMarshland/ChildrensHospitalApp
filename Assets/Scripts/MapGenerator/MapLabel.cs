
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class MapLabel : MonoBehaviour {

	static GameObject mainParent;

	public string label;
	string[] tags;
	int priority;
	Vector3 pathLocation;

	int id;

	TextMesh textMesh;
	GUIText guiText;
	GameObject marker;

	// Use this for initialization
	void Start () {

		//load the components
		textMesh = this.gameObject.GetComponent("TextMesh") as TextMesh;
		marker = this.transform.FindChild("Marker").gameObject;
		//guiText = this.transform.guiText;

		//change the text to what is should be
		textMesh.text = label;
		//guiText.text = label;
		this.name = "Label (" + label + ")";

		//guiText.pixelOffset = new Vector2(Screen.width/2, Screen.height/2);

		//make the text non-fuzzy
		float pixelRatio = (Camera.main.orthographicSize  * 2) / Camera.main.pixelHeight;
		float characterSize = 1f;
		transform.localScale = new Vector3(pixelRatio*10f, pixelRatio*10f, pixelRatio * 0.1f);

		marker.transform.localScale = new Vector3(1f, 1f, 4f) / (pixelRatio * 10f);



		//check if you're supposed to display this one
		if (priority <= 1){
			this.renderer.enabled = false;
			this.transform.GetChild(0).renderer.enabled = false;
		}

		//make sure the world know this label exists
		pathLocation = KaiTools.Point.pointToVector3(
			MapMaker.closestUnnocupiedTo(KaiTools.Point.vector3ToPoint(transform.position))
		);
		pathLocation = new Vector3(pathLocation.x, pathLocation.y, 0f);
		id = MapCameraControl.addLabel(this);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//Vector3 realPos = Camera.main.transform.position;// + Camera.main.transform.forward;
		//transform.LookAt(new Vector3(realPos.x, realPos.y, transform.position.z));
		//transform.Rotate(new Vector3(180f, 0f, 90f));
		float theta = Camera.main.transform.rotation.eulerAngles.x;//perpendicular to the direction of the camera
		transform.rotation = Quaternion.Euler(theta, 90f, 270f);//look in that direction

		marker.transform.position = this.transform.position + new Vector3(0f, 0f, 2f);
		marker.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
	}

	public bool termApplies(string term){
		for (int i = 0; i < tags.Length; i++){
			if (tags[i].Trim().Contains(term.Trim()) || term.Trim().Contains(tags[i].Trim())){
				return true;
			}
		}
		return false;
	}

	public void highlight(){
		this.renderer.enabled = true;
		this.transform.GetChild(0).renderer.enabled = true;

		textMesh.color = Color.magenta;

	}

	public void downlight(){
		if (priority <= 1){
			this.renderer.enabled = false;
			this.transform.GetChild(0).renderer.enabled = false;
		}

		textMesh.color = new Color(0f, 218f/255f, 19f/255f, 1f);
	}

	public string Label {
		get {
			return label;
		}
	}

	public int Id {
		get {
			return id;
		}
	}

	public string[] Tags {
		get {
			return tags;
		}
	}

	public int Priority {
		get {
			return priority;
		}
	}

	public Vector3 PathLocation {
		get {
			return pathLocation;
		}
	}

	public static MapLabel createLabel(Vector3 position, string alltext, int priority){
		if (mainParent == null){
			mainParent = new GameObject("Labels");
			mainParent.transform.position = Vector3.zero;
		}

		GameObject obj = GameObject.Instantiate(Resources.Load("Prefabs/MapLabel")) as GameObject;
		obj.transform.parent = mainParent.transform;
		obj.transform.position = position;

		MapLabel lab = obj.GetComponent("MapLabel") as MapLabel;
		string[] text = alltext.Split(new char[]{','}, System.StringSplitOptions.RemoveEmptyEntries);
		lab.label = text[0];
		lab.tags = text;
		lab.priority = priority;

		return lab;
	}

	public static void loadMarkers(){
		//TextAsset txt = Resources.Load("TextAssets/MarkerSave") as TextAsset;
		//string text = txt.text;

		if (!PlayerPrefs.HasKey("MarkerSave")){
			PlayerPrefs.SetString("MarkerSave", "");
			Debug.Log("If you see this message twice more than once at the beginning " +
				"and you have internet access, let me know");
		}

		string text = PlayerPrefs.GetString("MarkerSave");


		string[] lines = text.Trim().Split(new char[]{'\n', '\r', '\t'}, System.StringSplitOptions.RemoveEmptyEntries);

		foreach (string line in lines){
			string[] pieces = line.Trim().Split(new char[]{'|'});
			//Debug.Log(line.Trim());
			if (pieces.Length >= 4 && int.Parse(pieces[3]) > 0){
				Vector3 oldPos = new Vector3(float.Parse(pieces[1].Trim()), float.Parse(pieces[2].Trim()), -4f);
				createLabel(MapMaker.mapSpaceToWorldSpaceFull(oldPos), pieces[0].Trim(), int.Parse(pieces[3]));
			}
		}
	}

	public static IEnumerator updateMarkerSave(){
		WWW www = new WWW("http://marshlandgames.com/HospitalProject/Client/MarkerPlacement/show_markers.php");

		yield return www;

		//strip the header and footer off
		string stripped = www.text.Substring(85, www.text.Length - 85 - 8);
		//Debug.Log(stripped);

		//split it up by cell
		string[] piecesArr = Regex.Split(stripped, "<..>|<...>");
		List<string> pieces = new List<string>();
		foreach (string s in piecesArr){
			if (s.Trim() != ""){
				pieces.Add(s);
			}
		}

		//stick the cells back together in a meaningfull way 
		string newSaveString = "";
		for (int i = 0; i < pieces.Count - 3; i+= 4){
			newSaveString += pieces[i] + "|" + pieces[i+1] + "|" + pieces[i+2] + "|" + pieces[i+3] +"\n";
		}

		//save what the new save is
		PlayerPrefs.SetString("MarkerSave", newSaveString);

		//reload the markers
		//you have to clear the existing ones first though

		if (mainParent == null){
			mainParent = new GameObject("Labels");
			mainParent.transform.position = Vector3.zero;
		}

		for (int i = 0; i < mainParent.transform.childCount; i++){
			GameObject.Destroy(mainParent.transform.GetChild(i).gameObject);
		}
		loadMarkers();
		//Debug.Log(PlayerPrefs.GetString("MarkerSave"));
	}

}
