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
using System.Collections;
using System.Collections.Generic;
using PowerUI.Css;


namespace PowerUI{
	
	/// <summary>
	/// Handles a table row.
	/// </summary>
	
	public class TrTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"tr"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new TrTag();
		}
		
		public override void OnChildrenLoaded(){
			TableTag table=Element.parentNode.Handler as TableTag;
			if(table==null){
				return;
			}
			
			int columnCount=Element.childElementCount;
			
			if(table.ColumnWidths==null){
				table.ColumnWidths=new List<ComputedStyle>(columnCount);
			}
			
			int tableColumns=table.ColumnWidths.Count;
			
			if(columnCount>tableColumns){
				int delta=columnCount-tableColumns;
				
				for(int i=0;i<delta;i++){
					table.ColumnWidths.Add(null);
				}
			}
			
			for(int i=0;i<columnCount;i++){
				ComputedStyle activeWidest=table.ColumnWidths[i];
				ComputedStyle childColumn=Element.childNodes[i].Style.Computed;
				
				if(!childColumn.FixedWidth){
					continue;
				}
				
				if(activeWidest==null){
					// Must be widest - this is the first and only row so far.
					table.ColumnWidths[i]=childColumn;
				}else{
					// Is my child column wider?
					if(childColumn.PixelWidth>activeWidest.PixelWidth){
						// New wider column - use that.
						table.ColumnWidths[i]=childColumn;
					}
				}
			}
		}
		
	}
	
}