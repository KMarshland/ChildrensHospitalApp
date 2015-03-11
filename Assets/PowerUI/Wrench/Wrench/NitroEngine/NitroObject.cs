//--------------------------------------
//         Nitro Script Engine
//          Wrench Framework
//
//        For documentation or 
//    if you have any issues, visit
//         nitro.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Nitro{
	
	/// <summary>
	/// Nitro Objects are used when the type of something cannot be computed or when prototype is in use.
	/// These are essentially flexible objects.
	/// </summary>
	
	public class NitroObject{
		

		/// <summary>The underlaying object type.</summary>
		#if NETFX_CORE
		private TypeInfo ObjectType;
		#else
		private Type ObjectType;
		#endif
		
		/// <summary>E.g. fieldinfo for the underlying object.</summary>
		private MemberInfo Member;
		/// <summary>The underlying object if there is one.</summary>
		public object UnderlayObject;
		/// <summary>The underlying prototype map for this object.</summary>
		private Dictionary<string,NitroObject> PrototypeMap;
		
		
		/// <summary>Creates a new NitroObject.</summary>
		/// <param name="underObject">The underlying object, if any.</param>
		public NitroObject(object underObject){
			UnderlayObject=underObject;
		}
		
		public NitroObject(MemberInfo member,object underObject){
			Member=member;
			UnderlayObject=underObject;
		}
		
		/// <summary>Gets the value of this object.</summary>
		public object GetValue(){
			
			if(Member!=null){
				
				FieldInfo field=Member as FieldInfo;
				
				if(field==null){
					
					PropertyInfo prop=Member as PropertyInfo;
					
					if(prop==null){
						
						// It's a method! Create and return a dynamic method.
						return Member as MethodInfo;
						
					}else{
						
						return prop.GetValue(UnderlayObject,null);
						
					}
					
				}else{
					
					return field.GetValue(UnderlayObject);
					
				}
				
			}
			
			return UnderlayObject;
		}
		
		/// <summary>Sets the value of this object.</summary>
		public void SetValue(object value){
			
			if(Member==null){
				
				UnderlayObject=value;
			
			}else{
				
				FieldInfo field=Member as FieldInfo;
				
				if(field==null){
					
					PropertyInfo prop=Member as PropertyInfo;
					
					if(prop==null){
						
						// A little wierd, but ok - let's roll with it!
						// (They just overwrote a method with something else - probably another method).
						UnderlayObject=value;
						Member=null;
						
					}else{
						
						prop.SetValue(UnderlayObject,value,null);
						
					}
					
				}else{
					
					field.SetValue(UnderlayObject,value);
					
				}
				
			}
			
		}
		
		/// <summary>Executes this as a dynamic method with the given arguments.</summary>
		/// <param name="arguments">The arguments to pass to the method.</param>
		/// <returns>The return value of the method, if any.</returns>
		public virtual object Run(params object[] arguments){
			
			NitroBaseMethod mtd=UnderlayObject as NitroBaseMethod;
			
			if(mtd==null){
				
				string underlay;
				
				if(UnderlayObject==null){
					underlay="[null]";
				}else{
					underlay=UnderlayObject.ToString();
				}
				
				throw new Exception("Unable to run "+underlay+" as a method. See stack trace for more.");
			}
			
			// Run it:
			object result=mtd.RunBoxed(arguments);
			
			if(result==null || result==typeof(Nitro.Void)){
				return null;
			}
			
			return result;
			
		}
		
		/// <summary>Gets/ sets entities from this object. The heart of the</summary>
		public object this[string name]{
			get{
				
				name=name.ToLower();
				
				NitroObject val;
				
				if(PrototypeMap!=null){
				
					if(PrototypeMap.TryGetValue(name,out val)){
						
						if(val==null){
							return val;
						}
						
						return val.GetValue();
					}
					
				}
				
				if(ObjectType==null){
					// This will error if null - this is OK.
					#if NETFX_CORE
					ObjectType=UnderlayObject.GetType().GetTypeInfo();
					#else
					ObjectType=UnderlayObject.GetType();
					#endif
				}
				
				#if NETFX_CORE
				MemberInfo[] set=null;
				#else
				MemberInfo[] set=ObjectType.GetMember(name,BindingFlags.Public|BindingFlags.Instance|BindingFlags.FlattenHierarchy|BindingFlags.IgnoreCase);
				#endif
				
				if(set==null || set.Length==0){
					return null;
				}
				
				// Grab the first one - it's the only one we can reasonably use.
				MemberInfo info=set[0];
				
				// Push the info into the prototype map for fast future access:
				val=new NitroObject(info,UnderlayObject);
				
				if(PrototypeMap==null){
					PrototypeMap=new Dictionary<string,NitroObject>();
				}
				
				PrototypeMap[name]=val;
				
				// Finally, read the value now:
				return val.GetValue();
				
			}
			set{
				
				name=name.ToLower();
				
				NitroObject val;
				
				if(PrototypeMap!=null && PrototypeMap.TryGetValue(name,out val)){
					
					if(val!=null){
						
						val.SetValue(value);
					
					}
					
					return;
					
				}
				
				if(UnderlayObject!=null){
					
					// We might be setting a field.
					
					if(ObjectType==null){
						
						#if NETFX_CORE
						ObjectType=UnderlayObject.GetType().GetTypeInfo();
						#else
						ObjectType=UnderlayObject.GetType();
						#endif
						
					}
					
					#if NETFX_CORE
					MemberInfo[] set=null;
					#else
					MemberInfo[] set=ObjectType.GetMember(name,BindingFlags.Public|BindingFlags.Instance|BindingFlags.FlattenHierarchy|BindingFlags.IgnoreCase);
					#endif
					
					if(set!=null && set.Length>0){
						
						// Grab the first one - it's the only one we can reasonably use.
						MemberInfo info=set[0];
						
						// Great:
						val=new NitroObject(info,UnderlayObject);
						
						// Cache for fast access:
						
						if(PrototypeMap==null){
							PrototypeMap=new Dictionary<string,NitroObject>();
						}
						
						PrototypeMap[name]=val;
						
						// Finally, actually set:
						val.SetValue(value);
						
						return;
						
					}
					
				}
				
				// Straight write:
				val=new NitroObject(value);
		
				if(PrototypeMap==null){
					PrototypeMap=new Dictionary<string,NitroObject>();
				}
				
				PrototypeMap[name]=val;
				
			}
		}
		
	}
	
}