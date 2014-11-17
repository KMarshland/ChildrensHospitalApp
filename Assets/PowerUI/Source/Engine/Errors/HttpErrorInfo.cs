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

using System;
using UnityHttp;



namespace PowerUI{
	
	/// <summary>
	/// Contains information about a HTTP protocol error, such as a 404 page not found.
	/// </summary>
	
	public class HttpErrorInfo:ErrorInfo{
		
		/// <summary>The Http Request.</summary>
		public HttpRequest Request;
		
		
		public HttpErrorInfo(HttpRequest request){
			
			// Store the request:
			Request=request;
			
			// Grab the URL:
			Url=new FilePath(request.Url,"");
			
			// Grab the message:
			Message=request.Error;
			
		}
		
	}
	
}