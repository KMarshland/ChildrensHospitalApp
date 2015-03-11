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
	/// A html element which represents a single word.
	/// </summary>
	
	public class WordElement:Element{
		
		/// <summary>Creates a new html word element.</summary>
		/// <param name="document">The document this word will belong to.</param>
		/// <param name="parent">The parent html element. Should be a TextElement.</param>
		/// <param name="text">The text of this word.</param>
		public WordElement(Document document,Element parent,string text):base(document,parent){
			SetTag("word");
			
			// Words are CSS driven:
			style.innerText=text;
		}
		
		/// <summary>The letter count.</summary>
		/// <returns>The number of letters in this word.</returns>
		public int LetterCount(){
			return Style.Computed.Text.LetterCount();
		}
		
		/// <summary>Gets this word as a string.</summary>
		/// <returns>The word as a string.</returns>
		public override string ToString(){
			return Style.Computed.Text.ToString();
		}
		
		public override void ToString(System.Text.StringBuilder builder){
			Style.Computed.Text.ToString(builder);
		}
		
	}
	
}