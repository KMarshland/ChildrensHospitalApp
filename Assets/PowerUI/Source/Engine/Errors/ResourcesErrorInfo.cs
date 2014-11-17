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
	
	/// <summary>
	/// Contains information about a protocol error such as file not found. Applies to Resources and Http requests.
	/// </summary>
	
	public class FileErrorInfo:ErrorInfo{
		
		/// <summary>The text package essentially containing the request.</summary>
		public TextPackage Package;
		
		
		public FileErrorInfo(TextPackage package){
			
			// Store the package:
			Package=package;
			
			// Grab the URL:
			Url=new FilePath(package.Url,"");
			
			// Grab the message:
			Message=package.Error;
			
		}
		
	}
	
}