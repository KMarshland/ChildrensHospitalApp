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


#if !UNITY_METRO

using System.Threading;

#endif


namespace PowerUI{
	
	/// <summary>
	/// Callbacks can be used to make sure certain things run on Unities main thread.
	/// To use them, you must inherit this callback class and override OnRun.
	/// When you create an instance, call instance.Go(); to request a run on the main thread.
	/// Note that order is not necessarily retained if you happen to call Go on the main thread anyway - it will run immediately.
	/// </summary>
	
	public class Callback{
		
		/// <summary>Stored as a linked list - this is the next callback.</summary>
		public Callback NextCallback;
		
		
		/// <summary>Sends this callback to the main queue. OnRun will later be called.</summary>
		public void Go(){
		
			#if UNITY_METRO && UNITY_EDITOR
			
			OnRun();
			return;
			
			#elif UNITY_METRO
			
			if(Environment.CurrentManagedThreadId==Callbacks.MainThread){
				OnRun();
				return;
			}
			
			Callbacks.Add(this);
			
			#else
			
			
			if(Thread.CurrentThread==Callbacks.MainThread){
				OnRun();
				return;
			}
			
			Callbacks.Add(this);
			
			#endif
			
		}
		
		/// <summary>True if callbacks will run immediately with no delay.</summary>
		public static bool WillRunImmediately{
			get{

				#if UNITY_METRO && UNITY_EDITOR
				
				return true;
				
				#elif UNITY_METRO
				
				return (Environment.CurrentManagedThreadId==Callbacks.MainThread);
				
				#else
			
				return (Thread.CurrentThread==Callbacks.MainThread);
				
				#endif
				
			}
		}
		
		/// <summary>True if callbacks will be delayed until the next callback run.</summary>
		public static bool WillDelay{
			get{
				
				#if UNITY_METRO && UNITY_EDITOR
				
				return false;
				
				#elif UNITY_METRO
				
				return (Environment.CurrentManagedThreadId!=Callbacks.MainThread);
				
				#else
	
				return (Thread.CurrentThread!=Callbacks.MainThread);
				
				#endif

			}
		}
		
		/// <summary>Always called on the main thread. Runs this callback now.</summary>
		public virtual void OnRun(){
		
		}
		
	}
	
}