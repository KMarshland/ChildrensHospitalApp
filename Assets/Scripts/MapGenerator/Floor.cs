using UnityEngine;
//using System.IO;
using System.Collections;
using System.Collections.Generic;
//using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters.Binary;
using KaiTools;
using Pathfinding;

public class Floor {

	const int obstacleLayer = 8;
	static int currId = 1;

	bool [,] occupiedPoints;
	public Texture2D map;
	//Texture2D omap;

	GameObject allFather;

	int id;

	float buildTime;
	bool made;

	public Floor(Texture2D nmap){
		this.Id = currId;
		currId ++;
		map = nmap;
		made = false;
		//omap = nmap;
	}

	public void make(){//build a 3d world from an image

		float t = Time.realtimeSinceStartup;

		if (Resources.Load("Prefabs/Floors/Floor " + Id) == null){
			createOccupied();

			Debug.Log("Building floor " + Id);
			build();
		}

		buildTime = Time.realtimeSinceStartup - t;
		made = true;
	}

	public void spawn(){
		if (!made){
			make();
		}

		if (allFather == null){
			Object c = Resources.Load("Prefabs/Floors/Floor " + Id);
			if (c == null){
				build();
			} else {
				allFather = GameObject.Instantiate(c) as GameObject;
			}
		}

		this.Active = true;
	}

	public void build(){
		makeCubes();
		makeWaypoints();
	}

	public void createOccupied(){
		float t = Time.realtimeSinceStartup;
		
		findOccupiedPoints();
		Debug.Log("findoccupied: " + (Time.realtimeSinceStartup - t)); //=> 865 ms
		erodeMapping(2);
		Debug.Log("erode: " + (Time.realtimeSinceStartup - t)); //=> 1482 ms

	}
	
	public bool isOccupied(Point p){//check if a point is occupied, based on an image
		return isOccupied(p.x, p.y);
	}
	
	public bool isOccupied(int x, int y){//check if a point is occupied, based on an image
		
		if (x < 0 || y < 0){
			return false;
		}

		if (occupiedPoints == null){
			createOccupied();
		}

		if (x > occupiedPoints.GetLength(0) || y > occupiedPoints.GetLength(1)){
			return false;
		}
		
		return occupiedPoints[x, y];
	}

	public Vector3 closestUnnocupiedTo(int x, int y){
		return closestUnnocupiedTo(new Vector3(x, y, 0));
	}

	public Vector3 closestUnnocupiedTo(Vector3 v){
		return Point.pointToVector3(closestUnnocupiedTo(Point.vector3ToPoint(v)));
	}
	
	public Point closestUnnocupiedTo(Point p){

		if (!isOccupied(p)){
			return p;
		}
		
		//List<Vector3> vs = new List<Vector3>();
		
		int x = p.x;
		int y = p.y;
		
		//int maxDiameter = 500;
		
		int dx = 0;
		int dy = -1;
		
		for (int i = 0; (i < 500) && isOccupied(p); i++){
			//if ((-maxDiameter/2f < x) && (x <= maxDiameter/2f) && (-maxDiameter/2f < y) && (y <= maxDiameter/2f)){
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
			//}
		}
		return p;

		/*
		//No fucking clue what this is for or what I was thinking when writing it.
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
		*/
	}

	bool[,] findOccupiedPoints(){
		//store the size of it - marginally faster since it's being looped over
		int width = map.width;
		int height = map.height;
		
		int resolution = 1;//1 is full resolution, increase for lower res
		float resToTheMinusTwo = 1f/((resolution+0f) * (resolution+0f));//cache this value. Cuts off a few milliseconds
		float scale = 0.25f;//squish it a bit. Won't make generation faster, but will make pathing faster
		float scaleDivResolution = scale/((resolution + 0f));//cache this value. Cuts off a few milliseconds
		float tolerance = 0.85f;
		
		bool[,] oPoints = new bool[(int)(width),(int)(height)];
		
		//loop through all the pixels
		for (int x = 0; x < width; x += resolution){
			for (int y = 0; y < height; y += resolution){
				
				/*float grayscaleSum = 0;//the sum of all grayscale values
				
				//check a square of size res around the point. 
				for (int x2 = x; x2 < x + resolution; x2++){
					for (int y2 = y; y2 < y + resolution; y2++){
						grayscaleSum += map.GetPixel(x2,y2).grayscale;
					}
				}*/
				
				//If it's mostly black, make that point black
				if (map.GetPixel(x,y).grayscale * resToTheMinusTwo < tolerance){
					//if it's black, make a cube there. Otherwise leave it alone 
					//(makeCubeAtPoint(new Vector3(x * scaleDivResolution, y * scaleDivResolution, 0), scale)).transform.parent = parentObj.transform;
					oPoints[(int)(x * scaleDivResolution), (int)(y * scaleDivResolution)] = true;
				}
			}
		}

		occupiedPoints = oPoints;
		return oPoints;
	}

