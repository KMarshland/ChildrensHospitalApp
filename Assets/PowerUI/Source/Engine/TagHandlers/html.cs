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

namespace PowerUI{
	
	/// <summary>
	/// Represents the html tag. Note that this is added automatically by PowerUI and isn't required.
	/// </summary>
	
	public class HtmlTag:HtmlTagHandler{
		
		public HtmlTag(){
			IgnoreSelfClick=true;
		}
		
		public override string[] GetTags(){
			return new string[]{"html"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new HtmlTag();
		}
		
		public override void OnTagLoaded(){
			Element.Document.html=Element;
		}
		
		public override void HeightChanged(){
			// Fire the onresize event:
			Resized();
		}
		
		public override void WidthChanged(){
			// Fire the onresize event:
			Resized();
		}
		
		public void Resized(){
			
			// Grab the document:
			Document document=Element.Document;
			
			// Call the C# delegate:
			if(document.OnResized!=null){
				document.OnResized();
			}
			
			// Call the Nitro method:
			if(document.onresize!=null){
				document.onresize.Run();
			}
			
		}
		
	}
	
}