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
using System.Collections;
using System.Globalization;
using System.Collections.Generic;

namespace PowerUI.Css{

	/// <summary>
	/// This represents a parsed value of a css property.
	/// </summary>

	public class Value{
		
		
		public static int GetInnerIndex(ref string property){
			int innerIndex=-1;
			string[] propertyPieces=property.Split('-');
			int index=0;
			
			if(propertyPieces.Length>1){
				
				for(int i=0;i<propertyPieces.Length;i++){
					string piece=propertyPieces[i];
					
					if(piece=="top"||piece=="x"||piece=="r"){
						innerIndex=0;
						index=i;
						break;
					}else if(piece=="bottom"||(piece=="z"&&i!=0)||piece=="b"){ //exception here - if i is zero, we have "z-index".
						innerIndex=2;
						index=i;
						break;
					}else if(piece=="left"||piece=="a"){
						innerIndex=3;
						index=i;
						break;
					}else if(piece=="right"||piece=="g"||piece=="y"){
						innerIndex=1;
						index=i;
						break;
					}
					
				}
				
			}
			
			if(innerIndex!=-1){
				property="";
				
				for(int i=0;i<propertyPieces.Length;i++){
					if(i==index){
						continue;
					}
					
					if(property!=""){
						property+="-";
					}
					
					property+=propertyPieces[i];
				}
				
			}
			
			return innerIndex;
		}
	
		public static ValueType TypeOf(CssProperty property,ref string value){
		
			if(value=="inherit"){
				return ValueType.Inherit;
			}
			
			if(property.Name=="background-image"){
				value=value.Replace("url(","").Replace(")","").Replace("'","").Replace("\"","");
			}
			
			if(property.Type!=ValueType.Null){
				return property.Type;
			}
			
			return TypeOf(value);
		}
	
		public static bool IsSingle(string valueText){
			if(string.IsNullOrEmpty(valueText)){
				return false;
			}
			
			char[] chars=valueText.ToCharArray();
			
			for(int i=0;i<chars.Length;i++){
				if(chars[i]=='.' || chars[i]=='-'){
					continue;
				}
				// Is it 0-9?
				int charCode=(int)chars[i];
				if(charCode<48||charCode>57){
					return false;
				}
			}
			return true;
		}
	
		public static ValueType TypeOf(string valueText){
			if(valueText==""){
				return ValueType.Null;
			}else if(valueText.Contains(" ")){
				return ValueType.Rectangle;
			}else if(valueText.StartsWith("#")||valueText.StartsWith("rgba(")||valueText.StartsWith("rgb(")){
				return ValueType.Color;
			}else if(valueText.EndsWith("px")){
				return ValueType.Pixels;
			}else if(valueText.EndsWith("deg")){
				return ValueType.Degrees;
			}else if(valueText.EndsWith("rad")){
				return ValueType.Radians;
			}else if(valueText.EndsWith("%")){
				return ValueType.Percentage;
			}else if(valueText.EndsWith("em")){
				return ValueType.Em;
			}else if(valueText=="true" || valueText=="false"){
				return ValueType.Boolean;
			}else if(IsSingle(valueText)){
				return ValueType.Single;
			}else if(valueText=="inherit"){
				return ValueType.Inherit;
			}else if(valueText.StartsWith("calc(")){
				return ValueType.Calc;
			}
			
			return ValueType.Text;
		}
	
		/// <summary>If this is a pixel integer, the raw pixel value.</summary>
		public int PX;
		/// <summary>If this is a text value, e.g. "auto", the raw text value.</summary>
		public string Text;
		/// <summary>If this is a decimal, the raw decimal value.</summary>
		public float Single;
		/// <summary>If this is a boolean, the raw bool value.</summary>
		public bool Boolean;
		/// <summary>True if this value is important.</summary>
		public bool Important;
		/// <summary>The set of internal values, such as each individual value of padding.</summary>
		public Value[] InnerValues;
		/// <summary>A calculation if this value is calc(..).</summary>
		public Calculation Calculation;
		/// <summary>The type of value this is.</summary>
		public ValueType Type=ValueType.Null;
		
		
		
		/// <summary>Creates a new empty value.</summary>
		public Value(){}
		
		/// <summary>Creates a new value from the given value text and guesses what type it is.</summary>
		/// <param name="text">The value to set, e.g. "auto" or "14.9".</param>
		public Value(string text){
			Set(text);
		}
		
