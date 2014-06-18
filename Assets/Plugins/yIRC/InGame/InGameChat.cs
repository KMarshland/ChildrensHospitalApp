using UnityEngine;
using System;
using System.Collections;
using yIRC;

/// <summary>
/// Network - communication layer
/// </summary>
public class InGameChat : MonoBehaviour {
#if false
    public string serverName = "irc.freenode.net";
    public string userName = "yIRC_Client";
    public string channel = "#yIRC";
    public int port = 6667;
    public InGameChatUI uiChat = null;
    public IrcClient irc = null;

    /// <summary>
    /// Safe send message
    /// </summary>
    /// <param name="message">message to be sent</param>
    /// <returns>true if message sucessfully sent, else otherwise</returns>
    public bool SendChatMessage(string message) {
        if (irc != null && irc.IsConnected && channel!="") {
            irc.SendMessage(SendType.Message,channel,message);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Connect to the server; Subscribe to important events, Join channel
    /// </summary>
    public void Connect() {
        if (irc != null && irc.IsConnected)
            irc.Disconnect();

        irc = new IrcClient();
        irc.Encoding = System.Text.Encoding.UTF8;
        irc.SendDelay = 200; // set send delay (flood protection)
        irc.ActiveChannelSyncing = true; 

        irc.OnError += irc_OnError;
        irc.OnRawMessage += irc_OnRawMessage;
        irc.OnJoin += irc_OnJoin;
        irc.OnConnected += irc_OnConnected;

        try {
            uiChat.AddConnectedMessage("Connecting...");
            irc.Connect(serverName,port);
        }
        catch (ConnectionException e) {
            Log("Couldn't connect! Reason: " + e.Message);
        }

        try {
            uiChat.AddConnectedMessage("Loging in...");
            irc.Login(userName,"Player1",0,userName);

            uiChat.AddConnectedMessage("Entering channel...");
            irc.Join(channel);
        }
        catch (ConnectionException e) {
            Log(e.Message);
        }
        catch (Exception e) {
            Log("Error occurred! Message: " + e.Message);
            Log("Exception: " + e.StackTrace);
        }
    }

    void irc_OnConnected(object sender,EventArgs e) {
        uiChat.AddConnectedMessage("Connected");
    }

    void irc_OnJoin(object sender,JoinEventArgs e) {
        uiChat.AddConnectedMessage("Entered channel "+e.Channel);
    }
  
    void Update() {
        // Update irc: send and receive messages
        if (irc != null && irc.IsConnected)
            irc.ListenOnce(false);
    }

    void OnGUI() {
        if (irc==null || !irc.IsConnected) {
            Rect r = new Rect(5,5,75,25);
            if (GUI.Button(r,"Connect")) {
                Connect();
            }
        }
    }
    
    void Log(string message) {
        Debug.Log(message);
    }

    /// <summary>
    /// Handle all messages here. 
    /// For In-Game chat simply log all messages and show only messages for selected channel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void irc_OnRawMessage(object sender,IrcEventArgs e) {
        if (!string.IsNullOrEmpty(e.Data.Nick) && e.Data.Nick != userName && e.Data.Type == ReceiveType.ChannelMessage && e.Data.Channel == channel) 
            uiChat.AddChatMessage(e.Data.Nick,e.Data.Message);
        else 
            Log(e.Data.Message);
    }

    void irc_OnError(object sender,ErrorEventArgs e) {
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
            yield return new WaitForSeconds(1.0f);
            irc.Disconnect();
        }
        yield break;
    }
#endif
}
