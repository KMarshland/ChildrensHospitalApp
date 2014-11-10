﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapCameraControl : MonoBehaviour {

	//public static MapCameraControl main;

	MapMaker mapMaker;

	public int pointsPassed;
	public int instructionsPassed;
	public Vector3 direction;
	public float speed;
	float pDistance;
	bool detatched;

	bool birdsEye;
	Vector3 prePos;
	Vector3 prePosM;
	Vector3 postPos;
	Vector3 postPosM;
	Vector3 birdsRotation;
	Vector3 regularRotation;
	Vector3 birdsRotationM;
	Vector3 regularRotationM;

	string[] instructions;
	
	//static List<Vector3> pois;//points of interest
	//static List<string> poiNames;//and their names
	static List<MapLabel> poiMarkers;
	string searchDefault;
	Vector2 dropDownScrollPosFrom = Vector2.zero;//keep track of the scrolling for the drop down menu
	Vector2 dropDownScrollPosTo = Vector2.zero;//keep track of the scrolling for the drop down menu
	int dropDownNFrom;
	int dropDownIFrom;
	int dropDownWhichFrom;
	string dropDownSearchTermFrom;
	List<MapLabel> poiSearchFrom;
	int dropDownNTo;
	int dropDownITo;
	int dropDownWhichTo;
	string dropDownSearchTermTo;
	List<MapLabel> poiSearchTo;

	Transform mainCamera;
	Transform centralCube;

	List<MapLabel> remainingSegments;

	// Use this for initialization
	void Start () {

		//MapCameraControl.main = this;
		mainCamera = this.transform.FindChild("PrimaryRotator").FindChild("Main Camera");
		centralCube = GameObject.Find("CentralCube").transform;//this.transform.FindChild("CentralCube");


		mapMaker = this.gameObject.GetComponent("MapMaker") as MapMaker;

		//regularRotation = new Vector3(0f, 65f, 270f);
		regularRotation = mainCamera.localRotation.eulerAngles;
		regularRotationM = transform.localRotation.eulerAngles;
		birdsRotation = new Vector3(0f, 0f, 0f);
		birdsRotationM = new Vector3(0f, 0f, 0f);

		postPos = new Vector3(126,126,-240);
		postPosM = new Vector3(0,0,0);

		searchDefault = "Search";
		dropDownSearchTermFrom = searchDefault;
		dropDownSearchTermTo = searchDefault;
		//Debug.Log(Screen.width);
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (Input.GetKeyDown(KeyCode.Space)){
			/*if (detatched){
				transform.position = mapMaker.PathPoints[pointsPassed];
				detatched = false;
				speed = 0f;
			}*/
			//speed = (((int)(30f * speed) + 1)%(13))/10f;
			speed = 0.3f;
		}

		//if (!detatched && !birdsEye){
			move();
		//}

		Vector3 transl = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f);
		transform.position += transl + new Vector3(0f, 0f, 25f * Input.GetAxis("Mouse ScrollWheel"));
		
		/*if (Mathf.Abs(Input.GetAxis("Vertical")) > 0f || Mathf.Abs(Input.GetAxis("Horizontal")) > 0f){
			if (!birdsEye){
				detatched = true;
				
				//transform.position += new Vector3(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), 0f);
				transform.position += (transform.right * Input.GetAxis("Vertical")) - (transform.up * Input.GetAxis("Horizontal"));
			}
		}

		if (birdsEye){
			Vector3 transl = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f);
			mainCamera.localPosition += transl + new Vector3(0f, 0f, 25f * Input.GetAxis("Mouse ScrollWheel"));
			//transform.position += transl;
			//prePosM += transl;
		}
		
		if (Input.GetKeyDown(KeyCode.Escape)){
			if (birdsEye){
				transform.position = mapMaker.PathPoints[pointsPassed];
				detatched = false;

				mainCamera.localPosition = prePos;
				transform.position = prePosM;

				mainCamera.localRotation = Quaternion.Euler(regularRotation);
				transform.localRotation = Quaternion.Euler(regularRotationM);

				transform.FindChild("CentralCube").localScale = new Vector3(1f,1f,1f);
				centralCube.position += new Vector3(0f,0f,3) - prePosM;

				GameObject.Find("PlaneTransparent").renderer.enabled = false;
				birdsEye = false;
			} else {
				regularRotation = mainCamera.localRotation.eulerAngles;
				regularRotationM = transform.localRotation.eulerAngles;

				prePos = mainCamera.localPosition;
				prePosM = transform.position;

				mainCamera.localPosition = postPos;
				transform.position = postPosM;

				mainCamera.localRotation = Quaternion.Euler(birdsRotation);
				transform.localRotation = Quaternion.Euler(birdsRotationM);

				transform.FindChild("CentralCube").localScale = new Vector3(2f,2f,2f);
				centralCube.position += new Vector3(0f,0f,-3) + prePosM;

				GameObject.Find("PlaneTransparent").renderer.enabled = true;
				birdsEye = true;
			}
		}*/
	}

	void OnGUI(){
		if (!birdsEye && !detatched){
			instructionsGUI();
		}
		poiGUI();
	}
	
	void instructionsGUI(){
		if (instructions == null || instructions.Length == 0){
			return;
		}

		float theta = 0;

		if (pointsPassed + 2 < mapMaker.PathPoints.Length - 1){
			theta = Mathf.Rad2Deg * (Mathf.Atan2(
				(mapMaker.PathPoints[pointsPassed+2] - mapMaker.PathPoints[pointsPassed+1]).y, 
				(mapMaker.PathPoints[pointsPassed+2] - mapMaker.PathPoints[pointsPassed+1]).x
				) 
			                         
			                         - Mathf.Atan2(
				(mapMaker.PathPoints[pointsPassed+1] - mapMaker.PathPoints[pointsPassed]).y, 
				(mapMaker.PathPoints[pointsPassed+1] - mapMaker.PathPoints[pointsPassed]).x
				)
			);
		}
		
		theta = Mathf.Abs(theta) > 180 ? ((180f - theta)) % 360f : theta % 360f;
		
		string resi = pointsPassed >= mapMaker.PathPoints.Length - 1 ? "" : 
			"Go forward " + 
			Vector3.Distance(centralCube.position, mapMaker.PathPoints[pointsPassed+1]).ToString("F1") + 
			" units to the point (" + 
				mapMaker.PathPoints[pointsPassed+1].x + "," + 
				mapMaker.PathPoints[pointsPassed+1].y + "), " +
				"then turn " + /*(int)(Mathf.Abs(theta)) + " degrees to the " +*/ 
				(theta < 0 ? "right" : "left") + 
				".";

		GUI.Box(new Rect(5, 5, Screen.width - 10, 30), resi);
	}
	
	void poiGUI(){
		if (poiMarkers == null || poiMarkers.Count == 0){
			//Debug.Log("Oh noes! We ran out of markers! Check back next Tuesday for a refreshing mark. ");
			return;
		}

		GUI.Box(new Rect(5, 40, 250, 200), "Directions");
		
		int preFrom = dropDownWhichFrom;
		int preTo = dropDownWhichTo;

		

		//select where you're going from
		GUI.Label(new Rect(10, 70, 50, 25), "From: ");
		
		if (dropDownNFrom == 0){
			if (GUI.Button(new Rect(60, 70, 100, 25), poiMarkers[dropDownWhichFrom].Label)){
				
				if(dropDownNFrom==0){
					dropDownNFrom = 1;
					//GUI.SetNextControlName("drop_down_search_from");
				} else {
					dropDownNFrom=0;   
					//GUI.SetNextControlName("drop_down_search_to");
				}
				
			}
		}
		
		
		
		if (dropDownNFrom == 1 && dropDownNTo != 1){

			poiSearchFrom = dropDownSearchTermFrom == searchDefault ? poiMarkers : new List<MapLabel>();
			if (dropDownSearchTermFrom != searchDefault){
				if (dropDownSearchTermFrom == ""){
					poiSearchFrom = poiMarkers;
				} else {
					foreach (MapLabel ml in poiMarkers){
						if (ml.termApplies(dropDownSearchTermFrom)){
							poiSearchFrom.Add(ml);
						}
					}
				}
			}

			dropDownScrollPosFrom = GUI.BeginScrollView(
				new Rect (60, 70, ((26 * poiSearchFrom.Count + 26) > 115) ? 170 : 155, 115), dropDownScrollPosFrom, 
				new Rect (0, 0, 150, 26 * poiSearchFrom.Count + 26)
			);
			
			GUI.Box(new Rect(0,0,300,26 * poiSearchFrom.Count+26), "");  

			GUI.SetNextControlName ("drop_down_search_from");
			dropDownSearchTermFrom = GUI.TextField(new Rect(2,0,150,25), dropDownSearchTermFrom);
			if (UnityEngine.Event.current.type == EventType.Repaint){

				if (GUI.GetNameOfFocusedControl () == "drop_down_search_from"){
					if (dropDownSearchTermFrom == searchDefault){
						dropDownSearchTermFrom = "";
					}
				} else {
					if (dropDownSearchTermFrom == ""){
						dropDownSearchTermFrom = searchDefault;
					}
				}
			}

			for(dropDownIFrom=0; dropDownIFrom<poiSearchFrom.Count; dropDownIFrom++){
				
				if(GUI.Button(new Rect(2,dropDownIFrom*26 + 26,150,25), poiSearchFrom[dropDownIFrom].Label)){
					dropDownNFrom=0;
					dropDownWhichFrom=poiSearchFrom[dropDownIFrom].Id;         
					
				}               
				
				//GUI.Label(new Rect(5,dropDownI*28,130,25), poiNames[dropDownI]);            
				
			}
			
			GUI.EndScrollView();         
			
		}
		
		//select where you're going to
		GUI.Label(new Rect(10, 110, 50, 25), "To: ");
		
		if (dropDownNTo == 0 && dropDownNFrom != 1){
			if(GUI.Button(new Rect(60, 110, 100, 25), poiMarkers[dropDownWhichTo].Label)){
				
				if (dropDownNTo == 0){
					dropDownNTo = 1;
				} else {
					dropDownNTo=0;   
				}
				
			}
		}
		
		
		
		if (dropDownNTo == 1 && dropDownNFrom != 1){

			poiSearchTo = dropDownSearchTermTo == searchDefault ? poiMarkers : new List<MapLabel>();
			if (dropDownSearchTermTo != searchDefault){
				if (dropDownSearchTermTo == ""){
					poiSearchTo = poiMarkers;
				} else {
					foreach (MapLabel ml in poiMarkers){
						if (ml.termApplies(dropDownSearchTermTo)){
							poiSearchTo.Add(ml);
						}
					}
				}
			}
			
			dropDownScrollPosTo = GUI.BeginScrollView(
				new Rect (60, 110, ((26 * poiSearchTo.Count+26) > 115) ? 170 : 155, 115), dropDownScrollPosTo, 
				new Rect (0, 0, 150, 26 * poiSearchTo.Count+26)
				);
			
			GUI.Box(new Rect(0,0,300,26 * poiSearchTo.Count+26), "");  

			GUI.SetNextControlName("drop_down_search_to");
			dropDownSearchTermTo = GUI.TextField(new Rect(2,0,150,25), dropDownSearchTermTo);
			if (UnityEngine.Event.current.type == EventType.Repaint){
				if (GUI.GetNameOfFocusedControl() == "drop_down_search_to"){
					if (dropDownSearchTermTo == searchDefault){
						dropDownSearchTermTo = "";
					}
				} else {
					if (dropDownSearchTermTo == ""){
						dropDownSearchTermTo = searchDefault;
					}
				}
			}

			for(dropDownITo=0;dropDownITo<poiSearchTo.Count;dropDownITo++){
				
				if(GUI.Button(new Rect(2,dropDownITo*26 + 26,150,25), poiSearchTo[dropDownITo].Label)){
					dropDownNTo=0;
					dropDownWhichTo=poiSearchTo[dropDownITo].Id;         
					
				}
				
			}
			
			GUI.EndScrollView();         
			
		}
		
		//see if you need to do something else
		if (preFrom != dropDownWhichFrom || preTo != dropDownWhichTo && dropDownWhichFrom != dropDownWhichTo){
			resetPath(navigateFrom(poiMarkers[dropDownWhichFrom], poiMarkers[dropDownWhichTo]));
		}
	}

	List<MapLabel> navigateFrom(MapLabel a, MapLabel b){
		List<MapLabel> res = new List<MapLabel>();

		res.Add(a);

		if (a.OnFloor == b.OnFloor){
			res.Add(b);
		} else {
			//find out what elevators serve the starting floor
			List<MapLabel> availableElevators = MapLabel.ElevatorsOnFloor(a.OnFloor);

			bool direct = false;
			foreach (MapLabel m in availableElevators){
				if (m.IsElevator && m.isOnFloor(b.OnFloor)){
					res.Add(m);
					res.Add(b);
					direct = true;
					break;
				}
			}

			if (!direct){
				MapLabel goalLoc = b;

				List<MapLabel> open = availableElevators;
				List<MapLabel> closed = new List<MapLabel>();

				List<MapLabel> neighbors;
				MapLabel bsf;
				int bcsf;
				
				//choose best of the options
				bsf = open[0];
				bcsf = bsf.getFCost(goalLoc);
				foreach (MapLabel p in open){
					if (p.getFCost(goalLoc) < bcsf){
						bsf = p;
						bcsf = p.getFCost(goalLoc);
					}
				}
				open.Remove(bsf);
				closed.Add(bsf);
				
				for (int i = 0; i < 5000; i++){//keep on checking. 5000 checks max

					neighbors = MapLabel.ElevatorsOnFloor(bsf.OnFloor);
					foreach (int flo in bsf.ElevatorFloors){
						List<MapLabel> ex = MapLabel.ElevatorsOnFloor(flo);
						foreach (MapLabel m in ex){
							if (!neighbors.Contains(m)){
								neighbors.Add(m);
							}
						}
					}

					//look through the neighbors
					foreach (MapLabel p in neighbors){
						if (!closed.Contains(p)){//don't check ones you've already checked
							if (!open.Contains(p)){//if you don't already might want to check it
								open.Add(p);
								p.parentalLabel = bsf;
							}
						}
					}
					
					if (open.Count == 0){
						break;
					}
					
					//choose best of the options
					bsf = open[0];
					bcsf = 1; //bsf.getFCost(goalLoc);
					foreach (MapLabel p in open){
						if (p.getFCost(goalLoc) < bcsf){
							bsf = p;
							bcsf = p.getFCost(goalLoc);
						}
					}
					open.Remove(bsf);
					closed.Add(bsf);
					
					if (bsf.OnFloor == goalLoc.OnFloor){//are you done?
						List<MapLabel> reverseElevatorRoute = new List<MapLabel>();
						while (bsf != null){
							reverseElevatorRoute.Add(bsf);
							bsf = bsf.parentalLabel;
						}
						reverseElevatorRoute.Reverse();
						res.AddRange(reverseElevatorRoute);
						res.Add(goalLoc);
					}
				}
			}
		}

		if (true){
			string deb = "Calculated segments: \n";
			foreach (MapLabel m in res){
				deb += m.Label + "\n";
			}
			Debug.Log(deb);
		}

		return res;
	}
	
	void resetPath(List<MapLabel> segments){//totally recalculates path and instructions and such

		if (segments.Count < 2){
			return;
		}

		if (!segments[0].IsElevator){//if you're starting from something other than an elevator, go to its floor
			MapMaker.ActiveFloor = MapMaker.floors[segments[0].OnFloor];
		} else if (!segments[1].IsElevator){//however, if the place you're coming flor is an elevator...
			MapMaker.ActiveFloor = MapMaker.floors[segments[1].OnFloor];//just go to the floor you were planning on
		} else {//they're both elevators. Go to whatever floor they're both on
			//TODO find this out
		}
		
		mapMaker.StartPos = segments[0].PathLocation;
		mapMaker.EndPos = segments[1].PathLocation;

		segments[1].highlight();

		remainingSegments = segments.GetRange(1, segments.Count - 1);

		mapMaker.genPath();//generate the path
	}
	
	public void recalculateFromPath(){
		genInstructions();
		
		pointsPassed = 0;
		instructionsPassed = 0;
		pDistance = 10000f;

		direction = (mapMaker.PathPoints[pointsPassed + 1] - mapMaker.PathPoints[pointsPassed]).normalized;
		centralCube.position = mapMaker.PathPoints[0];

		moveToView(mapMaker.PathPoints[0], mapMaker.PathPoints[1]);
	}

	string[] genInstructions(){
		List<string> res = new List<string>();
		
		//res.Add("Go forward " + Vector3.Distance(mapMaker.Path[0], mapMaker.Path[1]) + 
		//        " units to the point (" + mapMaker.Path[1].x + "," + mapMaker.Path[1].y + "). ");
		
		for (int i = 0; i < mapMaker.PathPoints.Length - 2; i++){
			float theta = Mathf.Rad2Deg * (Mathf.Atan2(
				(mapMaker.PathPoints[i+2] - mapMaker.PathPoints[i+1]).y, (mapMaker.PathPoints[i+2] - mapMaker.PathPoints[i+1]).x
				) 
			                               
			                               - Mathf.Atan2(
				(mapMaker.PathPoints[i+1] - mapMaker.PathPoints[i]).y, (mapMaker.PathPoints[i+1] - mapMaker.PathPoints[i]).x
				)
			                               );
			
			theta = Mathf.Abs(theta) > 180 ? ((180f - theta)) % 360f : theta % 360f;
			
			string resi = "Go forward " + Vector3.Distance(mapMaker.PathPoints[i], mapMaker.PathPoints[i+1]) + 
				" units to the point (" + mapMaker.PathPoints[i+1].x + "," + mapMaker.PathPoints[i+1].y + "), " +
					"then turn " + /*(int)(Mathf.Abs(theta)) + " degrees to the " +*/ (theta < 0 ? "right" : "left") + 
					".";
			
			res.Add(resi);
		}
		
		res.Add("Go forward " + Vector3.Distance(mapMaker.PathPoints[mapMaker.PathPoints.Length-2], mapMaker.PathPoints[mapMaker.PathPoints.Length-1]) + 
		        " units to the point (" + mapMaker.PathPoints[mapMaker.PathPoints.Length-1].x + "," + mapMaker.PathPoints[mapMaker.PathPoints.Length-2].y + "). ");
		res.Add("The end!"); 
		
		instructions = res.ToArray();
		return instructions;
	}

	void move(){

		if (mapMaker.LinePoints == null || mapMaker.LinePoints.Length < 2){
			return;
		}

		if (pointsPassed >= mapMaker.PathPoints.Length - 1){
			finishSegment();
			return;
		}

		if (direction == null || Vector3.Distance(centralCube.position, mapMaker.LinePoints[instructionsPassed+1]) < 2f * speed){
			instructionsPassed = (instructionsPassed + 1) % (mapMaker.LinePoints.Length - 1);
			direction = (mapMaker.LinePoints[instructionsPassed + 1] - mapMaker.LinePoints[instructionsPassed]).normalized;
			centralCube.position = mapMaker.LinePoints[instructionsPassed];
			//Debug.Log(instructions[pointsPassed]);
		}

		float distance = Vector3.Distance(centralCube.position, mapMaker.PathPoints[pointsPassed+1]);

		if (direction == null || distance < speed || pDistance < distance){
			distance = 10000f;
			finishStretch();
		}

		pDistance = distance;

		//direction = new Vector3(1f, 0f, 0f);
		//transform.LookAt(transform.position + direction);
		//transform.Rotate(new Vector3(0f, -90f, 0f));
		
		//Quaternion toRotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x)));
		//transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, 0.01f * Time.time);

		//Debug.Log((direction * speed) + " bork bork " + Time.realtimeSinceStartup);

		centralCube.position += direction * speed;

	}

	void finishStretch(){
		pointsPassed ++;// = (pointsPassed + 1) % (mapMaker.PathPoints.Length - 1);
		speed = 0f;

		//Debug.Log(instructions[pointsPassed]);

		Vector3 a = mapMaker.PathPoints[pointsPassed];
		Vector3 b = pointsPassed >= mapMaker.PathPoints.Length - 1 ? a : mapMaker.PathPoints[pointsPassed + 1];

		if (pointsPassed >= mapMaker.PathPoints.Length - 1){
			finishSegment();
		}

		moveToView(a, b);
	}

	void moveToView(Vector3 a, Vector3 b){
		Vector2 center = new Vector2((a.x + b.x)/2f, (a.y + b.y)/2f);
		float maxDistance = Vector3.Distance(a, b) + 15;

		bool usingHeight = Mathf.Abs(a.x - b.x)/Mathf.Abs(a.y - b.y) > ((0f + Screen.width) / (0f + Screen.height));

		float scaleFactor = -0.75f * (usingHeight ? Screen.height : Screen.width) / 976f;

		this.transform.position = new Vector3(center.x, center.y, maxDistance * scaleFactor + 1f);
	}
	
	void finishSegment(){
		speed = 0f;
		pointsPassed = mapMaker.PathPoints.Length - 1;
		centralCube.position = mapMaker.PathPoints[pointsPassed];
		if (remainingSegments.Count > 1){
			Debug.Log("Reseting path");
			resetPath(remainingSegments);
		}


	}

	public static int addLabel(MapLabel ml){
		if (poiMarkers == null){
			poiMarkers = new List<MapLabel>();
		}

		//pois.Add(ml.PathLocation);
		//poiNames.Add(ml.Label);
		poiMarkers.Add(ml);

		return poiMarkers.Count - 1;
	}

}
