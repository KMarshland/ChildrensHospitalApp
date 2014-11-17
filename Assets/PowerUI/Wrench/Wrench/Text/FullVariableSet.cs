//--------------------------------------//          Wrench Framework////        For documentation or //    if you have any issues, visit//         wrench.kulestar.com////    Copyright � 2013 Kulestar Ltd//          www.kulestar.com//--------------------------------------namespace Wrench{		/// <summary>Use this OnVariableFind delegate to override finding certain variables	/// (e.g. &WelcomeUser; can call this delegate with "WelcomeUser" as code).	/// It's implemented by e.g. UI.Variables.OnFind/Speech.Variables.OnFind.</summary>	/// <param name="code">The name of the variable whose content is being searched for.</param>	public delegate string OnVariableFind(string code);		/// <summary>Use this onchange delegate to do something when a custom variable is changed.</summary>	/// <param name="code">The name of the variable that changed.</param>	public delegate void OnVariableChange(string code);			/// <summary>	/// This set represents a set of variables in text, denoted by &varName;	/// Implemented at e.g. UI.Variables/Speech.Variables.	/// The event OnFind can be used to resolve a variable not found in either custom variables or the language (for localization) set.	/// Nested variables (variables in a variables text) are evaluated at the point of replacement, not load.	/// </summary>		public class FullVariableSet{				/// <summary>The custom set of variables added and edited at runtime.</summary>		public VariableSet Custom;		/// <summary>The set of variables loaded from the language file.</summary>		public LanguageSet Language;		/// <summary>A callback called if a variable isn't found in either Custom/Language.</summary>		public event OnVariableFind OnFind;		/// <summary>A callback called when a custom variable in this set is changed.</summary>		public event OnVariableChange OnChange;						/// <summary>Creates a new complete variable set.</summary>		public FullVariableSet(){			Custom=new VariableSet();		}				/// <summary>Changes the language set. Called when the language is changed.</summary>		/// <param name="language">The new language set to use.</param>		public void ChangeLanguageSet(LanguageSet language){			Language=language;		}				/// <summary>Gets the content of a named variable.</summary>		/// <param name="variableString">The variable name.</param>		/// <returns>The variable value, if found; null otherwise.</returns>		public string GetValue(string variableString){						string variable=null;						if(Custom!=null){				// Custom variables take top priority - is it overriden?				variable=Custom.GetValue(variableString);				if(variable!=null){					return variable;				}			}						if(Language!=null){				// Language takes next priority - is this variable found in the language?				variable=Language.GetValue(variableString);				if(variable!=null){					return variable;				}			}						// Neither language or custom wanted it - how about the event?			if(OnFind!=null){				return OnFind(variableString);			}						return null;		}				/// <summary>Sets the group resolver to use when resolving a group name from a variable		/// that isn't found in the languages groups.</summary>		/// <param name="resolver">The resolver to use.</param>		public void SetGroupResolver(GroupResolve resolver){			Language.OnGroupResolve=resolver;		}				/// <summary>Sets a custom variable with the given name and value. This will take top priority.</summary>		/// <param name="code">The name of the variable to set.</param>		/// <param name="value">The value to set it to.</param>		public void SetValue(string code,string value){			if(Custom==null){				Custom=new VariableSet();			}			Custom[code]=value;			if(OnChange!=null){				OnChange(code);			}		}				/// <summary>Gets or sets custom variables.</summary>		/// <param name="code">The name of the variable to get/set.</param>		public string this[string code]{			get{				return GetValue(code);			}			set{				SetValue(code,value);			}		}			}	}