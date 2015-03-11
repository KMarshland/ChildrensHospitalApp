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
using UnityEngine;

namespace Blaze{

	public class Triangulator{

		/// <summary>True if the verts go around clockwise.
		/// In most cases this depends on which direction the polygon was drawn in.</summary>
		public bool Clockwise;
		public int[] Triangles;
		public Vector3[] Vertices;
		private int TriangleIndex;
		public int VertexCount;
		public int VertexOffset;
		public TriangulationVertex Current;
		
		
		public Triangulator(Vector3[] vertices):this(vertices,0,vertices.Length){}
		
		public Triangulator(Vector3[] vertices,int start,int vertexCount){
			Vertices=vertices;
			Select(start,vertexCount);
		}
		
		public void Select(int start,int vertexCount){
			
			VertexCount=vertexCount;
			VertexOffset=start;
			Current=null;
			
			if(vertexCount<=0){
				return;
			}
			
			TriangulationVertex first=null;
			TriangulationVertex last=null;
			
			int max=start+vertexCount;
			
			for(int i=start;i<max;i++){
				TriangulationVertex newVertex=new TriangulationVertex(Vertices[i],i);
				
				if(i==start){
					first=last=newVertex;
				}else{
					newVertex.Previous=last;
					last=last.Next=newVertex;
				}
			}
			
			last.Next=first;
			first.Previous=last;
			Current=first;
		}
		
		/// <summary>Call this to find the winding order of the polygon.</summary>
		public void FindWinding(){
			Clockwise=(GetSignedArea()>0f);
		}
		
		/// <summary>Gets the area of the polygon to triangulate.</summary>
		public float GetArea(){
			float area=GetSignedArea();
			if(area<0f){
				area=-area;
			}
			
			return area;
		}
		
		/// <summary>Gets the area of the polygon to triangulate.
		/// Note: may be negative.</summary>
		private float GetSignedArea(){
			float sum=0f;
			
			int max=VertexOffset+VertexCount;
			int last=max-1;
			
			for(int i=VertexOffset;i<max;i++){
				// Grab the current one:
				Vector3 node=Vertices[i];
				
				// Grab the next one:
				Vector3 nextNode=(i==last)?Vertices[VertexOffset]:Vertices[i+1];
				
				// Add them into the sum:
				sum+=node.x*nextNode.z - nextNode.x*node.z;
			}
			
			return sum/2f;
		}
		
		public void AddTriangle(int a,int b,int c){
			
			if(Clockwise){
				Triangles[TriangleIndex++]=b;
				Triangles[TriangleIndex++]=a;
				Triangles[TriangleIndex++]=c;
			}else{
				Triangles[TriangleIndex++]=a;
				Triangles[TriangleIndex++]=b;
				Triangles[TriangleIndex++]=c;
			}
		}
		
		public int[] Triangulate(){
			if(VertexCount<3){
				return new int[0];
			}else if(VertexCount==3){
				return new int[]{0,1,2};
			}
			
			int triangleCount=(VertexCount-2);
			
			Triangles=new int[triangleCount*3];
			
			Triangulate(Triangles,triangleCount,0);
			
			return Triangles;
		}
		
		public void Triangulate(int[] triangles,int triangleCount,int offset){
			
			Triangles=triangles;
			TriangleIndex=offset;
			
			int vertexMaximum=VertexOffset+VertexCount-1;
			
			for(int i=0;i<triangleCount;i++){
				// Foreach triangle..
				// Find a set of 3 vertices that make a valid 'ear'.
				
				// This keeps a linked loop (start is linked to end) of vertices going.
				TriangulationVertex current=Current;
				
				for(int vert=vertexMaximum;vert>=VertexOffset;vert--){
					TriangulationVertex A=current;
					TriangulationVertex B=current.Previous;
					TriangulationVertex C=current.Next;
					// Is this triangle convex?
					
					if(Clockwise){
						if(((B.Y - A.Y) * (C.X - A.X))<(((B.X - A.X) * (C.Y - A.Y)))){
							// Yes, not a valid option.
							current=current.Next;
							continue;
						}
						
						// Only vertices left in the loop can potentially go inside this triangle.
						// So, starting from the vertex after C, loop around until B is found.
						TriangulationVertex contained=current.Next.Next;
						
						while(contained!=current.Previous){
							if(InsideTriangle(A,B,C,contained)){
								goto NextVert;
							}
							contained=contained.Next;
						}
						
					}else{
						if(((B.Y - A.Y) * (C.X - A.X))>(((B.X - A.X) * (C.Y - A.Y)))){
							// Yes, not a valid option.
							current=current.Next;
							continue;
						}
						
						// Only vertices left in the loop can potentially go inside this triangle.
						// So, starting from the vertex after C, loop around until B is found.
						TriangulationVertex contained=current.Next.Next;
						
						while(contained!=current.Previous){
							
							if(InsideTriangleAnti(A,B,C,contained)){
								goto NextVert;
							}
							
							contained=contained.Next;
						}
						
					}
					
					AddTriangle(current.Index,current.Next.Index,current.Previous.Index);
					current.Remove();
					
					if(current==Current){
						Current=current.Next;
					}
					
					break;
					
					NextVert:
					current=current.Next;
				}
				
			}
			
		}
		