	void erodeMapping(int degree){
		occupiedPoints = erodeMapping(occupiedPoints, degree);
	}
	
	bool[,] erodeMapping(bool[,] mapping, int degree){
		
		//store the new values
		bool[,] nmapping = new bool[mapping.GetLength(0), mapping.GetLength(1)];
		
		//loop through the old ones
		for (int y = 0; y < mapping.GetLength(1); y++){
			bool topEdge = y == 0;
			bool bottomEdge = y == mapping.GetLength(1) - 1;
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
					
					if (topEdge){//it is on the top edge
						minNeighborNum --;
					} else if (mapping[x, y - 1]){//it isn't so check the tile one to the top
						neighborNum ++;
					}
					
					if (bottomEdge){//it is on the bottom edge
						minNeighborNum --;
					} else if (mapping[x, y + 1]){//it isn't so check the tile one to the bottom
						neighborNum ++;
					}
					
					if (minNeighborNum <= 0){
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

	void makeCubes(){
		makeCubesFromMapping(occupiedPoints);
	}
	
	void makeCubesFromMapping(bool[,] mapping){
		//loop through, for each y, from left to right, creating filled strips as it goes
		bool on = false;
		int startx;
		GameObject parentObj = new GameObject("Map Parent");
		parentObj.layer = obstacleLayer;
		parentObj.transform.parent = AllFather.transform;
		
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

	void makeWaypoints(){
		makeWaypointsFromMapping(occupiedPoints);
	}
	
	Vector3[] makeWaypointsFromMapping(bool[,] mapping){
		float thebeforetime = Time.realtimeSinceStartup;
		List<Vector3> points = new List<Vector3>();
		Transform parentT = (new GameObject("Waypoint Parent")).transform;
		parentT.parent = AllFather.transform;
		
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

	/*public void serialize() {

		float thebeforetime = Time.realtimeSinceStartup;
		IFormatter formatter = new BinaryFormatter();
		Stream stream = new FileStream("PointCache" + this.Id + ".bin", FileMode.Create, FileAccess.Write, FileShare.None);
		formatter.Serialize(stream, occupiedPoints);
		stream.Close();
		Debug.Log("Serialization time: " + (Time.realtimeSinceStartup - thebeforetime));
	}

	public void deserialize(string filename){
		float thebeforetime = Time.realtimeSinceStartup;

		IFormatter formatter = new BinaryFormatter();
		Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
		bool[,] obj = (bool[,]) formatter.Deserialize(stream);
		stream.Close();

		Debug.Log("Deserialization time: " + (Time.realtimeSinceStartup - thebeforetime));
	}*/

	public GameObject AllFather {
		get {
			if (allFather == null){
				allFather = new GameObject("Floor " + Id);
			}
			return allFather;
		}
	}

	public bool Active {
		get {
			if (allFather == null){
				return false;
			}
			return this.allFather.activeInHierarchy;
		} set {
			if (allFather == null){
				return;
			}
			this.allFather.SetActive(value);
		}
	}

	public int Id {
		get {
			return id;
		}
		set {
				id = value;
		}
	}
	
	static GameObject makeCubeAtPoint(Vector3 pos){//quick function to spawn a cube and move it somewhere
		return makeCubeAtPoint(pos, 1f);
	}
	
	static GameObject makeCubeAtPoint(Vector3 pos, float scale){//quick function to spawn a cube, scale it, and move it somewhere
		//spawn the cube
		GameObject obj = GameObject.Instantiate(Resources.Load("Prefabs/Cube")) as GameObject;
		obj.layer = obstacleLayer;
		//move it to where ever
		obj.transform.position = pos;
		obj.transform.localScale = new Vector3(scale, scale, scale);
		
		return obj;
	}
	
	static GameObject makeCubeStartingAtPoint(Vector3 pos, Vector3 scale){//quick function to spawn a cube, scale it, and move it somewhere
		//spawn the cube
		GameObject obj = GameObject.Instantiate(Resources.Load("Prefabs/Cube")) as GameObject;
		obj.layer = obstacleLayer;
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
	


}
