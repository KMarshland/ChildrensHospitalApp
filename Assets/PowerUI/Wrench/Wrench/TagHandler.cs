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

namespace Wrench{
	
	/// <summary>
	/// A tag handler represents methods for handling markup tags - for example a &lt;span and="attributes"&gt;.
	/// </summary>
	
	public class TagHandler{
		
		/// <summary>If you define a tag handler with tags which already exist, this priority allows your tag to override the existing one optionally.
		/// All system tags have a priority of zero.</summary>
		public int Priority;
		/// <summary>This is applied to every derivative of this tag handler. This enables tags to be grouped by file they are handling.
		/// e.g. "ui" extension and tag name "div" would become "ui-div" internally.</summary>
		public string TagExtension;
		
		
		/// <summary>Returns all tags that are handled by this handler, e.g. "div" or "span".
		/// Usually there will be just one in the set but some tags may wish to have more.</summary>
		public virtual string[] GetTags(){
			return null;
		}
		
		/// <summary>Returns true if this tag has no kids and closes itself. For example &lt;input type='text' /&gt;.
		/// Note that this method exists to make that final slash essentially optional.</summary>
		public virtual bool SelfClosing(){
			return false;
		}
		
		/// <summary>Makes a new instance of this tag handler. A global instance is made of this handler
		/// so this is used to generate a new instance in an efficient way.</summary>
		public virtual TagHandler GetInstance(){
			return null;
		}
		
		/// <summary>Called when the tag is instanced and the element plus its attributes and kids have been fully parsed.</summary>
		public virtual void OnTagLoaded(){}
		
		/// <summary>Called when the parser is reading the content of this tag for custom reading, e.g. a script/style tag.
		/// Non-self closing tags only. Anything that's not read by this method is assumed to be a child element.</summary>
		/// <param name="lexer">The lexer to read the content from.</param>
		public virtual void OnParseContent(MLLexer lexer){}
		
		/// <summary>Called on an instance of this handler when an attribute on the element it's attached to changes.
		/// It's also called when the tag is being loaded.</summary>
		/// <param name="attribute">The attribute that changed.</param>
		public virtual bool OnAttributeChange(string attribute){
			return false;
		}
		
	}
	
}