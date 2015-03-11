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


namespace PowerUI.Css{

	/// <summary>
	/// Holds a set of css style properties.
	/// </summary>

	public class Style{
		
		/// <summary>The element that this style belongs to, if any.</summary>
		public Element Element;
		/// <summary>The delimiter that seperates the property name from its value in css.</summary>
		public static char[] Delimiter=new char[]{':'};
		/// <summary>The mapping of css property (e.g. display) to value ("none" as a <see cref="PowerUI.Css.Value"/>).</summary>
		public Dictionary<CssProperty,Value> Properties=new Dictionary<CssProperty,Value>();
		
		
		/// <summary>Creates a new style for the given element.</summary>
		/// <param name="element">The element this style is for.</param>
		public Style(Element element){
			Element=element;
		}
		
		/// <summary>Creates a new, empty style definition.</summary>
		public Style(){}
		
		/// <summary>Creates a new style with the given css text string seperated by semicolons.</summary>
		/// <param name="text">A css text string to apply to this style.</param>
		public Style(string text){
			cssText=text;
		}
		
		/// <summary>Sets the css text of this style as a css string seperated by semicolons (;).</summary>
		public string cssText{
			set{
				Properties.Clear();
				
				if(!string.IsNullOrEmpty(value)){
					string[] pieces=value.Split(';');
					
					for(int i=0;i<pieces.Length;i++){
						string propertyLine=pieces[i];
						
						if(propertyLine==""){
							continue;
						}
						
						string[] property=propertyLine.Split(Delimiter,2);
						
						if(property.Length==1){
							continue;
						}
						
						Set(property[0],property[1]);
					}
				}
			}
		}
		
		/// <summary>Directly sets the named property to the given value.
		/// Note that this does *not* refresh the style onscreen in anyway - it simply writes the new property value.
		/// Internal to the layout engine only.</summary>
		/// <param name="property">The property to set the value of.</param>
		/// <param name="value">The new value for the property.</param>
		public void SetDirect(string property,string value){
			Set(property,value);
		}
		
		/// <summary>Gets the value of the given property, if any.</summary>
		/// <param name="cssProperty">The property to get the value of, e.g. "display".</param>
		/// <returns>The value of the property if found. Null otherwise.</returns>
		public Value Get(string cssProperty){
			
			// Get the inner index:
			int innerIndex=Css.Value.GetInnerIndex(ref cssProperty);
			
			// Get the property:
			CssProperty property=CssProperties.Get(cssProperty);
			
			if(property==null){
				// Property not found. Stop there.
				return null;
			}
			
			Value result=this[property];
			
			if(result==null){
				return null;
			}
			
			if(innerIndex!=-1){
				return result[innerIndex];
			}
			
			return result;
		}
		
