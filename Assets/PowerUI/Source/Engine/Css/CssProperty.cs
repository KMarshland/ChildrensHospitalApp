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


namespace PowerUI.Css{
	
	/// <summary>
	/// A CSS property. You can create custom ones by deriving from this class.
	/// Note that they are instanced globally.
	/// </summary>
	
	public class CssProperty{
		
		/// <summary>The main property name. Some properties use multiple names (e.g. content and inner-text).</summary>
		public string Name;
		/// <summary>Does this css property apply to text? E.g. font-size, color etc.</summary>
		public bool IsTextual;
		/// <summary>True if this property occurs along the x axis. E.g. width, left, right.</summary>
		public bool IsXProperty;
		/// <summary>True if this is width or height only.</summary>
		public bool IsWidthOrHeight;
		/// <summary>The optional type of this property, if it defines it.</summary>
		public ValueType Type=ValueType.Null;
		
		
		/// <summary>The set of all properties that this one will handle. Usually just one.
		/// e.g. "v-align", "vertical-align".
		public virtual string[] GetProperties(){
			return null;
		}
		
		/// <summary>Apply this CSS style to the given computed style.
		/// Note that you can grab the element from the computed style if you need that.</summary>
		/// <param name="style">The computed style to apply the property to.</param>
		/// <param name="value">The new value being applied.</param>
		public virtual void Apply(ComputedStyle style,Value value){}
		
		/// <summary>Sets the default value for this property into the given value.</summary>
		/// <param name="value">The value to write the default to.</param>
		/// <param name="type">The type of the given value. Optionally used.</param>
		public virtual void SetDefault(Css.Value value,ValueType type){
			
			value.Set("",type);
			
		}
		
		/// <summary>Call this if the current property requires a background image object.</summary>
		public BackgroundImage GetBackground(ComputedStyle style){
			
			BackgroundImage image=style.BGImage;
			
			if(image==null){
				style.BGImage=image=new BackgroundImage(style.Element);
				style.EnforceNoInline();
			}
			
			// Flag it as having a change:
			image.Changed=true;
			
			return image;
		}
		
		/// <summary>Call this if the current property requies a border object.</summary>
		public BorderProperty GetBorder(ComputedStyle style){
			
			BorderProperty border=style.Border;
			
			if(border==null){
				style.Border=border=new BorderProperty(style.Element);
				style.EnforceNoInline();
			}
			
			// Flag it as having a change:
			border.Changed=true;
			
			return border;
		}
		
		/// <summary>Call this if the current property requires a text object. NOTE: This one may be null.</summary>
		public TextRenderingProperty GetText(ComputedStyle style){
			
			// Grab it:
			TextRenderingProperty text=style.Text;
			
			if(text!=null){
				// Flag the change:
				text.Changed=true;
			}
			
			return text;
		}
		
	}
	
}