//--------------------------------------
//          Blaze Rasteriser
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
using UnityEngine;


namespace Blaze{

	public partial class VectorPath{
		
		/// <summary>True if any holes in this path have been sorted. Disabled by default at the moment.</summary>
		private bool HoleSorted=false;
		
		
		/// <summary>Sorts this path such that any holes it contains begin closest to it's containing contour.
		/// This essentially allows paths with holes (think hole in o!) to be correctly triangulated.</summary>
		public void HoleSort(){
			
			// Already sorted?
			if(HoleSorted){
				return;
			}
			
			HoleSorted=true;
			
			if(FirstPathNode==null){
				return;
			}
			
			// Have we actually got any holes?
			// Look for a close node followed by a node contained within the previously closed shape.
			
			// All the first nodes of each contour:
			List<VectorPoint> firstNodes=new List<VectorPoint>(1);
			
			// Add the first one:
			firstNodes.Add(FirstPathNode);
			
			// Find all the contours:
			VectorPoint current=FirstPathNode.Next;
			
			while(current!=null){
				
				if(current.IsClose){
					
					if(current.Next!=null){
						// The following node is a new contour:
						firstNodes.Add(current.Next);
					}
					
				}
				
				current=current.Next;
			}
			
			// How many have we got?
			int contourCount=firstNodes.Count;
			
			// Next, for each contour, check if it is contained in any of the previous contours:
			for(int i=1;i<contourCount;i++){
				
				// Grab the node:
				VectorPoint node=firstNodes[i];
				
				// Get it's coords:
				float currentX=node.X;
				float currentY=node.Y;
				
				// Do any previous contours contain the first node?
				for(int c=i-1;c>=0;c--){
					
					// Get the shapes end node - thats the previous node of the following contour:
					VectorPoint shapeEnd=firstNodes[c+1].Previous;
					
					// Get the shapes start node:
					VectorPoint shapeStart=firstNodes[c];
					
					// Does this contour contain the current point?
					if(Contains(currentX,currentY,shapeStart,shapeEnd)){
						
						// Yep it does! We have a hole (or a fill inside a hole).
						// We're now going to shuffle it.
						// We need it to connect to the nearest node of it's containing contour.
						VectorPoint nearest=Nearest(currentX,currentY,shapeStart,shapeEnd);
						
						if(nearest!=shapeEnd){
							
							// Here comes the "sort" - it's a simple process;
							// Think of it as simply moving a virtual line which goes between the inner shape and the outer shape.
							
							// Get the last vert of this hole shape:
							VectorPoint lastOfShape;
							
							if(i==contourCount-1){
								lastOfShape=LatestPathNode;
							}else{
								lastOfShape=firstNodes[i+1].Previous;
							}
							
							// Clear close status - this will cause it to act as if there's two nodes on top of each other:
							lastOfShape.IsClose=false;
							
							// Get the one that comes after nearest:
							VectorPoint nearestNext=nearest.Next;
							
							// Get the following node:
							VectorPoint originalNext=lastOfShape.Next;
							
							// We're about to:
							// - Remove the shape from the set
							// - Add a new node to the end of the shape located at nearest.
							// - Insert the result between nearest and it's follower.
							
							// Pop the shape out:
							shapeEnd.Next=originalNext;
							
							if(originalNext==null){
								LatestPathNode=shapeEnd;
							}else{
								originalNext.Previous=shapeEnd;
							}
							
							// Duplicate nearest with a moveto node:
							MoveToPoint point=new MoveToPoint(nearest.X,nearest.Y);
							point.Previous=lastOfShape;
							point.Next=nearestNext;
							PathNodeCount++;
							
							if(nearestNext==null){
								LatestPathNode=point;
							}else{
								nearestNext.Previous=point;
							}
							
							// Add shape between nearest and it's duplicate.
							nearest.Next=node;
							node.Previous=nearest;
							lastOfShape.Next=point;
							
						}
						
						// This return isn't quite correct, but it's a safety measure.
						// It's because we may have further holes, but they will test if their inside this one.
						return;
						//break;
						
					}
					
				}
				
			}
			
		}
		
		/// <summary>Gets the nearest node in this shape to the given point.</summary>
		public VectorPoint Nearest(float x,float y){
			return Nearest(x,y,FirstPathNode,LatestPathNode);
		}
		
		/// <summary>Gets the nearest node in the given section of this shape to the given point.</summary>
		public VectorPoint Nearest(float x,float y,VectorPoint from,VectorPoint to){
			
			VectorPoint nearest=null;
			float distance=float.MaxValue;
			
			// For each side, check if it is to the left of our point. if it is, flip contained.
			
			VectorPoint current=from;
			
			while(current!=null){
				
				float dx=current.X-x;
				float dy=current.Y-y;
				
				float dist=dx*dx + dy*dy;
				
				if(dist<distance){
					nearest=current;
					distance=dist;
				}
				
				if(current==to){
					// All done.
					break;
				}
				
				current=current.Next;
				
			}
			
			return nearest;
		}
		
