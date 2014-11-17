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
using System.Threading;
using Nitro;


namespace PowerUI{
	
	/// <summary> Used as an alternative to Nitro for UI timer events.</summary>
	public delegate void OnUITimer();
	
	/// <summary>
	/// Provides a way of interrupting a timer event.
	/// Returned by <see cref="PowerUI.UICode.setTimeout"/> and <see cref="PowerUI.UICode.setInterval"/>.
	/// This object can be passed to <see cref="PowerUI.UICode.clearInterval"/> to prevent any further timing events.
	/// </summary>
	
	public class UITimer{
		
		/// <summary>True if this is a one off timer event and not an interval.</summary>
		public bool OneOff;
		#if UNITY_WP8
		/// <summary>The system timer that will time the callback.</summary>
		public System.Threading.Timer InternalTimer;
		#elif UNITY_METRO
		// No timer appears to be available on Windows 8 :(
		public object InternalTimer;
		#else
		/// <summary>The system timer that will time the callback.</summary>
		public System.Timers.Timer InternalTimer;
		#endif
		/// <summary>An alternative to the callback. A delegate called when the time is up.</summary>
		public event OnUITimer OnComplete;
		/// <summary>The callback (A nitro method) to run when the time is up.</summary>
		public DynamicMethod<Nitro.Void> Callback;
		
		
		/// <summary>Creates a new timer defining how long to wait, the callback to run and if its an interval or not.</summary>
		/// <param name="oneOff">True if this timer is a single event. False will result in the callback being run until it's stopped.</param>
		/// <param name="interval">The time in milliseconds between callbacks.</param>
		/// <param name="callback">The callback (A nitro method) to run when the time is up.</param>
		public UITimer(bool oneOff,int interval,DynamicMethod<Nitro.Void> callback){
			Callback=callback;
			Setup(oneOff,interval);
		}
		
		/// <summary>Creates a new timer defining how long to wait, the callback to run and if its an interval or not.</summary>
		/// <param name="oneOff">True if this timer is a single event. False will result in the callback being run until it's stopped.</param>
		/// <param name="interval">The time in milliseconds between callbacks.</param>
		/// <param name="callback">The callback (A delegate) to run when the time is up.</param>
		public UITimer(bool oneOff,int interval,OnUITimer callback){
			OnComplete+=callback;
			Setup(oneOff,interval);
		}
		
		private void Setup(bool oneOff,int interval){
			if(interval<=0){
				throw new Exception("Invalid timing interval or callback.");
			}
			
			if(Callback==null && OnComplete==null){
				throw new Exception("A callback must be provided for timer methods.");
			}
			
			OneOff=oneOff;
			#if UNITY_WP8
			InternalTimer=new System.Threading.Timer(Elapsed,null,0,interval);
			#elif UNITY_METRO
			#else
			InternalTimer=new System.Timers.Timer();
			InternalTimer.Elapsed+=Elapsed;
			InternalTimer.Interval=interval;
			InternalTimer.Start();
			#endif
			
		}
		
		/// <summary>Stops this timer from running anymore.</summary>
		public void Stop(){
			if(InternalTimer==null){
				return;
			}
			
			#if UNITY_WP8
			InternalTimer.Dispose();
			#elif UNITY_METRO
			#else
			InternalTimer.Enabled=false;
			#endif
			
			InternalTimer=null;
		}
		
		#if UNITY_WP8 || UNITY_METRO
		/// <summary>Windows Phone 8. The method called when the system timer has waited for the specified interval.</summary>
		private void Elapsed(object state){

		#else

		/// <summary>The method called when the system timer has waited for the specified interval.</summary>
		private void Elapsed(object sender,System.Timers.ElapsedEventArgs e){
		
		#endif
		
			if(OneOff){
				Stop();
			}
			if(Callback!=null){
				Callback.Run();
			}
			if(OnComplete!=null){
				OnComplete();
			}
		}
		
	}
	
}