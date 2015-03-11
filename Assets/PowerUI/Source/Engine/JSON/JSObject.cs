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

namespace PowerUI{

	/// <summary>
	/// An object returned from JSON text.
	/// </summary>

	public class JSObject{
		
		/// <summary>The number of values in this object.</summary>
		public virtual int length{
			get{
				return 0;
			}
		}
		
		/// <summary>Adds the given string to this object.</summary>
		public void push(string value){
			push(new JSValue(value));
		}
		
		/// <summary>Adds the given JSON object to this object.</summary>
		public virtual void push(JSObject value){
		}
		
		/// <summary>Gets or sets entries from this object.</summary>
		public virtual JSObject this[string index]{
			get{
				return null;
			}
			set{
			}
		}
		
		/// <summary>Turns this JSON object into a JSON formatted string.</summary>
		public virtual string ToJSONString(){
			return "";
		}
		
	}
	
}