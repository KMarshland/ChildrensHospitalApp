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
using System.Reflection;

namespace PowerUI{

	/// <summary>
	/// Represents code parsed and loaded from a script block.
	/// </summary>

	public class UICode:Script{
		
		/// <summary>The current element that script is relative to. Accessible in nitro as 'this'.</summary>
		public Element This;
		/// <summary>The window that contains the document.</summary>
		public Window window;
		/// <summary>The document the script belongs to. Accessible in nitro as 'document'.</summary>
		public Document document;
		#if !NoNitroRuntime
		/// <summary>The script this was compiled from.</summary>
		public NitroCode BaseScript;
		#endif
		
		/// <summary>Always called when the script is ready to go.</summary>
		public virtual void OnWindowLoaded(){}
		
		/// <summary>Always called when the script is ready to go.</summary>
		public virtual void Start(){}
		
		/// <summary>Can be called when the script is finished with.</summary>
		public virtual void OnWindowClose(){}
		
		/// <summary>Returns true if this code contains the method with the given name.</summary>
		public bool ContainsMethod(string methodName){
			if(string.IsNullOrEmpty(methodName)){
				return false;
			}
			
			#if !NoNitroRuntime
			if(BaseScript!=null){
				return BaseScript.ContainsMethod(methodName);
			}
			#endif
			
			try{
				// Grab the method off this type:
				
				#if NETFX_CORE
				MethodInfo mInfo=GetType().GetTypeInfo().GetDeclaredMethod(methodName.ToLower());
				#else
				MethodInfo mInfo=GetType().GetMethod(methodName.ToLower());
				#endif
				
				// Method is contained if we found something:
				return (mInfo!=null);
			}catch(AmbiguousMatchException){
				return true;
			}
		}
		
		/// <summary>Returns true if this code contains the field with the given name.</summary>
		public bool ContainsField(string fieldName){
			if(string.IsNullOrEmpty(fieldName)){
				return false;
			}
			
			#if !NoNitroRuntime
			if(BaseScript!=null){
				return BaseScript.ContainsField(fieldName);
			}
			#endif
			
			// Grab the field off this type:
			
			#if NETFX_CORE
			FieldInfo fInfo=GetType().GetTypeInfo().GetDeclaredField(fieldName.ToLower());
			#else
			FieldInfo fInfo=GetType().GetField(fieldName.ToLower());
			#endif
			
			// Field is contained if we found something:
			return (fInfo!=null);
		}
		
		/// <summary>Escapes the given string, essentially making any HTML it contains literal.</summary>
		public string escapeHTML(string html){
			return Wrench.Text.Escape(html);
		}
		
		/// <summary>Parses the given text into a number.</summary>
		public int parseInt(string text){
			if(string.IsNullOrEmpty(text)){
				return 0;
			}
			
			return int.Parse(text);
		}
		
		public UITimer setInterval(DynamicMethod<Nitro.Void> method,int ms){
			return new UITimer(false,ms,method);
		}
		
		public UITimer setTimeout(DynamicMethod<Nitro.Void> method,int ms){
			return new UITimer(true,ms,method);
		}
		
		public void clearInterval(UITimer timer){
			if(timer!=null){
				timer.Stop();
			}
		}
		
	}
	
}