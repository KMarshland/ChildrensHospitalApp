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
using UnityEngine;


namespace PowerUI{
	
	/// <summary>
	/// Wraps copy/paste functionality into a simple interface.
	/// </summary>
	
	public static class Clipboard{
		
		/// <summary>Pastes the text from the clipboard.</summary>
		public static string Paste(){
			TextEditor editor=new TextEditor();
			editor.content.text="";
			editor.Paste();
			return editor.content.text;
		}
		
		/// <summary>Copies the given text onto the system clipboard.</summary>
		public static void Copy(string text){
			TextEditor editor=new TextEditor();
			editor.content.text=text;
			editor.SelectAll();
			editor.Copy();
		}
		
	}
	
}