		private bool InsideTriangleAnti(TriangulationVertex A, TriangulationVertex B,TriangulationVertex C,TriangulationVertex P){
	 
			float ax = C.X - B.X;
			float ay = C.Y - B.Y;
			float bpx = P.X - B.X;
			float bpy = P.Y - B.Y;
			
			float cross=(ax * bpy - ay * bpx); //A crossed with BP.
			
			if(cross<0f){
				return false;
			}else if(cross==0f){
				// Is B or C==P?
				if( (B.X==P.X && B.Y==P.Y) || (C.X==P.X && C.Y==P.Y) ){
					return false;
				}
			}
			
			float bx = A.X - C.X;
			float by = A.Y - C.Y;
			float cpx = P.X - C.X;
			float cpy = P.Y - C.Y;
			
			cross=(bx * cpy - by * cpx); //B crossed with CP.
			
			if(cross<0f){
				return false;
			}else if(cross==0f){
				// Is A or C==P?
				if( (A.X==P.X && A.Y==P.Y) || (C.X==P.X && C.Y==P.Y) ){
					return false;
				}
			}
			
			
			float cx = B.X - A.X;
			float cy = B.Y - A.Y;
			float apx = P.X - A.X;
			float apy = P.Y - A.Y;
			
			cross=(cx * apy - cy * apx); //C crossed with AP.
			
			if(cross<0f){
				return false;
			}else if(cross==0f){
				// Is A or B==P?
				if( (A.X==P.X && A.Y==P.Y) || (B.X==P.X && B.Y==P.Y) ){
					return false;
				}
			}
			
			
			return true;
		}
		
		private bool InsideTriangle(TriangulationVertex A, TriangulationVertex B,TriangulationVertex C,TriangulationVertex P){
	 
			float ax = C.X - B.X;
			float ay = C.Y - B.Y;
			float bpx = P.X - B.X;
			float bpy = P.Y - B.Y;
			
			float cross=(ax * bpy - ay * bpx); //A crossed with BP.
			
			if(cross>0f){
				return false;
			}else if(cross==0f){
				// Is B or C==P?
				if( (B.X==P.X && B.Y==P.Y) || (C.X==P.X && C.Y==P.Y) ){
					return false;
				}
			}
			
			float bx = A.X - C.X;
			float by = A.Y - C.Y;
			float cpx = P.X - C.X;
			float cpy = P.Y - C.Y;
			
			cross=(bx * cpy - by * cpx); //B crossed with CP.
			
			if(cross>0f){
				return false;
			}else if(cross==0f){
				// Is A or C==P?
				if( (A.X==P.X && A.Y==P.Y) || (C.X==P.X && C.Y==P.Y) ){
					return false;
				}
			}
			
			float cx = B.X - A.X;
			float cy = B.Y - A.Y;
			float apx = P.X - A.X;
			float apy = P.Y - A.Y;
			
			cross=(cx * apy - cy * apx); //C crossed with AP.
			
			if(cross>0f){
				return false;
			}else if(cross==0f){
				// Is A or B==P?
				if( (A.X==P.X && A.Y==P.Y) || (B.X==P.X && B.Y==P.Y) ){
					return false;
				}
			}
			
			
			return true;
		}
		
	}
	
}