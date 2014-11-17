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


namespace PowerUI{
	
	/// <summary>Helps display more friendly error results.</summary>
	/// <param name="error">Contains information about the error itself.</param>
	/// <param name="document">The document which should display the error. Note that this can be null if you use target="_blank" (outside Unity).</param>
	public delegate void ErrorHandler(ErrorInfo error,Document document);
	
	/// <summary>
	/// Manages events such as 404 pages.
	/// </summary>
	
	public static class ErrorHandlers{
		
		/// <summary>Used when a http:// or resources file link errors. This may be due to a 404/ file not found or network down.</summary>
		public static ErrorHandler PageNotFound;
		
	}
	
}