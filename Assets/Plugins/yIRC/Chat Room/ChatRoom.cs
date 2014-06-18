using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using yIRC;

public class ChatRoom : MonoBehaviour {
    public string serverName = "irc.freenode.net";
    public string userName = "hospital_Client";
    public string channel = "#yIRC";
    public int port = 6667;
    public yIRC.IrcClient irc = null;
    public GUISkin skin = null;

	public bool sendingMessage = false;
	public string usernameToMessage = "Kofthefens";

	public bool exited = false;

    private int activeChannel = 0;
    private string inputString = "";
	private string connectionLog = "Connecting";

    private static int USERNAME_BUTTON_WIDTH = 140;
    private static int SEND_BUTTON_WIDTH = USERNAME_BUTTON_WIDTH;
    private static int USER_LIST_WIDTH = USERNAME_BUTTON_WIDTH + 4;
    private static int USER_LIST_SCROLL_WIDTH = 15;
    private static int TEXT_AREA_BORDER = 80;

    private bool showSendButton = false;

    // List of chat history for all channels and private messages
    private Dictionary<string, string> channelsText = new Dictionary<string, string>();

    // List of scrolls positions for all channels and private messages
    private Dictionary<string, Vector2> channelsScrolls = new Dictionary<string, Vector2>();

    public void Start() {
        channelsText["SERVER"] = ""; // initialize server log
    }

    void OnGUI() {
        /*if (skin != null && GUI.skin != skin) {
            GUI.skin = skin;
        }

        if (irc == null || !irc.IsConnected) {
            if (GUILayout.Button("Connect", GUILayout.Width(USERNAME_BUTTON_WIDTH))) {
                Connect();
            }
            if (GUILayout.Button("Exit", GUILayout.Width(USERNAME_BUTTON_WIDTH))) {
                Application.Quit();
            }

        } else {
            DoChatWindow();
        }*/
    }

    /// <summary>
    /// Connect to the server; Subscribe to important events, Join channel
    /// </summary>
    public void Connect() {
        if (irc != null && irc.IsConnected)
            irc.Disconnect();

		exited = false;

        irc = new IrcClient();
        irc.Encoding = System.Text.Encoding.UTF8;
        irc.SendDelay = 200; // set send delay (flood protection)
        irc.ActiveChannelSyncing = true;

        irc.OnError += irc_OnError;
        irc.OnRawMessage += irc_OnRawMessage;
        irc.OnQueryMessage += irc_OnQueryMessage;

		connectionLog = "Connecting";

        try {
            irc.Connect(serverName, port);
        } catch (ConnectionException e) {
            Log("Couldn't connect! Reason: " + e.Message);
        }

        try {
            irc.Login(userName, "Player1", 0, userName);
            
			StartCoroutine(this.OnTheConnect());
		} catch (ConnectionException e) {
            Log(e.Message);
        } catch (Exception e) {
            Log("Error occurred! Message: " + e.Message);
            Log("Exception: " + e.StackTrace);
        }
    }

