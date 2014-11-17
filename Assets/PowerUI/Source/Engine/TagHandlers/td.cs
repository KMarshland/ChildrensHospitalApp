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

using PowerUI.Css;


namespace PowerUI{
	
	/// <summary>
	/// Handles a table cell.
	/// </summary>
	
	public class TdTag:HtmlTagHandler{
		
		/// <summary>The column number of this cell in the table.</summary>
		private int Column;
		/// <summary>The table this cell belongs to.</summary>
		private TableTag Table;
		
		
		public override string[] GetTags(){
			return new string[]{"td"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new TdTag();
		}
		
		public override void OnChildrenLoaded(){
			// Go up the tree looking for our table.
			Element parent=Element.parentNode;
			while(parent!=null){
				if(parent.Tag=="table"){
					// Got it!
					Table=parent.Handler as TableTag;
				}
				parent=parent.parentNode;
			}
			
			if(Table==null){
				return;
			}
			
			// What child number (column) is this element?
			Column=Element.childIndex;
		}
		
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="valign"){
				Element.Style.vAlign=Element["valign"];
				return true;
			}
			
			return false;
		}
		
		/// <summary>Makes sure the width of this element matches that of the biggest one in the column.</summary>
		public override void WidthChanged(){
			if(Table==null || Table.ColumnWidths==null){
				return;
			}
			
			ComputedStyle computed=Element.Style.Computed;
			ComputedStyle column=Table.ColumnWidths[Column];
			
			// How much style does this td have?
			int styleSize=computed.PixelWidth-computed.InnerWidth;
			
			computed.FixedWidth=true;
			
			if(column!=null){
				if(column!=computed){
					computed.InnerWidth=column.InnerWidth-styleSize;
					computed.SetPixelWidth(false);
				}else{
					// What if this cell isn't the widest anymore?
				}
			}else{
				// No particular element is the max width - use the NoWidthPixels size instead.
				computed.InnerWidth=Table.NoWidthPixels-styleSize;
				computed.SetPixelWidth(false);
			}
		}
		
	}
	
}