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
using Wrench;
using UnityEngine;


namespace PowerUI{
	
	/// <summary>
	/// A html element which represents renderable text.
	/// The children of this element should always be a <see cref="PowerUI.WordElement"/>.
	/// </summary>
	
	public class TextElement:Element,MLTextElement{
		
		/// <summary>A temporary store for words that are being parsed from the text.</summary>
		private string Text="";
		
		/// <summary>Creates a new text element that belongs to the given document.</summary>
		/// <param name="document">The document this element belongs to.</param>
		/// <param name="parent">The parent element for this new element.</param>
		public TextElement(Document document,Element parent):base(document,parent){
			SetTag("span");
		}
	
		/// <summary>Adds a character to the temporary stored word.</summary>
		public void AddCharacter(char character){
			Text+=character;
			if(character==' '){
				// Generate a new word:
				DoneWord(false);
			}
		}
		
		/// <summary>Finds the index of the nearest character to x pixels.</summary>
		/// <param name="x">The number of pixels from the left edge of this text element.</param>
		/// <param name="y">The number of pixels from the top edge of this text element.</param>
		/// <returns>The index of the nearest letter.</returns>
		public int LetterIndex(int x,int y){
			if(ChildNodes==null){
				return 0;
			}
			
			int widthSoFar=0;
			int lettersSoFar=0;
			
			for(int i=0;i<ChildNodes.Count;i++){
				WordElement word=(WordElement)ChildNodes[i];
				// Grab the words computed style:
				Css.ComputedStyle wordStyle=word.Style.Computed;
				// Find where the bottom of the line is at:
				int lineBottom=wordStyle.PixelHeight+wordStyle.ParentOffsetTop;
				
				if(lineBottom<y){
					// Not at the right line yet.
					lettersSoFar+=word.LetterCount();
					continue;
				}
				
				// Crank over the width:
				widthSoFar+=wordStyle.PixelWidth;
				
				
				if(x<=widthSoFar){
					int localWidthOffset=x-(widthSoFar-wordStyle.PixelWidth);
					lettersSoFar+=wordStyle.Text.LetterIndex(localWidthOffset);
					break;
				}else{
					lettersSoFar+=word.LetterCount();
				}
			}
			
			return lettersSoFar;
		}
		
		/// <summary>Finds the index of the nearest character to x pixels.</summary>
		/// <param name="x">The number of pixels from the left edge of this text element.</param>
		/// <returns>The index of the nearest letter.</returns>
		public int LetterIndex(int x){
			if(ChildNodes==null){
				return 0;
			}
			
			int widthSoFar=0;
			int lettersSoFar=0;
			
			for(int i=0;i<ChildNodes.Count;i++){
				WordElement word=(WordElement)ChildNodes[i];
				// Grab the words computed style:
				Css.ComputedStyle wordStyle=word.Style.Computed;
				
				// Bump up the current width:
				widthSoFar+=wordStyle.PixelWidth;
				
				if(x<=widthSoFar){
					int localWidthOffset=x-(widthSoFar-wordStyle.PixelWidth);
					lettersSoFar+=wordStyle.Text.LetterIndex(localWidthOffset);
					break;
				}else{
					lettersSoFar+=word.LetterCount();
				}
			}
			return lettersSoFar;
		}
		
		/// <summary>Turns the temporary buffer into a full child word element.</summary>
		public void DoneWord(bool lastOne){
			if(string.IsNullOrEmpty(Text)){
				return;
			}
			AppendNewChild(new WordElement(Document,this,Text));
			Text="";
		}
		
		/// <summary>Gets the relative position in pixels of the letter at the given index.</summary>
		/// <param name="index">The index of the letter in this text element.</param>
		/// <returns>The number of pixels from the left and top edges of this text element the letter is as a vector.</returns>
		public Vector2 GetPosition(int index){
			if(index==0||ChildNodes==null){
				return Vector2.zero;
			}
			
			int localOffset;
			WordElement word=GetWordWithLetter(index,out localOffset);
			
			if(word==null){
				return Vector2.zero;
			}
			
			Css.ComputedStyle computed=word.Style.Computed;
			float left=computed.ParentOffsetLeft + computed.Text.LocalPositionOf(localOffset);
			float top=computed.ParentOffsetTop;
			
			return new Vector2(left,top);
		}
		
		/// <summary>Gets the word containing the letter at the given index.</summary>
		/// <param name="index">The index of the letter to find in this text element.</param>
		/// <param name="localOffset">The index of the letter in the word.</param>
		/// <returns>The nearest word element found.</returns>
		public WordElement GetWordWithLetter(int index,out int localOffset){
			localOffset=0;
			if(ChildNodes==null){
				return null;
			}
			
			int lettersSoFar=0;
			for(int i=0;i<ChildNodes.Count;i++){
				
				WordElement word=(WordElement)ChildNodes[i];
				int letterCount=word.LetterCount();
				lettersSoFar+=letterCount;
				
				if(index<=lettersSoFar){
					localOffset=index-(lettersSoFar-letterCount);
					return word;
				}
			}
			
			return null;
		}
		
		/// <summary>Gets the content of this element as text.</summary>
		public override string ToTextString(){
			return ToString();
		}
		
		/// <summary>Gets the content of this element as text.</summary>
		public override string ToString(){
			if(ChildNodes==null){
				return "";
			}
			
			string result="";
			for(int i=0;i<ChildNodes.Count;i++){
				result+=ChildNodes[i].ToString();
			}
			return result;
		}
		
	}
	
}