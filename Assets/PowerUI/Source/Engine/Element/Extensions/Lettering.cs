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
using System.Collections;
using System.Collections.Generic;
using PowerUI.Css;


namespace PowerUI{
	
	/// <summary>
	/// A useful function which splits all letters in an element into their own individual elements.
	/// Great for animating each letter on it's own. Similar to lettering.js.
	/// </summary>

	public partial class Element{
		
		public void Lettering(){
			
			List<Element> newNodes=new List<Element>();
			
			Lettering(newNodes,this);
			
			//  Apply the new child set:
			childNodes=newNodes;
			
		}
		
		internal void Lettering(List<Element> into,Element parent){
			
			if(ChildNodes==null){
				return;
			}
			
			int count=ChildNodes.Count;
			
			for(int i=0;i<count;i++){
				
				// Grab the child node:
				Element child=ChildNodes[i];
				
				// Get the element type:
				Type type=child.GetType();
				
				if(type == typeof(TextElement)){
					
					// Apply to each word:
					child.Lettering(into,parent);
				
				}else if(type == typeof(WordElement)){
					
					// Split the word into its chars:
					WordElement word=(WordElement)child;
					
					// Grab the text object:
					TextRenderingProperty textProperty=word.Style.Computed.Text;
					
					if(textProperty==null || textProperty.Text==null){
						
						continue;
						
					}
					
					// Get the characters:
					char[] characters=textProperty.Text.ToCharArray();
					
					// How many chars?
					int characterCount=characters.Length;
					
					// Add each letter as a new element:
					for(int c=0;c<characterCount;c++){
						
						// The character(s) as a string:
						string charString;
						
						// Grab the character:
						char character=characters[c];
						
						// Surrogate pair?
						if(char.IsHighSurrogate(character) && c!=characters.Length-1){
							
							// Low surrogate follows:
							char lowChar=characters[c+1];
							
							c++;
							
							// Get the charcode:
							int code=char.ConvertToUtf32(character,lowChar);
							
							// Turn it back into a string:
							charString=char.ConvertFromUtf32(code);
							
						}else{
							
							charString=""+character;
							
						}
						
						// Create a word ele:
						WordElement result=new WordElement(Document,parent,charString);
						
						// Add it:
						into.Add(result);
						
					}
					
				}else{
					
					// Direct add:
					into.Add(child);
					
				}
				
			}
			
		}
	
	}
	
}