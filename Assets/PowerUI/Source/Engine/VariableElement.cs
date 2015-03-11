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
using Wrench;

namespace PowerUI{

	/// <summary>
	/// A html element which is created when using &variables;.
	/// They are tracked in this way so their content can be replaced
	/// if the language of the UI changes.
	/// </summary>

	public class VariableElement:Element,MLVariableElement{
		
		/// <summary>The &Name; used.</summary>
		private string Name;
		/// <summary>The set of arguments that can be passed into a variable at runtime. &Name(a0,a1,a2..);
		/// Arguments are accessed simply with &arg.X;</summary>
		public string[] Arguments;
		
		
		/// <summary>Creates a new variable element.</summary>
		/// <param name="document">The html document it will belong to.</param>
		/// <param name="parent">The parent html element.</param>
		public VariableElement(Document document,Element parent):base(document,parent){
			SetTag("span");
		}
		
		/// <summary>Changes the name of the variable. Thats the text used &Here;.</summary>
		public void SetVariableName(string name){
			Name=name;
		}
		
		/// <summary>Reloads the content of this variable if it's name matches the given one.</summary>
		/// <param name="name">The name of the variable to reset.</param>
		public override void ResetVariable(string name){
			if(Name==name){
				LoadNow(false);
			}
			
			// Allow resetting internal ones:
			base.ResetVariable(name);
		}
		
		/// <summary>Reloads the content of this variable element. This should
		/// be used when the language of the UI changes.</summary>
		public override void ResetAllVariables(){
			LoadNow(false);
		}
		
		/// <summary>Loads the content of this variable element by looking up the
		/// content for the variables name.</summary>
		/// <param name="innerElement">True if this element is being loaded from within another variable.</param>
		public void LoadNow(bool innerElement){
			string text=null;
			bool literal=false;
			// Handle exceptions - these are values which won't work if we pass them to a new lexer.
			// < and & most importantly, and > for completeness.
			
			if(Name=="amp"){
				text="&";
				literal=true;
			}else if(Name.StartsWith("0x")){
				// Direct hex character.
				int charcode=int.Parse(Name.Substring(2).Trim(),System.Globalization.NumberStyles.HexNumber);
				text=char.ConvertFromUtf32(charcode);
				literal=true;
			}else if(Name=="nbsp"){
				text=" ";
				literal=true;
			}else if(Name=="gt"){
				text=">";
				literal=true;
			}else if(Name=="lt"){
				text="<";
				literal=true;
			}else if(Name.StartsWith("arg.")){
				// Load the argument from the parent variable.
				string id=Name.Substring(4).Trim();
				text=((VariableElement)ParentNode).GetArgument(int.Parse(id));
			}else{
				text=GetVariableValue(Name);
			}
			
			// Do read content again on our new variable text. It can also be recursive because of this.
			IsRebuildingChildren=true;
			ChildNodes=null;
			ReadContent(new MLLexer(text,literal),innerElement,literal);
			IsRebuildingChildren=false;
		}
		
		/// <summary>Reads the runtime argument at the given index. Used with &arg.X; where x starts from 0.
		/// The arguments themselves originate from e.g. &variableName(arg0,arg1,arg2..);</summary>
		public string GetArgument(int id){
			if(Arguments==null || id>=Arguments.Length || id<0){
				throw new Exception("Argument ID ("+id+") out of range for this variable.");
			}
			return Arguments[id];
		}
		
		/// <summary>Sets the runtime argument set. See the Arguments variable.</summary>
		/// <param name="arguments">The new arguments.</param>
		public void SetArguments(string[] arguments){
			Arguments=arguments;
		}
		
		/// <summary>Gets this elements content as text.</summary>
		/// <returns>A text string of the elements content.</returns>
		public override string ToTextString(){
			if(ChildNodes==null){
				return "";
			}
			string result="";
			for(int i=0;i<ChildNodes.Count;i++){
				result+=ChildNodes[i].ToTextString();
			}
			return result;
		}
		
		/// <summary>Gets this elements content as text.</summary>
		/// <returns>A text string of the elements content.</returns>
		public override string ToString(){
			return "&"+Name+";";
		}
		
		public override void ToString(System.Text.StringBuilder builder){
			builder.Append("&");
			builder.Append(Name);
			builder.Append(";");
		}
		
	}
	
}