		/// <summary>Sets the named property on this style to the given value.</summary>
		/// <param name="property">The property to set or overwrite. e.g. "display".</param>
		/// <param name="value">The value to set the property to, e.g. "none".</param>
		public void Set(string property,string value){
			property=property.Trim();
			bool important=false;
			
			if(!string.IsNullOrEmpty(value) && property!="inner-text"){
				value=value.Trim();
				
				if(value.EndsWith("!important")){
					// It's important!
					important=true;
					
					// Chop it off:
					value=value.Substring(0,value.Length-10);
					
					// Trim again:
					value=value.Trim();
					
				}else if(value.EndsWith("! important")){
					// It's important!
					important=true;
					
					// Chop it off:
					value=value.Substring(0,value.Length-11);
					
					// Trim again:
					value=value.Trim();
				}
				
			}
			
			if(property=="opacity"){
				property="color-overlay-a";
			}
			
			int innerIndex=Css.Value.GetInnerIndex(ref property);
			
			if(property=="border"){
				string width="";
				string colour="";
				string[] pieces=value.Split(' ');
				
				if(innerIndex!=-1){
					// E.g. border-left. Width or colour.

					foreach(string piece in pieces){
						ValueType type=Value.TypeOf(piece);
						
						if(type==ValueType.Color){
							colour=piece;
						}else if(type!=ValueType.Text){
							width=piece;
						}
					}
					
					if(width!=""){
						Set("border-width",width,innerIndex,important);
					}
					
					if(colour!=""){
						Set("border-color",colour,innerIndex,important);
					}
					
					return;
				}
				
				string style="";
				
				foreach(string piece in pieces){
					ValueType type=Value.TypeOf(piece);
					if(type==ValueType.Color){
						if(colour!=""){
							colour+=" ";
						}
						
						colour+=piece;
					}else if(type==ValueType.Text){
						style=piece;
					}else{
						if(width!=""){
							width+=" ";
						}
						
						width+=piece;
					}
				}
				
				Set("border-style",style,-1,important);
				Set("border-width",width,-1,important);
				Set("border-color",colour,-1,important);
				
				return;
			}else if(property=="text-decoration"){
				
				string decoLine="";
				string colour="";
				string[] pieces=value.Split(' ');
				
				foreach(string piece in pieces){
					
					ValueType type=Value.TypeOf(piece);
					
					if(piece=="initial" || type==ValueType.Color){
						colour=piece;
					}else{
						decoLine=piece;
					}
					
				}
				
				Set("text-decoration-line",decoLine,-1,important);
				
				if(colour!=""){
					Set("text-decoration-color",colour,-1,important);
				}
				
				
				
				return;
			}else if(property=="background"){
				string repeat="";
				string position="";
				string colour="";
				string image="";
				string[] pieces=value.Split(' ');
				
				foreach(string piece in pieces){
					
					if(piece==""){
						continue;
					}
					
					if(piece.StartsWith("url(") || piece=="none"){
						image=piece;
					}else if(piece.Contains("repeat")){
						
						if(repeat!=""){
							repeat+=" ";
						}
						
						repeat+=piece;
					}else{
						ValueType type=Value.TypeOf(piece);
						
						if(type==ValueType.Color){
							colour=piece;
						}else{
							//position.
							if(position!=""){
								position+=" ";
							}
							position+=piece;
						}
					}
				}
				
				if(colour!=""){
					Set("background-color","#"+colour,-1,important);
				}else{
					Set("background-color","",-1,important);
				}
				
				Set("background-image",image,-1,important);
				Set("background-repeat",repeat,-1,important);
				Set("background-position",position,-1,important);
				return;
			}
			
			Set(property,value,innerIndex,important);
		}
		
		/// <summary>Gets the computed form of this style.</summary>
		/// <returns>The computed style.</returns>
		public virtual ComputedStyle GetComputed(){
			return null;
		}
		
		/// <summary>Sets the named property on this style to the given value. An inner value may be set; For example,
		/// setting the red component of color-overlay (color-overlay-r) becomes color-overlay and an innerIndex of 0.</summary>
		/// <param name="property">The property to set or overwrite. e.g. "display".</param>
		/// <param name="value">The value to set the property to, e.g. "none".</param>
		/// <param name="innerIndex">The index of the inner value to set, if any. -1 to set the whole property.</param>
		private void Set(string cssProperty,string value,int innerIndex,bool important){
			
			if(Element!=null && Element.Document.AotDocument){
				return;
			}
			
			// Get the property:
			CssProperty property=CssProperties.Get(cssProperty);
			
			if(property==null){
				// It doesn't exist!
				return;
			}
			
			if(value==""){
				value=null;
			}
			
			if(value==null){
				
				Properties.Remove(property);
				// Was in there (and not blank) - change it.
				OnChanged(property,null);
				
				return;
			}
			
			// Get the type of the property:
			ValueType type=Css.Value.TypeOf(property,ref value);
			
			// Read or create the raw property value:
			Value propertyValue=GetRawValue(property,type);
			
			if(innerIndex!=-1){
				
				// Writing to an inner value, e.g. border-left. Grab it:
				Value innerValue=propertyValue[innerIndex];
				
				if(innerValue==null){
					innerValue=propertyValue[innerIndex]=new Value();
				}
				
				innerValue.Set(value);
				
				// Apply its importance:
				innerValue.Important=important;
				
			}else{
				
				if(type==ValueType.Null){
					propertyValue.Set(value);
					
				}else{
					propertyValue.Set(value,type);
				}
				
				// Apply its importance:
				propertyValue.Important=important;
				
			}
			
			// Let the sheet know the value changed:
			if(type==ValueType.Null){
				OnChanged(property,null);
			}else{
				OnChanged(property,propertyValue);
			}
		}
		
