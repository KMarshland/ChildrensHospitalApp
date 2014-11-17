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
using Nitro;
using PowerUI.Css;
using System.Collections;
using System.Collections.Generic;


namespace PowerUI{
	
	/// <summary>
	/// The AnimationCompleted delegate is an alternative to using a Nitro callback when the animation is done.
	/// Used with the OnDone method.
	/// </summary>
	public delegate void AnimationCompleted(UIAnimation animation);
		
	/// <summary>
	/// Handles all actively animated css properties. Also keeps track of a single set
	/// of css properties as used in any animate method (<see cref="PowerUI.Element.animate"/>)
	/// and can be used to monitor the progress of an animation.
	/// </summary>
	
	public class UIAnimation{
		
		/// <summary>Actively animated properties are stored in a linked list. This is the tail of that list.</summary>
		public static AnimatedProperty LastProperty;
		/// <summary>Actively animated properties are stored in a linked list. This is the head of that list.</summary>
		public static AnimatedProperty FirstProperty;
		
		/// <summary>Removes all active animations.</summary>
		public static void Clear(){
			LastProperty=FirstProperty=null;
		}
		
		/// <summary>Called at the UI update rate to progress the currently animated properties.</summary>
		public static void Update(){
			if(FirstProperty==null){
				return;
			}
			
			AnimatedProperty current=FirstProperty;
			while(current!=null){
				current.Update();
				current=current.PropertyAfter;	
			}
		}
		
		/// <summary>Searches the current animated properties for the named property on the given element.</summary>
		/// <param name="animating">The element being animated.</param>
		/// <param name="property">The CSS property to look for. Note: Must not be a composite property such as color-overlay.
		/// Must be a full property such as color-overlay-r.</param>
		/// <returns>An AnimatedProperty if it was found; Null otherwise.</returns>
		public static AnimatedProperty GetAnimatedProperty(Element animating,string property){
			
			// Grab the inner index:
			int innerIndex=Css.Value.GetInnerIndex(ref property);
			
			// Get the property:
			return GetAnimatedProperty(animating,CssProperties.Get(property),innerIndex);
		}
		
		/// <summary>Searches the current animated properties for the named property on the given element.</summary>
		/// <param name="animating">The element being animated.</param>
		/// <param name="property">The CSS property to look for. Note: Must not be a composite property such as color-overlay.
		/// Must be a full property such as color-overlay-r.</param>
		/// <returns>An AnimatedProperty if it was found; Null otherwise.</returns>
		public static AnimatedProperty GetAnimatedProperty(Element animating,CssProperty property,int innerIndex){
			
			if(FirstProperty==null){
				return null;
			}
			
			AnimatedProperty current=FirstProperty;
			
			while(current!=null){
				if(current.Animating==animating && current.PropertyInfo==property && current.InnerIndex==innerIndex){
					return current;
				}
				
				current=current.PropertyAfter;
			}
			
			return null;
		}
		
