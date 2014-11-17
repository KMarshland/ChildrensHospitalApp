//--------------------------------------//          Wrench Framework////        For documentation or //    if you have any issues, visit//         wrench.kulestar.com////    Copyright � 2013 Kulestar Ltd//          www.kulestar.com//--------------------------------------using System;using System.Collections;using System.Collections.Generic;namespace Wrench{		/// <summary>	/// This object represents any Markup Language (ML) tag such as html, sml, xml etc.	/// </summary>		public class MLElement{				/// <summary>The raw tag as a string. e.g. "div","span" etc in html.</summary>		public string Tag;		/// <summary>True if this tag closes itself and doesn't need an end ("/div" for example) tag.</summary>		protected bool SelfClosing;		/// <summary>The set of attributes on this tag. An attribute is e.g. style="display:none;".</summary>		protected Dictionary<string,string> Properties=new Dictionary<string,string>();				/// <summary>Sets the tag of this element.</summary>		public virtual void SetTag(string tag){			Tag=tag;		}				/// <summary>Gets the handler which determines how this element and it's properties are used.</summary>		/// <returns>A tag handler which manages this element.</returns>		public virtual TagHandler GetHandler(){			return null;		}				/// <summary>Reads a tag from the given lexer. Note that this does not read it's children or closing tag.</summary>		/// <param name="lexer">The lexer the tag should be read from.</param>		protected void ReadTag(MLLexer lexer){			bool first=true;			char peek=lexer.Peek();						if(peek=='<'){				lexer.Read();				peek=lexer.Peek();			}						while(peek!=StringReader.NULL&&peek!='>'){				string property;				string value;				PropertyTextReader.Read(lexer,first||SelfClosing,out property,out value);				property=property.ToLower();				if(first){					first=false;					SetTag(property);				}else if(property=="/"){					SelfClosing=true;				}else{					this[property]=value;				}				peek=lexer.Peek();			}			if(peek=='>'){				lexer.Read();			}		}				/// <summary>Gets or sets the named attribute of this tag. An attribute is e.g. style="display:none;".</summary>		/// <param name="property">The name of the attribute to get/set.</param>		public string this[string property]{			get{				string result=null;				Properties.TryGetValue(property,out result);				return result;			}						set{				// Did it change?				string before=this[property];				Properties[property]=value;				if(before!=value){					// Sure did!					GetHandler().OnAttributeChange(property);				}			}		}				/// <summary>Reloads the content of variables if it's name matches the given one.</summary>		/// <param name="name">The name of the variable to reset.</param>		public virtual void ResetVariable(string name){}				/// <summary>Re-resolves all variable tags. This is used when the language is changed.</summary>		public virtual void ResetAllVariables(){}				/// <summary>Reads the children for this tag from a lexer.</summary>		/// <param name="lexer"></param>		/// <param name="innerElement">True if we're looking for the closing tag of this element to exit.		/// If its found, this method safely returns. Unbalanced tags will otherwise throw an exception.</param>		/// <param name="literal">Literal is true if the content should be read 'as is', ignoring any tags.</param>		protected void ReadContent(MLLexer lexer,bool innerElement,bool literal){			char Peek=lexer.Peek();			string variableString="";			bool readingVariable=false;			MLTextElement textElement=null;			List<string> variableArguments=null;						while(Peek!=StringReader.NULL){				if(readingVariable){					lexer.Read();					if(Peek==';'){						readingVariable=false;												// First, flush textElement if we're working on one (likely):						if(textElement!=null){							// Make sure it adds the last word:							textElement.DoneWord(true);							textElement=null;						}						// Check if this string (e.g. &WelcomeMessage;) is provided by the variable set:						// Generate a new variable element:						MLVariableElement varElement=CreateVariableElement();						varElement.SetVariableName(variableString);						if(variableArguments!=null){							varElement.SetArguments(variableArguments.ToArray());							variableArguments=null;						}						varElement.LoadNow(innerElement);						variableString="";					}else if(Peek=='('){						// Read runtime argument set. &WelcomeMessage('heya!');						Peek=lexer.Peek();						string currentArg="";						variableArguments=new List<string>();												while(Peek!=StringReader.NULL){														if(Peek==')'){								if(currentArg!=""){									// Add it:									variableArguments.Add(currentArg);								}								// Read it off:								lexer.Read();								break;							}else if(Peek==','){								// Done one parameter - onto the next.								variableArguments.Add(currentArg);								currentArg="";							}else if(Peek=='"' || Peek=='\''){								// One of our args is a "string".								// Use the string reader of the PropertyTextReader to read it.								currentArg=PropertyTextReader.ReadString(lexer);								// We don't want to read a char off, so continue here.								// Peek the next one:								Peek=lexer.Peek();								continue;							}else if(Peek!=' '){								// Generall numeric args will fall down here.								// Disallowing spaces means the set can be spaced out like so: (14, 21)								currentArg+=Peek;							}							// Read off the char:							lexer.Read();							// Peek the next one:							Peek=lexer.Peek();						}					}else{						variableString+=Peek;					}				}else if(!literal&&Peek=='<'){					if(textElement!=null){						// Make sure it adds the last word:						textElement.DoneWord(true);						textElement=null;					}					// Read off the <.					lexer.Read();					if(lexer.Peek()=='/'){						// Should be the closure (</tag>) for 'this' element.												// Read off the /:						lexer.Read();												char closePeek=lexer.Peek();						string tag="";						while(closePeek!='>'&&closePeek!=StringReader.NULL){							tag+=closePeek;							lexer.Read();							closePeek=lexer.Peek();						}												if(closePeek=='>'){							// Read it off:							lexer.Read();						}												if(innerElement && (tag==Tag || tag.ToLower()==Tag)){							// Closure for this element read off.							// Time to get outta here!							return;						}else{							int charNumber;							int line=lexer.GetLineNumber(out charNumber);							throw new Exception("Unbalanced tags detected: "+tag+" is closing "+Tag+" at line "+line+", character "+charNumber+". "+lexer.ReadLine(line)+".");						}					}else{						MLElement tag=CreateTagElement(lexer);						TagHandler handler=tag.GetHandler();						if(tag.SelfClosing){							tag.OnChildrenLoaded();							handler.OnTagLoaded();						}else{							handler.OnParseContent(lexer);							// Read its kids and possibly its ending tag (the true means we're expecting an ending tag):							tag.ReadContent(lexer,true,false);							tag.OnChildrenLoaded();							handler.OnTagLoaded();						}					}				}else if(!literal&&Peek=='&'){					// E.g. &gt; &amp; etc.					readingVariable=true;					// Read off the &:					lexer.Read();				}else{					if(textElement==null){						textElement=CreateTextElement();						}					textElement.AddCharacter(lexer.Read());				}								Peek=lexer.Peek();			}			if(textElement!=null){				textElement.DoneWord(true);			}		}				/// <summary>Called from within this element to create a variable element. 		/// Variable elements holds the resolved value of a &variable;.</summary>		protected virtual MLVariableElement CreateVariableElement(){			return null;		}				/// <summary>Called from within this element to create a raw text element.</summary>		protected virtual MLTextElement CreateTextElement(){			return null;		}				/// <summary>Called from within this element to generate a new tag element.</summary>		protected virtual MLElement CreateTagElement(MLLexer lexer){			return null;		}				/// <summary>Called when this elements children are fully loaded.		public virtual void OnChildrenLoaded(){}				/// <summary>Called from within this element when it parses an attempts to resolve a &variable;.</summary>		protected virtual string GetVariableValue(string variable){			return null;		}				public override string ToString(){			string result="<"+Tag;			if(Properties.Count>0){				foreach(KeyValuePair<string,string> property in Properties){					result+=" "+property.Key+"=\""+property.Value+"\"";				}			}			if(SelfClosing){				result+="/";			}			return result+">";		}			}	}