		/// <summary>Gets or creates the base value for the given property.
		/// The base value is essentially the value held directly in this style sheet.
		/// E.g. if the value you're setting is the R channel of color-overlay, this sets up the color-overlay value for you.</summary>
		/// <returns>The raw value (which may have just been created).</returns>
		public Css.Value GetRawValue(CssProperty property,ValueType type){
			
			Css.Value propertyValue;
			
			// Does it exist already?
			if(!Properties.TryGetValue(property,out propertyValue)){
				
				// Nope! Create it now. Does the computed style hold a value instead?
				ComputedStyle computed=GetComputed();
				
				if(computed!=null && computed.Properties.TryGetValue(property,out propertyValue) && propertyValue!=null){
					// Let's derive from the computed form.
					propertyValue=propertyValue.Copy();
					Properties[property]=propertyValue;
				}else{
					// Needs to be created.
					Properties[property]=propertyValue=new Value();
					
					if(type==ValueType.Null){
						type=ValueType.Rectangle;
					}
					
					// Set the default value:
					property.SetDefault(propertyValue,type);
					
					if(type==ValueType.Inherit){
						
						// Set inherit:
						propertyValue.Type=ValueType.Inherit;
						
					}
					
					
				}
				
			}
			
			if(propertyValue.Type==ValueType.Inherit && type!=ValueType.Inherit){
				// Special case - we need to duplicate it.
				
				Properties[property]=propertyValue=propertyValue.Copy();
				propertyValue.Type=type;
				
			}
			
			return propertyValue;
			
		}
		
		/// <summary>called when the named property changes.</summary>
		/// <param name="property">The property that changed.</param>
		/// <param name="newValue">It's new fully parsed value. May be null.</param>
		public virtual void OnChanged(CssProperty property,Value newValue){
		}
		
		/// <summary>Gets or sets the parsed value of this style by property name.</summary>
		/// <param name="property">The property to get the value for.</param>
		public Value this[string cssProperty]{
			
			get{
				Value result;
				
				// Get the property:
				int innerIndex;
				CssProperty property=CssProperties.Get(cssProperty,out innerIndex);
				
				if(property==null){
					return null;
				}
				
				if(Properties.TryGetValue(property,out result)){
				
					if(innerIndex!=-1){
						
						// Grab the inner index:
						result=result[innerIndex];
						
					}
					
				}
				
				return result;
			}
			
			set{
				// Get the CSS property:
				int innerIndex;
				CssProperty property=CssProperties.Get(cssProperty,out innerIndex);
				
				if(property==null){
					return;
				}
				
				if(innerIndex!=-1){
					// Apply to inner value.
					
					// First, get the value we're applying the inner property to:
					Css.Value mainValue=GetRawValue(property,value.Type);
					
					// And apply it now:
					mainValue[innerIndex]=value;
					
					// Update value to the property which actually changed:
					value=mainValue;
					
					return;
				}else{
				
					// Applying it to a main property:
					
					if(value==null){
						// Remove it:
						Properties.Remove(property);
					}else{
						// Add it now:
						Properties[property]=value;
					}
				
				}
				
				// Let the sheet know the value changed:
				if(value==null || value.Type==ValueType.Null){
					OnChanged(property,null);
				}else{
					OnChanged(property,value);
				}
				
			}
		}
		
		/// <summary>Gets or sets the parsed value of this style by property name.</summary>
		/// <param name="property">The property to get the value for.</param>
		public Value this[CssProperty property]{
			get{
				Value result;
				Properties.TryGetValue(property,out result);
				return result;
			}
			set{
				Properties[property]=value;
			}
		}
		
		/// <summary>Gets the given property as a css string. May optionally read the given inner index of it as a css string.</summary>
		/// <param name="property">The property to get as a string.</param>
		/// <param name="innerIndex">The inner value to get from the property. -1 for the whole property.</param>
		/// <returns>The property as a css string, e.g. color-overlay may return "#ffffff".</returns>
		public virtual string GetString(string cssProperty,int innerIndex){
			
			// Get the property:
			CssProperty property=CssProperties.Get(cssProperty);
			
			if(property==null){
				// Doesn't exist - can't have been set.
				return "";
			}
			
			// Get the value:
			Value propertyValue=this[property];
			
			if(propertyValue==null){
				return "";
			}
			
			if(innerIndex!=-1){
				
				propertyValue=propertyValue[innerIndex];
				
				if(propertyValue==null){
					return "";
				}
				
			}
			
			return propertyValue.ToString();
		}
		
		/// <summary>Gets the given property as a css string.</summary>
		/// <param name="property">The property to get as a string.</param>
		/// <returns>The property as a css string, e.g. color-overlay may return "#ffffff".</returns>
		public virtual string GetString(string cssProperty){
			
			// Read the value:
			Value propertyValue=this[cssProperty];
			
			if(propertyValue==null){
				return "";
			}
			
			return propertyValue.ToString();
		}
		
	}
	
}