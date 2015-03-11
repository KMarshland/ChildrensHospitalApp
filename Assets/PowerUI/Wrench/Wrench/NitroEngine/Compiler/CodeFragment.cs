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
using System.Reflection.Emit;
using System.Collections.Generic;
using Wrench;

namespace Nitro{
	
	/// <summary>
	/// Represents a fragment of the text input code. Once compiled, it becomes a CompiledFragment.
	/// CompiledFragments are also types of CodeFragment as this allows them to be parented into the original structure.
	/// </summary>
	
	public class CodeFragment{
		
		/// <summary>Returns the index in the given array of the given character if it's found in it.</summary>
		/// <param name="arrayToSearch">The set of characters to look through.</param>
		/// <param name="charToFind">The character to find in the set.</param>
		/// <returns>The index in the set, if found; -1 otherwise.</returns>
		public static int IsOfType(char[] arrayToSearch,char charToFind){
			for(int i=0;i<arrayToSearch.Length;i++){
				if(arrayToSearch[i]==charToFind){
					return i;
				}
			}
			return -1;
		}
		
		
		/// <summary>A type (:TYPE) which was explicitly given to this block.</summary>
		public TypeFragment GivenType;
		/// <summary>The child after this one.</summary>
		public CodeFragment NextChild;
		/// <summary>The last child of this fragment, stored as a linked list.</summary>
		public CodeFragment LastChild;
		/// <summary>The first child of this fragment, stored as a linked list.</summary>
		public CodeFragment FirstChild;
		/// <summary>The child before this one.</summary>
		public CodeFragment PreviousChild;
		/// <summary>The parent fragment of this, if any.</summary>
		public CodeFragment ParentFragment;
		
		/// <summary>Throws an error, outputting the line number it occured on.</summary>
		/// <param name="message">A message saying why this error occured.</param>
		public void Error(string message){
			throw new CompilationException(message);
		}
		
		/// <summary>Gets the line number if there is one.</summary>
		public virtual int GetLineNumber(){
			
			if(ParentFragment==null){
				return -1;
			}
			
			return ParentFragment.GetLineNumber();
			
		}
		
		/// <summary>A value which states if this fragment accesses members (methods/fields) of something.</summary>
		/// <returns>True if this fragment is a PropertyOperation or a MethodOperation; false otherwise.</returns>
		public virtual bool IsMemberAccessor(){
			return false;
		}
		
		/// <summary>Attempts to compile this fragment into the given method.</summary>
		/// <param name="method">The method to compile it into.</param>
		public virtual CompiledFragment Compile(CompiledMethod method){
			return null;
		}
		
		/// <summary>Adds this code fragment to the beginning of the given parents child set.</summary>
		/// <param name="parent">The parent to parent this fragment to.</param>
		public void AddToStart(CodeFragment parent){
			if(parent==null){
				return;
			}
			ParentFragment=parent;
			if(parent.FirstChild==null){
				parent.FirstChild=parent.LastChild=this;
			}else{
				NextChild=parent.FirstChild;
				parent.FirstChild=NextChild.PreviousChild=this;
			}
		}
		
		/// <summary>Adds this code fragment as a child before the given one.</summary>
		/// <param name="frag">The fragment to add this before.</param>
		public void AddBefore(CodeFragment frag){
			AddAfter(frag);
			frag.Remove();
			frag.AddAfter(this);
		}
		
		/// <summary>Adds this code fragment as a child after the given one.</summary>
		/// <param name="frag">The fragment to add this one after.</param>
		public void AddAfter(CodeFragment frag){
			if(frag==null||frag.ParentFragment==null){
				// It's not added to anything anyway.
				return;
			}
			ParentFragment=frag.ParentFragment;
			NextChild=frag.NextChild;
			if(NextChild!=null){
				NextChild.PreviousChild=this;
			}
			frag.NextChild=this;
			PreviousChild=frag;
		}
		
		/// <summary>Removes this fragment from its parent.</summary>
		public void Remove(){
			if(ParentFragment==null){
				return;
			}
			if(ParentFragment.FirstChild==this){
				ParentFragment.FirstChild=NextChild;
			}
			if(ParentFragment.LastChild==this){
				ParentFragment.LastChild=PreviousChild;
			}
			
			if(NextChild!=null){
				NextChild.PreviousChild=PreviousChild;
			}
			
			if(PreviousChild!=null){
				PreviousChild.NextChild=NextChild;
			}
			
			PreviousChild=NextChild=null;
		}
		
		/// <summary>Adds the given fragment as a child of this one.</summary>
		/// <param name="child">The fragment to add as a child.</param>
		public void AddChild(CodeFragment child){
			
			if(child.GetType()==typeof(OperationFragment) && !child.IsParent){
				// Don't add empty operations
				return;
			}
			
			child.ParentFragment=this;
			
			if(FirstChild==null){
				FirstChild=LastChild=child;
			}else{
				child.PreviousChild=LastChild;
				LastChild=LastChild.NextChild=child;
			}
			
		}
		
		/// <summary>Defines if a fragment can be given a :TYPE. E.g. Brackets (for casting, (A):TYPE) and variables can.</summary>
		/// <returns>True if it can, false otherwise.</returns>
		public virtual bool Typeable(){
			return false;
		}
		
		/// <summary>Is this fragment a parent or not?</summary>
		/// <returns>True if it is, false otherwise.</returns>
		public bool IsParent{
			get{
				return (FirstChild!=null);
			}
		}
		
		/// <summary>How many children does this fragment have?</summary>
		/// <returns>The number of children.</returns>
		public int ChildCount(){
			if(!IsParent){
				return 0;
			}
			
			int count=0;
			CodeFragment current=FirstChild;
			
			while(current!=null){
				count++;
				current=current.NextChild;
			}
			
			return count;
		}
		
		/// <summary>Adds this fragment as a child to the given fragment.
		/// It may be overriden by some types of fragment as they may wish to handle it differently.</summary>
		/// <param name="to">The parent to add this fragment to.</param>
		/// <param name="sr">The lexer containing the original text code.</param>
		public virtual AddResult AddTo(CodeFragment to,CodeLexer sr){
			to.AddChild(this);
			return AddResult.Ok;
		}
		
		/// <summary>Converts this fragment into a code string.</summary>
		public override string ToString(){
			string result="";
			
			if(IsParent){
				
				CodeFragment current=FirstChild;
				
				while(current!=null){
					result+=current.ToString();
					current=current.NextChild;
				}
				
			}
			
			return result;
		}
		
	}
	
}