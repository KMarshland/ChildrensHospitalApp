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

using Nitro;
using System;
using UnityHttp;

namespace PowerUI{

	/// <summary>
	/// Wraps around the UnityHttp.Http class so Nitro can easily perform web requests.
	/// </summary>

	public static class Ajax{
		
		/// <summary>Performs a simple get request, calling the callback with the result.</summary>
		/// <param name="url">The URL to request.</param>
		/// <param name="callback">The callback to run when the request completes.</param>
		public static void Get(string url,DynamicMethod<Nitro.Void> callback){
			Get(url,callback,null);
		}
		
		/// <summary>Performs a simple get request, calling the callback with the result.</summary>
		/// <param name="url">The URL to request.</param>
		/// <param name="callback">The callback to run when the request completes.</param>
		/// <param name="extraData">A custom object to pass to the callback when the request completes.</param>
		public static void Get(string url,DynamicMethod<Nitro.Void> callback,object extraData){
			HttpRequest request=new HttpRequest(url,OnAjaxDone);
			request.ExtraData=new object[]{callback,extraData};
			request.Send();
		}
		
		/// <summary>Performs a post request, sending the given post data.</summary>
		/// <param name="url">The URL to request.</param>
		/// <param name="postData">The data to send (as a UTF8 string).</param>
		public static void Post(string url,string postData){
			Post(url,postData,null,null);
		}
		
		/// <summary>Performs a post request, sending the given post data.</summary>
		/// <param name="url">The URL to request.</param>
		/// <param name="postData">The data to send (as a UTF8 string).</param>
		/// <param name="callback">The callback to run when the request completes.</param>
		public static void Post(string url,string postData,DynamicMethod<Nitro.Void> callback){
			Post(url,postData,callback,null);
		}
		
		/// <summary>Performs a post request, sending the given post data.</summary>
		/// <param name="url">The URL to request.</param>
		/// <param name="postData">The data to send (as a UTF8 string).</param>
		/// <param name="callback">The callback to run when the request completes.</param>
		/// <param name="extraData">A custom object to pass to the callback when the request completes.</param>
		public static void Post(string url,string postData,DynamicMethod<Nitro.Void> callback,object extraData){
			HttpRequest request=new HttpRequest(url,OnAjaxDone);
			request.ExtraData=new object[]{callback,extraData};
			request.PostData=System.Text.Encoding.UTF8.GetBytes(postData);
			request.Send();
		}
		
		/// <summary>The callback used to process a completed request.</summary>
		/// <param name="request">The HttpRequest that has now completed.</param>
		private static void OnAjaxDone(HttpRequest request){
			object[] extraData=(object[])request.ExtraData;
			DynamicMethod<Nitro.Void> callback=(DynamicMethod<Nitro.Void>)extraData[0];
			
			request.ExtraData=extraData[1];
			
			if(callback!=null){
				callback.Run(request);
			}
		}
		
	}
	
}