    void Update() {
        // Update irc: send and receive messages
        if (irc != null && irc.IsConnected)
            irc.ListenOnce(false);

        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter)) {
            GUI.FocusControl("Input");
        }

        showSendButton = inputString.Length > 0;
    }

    void Log(string message) {
        channelsText["SERVER"] += message;
        Debug.Log(message);
    }

    /// <summary>
    /// Handle all messages here. 
    /// For In-Game chat simply log all messages and show only messages for selected channel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void irc_OnRawMessage(object sender, IrcEventArgs e) {
        // Put all non-channel messages in "SERVER" tab
        string channel = string.IsNullOrEmpty(e.Data.Channel) || e.Data.Type != ReceiveType.ChannelMessage ? "SERVER" : e.Data.Channel;
        string message = string.IsNullOrEmpty(e.Data.Nick) ? "* " + e.Data.Message : string.Format("<{0}> {1}", e.Data.Nick, e.Data.Message);
        if (!channelsText.ContainsKey(channel))
            channelsText[channel] = ""; // Initiate dictionary entry
        else
            channelsText[channel] += "\r\n"; // Or append new line characters

        channelsText[channel] += message;

        // Scroll down on new message
        if (channelsScrolls.ContainsKey(channel))
            channelsScrolls[channel] = new Vector2(0.0f, float.MaxValue);
    }

    /// <summary>
    /// Analyze private messages
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void irc_OnQueryMessage(object sender, IrcEventArgs e) {
        if (!channelsText.ContainsKey(e.Data.Nick))
            channelsText[e.Data.Nick] = ""; // Initiate dictionary entry
        else if (channelsText[e.Data.Nick].Length > 2)
            channelsText[e.Data.Nick] += "\r\n"; // Or append new line characters

        string message = string.Format("<{0}> {1}", e.Data.Nick, e.Data.Message);
        channelsText[e.Data.Nick] += message;
        // Scroll down on new message
        if (channelsScrolls.ContainsKey(e.Data.Nick))
            channelsScrolls[e.Data.Nick] = new Vector2(0.0f, float.MaxValue);
    }

    // Render chat window
    private string nextFrameChannel = "";
    private Vector2 usersScroll = Vector2.zero;
    public void DoChatWindow() {
        List<string> channels = new List<string>();
        channels.Add("SERVER");
        channels.AddRange(irc.GetChannels());

        // Add active windows to list
        foreach (var v in channelsText) {
            if (v.Key != "SERVER" && channels.IndexOf(v.Key) == -1)
                channels.Add(v.Key);
        }

        // Full area
		try {
	        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
	        // Channels / text area / input
	        GUILayout.BeginVertical();

	        // Channels
	        // Open chat window on nickname button click
	        if (nextFrameChannel != "") {
	            activeChannel = channels.IndexOf(nextFrameChannel);
	            nextFrameChannel = "";
	        }

			string[] channelsArray = new string[0];
			if (activeChannel == 0){
				channelsArray = channels.ToArray();
			} else {
				channelsArray = new string[]{ channels.ToArray()[1] };
			}

	        activeChannel = GUILayout.Toolbar(activeChannel, channelsArray);

	        // Text area / users
	        GUILayout.BeginHorizontal(skin.window, null);
	        // Get scroll value for selected channel
	        Vector2 scroll = new Vector2(0.0f, float.MaxValue);
	        if (channelsScrolls.ContainsKey(channels[activeChannel]))
	            scroll = channelsScrolls[channels[activeChannel]];
	        channelsScrolls[channels[activeChannel]] = GUILayout.BeginScrollView(scroll);

			string text;

			if (activeChannel > 0){
	        	text = channelsText.ContainsKey(channels[activeChannel]) ? channelsText[channels[activeChannel]] : "";
			} else {
				text = connectionLog;
			}

			GUILayout.TextArea(text, GUILayout.Width(Screen.width - (USER_LIST_WIDTH + TEXT_AREA_BORDER)));

	        GUILayout.EndScrollView();

	        // Users list        
	        if (activeChannel > 0) {
	            Channel channel = irc.GetChannel(channels[activeChannel]);
	            if (channel != null) {
	                if (channel.Users.Values.Count != 0) {
	                    usersScroll = GUILayout.BeginScrollView(usersScroll, GUILayout.Width(USER_LIST_WIDTH + USER_LIST_SCROLL_WIDTH));
	                    GUILayout.BeginVertical();

	                    foreach (ChannelUser v in channel.Users.Values) {
	                        if (GUILayout.Button(v.Nick, GUILayout.Width(USERNAME_BUTTON_WIDTH))) {
	                            // Chat window opened for a first time 
	                            // Focus it in the next frame
	                            if (!channelsText.ContainsKey(v.Nick)) {
	                                channelsText[v.Nick] = "";
	                                nextFrameChannel = v.Nick;
	                            }
	                            // Select allready opened chat window
	                            if (channels.Contains(v.Nick))
	                                activeChannel = channels.IndexOf(v.Nick);
	                        }
	                    }
	                    GUILayout.EndVertical();
	                    GUILayout.EndScrollView();
	                }
	            }
	        }
	        GUILayout.EndHorizontal();

	        // Input / send button
	        if (activeChannel > 0) {
	            GUILayout.BeginHorizontal();
	            GUI.SetNextControlName("Input");
	            inputString = GUILayout.TextField(inputString);
	            if (GUI.GetNameOfFocusedControl() == string.Empty) {
	                GUI.FocusControl("Input");
	            }

	            if (showSendButton) {
	                // Input control supresst all keycodes, use events system to process enter-code
	                Event e = Event.current;
	                bool bUserHasHitReturn = e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter;
	                if (GUILayout.Button("SEND", GUILayout.Width(SEND_BUTTON_WIDTH)) || bUserHasHitReturn) {
	                    if (inputString.Length != 0) {
	                        irc.SendMessage(SendType.Message, channels[activeChannel], inputString);
	                        if (!channelsText.ContainsKey(channels[activeChannel]))
	                            channelsText[channels[activeChannel]] = "";
	                        else
	                            channelsText[channels[activeChannel]] += "\r\n";

	                        channelsText[channels[activeChannel]] += "<" + "Me" + "> " + inputString;

							inputString = "";
	                    }
	                }
	            }
	            GUILayout.EndHorizontal();
	        }
	        if (GUILayout.Button("Exit")) {
				StartCoroutine(StopIRC());						
	        }
	        GUILayout.EndVertical();
	        GUILayout.EndArea();
		} catch (Exception){
			try {
				GUILayout.EndVertical();
				GUILayout.EndArea();
			} catch (Exception){

			}
		}
	}

    void irc_OnError(object sender, ErrorEventArgs e) {
        Debug.LogError(e.ErrorMessage);
    }

    /// <summary>
    /// Close connection on application exit
    /// </summary>
    void OnApplicationQuit() {
        StartCoroutine(StopIRC());
    }

    private IEnumerator StopIRC() {
        if (irc != null && irc.IsConnected) {
            irc.Quit("Application close");

			try {
				irc.Disconnect();
			}
			catch (Exception){				
			}
				
        }

		exited = true;
        yield break;
    }

	private IEnumerator OnTheConnect() {
		while (!irc.IsConnected || !channelsText["SERVER"].Contains("End of /MOTD command.")){
			connectionLog += ".";
			yield return new WaitForSeconds(0.25f);
		}

		if (sendingMessage){

			channelsText[usernameToMessage] = "";
			nextFrameChannel = usernameToMessage;
		} else {
			irc.Join(channel);
		}
	}

}