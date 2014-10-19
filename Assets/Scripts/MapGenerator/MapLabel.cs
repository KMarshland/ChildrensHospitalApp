
using UnityEngine;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class MapLabel : MonoBehaviour {

	static GameObject mainParent;
	static Dictionary<int, MapLabel> mapLabels = new Dictionary<int, MapLabel>();

	public string label;
	string[] tags;
	int priority;
	Vector3 pathLocation;
	int floor;
	bool visible;
	bool isElevator;
	int[] elevatorFloors;

	int id;

	TextMesh textMesh;
	GUIText guitext;
	GameObject marker;

	public MapLabel parentalLabel;//used for threading a route through elevators

	//bool addedToMapping = false;

	// Use this for initialization
	void Start () {

		mapLabels[this.Id] = this;

		//load the components
		textMesh = this.gameObject.GetComponent("TextMesh") as TextMesh;
		marker = this.transform.FindChild("Marker").gameObject;
		//guiText = this.transform.guiText;

		//change the text to what is should be
		textMesh.text = label;
		checkVisible();
		//guiText.text = label;
		this.name = "Label (" + label + ")";

		//guiText.pixelOffset = new Vector2(Screen.width/2, Screen.height/2);

		//make the text non-fuzzy
		float pixelRatio = (Camera.main.orthographicSize  * 2) / Camera.main.pixelHeight;
		//float characterSize = 1f;
		transform.localScale = new Vector3(pixelRatio*10f, pixelRatio*10f, pixelRatio * 0.1f);

		marker.transform.localScale = new Vector3(1f, 1f, 4f) / (pixelRatio * 10f);



		//check if you're supposed to display this one
		if (priority <= 1){
			this.renderer.enabled = false;
			this.transform.GetChild(0).renderer.enabled = false;
		}

		//make sure the world know this label exists
		pathLocation = /*MapMaker.ActiveFloor.closestUnnocupiedTo*/(transform.position);
		pathLocation = new Vector3(pathLocation.x, pathLocation.y, 0f);
		id = MapCameraControl.addLabel(this);

		scanSelf();
	}

	void scanSelf(){
		if (this.Visible){
			PointGraph navg = (PointGraph)AstarPath.active.graphs[0];
			AstarPath.active.AddWorkItem (new AstarPath.AstarWorkItem (delegate () {
				//float f = Time.realtimeSinceStartup;
				PointNode p = navg.AddNode((Int3)pathLocation);

				if (p != null){
					Dictionary<GraphNode, float> cons = new Dictionary<GraphNode, float>();
					navg.GetNodes((delegate (GraphNode node) {
						float distance;
						if (navg.IsValidConnection(p, node, out distance)){
							//Debug.Log(distance);
							cons[node] = distance;
						}
						return true;
					}));


					foreach (GraphNode g in cons.Keys){
						p.AddConnection(g, (uint)cons[g]);
						g.AddConnection(p, (uint)cons[g]);
					}

					//Debug.Log("Found " + cons.Count + " valid connections");
					//Debug.Log("Scan time: " + (Time.realtimeSinceStartup - f));
				}
			}, null));
		}
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

	public void checkVisible(){
		int realFloor = MapMaker.ActiveFloor.Id - 1;
		bool vis = false;
		vis = vis || (realFloor == floor);

		if (isElevator){
			for (int i = 0; i < elevatorFloors.Length; i++){
				vis = vis || (realFloor == elevatorFloors[i]);
			}
		}

		this.Visible = vis;
	}

	public bool termApplies(string term){
		for (int i = 0; i < tags.Length; i++){
			if (tags[i].Trim().Contains(term.Trim()) || term.Trim().Contains(tags[i].Trim())){
				return true;
			}
		}
		return false;
	}

	public int getFCost(MapLabel other){//weird ass heuristic. I pity the fool who has to debug this
		if (this.IsElevator){
			int bestCost = Mathf.Abs(other.OnFloor - this.OnFloor);
			foreach (int f in this.ElevatorFloors){
				if (Mathf.Abs(other.OnFloor - f) < bestCost){
					bestCost = Mathf.Abs(other.OnFloor - f);
				}
			}
			return bestCost;
		} else {
			return Mathf.Abs(other.OnFloor - this.OnFloor);
		}
	}

	public bool isOnFloor(int f){
		if (isElevator){
			foreach (int i in ElevatorFloors){
				if (i == f){
					return true;
				}
			}
			return false;
		} else {
			return this.OnFloor == f;
		}
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

	public bool Visible {
		get {
			return this.gameObject.activeInHierarchy;
		} set {
			this.gameObject.SetActive(value);
		}
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

	public int OnFloor {
		get {
			return floor;
		}
	}

	public bool IsElevator {
		get {
			return isElevator;
		}
	}

	public int[] ElevatorFloors {
		get {
			return elevatorFloors;
		}
	}

	public static Dictionary<int, MapLabel> MapLabels {
		get {
			return mapLabels;
		}
	}

	public static MapLabel createLabel(Vector3 position, int nid, string alltext, int priority, int flor, bool elevator, int[] servicedFloors){
		if (mainParent == null){
			mainParent = new GameObject("Labels");
			mainParent.tag = "Waypoints";
			mainParent.transform.position = Vector3.zero;
		}

		GameObject obj = GameObject.Instantiate(Resources.Load("Prefabs/MapLabel")) as GameObject;
		obj.transform.parent = mainParent.transform;
		obj.transform.position = position;

		MapLabel lab = obj.GetComponent("MapLabel") as MapLabel;
		lab.id = nid;
		string[] text = alltext.Split(new char[]{','}, System.StringSplitOptions.RemoveEmptyEntries);
		lab.label = text.Length >= 1 ? text[0] : "";
		lab.tags = text;
		lab.priority = priority;
		lab.floor = flor;
		lab.isElevator = elevator;
		lab.elevatorFloors = servicedFloors;

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
			if (pieces.Length >= 6){
				int nid = int.Parse(pieces[0].Trim());
				string ntext = pieces[1].Trim();
				float x = float.Parse(pieces[2].Trim());
				float y = float.Parse(pieces[3].Trim());
				int pri = int.Parse(pieces[4].Trim());
				int flor = int.Parse(pieces[5].Trim());
				bool ele = (int.Parse(pieces[6].Trim()) == 1);
				string[] fls = pieces[7].Trim().Split(new char[]{','}, System.StringSplitOptions.RemoveEmptyEntries);
				int[] efls = new int[fls.Length];
				for (int i = 0; i < fls.Length; i++){
					efls[i] = int.Parse(fls[i]);
				}


				if (pri > 0){
					Vector3 oldPos = new Vector3(x, y, -4f);
					createLabel(MapMaker.mapSpaceToWorldSpaceFull(oldPos), nid, ntext, pri, flor, ele, efls);
				}
			}
		}

		//MapMaker.needsToScan = true;
	}

	public static IEnumerator updateMarkerSave(){
		WWW www = new WWW("http://marshlandgames.com/HospitalProject/Client/MarkerPlacement/show_formatted_markers.php");

		yield return www;

		//Debug.Log(www.text);

		string[] piecesArr = www.text.Split(new string[]{"<br />"}, System.StringSplitOptions.RemoveEmptyEntries);

		string newSaveString = "";
		for (int i = 0; i < piecesArr.Length; i++){
			newSaveString += piecesArr[i] + "\n";
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

	public static List<MapLabel> ElevatorsOnFloor(int floor){
		List<MapLabel> availableElevators = new List<MapLabel>();
		foreach (MapLabel m in MapLabel.MapLabels.Values){
			if (m.IsElevator && m.isOnFloor(floor)){
				availableElevators.Add(m);
			}
		}
		return availableElevators;
	}

	public static void onFloorChange(){
		foreach (int i in mapLabels.Keys){
			mapLabels[i].checkVisible();
			mapLabels[i].scanSelf();
		}
	}

}
