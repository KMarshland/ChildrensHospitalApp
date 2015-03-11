//--------------------------------------
//               PowerUI
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

#if UNITY_2_6 || UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4
	#define PRE_UNITY3_5
#endif

#if PRE_UNITY3_5 || UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6
	#define PRE_UNITY5
#endif

using System;
using UnityEngine;
using PowerUI;
using PowerUI.Css;
using Nitro;
using Wrench;
using UnityHttp;
using InfiniText;

#if IsolatePowerUI || UNITY_4_6 || !PRE_UNITY5
namespace PowerUI{
#endif

/// <summary>
/// This class holds global variables and methods for managing PowerUI.
/// The most important ones are:
/// <see cref="UI.Start"/>
/// <see cref="UI.Update"/>
/// <see cref="UI.document"/>
/// </summary>

public static class UI{
	
	/// <summary>The default max update rate in fps. Note that input is decoupled from this.</summary>
	public const int DefaultRate=40;
	
	/// <summary>The PowerUI layer's index. Used to make sure the UI is not visible with other cameras.</summary>
	public static int Layer;
	/// <summary>True if Start has been called.</summary>
	private static bool Started;
	/// <summary>A custom content element. You can set this to whatever element you want for easy access.</summary>
	public static Element content;
	/// <summary>The time in seconds between redraws. <see cref="UI.SetRate"/>.</summary>
	public static float RedrawRate;
	/// <summary>The unity camera that will render all UI elements.</summary>
	public static Camera GUICamera;
	/// <summary>The main UI document. This is what you should use to add content to the UI.</summary>
	public static Document document;
	/// <summary>True if the UI consumed the current frames input event.</summary>
	public static bool HandledInput;
	/// <summary>The field of view of the perspective UI camera.</summary>
	private static float FieldOfView;
	/// <summary>How much time, in seconds, has passed since our last redraw.</summary>
	private static float RedrawTimer;
	/// <summary>The gameobject which all UI elements are parented to.</summary>
	public static GameObject GUINode;
	/// <summary>The renderer which renders the main UI.</summary>
	private static Renderman Renderer;
	/// <summary>How much space should be given for elements.
	/// This distance essentially defines how much z-index is allowed.</summary>
	public static float CameraDistance;
	/// <summary>The gameobject that our <see cref="UI.GUICamera"/> is parented to.</summary>
	public static GameObject CameraNode;
	/// <summary>The transform of the <see cref="UI.GUICamera"/>.</summary>
	public static Transform CameraTransform;
	/// <summary>This variable set is used when parsing the HTML.
	/// &Variables; can be used to replace text with custom values; e.g. in localization or &Username;.</summary>
	public static FullVariableSet Variables;
	/// <summary>All world UI's are kept in a linked list for updates. This is the tail of the list.</summary>
	public static WorldUI LastWorldUI;
	/// <summary>All world UI's are kept in a linked list for updates. This is the head of the list.</summary>
	public static WorldUI FirstWorldUI;
	/// <summary>The default depth of the main camera.</summary>
	public static int CameraDepth=100;
	/// <summary>A set of cameras available for use with the main UI. This is only ever used with inline cameras.</summary>
	public static CameraPool MainCameraPool;
	/// <summary>Updates the UI globally. Note that this is setup automatically for you.</summary>
	private static StandardUpdater GlobalUpdater;
	/// <summary>The type that all code derives from. Must be UICode or something that inherits UICode.</summary>
	public static Type BaseCodeType=typeof(UICode);
	/// <summary>This event is fired when PowerUI generates a new camera. This is the best way to apply custom rendering to the main UI.</summary>
	public static event CameraCreated OnCameraCreated;
	/// <summary>A loader which pulls languages from the Resources/Languages/UI folder.</summary>
	private static UnityLanguageLoader LanguageLoader;
	/// <summary>Called when a document wants the default security domain. Set with a custom method to provide your own.</summary>
	public static GetSecurityDomain OnGetSecurityDomain;
	/// <summary>The current camera mode PowerUI is using. The default is Perspective. See UI.CameraMode to change this.</summary>
	private static CameraMode CurrentCameraMode=CameraMode.Perspective;
	
	
	/// <summary>Gets the content of the given file from resources.</summary>
	/// <param name="resource">The path to the file in Resources.</param>
	/// <returns>The file content which should be html formatted.</returns>
	public static string LoadHTML(string resource){
		string resourceName=resource.Trim();
		
		if(resourceName.EndsWith(".html")){
			resourceName=resourceName.Substring(0,resourceName.Length-5);
		}
		
		TextAsset asset=((TextAsset)Resources.Load(resourceName));
		
		if(asset==null){
			return "";
		}
		
		return asset.text;
	}
	
