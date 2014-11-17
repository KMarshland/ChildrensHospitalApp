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
using System.Collections.Generic;

namespace PowerUI{

	/// <summary>
	/// Represents a font suitable for displaying text on the screen with.
	/// There is one of these per <see cref="PowerUI.Renderman"/>.
	/// </summary>

	public class DynamicFont{
		
		/// <summary>The default font size, as set in the fonts themselves.</summary>
		public static float DefaultSize=16;
		
		/// <summary>Creates a new font by loading the Unity font from resources.</summary>
		/// <param name="name">The name of the font to load.</param>
		/// <param name="styleID">The styleID (bold, italic, bolditalic) to use. Utilized when rendering large text.</param>
		/// <returns>A new dynamic font.</returns>
		public static DynamicFont LoadFrom(string name,Renderman renderer){
			if(name==null || renderer.RootDocument.AotDocument){
				return null;
			}
			
			int styleID=0;
			
			if(name.EndsWith(" BoldItalic")){
				styleID=3;
			}else if(name.EndsWith(" Bold")){
				styleID=2;
			}else if(name.EndsWith(" Italic")){
				styleID=1;
			}
			
			Font font=Resources.Load(name) as Font;
			
			if(font==null){
				// Is it arial?
				if(name.ToLower()=="arial"){
					name="Arial";
					font=(Font)Resources.GetBuiltinResource(typeof(Font),"Arial.ttf");
				}else{
					return null;
				}
			}
			
			return new DynamicFont(font,name,styleID,renderer);
		}
		
		/// <summary>The unity font.</summary>
		public Font RawFont;
		/// <summary>Used to define e.g. a custom Bold, Italic or BoldItalic font.</summary>
		public int StyleID;
		/// <summary>The name of the font.</summary>
		public string Name;
		/// <summary>A character which represents a whitespace.</summary>
		public DynamicCharacter Space;
		/// <summary>The set of all styled character sets. Normal, Bold, Italic, Bold and Italic.</summary>
		public DynamicCharSet[] CharacterSets=new DynamicCharSet[4];
		/// <summary>The height of the ascending section of characters at the default font size.</summary>
		public float Ascend;
		/// <summary>The height of the descending section of characters at the default font size.</summary>
		public float Descend;
		/// <summary>The height of the meanline at the default font size.</summary>
		public float Meanline;
		/// <summary>The height of the baseline at the default font size.</summary>
		public float Baseline;
		/// <summary>The height of a character above the baseline at the default fontsize.</summary>
		public float FullAscend;
		/// <summary>The renderer this font belongs to.</summary>
		public Renderman Renderer;
		/// <summary>The total height of the largest possible character at the default fontsize.</summary>
		public float CharacterSize;
		/// <summary>True if the font atlas has been rebuilt and all characters must change.</summary>
		public bool RebuildRequired;
		/// <summary>The texture that this fonts characters are rendered to.</summary>
		public Texture2D FontTexture;
		/// <summary>True if this font is actively preparing for layout.</summary>
		private bool PreparingForLayout;
		
		
		/// <summary>Creates a new displayable font from the given unity font
		/// for use with the given renderer.</summary>
		/// <param name="font">The unity font to display text with.</param>
		/// <param name="renderer">The renderer which will display the text.</param>
		public DynamicFont(Font font,string name,int styleID,Renderman renderer){
			Name=name;
			RawFont=font;
			StyleID=styleID;
			Renderer=renderer;
			// This ones a hybrid - the font is being applied to the same mesh as things like images with the help of a renderman.
			UpdateTexture();
			
			RawFont.textureRebuildCallback+=OnTextureRebuild;
			Space=new DynamicCharacter();
			int size=(int)DefaultSize;
			RawFont.RequestCharactersInTexture("agl",size,FontStyle.Normal);
			
			DynamicCharacter aChar=GetCharacter('a',size,0,true);
			Meanline=aChar.Height;
			
			DynamicCharacter jChar=GetCharacter('g',size,0,true);
			Descend=jChar.Height-Meanline;
			
			DynamicCharacter lChar=GetCharacter('l',size,0,true);
			Ascend=lChar.Character.vert.y - aChar.Character.vert.y;
			
			Baseline=aChar.Character.vert.yMax; // -9. Characters scale about this.
			
			FullAscend=Meanline+Ascend;
			CharacterSize=FullAscend+Descend;
			
		}
		
		public FilterMode FilterMode{
			get{
				return FontTexture.filterMode;
			}
			set{
				FontTexture.filterMode=value;
			}
		}
		
