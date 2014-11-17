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
	/// Represents a texture atlas which is dynamically added to and modified.
	/// It internally tracks which textures it has to prevent frequent rebuilds.
	/// By using a texture atlas like this, many drawcalls can be compressed to one.
	/// It also supports dynamic textures. These write directly to the atlas pixels.
	/// </summary>

	public class TextureAtlas{
	
		/// <summary>The length of the sides of the atlas, in pixels.</summary>
		public int Dimension;
		/// <summary>The block of all pixels of this atlas.</summary>
		public Color32[] Pixels;
		/// <summary>True if any pixels were changed.</summary>
		public bool PixelChange;
		/// <summary>True if an image was removed from the atlas and it now has a 'hole'.</summary>
		public bool CanOptimize;
		/// <summary>The pixels of the atlas as a displayable texture.</summary>
		public Texture2D Texture;
		/// <summary>True if something failed to be added that would otherwise have fit.</summary>
		public bool OptimizeRequested;
		/// <summary>Images which are removed are stored in a linked list. This is the tail of the list.</summary>
		public AtlasLocation LastEmpty;
		/// <summary>Images which are removed are stored in a linked list. This is the head of the list.</summary>
		public AtlasLocation FirstEmpty;
		/// <summary>A map of current textures to their location on the atlas.</summary>
		public Dictionary<Texture2D,AtlasLocation> ActiveTextures;
		
		
		/// <summary>Generates a new texture atlas of size 1024px x 1024px.</summary>
		public TextureAtlas():this(1024){}
		
		/// <summary>Generates a new atlas with the given x/y dimension (all atlases are square).</summary>
		/// <param name="dimension">The length in pixels of the side of the atlas.</param>
		public TextureAtlas(int dimension){
			Dimension=dimension;
			Texture=new Texture2D(dimension,dimension,TextureFormat.ARGB32,false);
			Texture.filterMode=FilterMode.Point;
			Texture.wrapMode=TextureWrapMode.Clamp;
			Pixels=new Color32[dimension*dimension];
			Reset();
		}
		
		/// <summary>Clears all content from this atlas</summary>
		private void Reset(){
			FirstEmpty=LastEmpty=null;
			ActiveTextures=new Dictionary<Texture2D,AtlasLocation>();
			
			// Add the root atlas location (NB: it adds itself internally)
			AtlasLocation root=new AtlasLocation(this,0,0,Dimension,Dimension);
			
			// Immediately mark this as empty:
			root.AddToEmptySet();
		}
		
		/// <summary>Flushes changes to the pixel set to the texture.</summary>
		public void Flush(){
			if(!PixelChange){
				// No pixels changed anyway.
				return;
			}
			
			PixelChange=false;
			Texture.SetPixels32(Pixels);
			Texture.Apply(false);
		}
		
		/// <summary>Gets the location of the given texture on the atlas.</summary>
		/// <param name="texture">The texture to find.</param>
		/// <returns>The location on the atlas of the texture. Null if it wasn't found.</returns>
		public AtlasLocation Get(Texture2D texture){
			AtlasLocation result;
			ActiveTextures.TryGetValue(texture,out result);
			return result;
		}
		
		/// <summary>Adds a texture to the atlas if it's not already on it.</summary>
		/// <param name="texture">The texture to add.</param>
		/// <returns>The location of the texture on the atlas.</returns>
		public AtlasLocation Add(Texture2D texture){
			return Add(texture,texture.width,texture.height);
		}
		
		/// <summary>Adds the given texture to the atlas if it's not already on it,
		/// taking up a set amount of space on the atlas.</summary>
		/// <param name="texture">The texture to add.</param>
		/// <param name="width">The x amount of space to take up on the atlas.</param>
		/// <param name="height">The y amount of space to take up on the atlas.</param>
		/// <returns>The location of the texture on the atlas.</returns>
		private AtlasLocation Add(Texture2D texture,int width,int height){
			
			AtlasLocation result=Get(texture);
			
			if(result!=null){
				return result;
			}
			
			// Any chance of fitting at all?
			if(width>Dimension || height>Dimension){
				// Nope!
				return null;
			}
			
			// Fast check - was this texture recently removed? We might have the chance of restoring it.
			// Their added at the back, so naturally, start at the end of the empty set
			// and go back until we hit one with a null texture.
			
			AtlasLocation currentE=LastEmpty;
			while(currentE!=null){
				
				if(currentE.Texture==null){
					// Nope! Shame.
					break;
				}else if(currentE.Texture==texture){
					// Ace! Time to bring it back from the dead.
					currentE.Select(texture,width,height);
					ActiveTextures[texture]=currentE;
					return currentE;
				}
				
				currentE=currentE.EmptyBefore;
			}
			
			// Look for a spot to park this texture in the set of empty blocks.
			
			// The aim is to make it fit in the smallest empty block possible to save space.
			// This is done with a 'fitFactor' - this is simply the difference between the blocks area and the textures area.
			// We want this value to be as small as possible.
			int fitFactor=0;
			AtlasLocation currentAccepted=null;
			
			int area=width*height;
			
			AtlasLocation currentEmpty=FirstEmpty;
			
			while(currentEmpty!=null){
				int factor=currentEmpty.FitFactor(width,height,area);
				
				if(factor==0){
					// Perfect fit - break right now; can't beat that!
					currentAccepted=currentEmpty;
					break;
					
				}else if(factor!=-1){
					// We can possibly fit here - is it the current smallest?
					if(currentAccepted==null||factor<fitFactor){
						// Yep! select it.
						fitFactor=factor;
						currentAccepted=currentEmpty;
					}
				}
				
				currentEmpty=currentEmpty.EmptyAfter;
			}
			
			if(currentAccepted==null){
				// No space in this atlas to fit it in. Stop there.
				
				if(CanOptimize){
					// Request an optimise:
					OptimizeRequested=true;
				}
				
				return null;
			}
			
			ActiveTextures[texture]=currentAccepted;
			
			// And burn in the texture to the location (nb: it internally also writes the pixels to the atlas).
			currentAccepted.Select(texture,width,height);
			
			return currentAccepted;
		}
		
		/// <summary>Replaces one texture with another. Note that they must be the same size.</summary>
		/// <param name="texture">The target texture to replace.</param>
		/// <param name="with">The replacement texture.</param>
		/// <returns>The location of the texture on the atlas.</returns>
		public AtlasLocation Replace(Texture2D texture,Texture2D with){
			AtlasLocation result=Get(texture);
			ActiveTextures.Remove(texture);
			
			if(result==null){
				return Add(with);
			}
			
			ActiveTextures[with]=result;
			return result;
		}
		
		/// <summary>Removes a texture from the atlas.</summary>
		/// <param name="texture">The texture to remove.</param>
		public void Remove(Texture2D texture){
			if(texture==null){
				return;
			}
			
			AtlasLocation location=Get(texture);
			ActiveTextures.Remove(texture);
			
			if(location==null){
				return;
			}
			
			CanOptimize=true;
			
			// Make the location available:
			location.Deselect();
		}
		
		/// <summary>Optimizes the atlas by removing all 'holes' (removed images) from the atlas.
		/// It reconstructs the whole atlas (only when there are actually holes), so this method should be considered expensive.
		/// This is only ever called when we fail to add something to the atlas; Theres no performace issues of a non-optimized atlas.
		/// Instead it just simply has very fragmented space available.</summary>
		public void Optimize(){
			if(!CanOptimize){
				// It'll do as it is.
				return;
			}
			
			// Make sure it's not called again:
			CanOptimize=false;
			OptimizeRequested=false;
			
			Dictionary<Texture2D,AtlasLocation> allTextures=ActiveTextures;
			
			// Clear the textures and add in the starting empty location.
			Reset();
			
			// Next up, add them all back in, and that's it!
			// The optimizing comes from them trying to fit in the smallest possible gap they can when added.
			foreach(KeyValuePair<Texture2D,AtlasLocation> kvp in allTextures){
				Add(kvp.Key);
			}
		}
		
		/// <summary>Destroys this atlas when it's no longer needed.</summary>
		public void Destroy(){
			if(Texture!=null){
				GameObject.Destroy(Texture);
				Texture=null;
			}
		}
		
	}
	
}