		/// <summary>True if this animation decelerates.</summary>
		public bool Decelerate;
		/// <summary>The total time in seconds that this animation lasts for.</summary>
		public float TotalTime;
		/// <summary>The element being animated.</summary>
		public Element Animating;
		/// <summary>The time in seconds to begin decelerating at.</summary>
		public float DecelerateAt;
		/// <summary>True if this animation has finished; false otherwise.</summary>
		private bool FinishedPlaying;
		/// <summary>The time in seconds that the animation should remain at a constant speed for.</summary>
		public float ConstantSpeedTime;
		/// <summary>All current animations are stored in a linked list. This is the next one.</summary>
		public UIAnimation NextAnimation;
		/// <summary>A shortcut to the element style object.</summary>
		public ElementStyle ElementStyle;
		/// <summary>The time in seconds that this animation should accelerate for.</summary>
		public float TimeToAccelerateFor;
		/// <summary>The time in seconds that this animation should decelerate for.</summary>
		public float TimeToDecelerateFor;
		/// <summary>The method to call (a c# or JS method) when the animation is done playing.</summary>
		public AnimationCompleted OnComplete;
		/// <summary>The dynamic method (a nitro function) to call when this animation is done.</summary>
		public DynamicMethod<Nitro.Void> Done;
		
		
		/// <summary>Creates a new UIAnimation for animating CSS and immediately animates it.
		/// See <see cref="PowerUI.Element.animate"/>.</summary>
		/// <param name="animating">The element being animated.</param>
		/// <param name="properties">The CSS property string. Each property should define the value it will be when the animation is done.</param>
		/// <param name="constantSpeedTime">How long this animation lasts for at a constant speed.</param>
		/// <param name="timeToAccelerateFor">How long this animation accelerates for. Creates smooth animations when used.</param>
		/// <param name="timeToDecelerateFor">How long this animation decelerates for. Creates smooth animations when used.</param>
		public UIAnimation(Element animating,string properties,float constantSpeedTime,float timeToAccelerateFor,float timeToDecelerateFor){
			Animating=animating;
			ElementStyle=Animating.Style;
			
			if(string.IsNullOrEmpty(properties)){
				Wrench.Log.Add("No properties given to animate!");
				return;
			}
			
			if(constantSpeedTime<0f){
				constantSpeedTime=0f;
			}
			
			if(timeToAccelerateFor<0f){
				timeToAccelerateFor=0f;
			}
			
			if(timeToDecelerateFor<0f){
				timeToDecelerateFor=0f;
			}
			
			TotalTime=(timeToDecelerateFor + timeToAccelerateFor + constantSpeedTime);
			
			ConstantSpeedTime=constantSpeedTime;
			TimeToAccelerateFor=timeToAccelerateFor;
			TimeToDecelerateFor=timeToDecelerateFor;
			
			if(TotalTime==0f){
				// Instant - probably a fault somewhere in the users request, so we won't do anything.
				Wrench.Log.Add("Instant css animation request ignored. Told to take no time to transition.");
				return;
			}
			
			if(timeToDecelerateFor==0f){
				Decelerate=false;
			}else{
				Decelerate=true;
				DecelerateAt=timeToAccelerateFor + constantSpeedTime;
			}
			
			if( properties.StartsWith(".") || properties.StartsWith("#") ){
				
				// Targeting a selector, e.g. #fadedBox
				// First, get the selector style:
				Css.SelectorStyle selector=animating.Document.getStyleBySelector(properties);
				
				if(selector==null){
					return;
				}
				
				// Animate each property:
				foreach(KeyValuePair<CssProperty,Css.Value> kvp in selector.Properties){
					
					
					// Is it a composite property?
					Css.Value value=kvp.Value;
					
					// Grab the type:
					Css.ValueType type=value.Type;
					
					if(type==Css.ValueType.Null || type==Css.ValueType.Text){
						// Can't deal with either of these.
						continue;
					}
					
					if(type==Css.ValueType.Rectangle || type==Css.ValueType.Point || type==Css.ValueType.Color){
						
						// Animate it (note that we don't need to copy it):
						AnimateComposite(kvp.Key,value);
					}else{
						
						// Animate it (note that we don't need to copy it):
						Animate(kvp.Key,-1,value,true);
					}
					
				}
				
				return;
			}
			
			string[] propertySet=properties.Split(';');
			
			foreach(string currentProperty in propertySet){
				if(currentProperty==""){
					continue;
				}
				
				string[] keyValue=currentProperty.Split(Css.Style.Delimiter,2);
				
				if(keyValue.Length!=2){
					continue;
				}
				
				string key=keyValue[0].Trim();
				
				if(key=="opacity"){
					key="color-overlay-a";
				}
				
				// Grab the inner index:
				int innerIndex=Css.Value.GetInnerIndex(ref key);
				
				// Get the property:
				CssProperty property=CssProperties.Get(key);
				
				if(property==null){
					Wrench.Log.Add("Warning: CSS property '"+keyValue[0]+"' not found.");
					continue;
				}
				
				// Trim shouldn't be applied to inner-text's value, but we can't animate that anyway! This is all we need to do:
				string value=keyValue[1].Trim();
				
				// Key could be a composite property - for example padding, which needs to be broken down into it's individual inner elements (e.g. padding-left)
				
				Css.ValueType type;
				
				if(innerIndex==-1){
					type=Css.Value.TypeOf(property,ref value);
				}else{
					type=Css.Value.TypeOf(value);
				}
				
				if(type==Css.ValueType.Null || type==Css.ValueType.Text){
					// Can't deal with either of these.
					continue;
				}else if(type==Css.ValueType.Rectangle || type==Css.ValueType.Point || type==Css.ValueType.Color){
					// We have a composite property (and we're animating the whole thing).
					Css.Value tempValue=new Css.Value();
					tempValue.Set(value,type);
					
					// Animate it:
					AnimateComposite(property,tempValue);
				}else{
					Animate(property,innerIndex,new Css.Value(value,type),true);
				}
			}
		}
		