		/// <summary>Creates a new value from the given value as text and
		/// a type of the value represented in the text.</summary>
		/// <param name="text">The value to set, e.g. "auto" or "14.9".</param>
		/// <param name="type">The type of the value, e.g. percentage.</param>
		public Value(string text,ValueType type){
			Set(text,type);
		}
		
		/// <summary>Checks if this is 'auto'</summary>
		/// <returns>True if this value is 'auto'</returns>
		public bool IsAuto(){
			return Type==ValueType.Text && Text.ToLower()=="auto";
		}
		
		/// <summary>Checks if this is an absolute value and is not a percentage/em/ rectangle containing percents.</summary>
		/// <returns>True if this value is absolute; false if it is relative.</returns>
		public bool IsAbsolute(){
			// True if this value is fixed (i.e. not a percentage, or a rectangle containing percentages).
			if(Type==ValueType.Percentage||Type==ValueType.Em||Type==ValueType.Inherit||Type==ValueType.Calc){
				return false;
			}
			
			if(InnerValues!=null){
				for(int i=0;i<InnerValues.Length;i++){
					if(!InnerValues[i].IsAbsolute()){
						return false;
					}
				}
			}
			
			return true;
		}
		
		/// <summary>Checks if two values are equal.</summary>
		/// <param name="value">The value to check for equality with this. Always returns false if null.</param>
		/// <returns>True if this and the given value are equal; false otherwise.</returns>
		public bool Equals(Value value){
			if(value==null || value.Type!=Type){
				return false;
			}
			
			switch(Type){
				case ValueType.Null:
					return true;
				case ValueType.Boolean:
					return (value.Boolean==Boolean);
				case ValueType.Pixels:
					return (value.PX==PX);
				case ValueType.Percentage:
					return (value.Single==Single);
				case ValueType.Em:
					return (value.Single==Single);
				case ValueType.Radians:
					return (value.Single==Single);
				case ValueType.Degrees:
					return (value.Single==Single);
				case ValueType.Single:
					return (value.Single==Single);
				case ValueType.Text:
					return (value.Text==Text);
				case ValueType.Calc:
					return (value.Single==Single);
				default:
				
				if(InnerValues==null){
					return (value.InnerValues==null);
				}else if(value.InnerValues==null){
					return (InnerValues==null);
				}
				
				for(int i=InnerValues.Length-1;i>=0;i--){
					if(InnerValues[i]==null){
						if(value.InnerValues[i]!=null){
							return false;
						}
					}else if(!InnerValues[i].Equals(value.InnerValues[i])){
						return false;
					}
				}
				
				return true;
			}
			
		}
		
		/// <summary>Converts this relative value (such as a percentage or em) into a fixed one.</summary>
		/// <param name="property">The property that this value represents and is being made absolute.</param>
		/// <param name="element">The element holding all the values that represent 100%.</param>
		public void MakeAbsolute(CssProperty property,Element element){
			if(element.ParentNode==null){
				PX=0;
				return;
			}
			
			ComputedStyle parentStyle=element.ParentNode.Style.Computed;
			
			switch(Type){
				case ValueType.Em:
					BakePX(ParentFontSize(parentStyle));
				break;
				case ValueType.Percentage:
				
					ComputedStyle computed=element.Style.Computed;
					
					// Is this along x?
					if(property.IsXProperty){
						// Yep!
						BakePX(parentStyle.InnerWidth-computed.PaddingLeft-computed.PaddingRight);
					}else{
						// Nope!
						BakePX(parentStyle.InnerHeight-computed.PaddingTop-computed.PaddingBottom);
					}
				
				break;
				case ValueType.Inherit:
					
					InheritFrom(parentStyle[property]);
				
				break;
				case ValueType.Calc:
					
					computed=element.Style.Computed;
					
					
					int size=0;
					
					// Is this along x?
					if(property.IsXProperty){
						// Yep!
						size=parentStyle.InnerWidth-computed.PaddingLeft-computed.PaddingRight;
					}else{
						// Nope!
						size=parentStyle.InnerHeight-computed.PaddingTop-computed.PaddingBottom;
					}
					
					PX=Calculation.Run(size);
					
				break;
				default:
				
					computed=element.Style.Computed;
					
					// It's a box or point - compute all values [y,x,y,x]
					// Both will have the first two values:
					bool useWidth=(Type==ValueType.Point); //x is first for a point.
					
					// Don't include padding in the value.
					int padding=useWidth?computed.PaddingLeft+computed.PaddingRight:computed.PaddingTop+computed.PaddingBottom;
					
					// The cached fontsize if any of these use EM; Chances are more than one will.
					int parentFontSize=-1;
					
					for(int i=0;i<InnerValues.Length;i++){
						Value innerValue=InnerValues[i];
						
						if(innerValue.Type==ValueType.Em){
							if(parentFontSize==-1){
								parentFontSize=ParentFontSize(parentStyle);
							}
							innerValue.BakePX(parentFontSize);
						}else{
							// Whats the block size?
							int value=useWidth?parentStyle.InnerWidth:parentStyle.InnerHeight;
							// Bake a percentage based on the size:
							innerValue.BakePX(value-padding);
						}
						// And flip useWidth:
						useWidth=!useWidth;
					}
				
				break;
			}
			
		}
		