		/// <summary>Does this path contain the given point?</summary>
		public bool Contains(float x,float y){
			return Contains(x,y,FirstPathNode,LatestPathNode);
		}
		
		/// <summary>Does the given section of this path contain the given point?</summary>
		public bool Contains(float x,float y,VectorPoint from,VectorPoint to){
			
			bool contained=false;
			
			// For each side, check if it is to the left of our point. if it is, flip contained.
			
			VectorPoint current=from;
			
			while(current!=null){
				
				VectorPoint pointB=(current==from)?to:current.Previous;
				
				// Figure out the bounding box of the line.
				// We're going to see if the point is outside it - if so, skip.
				
				float minX=(current.X<pointB.X)?current.X:pointB.X;
				
				// Point is to the left of tbe bounding box - ignore.
				if(minX>x){
					goto Next;
				}
				
				float maxX=(current.X>pointB.X)?current.X:pointB.X;
				
				// Point is to the right of this lines bounding box - ignore.
				// We do an inclusive ignore here as the line attached to this one might include it too.
				if(maxX<=x){
					goto Next;
				}
				
				float minY=(current.Y<pointB.Y)?current.Y:pointB.Y;
				
				// Point is below this lines bounding box - ignore.
				if(minY>y){
					goto Next;
				}
				
				// Special case if the point is above.
				float maxY=(current.Y>pointB.Y)?current.Y:pointB.Y;
				
				// We do an inclusive check here as the line attached to this one might include it too.
				if(maxY<=y){
					//The point is above for sure.
					contained=!contained;
					goto Next;
				}
				
				
				// It's sloping. What side of the line are we on? If we're on the right, the line is to the left.
				float gradient=(pointB.Y-current.Y)/(pointB.X-current.X);
				float c=current.Y-(gradient*current.X);
				
				// y<=mx+c means we're on the right, or on the line.
				if(((gradient*x)+c)<=y){
					contained=!contained;
				}
				
				if(current==to){
					// All done.
					break;
				}
				
				Next:
				current=current.Next;
				
			}
			
			return contained;
		}
		
		
		public void GetVertices(Vector3[] vertices,Vector3[] normals,float accuracy,float offsetX,float offsetY,float scale,ref int index,List<int> contourStarts){
			
			// We need to know where index starts at:
			int offset=index;
			
			// Next, we must consider all extra points caused by curves:
			VectorPoint current=FirstPathNode;
			
			if(current!=null){
				contourStarts.Add(0);
			}
			
			while(current!=null){
				
				if(current.IsCurve){
					
					// It's a curve - get the line length:
					VectorLine line=current as VectorLine;
					
					float length=line.Length;
					
					int count=(int)(length/accuracy);
					
					if(count>0){
						
						float deltaC=1f/(count+1);
						float c=deltaC;
						
						for(int i=0;i<count;i++){
							
							// Read curve at c:
							float x;
							float y;
							
							line.SampleAt(c,out x,out y);
							
							vertices[index]=new Vector3(offsetX+(x * scale),offsetY+(y * scale),0f);
							//normals[index]=new Vector3(current.NormalX,current.NormalY,0f);
							index++;
							
							c+=deltaC;
							
						}
						
					}
					
				}
				
				if(current.IsClose){
					
					// Immediately following current is the next first node, if it exists.
					if(current.Next!=null){
						// Great, add it:
						contourStarts.Add(index-offset);
					}
					
				}else{
				
					vertices[index]=new Vector3(offsetX+(current.X * scale),offsetY+(current.Y * scale),0f);
					
					//normals[index]=new Vector3(current.NormalX,current.NormalY,0f);
					
					index++;
					
				}
				
				// Hop to the next one:
				current=current.Next;
			}
			
		}
		
		public int GetVertexCount(float accuracy){
			
			int vertCount=0;
			
			// Next, we must consider all extra points caused by curves:
			VectorPoint current=FirstPathNode;
			
			while(current!=null){
				
				if(current.IsCurve){
					
					// It's a curve - get the line length:
					VectorLine line=current as VectorLine;
					
					float length=line.Length;
					
					// And just add on extra points:
					vertCount+=(int)(length/accuracy);
					
				}
				
				if(!current.IsClose){
					vertCount++;
				}
				
				// Hop to the next one:
				current=current.Next;
			}
			
			return vertCount;
			
		}
	
	}
	
}