	/// <summary>Used internally to fire the OnCameraCreated event.</summary>
	public static void CameraGotCreated(Camera camera){
		
		#if !PRE_UNITY3_5
		// Setup the sort order:
		camera.transparencySortMode=TransparencySortMode.Orthographic;
		#endif
		
		// Fire the event:
		if(OnCameraCreated!=null){
			OnCameraCreated(camera);
		}
		
	}
	
	/// <summary>Startup the UI. Now called internally, but can be called at the start of any scene using PowerUI.</summary>
	public static void Start(){
		Start(false);
	}
	
	/// <summary>Used internally - don't call this one. Startup the UI for use in the Editor with AOT Nitro.</summary>
	/// <param name="nitroAot">True if no gameobject should be generated.</param>
	public static void Start(bool nitroAot){
		
		if(!Started){
			Started=true;
			
			// Setup atlas stacks:
			AtlasStacks.Start();
			
			// Hookup the wrench logging method:
			Wrench.Log.OnLog+=OnLogMessage;
			
			// Hookup the InfiniText logging method:
			InfiniText.Fonts.OnLog+=OnLogMessage;
			
			// Startup the tag handlers:
			Wrench.TagHandlers.Setup();
			
			// Startup the file protocols (internally also starts up the CSS engine):
			FileProtocols.Setup();
			
			// Setup the text/language service:
			if(Variables==null){
				Variables=new FullVariableSet();
				
				if(!nitroAot){
					// Sign up to the variable on change event - whenever a custom var is changed, we need to refresh the screen.
					Variables.OnChange+=OnVariableChange;
					// Ensure that variables is set to whatever the default/current language is.
					OnLanguageChange(Wrench.Text.Language);
					// Sign on to the event that occurs when the language changes.
					Wrench.Text.OnLanguageChanged+=OnLanguageChange;
					// Sign on to the event that occurs when the gender changes.
					Wrench.Text.OnGenderChanged+=ResolveAllVariables;
				}
			}
			
			// Setup the callback queue:
			Callbacks.Start();
			
			// Setup the character providers (for e.g. Emoji):
			CharacterProviders.Setup();
			
			Layer=LayerMask.NameToLayer("PowerUI");
			
			if(Layer<0){
				// Invalid layer.
				#if UNITY_EDITOR
				
				// Create the new layer now (this will actually be a permanent change):
				Layer=PowerUI.LayerManager.Add();
				
				#else
				throw new Exception("Error: PowerUI layer not found. Go to Edit->Project Settings->Tags and add a layer called PowerUI to fix this."+
									" Don't forget to make sure it doesn't render with your main camera too!"
									);
				#endif
			}
			
			// Default FPS:
			SetRate(DefaultRate);
			
			#if !NoNitroRuntime
			// Setup the compiler:
			NitroCode.Setup();
			#endif
		}
		
		if(nitroAot){
			return;
		}
		
		GUINode=GameObject.Find("#PowerUI");
		
		if(GUINode==null){
			// Not started yet.
			
			// Create the UI game object:
			GUINode=new GameObject();
			GUINode.name="#PowerUI";
			
			// Create the camera:
			CameraNode=new GameObject();
			CameraNode.name="Camera";
			
			// Create the updater:
			GlobalUpdater=GUINode.AddComponent<StandardUpdater>();
			
			// Setup the camera:
			GUICamera=CameraNode.AddComponent<Camera>();
		}else{
			// Already started, but we might have updated.
			
			if(CameraNode==null){
				// This can happen if the PowerUI assembly is actively reloaded (e.g. runtime updates).
				CameraNode=GameObject.Find("#PowerUI/Camera");
				CameraTransform=CameraNode.transform;
				GUICamera=CameraNode.GetComponent<Camera>();
			}else{
				// Already started!
				return;
			}
			
		}
		
		// Hide the PowerUI layer from all cameras other than GUICamera: 
		Camera[] cameras=Camera.allCameras;
		
		int layerMask=~(1<<UI.Layer);
		
		for(int i=0;i<cameras.Length;i++){
			// Grab the camera:
			Camera camera=cameras[i];
			
			// Is it the GUICamera?
			if(camera==GUICamera){
				continue;
			}
			
			// Hide the UI layer from it:
			camera.cullingMask&=layerMask;
		}
		
		// Setup the transform:
		CameraTransform=CameraNode.transform;
		CameraTransform.parent=GUINode.transform;
		
		GUICamera.nearClipPlane=0.2f;
		GUICamera.depth=CameraDepth;
		GUICamera.clearFlags=CameraClearFlags.Depth;
		GUICamera.cullingMask=(1<<UI.Layer);
		GUICamera.renderingPath=RenderingPath.Forward;
		
		SetCameraDistance(60f);
		SetFieldOfView(60f);
		
		Renderer=new Renderman();
		
		// Render Mesh.OutputGameObject with the GUI camera:
		Renderer.RenderWithCamera(UI.Layer);
		document=Renderer.RootDocument;
		document.window.top=document.window;
		
		// Some overriding default UI settings:
		document.html.Style.Computed.ChangeTagProperty("color",new PowerUI.Css.Value("#ffffff",PowerUI.Css.ValueType.Color));
		document.html.Style.Computed.ChangeTagProperty("font-size",new PowerUI.Css.Value("14px",PowerUI.Css.ValueType.Pixels));
		document.body.Style.Computed.ChangeTagProperty("overflow",new PowerUI.Css.Value("hidden hidden",PowerUI.Css.ValueType.Point));
		
		UpdateTextDirection();
		
		// Fire the camera event:
		CameraGotCreated(GUICamera);
		
	}
	
