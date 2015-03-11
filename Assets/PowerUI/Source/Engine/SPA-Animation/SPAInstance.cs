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

namespace PowerUI{
	
	/// <summary>A callback which can be used to get notified each frame.</summary>
	public delegate void OnSPAProgress(int frameID,SPAInstance instance);
	
	/// <summary>
	/// A single instance of an SPA animation.
	/// It's instanced like this so that the same animation
	/// can be played back multiple times at once; the instance
	/// keeps track of which frame the animation is currently at.
	/// </summary>
	
	public class SPAInstance{
		
		/// <summary>True if the animation is over.</summary>
		public bool Done;
		/// <summary>Which frame ID we're up to in the current sprite.</summary>
		public int AtFrame;
		/// <summary>The animation this instance belongs to.</summary>
		public SPA Animation;
		/// <summary>True if this animation should loop.</summary>
		public bool Loop=true;
		/// <summary>The frame ID in the overall animation.</summary>
		public int OverallFrame;
		/// <summary>How long to wait before advancing the frame, in seconds.</summary>
		public float FrameDelay;
		/// <summary>How long we've waited so far since the frame was last advanced.</summary>
		public float CurrentDelay;
		/// <summary>The current sprite we're using. Contains the graphic itself.</summary>
		public SPASprite CurrentSprite;
		/// <summary>The material this instance is using.</summary>
		public Material AnimatedMaterial;
		/// <summary>Currently playing instances are stored in a linked list.
		/// This is the instance in the list after this one.
		/// <see cref="PowerUI.SPA.FirstInstance"/>
		/// </summary>
		public SPAInstance InstanceAfter;
		/// <summary>Currently playing instances are stored in a linked list.
		/// This is theinstance in the list before this one.
		/// <see cref="PowerUI.SPA.FirstInstance"/>
		/// </summary>
		public SPAInstance InstanceBefore;
		
		/// <summary>Creates a new playable instance of the given SPA animation.</summary>
		public SPAInstance(SPA animation){
			Animation=animation;
			AnimatedMaterial=new Material(SPA.IsolationShader);
			SetSprite(0);
			FrameDelay=1f/(float)Animation.FrameRate;
		}
		
		/// <summary>Advances the animation. Different animations may be at different
		/// frame rates, so this regulates the frame rate too.</summary>
		public void Update(){
			CurrentDelay+=Time.deltaTime;
			if(CurrentDelay<FrameDelay){
				return;
			}
			AtFrame++;
			OverallFrame++;
			CurrentDelay=0f;
			if(AtFrame==CurrentSprite.FrameCount){
				AtFrame=0;
				// Time for the next sprite.
				int nextSpriteID=CurrentSprite.ID+1;
				if(nextSpriteID==Animation.Sprites.Length){
					if(Loop){
						nextSpriteID=0;
						OverallFrame++;
					}else{
						Stop();
						return;
					}
				}
				SetSprite(nextSpriteID);
			}
			// Update the material offset.
			int imageX=AtFrame/CurrentSprite.VerticalFrameCount;
			int imageY=AtFrame-(imageX*CurrentSprite.VerticalFrameCount);
			AnimatedMaterial.SetTextureOffset(
												"_Sprite",
												new Vector2(
															imageX*CurrentSprite.TextureScale.x,
															imageY*CurrentSprite.TextureScale.y
															)
											);
		}
		
		/// <summary>Sets the sprite with the given ID as the active one.</summary>
		/// <param name="index">The ID of the sprite.</param>
		private void SetSprite(int index){
			CurrentSprite=Animation.Sprites[index];
			AnimatedMaterial.SetTexture("_Sprite",CurrentSprite.Sprite);
			// Update the material tiling:
			AnimatedMaterial.SetTextureScale("_Sprite",CurrentSprite.TextureScale);
		}
		
		/// <summary>Stops this instance. If you continue to display it after this is called,
		/// the animation will appear frozen on the last frame.</summary>
		public void Stop(){
			if(Done){
				return;
			}
			Done=true;
			
			// Remove this from the queue so that it's not updated anymore:
			if(InstanceBefore==null){
				SPA.FirstInstance=InstanceAfter;
			}else{
				InstanceBefore.InstanceAfter=InstanceAfter;
			}
			
			if(InstanceAfter==null){
				SPA.LastInstance=InstanceBefore;
			}else{
				InstanceAfter.InstanceBefore=InstanceBefore;
			}
		}
		
	}
	
}