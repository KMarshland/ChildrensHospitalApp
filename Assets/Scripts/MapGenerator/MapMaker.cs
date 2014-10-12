using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KaiTools;
using Pathfinding;

public class MapMaker : MonoBehaviour {

	public static MapMaker instance;
	public static bool needsToScan = false;

	public Texture2D[] maps;//the image map that you can easily set

	Vector3 startPos;//point you're pathing from
	Vector3 endPos;//point you're pathing to
	
	LineRenderer line;//the linerenderer that draws the path
	static Vector3[] pathPoints = new Vector3[]{new Vector3(0,0,0)};//the list of points on the path
	static List<Vector3> linePoints = new List<Vector3>();
	static Path path;

	static Seeker seeker;
	static AstarPath astarPath;

	public static Floor[] floors = new Floor[]{
		new Floor(Resources.Load("Textures/Maps/CMHFloor0") as Texture2D),
		new Floor(Resources.Load("Textures/Maps/CMHFloor1") as Texture2D),
		new Floor(Resources.Load("Textures/Maps/CMHFloor2") as Texture2D),
		new Floor(Resources.Load("Textures/Maps/CMHFloor3") as Texture2D),
		new Floor(Resources.Load("Textures/Maps/CMHFloor4") as Texture2D),
		new Floor(Resources.Load("Textures/Maps/CMHFloor5") as Texture2D),
		new Floor(Resources.Load("Textures/Maps/CMHFloor6") as Texture2D)
	};
	static Floor floor;

	public string[] extraLog;
	static Dictionary<string, float> logs = new Dictionary<string, float>();

	// Use this for initialization
	void Start () {
		instance = this;

		float theBeforeTime = Time.realtimeSinceStartup;
		/*for (int i = 0; i < floors.Length; i++){
			floors[i].Id = i+1;
			//floors[i].make();
		}*/



		line = this.GetComponent<LineRenderer>();//get a reference to the line
		seeker = GetComponent<Seeker>();
		astarPath = GetComponent<AstarPath>();
		//logs["Getting components"] = Time.realtimeSinceStartup - theBeforeTime;

		ActiveFloor = floors[0];
		//logs["Set active floor"] = Time.realtimeSinceStartup - theBeforeTime;

		//add the waypoints to the graph
		//float thebeforetime = Time.realtimeSinceStartup;

		PlayerPrefs.SetString("MarkerSave", "");
		MapLabel.loadMarkers();
		//logs["Initial marker load"] = Time.realtimeSinceStartup - theBeforeTime;
		if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork){
			StartCoroutine(MapLabel.updateMarkerSave());
		}
		//logs["Start update of markers"] = Time.realtimeSinceStartup - theBeforeTime;

		logs["Startup time"] = Time.realtimeSinceStartup - theBeforeTime;

		setLogs();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (needsToScan){
			Debug.Log("Scanned from update");
			AstarPath.active.Scan();
			needsToScan = false;
		}

