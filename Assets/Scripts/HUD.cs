using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using KaiTools;

public class HUD : MonoBehaviour {

	Dictionary<string, Action> screens;
	public string screenID;
	public string defaultScreenID;

	GUISkin mainSkin;

	Vector2 menuScrollPosition;

	MenuItem[] menuItems;

	ChatRoom chat;
	
	// Use this for initialization
	void Start () {

		//initialize the list of screens
		screens = new Dictionary<string, Action>();

		//map out which ids map to which functions
		screens.Add("chat", chatScreenGUI);
		screens.Add("main", mainScreenGUI);
		screens.Add("general information", generalInfoScreenGUI);
		screens.Add("contact", contactScreenGUI);
		screens.Add("menu", menuScreenGUI);

		//set the default screen that this starts on
		defaultScreenID = defaultScreenID == "" ? "main" : defaultScreenID;//set the deafult screen id to main if you haven't set it elsewhere
		screenID = defaultScreenID;

		//dynamically load the GUISkins
		mainSkin = Resources.Load("GUISkins/MainSkin") as GUISkin;

		//Set the background color
		//setBackgroundColor(Color.white); it's already set to this by default for slight performance boost

		//preload the menu items
		menuItems = new MenuItem[]{
			new MenuItem("Textures/imageempty", "Name of Menu Item", "Description and price"),
			new MenuItem("Textures/imageempty", "Name of Menu Item", "Description and price"),
			new MenuItem("Textures/imageempty", "Name of Menu Item", "Description and price"),
			new MenuItem("Textures/imageempty", "Name of Menu Item", "Description and price"),
			new MenuItem("Textures/imageempty", "Name of Menu Item", "Description and price"),
			new MenuItem("Textures/imageempty", "Name of Menu Item", "Description and price"),
			new MenuItem("Textures/imageempty", "Name of Menu Item", "Description and price"),
			new MenuItem("Textures/imageempty", "Name of Menu Item", "Description and price"),
			new MenuItem("Textures/imageempty", "Name of Menu Item", "Description and price"),
			new MenuItem("Textures/imageempty", "Name of Menu Item", "Description and price"),
			new MenuItem("Textures/imageempty", "Name of Menu Item", "Description and price"),
			new MenuItem("Textures/imageempty", "Name of Menu Item", "Description and price"),
			new MenuItem("Textures/imageempty", "Name of Menu Item", "Description and price"),
			new MenuItem("Textures/imageempty", "Name of Menu Item", "Description and price"),
			new MenuItem("Textures/imageempty", "Name of Menu Item", "Description and price"),
			new MenuItem("Textures/imageempty", "Name of Menu Item", "Description and price"),
			new MenuItem("Textures/imageempty", "Name of Menu Item", "Description and price")
		};

		//get the reference to the chatroom instance
		chat = this.gameObject.GetComponent("ChatRoom") as ChatRoom;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//All GUI stuff must be called from here
	void OnGUI(){

		setGUIDefaults();//sets the default looks. Can always be overridden in indivdual screens

		//make sure that the screen exists
		Action screen;
		if (screens.TryGetValue(screenID, out screen)){
			screen();//call whichever screen is currently active
		} else {
			screenID = defaultScreenID;//there's an error - revert to the main screen
		}
	}

	void mainScreenGUI(){
		GUI.skin.button.fontSize = (int)(0.025f * Screen.width);//scale the font size

		//create buttons linking to the stuff, each taking up a bit less than a third of the screen, with padding in betwen
		if (GUI.Button(new FRect(0.02f, 0.05f, 0.3066f, 0.9f).toRect, "General Information")){
			screenID = "general information";
		}

		if (GUI.Button(new FRect(0.3466f, 0.05f, 0.3066f, 0.9f).toRect, "Chat")){
			screenID = "chat";
		}

		if (GUI.Button(new FRect(0.67333f, 0.05f, 0.3066f, 0.9f).toRect, "Contact - this works in mobile but not desktop because I'm too lazy to bother making it work")){
			Application.OpenURL("mailto:sophiaporter7@gmail.com");
			screenID = "contact";
		}

		
	}

	void chatScreenGUI(){
		GUI.skin = chat.skin;//gui skin fun
		if (chat.irc == null || !chat.irc.IsConnected) {
			chat.userName = "HospitalAppUser";//set the username of the person messaging. This won't be displayed
			chat.usernameToMessage = "Kofthefens";//set the username of the doctor or person it's set to message to
			chat.sendingMessage = true;//make sure it knows it's supposed to be sending a message
			chat.Connect();//try connecting to the server
		} else {//you must already be connected
			chat.DoChatWindow();//so show the chat stuff
		}

		if (chat.exited){
			screenID = "main";
		}
	}

	void generalInfoScreenGUI(){
		//we want 6 buttons, plus a back button in the corner
		GUI.skin.button.fontSize = (int)(0.02f * Screen.width);//scale the font size

		//back button
		if (GUI.Button(new FRect(0.01f, 0.01f, 0.075f, 0.05f).toRect, "Back")){
			screenID = "main";
		}

		GUI.skin.button.fontSize = (int)(0.025f * Screen.width);//rescale the font size
		//top row
		if (GUI.Button(new FRect(0.02f, 0.1f, 0.3066f, 0.375f).toRect, "Menu")){
			screenID = "menu";
		}
		
		if (GUI.Button(new FRect(0.3466f, 0.1f, 0.3066f, 0.375f).toRect, "[Button]")){
			screenID = "";
		}
		
		if (GUI.Button(new FRect(0.67333f, 0.1f, 0.3066f, 0.375f).toRect, "[Button]")){
			screenID = "";
		}

		//bottom row
		if (GUI.Button(new FRect(0.02f, 0.525f, 0.3066f, 0.375f).toRect, "[Button]")){
			screenID = "";
		}
		
		if (GUI.Button(new FRect(0.3466f, 0.525f, 0.3066f, 0.375f).toRect, "[Button]")){
			screenID = "";
		}
		
		if (GUI.Button(new FRect(0.67333f, 0.525f, 0.3066f, 0.375f).toRect, "[Button]")){
			screenID = "";
		}

	}

	void contactScreenGUI(){
		screenID = "main";//will open the email in a new window, so may as well return to the main screen
	}

	void menuScreenGUI(){

		//add fancy ass header at the top
		GUI.Box(new FRect(-1f, -1f, 3f, 1.15f).toRect, "");
		GUI.skin.label.fontSize = (int)(0.05f * Screen.width);//scale the font size
		float xpix = GUI.skin.label.CalcSize(new GUIContent("Menu")).x;
		GUI.Label(new FRect(0.5f - (0.5f * xpix / Screen.width), 0.01f, 1f, 1f).toRect, "Menu");

		GUI.skin.button.fontSize = (int)(0.02f * Screen.width);//scale the font size
		GUI.backgroundColor = Color.white;
		GUI.contentColor = Color.black;
		if (GUI.Button(new FRect(0.01f, 0.05f, 0.075f, 0.05f).toRect, "Back")){
			screenID = "general information";
		}

		//generate the scroll area
		//note: MenuItem is a self defined struct. The list is created in Start()
		menuScrollPosition = GUI.BeginScrollView(new FRect(0.025f, 0.2f, 0.9f, 0.75f).toRect, 
		                                     menuScrollPosition, new FRect(0, 0, 0.85f, 0.1f * (menuItems.Length)).toRect, false, false);

		//make labels black, as the default was white
		GUI.contentColor = Color.black;

		//and draw the items in the area
		for (int i = 0; i < menuItems.Length; i++){
			//draw the thumbnail
			GUI.DrawTexture(new FRect(0f, 0.1f*i, 0.09f, 0.09f).toRect, menuItems[i].Image, ScaleMode.ScaleToFit);

			//draw the header
			GUI.skin.label.fontSize = 24;//(int)(0.02f * Screen.width);//scale the font size
			GUI.Label(new FRect(0.1f, 0.1f*i - 0.01f, 0.8f, 0.15f).toRect, menuItems[i].Header);

			//draw the content
			GUI.skin.label.fontSize = 18;//(int)(0.015f * Screen.width);//scale the font size
			GUI.Label(new FRect(0.1f, 0.1f*i + 0.04f, 0.8f, 0.06f).toRect, menuItems[i].Content);
		}
		GUI.EndScrollView();
	}

	void setGUIDefaults(){
		GUI.skin = mainSkin;//set the GUI skin to the default
		GUI.backgroundColor = Color.black;//make the buttons black
	}

	void setBackgroundColor(Color color){
		//Doesn't really need to be done from here, but it's handy to have
		Camera.main.backgroundColor = color;
	}
}
