using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// User - interface layer
/// </summary>
public class InGameChatUI : MonoBehaviour {
#if false
    public InGameChat irc = null;
    public UILabel text = null; // Reference on text object, used to add and format messages
    public UIInput input = null; // Input control
    public UIScrollBar scroll = null;
    public UISprite background = null;
    public UIPanel rootPanel = null;  // used to collapse animation
    public AudioSource messageAudio = null; 
    public BoxCollider topCollider = null; // used to block user input then chat is collapsed

    private float startBackgroundAlpha;
    private float topColliderStartY;
    private Vector4 startPanelClip;
    private bool dragging = false;
    private bool hovered = false;

    /// <summary>
    /// Update text collider according to the text size
    /// </summary>
    private void UpdateTextCollider(){
        BoxCollider box = text.collider as BoxCollider;
        Bounds textBounds = NGUIMath.CalculateRelativeWidgetBounds(text.transform);
        Bounds scrollBounds = NGUIMath.CalculateRelativeWidgetBounds(scroll.transform);
        box.center = textBounds.center;
        box.size = textBounds.extents;

        if (scroll.barSize < 1.0f) {
            Vector3 b = scrollBounds.extents / 2.0f;

            b.y=b.z=0.0f;
            box.center -=  b / 4;
            box.size -= b / 2; 
        }
    }

    void Start() {        
        UpdateTextCollider();

        text.panel.clipping = UIDrawCall.Clipping.SoftClip;
        text.panel.clipSoftness = new Vector2(0.0f,5.0f);

        topColliderStartY = topCollider.transform.position.y;
        startBackgroundAlpha = background.alpha;
        startPanelClip = rootPanel.clipRange;

        UIEventListener.Get(text.gameObject).onHover += TextHover;
        UIEventListener.Get(input.gameObject).onHover += TextHover;
        UIEventListener.Get(text.gameObject).onDrag += TextDrag;
    }

    void TextHover(GameObject go,bool value) {
        hovered = value;
    }

    void TextDrag(GameObject go,Vector2 vec) {
        dragging = true;
    }

    private IEnumerator SelectRoutine() {
        yield return new WaitForSeconds(0.01f);
        input.selected = true;
        yield break;
    }    

	void Update () {
        // Select input control on [enter] button
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
            if (!input.selected) {
                StartCoroutine(SelectRoutine());
            }
        }

        // Keep alpha and clip values, when contorl is hovered or draggin
        if (input.selected || dragging || hovered) {
            topCollider.transform.position = new Vector3(topCollider.transform.position.x,topColliderStartY,topCollider.transform.position.z);
            background.alpha = startBackgroundAlpha;
            if (startPanelClip.w != rootPanel.clipRange.w) {
                float delta = startPanelClip.w - rootPanel.clipRange.w;
                rootPanel.clipRange = new Vector4(rootPanel.clipRange.x,rootPanel.clipRange.y + delta / 2.0f,rootPanel.clipRange.z,rootPanel.clipRange.w + delta);
                text.panel.clipRange = new Vector4(text.panel.clipRange.x,text.panel.clipRange.y + delta / 2.0f,text.panel.clipRange.z,text.panel.clipRange.w + delta);
            }
            dragging = false;
        }
        else { // Fade out slowly
            background.alpha -= Time.deltaTime/3.0f;
            background.alpha = Mathf.Clamp(background.alpha,0,255);

            if (text.panel.clipRange.w > 75.0f) {
                float clipValue = Time.deltaTime*50.0f;
                topCollider.transform.localPosition = new Vector3(topCollider.transform.localPosition.x,topCollider.transform.localPosition.y - clipValue,topCollider.transform.localPosition.z);
                rootPanel.clipRange = new Vector4(rootPanel.clipRange.x,rootPanel.clipRange.y - clipValue/2.0f,rootPanel.clipRange.z,rootPanel.clipRange.w - clipValue);
                text.panel.clipRange = new Vector4(text.panel.clipRange.x,text.panel.clipRange.y - clipValue / 2.0f,text.panel.clipRange.z,text.panel.clipRange.w - clipValue);
            }
        }
	}

    /// <summary>
    /// Add colored chat message nicname and message have different colors
    /// </summary>
    /// <param name="nickname">User's nickname</param>
    /// <param name="message">Chat message</param>
    public void AddChatMessage(string nickname,string message) {
        if (!string.IsNullOrEmpty(text.text))
            text.text += "\r\n";
       
        text.text += string.Format("[00FF00]{0}:[FFFFFF] {1}",nickname,message);
        scroll.scrollValue = 1.0f;
        UpdateTextCollider();
        if (audio != null)
            audio.Play();
    }

    /// <summary>
    /// Add colored system message
    /// </summary>
    /// <param name="message"></param>
    /// <param name="color"></param>
    public void AddSystemMessage(string message,string color){
        if (!string.IsNullOrEmpty(text.text))
            text.text += "\r\n";

        text.text += string.Format("[{0}]{1}",color,message);
        scroll.scrollValue = 1.0f;
        UpdateTextCollider();
        if (audio != null)
            audio.Play();
    }
    
    public void AddConnectedMessage(string message) {
        AddSystemMessage(message,"FFC90E");
    }

    public void AddErrorMessage(string message) {
        AddSystemMessage(message,"FF1010");
    }

    public void AddWarningMessage(string message) {
        AddSystemMessage(message,"C3C3C3");
    }

    public void AddKillMessage(string message) {
        AddSystemMessage(message,"FFC90E");
    }
    
    void OnSubmit(){
        if (input.text != "") {
            if (irc.SendChatMessage(input.text)) {
                AddChatMessage(irc.userName,input.text);
                input.text = "";
            }
        }
    }
#endif
}