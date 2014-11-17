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

using Wrench;
using System.Collections;
using System.Collections.Generic;


namespace PowerUI{
	
	/// <summary>
	/// Allows easy iteration through all elements of a document.
	/// See Document.allElements to use this.
	/// </summary>
	
	public class DocumentElements:IEnumerable<Element>{
		
		/// <summary>The document containing the elements to enumerate.</summary>
		public Document Document;
		/// <summary>True if the iteration should skip the following children of the current element.</summary>
		public bool SkipChildren;
		
		
		public DocumentElements(Document document){
			Document=document;
		}
		
		/// <summary>Iterates through the given element.</summary>
		/// <param name="element">The element to iterate through.</param>
		public IEnumerable<Element> IterateThrough(Element element){
			// First, return the element:
			yield return element;
			
			// Should we skip kids?
			if(SkipChildren){
				SkipChildren=false;
			}else if(element.childNodes!=null){
				// It's got kids - loop through those:
				List<Element> children=element.childNodes;
				// Grab how many their are:
				int childCount=children.Count;
				
				for(int i=0;i<childCount;i++){
					// Iterate through each child, returning the result:
					foreach(Element child in IterateThrough(children[i])){
						yield return child;
					}
				}
			}
		}
		
		public IEnumerator<Element> GetEnumerator(){
			foreach(Element result in IterateThrough(Document.html)){
				yield return result;
			}
		}
		
		IEnumerator IEnumerable.GetEnumerator(){
			return GetEnumerator();
		}
		
	}
	
}