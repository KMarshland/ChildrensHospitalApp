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
	/// Reads properties/ attributes from a lexer.
	/// These are of the form propertyName="propertyValue", propertyName=value or singleProperty (i.e. no value).
	/// </summary>

	public static class PropertyTextReader{
		
		/// <summary>Reads a property and its value from the given lexer.</summary>
		/// <param name="lexer">The lexer to read the property from.</param>
		/// <param name="selfClosing">True if the tag being read is a self closing one.</param>
		/// <param name="property">The name of the property that was read.</param>
		/// <param name="value">The value of the property, if there was one. Null otherwise.</param>
		public static void Read(MLLexer lexer,bool selfClosing,out string property,out string value){
			
			System.Text.StringBuilder builder=new System.Text.StringBuilder();
			
			SkipSpaces(lexer);
			char peek=lexer.Peek();
			
			while(peek!=StringReader.NULL&&peek!=' '&&peek!='='&&peek!='>'&&(peek!='/'||builder.Length==0)){
				builder.Append(char.ToLower(lexer.Read()));
				peek=lexer.Peek();
			}
			
			// Get the property:
			property=builder.ToString();
			
			// Clear it:
			builder.Length=0;
			
			SkipSpaces(lexer);
			peek=lexer.Peek();
			
			if(peek=='='){
				// Here comes the value!
				lexer.Read();
				SkipSpaces(lexer);
				peek=lexer.Peek();
				
				if(peek==StringReader.NULL){
					value=builder.ToString();
					return;
				}else if(peek=='"'||peek=='\''){
					ReadString(lexer,builder);
				}else{
					char character=lexer.Peek();
					
					// Read until we hit a space.
					while(character!=' ' && character!='>'){
					
						if(character=='/' && lexer.Peek(1)=='>'){
							// End only if this is a self-closing tag.
							
							if(selfClosing){
								value=builder.ToString();
								return;
							}
						}
						
						lexer.Read();
						builder.Append(character);
						character=lexer.Peek();
						
						if(lexer.DidReadJunk){
							break;
						}
						
					}
				}
			}
			
			SkipSpaces(lexer);
			value=builder.ToString();
		}
		
		/// <summary>Reads a "string" or a 'string' from the lexer. Delimiting can be done with a backslash.</summary>
		/// <param name="lexer">The lexer the string will be read from.</param>
		/// <param name="builder">The builder to read it into.</summary>
		public static void ReadString(MLLexer lexer,System.Text.StringBuilder builder){
			lexer.Literal=true;
			char quote=lexer.Read();
			char character=lexer.Read();
			bool delimited=false;
			
			while(delimited||character!=quote&&character!=StringReader.NULL){
				if(character=='\\'&&!delimited){
					delimited=true;
				}else{
					delimited=false;
					builder.Append(character);
				}
				
				character=lexer.Read();
			}
			
			// Exit literal mode:
			lexer.ExitLiteral();
			
		}
		
		/// <summary>Skips any whitespaces that occur next in the given lexer.</summary>
		/// <param name="lexer">The lexer to skip spaces within.</param>
		public static void SkipSpaces(MLLexer lexer){
			char peek=lexer.Peek();
			
			while(peek==' '){
				lexer.Read();
				peek=lexer.Peek();
			}
		}
		
	}
	
}