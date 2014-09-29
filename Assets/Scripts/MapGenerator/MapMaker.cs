using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KaiTools;
using Pathfinding;

public class MapMaker : MonoBehaviour {

	public static MapMaker instance;

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
	public static Floor floor = floors[0];

	// Use this for initialization
	void Start () {
		instance = this;

		float theBeforeTime = Time.realtimeSinceStartup;

		/*for (int i = 0; i < floors.Length; i++){
			floors[i].Id = i+1;
			//floors[i].make();
		}*/


		line = this.GetComponent("LineRenderer") as LineRenderer;//get a reference to the line

		seeker = GetComponent<Seeker>();
		astarPath = GetComponent<AstarPath>();
		floor.spawn();
		loadGraphForFloor(floor);

		//add the waypoints to the graph
		//float thebeforetime = Time.realtimeSinceStartup;

		PlayerPrefs.SetString("MarkerSave", "");
		MapLabel.loadMarkers();
		if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork){
			StartCoroutine(MapLabel.updateMarkerSave());
		}

		Debug.Log("Startup time: " + (1000f * (Time.realtimeSinceStartup - theBeforeTime)).ToString("F0") + " ms");
	}

	void loadGraphForFloor(Floor f){
		TextAsset cache = (Resources.Load("PathCaches/Floor" + (f.Id)) as TextAsset);
		if (cache != null){//load the cache if it exists
			byte[] bytes = cache.bytes;
			AstarPath.active.astarData.DeserializeGraphs (bytes);
		} else {//scan the graph
			AstarPath.active.Scan();
		}
	}

	
	// Update is called once per frame
	void FixedUpdate () {

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
