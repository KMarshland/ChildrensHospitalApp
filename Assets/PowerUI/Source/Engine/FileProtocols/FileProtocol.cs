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
using PowerUI.Css;

namespace PowerUI{
	
	/// <summary>
	/// Represents a custom protocol:// as used by PowerUI files.
	/// For example, if you wish to deliver content in a custom way to PowerUI, implement a new FileProtocol (e.g. 'cdn')
	/// Then, setup its OnGetGraphic function.
	/// </summary>
	
	public class FileProtocol{
		
		/// <summary>Should images delivered with this protocol append resolution based names?</summary>
		public bool UseResolution=true;
		
		
		/// <summary>Returns all protocol names:// that can be used for this protocol.
		/// e.g. new string[]{"cdn","net"}; => cdn://file.png or net://file.png</summary>
		public virtual string[] GetNames(){
			return null;
		}
		
		/// <summary>Does the item at the given location have full access to the code security domain?
		/// Used by Nitro. If it does not have full access, the Nitro security domain is asked about the path instead.
		/// If you're unsure, leave this false! If your game uses simple web browsing, 
		/// this essentially stops somebody writing dangerous Nitro on a remote webpage and directing your game at it.</summary>
		public virtual bool FullAccess(FilePath path){
			return false;
		}
		
		/// <summary>Get the file at the given path as some html text using this protocol.
		/// Once it's been retrieved, this must call package.GotText(theText) internally.</summary>
		public virtual void OnGetText(TextPackage package,FilePath path){}
		
		/// <summary>Get the file at the given path as a MovieTexture (Unity Pro only!), Texture2D,
		/// SPA Animation or DynamicTexture using this protocol.
		/// Once it's been retrieved, this must call package.GotGraphic(theObject) internally.</summary>
		public virtual void OnGetGraphic(ImagePackage package,FilePath path){}
		
		/// <summary>Submits the given form to the given path using this protocol.</summary>
		public virtual void OnPostForm(FormData form,Element formElement,FilePath path){}
		
		/// <summary>The user clicked on the given link which points to the given path.</summary>
		public virtual void OnFollowLink(Element linkElement,FilePath path){}
		
	}
	
}