	/// <summary>Used to hide the UI from the given camera if it's generated entirely at runtime.</summary>
	public static void HideFromCamera(Camera camera){
		// Get the inverted layerID:
		int layer=~(1<<Layer);
		
		// Pop it onto the cameras culling mask:
		camera.cullingMask&=layer;
	}
	
	/// <summary>The current UI resolution, used for pixel perfect clarity if you target both high and low resolution screens. 
	/// Note that this should only be changed before you apply any HTML to your UI's.</summary>
	public static ResolutionInfo Resolution{
		get{
			return ScreenInfo.Resolution;
		}
		set{
			ScreenInfo.Resolution=value;
		}
	}
	
	/// <summary>What sort of camera should PowerUI use to render the main UI? Perspective is the default.</summary>
	public static CameraMode CameraMode{
		get{
			return CurrentCameraMode;
		}
		set{
			if(value==CurrentCameraMode){
				return;
			}
			
			// Change the current mode:
			CurrentCameraMode=value;
			
			// Apply the new settings to the camera:
			if(value==CameraMode.Orthographic){
				GUICamera.orthographic=true;
			}else{
				GUICamera.orthographic=false;
			}
			
			// Reapply the camera distance. This makes sure the new camera is setup correctly
			// and it also sets up some depth variables used in rendering.
			SetCameraDistance(CameraDistance);
			
			// Queue up a layout:
			Renderer.RequestLayout();
		}
	}
	
