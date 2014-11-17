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
	/// Handles a table.
	/// </summary>
	
	public class TableTag:HtmlTagHandler{
		
		/// <summary>The size of a column if there is no particular max element.</summary>
		public int NoWidthPixels;
		/// <summary>The set of styles from the widest elements in each column.</summary>
		public List<ComputedStyle> ColumnWidths;
		
		
		public override string[] GetTags(){
			return new string[]{"table"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new TableTag();
		}
		
		public override void OnChildrenLoaded(){
			WidthChanged();
		}
		
		public override void WidthChanged(){
			if(ColumnWidths==null){
				// No rows.
				return;
			}
			
			// First, how many columns have no set width, and how much space is left for them?
			// That's the amount of nulls in the ColumnWidths list.
			int noWidth=0;
			int spaceLeft=Element.Style.Computed.InnerWidth;
			for(int i=0;i<ColumnWidths.Count;i++){
				ComputedStyle column=ColumnWidths[i];
				if(column==null){
					noWidth++;
				}else{
					spaceLeft-=column.PixelWidth;
				}
			}
			
			if(spaceLeft>0 && noWidth>0){
				// Spread it evenly:
				NoWidthPixels=spaceLeft/noWidth;
			}else{
				NoWidthPixels=0;
			}
			
			// Make sure all td's have the width of the max.
			// This is required as not all child tds will have WidthChanged called directly.
			List<Element> allCells=Element.getElementsByTagName("td");
			
			foreach(Element element in allCells){
				TdTag tag=(TdTag)(element.Handler);
				tag.WidthChanged();
			}
			
			// Same for th too
			// This is required as not all child ths will have WidthChanged called directly.
			allCells=Element.getElementsByTagName("th");
			
			foreach(Element element in allCells){
				ThTag tag=(ThTag)(element.Handler);
				tag.WidthChanged();
			}
			
		}
		
	}
	
}