		private void AnimateComposite(CssProperty property,Css.Value value){
			
			bool isPoint=(value.Type==Css.ValueType.Point);
			
			Animate(property,0,value[0],false);
			Animate(property,1,value[1],isPoint);
			
			if(!isPoint){
				Animate(property,2,value[2],false);
				Animate(property,3,value[3],true);
			}
			
		}
		
		/// <summary>Called when this animation is finished.</summary>
		public void Finished(){
			if(FinishedPlaying){
				return;
			}
			
			FinishedPlaying=true;
			
			try{
				if(Done!=null){
					Done.Run(this);
				}
				if(OnComplete!=null){
					OnComplete(this);
				}
			}catch(Exception e){
				Wrench.Log.Add("Error running animation OnDone method: "+e);
			}
		}
		
		/// <summary>Starts animating the named property and target value. Must not be composite properties.
		/// (e.g. color-overlay-r instead of color-overlay)</summary>
		/// <param name="property">The property to update.</param>
		/// <param name="innerIndex">The inner index of the property to update.</param>
		/// <param name="value">The target value of the property.</param>
		/// <param name="updateCss">True if this property should update CSS/ the screen when it's progressed.</param>
		private void Animate(CssProperty property,int innerIndex,Css.Value value,bool updateCss){
			// Check if this property is already animated - if so, interrupt it and override with our new values.
			// There won't be many actively animated properties, so looping through the update queue is fast anyway.
			AnimatedProperty animProperty=GetAnimatedProperty(Animating,property,innerIndex);
			
			if(animProperty!=null){
				animProperty.Animate(this,value,ConstantSpeedTime,TimeToAccelerateFor,TimeToDecelerateFor,updateCss);
			}else{
				// Otherwise we want to create a new AnimatedProperty and stick it into the queue:
				animProperty=new AnimatedProperty(this,property,innerIndex,value,ConstantSpeedTime,TimeToAccelerateFor,TimeToDecelerateFor,updateCss);
				animProperty.AddToQueue();
			}
		}
		
		/// <summary>Calls the given function when the animation is over.</summary>
		public void OnDone(DynamicMethod<Nitro.Void> method){
			Done=method;
		}
		
		/// <summary>Calls the given delegate when the animation is over.</summary>
		public void OnDone(AnimationCompleted onComplete){
			OnComplete=onComplete;
		}
		
		/// <summary>Call this to halt the animation early. This internally will cause the finished event to run.</summary>
		public void Stop(){
			Stop(true);
		}
		
		/// <summary>Call this to halt the animation early.</summary>
		/// <param name="runEvent">Optionally run the finished event if there is one.</param>
		public void Stop(bool runEvent){
			
			if(FinishedPlaying){
				return;
			}
			
			// Find all properties belonging to this animation:
			AnimatedProperty current=FirstProperty;
			
			while(current!=null){
				
				// Grab the next one, just incase stop gets called:
				AnimatedProperty next=current.PropertyAfter;
				
				if(current.Animation==this){
					current.Stop();
				}
				
				// Hop to the next one:
				current=next;
			}
			
			if(runEvent){
				// Call finished:
				Finished();
			}
			
		}
		
	}
	
}