	/// <summary>How the main UI renders images; either on an atlas or with them 'as is'.
	/// Default is Atlas.</summary>
	public static PowerUI.RenderMode RenderMode{
		get{
			if(Renderer==null){
				Start();
			}
			return Renderer.RenderMode;
		}
		set{
			if(Renderer==null){
				Start();
			}
			Renderer.RenderMode=value;
		}
	}
	
	[Obsolete("Atlases are now always global. If you wish to define their size, see AtlasStacks.InitialSize instead.")]
	public static int AtlasSize;
	
	/// <summary>Sets how many world units are used between elements at different depths. Default is 0.05.</summary>
	/// <param name="gaps">The distance between elements to use.</param>
	public static void SetDepthResolution(float gaps){
		Renderer.DepthResolution=gaps;
	}
	
	/// <summary>Gets the renderer which renders this UI.</summary>
	public static Renderman GetRenderer(){
		return Renderer;
	}
	
	/// <summary>The mode PowerUI uses for recieving input. Physics mode is more intensive than Screen mode,
	/// however using physics mode means transformed elements receive input correctly.</summary>
	public static InputMode InputMode{
		get{
			return PowerUI.Input.Mode;
		}
		set{
			if(PowerUI.Input.Mode==value){
				return;
			}
			
			PowerUI.Input.Mode=value;
			
			// Update the renderer:
			Renderer.SetInputMode(value);
		}
	}
	
	/// <summary>The mode PowerUI uses for recieving input for WorldUI's. Physics mode is more intensive than Screen mode,
	/// however using physics mode means transformed elements receive input correctly. None by default.</summary>
	public static InputMode WorldInputMode{
		get{
			return PowerUI.Input.WorldInputMode;
		}
		set{
			if(PowerUI.Input.WorldInputMode==value){
				return;
			}
			
			PowerUI.Input.WorldInputMode=value;
			
			// Update all active worldUI renderers:
			WorldUI currentWorldUI=FirstWorldUI;
			
			while(currentWorldUI!=null){
				currentWorldUI.SetInputMode(value);
				currentWorldUI=currentWorldUI.UIAfter;
			}
		}
	}
	
	/// <summary>The image filter mode.</summary>
	public static FilterMode FilterMode{
		get{
			if(!Started){
				Start();
			}
			
			return Renderer.FilterMode;
		}
		set{
			if(!Started){
				Start();
			}
			
			Renderer.FilterMode=value;
		}
	}
	
	/// <summary>Sets the text filter mode. Note: shared fonts will be affected in all other UI's.</summary>
	public static FilterMode TextFilterMode{
		get{
			if(!Started){
				Start();
			}
			
			return Renderer.TextFilterMode;
		}
		set{
			if(!Started){
				Start();
			}
			
			Renderer.TextFilterMode=value;
		}
	}
	
	/// <summary>Called by wrench when it logs a message.</summary>
	/// <param name="message">The message to log.</param>
	public static void OnLogMessage(string message){
		UnityEngine.Debug.Log(message);
	}
	
	/// <summary>Sets the group resolver to use when resolving a group name from a variable
	/// that isn't found in the languages groups. E.g. &cdn.text; can be handled in custom ways here.</summary>
	/// <param name="resolver">The resolver to use.</param>
	public static void SetGroupResolver(GroupResolve resolver){
		Variables.SetGroupResolver(resolver);
	}
	
	/// <summary>Sets the distance of the camera, essentially defining the amount of z-index available.</summary>
	/// <param name="distance">The distance of the camera from the origin along the z axis in world units.</param>
	public static void SetCameraDistance(float distance){
		CameraDistance=distance;
		GUICamera.farClipPlane=CameraDistance*2f;
		UI.CameraTransform.localPosition=new Vector3(0f,0f,-CameraDistance);
		
		if(CurrentCameraMode==CameraMode.Perspective){
			ScreenInfo.DepthDepreciation=1f/distance;
		}else{
			ScreenInfo.DepthDepreciation=0f;
		}
	}
	
