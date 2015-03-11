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
using System.Text;
using System.Collections;
using System.Collections.Generic;
using PowerUI.Css;

namespace PowerUI{

	/// <summary>
	/// Select dropdowns. Supports the onchange="nitroMethod" and name attributes.
	/// </summary>

	public class SelectTag:HtmlTagHandler{
		
		/// <summary>True if this select is currently dropped down.</summary>
		public bool Dropped;
		/// <summary>The index of the selected option.</summary>
		public int SelectedIndex=-1;
		/// <summary>The element that displays the selected option.</summary>
		private Element DisplayText;
		/// <summary>The set of options this select provides.</summary>
		public List<Element> Options;
		
		
		public SelectTag(){
			// Make sure this tag is focusable:
			IsFocusable=true;
		}
		
		public override string[] GetTags(){
			return new string[]{"select"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new SelectTag();
		}
		
		/// <summary>Gets the index for the given element.</summary>
		/// <param name="element">The element to look for.</param>
		/// <returns>The index of the element.</returns>
		public int GetSelectID(Element element){
			if(Options==null){
				return -1;
			}
			
			for(int i=0;i<Options.Count;i++){
				if(Options[i]==element){
					return i;
				}
			}
			
			return -1;
		}
		
		public override void OnResetVariable(string name){
			if(DisplayText!=null){
				DisplayText.ResetVariable(name);
			}
			
			if(Options==null){
				return;
			}
			
			foreach(Element option in Options){
				option.ResetVariable(name);
			}
		}
		
		public override void OnResetAllVariables(){
			if(DisplayText!=null){
				DisplayText.ResetAllVariables();
			}
			
			if(Options==null){
				return;
			}
			
			foreach(Element option in Options){
				option.ResetAllVariables();
			}
		}
		
		public override void OnTagLoaded(){
			// Append the text,dropdown and button.
			
			// Grab the options:
			Options=Element.childNodes;
			// Clear the childnodes - this prevents the .innerHTML below clearing the list we just grabbed.
			Element.childNodes=null;
			// Clear the innerHTML of our dropdown and write in the new content (e.g. the button/dropdown itself).
			Element.innerHTML="<span style='height:100%;'></span><ddbutton>";
			// Next, grab the element we want from our new innerHTML.
			// Text, for showing the current selection.
			DisplayText=Element.childNodes[0];
			
			if(Options!=null){
				// Find the selected option, if there is one:
				for(int i=Options.Count-1;i>=0;i--){
					Element element=Options[i];
					OptionTag optionTag=element.Handler as OptionTag;
					
					if(optionTag!=null && optionTag.Selected){
						// Found the selected option. The last option is used (thus we go through it backwards).
						// Must be done like this because this tags innerHTML won't be available until this occurs for the select to display.
						SetSelected(element);
						break;
					}
				}
			}
			
			if(SelectedIndex!=-1){
				return;
			}
			// Nothing had the selected attribute (<option .. selected ..>); if it did, it would have SetSelected already.
			// We'll select the first one by default, if it exists.
			// -2 Prompts it to not call onchange, then set index 0.
			SetSelected(-2);
		}
		
		/// <summary>Drops this select box down.</summary>
		public void Drop(){
			if(Dropped){
				return;
			}
			Dropped=true;
			Element.Focus();
			Element ddBox=GetDropdownBox();
			
			if(ddBox==null){
				return;
			}
				
			ddBox.style.display="block";
			
			// Locate it to the select:
			ComputedStyle computed=Element.Style.Computed;
			ComputedStyle boxComputed=ddBox.Style.Computed;
			boxComputed.PositionLeft=computed.OffsetLeft;
			boxComputed.InnerWidth=computed.InnerWidth;
			boxComputed.FixedWidth=true;
			boxComputed.SetPixelWidth(true);
			
			boxComputed.PositionTop=computed.OffsetTop+computed.PaddedHeight;
			
			
			ddBox.childNodes=null;
			ddBox.innerHTML="";
			
			if(Options!=null){
				foreach(Element child in Options){
					ddBox.AppendNewChild(child);
				}
			}
			
		}
		
		public override void OnBlur(){
			// Is the new focused element a child of ddbox?
			Element current=Input.Focusing;
			
			while(current!=null){
				
				if(current.Tag=="ddbox"){
					return;
				}
				
				current=current.parentNode;
			}
			
			Hide();
		}
		
		/// <summary>Closes this dropdown.</summary>
		public void Hide(){
			if(!Dropped){
				return;
			}
			
			Dropped=false;
			Element ddBox=GetDropdownBox();
			
			if(ddBox!=null){
				ddBox.style.display="none";
			}
			
		}
		
		/// <summary>Attempts to get the box that will show the options.</summary>
		/// <returns>The dropdown box if it was found; null otherwise.</returns>
		private Element GetDropdownBox(){
			
			return Element.Document.window.top.document.DropdownBox;
		}
		
		/// <summary>Gets the currently selected value.</summary>
		/// <returns>The currently selected value.</returns>
		public string GetValue(){
			if(SelectedIndex==-1 || Options==null){
				return "";
			}
			
			Element selected=Options[SelectedIndex];
			
			if(selected==null){
				return "";
			}
			
			string result=selected["value"];
			
			if(result==null){
				return "";
			}
			
			return result;
		}
		
		/// <summary>Sets the given element as the selected option.</summary>
		/// <param name="element">The option to set as the selected value.</param>
		public void SetSelected(Element element){
			// Find which option # the element is.
			// And set the innerHTML text to the option text. 
			int index=GetSelectID(element);
			
			if(index==SelectedIndex){
				return;
			}
			
			SetSelected(index,element,true);
		}
		
		/// <summary>Sets the option at the given index as the selected one.</summary>
		/// <param name="index">The index of the option to select.</param>
		public void SetSelected(int index){
			if(Options==null || index>=Options.Count){
				return;
			}
			
			bool runOnChange=true;
			
			if(index==-2){
				// Setup/ auto selected when the tag has just been parsed.
				runOnChange=false;
				index=0;
			}
			
			Element element=null;
			if(index>=0){
				element=Options[index];
			}
			SetSelected(index,element,runOnChange);
		}
		
		/// <summary>Sets the option at the given index as the selected one.</summary>
		/// <param name="index">The index of the option to select.</param>
		/// <param name="element">The element at the given index.</param>
		/// <param name="runOnChange">True if the onchange event should run.</param>
		private void SetSelected(int index,Element element,bool runOnChange){
			if(index==SelectedIndex){
				return;
			}
			
			SelectedIndex=index;
			
			if(index<0||element==null){
				// Clear the option text:
				DisplayText.innerHTML="";
			}else{
				DisplayText.innerHTML=element.innerHTML;
			}
			
			// Call onchange, but only if the dropdown didn't auto select this option because it's starting up.
			if(runOnChange){
				Element.Run("onchange");
			}
		}
		
		public override bool OnClick(UIEvent clickEvent){
			
			if(!clickEvent.heldDown && Element.MouseWasDown()){
				
				if(Dropped){
					Hide();
				}else{
					Drop();
				}
				
			}
			
			base.OnClick(clickEvent);
			clickEvent.stopPropagation();
			
			return true;
		}
		
		public override void OnMouseMove(UIEvent e){
			base.OnMouseMove(e);
			
			if(e.leftMouseDown){
				// Left mouse button is currently down.
				
				// Grab the dropdown box:
				Element dropdown=GetDropdownBox();
				
				// Is it outside the select menu and outside the dropdown box?
				if(	!Element.IsMousedOver() && !dropdown.IsMousedOver() ){
					// Yep it is - Clicked outside the dropdown menu.
					
					// Hide it now:
					Hide();
					
				}
				
			}
			
		}
		
	}
	
	
	public partial class Element{
		
		/// <summary>Updates the current selected element in a dropdown menu.</summary>
		public int selectedIndex{
			get{
				
				SelectTag tag=Handler as SelectTag;
				
				if(tag==null){
					return -1;
				}
				
				return tag.SelectedIndex;
			}
			set{
				
				SelectTag tag=Handler as SelectTag;
				
				if(tag==null){
					return;
				}
				
				tag.SetSelected(value);
				
			}
		}
		
	}
	
}
