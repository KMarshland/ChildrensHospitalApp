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

namespace PowerUI{

	/// <summary>
	/// A single css property being animated. Note that composite properties such as colours or rotations must be broken down into
	/// their inner properties (e.g. rgba, xyz). This is done internally by <see cref="PowerUI.UIAnimation"/>.
	/// </summary>
	
	public class AnimatedProperty{
		
		/// <summary>Which state the animation is at. Stage 0 = Accel, 1 = Constant speed, 2 = Decel.</summary>
		public int Stage;
		/// <summary>The current speed in units per second of the animation.</summary>
		public float Speed;
		/// <summary>The top speed in units per second of the animation that it will accelerate up to.</summary>
		public float MaxSpeed;
		/// <summary>True if this particular property should flush the output to the screen. This is required if
		/// there are multiple inner properties for a particular CSS property.</summary>
		public bool UpdateCss;
		/// <summary>The inner index of this value.</summary>
		public int InnerIndex;
		/// <summary>True if this animation should decelerate and generate a smooth stop.</summary>
		public bool Decelerate;
		/// <summary>The current time in seconds that has passed since the animation started.</summary>
		public float CurrentTime;
		/// <summary>The value that should be applied right now in terms of units.</summary>
		public float ActiveValue;
		/// <summary>The value that this property will be at the end of the animation.</summary>
		public float TargetValue;
		/// <summary>The acceleration rate in units per second.</summary>
		public float Acceleration;
		/// <summary>The deceleration rate in units per second.</summary>
		public float Deceleration;
		/// <summary>The parent animation that this property belongs to.</summary>
		public UIAnimation Animation;
		/// <summary>The CSS value object that the current value of this is applied to.</summary>
		public Css.Value ValueObject;
		/// <summary>The property being animated.</summary>
		public CssProperty PropertyInfo;
		/// <summary>The CSS property that this value is a part of. For example, if this property currently being animated is 
		/// the red component of the colour overlay, the property value object is the colour overlay as a whole.</summary>
		public Css.Value PropertyValueObject;
		/// <summary>Currently animated properties are stored in a linked list. This is the next one in the list.</summary>
		public AnimatedProperty PropertyAfter;
		/// <summary>Currently animated properties are stored in a linked list. This is the one before this in the list.</summary>
		public AnimatedProperty PropertyBefore;
		
		
		/// <summary>Creates a new animated property.</summary>
		/// <param name="animation">The animation that this property is a part of.</param>
		/// <param name="property">The property being animated.</param>
		/// <param name="targetValue">The value that this property will be when the animation is over.</param>
		/// <param name="constantSpeedTime">How long the animation will change the value at a constant speed for.</param>
		/// <param name="timeToAccelerateFor">How long the animation will accelerate for. This produces a smoother animation.</param>
		/// <param name="timeToDecelerateFor">How long the animation will decelerate for. This produces a smoother animation.</param>
		/// <param name="updateCss">True if this particular property should flush its changes to css/the screen.</param>
		public AnimatedProperty(UIAnimation animation,CssProperty property,int innerIndex,Css.Value targetValue,float constantSpeedTime,float timeToAccelerateFor,float timeToDecelerateFor,bool updateCss){
			Animation=animation;
			PropertyInfo=property;
			InnerIndex=innerIndex;
			Css.ValueType type=targetValue.Type;
			
			if(!Animation.ElementStyle.Properties.TryGetValue(property,out ValueObject)){
				ComputedStyle computed=Animation.ElementStyle.Computed;
				
				if(computed!=null && computed.Properties.TryGetValue(property,out ValueObject)){
					// Let's derive from the computed form.
					ValueObject=ValueObject.Copy();
				}else{
					ValueObject=new Css.Value();
					
					if(innerIndex!=-1 || type==Css.ValueType.Null){
						type=Css.ValueType.Rectangle;
					}
					
					property.SetDefault(ValueObject,type);
					
				}
				
				Animation.ElementStyle.Properties[property]=ValueObject;
				
			}
			
			if(ValueObject.Type==Css.ValueType.Inherit){
				// Special case - we need to duplicate it.
				Animation.ElementStyle.Properties[property]=ValueObject=ValueObject.Copy();
				ValueObject.Type=type;
			}
			
			PropertyValueObject=ValueObject;
			
			if(innerIndex!=-1){
				Css.Value innerValue=ValueObject[innerIndex];
				
				if(innerValue==null){
					ValueObject[innerIndex]=innerValue=new Css.Value();
				}
				
				ValueObject=innerValue;
			}
			
			// Set our starting value:
			ActiveValue=ValueObject.ToFloat();
			Animate(Animation,targetValue,constantSpeedTime,timeToAccelerateFor,timeToDecelerateFor,updateCss);
		}
		
		/// <summary>Animates this property now.</summary>
		/// <param name="animation">The animation that this property is a part of.</param>
		/// <param name="targetValue">The value that this property will be when the animation is over.</param>
		/// <param name="constantSpeedTime">How long the animation will change the value at a constant speed for.</param>
		/// <param name="timeToAccelerateFor">How long the animation will accelerate for. This produces a smoother animation.</param>
		/// <param name="timeToDecelerateFor">How long the animation will decelerate for. This produces a smoother animation.</param>
		/// <param name="updateCss">True if this particular property should flush its changes to css/the screen.</param>
		public void Animate(UIAnimation animation,string targetValue,float constantSpeedTime,float timeToAccelerateFor,float timeToDecelerateFor,bool updateCss){
			// Get the target float value for calculating our transition speeds as an object:
			Css.Value targetValueObject=new Css.Value();
			targetValueObject.Set(targetValue);
			
			Animate(animation,targetValueObject,constantSpeedTime,timeToAccelerateFor,timeToDecelerateFor,updateCss);
		}
		