	/// <summary>Gets the distance of the main UI camera. 
	/// Essentially defines how much z-index space it has.</summary>
	public static float GetCameraDistance(){
		return CameraDistance;
	}
	
	/// <summary>Sets the field of view of the UI camera.</summary>
	/// <param name="fov">The field of view, in degrees.</param>
	public static void SetFieldOfView(float fov){
		FieldOfView=fov;
		GUICamera.fieldOfView=FieldOfView;
	}
	
	/// <summary>Gets the field of view of the UI camera.</summary>
	public static float GetFieldOfView(){
		return FieldOfView;
	}
	
	/// <summary>Clears all content from the UI and all WorldUI's. 
	/// Please note that it is safer to set innerHTML to a blank string for a particular UI than calling this.</summary>
	public static void ClearAll(){
		content=null;
		
		if(Renderer!=null){
			Renderer.Destroy();
			Renderer=null;
			document=null;
		}
		
		Fonts.Clear();
		AtlasStacks.Clear();
		Http.Clear();
		SPA.Clear();
		UIAnimation.Clear();
		DynamicTexture.RemoveAll();
		PowerUI.Input.Clear();
		ScreenInfo.Clear();
		WorldUI currentWorldUI=FirstWorldUI;
		
		while(currentWorldUI!=null){
			currentWorldUI.Destroy();
			currentWorldUI=currentWorldUI.UIAfter;
		}
		
		LastWorldUI=null;
		FirstWorldUI=null;
	}
	
	/// <summary>Clears and entirely destroys the UI and WorldUI's. Calling this is not a requirement and can only be reversed by calling Start again.</summary>
	public static void Destroy(){
		ClearAll();
		
		if(GUINode!=null){
			GameObject.Destroy(GUINode);
			GUINode=null;
			GUICamera=null;
			CameraNode=null;
			CameraTransform=null;
		}
		
		if(GlobalUpdater!=null){
			GameObject.Destroy(GlobalUpdater);
			GlobalUpdater=null;
		}
		
		Started=false;
		Variables=null;
		LastWorldUI=null;
		FirstWorldUI=null;
		
		// Clear batch pool:
		UIBatchPool.Clear();
	}
	
	/// <summary>Gets a loader which tells the wrench framework where to look for
	/// language specific UI text (&variable; content).
	/// This is set to Resources/Languages/UI.</summary>
	private static Wrench.UnityLanguageLoader GetLanguageLoader(){
		if(LanguageLoader==null){
			LanguageLoader=new Wrench.UnityLanguageLoader("UI");
		}
		
		return LanguageLoader;
	}
	
	/// <summary>Called by Wrench when the language of the UI changes.
	/// This changes the language of all on screen &variables;.
	/// <see cref="UI.Language"/>.</summary>
	/// <param name="code">The language code/id (e.g. en).</param>
	public static void OnLanguageChange(string code){
		// Get the new language:
		LanguageSet language=(LanguageSet)(GetLanguageLoader().Get(code));
		// Our variable set is just the language, like so:
		Variables.ChangeLanguageSet(language);
		
		// Update default text direction (e.g. if we jumped from english to arabic, or back):
		UpdateTextDirection();
		
		// Re-resolve any already applied UI elements:
		ResolveAllVariables();
	}
	
	/// <summary>Adds a group of variables from the given xml string. Used to stream in languages at runtime.
	/// A group is used with &groupName.varName;</summary>
	public static void AddTextGroup(string text,string groupName){
		Variables.Language.AddGroup(text,groupName);
		
		// Re-resolve any already applied UI elements:
		ResolveAllVariables();
	}
	
