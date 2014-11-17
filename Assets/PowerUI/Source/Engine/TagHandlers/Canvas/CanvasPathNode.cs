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
using PowerUI.Css;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PowerUI{
	
	/// <summary>
	/// Represents a node (point) on a path being drawn to a canvas.
	/// </summary>
	
	public class CanvasPathNode{
		
		/// <summary>The X coordinate of this path node.</summary>
		public float X;
		/// <summary>The Y coordinate of this path node.</summary>
		public float Y;
		/// <summary>True if there is a line between this node and Next.
		/// Otherwise this is essentially a moveTo node.</summary>
		public bool IsLineAfter;
		/// <summary>Path nodes are stored as a linked list. The one following this node.</summary>
		public CanvasPathNode Next;
		/// <summary>Path nodes are stored as a linked list. The one before this node.</summary>
		public CanvasPathNode Previous;
		
		
		/// <summary>Creates an empty path node.</summary>
		public CanvasPathNode(){}
		
		/// <summary>Creates a node at the given point.</summary>
		/// <param name="x">The X coordinate of this path node.</param>
		/// <param name="y">The Y coordinate of this path node.</param>
		public CanvasPathNode(float x,float y){
			X=x;
			Y=y;
		}
		
		/// <summary>Puts this node at the given point.</summary>
		/// <param name="x">The X coordinate of this path node.</param>
		/// <param name="y">The Y coordinate of this path node.</param>
		public void Place(float x,float y){
			X=x;
			Y=y;
		}
		
		/// <summary>Calculates the bounding box of the line between this node and the previous one.</summary>
		public virtual void RecalculateBounds(){}
		
		/// <summary>Precomputes any information this node needs to be able to render its line.</summary>
		public virtual void Setup(){}
		
		/// <summary>Used internally. Renders the line between this point and the next one, if there is one.</summary>
		/// <param name="data">The image to draw to.</param>
		public virtual void RenderLine(CanvasContext context){
		}
		
		/// <summary>The exact length of the straight line between this node and the one before it.</summary>
		public float LineLength{
			get{
				if(Previous==null){
					return 0f;
				}
				
				float dx=(Previous.X-X);
				float dy=(Previous.Y-Y);
				
				return Mathf.Sqrt( (dx*dx) + (dy*dy) );
			}
		}
		
	}
	
}