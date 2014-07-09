using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KaiTools;
using Pathfinding;

public class MapMaker : MonoBehaviour {

	public Texture2D map;//the image map that you can easily set
	static Texture2D omap;//a static version of that so the path algo can access it easily

	Vector3 startPos;//point you're pathing from
	Vector3 endPos;//point you're pathing to
	
	LineRenderer line;//the linerenderer that draws the path
	static Vector3[] pathPoints;//the list of points on the path
	static List<Vector3> linePoints;
	static Path path;
	static bool [,] occupiedPoints;
	static Vector3[] waypoints;

	static Seeker seeker;
	static AstarPath astarPath;

	// Use this for initialization
	void Start () {
		//float theBeforeTime = Time.realtimeSinceStartup;

		line = this.GetComponent("LineRenderer") as LineRenderer;//get a reference to the line
		make(map);//spawn the cubes for the map
		omap = map;//set the static version to the editable version
		pathPoints = new Vector3[]{new Vector3(0,0,0)};
		linePoints = new List<Vector3>();

		seeker = GetComponent<Seeker>();
		astarPath = GetComponent<AstarPath>();
		astarPath.Scan();

		/*GridGraph navg = (GridGraph)astarPath.graphs[0];
		navg.GetNodes( delegate ( GraphNode node ) {
			node.Walkable = !isOccupied((node.position.x - 750)/1000, (node.position.y + 1000)/1000);

			//if (node.NodeIndex == 1){
			//	Debug.Log(node.position.x + ", " + node.position.y + ", " + node.position.z);
			//}

			return true;
		});
		navg.erodeIterations = 1;
		navg.ErodeWalkableArea();
		navg.erodeIterations = 0;*/

		//add the waypoints to the graph
		float thebeforetime = Time.realtimeSinceStartup;


		/*PointGraph navg = (PointGraph)astarPath.graphs[0];
		navg.ScanGraph();

		AstarPath.active.AddWorkItem (new AstarPath.AstarWorkItem (delegate () {

		}, null));*/

		/*Dictionary<int, List<PointNode>> xPoints = new Dictionary<int, List<PointNode>>();
		Dictionary<int, List<PointNode>> yPoints = new Dictionary<int, List<PointNode>>();
		AstarPath.active.AddWorkItem (new AstarPath.AstarWorkItem (delegate () {
			foreach (Vector3 v in waypoints){
				PointNode p = navg.AddNode((Int3)v);
				if (!xPoints.ContainsKey(p.position.x)){
					xPoints[p.position.x] = new List<PointNode>();
				} else {
					//PointNode p0 = xPoints[p.position.x][xPoints[p.position.x].Count - 1];
					xPoints[p.position.x].Add(p);
					//p0.AddConnection(p, 1);
					//p.AddConnection(p0, 1);
				}

				if (!yPoints.ContainsKey(p.position.y)){
					yPoints[p.position.y] = new List<PointNode>();
				} else {
					//PointNode p0 = xPoints[p.position.x][xPoints[p.position.x].Count - 1];
					yPoints[p.position.y].Add(p);
					//p0.AddConnection(p, 1);
					//p.AddConnection(p0, 1);
				}
				//p.AddConnection();
				//navg.AddToLookup(p);

				//p.RecalculateConnectionCosts();
			}
		}, new System.Func<bool, bool>(delegate(bool arg) {
			foreach (int key in xPoints.Keys){
				for (int i = 1; i < xPoints[key].Count; i++){
					PointNode p0 = xPoints[key][i];
					PointNode p1 = xPoints[key][i-1];

					int x0 = p0.position.x/1000;
					int x1 = p1.position.x/1000;
					int y = p0.position.y/1000;

					bool validConnection = true;
					for (int x = x1; x <= x0; x++){
						validConnection &= !isOccupied(x,y);
					}
					Debug.Log(validConnection + ", " + x0 + ", " + x1 + ", " + y);

					if (validConnection){
						p0.AddConnection(p1, (uint)(x0 - x1));
						p1.AddConnection(p0, (uint)(x0 - x1));
						p0.RecalculateConnectionCosts();
						p1.RecalculateConnectionCosts();
					}
				}
				//Debug.Log(key);
			}

			return true;
		})));*/



		
		//Debug.Log((Time.realtimeSinceStartup - thebeforetime) + " seconds elapsed in waypoint adding");

		//Debug.Log((Time.realtimeSinceStartup - theBeforeTime) + " seconds elapsed");
		PlayerPrefs.SetString("MarkerSave", "");
		MapLabel.loadMarkers();
		if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork){
			StartCoroutine(MapLabel.updateMarkerSave());
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
		/*
		//generate the path
		Point[] npath = Point.arraylistToArray(Point.aStar(Point.vector3ToPoint(startPos), Point.vector3ToPoint(endPos)));
		//smooth the path
		npath = Point.smoothPathBasic(npath);//use the fast but imperfect one first
		npath = Point.smoothPathAdvanced(npath);//now that you cut down on the number of points, go over it nicely
		//draw the path
		pathPoints = Point.pointArrayToVector3Array(npath);
		*/

		//makeLine(pathPoints);

		//Debug.Log(startPos);
		//Debug.Log(endPos);

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

	public static bool isOccupied(Point p){//check if a point is occupied, based on an image
		return isOccupied(p.x, p.y);
	}

	public static bool isOccupied(int x, int y){//check if a point is occupied, based on an image
		
		if (x < 0 || y < 0){
			return false;
		}
		
		if (x > occupiedPoints.GetLength(0) || y > occupiedPoints.GetLength(1)){
			return false;
		}
		
		return occupiedPoints[x, y];
	}

	public static Vector3 closestUnnocupiedTo(Vector3 v){
		return Point.pointToVector3(closestUnnocupiedTo(Point.vector3ToPoint(v)));
	}

	public static Point closestUnnocupiedTo(Point p){

		if (!isOccupied(p)){
			return p;
		}

		//List<Vector3> vs = new List<Vector3>();

		int x = p.x;
		int y = p.y;

		int maxDiameter = 500;

		int dx = 0;
		int dy = -1;

		for (int i = 0; i < maxDiameter * maxDiameter && isOccupied(p); i++){
			if ((-maxDiameter/2f < x) && (x <= maxDiameter/2f) && (-maxDiameter/2f < y) && (y <= maxDiameter/2f)){
				//Debug.Log(x + ", " + y);
				//vs.Add(new Vector3(x, y, 0f));
				p = new Point(x, y, p.z);

				if (x == y || (x < 0 && x == -1 * y) || (x > 0 && x == 1-y)){
					//dx, dy = -dy, dx;
					int pdy = dy;
					dy = dx;
					dx = -1*pdy;
				}
				x += dx;
				y += dy;
			}
		}

		Point[] adj = p.getAdjacentPoints2d();
		int best = 0;
		Point bestP = p;

		Point[] currAdj = adj;

		for (int i = 0; i < currAdj.Length; i++){
			if (!isOccupied(currAdj[i])){
				best ++;
			}
		}

		foreach (Point cp in adj){
			currAdj = cp.getAdjacentPoints2d();
			int openNeighbors = 0;
			for (int i = 0; i < currAdj.Length; i++){
				if (!isOccupied(currAdj[i])){
					openNeighbors ++;
				}
			}

			if (openNeighbors > best){
				best = openNeighbors;
				bestP = cp;
			}
		}

		return bestP;
	}
	
	static void make(Texture2D map){//build a 3d world from an image
		//store the size of it - marginally faster since it's being looped over
		int width = map.width;
		int height = map.height;

		int resolution = 1;//1 is full resolution, increase for lower res
		float resToTheMinusTwo = 1f/((resolution+0f) * (resolution+0f));//cache this value. Cuts off a few milliseconds
		float scale = 0.25f;//squish it a bit. Won't make generation faster, but will make pathing faster
		float scaleDivResolution = scale/((resolution + 0f));//cache this value. Cuts off a few milliseconds
		float tolerance = 0.85f;

		occupiedPoints = new bool[(int)(width),(int)(height)];

		//loop through all the pixels
		for (int x = 0; x < width; x += resolution){
			for (int y = 0; y < height; y += resolution){

				float grayscaleSum = 0;//the sum of all grayscale values

				//check a square of size res around the point. 
				for (int x2 = x; x2 < x + resolution; x2++){
					for (int y2 = y; y2 < y + resolution; y2++){
						grayscaleSum += map.GetPixel(x2,y2).grayscale;
					}
				}

				//If it's mostly black, make that point black
				if (grayscaleSum * resToTheMinusTwo < tolerance){
					//if it's black, make a cube there. Otherwise leave it alone 
					//(makeCubeAtPoint(new Vector3(x * scaleDivResolution, y * scaleDivResolution, 0), scale)).transform.parent = parentObj.transform;
					occupiedPoints[(int)(x * scaleDivResolution), (int)(y * scaleDivResolution)] = true;
				}
			}
		}

		occupiedPoints = erodeMapping(occupiedPoints, 2);
		//makeCubesFromMapping(occupiedPoints);
		//waypoints = makeWaypointsFromMapping(occupiedPoints);
	}

	static bool[,] erodeMapping(bool[,] mapping, int degree){

		//store the new values
		bool[,] nmapping = new bool[mapping.GetLength(0), mapping.GetLength(1)];

		//loop through the old ones
		for (int y = 0; y < mapping.GetLength(1); y++){
			for (int x = 0; x < mapping.GetLength(0); x++){
				if (mapping[x,y]){//if it is occupied, check if it needs to be eroded
					int neighborNum = 0;//store how many neighbors it has
					int minNeighborNum = degree;//how many neighbors does it need?

					if (x == 0){//it is on the left edge
						minNeighborNum --;
					} else if (mapping[x - 1, y]){//it isn't so check the tile one to the left
						neighborNum ++;
					}

					if (x == mapping.GetLength(0) - 1){//it is on the right edge
						minNeighborNum --;
					} else if (mapping[x + 1, y]){//it isn't so check the tile one to the right
						neighborNum ++;
					}

					if (y == 0){//it is on the top edge
						minNeighborNum --;
					} else if (mapping[x, y - 1]){//it isn't so check the tile one to the top
						neighborNum ++;
					}
					
					if (y == mapping.GetLength(1) - 1){//it is on the bottom edge
						minNeighborNum --;
					} else if (mapping[x, y + 1]){//it isn't so check the tile one to the bottom
						neighborNum ++;
					}

					if (minNeighborNum <= 0){//it doesn't need eroding
						nmapping[x,y] = true;
					} else {
						nmapping[x,y] = neighborNum >= minNeighborNum;//if it has enough neighbors, keep it occupied, otherwise nuke it
					}
				} else {//if  it's not occupied, it will stay unoccupied
					nmapping[x,y] = false;
				}
			}
		}

		return nmapping;
	}

	static void makeCubesFromMapping(bool[,] mapping){
		//loop through, for each y, from left to right, creating filled strips as it goes
		bool on = false;
		int startx;
		GameObject parentObj = new GameObject("Map Parent");

		for (int y = 0; y < mapping.GetLength(1); y++){

			on = false;
			startx = 0;

			for (int x = 0; x < mapping.GetLength(0); x++){
				if (on && !mapping[x,y]){//turning off
					on = false;
					//fill from where it started to here (well, 1 before here, where it was still on)
					makeCubeStartingAtPoint(
						new Vector3(x, y, 0f), 
						new Vector3((x - startx), 1f, 1f)
					).transform.parent = parentObj.transform;
				} else if (!on && mapping[x,y]){//turning on
					on = true;
					startx = x;//remember where it started
				}
			}
		}

		parentObj.transform.localScale = new Vector3(1f, 1f, 2f);
		parentObj.transform.position += new Vector3(-0.5f, 0.5f, 0f);
	}

	static Vector3[] makeWaypointsFromMapping(bool[,] mapping){
		float thebeforetime = Time.realtimeSinceStartup;
		List<Vector3> points = new List<Vector3>();
		Transform parentT = (new GameObject("Waypoint Parent")).transform;

		for (int y = 0; y < mapping.GetLength(1); y++){
			for (int x = 0; x < mapping.GetLength(0); x++){
				if (!mapping[x,y]){

					//int neighborNum = 0;//store how many neighbors it has
					//int minNeighborNum = 1;//how many neighbors does it need?

					bool leftEdge = (x == 0);
					bool rightEdge = (x == mapping.GetLength(0) - 1);
					bool topEdge = (y == mapping.GetLength(1) - 1);
					bool bottomEdge = (y == 0);

					//int sideNum = 0;
					//int diagonalNum = 0;
					bool[] side = new bool[4];//left, right, top, bottom
					bool[] diagonal = new bool[4];//UL, UR, BL, BR

					if (leftEdge || mapping[x - 1, y]){
						side[0] = true;
					} else {
						side[0] = false;
					}
					
					if (rightEdge || mapping[x + 1, y]){
						side[1] = true;
					} else {
						side[1] = false;
					}
					
					if (topEdge || mapping[x, y + 1]){
						side[2] = true;
					} else {
						side[2] = false;
					}
					
					if (bottomEdge || mapping[x, y - 1]){
						side[3] = true;
					} else {
						side[3] = false;
					}

					if (topEdge && leftEdge){
						diagonal[0] = true;
					} else if (!(topEdge || leftEdge) && mapping[x - 1, y + 1]){
						diagonal[0] = true;
					} else {
						diagonal[0] = false;
					}

					if (topEdge && rightEdge){
						diagonal[1] = true;
					} else if (!(topEdge || rightEdge) && mapping[x + 1, y + 1]){
						diagonal[1] = true;
					} else {
						diagonal[1] = false;
					}

					if (bottomEdge && leftEdge){
						diagonal[2] = true;
					} else if (!(bottomEdge || leftEdge) && mapping[x - 1, y - 1]){
						diagonal[2] = true;
					} else {
						diagonal[2] = false;
					}
					
					if (bottomEdge && rightEdge){
						diagonal[3] = true;
					} else if (!(bottomEdge || rightEdge) && mapping[x + 1, y - 1]){
						diagonal[3] = true;
					} else {
						diagonal[3] = false;
					}

					/*if (diagonalNum >= 1){
						if (diagonalNum < sideNum || sideNum == 0){
							spawnWaypoint(new Vector3(x, y, 0)).parent = parentT;
							points.Add(new Vector3(x, y, 0));
						}
					}*/
					bool spawning = false;
					for (int i = 0; i < 4; i++){
						if (diagonal[i]){
							if (!(side[i%2] ^ side[Mathf.RoundToInt((i-0.5f)/2f)%2 + 2])){
								spawning = true;
							}
						}
					}

					if (spawning){
						spawnWaypoint(new Vector3(x, y, 0)).parent = parentT;
						points.Add(new Vector3(x, y, 0));
					}
				}
			}
		}
		
		Debug.Log(
			(Time.realtimeSinceStartup - thebeforetime) + 
			" seconds elapsed to find " + points.Count + " waypoints"
		);

		return points.ToArray();
	}

	static GameObject makeCubeAtPoint(Vector3 pos){//quick function to spawn a cube and move it somewhere
		return makeCubeAtPoint(pos, 1f);
	}

	static GameObject makeCubeAtPoint(Vector3 pos, float scale){//quick function to spawn a cube, scale it, and move it somewhere
		//spawn the cube
		GameObject obj = GameObject.Instantiate(Resources.Load("Prefabs/Cube")) as GameObject;
		//move it to where ever
		obj.transform.position = pos;
		obj.transform.localScale = new Vector3(scale, scale, scale);

		return obj;
	}

	static GameObject makeCubeStartingAtPoint(Vector3 pos, Vector3 scale){//quick function to spawn a cube, scale it, and move it somewhere
		//spawn the cube
		GameObject obj = GameObject.Instantiate(Resources.Load("Prefabs/Cube")) as GameObject;
		//move it to where ever
		obj.transform.position = pos - (scale*0.5f);
		obj.transform.localScale = scale;
		
		return obj;
	}

	static Transform spawnWaypoint(Vector3 pos){
		Transform obj = (GameObject.Instantiate(Resources.Load("Prefabs/Waypoint")) as GameObject).transform;
		//move it to where ever
		obj.position = pos;
		
		return obj;
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