		public DynamicFont GetAlternateFont(int styleID){
			if(StyleID==styleID){
				return this;
			}
			
			// Name includes 'my' style - e.g. it could be "Vera Bold".
			string name=Name;
			
			// Trim the current style tag from the name of this font:
			if(StyleID!=0){
				int length=name.Length;
				if(StyleID==3){
					// BoldItalic (length is 11 with space).
					name=name.Substring(length-11);
				}else if(StyleID==2){
					// Bold (length is 5 with space).
					name=name.Substring(length-5);
				}else if(StyleID==1){
					// Italic (length is 7 with space).
					name=name.Substring(length-7);
				}
			}
			
			// And next up append the one we want:
			if(styleID==3){
				name+=" BoldItalic";
			}else if(styleID==2){
				name+=" Bold";
			}else if(styleID==1){
				name+=" Italic";
			}
			
			DynamicFont result=Renderer.GetOrCreateFont(name);
			
			if(result!=null){
				return result;
			}
			
			return this;
		}
		
		/// <summary>Called when the font atlas is rebuilt.</summary>
		private void OnTextureRebuild(){
			UpdateTexture();
			RebuildRequired=true;
			if(!PreparingForLayout){
				Renderer.RequestLayout();
			}
		}
		
		/// <summary>Gets the full ascend above the baseline of a character at the given fontsize.</summary>
		/// <param name="fontSize">The fontsize to use.</param>
		/// <returns>The full ascent height.</returns>
		public float GetFullAscend(int fontSize){
			return FullAscend*fontSize/DefaultSize;
		}
		
		/// <summary>Gets the standard size of a space for the given font size.</summary>
		/// <param name="fontSize">The size of the font.</param>
		/// <returns>The space size.</returns>
		public float GetSpaceSize(int fontSize){
			return (float)fontSize/3f;
		}
		
		/// <summary>Gets the ascent of a character at the given fontsize.</summary>
		/// <param name="fontSize">The fontsize to use.</param>
		/// <returns>The ascent height.</returns>
		public float GetAscend(int fontSize){
			return Ascend*fontSize/DefaultSize;
		}
		
		/// <summary>Gets the descent of a character at the given fontsize.</summary>
		/// <param name="fontSize">The fontsize to use.</param>
		/// <returns>The descent.</returns>
		public float GetDescend(int fontSize){
			return Descend*fontSize/DefaultSize;
		}
		
		/// <summary>Gets the meanline height at the given fontsize.</summary>
		/// <param name="fontSize">The fontsize to use.</param>
		/// <returns>The meanline height.</returns>
		public float GetMeanline(int fontSize){
			return Meanline*fontSize/DefaultSize;
		}
		
		/// <summary>Gets the maximum height of a character at the given fontsize.</summary>
		/// <param name="fontSize">The fontsize to use.</param>
		/// <returns>The maximum height.</returns>
		public float GetHeight(int fontSize){
			return CharacterSize*fontSize/DefaultSize;
		}
		
		/// <summary>Updates the given character with the raw character info from the font.</summary>
		/// <param name="character">The character to display.</param>
		/// <param name="fontSize">The fontsize to display it at.</param>
		/// <param name="styleID">The style of the character:
		/// 0 = Normal
		/// 1 = Bold
		/// 2 = Italic
		/// 3 = Bold Italic</param>
		/// <param name="isAvailable">Assumes the character is available and searches for it's raw info.</param>
		/// <returns>The displayable character.</returns>
		public DynamicCharacter GetCharacter(char character,int fontSize,int styleID,bool isAvailable){
			
			DynamicCharacter result=GetCharacter(character,fontSize,styleID);
			
			FontStyle fontStyle=CharacterSets[styleID].Style;
			
			CharacterInfo charInfo;
			RawFont.GetCharacterInfo(character,out charInfo,fontSize,fontStyle);
			result.SetCharacter(charInfo);
			
			return result;
		}
		
