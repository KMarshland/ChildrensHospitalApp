using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KaiTools{
	public class Point {
		
		public int x, y, z;//its position
		public Point parent;//have a parent point , to be used in pathing
		public bool occupied;//quickly see whether there's somethign there
		public int timesTried;
		
		public Point(int nx, int ny, int nz){//constructor with no parent
			//set the attributes
			x = nx;
			y = ny;
			z = nz;
			parent = null;

			//check if its occupied
			occupied = MapMaker.isOccupied(this);//use the image to check as this is a TON faster
			//checkOccupied();
		}
		
		public Point(int nx, int ny, int nz, Point nparent){//constructor with a parent
			//set the attributes
			x = nx;
			y = ny;
			z = nz;
			parent = nparent;

			//check if its occupied
			occupied = MapMaker.isOccupied(this);//use the image to check as this is a TON faster
			//checkOccupied();
		}
		
		public void checkOccupied(){//a function to use colliders to see if this point is occupied
			Collider[] stuff = Physics.OverlapSphere((new Vector3((float) x, (float) y, (float) z)), 0.5f);
			if (stuff.Length > 0){
				for (int i = 0; i < stuff.Length; i++){
					if (stuff[i].tag != "PathableThrough" && stuff[i].tag != "Background"){
						occupied = true;
						//Debug.Log(stuff[0].gameObject.name);
						return;
					}
				}
			} else {
				occupied = false;
			}
		}
		
		public int getFCost(Point endPoint){//get the total cost when pathing
			int fCost = 0;
			fCost = getMovementCost() + getHeuristicCost(endPoint);
			return fCost;
		}
		
		public int getMovementCost(){//get the cost to move - diagonal is a slightly higher cost
			int cost = 0;
			
			if (parent == null){
				return cost;	
			} else {
				cost += parent.getMovementCost();
				
				if (parent.x != x){
					if (parent.y != y){
						if (parent.z != z){
							cost += 17;
						} else {
							cost += 14;
						}
					} else {
						if (parent.z != z){
							cost += 14;
						} else {
							cost += 10;
						}
					}
				} else {
					if (parent.y != y){
						if (parent.z != z){
							cost += 14;
						} else {
							cost += 10;
						}
					} else {
						if (parent.z != z){
							cost += 10;
						} else {
							cost += 10;
						}
					}
				}
			}
			
			return cost;
		}
		
		public int getHeuristicCost(Point endPoint){//get the heuristic cost. This tells it whether it's going in the right general direction
			//use the Manhattan method - is this point closer to the end or not?

			int cost = 0;
			
			int distanceX = Mathf.Abs(endPoint.x - this.x);
			int distanceY = Mathf.Abs(endPoint.y - this.y);
			int distanceZ = Mathf.Abs(endPoint.z - this.z);
			
			cost = (distanceX + distanceY + distanceZ) * 10;
			
			return cost;
		}
		
		public bool isEqualTo(Point point){//override to see if it's are equal by comparing the position
			if ((this.x == point.x) && (this.y == point.y) && (this.z == point.z)){
				return true;	
			} else {
				return false;
			}
		}
		
		public Point[] getAdjacentPoints2d(){//get a list of the neighboring points
			List<Point> adjacentPoints = new List<Point>();

			//just list the ones with coordinates +- 1
			for (int nx = -1; nx <= 1; nx ++){
				for (int ny = -1; ny <= 1; ny ++){
					if ((nx != 0) || (ny != 0)){//make sure you don't accidentally select the point itself
						adjacentPoints.Add(new Point(x + nx, y + ny, z, this));
					}
				}
			}
			
			Point[] arr = listToArray(adjacentPoints);
			
			return arr;
		}
		
		public Point[] getAdjacentPoints3d(){//get a list of the neighboring points in 3d space
			List<Point> adjacentPoints = new List<Point>();

			//same process as 2d, but with an extra dimension - coordinates +- 1
			for (int nx = -1; nx <= 1; nx ++){
				for (int ny = -1; ny <= 1; ny ++){
					for(int nz = -1; nz <= 1; nz ++){
						if ((nx != 0) | (ny != 0) | (nz != 0)){
							adjacentPoints.Add(new Point(x + nx, y + ny, z + nz, this));
						}
					}
				}
			}
			
			Point[] arr = listToArray(adjacentPoints);
			
			return arr;
		}
		
		public bool isPartOf(Point[] arr){//quick function to see if an array contains this point
			foreach (Point p in arr){
				if (this.isEqualTo(p)){
					return true;
				}
			}
			return false;
		}

		public int directionTo(Point other){
			// 321
			// 4.0
			// 567
			if (other.x == x){
				if (other.y < y){
					return 6;
				} else if (other.y > y){
					return 2;
				}
			} else if (other.x > x){
				if (other.y == y){
					return 0;
				} else if (other.y < y){
					return 7;
				} else if (other.y > y){
					return 1;
				}
			} else if (other.x < x){
				if (other.y == y){
					return 4;
				} else if (other.y < y){
					return 5;
				} else if (other.y > y){
					return 3;
				}
			}

			return -1;
		}

		public static List<Point> aStarOld(Point startLoc, Point goalLoc){//old function that only sometimes works. DO NOT USE
			List<Point> open = new List<Point>();
			List<Point> attemptingPath = new List<Point>();
			List<Point> triedPoints = new List<Point>();
			
			open.Add(startLoc);
			attemptingPath.Add(startLoc);
			
			Point pointToTry = startLoc;
			Point bestSoFar = startLoc;
			
			while (!pointToTry.isEqualTo(goalLoc)){
				
				bool is3d = false;
				Point[] adjacentpoints;
				
				// Generates points next to current one
				if (is3d){
					adjacentpoints = pointToTry.getAdjacentPoints3d();
				} else {
					adjacentpoints = pointToTry.getAdjacentPoints2d();
				}
				// Finds one with lowest cost, and goes from there
				int triedNum = 0;
				bool okay = true;
				for (int i = 0; i < 8; i++){
					if (adjacentpoints[i].getFCost(goalLoc) < bestSoFar.getFCost(goalLoc) + 1){
						
						foreach(Point p in triedPoints){
							if (p.isEqualTo(adjacentpoints[i])){
								okay = false;
							}
						}
						
						if ((!adjacentpoints[i].occupied)&(okay)){
							bestSoFar = adjacentpoints[i];
						} else {
							triedNum += 1;
						}
						
					}
				}
				
				if (triedNum > 24 || triedPoints.Count > 100){
					attemptingPath = null;
					attemptingPath = new List<Point>();
					Debug.LogError("Cannot find path - too many obstacles");
					return attemptingPath;
				}
				
				if (bestSoFar.isEqualTo(startLoc)){
					//attemptingPath = null;
					//attemptingPath = new List<Point>();
					//Debug.LogError("Cannot find path - invalid start");
					//return attemptingPath;
				} else {
					triedPoints.Add(bestSoFar);
				}
				
				pointToTry = bestSoFar;
				attemptingPath.Add(pointToTry);
				triedPoints.Add(pointToTry);
				
			}
			
			return attemptingPath;
		}
		
		public static List<Point> aStar(Point startLoc, Point goalLoc){//this function gives you the path between one point and another
			//keep track of what you've tried and what you might want to try
			List<Point> open = new List<Point>();
			List<Point> closed = new List<Point>();

			//where do you start from?
			open.Add(startLoc);
			open.AddRange(arrayToList(startLoc.getAdjacentPoints2d()));
			open.Remove(startLoc);
			closed.Add(startLoc);
			
			Point[] neighbors;
			Point bsf;
			int bcsf;
			
			//choose best of the options
			bsf = open[0] ;
			bcsf = bsf.getFCost(goalLoc);
			foreach (Point p in open){
				if (!p.occupied && p.getFCost(goalLoc) < bcsf){
					bsf = p;
					bcsf = p.getFCost(goalLoc);
				}
			}
			open.Remove(bsf);
			closed.Add(bsf);
			
			for (int i = 0; i < 5000; i++){//keep on checking. 5000 checks max
				//add best options neighbors in as well
				neighbors = bsf.getAdjacentPoints2d();
				//look through the neighbors
				foreach (Point p in neighbors){
					if (!p.occupied){//is it's occupied, we don't care
						if (!p.isPartOf(listToArray(closed))){//don't check ones you've already checked
							if (p.isPartOf(listToArray(open))){//if you already might want to check it
								//dinf out whether there's a better way to get to it. 
								//This is good if you want the optimal path, but if you take it out it still gives a decent path
								for (int j = 0; j < open.Count; j++){
									Point po = open[j];
									if (po.isEqualTo(p)){
										if (p.getFCost(goalLoc) < po.getFCost(goalLoc)){
											open.Remove(po);
											open.Add(p);
										}
									}
								}
							} else {//you DO want to check this one
								open.Add(p);
							}
						}
					}
				}

				if (open.Count == 0){
					return buildListFromParents(bsf);
				}

				//choose best of the options
				bsf = open[0];
				bcsf = bsf.getFCost(goalLoc);
				foreach (Point p in open){
					if (!p.occupied && p.getFCost(goalLoc) < bcsf){
						bsf = p;
						bcsf = p.getFCost(goalLoc);
					}
				}
				open.Remove(bsf);
				closed.Add(bsf);
				
				if (bsf.isEqualTo(goalLoc)){//are you done?
					return buildListFromParents(bsf);
				}
			}

			//you have to return something; return what you've got. Maybe it'll be useful
			return buildListFromParents(bsf);
		}

		/*public static Point[] smoothPathBasic(Point[] path){//fast and culls the vast majority of useless points

			if (path.Length <= 2){//you can't cull it any more
				return path;
			}

			int direction = path[0].directionTo(path[1]);//what direction is it going in?

			List<Point> npath = new List<Point>();
			npath.Add(path[0]);//always add the first point

			for (int i = 1; i < path.Length-1; i++){//loop through the points
				int dir = path[i].directionTo(path[i+1]);//check what direction it's going in
				if (dir != direction){//only add it if it's a change in direction
					direction = dir;
					npath.Add(path[i]);
				}
			}
			npath.Add(path[path.Length - 1]);//always add the last point

			//Debug.Log("Went from " + path.Length + " to " + npath.Count + " points");

			return listToArray(npath);
		}*/

		public static Vector3[] smoothPathBasic(Vector3[] path){//fast and culls the vast majority of useless points

			for (int i = 0; i < path.Length; i++){
				path[i] -= new Vector3(0f, 0f, path[i].z);
			}

			if (path.Length <= 2){//you can't cull it any more
				return path;
			}


			Vector3 dir = (path[1] - path[0]).normalized;

			List<Vector3> npath = new List<Vector3>();
			npath.Add(path[0]);//always add the first point
			
			for (int i = 1; i < path.Length-1; i++){//loop through the points
				Vector3 direction = (path[i + 1] - path[i]).normalized;//check what direction it's going in
				if (dir.x != direction.x || dir.y != direction.y || dir.z != direction.z){//only add it if it's a change in direction
					dir = direction;
					npath.Add(path[i]);
				}
			}
			npath.Add(path[path.Length - 1]);//always add the last point

			return npath.ToArray();

			
			/*int direction = vector3ToPoint(path[0]).directionTo(vector3ToPoint(path[1]));//what direction is it going in?
			
			List<Vector3> npath = new List<Vector3>();
			npath.Add(path[0]);//always add the first point
			
			for (int i = 1; i < path.Length-1; i++){//loop through the points
				int dir = vector3ToPoint(path[i]).directionTo(vector3ToPoint(path[i+1]));//check what direction it's going in
				if (dir != direction){//only add it if it's a change in direction
					direction = dir;
					npath.Add(path[i]);
				}
			}
			npath.Add(path[path.Length - 1]);//always add the last point
			
			//Debug.Log("Went from " + path.Length + " to " + npath.Count + " points");
			Vector3[] ar = new Vector3[npath.Count];
			npath.CopyTo(ar);
			return ar;*/
		}

		public static Vector3[] smoothPathMedium(Vector3[] path){
			return pointArrayToVector3Array(smoothPathMedium(vector3ArrayToPointArray(path)));
		}

		public static Point[] smoothPathMedium(Point[] path){//slow but culls most useless points
			if (path.Length <= 2){//you can't cull it any more
				return path;
			}

			//Debug.Log("_ " + path.Length);

			//how many points do you check at a time?
			//for the fastest algorithm, this would be set to the average length of a straight segment
			int chunkSize = 5;

			List<Point> npath = new List<Point>();
			npath.Add(path[0]);//always add the first point

			Point currA = path[0];
			int i = 0;
			int distance = path.Length <= i + chunkSize ? path.Length - 1 - i : chunkSize;
			i += distance;
			
			bool running = true;
			bool forward = false;

			while (running){
				//Debug.Log(i);
				Point currB = path[i];
				if (!simpleCastHit(currA, currB)){//you have a line of sight - move on
					i++;
					distance ++;
					forward = true;
					if (i >= path.Length){//you're at the end of the line
						running = false;
						break;
					}
				} else {
					if (forward){//you came from something ahead that worked
						//add the previous point (the one that DID work)
						//Debug.Log(i);
						i --;
						currB = path[i];
						npath.Add(currB);
						//set this point to be the one you're going from
						currA = currB;
						
						//set the next distance to the default of the chunksize, being sure not to go over the end
						distance = path.Length <= i + chunkSize ? path.Length - 1 - i : chunkSize;
						//set up where at what you're checking
						i += distance;
						
						currB = path[i];
						
						if (distance == 0){//you're done
							running = false;
							break;
						}

					} else {//you didn't, so keep going back
						distance --;//check something one back
						if (distance < 1){//you can't shrink it down any more
							//add this point
							//Debug.Log(i);
							npath.Add(currB);
							//set this point to be the one you're going from
							currA = currB;

							//set the next distance to the default of the chunksize, being sure not to go over the end
							distance = path.Length <= i + chunkSize ? path.Length - 1 - i : chunkSize;
							//set up where at what you're checking
							i += distance;

							currB = path[i];

							if (distance == 0){//you're done
								running = false;
								break;
							}
						} else {//you can shrink it down; do so!
							i--;
						}
					}
					forward = false;
				}
			}

			npath.Add(path[path.Length - 1]);//always add the last point

			//Debug.Log("Went from " + path.Length + " to " + npath.Count + " points");
			
			return listToArray(npath);
		}

		public static Vector3[] simplify(Vector3[] path){
			if (path.Length <= 2){//you can't cull it any more
				return path;
			}
			
			List<Vector3> npath = new List<Vector3>();
			npath.Add(path[0]);

			for (int i = 0; i < path.Length; i++){
				//path[i] = MapMaker.closestUnnocupiedTo(path[i]);
			}
			
			//Vector3 curP = path[0];

			for (int i = 0; i < npath.Count - 2; i++){
				float theta = Mathf.Rad2Deg * (Mathf.Atan2(
					(npath[i+2] - npath[i+1]).y, (npath[i+2] - npath[i+1]).x
					) 
				                               
				                               - Mathf.Atan2(
					(npath[i+1] - npath[i]).y, (npath[i+1] - npath[i]).x
					)
				                               );
				
				theta = Mathf.Abs(theta) > 180f ? ((180f - theta)) % 360f : theta % 360f;

				if (Mathf.Abs(theta) < 30f){
					npath.RemoveAt(i);
				}
			}

			int it = 0;
			while (it < path.Length - 2){
				npath.Add(path[it]);
				for (int j = 0; j < path.Length - it - 1; j++){
					if (Vector3.Distance(path[it-j], path[it+1]) < 2f){
						it++;//skip the next point
					} else {
						break;
					}
				}
				it++;
			}
			
			return npath.ToArray();
		}

		public static Vector3[] smoothPathAdvanced(Vector3[] path){

			Debug.Log(path.Length);
			path = simplify(path);
			Debug.Log(path.Length);
			//Debug.Log(complexCastHit(path[0], path[1]));

			return path;

			if (path.Length <= 2){//you can't cull it any more
				return path;
			}

			List<Vector3> npath = new List<Vector3>();
			npath.Add(path[0]);

			//Vector3 curP = path[0];
			int i = 0;
			float thebeforetime = Time.realtimeSinceStartup;
			while (i < path.Length - 1){
				for (int j = path.Length - 1; j > i; j--){
					if (!complexCastHit(path[i], path[j])){//there's a straight line from here to there 
						npath.Add(path[j]);
						i = j;
						break;
					}
				}
				if (Time.realtimeSinceStartup - thebeforetime > 10f){
					Debug.Log("Elapsed time: " + (Time.realtimeSinceStartup - thebeforetime) + "\nPoint " + i);
					break;
				}
			}

			return npath.ToArray();
		}

		public static bool simpleCastHit(Point a, Point b){
			return simpleCastHit(pointToVector3(a), pointToVector3(b));
		}

		public static bool simpleCastHit(Vector3 a, Vector3 b){

			int x0 = (int)a.x;
			int y0 = (int)a.y;
			int x1 = (int)a.x;
			int y1 = (int)b.y;

			int dx, dy;

			if (x0 == x1 && y0 == y1){//if they're the same point, nothing is in between
				return false;
			} else if (x0 == x1){//it is vertical
				dy = y0 < y1 ? 1 : -1;
				for (int y = y0; y < y1; y+=dy){
					if (MapMaker.isOccupied(x0, y)){
						return true;
					}
				}
				return false;
			} else if (y0 == y1){//it is horizontal
				dx = x0 < x1 ? 1 : -1;
				for (int x = x0; x < x1; x+=dx){
					if (MapMaker.isOccupied(x, y0)){
						return true;
					}
				}
				return false;
			}

			dx = x0 < x1 ? 1 : -1;
			dy = y0 < y1 ? 1 : -1;
			float m = (y1 - y0)/(x1 - x0);
			int widthToCheck = 1;

			if (Mathf.Abs(m) < 1f){//it's closer to horizontal: check y=f(x) +- 1
				for (int x = x0; x < x1; x+=dx){
					int y = (int)(m*x + y0);
					for (int w = -1 * widthToCheck; w <= widthToCheck; w++){
						if (MapMaker.isOccupied(x, y + w)){
							return true;
						}
					}
				}
				return false;
			} else {//it's closer to vertical: check x=f(y) +- 1
				m = 1f/m;
				for (int y = y0; y < y1; y+=dy){
					int x = (int)(m*y + x0);
					for (int w = -1 * widthToCheck; w <= widthToCheck; w++){
						if (MapMaker.isOccupied(x + w, y)){
							return true;
						}
					}
				}
				return false;
			}




			//set up a RayCast
			//Ray r = new Ray(a, b - a);
			//return Physics.Raycast(r);

		}

		public static bool complexCastHit(Point a, Point b){
			return complexCastHit(pointToVector3(a), pointToVector3(b));
		}
		
		public static bool complexCastHit(Vector3 a, Vector3 b){
			Ray r = new Ray(a, b - a);
			return Physics.Raycast(r);
		}
		
		public static Point vector3ToPoint(Vector3 vect){//conversion from a Vector3 to a Point
			return new Point((int)vect.x, (int)vect.y, (int)vect.z);
		}
		
		public static Vector3 pointToVector3(Point p){//conversion from a Point to a Vector3
			return new Vector3(p.x, p.y, p.z);
		}
		
		public static Point[] listToArray(List<Point> arr){//convert an arraylist to an array. Consider updating everything to a List<>
			/*Point[] res = new Point[arr.Count];
			for (int i = 0; i < res.Length; i++){
				res[i] = arr[i] ;
			}
			
			return res;*/
			return arr.ToArray();
		}
		
		public static List<Point> arrayToList(Point[] arr){//convert an array to an array list. Consider using AddRange
			List<Point> res = new List<Point>();
			foreach (Point p in arr){
				res.Add(p);
			}
			return res;
		}
		
		public static List<Point> buildListFromParents(Point p){//what is the series of points that you took to get here?
			List<Point> res = new List<Point>();
			
			Point latest = p;
			res.Add(latest);

			//continually check the parent of that point until you reach the end
			while (latest.parent != null){
				res.Add(latest.parent);
				latest = latest.parent;
			}
			
			res.Reverse();
			
			return res;
		}

		public static Vector3[] pointArrayToVector3Array(Point[] arr){
			Vector3[] vs = new Vector3[arr.Length];
			for (int i = 0; i < vs.Length; i++){
				vs[i] = pointToVector3(arr[i]);
			}
			return vs;
		}

		public static Point[] vector3ArrayToPointArray(Vector3[] arr){
			Point[] vs = new Point[arr.Length];
			for (int i = 0; i < vs.Length; i++){
				vs[i] = vector3ToPoint(arr[i]);
			}
			return vs;
		}

		public static bool checkOccupied(Point p){//a function to use colliders to see if this point is occupied
			Collider[] stuff = Physics.OverlapSphere((new Vector3((float) p.x, (float) p.y, (float) p.z)), 0.5f);
			if (stuff.Length > 0){
				for (int i = 0; i < stuff.Length; i++){
					if (stuff[i].tag != "PathableThrough" && stuff[i].tag != "Background"){
						return true;
					}
				}
			} 

			return false;
		}
		
	}
}