		/// <summary>Animates this property now.</summary>
		/// <param name="animation">The animation that this property is a part of.</param>
		/// <param name="targetValue">The parsed value that this property will be when the animation is over.</param>
		/// <param name="constantSpeedTime">How long the animation will change the value at a constant speed for.</param>
		/// <param name="timeToAccelerateFor">How long the animation will accelerate for. This produces a smoother animation.</param>
		/// <param name="timeToDecelerateFor">How long the animation will decelerate for. This produces a smoother animation.</param>
		/// <param name="updateCss">True if this particular property should flush its changes to css/the screen.</param>
		public void Animate(UIAnimation animation,Css.Value targetValue,float constantSpeedTime,float timeToAccelerateFor,float timeToDecelerateFor,bool updateCss){
			
			Animation=animation;
			ValueObject.Type=targetValue.Type;
			
			Stage=0;
			Speed=0f;
			CurrentTime=0f;
			UpdateCss=updateCss;
			
			PropertyAfter=PropertyBefore=null;
			// Find the max speed. This is what we accelerate to.
			// Speed (y) / time (x) graph:
			//  /| |-| |\
			//	A   B   C. A = accelerate, b=constant, c=decelerate.
			// Max speed = top y value.
			// Distance travelled = area of the graph. This should match target - current value.
			
			TargetValue=targetValue.ToFloat();
			
			float unitsDelta=TargetValue - ActiveValue;
			
			MaxSpeed=(unitsDelta*UI.RedrawRate) / ( (0.5f*timeToAccelerateFor) + constantSpeedTime + (0.5f*timeToDecelerateFor) );
			
			if(timeToAccelerateFor==0f){
				// Skip acceleration stage.
				Stage=1;
				Speed=MaxSpeed;
			}else{
				Acceleration=MaxSpeed*UI.RedrawRate / timeToAccelerateFor;
			}
			
			if(timeToDecelerateFor!=0f){
				Deceleration=MaxSpeed*UI.RedrawRate / timeToDecelerateFor;
			}
		}
		
		public void Update(){
			CurrentTime+=UI.RedrawRate;
			
			// Get the element being animated:
			Element element=Animation.Animating;
			
			if(!element.isRooted){
				
				// Immediately stop - the element was removed (don't call the finished event):
				Stop();
				
				return;
				
			}
			
			if(CurrentTime>=Animation.TotalTime){
				// Done!
				
				// Remove from the update queue:
				Stop();
				
				// Make sure we are exactly the right value.
				ActiveValue=TargetValue;
				
				// Write it out to the CSS value:
				ValueObject.SetFloat(ActiveValue);
				
				if(UpdateCss){
					
					// If we're the main animation (updateCss is true), tell the style it changed:
					// Note that in grouped properties, only the last one actually runs the update.
					element.style.OnChanged(PropertyInfo,PropertyValueObject);
					
					// And call the done function:
					Animation.Finished();
					
				}
				
				// Stop there:
				return;
				
			}else{
				if(Stage==0){
					// Accelerating.
					Speed+=Acceleration;
					if(CurrentTime>=Animation.TimeToAccelerateFor){
						Speed=MaxSpeed;
						Stage++;
					}
				}
				
				if(Animation.Decelerate){
					if(Stage==2){
						// Decelerate.
						Speed-=Deceleration;
					}
					if(Stage==1 && CurrentTime>=Animation.DecelerateAt){
						// Start decelerating.
						Stage++;
					}
				}
				
				ActiveValue+=Speed;
				
				if((MaxSpeed<0 && ActiveValue<TargetValue) || (MaxSpeed>0 && ActiveValue>TargetValue)){
					ActiveValue=TargetValue;
				}
			}
			
			// Write it out to the CSS value:
			ValueObject.SetFloat(ActiveValue);
			
			if(UpdateCss){
				// And Tell the style it changed:
				// Note that in grouped properties, only the last one actually runs the update.
				element.style.OnChanged(PropertyInfo,PropertyValueObject);
			}
			
		}
		
		public void AddToQueue(){
			// Don't call if it's known to already be in the update queue.
			if(UIAnimation.FirstProperty==null){
				UIAnimation.FirstProperty=UIAnimation.LastProperty=this;
			}else{
				PropertyBefore=UIAnimation.LastProperty;
				UIAnimation.LastProperty = UIAnimation.LastProperty.PropertyAfter = this;
			}
		}
		
		public void Stop(){
			if(PropertyBefore==null){
				UIAnimation.FirstProperty=PropertyAfter;
			}else{
				PropertyBefore.PropertyAfter=PropertyAfter;
			}
			
			if(PropertyAfter==null){
				UIAnimation.LastProperty=PropertyBefore;
			}else{
				PropertyAfter.PropertyBefore=PropertyBefore;
			}
			
		}
		
		/// <summary>The prime property being animated.</summary>
		public string Property{
			get{
				return PropertyInfo.Name;
			}
		}
		
		/// <summary>The element being animated.</summary>
		public Element Animating{
			get{
				return Animation.Animating;
			}
		}
		
	}
	
}