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

namespace Nitro{

	/// <summary>
	/// Helper classes for looking for modifiers such as *private* var.
	/// </summary>

	public static class Modifiers{
	
		/// <summary>Checks for a given modifier e.g. 'private' in the given fragment of code.</summary>
		/// <param name="fragment">The fragment to check.</param>
		/// <param name="modifier">The modifier to check for.</param>
		/// <returns>True if this fragment contained the given modifier. The fragment is also removed from the code.</returns>
		public static bool Check(CodeFragment fragment,string modifier){
			if(fragment==null||fragment.GetType()!=typeof(VariableFragment)){
				return false;
			}
			bool result=(((VariableFragment)fragment).Value==modifier);
			if(result){
				fragment.Remove();
			}
			return result;
		}
		
		/// <summary>Looks for 'private' before the given fragment. If found, it's also removed.</summary>
		/// <param name="fragment">The fragment to check before.</param>
		/// <param name="isPublic">True if private was not found.</param>
		public static void Handle(CodeFragment fragment,out bool isPublic){
			isPublic=!Modifiers.Check(fragment.PreviousChild,"private");
		}
		
		/// <summary>Skips over any modifiers found in the given code fragment, returning the first non-modifier fragment found.</summary>
		/// <param name="fragment">The place to start from.</param>
		/// <returns>The first non-modifier fragment found. May be the given fragment if it is not a modifier.</returns>
		public static CodeFragment Skip(CodeFragment fragment){
			if(fragment==null||fragment.GetType()!=typeof(VariableFragment)){
				return fragment;
			}
			VariableFragment vfragment=(VariableFragment)fragment;
			if(vfragment.Value=="private"){
				if(fragment.NextChild==null){
					vfragment.Error("Error: 'private' is an access modifier and it must be followed by something. (e.g. private var i..)");
				}
				return Skip(fragment.NextChild);
			}
			return fragment;
		}
		
	}
	
}