		/// <summary>Inherits from the given parent value by copying them.</summary>
		/// <param name="parent">The value to inherit from.</param>
		public void InheritFrom(Value parent){
			// e.g. font-size:inherit;
			// Simply copies the parent objects properties exactly.
			if(parent==null){
				PX=0;
				Text=null;
				Boolean=false;
				InnerValues=null;
				Single=0f;
			}else{
				PX=parent.PX;
				Text=parent.Text;
				Single=parent.Single;
				Boolean=parent.Boolean;
				InnerValues=parent.InnerValues;	
			}
			
		}
		
		/// <summary>Gets the fontsize for the given computed style.</summary>
		/// <param name="parentStyle">The style to get the fontsize from. Used for em calculations.</param>
		private int ParentFontSize(ComputedStyle style){
			if(style==null){
				return 12;
			}
			
			if(style.Text!=null){
				return style.Text.FontSize;
			}
			
			// Note that most of the following is actually the namespace; this is just a single static var.
			Value fontSize=style[Css.Properties.FontSize.GlobalProperty];
			
			if(fontSize==null){
				return 12;
			}else{
				return fontSize.PX;
			}
		}
		
		/// <summary>Converts the percentage held by this value into a fixed pixel number.</summary>
		/// <param name="size">The number that represents 100%.</param>
		public void BakePX(int size){
			// Percent to PX.
			
			if(Type==ValueType.Percentage || Type==ValueType.Em){
				PX=(int)(Single*size);
			}
			
		}
		
		/// <summary>Duplicates this value.</summary>
		/// <returns>A duplicated copy of this value. Note that if this value has inner values, they are copied too.</returns>
		public Value Copy(){
			Value result=new Value();
			result.Type=Type;
			
			result.PX=PX;
			result.Text=Text;
			result.Single=Single;
			result.Boolean=Boolean;
			
			if(InnerValues!=null){
				result.InnerValues=new Value[InnerValues.Length];
				
				for(int i=0;i<InnerValues.Length;i++){
					Value value=InnerValues[i];
					
					if(value==null){
						continue;
					}
					
					result[i]=value.Copy();
				}
				
			}
			
			return result;
		}
		
		/// <summary>Sets the given floating point number as the value.</summary>
		/// <param name="value">The value to set.</param>
		public void SetFloat(float value){
			if(Type==ValueType.Pixels){
				PX=(int)value;
			}else if(Type==ValueType.Em || Type==ValueType.Percentage || Type==ValueType.Degrees || Type==ValueType.Single){
				Single=value;
			}else if(Type==ValueType.Boolean){
				Boolean=(value>=0.5f);
			}
		}
		
		/// <summary>Converts this value to a floating point number.</summary>
		/// <returns>The floating point represented by this value if possible. 0 otherwise.</returns>
		public float ToFloat(){
			if(Type==ValueType.Pixels){
				return (float)PX;
			}else if(Type==ValueType.Em || Type==ValueType.Percentage || Type==ValueType.Degrees || Type==ValueType.Single){
				return Single;
			}else if(Type==ValueType.Boolean){
				return Boolean?1f:0f;
			}
			return 0f;
		}
		
		/// <summary>Converts this value to a Unity colour.</summary>
		/// <returns>The unity colour represented by this value.</returns>
		public Color ToColor(){
			return new Color(
				GetFloat(0),
				GetFloat(1),
				GetFloat(2),
				GetFloat(3)
			);
		}
		
