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
using System.Reflection;
using System.Collections.Generic;
using Nitro;
using Wrench;


namespace PowerUI.Css{
	
	/// <summary>
	/// A global lookup of property name to property.
	/// CssHandlers are instanced globally and mapped to the property names they accept.
	/// Note that properties are not instanced per element.
	/// </summary>
	
	public static class CssProperties{
		
		/// <summary>The lookup itself. Matches property name (e.g. "color") to the property that will process it.</summary>
		public static Dictionary<string,CssProperty> Properties;
		/// <summary>All properties which apply directly to text. E.g. color,font-size etc.</summary>
		public static Dictionary<string,CssProperty> TextProperties;
		
		
		#if NETFX_CORE
		
		public static void Setup(){
			
			if(Properties!=null){
				return;
			}
			
			Properties=new Dictionary<string,CssProperty>();
			TextProperties=new Dictionary<string,CssProperty>();
			
			// For each type..
			foreach(TypeInfo type in Assemblies.Current.DefinedTypes){
				
				if(type.IsGenericType){
					continue;
				}
				
				if(type.IsSubclassOf(typeof(CssProperty))){
					// Got one! Add it now.
					Add((CssProperty)Activator.CreateInstance(type.AsType()));
				}
				
			}
			
			// Setup CSS functions (and units):
			CssFunctions.Setup();
			
		}
		
		#else
		
		/// <summary>Sets up the global lookup by searching for any classes which inherit from CssProperty.</summary>
		public static void Setup(Type[] allTypes){
			if(Properties!=null){
				return;
			}
			
			Properties=new Dictionary<string,CssProperty>();
			TextProperties=new Dictionary<string,CssProperty>();
			
			// Load any existing CssProperties.
			
			// For each type..
			for(int i=allTypes.Length-1;i>=0;i--){
				Type type=allTypes[i];
				
				if(type.IsGenericType){
					continue;
				}
				
				if(TypeData.IsSubclassOf(type,typeof(CssProperty))){
					// Got one! Add it now.
					Add((CssProperty)Activator.CreateInstance(type));
				}
				
			}
			
			// Setup CSS functions (and units):
			CssFunctions.Setup(allTypes);
			
		}
		
		#endif
		
		/// <summary>Gets the default CSS property.</summary>
		public static CssProperty Default(){
			return new CssProperty();
		}
		
		/// <summary>Adds a CSS property to the global set.
		/// This is generally done automatically, but you can also add one manually if you wish.</summary>
		/// <param name="property">The property to add.</param>
		/// <returns>True if adding it was successful.</returns>
		public static bool Add(CssProperty property){
			
			string[] tags=property.GetProperties();
			
			if(tags==null||tags.Length==0){
				return false;
			}
			
			for(int i=0;i<tags.Length;i++){
				// Grab the property:
				string propertyName=tags[i].ToLower();
				
				// Is it the first? If so, set the name:
				if(i==0){
					property.Name=propertyName;
				}
				
				// Add it to properties:
				Properties[propertyName]=property;
				
				if(property.IsTextual){
					// Add it to text properties too:
					TextProperties[propertyName]=property;
				}
			}
			
			return true;
		}
		
		/// <summary>Attempts to find the property with the given name.
		/// If it's not found, a default property which is known to exist can be returned instead.
		/// For example, property "color".</summary>
		/// <param name="property">The property to look for.</param>
		/// <param name="defaultProperty">If the given property is not found, this is used instead.</param>
		/// <returns>The global property object.</returns>
		public static CssProperty Get(string property,string defaultProperty){
			CssProperty globalProperty=Get(property);
			
			if(globalProperty==null){
				globalProperty=Get(defaultProperty);
			}
			
			return globalProperty;
		}
		
		/// <summary>Attempts to find the named property, returning the global property if it's found. This works with all properties as it resolve the index too.</summary>
		/// <param name="property">The property to look for.</param>
		/// <param name="innerIndex">Some properties are sets of properties, such as color and color-r. color-r is an "inner" property of color.</param>
		/// <returns>The global CssProperty if the property was found; Null otherwise.</returns>
		public static CssProperty Get(string property,out int innerIndex){
			
			// Resolve the inner index:
			innerIndex=Css.Value.GetInnerIndex(ref property);
			
			// Resolve the property:
			CssProperty globalProperty=null;
			Properties.TryGetValue(property,out globalProperty);
			
			return globalProperty;
		}
		
		/// <summary>Attempts to find the named property, returning the global property if it's found.</summary>
		/// <param name="property">The property to look for.</param>
		/// <returns>The global CssProperty if the property was found; Null otherwise.</returns>
		public static CssProperty Get(string property){
			CssProperty globalProperty=null;
			Properties.TryGetValue(property,out globalProperty);
			return globalProperty;
		}

	}
	
}