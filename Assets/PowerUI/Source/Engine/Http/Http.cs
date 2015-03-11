//--------------------------------------
//          Kulestar Unity HTTP
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using UnityEngine;

namespace UnityHttp{
	
	/// <summary>
	/// Performs Http requests independently.
	/// Note that you must call Update to keep all the requests active.
	/// </summary>
	
	public static class Http{
		
		/// <summary>The time since the last update in seconds.</summary>
		public static float UpdateTimer;
		/// <summary>The time in seconds between updates.</summary>
		private static float TimerLimit=0.1f;
		/// <summary>Active http requests are stored in a linked list. This is the tail of the list.</summary>
		public static HttpRequest LastRequest;
		/// <summary>Active http requests are stored in a linked list. This is the head of the list.</summary>
		public static HttpRequest FirstRequest;
		/// <summary>The amount of requests that are currently active.</summary>
		public static int CurrentActiveRequests;
		/// <summary>Waiting http requests are stored in a linked list - added here when MaxSimultaneousRequests is exceeded. 
		/// This is the tail of the list.</summary>
		public static HttpRequest LastWaitingRequest;
		/// <summary>Waiting http requests are stored in a linked list - added here when MaxSimultaneousRequests is exceeded. 
		/// This is the head of the list.</summary>
		public static HttpRequest FirstWaitingRequest;
		/// <summary>The maximum amount of simultaneous requests that will be allowed.
		/// Exceeding this will result in the additional requests entering the waiting list. -1 (default) is no limit.</summary>
		public static int MaxSimultaneousRequests=-1;
		
		/// <summary>Sets the rate at which http requests are advanced.</summary>
		/// <param name="fps">The rate in frames per second.</param>
		public static void SetRate(int fps){
			if(fps<=0){
				// Default rate is 10fps.
				fps=10;
			}
			TimerLimit=1f/(float)fps;
		}
		
		/// <summary>Clears all active requests.</summary>
		public static void Clear(){
			FirstRequest=LastRequest=null;
		}
		
		/// <summary>Requests a file over the internet.</summary>
		/// <param name="url">The url to request.</param>
		public static void Request(string url){
			Request(url,null,null);
		}
		
		/// <summary>Requests a file over the internet.</summary>
		/// <param name="url">The url to request.</param>
		/// <param name="onDone">A method to call with the result.</param>
		public static void Request(string url,OnHttpEvent onDone){
			Request(url,onDone,null);
		}
		
		/// <summary>Requests a file over the internet.</summary>
		/// <param name="url">The url to request.</param>
		/// <param name="onDone">A method to call with the result.</param>
		/// <param name="extraData">An object which will be available in the onDone method.</param>
		public static void Request(string url,OnHttpEvent onDone,object extraData){
			HttpRequest request=new HttpRequest(url,onDone);
			request.ExtraData=extraData;
			request.Send();
		}
		
		/// <summary>Requests a file over the internet and posts form data to it.</summary>
		/// <param name="url">The url to request.</param>
		/// <param name="onDone">A method to call with the result.</param>
		/// <param name="form">A http form to send with the request.</param>
		public static void Request(string url,OnHttpEvent onDone,WWWForm form){
			HttpRequest request=new HttpRequest(url,onDone);
			
			if(form!=null){
				request.AttachForm(form);
			}
			
			request.Send();
		}
		
		/// <summary>Encodes the given piece of text so it's suitable to go into a post or get string.</summary>
		/// <param name="text">The text to encode.</param>
		/// <returns>The url encoded text.</returns>
		public static string UrlEncode(string text){
			return System.Uri.EscapeDataString(text);
		}
		
		/// <summary>Metered. Advances all the currently active http requests.</summary>
		public static void Update(){
			UpdateTimer+=Time.deltaTime;
			if(UpdateTimer<TimerLimit){
				return;
			}
			Flush();
		}
		
		/// <summary>Advances all the currently active http requests.</summary>
		public static void Flush(){
			UpdateTimer=0f;
			if(FirstRequest==null){
				return;
			}
			HttpRequest current=FirstRequest;
			while(current!=null){
				current.Update();
				current=current.RequestAfter;
			}
		}
		
		public static void UpdateWaitingList(){
			// Dequeue from the waiting list.
			if(FirstWaitingRequest==null){
				return;
			}
			
			// Grab the one at the front:
			HttpRequest waiting=FirstWaitingRequest;
			// Pop it from the list:
			waiting.Remove(true);
			// And queue it up in the main list:
			Queue(waiting);
		}
		
		public static void Queue(HttpRequest request){
			
			if(MaxSimultaneousRequests!=-1 && CurrentActiveRequests==MaxSimultaneousRequests){
				// Add to the waiting list:
				if(FirstWaitingRequest==null){
					LastWaitingRequest=FirstWaitingRequest=request;
				}else{
					request.RequestBefore=LastWaitingRequest;
					LastWaitingRequest=LastWaitingRequest.RequestAfter=request;
				}
				// The waiting list will be updated when an active request completes.
			}else{
				// Bump up the active count:
				CurrentActiveRequests++;
				
				// Add to main queue:
				if(FirstRequest==null){
					LastRequest=FirstRequest=request;
				}else{
					request.RequestBefore=LastRequest;
					LastRequest=LastRequest.RequestAfter=request;
				}
				
			}
			
		}
		
	}
	
}