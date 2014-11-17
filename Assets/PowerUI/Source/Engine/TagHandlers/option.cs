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
	/// Handles option tags for dropdowns. Supports the selected and value="" attributes.
	/// </summary>

	public class OptionTag:HtmlTagHandler{
		
		/// <summary>True if this is the selected option.</summary>
		public bool Selected;
		/// <summary>True if this tag has been fully loaded. False if it's still being parsed.</summary>
		private bool FullyLoaded;
		/// <summary>The select dropdown that this option belongs to.</summary>
		public SelectTag Dropdown;
		
		
		public override string[] GetTags(){
			return new string[]{"option"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new OptionTag();
		}
		
		public override bool OnAttributeChange(string property){	
			if(base.OnAttributeChange(property)){
				return true;
			}
			if(property=="selected"){
				string isSelected=Element["selected"];
				Selected=(string.IsNullOrEmpty(isSelected) || isSelected=="1" || isSelected=="true" || isSelected=="yes");
				if(FullyLoaded){
					// Tell the select:
					Dropdown.SetSelected(Element);
				}	
				return true;
			}
			return false;
		}
		
		public override void OnTagLoaded(){
			FullyLoaded=true;
			Dropdown=GetSelect(Element.parentNode);
		}
		
		/// <summary>Gets or finds the parent select tag that this option belongs to.</summary>
		/// <param name="element">The element to check if it's a select.</param>
		/// <returns>The select tag handler if found; null otherwise.</returns>
		private SelectTag GetSelect(Element element){
			if(element==null){
				return null;
			}
			if(element.Tag=="select"){
				return (SelectTag)(element.Handler);
			}
			return GetSelect(element.parentNode);
		}
		
		public override bool OnClick(UIEvent clickEvent){
			
			// No bubbling:
			clickEvent.stopPropagation();
			
			if(clickEvent.heldDown || Dropdown==null){
				return true;
			}
			
			Dropdown.SetSelected(Element);
			Dropdown.Hide();
			
			return true;
		}
		
	}
	
}