		/// <summary>Converts this value to a css useable string.</summary>
		/// <returns>A css formatted string.</returns>
		public override string ToString(){
			
			switch(Type){
				case ValueType.Pixels:
					return PX+"px";
				case ValueType.Em:
					return Single+"em";
				case ValueType.Percentage:
					return (Single*100f)+"%";
				case ValueType.Degrees:
					return Single+"deg";
				case ValueType.Single:
					return Single.ToString();
				case ValueType.Boolean:
					return Boolean?"1":"0";
				case ValueType.Color:
					if(InnerValues==null){
						return "#00000000";
					}
					
					string result="#"+InnerValues[0].ToHex()+InnerValues[1].ToHex()+InnerValues[2].ToHex();
					Value alpha=InnerValues[3];
					
					if(alpha.Single!=1f){
						result+=alpha.ToHex();
					}
					
					return result;
				case ValueType.Rectangle:
					return InnerValueString();
				case ValueType.Point:
					return InnerValueString();
				case ValueType.Radians:
					return Single+"rad";
				case ValueType.Calc:
					return "calc("+Calculation.ToString()+")";
				default:
					return Text; // e.g. 'auto'
			}	
				
		}
		
		private string InnerValueString(){
			
			if(InnerValues==null){
				return "";
			}
			
			string result="";
			
			foreach(Value value in InnerValues){
				string str=value.ToString();
				
				if(str==""){
					continue;
				}
				
				if(result!=""){
					result+=" "+str;
				}else{
					result+=str;
				}
			}
			
			return result;
		}
		
		/// <summary>Empties this value.</summary>
		public void Reset(){
			Set("");
		}
		
		/// <summary>Sets the value and guesses it's type.</summary>
		/// <param name="valueText">The value to set, e.g. "auto" or "14.9".</param>
		public void Set(string valueText){
			Set(valueText,TypeOf(valueText));
		}
		
		/// <summary>Sets the value which is of a known given type.</summary>
		/// <param name="text">The value to set, e.g. "auto" or "14.9".</param>
		/// <param name="type">The type of the value, e.g. percentage.</param>
		public void Set(string valueText,ValueType type){
			Type=type;
			
			switch(Type){
				case ValueType.Null:
					
					Text="";
					
				break;
				case ValueType.Color:
				
					valueText=valueText.Replace("#","");
					float r=0f;
					float g=0f;
					float b=0f;
					float a=1f;
					
					if(valueText.Contains("rgba") || valueText.Contains("rgb")){
						valueText=valueText.Replace(")","").Replace("rgba(","").Replace("rgb(","");
						
						string[] pieces=valueText.Split(',');
						
						// How many pieces?
						int count=pieces.Length;
						
						float.TryParse(pieces[0],out r);
						
						if(count>1){
							float.TryParse(pieces[1],out g);
						}
						
						if(count>2){
							float.TryParse(pieces[2],out b);
						}
						
						if(count>3){
							float.TryParse(pieces[3],out a);
						}
						
					}else{
						int temp;
						
						if(valueText.Contains(" ")){
							string[] pieces=valueText.Split(' ');
							valueText=pieces[0];
						}
						
						if(valueText.Length==3){
							// Shorthand hex colour, e.g. #f0f. Each character is essentially duplicated.
							
							// R:
							int.TryParse(valueText.Substring(0,1),NumberStyles.HexNumber,null,out temp);
							r=DoubleNibble(temp)/255f;
							// G:
							int.TryParse(valueText.Substring(1,1),NumberStyles.HexNumber,null,out temp);
							g=DoubleNibble(temp)/255f;
							// B:
							int.TryParse(valueText.Substring(2,1),NumberStyles.HexNumber,null,out temp);
							b=DoubleNibble(temp)/255f;
						}else{
							// Full hex colour, possibly also including alpha.
							
							if(valueText.Length>=2){
								int.TryParse(valueText.Substring(0,2),NumberStyles.HexNumber,null,out temp);
								r=temp/255f;
							}
							
							if(valueText.Length>=4){
								int.TryParse(valueText.Substring(2,2),NumberStyles.HexNumber,null,out temp);
								g=temp/255f;
							}
							
							if(valueText.Length>=6){
								int.TryParse(valueText.Substring(4,2),NumberStyles.HexNumber,null,out temp);
								b=temp/255f;
							}
							
							if(valueText.Length>=8){
								int.TryParse(valueText.Substring(6,2),NumberStyles.HexNumber,null,out temp);
								a=temp/255f;
							}
						}
					}
					
					SetInnerValues(4);
					Value value=InnerValues[0];
					value.Type=ValueType.Single;
					value.Single=r;
					value=InnerValues[1];
					value.Type=ValueType.Single;
					value.Single=g;
					value=InnerValues[2];
					value.Type=ValueType.Single;
					value.Single=b;
					value=InnerValues[3];
					value.Type=ValueType.Single;
					value.Single=a;
					
				break;
				case ValueType.Pixels:
					
					if(string.IsNullOrEmpty(valueText)){
						PX=0;
					}else{
						
						if(valueText.EndsWith("fpx")){
							// Fixed pixel value. Resolution independent value.
							
							// Attempt to parse it out:
							int.TryParse(valueText.Replace("fpx",""),out PX);
							
						}else if(int.TryParse(valueText.Replace("px",""),out PX)){
							PX=(int)(PX*ScreenInfo.ResolutionScale);
						}
					}
					
				break;
				case ValueType.Point:
					
					// 2D point, e.g. overflow.
					SetInnerValues(valueText,2);
					
				break;
				case ValueType.Rectangle:
					
					// Rectangle set of 4 values.
					SetInnerValues(valueText,4);
					
				break;
				case ValueType.Em:
				
					if(string.IsNullOrEmpty(valueText)){
						Single=1f;
					}else{
						float.TryParse(valueText.Replace("em",""),out Single);
					}
					
				break;
				case ValueType.Radians:
				
					if(string.IsNullOrEmpty(valueText)){
						Single=0f;
					}else{
						float.TryParse(valueText.Replace("rad",""),out Single);
					}
					
				break;
				case ValueType.Percentage:
				
					if(string.IsNullOrEmpty(valueText)){
						Single=0f;
					}else{
						float.TryParse(valueText.Replace("%",""),out Single);
						Single/=100f;
					}
				
				break;
				case ValueType.Single:
				
					if(string.IsNullOrEmpty(valueText)){
						Single=0f;
					}else{
						float.TryParse(valueText,out Single);
					}
					
				break;
				case ValueType.Degrees:
					if(string.IsNullOrEmpty(valueText)){
						Single=0f;
					}else{
						float.TryParse(valueText.Replace("deg",""),out Single);
					}
				break;
				case ValueType.Boolean:
					Boolean=(valueText=="1");
				break;
				case ValueType.Calc:
					Calculation=new Calculation(valueText.Substring(5,valueText.Length-5-1));
				break;
				default:
					Text=valueText;
				break;
			}
			
		}
		
