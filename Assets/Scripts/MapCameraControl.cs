using UnityEngine;
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

	//bool birdsEye;
	/*Vector3 prePos;
	Vector3 prePosM;
	Vector3 postPos;
	Vector3 postPosM;
	Vector3 birdsRotation;
	Vector3 regularRotation;
	Vector3 birdsRotationM;
	Vector3 regularRotationM;*/

	bool movementEnabled;

	string[] instructions;
	
	//static List<Vector3> pois;//points of interest
	//static List<string> poiNames;//and their names
	static List<MapLabel> poiMarkers = new List<MapLabel>();
	MapLabel poiFrom;
	MapLabel poiTo;

	//Transform mainCamera;
	Transform centralCube;

	List<MapLabel> remainingSegments;
	
	public enum UIName{
		Map,
		Landing,
		WhereFrom,
		WhereTo,
		Confirm
	};

	UIName activeScreen;
	Dictionary<string, PowerUI.EventHandler> trackedEvents = new Dictionary<string, PowerUI.EventHandler>();

	ElasticConnection elasticConnection;

	// Use this for initialization
	void Start () {

		//MapCameraControl.main = this;
		//mainCamera = this.transform.FindChild("PrimaryRotator").FindChild("Main Camera");
		centralCube = GameObject.Find("CentralCube").transform;//this.transform.FindChild("CentralCube");


		mapMaker = this.gameObject.GetComponent("MapMaker") as MapMaker;
		elasticConnection = this.gameObject.GetComponent<ElasticConnection>();

		//regularRotation = new Vector3(0f, 65f, 270f);
		/*regularRotation = mainCamera.localRotation.eulerAngles;
		regularRotationM = transform.localRotation.eulerAngles;
		birdsRotation = new Vector3(0f, 0f, 0f);
		birdsRotationM = new Vector3(0f, 0f, 0f);

		postPos = new Vector3(126,126,-240);
		postPosM = new Vector3(0,0,0);*/

		activeScreen = UIName.Landing;
		initUI();
		MapMaker.ActiveFloor = MapMaker.floors[0];
	}

	// Update is called once per frame
	void FixedUpdate () {

		if (Input.GetKeyDown(KeyCode.Escape)){
			speed = 0f;
			ActiveScreen = UIName.Landing;
		}

		if (movementInitiated()){
			//nextStep();
		}

		move();

		Vector3 transl = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f) + 
			new Vector3(0f, 0f, 25f * Input.GetAxis("Mouse ScrollWheel"));
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved){
			var delts = Input.GetTouch(0).deltaPosition;
			transl -= new Vector3(delts.x, delts.y, 0f)*-0.0005f*transform.position.z;

			if (Input.touchCount == 2){
				// Store both touches.
				Touch touchZero = Input.GetTouch(0);
				Touch touchOne = Input.GetTouch(1);
				
				// Find the position in the previous frame of each touch.
				Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
				Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
				
				// Find the magnitude of the vector (the distance) between the touches in each frame.
				float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
				float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
				
				// Find the difference in the distances between each frame.
				float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
				
				transl -= new Vector3(0f, 0f, deltaMagnitudeDiff*-0.0005f*transform.position.z);
			}
		}

		if (transform.position.z + transl.z > -0.5f){//don't go down too far
			transl += new Vector3(0f, 0f, -0.5f - transform.position.z);
		}

		if (movementEnabled){
			transform.position += transl;
			elasticConnection.InitialMovementRelation += transl;
		}

		//show/hide the refresh button
		//Debug.Log(MapLabel.LabelCount);
		if (MapLabel.LabelCount == 0){
			PowerUI.UI.document.getElementById("refreshButton").className = "button refreshButton landingButton";
		} else {
			PowerUI.UI.document.getElementById("refreshButton").className = "button landingButton hidden";
		}

		if (ActiveScreen == UIName.WhereFrom || ActiveScreen == UIName.WhereTo){//implement a scroll-by-swipe manually
			if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved){
				string elId = ActiveScreen == UIName.WhereFrom ? "fromSearchResults" : "toSearchResults";
				PowerUI.Element el = PowerUI.UI.document.getElementById(elId);
				var touch = Input.GetTouch(0);

				Vector2 scrollAmount = touch.deltaPosition;
				//(Input.GetTouch(0).position);
				Vector2 point = touch.position - scrollAmount;
				if (el.Style.Computed.Contains((int)point.x,(int)point.y)){
					scrollAmount *= 5;
					el.scrollBy(0, (int)scrollAmount.y);
				}
			}
		}

		//Debug.Log(PowerUI.UI.document.getElementById("refreshButton").className);
	}

	bool movementInitiated(){
		return Input.GetKeyDown(KeyCode.Space) || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended;
	}

	void OnGUI(){
		//poiGUI();

		if (ActiveScreen == UIName.Map){
			instructionsGUI();
		}
	}

	void initUI(){
		
		UIName startScreen = activeScreen;
		
		//hide everything except for what you're starting on
		System.Array all = System.Enum.GetValues(typeof(UIName));
		foreach (UIName u in all){
			if (u != startScreen){
				hideScreen((UIName)u);
			}
		}

		//set up events for the map screen
		trackedEvents["startDirections"] = delegate(PowerUI.UIEvent mouseEvent){
			resetGUI();
			ActiveScreen = UIName.WhereFrom;
		};


		PowerUI.UI.document.getElementById("getDirectionsButton").OnClick += trackedEvents["startDirections"];

		PowerUI.UI.document.getElementById("backToLandingButton").OnClick += delegate(PowerUI.UIEvent mouseEvent){
			ActiveScreen = UIName.Landing;
		};

		var floorSelectDiv = PowerUI.UI.document.getElementById("floorSelectDiv");
		for (int i = 0; i < MapMaker.floors.Length; i++){
			PowerUI.Element nEl = new PowerUI.Element("div");
			nEl.className = "button floorSelectButton";
			nEl.textContent = i == 0 ? "Ground floor" : "Floor " + i;
			var k = i;//we don't want a reference to the old variable gunking up the works
			nEl.OnClick += delegate(PowerUI.UIEvent mouseEvent){
				floorSelectDiv.childNodes[MapMaker.ActiveFloor != null ? MapMaker.ActiveFloor.Id-1 : 0].className = "button floorSelectButton";
				MapMaker.ActiveFloor = MapMaker.floors[k];
				nEl.className = "button floorSelectButton floorSelectButtonHighlighted";
			};
			floorSelectDiv.AppendNewChild(nEl);
		}
		floorSelectDiv.childNodes[MapMaker.ActiveFloor != null ? MapMaker.ActiveFloor.Id-1 : 0].className = "button floorSelectButton floorSelectButtonHighlighted";

		//set up events for the landing screen
		PowerUI.UI.document.getElementById("browseButton").OnClick += delegate(PowerUI.UIEvent mouseEvent){
			ActiveScreen = UIName.Map;
			startBrowse();
		};
		PowerUI.UI.Variables["refreshDisplayType"] = "block";

		PowerUI.UI.document.getElementById("refreshButton").OnClick += delegate(PowerUI.UIEvent mouseEvent){
			MapMaker.instance.startMarkerUpdate();
		};
		
		PowerUI.UI.document.getElementById("startButton").OnClick += trackedEvents["startDirections"];
		
		//set up events for the where from? page
		PowerUI.UI.document.getElementById("backToStartButton").OnClick += delegate(PowerUI.UIEvent mouseEvent){
			ActiveScreen = UIName.Landing;
		};
		
		PowerUI.UI.document.getElementById("fromLobbyButton").OnClick += delegate(PowerUI.UIEvent mouseEvent){
			poiFrom = (MapLabel.MapLabels != null &&  MapLabel.MapLabels.ContainsKey(103)) ? MapLabel.MapLabels[103] : poiMarkers[0];
			ActiveScreen = UIName.WhereTo;
			PowerUI.UI.Variables["fromLabel"] = poiFrom.Label;
		};
		
		trackedEvents["fromListener"] = delegate(PowerUI.UIEvent keyEvent){
			//Debug.Log(keyEvent.keyCode);
			string term = PowerUI.UI.document.getElementById("fromSearch").value;
			if (term == null){
				term = "";
			}
			
			
			List<MapLabel> search = MapLabel.Search(poiMarkers, term);

			var resultDiv = PowerUI.UI.document.getElementById("fromSearchResults");
			resultDiv.innerHTML = "";

			foreach(MapLabel m in search){
				MapLabel ml = m;
				PowerUI.Element nEl = new PowerUI.Element("div");
				nEl.className = "button searchResult";
				nEl.textContent = ml.Label;
				nEl.style.width = (PowerUI.UI.document.getElementById("fromSearch").pixelWidth - 50) + "px";
				nEl.OnClick += delegate(PowerUI.UIEvent mouseEvent){
					poiFrom = ml;
					ActiveScreen = UIName.WhereTo;
					PowerUI.UI.Variables["fromLabel"] = ml.Label;
				};
				resultDiv.AppendNewChild(nEl);
			}
		};
		PowerUI.UI.document.getElementById("fromSearch").OnKeyUp += trackedEvents["fromListener"];
		
		//set up events for the where to? page
		PowerUI.UI.document.getElementById("backToFromButton").OnClick += delegate(PowerUI.UIEvent mouseEvent){
			ActiveScreen = UIName.WhereFrom;
		};
		
		trackedEvents["toListener"] = delegate(PowerUI.UIEvent keyEvent){
			//Debug.Log(keyEvent.keyCode);
			string term = PowerUI.UI.document.getElementById("toSearch").value;
			if (term == null){
				term = "";
			}
			
			
			List<MapLabel> search = MapLabel.Search(poiMarkers, term);
			
			var resultDiv = PowerUI.UI.document.getElementById("toSearchResults");
			resultDiv.innerHTML = "";
			
			foreach(MapLabel m in search){
				MapLabel ml = m;
				PowerUI.Element nEl = new PowerUI.Element("div");
				nEl.className = "button searchResult";
				nEl.textContent = ml.Label;
				nEl.style.width = (PowerUI.UI.document.getElementById("toSearch").pixelWidth - 50) + "px";
				nEl.OnClick += delegate(PowerUI.UIEvent mouseEvent){
					poiTo = ml;
					ActiveScreen = UIName.Confirm;
					PowerUI.UI.Variables["toLabel"] = ml.Label;
				};
				resultDiv.AppendNewChild(nEl);
			}
		};
		PowerUI.UI.document.getElementById("toSearch").OnKeyUp += trackedEvents["toListener"];
		
		//set up events for the confirm page
		PowerUI.UI.document.getElementById("backToToButton").OnClick += delegate(PowerUI.UIEvent mouseEvent){
			ActiveScreen = UIName.WhereTo;
		};
		
		PowerUI.UI.document.getElementById("goButton").OnClick += delegate(PowerUI.UIEvent mouseEvent){
			ActiveScreen = UIName.Map;
			resetGUI();
			startNavigation();
		};

		foreach (var el in PowerUI.UI.document.getElementsByClassName("searchResults")){
			int delta = el.id == "fromSearchResults" ? 375 : 275;
			el.style.height = (Screen.height - delta) + "px";
			el.style.marginLeft = (int)(Screen.width/2f - 300) + "px";
		}
	}
	
	void resetGUI(){
		var reses = PowerUI.UI.document.getElementsByClassName("searchResults");
		foreach (var r in reses){
			r.innerHTML = "";
		}
		
		var inps = PowerUI.UI.document.getElementsByClassName("searchBoxInput");
		foreach (var i in inps){
			i.value = "";
		}
		//Debug.Log("1");
		trackedEvents["fromListener"](null);
		trackedEvents["toListener"](null);
	}
	
	public UIName ActiveScreen {
		get {
			return activeScreen;
		} set {
			activeScreen = value;
			showScreen(activeScreen);
			System.Array all = System.Enum.GetValues(typeof(UIName));
			foreach (UIName u in all){
				if (u != activeScreen){
					hideScreen((UIName)u);
				}
			}
		}
	}

	void showScreen(UIName id){
		PowerUI.UI.document.getElementById(id.ToString()).style.display = "block";
	}

	void hideScreen(UIName id){
		PowerUI.UI.document.getElementById(id.ToString()).style.display = "none";
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

		string distanceStr = "";
		string coordsStr = "";
		//string turnStr = "";
		string thenStr = "";
		string finishedStr = "You have reached your destination";

		if (pointsPassed < mapMaker.PathPoints.Length - 1){
			distanceStr = Vector3.Distance(centralCube.position, mapMaker.PathPoints[pointsPassed+1]).ToString("F1");
			coordsStr = "(" + 
				mapMaker.PathPoints[pointsPassed+1].x.ToString("F0") + "," + 
				mapMaker.PathPoints[pointsPassed+1].y.ToString("F0") + ")";

			if (theta > 0.1f){
				thenStr = ", then turn " + theta + " degrees " + (theta < 0 ? "right" : "left");
			} else {
				thenStr = "";
			}
		}

		if (remainingSegments.Count > 1){
			finishedStr = "Take the elevator to floor " + remainingSegments[1].OnFloor;
		}

		string resi = pointsPassed >= mapMaker.PathPoints.Length - 1 ? finishedStr : 
			"Go forward " + 
			distanceStr + 
			" units to the point " +
			coordsStr + "" +
			thenStr + 
		".";

		GUI.Box(new Rect(5, 5, Screen.width - 10, 30), resi);
	}

	void startBrowse(){
		PowerUI.UI.Variables["getDirectionsLabel"] = "Get Directions";
		PowerUI.UI.Variables["mapBackLabel"] = "Back";
		PowerUI.UI.document.getElementById("stepsButtonDiv").innerHTML = "";
		this.transform.rotation = Quaternion.identity;//unrotate
		elasticConnection.ResetRotationRelation(new Vector3(0, 0, centralCube.rotation.eulerAngles.x + 90f));
		movementEnabled = true;
	}

	void startNavigation(){
		PowerUI.UI.Variables["getDirectionsLabel"] = "Change destination";
		PowerUI.UI.Variables["mapBackLabel"] = "Home";
		PowerUI.UI.document.getElementById("stepsButtonDiv").innerHTML = PowerUI.UI.document.getElementById("stepsButtonDivTemplate").innerHTML;

		foreach (var el in PowerUI.UI.document.getElementsByClassName("nextStep")){
			el.OnClick += delegate(PowerUI.UIEvent mouseEvent){
				nextStep();
			};
		}

		foreach (var el in PowerUI.UI.document.getElementsByClassName("previousStep")){
			el.OnClick += delegate(PowerUI.UIEvent mouseEvent){
				previousStep();
			};
		}

		movementEnabled = false;

		resetPath(navigateFrom(poiFrom, poiTo));
	}

	void nextStep(){
		speed = 0.3f;
	}

	void previousStep(){
		speed = 0f;
		pointsPassed--;
		pointsPassed = pointsPassed < 0 ? 0 : pointsPassed;
		direction = (mapMaker.PathPoints[pointsPassed + 1] - mapMaker.PathPoints[pointsPassed]).normalized;
		centralCube.position = mapMaker.PathPoints[pointsPassed];
		
		moveToView(mapMaker.PathPoints[pointsPassed], mapMaker.PathPoints[pointsPassed + 1]);
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

		if (Vector3.Distance(centralCube.position, mapMaker.LinePoints[instructionsPassed+1]) < 2f * speed){
			instructionsPassed = (instructionsPassed + 1) % (mapMaker.LinePoints.Length - 1);
			direction = (mapMaker.LinePoints[instructionsPassed + 1] - mapMaker.LinePoints[instructionsPassed]).normalized;
			centralCube.position = mapMaker.LinePoints[instructionsPassed];
			//Debug.Log(instructions[pointsPassed]);
		}

		float distance = Vector3.Distance(centralCube.position, mapMaker.PathPoints[pointsPassed+1]);

		if (distance < speed || pDistance < distance){
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
		centralCube.LookAt(centralCube.position + direction);

		float theta = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
		elasticConnection.ResetRotationRelation(new Vector3(0, 0, theta));
		MapLabel.RotZ = theta;
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
		//Vector2 center = new Vector2((a.x + b.x)/2f, (a.y + b.y)/2f);
		float maxDistance = Vector3.Distance(a, b) + 15;

		bool usingHeight = Mathf.Abs(a.x - b.x)/Mathf.Abs(a.y - b.y) > ((0f + Screen.width) / (0f + Screen.height));

		float scaleFactor = -0.75f * (usingHeight ? Screen.height : Screen.width) / 976f;

		//this.transform.position = new Vector3(a.x, a.y, maxDistance * scaleFactor + 1f);
		elasticConnection.ResetMovementRelation(new Vector3(a.x, a.y, maxDistance * scaleFactor + 1f));
		//elasticConnection.ResetRotationRelation(new Vector3(0, 0, centralCube.rotation.eulerAngles.x + 90f));
	}
	
	void finishSegment(){

		pointsPassed = mapMaker.PathPoints.Length - 1;
		centralCube.position = mapMaker.PathPoints[pointsPassed];
		if (remainingSegments.Count > 1 && speed > 0.01f){
			Debug.Log("Reseting path");
			speed = 0f;
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