	/// <summary>Makes sure the default text direction matches the one given by the language (e.g. Arabic may tell PowerUI it goes leftwards).
	/// Applies to the main UI and all in-world UI's.</summary>
	private static void UpdateTextDirection(){
		LanguageSet language=(LanguageSet)(GetLanguageLoader().Get(Language));
		if(language==null){
			return;
		}
		bool goesLeftwards=language.GoesLeftwards;
		
		// For the main UI:
		UpdateTextDirection(goesLeftwards,document);
		
		// And all in-world UI's:
		WorldUI worldUI=FirstWorldUI;
		while(worldUI!=null){
			UpdateTextDirection(goesLeftwards,worldUI.document);
			worldUI=worldUI.UIAfter;
		}
		
	}
	
	/// <summary>Makes sure the default text direction is as given for the given html document.</summary>
	/// <param name="goesLeftwards">True if the default text direction is leftwards.</param>
	/// <param name="htmlDocument">The document to apply the setting to.</param>
	private static void UpdateTextDirection(bool goesLeftwards,Document htmlDocument){
		if(htmlDocument!=null && htmlDocument.html!=null){
			bool currentLeftwards=(htmlDocument.html.style.direction=="rtl");
			if(currentLeftwards != goesLeftwards){
				if(goesLeftwards){
					htmlDocument.html.style.direction="rtl";
				}else{
					htmlDocument.html.style.direction="ltr";
				}
			}
		}
	}
	
	public static void OnVariableChange(string code){
		if(code=="*"){
			ResolveAllVariables();
		}else{
			// The main UI:
			ResolveVariable(document,code);
			
			// And all WorldUI's:
			WorldUI worldUI=FirstWorldUI;
			while(worldUI!=null){
				ResolveVariable(worldUI.document,code);
				worldUI=worldUI.UIAfter;
			}
		}
	}
	
	/// <summary>Resolves all &variables; as used by the given document their new values. Used when e.g. the language changes.</summary>
	/// <param name="htmlDocument">The document to update the variables for.</param>
	private static void ResolveVariable(Document htmlDocument,string code){
		if(htmlDocument!=null && htmlDocument.html!=null){
			htmlDocument.html.ResetVariable(code);
		}
	}
	
	/// <summary>Resolves all &variables; as used by PowerUI to their new values. Used when e.g. the language changes.</summary>
	private static void ResolveAllVariables(){
		// Re-resolve all current variables.
		
		// First, the main UI:
		ResolveAllVariables(document);
		
		// And all WorldUI's:
		WorldUI worldUI=FirstWorldUI;
		while(worldUI!=null){
			ResolveAllVariables(worldUI.document);
			worldUI=worldUI.UIAfter;
		}
	}
	
	/// <summary>Resolves all &variables; as used by the given document their new values. Used when e.g. the language changes.</summary>
	/// <param name="htmlDocument">The document to update the variables for.</param>
	private static void ResolveAllVariables(Document htmlDocument){
		if(htmlDocument!=null && htmlDocument.html!=null){
			htmlDocument.html.ResetAllVariables();
		}
	}
	
	/// <summary>Gets or sets the language code (e.g. "en") of the UI. This will internally update
	/// any onscreen variables for localization so you won't need to restart the game.
	/// Please refer to our guides on localization on the website for further help with this.</summary>
	public static string Language{
		get{
			return Wrench.Text.Language;
		}
		set{
			Wrench.Text.Language=value;
		}
	}
	
	/// <summary>Gets or sets the gender (boy, girl, either). This is used for localization
	/// where some text is better off being specific depending on what gender the player is - e.g. when using she/ he.
	/// </summary>
	public static string Gender{
		get{
			return Wrench.Text.Gender.ToString();
		}
		set{
			value=value.Trim().ToLower();
			if(string.IsNullOrEmpty(value)){
				value="";
			}
			Wrench.Gender gender=Wrench.Gender.Either;
			if(value=="boy"||value=="male"){
				gender=Wrench.Gender.Boy;
			}else if(value=="girl"||value=="female"){
				gender=Wrench.Gender.Girl;
			}
			Wrench.Text.Gender=gender;
		}
	}
	