		/// <summary>Resolves a mixed pixel and % value.</summary>
		public int GetMixed(int dimension){
			
			switch(Type){
				case ValueType.Mixed:
					return (int)(dimension*Single)+PX;
				case ValueType.Pixels:
					return PX;
				default:
					return (int)(dimension*Single);
			}
			
		}
		
		public void SetPercent(float percent){
			
			Type=ValueType.Percentage;
			Single=percent;
			
		}
		
		/// <summary>Offsets this percentage value by a given percentage or pixel value.</summary>
		public void Offset(Value by){
			
			if(Single==1f){
				
				// Take from it.
				if(by.PX!=0){
					// Set:
					PX=-by.PX;
				}else{
					// Decrease:
					Single-=by.Single;
				}
				
			}else{
				
				// Add to it.
				if(by.PX!=0){
					// Set:
					PX=by.PX;
				}else{
					// Increase:
					Single+=by.Single;
				}
				
			}
			
			// It's a mixed unit type:
			Type=ValueType.Mixed;
			
		}
		
		/// <summary>Are all entries in this rectangle the same?</summary>
		public bool AllSameValues(){
			
			if(InnerValues==null || InnerValues.Length<=1){
				return true;
			}
			
			// For each inner value, compare it to the first one.
			Css.Value first=InnerValues[0];
			
			// For each other one..
			for(int i=1;i<InnerValues.Length;i++){
				
				if(!InnerValues[i].Equals(first)){
					return false;
				}
				
			}
			
			return true;
		}
		
		/// <summary>Duplicates the given nibble (4 bit number) and places the result alongside in the same byte.
		/// E.g. c in hex becomes cc.</summary>
		/// <param name="nibble">The nibble to duplicate.</param>
		private int DoubleNibble(int nibble){
			return ((nibble<<4) | nibble);
		}
		
