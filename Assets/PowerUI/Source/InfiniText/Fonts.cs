//--------------------------------------
//             InfiniText
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using Blaze;
using System.Collections;
using System.Collections.Generic;


namespace InfiniText{
	
	/// <summary>A delegate used when InfiniText logs messages.</summary>
	public delegate void OnLogEvent(string text);
	
	/// <summary>The main class for handling global font settings.</summary>
	public static class Fonts{
		
		/// <summary>Should InfiniText use the OS/2 metrics? This generally implies "like Windows/IE" layout. "Like Firefox" is the default (false, HHEA metrics).</summary>
		public static bool UseOS2Metrics=false;
		/// <summary>A globally unique glyph ID. Used when placing glyphs on atlases.</summary>
		public static int GlyphID=1;
		/// <summary>The font size that auto-aliasing is relative to.</summary>
		public const float AutoAliasRelative=16f;
		/// <summary>The aliasing offset when auto-alias is used.</summary>
		public const float AutoAliasOffset=0.5f;
		/// <summary>The ramp used when auto aliasing is active.</summary>
		public const float AutoAliasRamp=0.01f;
		/// <summary>The current text aliasing value.</summary>
		public static float Aliasing=0.5f;
		/// <summary>The main rasteriser used for drawing fonts. Sets itself up as an SDF rasteriser by default.</summary>
		public static Scanner Rasteriser;
		/// <summary>Should font glyphs be loaded when the font is? If yes, font loading takes slightly longer (especially if you use large fonts) but you'll instead have the entire font loaded which can be useful.</summary>
		public static bool Preload=false;
		/// <summary>Should font faces be entirely deferred? If yes, font faces are loaded up only when they are first used (at which point preloading may also occur).</summary>
		public static bool DeferLoad=true;
		/// <summary>See InvertNormals.</summary>
		public static bool InvertedNormals=false;
		/// <summary>See SetExtrude. Not disabled by default.</summary>
		public static bool DisableExtrude=false;
		/// <summary>Change PixelHeight instead. The SDF draw height.</summary>
		public static float SdfPixelHeight=40f;
		/// <summary>The raw size of the default SDF spread. The bigger this is, the more anti-aliasing and bigger glows you can use.</summary>
		public static int SdfSize=4;
		/// <summary>The SDF outline location. 0-1.</summary>
		public static float OutlineLocation=0.74f;
		/// <summary>All available font families.</summary>
		public static Dictionary<string,FontFamily> All;
		/// <summary>The delegate used when InfiniText logs a message.</summary>
		public static OnLogEvent OnLog;
		
		
		/// <summary>Logs a message.</summary>
		public static void OnLogMessage(string message){
			
			if(OnLog!=null){
				OnLog(message);
			}
			
		}
		
		/// <summary>Wrapper function for loading a font from the given data. Note that loaded fonts are cached once loaded.</summary>
		public static FontFace Load(byte[] data){
			
			return FontLoader.Load(data);
			
		}
		
		/// <summary>Globally enables/ disables SDF extrusion. Active by default. In rare cases can cause fonts to go streaky or over-bold.</summary>
		public static void SetExtrude(bool active){
			
			// Invert as we have an isDisabled property:
			active=!active;
			
			DisableExtrude=active;
			
			foreach(KeyValuePair<string,FontFamily> font in All){
				font.Value.SetExtrude(active);
			}
			
		}
		
		/// <summary>Globally inverts the normals of font glyphs. Can be used to make letters more/less bold.</summary>
		public static void InvertNormals(){
			
			InvertedNormals=!InvertedNormals;
			
			foreach(KeyValuePair<string,FontFamily> font in All){
				font.Value.InvertNormals();
			}
			
		}
		
		/// <summary>Updates the bottom/ top alias values. Used by Aliasing and outlineLocation.</summary>
		private static void UpdateAliasValues(){
		
			#if !STANDALONE
			UnityEngine.Shader.SetGlobalFloat("BottomFontAlias",OutlineLocation-Aliasing);
			UnityEngine.Shader.SetGlobalFloat("TopFontAlias",OutlineLocation+Aliasing);
			#endif
			
		}
		
		/// <summary>The SDF draw height. Default is 32.</summary>
		public static float PixelHeight{
			get{
				return SdfPixelHeight;
			}
			set{
				SdfPixelHeight=value;
				
				if(Rasteriser!=null){
					Rasteriser.DrawHeight=(int)value;
				}
				
			}
		}
		
		/// <summary>Clears all fonts.</summary>
		public static void Clear(){
			All=null;
			GlyphID=1;
		}
		
		/// <summary>Sets up InfiniText. Called automatically when the first font is loaded.</summary>
		public static void Start(){
			
			if(Rasteriser!=null){
				return;
			}
			
			UpdateAliasValues();
			
			// Setup and start the rasteriser:
			Rasteriser=new Scanner();
			Rasteriser.SDFSize=SdfSize;
			Rasteriser.DrawHeight=(int)SdfPixelHeight;
			Rasteriser.Start();
			UpdateAliasValues();
			
		}
		
		/// <summary>Gets or creates a font family by name.</summary>
		public static FontFamily GetOrCreate(string name){
			
			FontFamily result=Get(name);
			
			if(result==null){
				
				if(Rasteriser==null){
					// Start now:
					Start();
				}
				
				result=new FontFamily(name);
				
				if(All==null){
					All=new Dictionary<string,FontFamily>();
				}
				
				All[name.ToLower()]=result;
				
			}
			
			return result;
		}
		
		/// <summary>Tries to get a font family by name.</summary>
		/// <returns>Null if the family isn't loaded.</returns>
		public static FontFamily Get(string name){
			
			if(All==null){
				return null;
			}
			
			// Lowercase the name:
			name=name.ToLower();
			
			FontFamily result;
			All.TryGetValue(name,out result);
			
			return result;
		}
		
	}

}