//--------------------------------------
//          Wrench Framework
//
//        For documentation or 
//    if you have any issues, visit
//         wrench.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;

namespace Wrench{

	/// <summary>
	/// This represents the language tag seen at the top of a language file.
	/// </summary>

	public class LanguageElement:MLElement{
		
		/// <summary>The set this element belongs to.</summary>
		public LanguageSet Parent;
		/// <summary>The tag handler for this element.</summary>
		public TagHandler Handler;
		
		
		public LanguageElement(LanguageSet parent,MLLexer lexer){
			Parent=parent;
			// This is the 'top' tag - start reading it's content.
			ReadContent(lexer,false,false);
		}
		
		public LanguageElement(LanguageSet parent,MLLexer lexer,bool innerElement){
			Parent=parent;
			ReadTag(lexer);
		}
		
		protected override MLElement CreateTagElement(MLLexer lexer){
			// Only ever called at the top level - push the new child straight to the language set.
			LanguageElement result=new LanguageElement(Parent,lexer,true);
			if(result.Tag=="language"){
				LanguageTag language=result.Handler as LanguageTag;
				language.Apply(Parent,result);
			}else{
				// Must be a variable - if it isn't, error.
				VariableTag varTag=result.Handler as VariableTag;
				if(varTag==null){
					throw new Exception("<"+result.Tag+"> not expected here - this file supports only <var>/<v> and <language> at it's top level (anything can go inside var/v though).");
				}
				varTag.Parent=Parent;
			}
			return result;
		}
		
		public override void SetTag(string tag){
			base.SetTag(tag);
			// Var is our default tag handler:
			Handler=TagHandlers.GetHandler("lang-"+Tag,"lang-var");
			if(!SelfClosing){
				SelfClosing=Handler.SelfClosing();
			}
			VariableTag varTag=Handler as VariableTag;
			if(varTag!=null){
				varTag.Element=this;
			}
		}
		
		public override TagHandler GetHandler(){
			return Handler;
		}
		
	}
	
}