	/// <summary>Gets a list of all available languages for use with the UI.</summary>
	/// <returns>A list of variable sets. Each one represents a UI language.</returns>
	public static LanguageSet[] GetAllLanguages(){
		return GetLanguageLoader().AllLanguages();
	}
	
	/// <summary>Asks the renderer for a layout (Used internally).</summary>
	public static void RequestLayout(){
		if(Renderer==null){
			return;
		}
		Renderer.RequestLayout();
	}
	
	/// <summary>Sets the update rate of the UI.</summary>
	/// <param name="fps">The rate in frames per second. Default is UI.DefaultRate.</param>
	public static void SetRate(int fps){
		if(fps<=0){
			fps=DefaultRate;
		}
		
		RedrawRate=1f/(float)fps;
		
		// Let the atlases know:
		AtlasStacks.SetRate(fps);
	}
	
	/// <summary>Returns true if the given x/y coordinate is over the UI.</summary>
	/// <param name="x">The on screen x coordinate as measured from the left.</param>
	/// <param name="y">The on screen y coordinate as measured from the top.</param>
	/// <returns>True if the mouse is over an element.</returns>
	public static bool MouseOver(float x,float y){
		//Returns true if the given point is over the UI.
		return PowerUI.Input.OnMouseOver((int)x,(int)y);
	}
	
	/// <summary>Handles mouse and keyboard input. Call this from OnGUI.</summary>
	/// <returns>True if the UI consumed the current event.</returns>
	#if !NoPowerUIInput
	[Obsolete("You don't need to call HandleInput at all anymore! If you want the return value, enable custom input in Window/PowerUI/General Settings and you can then call this as before.")]
	#endif
	public static bool HandleInput(){
		#if !NoPowerUIInput
		return HandledInput;
		#else
		return InternalHandleInput();
		#endif
	}
	
	/// <summary>Handles mouse and keyboard input. Called from OnGUI internally. See HandleInput.</summary>
	/// <returns>True if the UI consumed the current event.</returns>
	public static bool InternalHandleInput(){
		PowerUI.Input.SetMousePosition(Event.current.mousePosition);
		bool result=false;
		
		switch(Event.current.type){
			case EventType.MouseDown:
				Vector2 mp=PowerUI.Input.MousePosition;
				
				if(PowerUI.Input.OnMouseDown((int)mp.x,(int)mp.y)){
					result=true;
				}
			break;
			case EventType.MouseUp:
				Vector2 mouse=PowerUI.Input.MousePosition;
				
				if(PowerUI.Input.OnMouseUp((int)mouse.x,(int)mouse.y)){
					result=true;
				}
			break;
			case EventType.KeyUp:
				result=PowerUI.Input.OnKeyUp((int)Event.current.keyCode,Event.current.character);
			break;
			case EventType.KeyDown:
				result=PowerUI.Input.OnKeyDown((int)Event.current.keyCode,Event.current.character);
			break;
		}
		
		HandledInput=result;
		return result;
	}
	
	/// <summary>Gets the default security domain for Nitro scripts.</summary>
	public static NitroDomainManager DefaultSecurityDomain{
		get{
			if(OnGetSecurityDomain!=null){
				return OnGetSecurityDomain();
			}
			return new UIScriptDomainManager();
		}
	}
	
	/// <summary>Gets/sets if the UI should log messages to the console. Default is true.</summary>
	public static bool Log{
		get{
			return Wrench.Log.Active;
		}
		set{
			Wrench.Log.Active=value;
		}
	}
	
	/// <summary>Shortcut for calling UI.Start then using UI.document.innerHTML. Applies the given HTML to the whole main UI.</summary>
	public static string Html{
		get{
			if(document==null){
				return "";
			}
			
			return document.innerHTML;
		}
		set{
			if(document==null){
				UI.Start();
			}
			
			document.innerHTML=value;
		}
	}
	
	/// <summary>Shortcut for calling UI.Start then using UI.document.innerHTML. Applies the given HTML to the whole main UI.</summary>
	public static string innerHTML{
		get{
			return Html;
		}
		set{
			Html=value;
		}
	}
	
