//--------------------------------------
//          Kulestar Unity HTTP
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnityHttp{
	
	/// <summary>A delegate for methods called when the http request is done or ready for use.</summary>
	public delegate void OnHttpEvent(HttpRequest request);
	
	/// <summary>
	/// Represents a single http request.
	/// </summary>

	public class HttpRequest{
		
		/// <summary>True if the request is ready (but not necessarily complete).</summary>
		public bool Ready;
		/// <summary>The url that was requested.</summary>
		public string Url;
		/// <summary>True when this request has started up.</summary>
		public bool Started;
		/// <summary>The unity WWW object which performs the underlying request.</summary>
		public WWW WWWRequest;
		/// <summary>Post data to send with the request. See AttachForm.</summary>
		public byte[] PostData;
		/// <summary>A custom object used for passing data to the callbacks.</summary>
		public object ExtraData;
		#if UNITY_WP8 || UNITY_METRO || UNITY_4_5 || UNITY_4_6
		/// <summary>The headers sent with this request.</summary>
		public Dictionary<string,string> Headers;
		#else
		/// <summary>The headers sent with this request.</summary>
		public Hashtable Headers;
		#endif
		#if !UNITY_IPHONE && !UNITY_ANDROID && !UNITY_BLACKBERRY && !UNITY_WP8
		/// <summary>The video being downloaded, if any. Note: Pro only.</summary>
		public MovieTexture Movie;
		#endif
		/// <summary>Active requests are in a linked list. The http request that follows this one.</summary>
		public HttpRequest RequestAfter;
		/// <summary>Active requests are in a linked list. The http request that is before this one.</summary>
		public HttpRequest RequestBefore;
		/// <summary>The enumerator which processes the unity WWW object.</summary>
		private IEnumerator LoadingEnumerator;
		/// <summary>An event called when the request is completely finished.</summary>
		public event OnHttpEvent OnRequestDone;
		/// <summary>An event called when the data is ready for use (e.g. videos).</summary>
		public event OnHttpEvent OnRequestReady;
		
		
		/// <summary>Creates a new http request to the given url.</summary>
		/// <param name="url">The url to request.</param>
		/// <param name="onDone">A method to call with the result.</param>
		public HttpRequest(string url,OnHttpEvent onDone){
			Url=url;
			OnRequestDone+=onDone;
		}
		
		/// <summary>Creates a new http request to the given URL.</summary>
		/// <param name="url">The url to request.</param>
		public HttpRequest(string url){
			Url=url;
		}
		
		/// <summary>Adds the given form to this request.
		/// Note that if you wish to also use custom headers with a form, call this first.
		/// Then, add to the Headers property.</summary>
		/// <param name="form">The form to attach to this request.</param>
		public void AttachForm(WWWForm form){
			PostData=form.data;
			
			#if !UNITY_WP8 && !UNITY_METRO && (UNITY_4_5 || UNITY_4_6)
			Headers=ToDictionary(form.headers);
			#else
			Headers=form.headers;
			#endif
		}
		
		#if !UNITY_WP8 && !UNITY_METRO
		/// <summary>Unavailable on Windows 8. Sets the headers using a hashtable set.</summary>
		/// <param name="headers">The headers to set.</param>
		[Obsolete("Depreciated as of Unity 4.5+. Use a Dictionary<string,string> instead.")]
		public void SetHeaders(Hashtable headers){
			#if UNITY_4_5 || UNITY_4_6
			
			if(headers==null){
				Headers=null;
				return;
			}
			
			Headers=ToDictionary(headers);
			
			#else
			Headers=headers;
			#endif
		}
		
		/// <summary>Converts a hashtable to a dictionary.</summary>
		private Dictionary<string,string> ToDictionary(Hashtable headers){
			
			Dictionary<string,string> result=new Dictionary<string,string>();
			
			foreach(string key in headers.Keys){
				result[key]=(string)headers[key];
			}
			
			return result;
		}
		#endif
		
		/// <summary>Sets the headers using a generic dictionary set.</summary>
		/// <param name="headers">The headers to set.</param>
		public void SetHeaders(Dictionary<string,string> headers){
			#if UNITY_WP8 || UNITY_METRO || UNITY_4_5 || UNITY_4_6
			Headers=headers;
			#else
			Headers=new Hashtable(headers);
			#endif
		}
		
		/// <summary>Sets the PostData from the given UTF8 string.</summary>
		/// <param name="toPost">The string to POST.</param>
		public void SetPost(string toPost){
			PostData=System.Text.Encoding.UTF8.GetBytes(toPost);
		}
		
		/// <summary>Sends this request.
		/// Note that this does not block. Instead, OnRequestDone will be called when it's done.</summary>
		public void Send(){
			if(PostData==null && Headers==null){
				WWWRequest=new WWW(Url);
			}else{
				WWWRequest=new WWW(Url,PostData,Headers);
			}
			
			#if !UNITY_IPHONE && !UNITY_ANDROID && !UNITY_BLACKBERRY && !UNITY_WP8
			if(Url.EndsWith(".ogg")){
				Movie=WWWRequest.movie;
			}
			#endif
			
			// Let's go!
			LoadingEnumerator=Loader();
			
			// Push this onto the front of our update queue:
			Http.Queue(this);
		}
		
		/// <summary>Removes this request from the active linked list. It won't be updated anymore.</summary>
		public void Remove(){
			Remove(false);
		}
		
		/// <summary>Removes this request from a linked list.</summary>
		/// <param name="waitingList">True if it should be removed from the waiting queue; false for the active queue.</param>
		public void Remove(bool waitingList){
			if(RequestBefore==null){
				if(waitingList){
					Http.FirstWaitingRequest=RequestAfter;
				}else{
					Http.FirstRequest=RequestAfter;
				}
			}else{
				RequestBefore.RequestAfter=RequestAfter;
			}
			
			if(RequestAfter==null){
				if(waitingList){
					Http.LastWaitingRequest=RequestBefore;
				}else{
					Http.LastRequest=RequestBefore;
				}
			}else{
				RequestAfter.RequestBefore=RequestBefore;
			}
			
			if(waitingList){
				RequestAfter=null;
				RequestBefore=null;
			}else{
				Http.CurrentActiveRequests--;
				Http.UpdateWaitingList();
			}
		}
		
		/// <summary>True if the request had an issue. <see cref="PowerUI.HttpRequest.Error"/> is the error.</summary>
		public bool Errored{
			get{
				return (Error!=null);
			}
		}
		
		/// <summary>True if there was no issues.</summary>
		public bool Ok{
			get{
				return (Error==null);
			}
		}
		
		/// <summary>The error, if any, that occured whilst attempting to load the url.</summary>
		public string Error{
			get{
				return WWWRequest.error;
			}
		}
		
		/// <summary>The method which performs the loading of the unity WWW object.</summary>
		public IEnumerator Loader(){
			yield return WWWRequest;
		}
		
		/// <summary>The response as text. Empty string if there was an error. </summary>
		public string Text{
			get{
				if(Errored){
					return "";
				}
				return WWWRequest.text;
			}
		}
		
		/// <summary>The raw bytes of the response. Null if there was an error.</summary>
		public byte[] Bytes{
			get{
				if(Errored){
					return null;
				}
				return WWWRequest.bytes;
			}
		}
		
		#if !UNITY_IPHONE && !UNITY_ANDROID && !UNITY_BLACKBERRY && !UNITY_WP8
		/// <summary>The response as a video. Null if there was an error.</summary>
		public MovieTexture Video{
			get{
				if(Errored){
					return null;
				}
				return Movie;
			}
		}
		#endif
		
		/// <summary>The response as an image. Null if there was an error.</summary>
		public Texture2D Image{
			get{
				if(Errored){
					return null;
				}
				return WWWRequest.texture;
			}
		}
		
		/// <summary>The current download progress.</summary>
		public float Progress{
			get{
				return WWWRequest.progress;
			}
		}
		
		/// <summary>Advances this request by checking in on it's progress.</summary>
		public void Update(){
			if(!Started){
				Started=true;
				LoadingEnumerator.MoveNext();
			}
			if(WWWRequest.isDone){
				try{
					if(OnRequestDone!=null){
						if(Errored){
							// Log the error for clarity:
							Wrench.Log.Add("HTTP ERROR: "+Error);
						}
						OnRequestDone(this);
					}
				}catch(Exception e){
					Wrench.Log.Add("Error in HTTP DONE function when loading url '"+Url+"' - "+e);
				}
				
				// Pop it from the update queue:
				Remove();
			#if !UNITY_IPHONE && !UNITY_ANDROID && !UNITY_BLACKBERRY && !UNITY_WP8
			}else if(!Ready && Movie!=null && Movie.isReadyToPlay){
				// Downloaded it far enough to try playing it.
				Ready=true;
				try{
					if(OnRequestReady!=null){
						OnRequestReady(this);
					}
				}catch(Exception e){
					Wrench.Log.Add("Error in HTTP READY function when loading url '"+Url+"' - "+e);
				}
			#endif
			}
		}
		
	}
	
}