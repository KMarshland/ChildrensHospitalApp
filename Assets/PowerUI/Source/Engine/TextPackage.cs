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
	
	/// <summary>A delegate used for callbacks when text is done loading and can now be used.</summary>
	public delegate void OnTextReady(TextPackage package);
	
	/// <summary>
	/// Retrieves a block of text for use on the UI.
	/// </summary>
	
	public class TextPackage{
		
		/// <summary>The text that was retrieved. You must check if it is <see cref="PowerUI.TextPackage.Ok"/> first.</summary>
		public string Text;
		/// <summary>Any error that occured whilst retrieving the text.</summary>
		public string Error;
		/// <summary>The file that was requested.</summary>
		public FilePath File;
		/// <summary>The type of file that was requested (e.g. "txt" or "html")</summary>
		public string FileType;
		/// <summary>A custom data object for passing anything else when the callback runs.</summary>
		public object ExtraData;
		/// <summary>The callback to run when the text has been retrieved (or an error occured).</summary>
		public event OnTextReady TextReady;
		
		
		/// <summary>Creates a new text package for the named file to get.
		/// You must then call <see cref="PowerUI.TextPackage.Get"/> to perform the request.</summary>
		/// <param name="src">The file to get.</param>
		/// <param name="relativeTo">The path the file to get is relative to, if any (may be null).</param>
		public TextPackage(string src,string relativeTo){
			SetPath(src,relativeTo);
		}
		
		/// <summary>Sends the request off and defines a callback to run when the result is ready.</summary>
		/// <param name="textReady">The callback to run when the text has been retrieved.
		/// Note that the callback must check if the result is <see cref="PowerUI.TextPackage.Ok"/>.</param>
		public void Get(OnTextReady textReady){
			TextReady+=textReady;
			if(string.IsNullOrEmpty(Url)){
				GotText("");
				return;
			}
			
			// Do we have a file protocol handler available?
			FileProtocol fileProtocol=File.Handler;
			
			if(fileProtocol!=null){
				fileProtocol.OnGetText(this,File);
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
		
		/// <summary>Got text is called by the file handler when the response is received.</summary>
		/// <param name="text">The text that was recieved.</param>
		public void GotText(string text){
			GotText(text,null);
		}
		
		/// <summary>Got text is called by the file handler when the response is received.</summary>
		/// <param name="text">The text that was recieved, if any.</param>
		/// <param name="error">An error that occured, if any.</param>
		public void GotText(string text,string error){
			Error=error;
			Text=text;
			if(TextReady!=null){
				TextReady(this);
			}
		}
		
		/// <summary>True if there is no error and the text is ok.</summary>
		public bool Ok{
			get{
				return (Error==null);
			}
		}
		
		/// <summary>True if there was an error and the text is not ok.</summary>
		public bool Errored{
			get{
				return (Error!=null);
			}
		}
		
	}
	
}