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
using System.Threading;

namespace PowerUI{
	
	/// <summary>
	/// This callback is used to ensure that resources protocol content is loaded on the main thread.
	/// </summary>
	
	public class ResourcesProtocolCallback:Callback{
		
		/// <summary>The path of the file to load.</summary>
		public FilePath Path;
		/// <summary>A text package for the actively loading text.</summary>
		public TextPackage Text;
		/// <summary>A data package for the actively loading raw data.</summary>
		public DataPackage Data;
		/// <summary>An image package for the actively loading image.</summary>
		public ImagePackage Images;
		/// <summary>Fast reference to the resources:// protocol handler. Could alternatively use FileProtocols.Get.</summary>
		public FileProtocol Protocol;
		
		
		public ResourcesProtocolCallback(DataPackage package,FilePath path){
			Data=package;
			Path=path;
		}
		
		public ResourcesProtocolCallback(TextPackage package,FilePath path){
			Text=package;
			Path=path;
		}
		
		public ResourcesProtocolCallback(ImagePackage package,FilePath path){
			Images=package;
			Path=path;
		}
		
		public override void OnRun(){
			
			// Simply run it again - we know for sure we're on the main thread, so this can't go recursive.
			
			if(Text!=null){
				
				// This was a text callback.
				Protocol.OnGetText(Text,Path);
			
			}else if(Data!=null){
				
				// This was a data callback.
				Protocol.OnGetData(Data,Path);
				
			}else{
				
				// This is an image callback.
				Protocol.OnGetGraphic(Images,Path);
				
			}
			
		}
		
	}
	
}