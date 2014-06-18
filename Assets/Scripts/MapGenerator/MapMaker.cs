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

		GridGraph navg = (GridGraph)astarPath.graphs[0];
		navg.GetNodes( delegate ( GraphNode node ) {
			node.Walkable = !isOccupied((node.position.x - 750)/1000, (node.position.y + 1000)/1000);

			/*if (node.NodeIndex == 1){
				Debug.Log(node.position.x + ", " + node.position.y + ", " + node.position.z);
			}*/

			return true;
		});
		navg.erodeIterations = 1;
		navg.ErodeWalkableArea();
		navg.erodeIterations = 0;

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

		SimpleSmoothModifier ssm = GetComponent<SimpleSmoothModifier>();
		ssm.iterations = 2;
		ssm.offset = 1f;
		List<Vector3> ppp = new List<Vector3>();
		ppp.AddRange(pathPoints);
		//ssm.smoothType = SimpleSmoothModifier.SmoothType.Bezier;
		linePoints = ssm.SmoothSimple(ppp);
		linePoints = ppp;

		//linePoints = new List<Vector3>();
		//linePoints.AddRange(pathPoints);
		//Debug.Log(linePoints.Count);
		/*int chopper = linePoints.Count - 1;
		for (int i = linePoints.Count - 2; i >= 0; i--){
			if (linePoints[i].x == linePoints[i+1].x && 
			    linePoints[i].y == linePoints[i+1].y && 
			    linePoints[i].z == linePoints[i+1].z){
				chopper = i;
			}
		}
		linePoints.RemoveRange(chopper, linePoints.Count - chopper);*/
		//ssm.iterations = 0;



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
		makeCubesFromMapping(occupiedPoints);
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