	/// <summary>Prompts the UI to redraw on the next Update. You don't need to call this, however
	/// it can be useful in rare situations where you require the UI to bypass it's usual redraw
	///  rate (see SetRate) and refresh immediately.</summary>
	public static void RedrawNextUpdate(){
		RedrawTimer=float.MaxValue;
	}
	
	/// <summary>In general you should never call this. It forces a full Update to occur right now;
	/// this can only be called from the main Unity thread.</summary>
	public static void RedrawNow(){
		// Request a redraw next update:
		RedrawNextUpdate();
		// Force it by running an update right now:
		InternalUpdate();
	}
	
	/// <summary>Requests all UI's to update. This occurs when an atlas has been optimised.</summary>
	public static void RequestFullLayout(){
		
		// Request renderer layout (full):
		Renderer.RequestLayout();
		
		WorldUI current=FirstWorldUI;
		while(current!=null){
			
			if(current.Renderer!=null){
				// Request worldUI layout (full):
				current.Renderer.RequestLayout();
			}
			
			current=current.UIAfter;
		}
		
	}
	
	/// <summary>Updates the UI. Don't call this - PowerUI knows when it's needed; This is done from Start and WorldUI constructors.</summary>
	public static void InternalUpdate(){
		
		// Update any callbacks:
		if(Callbacks.FirstToRun!=null){
			Callbacks.RunAll();
		}
		
		// Update animations:
		SPA.Update();
		
		// Update any Http requests:
		Http.Update();
		
		if(WorldUI.LiveUpdatablesAvailable){
			WorldUI.UpdateAll();
		}
		
		RedrawTimer+=Time.deltaTime;
		
		if(RedrawTimer<RedrawRate){
			return;
		}
		RedrawTimer=0f;
		
		if(GUICamera==null){
			return;
		}
		
		// Atlases:
		AtlasStacks.Update();
		
		// Screen size:
		ScreenInfo.Update();
		
		// Update Input (mouse/keys etc).
		PowerUI.Input.Update();
		
		// Animations:
		UIAnimation.Update();
		
		// Dynamic graphics:
		DynamicTexture.Update();
		
		// Redraw the root html document (if it needs to be redrawn):
		Renderer.Update();
		
		if(MainCameraPool!=null && MainCameraPool.DidLayout){
			// The main UI did a layout and we have a camera pool.
			// We now need to do some post-layout spring cleaning.
			MainCameraPool.ClearPool();
		}
		
		// Redraw any in-world documents (if they need it).
		
		// Did we call update all above?
		bool worldUIRequiresUpdate=!WorldUI.LiveUpdatablesAvailable;
		
		// Clear the flag:
		WorldUI.LiveUpdatablesAvailable=false;
		
		if(FirstWorldUI!=null){
			
			WorldUI current=FirstWorldUI;
			while(current!=null){
					
				if(worldUIRequiresUpdate){
					// Update:
					current.Update();
					
					// Was it destroyed?
					if(current.Renderer==null){
						// Hop to the next one:
						current=current.UIAfter;
						
						continue;
					}
					
				}
				
				if(current.Expires){
					current.ExpiresIn-=RedrawRate;
					
					if(current.ExpiresIn<=0f){
						
						// Expire it:
						current.Expire();
						
						// Hop to the next one:
						current=current.UIAfter;
						
						continue;
					}
					
				}
				
				// Update the renderer:
				current.Renderer.Update();
				
				// Update the flag:
				if(current.PixelPerfect || current.AlwaysFaceCamera){
					// We have at least one which is updateable:
					WorldUI.LiveUpdatablesAvailable=true;
				}
				
				current=current.UIAfter;
			}
			
		}
		
		// Flush any atlases:
		AtlasStacks.Flush();
		
	}
	
}

#if IsolatePowerUI || UNITY_4_6 || !PRE_UNITY5
}
#endif
