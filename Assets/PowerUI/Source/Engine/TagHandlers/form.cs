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

namespace PowerUI{
	
	/// <summary>
	/// Represents a html form which lets you collect information from the player.
	/// For those new to html, see input and select tags.
	/// Supports onsubmit="nitroMethodName" and the action attributes.
	/// </summary>
	
	public class FormTag:HtmlTagHandler{
		
		/// <summary>The url to post the form to.</summary>
		public string Action;
		
		
		public override string[] GetTags(){
			return new string[]{"form"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new FormTag();
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="onsubmit"){
				return true;
			}else if(property=="action"){
				Action=Element["action"];
				return true;
			}
			
			return false;
		}
		
		/// <summary>Gets all input elements contained within this form.</summary>
		/// <returns>A list of all input elements.</returns>
		public List<Element> GetAllInputs(){
			List<Element> results=new List<Element>();
			GetAllInputs(results,Element);
			return results;
		}
		
		/// <summary>Gets all inputs from the given element, adding the results to the given list.</summary>
		/// <param name="results">The list that all results are added to.</param>
		/// <param name="element">The element to check.</param>
		private void GetAllInputs(List<Element> results,Element element){
			List<Element> kids=element.childNodes;
			if(kids==null){
				return;
			}
			for(int i=0;i<kids.Count;i++){
				Element child=kids[i];
				if(child.Tag=="input"||child.Tag=="select"||child.Tag=="textarea"){
					results.Add(child);
				}else{
					GetAllInputs(results,child);
				}
			}
		}
		
		/// <summary>Gets the selected input by the given name attribute.
		/// E.g. there may be more than one input element (such as with radios); this is the active one.</summary>
		public Element this[string name]{
			get{
				List<Element> allWithName=Element.getElementsByAttribute("name",name);
				if(allWithName.Count==0){
					return null;
				}
				
				if(allWithName.Count==1){
					return allWithName[0];
				}
				
				// We have more than one. If it's a radio, return the one which is selected.
				// Otherwise, return the last one.
				
				InputTag tag=((InputTag)(allWithName[0].Handler));
				if(tag.Type==InputType.Radio){
					
					// Which is selected?
					foreach(Element radio in allWithName){
						if(((InputTag)(radio.Handler)).Checked){
							return radio;
						}
					}
				}
				return allWithName[allWithName.Count-1];
			}
		}
		
		/// <summary>True if this form has a submit button within it.</summary>
		public bool HasSubmitButton{
			get{
				// Get all inputs:
				List<Element> allInputs=Element.getElementsByTagName("input");
				
				// Are any a submit?
				foreach(Element element in allInputs){
					if(element["type"]=="submit"){
						return true;
					}
				}
				
				return false;
			}
		}
		
		/// <summary>Submits this form.</summary>
		public void submit(){
			
			// Generate a nice dictionary of the form contents.
			
			// Step 1: find the unique names of the elements:
			Dictionary<string,string> uniqueValues=new Dictionary<string,string>();
			
			List<Element> allInputs=GetAllInputs();
			
			foreach(Element element in allInputs){
				string type=element["type"];
				if(type=="submit"||type=="button"){
					// Don't want buttons in here.
					continue;
				}
				
				string name=element["name"];
				if(name==null){
					name="";
				}
				
				// Step 2: For each one, get their value.
				// We might have a name repeated, in which case we check if they are radio boxes.
				
				if(uniqueValues.ContainsKey(name)){
					// Ok the element is already added - we have two with the same name.
					// If they are radio, then only overwrite value if the new addition is checked.
					// Otherwise, overwrite - furthest down takes priority.
					if(element.Tag=="input"){
						InputTag tag=(InputTag)(element.Handler);
						if(tag.Type==InputType.Radio&&!tag.Checked){
							// Don't overwrite.
							continue;
						}
					}
				}
				string value=element.value;
				if(value==null){
					value="";
				}
				uniqueValues[name]=value;
			}
			
			FormData formData=new FormData(uniqueValues);
			
			// Hook up the form element:
			formData.form=Element;
			
			object result=Element.Run("onsubmit",formData);
			
			if( !string.IsNullOrEmpty(Action) && (result==null||!(bool)result) ){
				// Post to Action.
				FilePath path=new FilePath(Action,Element.Document.basepath,false);
				
				FileProtocol fileProtocol=path.Handler;
				
				if(fileProtocol!=null){
					fileProtocol.OnPostForm(formData,Element,path);
				}
			}
		}
		
	}
	
	
	public partial class Element{
		
		
		/// <summary>Internal use only. <see cref="PowerUI.Element.formElement"/>.
		/// Scans up the DOM to find the parent form element.</summary>
		/// <returns>The parent form element, if found.</returns>
		public Element GetForm(){
			if(Tag=="form"){
				return this;
			}
			if(ParentNode==null){
				return null;
			}
			return ParentNode.GetForm();
		}
		
		/// <summary>Submits the form this element is in.</summary>
		public void submit(){
			FormTag elementForm=form;
			
			if(elementForm!=null){
				elementForm.submit();
			}
		}
		
		/// <summary>Scans up the DOM to find the parent form element.
		/// Note: <see cref="PowerUI.Element.form"/> may be more useful than the element iself.</summary>
		public Element formElement{
			get{
				return GetForm();
			}
		}
		
		/// <summary>Scans up the DOM to find the parent form element's handler.
		/// The object returned provides useful methods such as <see cref="PowerUI.FormTag.submit"/>. </summary>
		public FormTag form{
			get{
				Element formElement=GetForm();
				if(formElement==null){
					return null;
				}
				return ((FormTag)(formElement.Handler));
			}
		}
		
		
	}
	
}