		/// <summary>Gets the numbered inner values text.</summary>
		/// <param name="index">The number of the inner value.</param>
		/// <returns>The text of the numbered inner value, if it's available. Null otherwise.</returns>
		public string GetText(int index){
			Value value=this[index];
			
			if(value==null){
				return null;
			}
			
			return value.Text;
		}
		
		/// <summary>Gets the numbered inner values floating point number in radians.</summary>
		/// <param name="index">The number of the inner value.</param>
		/// <returns>The floating point number of the numbered inner value, if it's available. 0 otherwise.</returns>
		public float GetRadians(int index){
			// Read the value:
			Value value=this[index];
			
			if(value==null){
				return 0f;
			}
			
			// Read the value:
			switch(value.Type){
				case ValueType.Pixels:
					
					// It's a pixel value:
					return (float)value.PX;
					
				case ValueType.Radians:
					
					// Great, it's already a radian value!
					return value.Single;
					
				case ValueType.Degrees:
					
					// It's a degree value:
					return value.Single*Mathf.Deg2Rad;
					
				default:
					// It's something else, but likely something like a percentage:
					return value.Single;
			}
			
		}
		
		/// <summary>Gets the numbered inner values floating point number.</summary>
		/// <param name="index">The number of the inner value.</param>
		/// <returns>The floating point number of the numbered inner value, if it's available. 0 otherwise.</returns>
		public float GetFloat(int index){
			Value value=this[index];
			
			if(value==null){
				return 0f;
			}
			
			if(value.Type==ValueType.Pixels){
				return (float)value.PX;
			}
			
			return value.Single;
		}
		
		/// <summary>Gets the numbered inner value as pixels.</summary>
		/// <param name="index">The number of the inner value.</param>
		/// <returns>The pixels from the numbered inner value, if it's available. 0 otherwise.</returns>
		public int GetPX(int index){
			Value value=this[index];
			
			if(value==null){
				return 0;
			}
			
			return value.PX;
		}
		
		/// <summary>Gets the numbered inner value as a colour.</summary>
		/// <param name="index">The number of the inner value.</param>
		/// <returns>The colour from the numbered inner value, if it's available. Clear otherwise.</returns>
		public Color GetColor(int index){
			Value value=this[index];
			
			if(value==null){
				return Color.clear;
			}
			
			return value.ToColor();
		}
		
		/// <summary>Converts this value into a hex string that is 2 characters long.</summary>
		public string ToHex(){
			if(Type==ValueType.Single){
				int value=(int)(Single*255f);
				
				return value.ToString("X2");
			}else{
				return PX.ToString("X2");
			}
		}
		
		/// <summary>Gets or sets the numbered inner value. Used if this is a rectangle/ colour etc.</summary>
		/// <param name="index">The number of the inner value.</param>
		public Value this[int index]{
			get{
				if(InnerValues==null){
					return null;
				}
				
				return InnerValues[index];
			}
			set{
				if(InnerValues==null){
					return;
				}
				
				InnerValues[index]=value;
			}
		}
		
		/// <summary>Sets defaults for the given number of inner values.</summary>
		/// <param name="count">The number of inner values to setup.</param>
		public void SetInnerValues(int count){
			if(InnerValues==null||InnerValues.Length!=count){
				InnerValues=new Value[count];
				
				for(int i=0;i<count;i++){
					InnerValues[i]=new Value();
				}
			}
		}
		
		/// <summary>Sets the given fixed number of inner values of this to the given css text string.</summary>
		/// <param name="valueText">The value to use for the inner values separated by spaces. E.g. "50% 50%"</param>
		/// <param name="count">The number of inner values this value should have. If the value text does not have
		/// enough chunks to fit in this number of inner values, the value text is repeated so they do.
		/// For example, "10 40" to fit in 4 inner values becomes "10 40 10 40"; "1 2 3" to fit in 4 becomes "1 2 3 1" etc.</param>
		public void SetInnerValues(string valueText,int count){
			string[] pieces=valueText.Split(' ');
			
			if(pieces.Length>count){
				count=pieces.Length;
			}
			
			SetInnerValues(count);
			int index=0;
			
			// Repeat our given values to fill out innervalues.
			foreach(Value value in InnerValues){
				string piece=pieces[index++];
				
				if(piece==""){
					value.Set("",ValueType.Pixels);
				}else{
					value.Set(piece);
				}
				
				if(index==pieces.Length){
					index=0;
				}
			}
		}
		
	}
	
}