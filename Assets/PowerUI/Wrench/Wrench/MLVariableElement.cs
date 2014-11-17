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
	
	public interface MLVariableElement{
		
		string GetArgument(int id);
		void LoadNow(bool innerElement);
		void SetVariableName(string name);
		void SetArguments(string[] arguments);
		
	}
	
}