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
	
	/// <summary>A delegate used for callbacks when generic binary is done loading and can now be used.</summary>
	public delegate void OnDataReady(DataPackage package);
	
	/// <summary>
	/// Retrieves a block of binary data for use on the UI. Used by e.g. fonts.
	/// </summary>
	
	public class DataPackage{
		
		/// <summary>The data that was retrieved. You must check if it is Ok first.</summary>
		public byte[] Data;
		/// <summary>Any error that occured.</summary>
		public string Error;
		/// <summary>The file that was requested.</summary>
		public FilePath File;
		/// <summary>The type of file that was requested (e.g. "woff" or "ttf")</summary>
		public string FileType;
		/// <summary>A custom data object for passing anything else when the callback runs.</summary>
		public object ExtraData;
		/// <summary>The callback to run when complete.</summary>
		public event OnDataReady Ready;
		
		
		/// <summary>Creates a new package to get the named file.
		/// You must then call Get to perform the request.</summary>
		/// <param name="src">The file to get.</param>
		/// <param name="relativeTo">The path the file to get is relative to, if any (may be null).</param>
		public DataPackage(string src,string relativeTo){
			SetPath(src,relativeTo);
		}
		
		/// <summary>Sends the request off and defines a callback to run when the result is ready.</summary>
		/// <param name="ready">The callback to run.
		/// Note that the callback must check if the result is Ok.</param>
		public void Get(OnDataReady ready){
			Ready+=ready;
			if(string.IsNullOrEmpty(Url)){
				GotData(null,"Empty path!");
				return;
			}
			
			// Do we have a file protocol handler available?
			FileProtocol fileProtocol=File.Handler;
			
			if(fileProtocol!=null){
				fileProtocol.OnGetData(this,File);
			}
		}
		
		/// <summary>The fully resolved URL requested.</summary>
		public string Url{
			get{
				return File.Url;
			}
		}
		
		/// <summary>Sets up the filepath to the given url which may be relative to a given location.</summary>
		/// <param name="src">The file to get.</param>
		/// <param name="relativeTo">The path the file to get is relative to, if any. May be null.</param>
		private void SetPath(string src,string relativeTo){
			File=new FilePath(src,relativeTo,false);
			FileType=File.Filetype.ToLower();
		}
		
		/// <summary>Got data is called by the file handler when the response is received.</summary>
		/// <param name="data">The data that was recieved.</param>
		public void GotData(byte[] data){
			GotData(data,null);
		}
		
		/// <summary>Got data is called by the file handler when the response is received.</summary>
		/// <param name="data">The data that was recieved, if any.</param>
		/// <param name="error">An error that occured, if any.</param>
		public void GotData(byte[] data,string error){
			Error=error;
			Data=data;
			if(Ready!=null){
				Ready(this);
			}
		}
		
		/// <summary>True if there is no error and the data is ok.</summary>
		public bool Ok{
			get{
				return (Error==null);
			}
		}
		
		/// <summary>True if there was an error and the data is not ok.</summary>
		public bool Errored{
			get{
				return (Error!=null);
			}
		}
		
	}
	
}