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
using InfiniText;
using PowerUI;
using PowerUI.Css;
using Blaze;


namespace InfiniText{
	
	/// <summary>
	/// Represents a character within a font for display on the screen.
	/// Extends the InfiniText glyph object. This saves memory and avoids fragmentation.
	/// </summary>
	
	public partial class Glyph{
		
		/// <summary>A graphical image representation of this character for e.g. Emoji.</summary>
		public ImagePackage Image;
		/// <summary>The rendered location of this SDF character. Also tracks on-screen counts internally.</summary>
		public AtlasLocation Location;
		
		
		/// <summary>Called when this character goes on screen.</summary>
		public void OnScreen(){
			
			if(Location==null){
				Location=AtlasStacks.Text.RequireImage(this);
			}
			
			if(Location!=null){
				Location.UsageCount++;
			}
			
		}
		
		/// <summary>Called when this character goes on screen.</summary>
		public void OffScreen(){
			
			if(Location==null){
				return;
			}
			
			if(Location.DecreaseUsage()){
				Location=null;
			}
			
		}
		
		/// <summary>True if this is a rightwards character (e.g. Arabic).</summary>
		public bool Rightwards{
			get{
				return RightToLeft.Rightwards(RawCharcode);
			}
		}
		
		/// <summary>True if this character is a whitespace of any kind.</summary>
		public bool Space{
			get{
				// It's a "space" if it has no path.
				return (FirstPathNode==null);
			}
		}
		
		/// <summary>Called when an image is found for this character. Used by e.g. Emoji.</summary>
		/// <param name="package">The image that was found.</param>
		public void SetupImage(ImagePackage package){
			
			if(package==null){
				Image=null;
				return;
			}
			
			Image=package;
			
			// Great! We found this character somewhere else as an image:
			Width=Image.Width();
			Height=Image.Height();
			
			// And apply delta width too:
			AdvanceWidth=Width;
			
			// Queue up a global layout.
			UI.document.Renderer.RequestLayout();
			
		}
		
	}
	
}