		if (Input.GetKeyDown(KeyCode.UpArrow)){
			//Debug.Log("Was " + ActiveFloor.Id);
			ActiveFloor = floors[ActiveFloor.Id % floors.Length];
			//Debug.Log("Now " + ActiveFloor.Id);
		}
	}

	void makeLine(Vector3[] points){//a handy-dandy function to turn a list of points into something on the screen

		int height = 3;

		//tell it how many points there are
		line.SetVertexCount(points.Length * (height));

		//loop through all the points and add them as vector3s
		int tc = 0;
		for (int h = 0; h < height; h++){
			if (h % 2 == 0){
				for (int i = 0; i < points.Length; i++){
					line.SetPosition(tc, points[i] + new Vector3(0f, 0f, h*(-0.5f)));
					tc ++;
				}
			} else {
				for (int i = 0; i < points.Length; i++){
					line.SetPosition(tc, points[points.Length - 1 - i] + new Vector3(0f, 0f, h*(-0.5f)));
					tc ++;
				}
			}
		}
	}

	void setLogs(){
		extraLog = new string[logs.Count];
		int _i = 0;
		foreach (string s in logs.Keys){
			extraLog[_i] = s + ": " + (1000f * (logs[s])).ToString("F0") + " ms";
			_i ++;
		}

		string q = "";
		foreach (string s in extraLog){
			q += s + "\n";
		}
		//Debug.Log(q);
	}

	public void genPath(){//allows you to repath and display in one call

		seeker.StartPath (startPos, endPos, OnPathComplete);
	}
	
	public void OnPathComplete(Path p) {
		if (p.error){
			//Debug.Log("Error: " + p.errorLog);
			return;
		}

		path = p;
		pathPoints = Point.smoothPathAdvanced(Point.smoothPathBasic(path.vectorPath.ToArray()));
		/*Debug.Log("A: " + path.vectorPath.ToArray().Length + "\nB:" + 
		          Point.smoothPathBasic(path.vectorPath.ToArray()).Length + "\nC:" + 
		          pathPoints.Length);*/
		/*foreach (Vector3 v in pathPoints){
			//Debug.Log(v);
		}*/

		//SimpleSmoothModifier ssm = GetComponent<SimpleSmoothModifier>();
		//ssm.iterations = 2;
		//ssm.offset = 1f;
		List<Vector3> ppp = new List<Vector3>();
		ppp.AddRange(pathPoints);
		//ssm.smoothType = SimpleSmoothModifier.SmoothType.Bezier;
		//linePoints = ssm.SmoothSimple(ppp);
		linePoints = ppp;

		makeLine(linePoints.ToArray());
		//makeLine(pathPoints);

		GetComponent<MapCameraControl>().recalculateFromPath();
	}

	public Vector3 StartPos {
				get {
						return startPos;
				}
				set {
						startPos = value;
				}
		}

	public Vector3 EndPos {
				get {
						return endPos;
				}
				set {
						endPos = value;
				}
		}

	public Vector3[] PathPoints {
		get {
			return pathPoints;
		}
	}

	public Vector3[] LinePoints {
		get {
			return linePoints.ToArray();
		}
	}

	static void loadGraphForFloor(Floor f){
		TextAsset cache = (Resources.Load("PathCaches/Floor" + (f.Id)) as TextAsset);
		if (cache != null){//load the cache if it exists
			byte[] bytes = cache.bytes;
			AstarPath.active.astarData.DeserializeGraphs (bytes);
			//Debug.Log("Deserialized successfully");
		} else {//scan the graph
			MapMaker.needsToScan = true;
			Debug.LogWarning("You could feed starving children in Africa with all the processing power this wastes. " + 
			                 "You better have a damn good reason for not loading it from the cache or dynamically adding a single point.");
		}
	}

	public static Floor ActiveFloor {
		get {
			return floor;
		} set {
			float theBeforeTime = Time.realtimeSinceStartup;
			if (floor != null){
				floor.Active = false;
			}
			//logs["Activate floor"] = Time.realtimeSinceStartup - theBeforeTime;
			floor = value;
			//logs["Set floor value"] = Time.realtimeSinceStartup - theBeforeTime;
			floor.spawn();
			//logs["Spawn floor"] = Time.realtimeSinceStartup - theBeforeTime;
			loadGraphForFloor(floor);
			//logs["Load graph"] = Time.realtimeSinceStartup - theBeforeTime;
			MapMaker.instance.setLogs();
			MapLabel.onFloorChange();
		}
	}

	public static Vector3 mapSpaceToWorldSpace(Vector3 mapSpace){
		return new Vector3(
			(mapSpace.x * 0.25f) + 0.0f,
			(0.25f * mapSpace.y) + 0.0f,
			mapSpace.z
			);
		
	}
	
	public static Vector3 mapSpaceToWorldSpaceFull(Vector3 mapSpace){
		return new Vector3(
			(mapSpace.x * 0.228086f) + 0.271914f,
			(-0.31365313653f * mapSpace.y) + 254.313653137f,
			mapSpace.z
			);
	}

}