		/// <summary>Gets a displayable character as a surrogate pair from this font.</summary>
		/// <param name="lowCharacter">The low surrogate character to display.</param>
		/// <param name="highCharacter">The high surrogate character to display.</param>
		/// <param name="fontSize">The fontsize to display it at.</param>
		/// <param name="styleID">The style of the character:
		/// 0 = Normal
		/// 1 = Bold
		/// 2 = Italic
		/// 3 = Bold Italic</param>
		/// <returns>The displayable character.</returns>
		public DynamicCharacter GetCharacter(char lowCharacter,char highCharacter,int fontSize,int styleID){ 
			
			// Does this character exist in our lookups?
			// If it does, return it straight away.
			// Otherwise, add it to our lookups and return that - we also need to flag a change has occured.
			// This will then result in that character being requested.
			
			DynamicCharSet charSet=CharacterSets[styleID];
			if(charSet==null){
				charSet=CharacterSets[styleID]=new DynamicCharSet(this,styleID);
			}
			
			return charSet.GetCharacter(lowCharacter,highCharacter,fontSize);
		}
		
		/// <summary>Gets a displayable character from this font.</summary>
		/// <param name="character">The character to display.</param>
		/// <param name="fontSize">The fontsize to display it at.</param>
		/// <param name="styleID">The style of the character:
		/// 0 = Normal
		/// 1 = Bold
		/// 2 = Italic
		/// 3 = Bold Italic</param>
		/// <returns>The displayable character.</returns>
		public DynamicCharacter GetCharacter(char character,int fontSize,int styleID){ 
			if(character==' '){
				return Space;
			}
			
			// Does this character exist in our lookups?
			// If it does, return it straight away.
			// Otherwise, add it to our lookups and return that - we also need to flag a change has occured.
			// This will then result in that character being requested.
			
			DynamicCharSet charSet=CharacterSets[styleID];
			if(charSet==null){
				charSet=CharacterSets[styleID]=new DynamicCharSet(this,styleID);
			}
			
			return charSet.GetCharacter(character,fontSize);
		}
		
		/// <summary>Prepares all text for an allocation by setting them as all being not in use.</summary>
		public void PrepareForAllocate(){
			for(int i=0;i<4;i++){
				DynamicCharSet charSet=CharacterSets[i];
				if(charSet==null){
					continue;
				}
				charSet.PrepareForAllocate();
			}
		}
		
		public void UpdateTexture(){
			FontTexture=GetTexture();
			FontTexture.filterMode=FilterMode.Point;
			FontTexture.wrapMode=TextureWrapMode.Clamp;
			FontTexture.anisoLevel=0;
		}
		
		/// <summary>When the font atlas rebuilds, all characters must be re-requested. This method re-requests all sets of characters.</summary>
		public void RebuildAll(){
			RebuildRequired=false;
			RebuildUpto(null);
		}
		
		/// <summary>Called after PrepareForLayout. Makes sure all characters in the buffer are up-to-date.</summary>
		/// <param name="infoRequired">True if HasCharacterInfo must be false for a request to occur. Requested anyway if false.</param>
		public void RefreshBufferedCharacters(bool infoRequired){
			for(int i=0;i<4;i++){
				DynamicCharSet charSet=CharacterSets[i];
				if(charSet==null){
					continue;
				}
				charSet.RefreshBufferedCharacters(infoRequired);
			}
		}
		
		/// <summary>When the font atlas rebuilds, all characters before the set that caused the rebuild must be re-requested.
		/// This method re-requests all sets up to the given one (which should be the one that caused a rebuild).</summary>
		/// <param name="stopAt">The set to stop rebuilding at (exclusive).</param>
		public void RebuildUpto(FontSizeCharSet stopAt){
			for(int i=0;i<4;i++){
				DynamicCharSet charSet=CharacterSets[i];
				if(charSet==null){
					continue;
				}
				if(charSet.RebuildUpto(stopAt)){
					return;
				}
			}
		}
		
		/// <summary>Called before the characters are displayed.
		/// It ensures they are all ready to go and on the font atlas.</summary>
		public void PrepareForLayout(){
			
			PreparingForLayout=true;
			
			// Firstly, request all characters in the font.
			for(int i=0;i<4;i++){
				DynamicCharSet charSet=CharacterSets[i];
				if(charSet==null){
					continue;
				}
				charSet.PrepareForLayout();
			}
			
			// Flag used for knowing if we're actively preparing font sets for layout.
			PreparingForLayout=false;
			
			if(RebuildRequired){
				// If the font texture rebuilds, we'll need to re-request all the characters.
				RebuildAll();
				RefreshBufferedCharacters(false);
			}else{
				// Next up, refresh any buffered characters to make sure their up-to-date.
				RefreshBufferedCharacters(true);
			}
		}
		
		/// <summary>Gets the unity font atlas texture.</summary>
		public Texture2D GetTexture(){
			return (Texture2D)(RawFont.material.mainTexture);
		}
		
	}
	
}