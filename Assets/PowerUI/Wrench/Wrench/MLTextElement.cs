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
	
	public interface MLTextElement{
		
		void DoneWord(bool lastOne);
		void AddCharacter(char character);
		
	}
	
}