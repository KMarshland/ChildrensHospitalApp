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


namespace Blaze{
	
	/// <summary>
	/// A node which immediately follows a straight line.
	/// </summary>
	
	public partial class StraightLinePoint:VectorLine{
		
		/// <summary>Creates a new straight line node for the given point.</summary>
		public StraightLinePoint(float x,float y):base(x,y){}
		
		
		public override VectorPoint Split(float t,VectorPath path){
			
			// Create the next one:
			StraightLinePoint point=new StraightLinePoint(X,Y);
			
			// Get previous:
			float previousX=Previous.X;
			float previousY=Previous.Y;
			
			// Get deltas:
			float dx=X-previousX;
			float dy=Y-previousY;
			
			float nX=previousX + (t*dx);
			float nY=previousY + (t*dy);
			
			X=nX;
			Y=nY;
			
			path.PathNodeCount++;
			
			// Insert after this:
			if(Next==null){
				path.LatestPathNode=point;
			}else{
				point.Next=Next;
				Next.Previous=point;
			}
			
			point.Previous=this;
			Next=point;
			
			return point;
		}
		
		/// <summary>Samples this line at the given t value.</summary>
		public override void SampleAt(float t,out float x,out float y){
			
			// Get previous:
			float previousX=Previous.X;
			float previousY=Previous.Y;
			
			// Get deltas:
			float dx=X-previousX;
			float dy=Y-previousY;
			
			x=previousX + (t*dx);
			y=previousY + (t*dy);
			
		}
		
		public override void RecalculateBounds(VectorPath path){
			
			// Get deltas:
			float dx=X-Previous.X;
			float dy=Y-Previous.Y;
			
			// Length:
			Length=(float)Math.Sqrt((dx*dx)+(dy*dy));
			
			base.RecalculateBounds(path);
			
		}
		
		public override void ComputeLinePoints(PointReceiver output){
			
			// Get previous:
			float previousX=Previous.X;
			float previousY=Previous.Y;
			
			// Get deltas:
			float dx;
			float dy;
			
			float extrudeBy=output.ExtrudeBy;
			
			if(extrudeBy==0f){
				dx=X-previousX;
				dy=Y-previousY;
			}else{
				previousX+=Previous.NormalX * extrudeBy;
				previousY+=Previous.NormalY * extrudeBy;
				dx=X + (NormalX * extrudeBy) - previousX;
				dy=Y + (NormalY * extrudeBy) - previousY;
			}
			
			// Divide length by the amount we advance per pixel to get the number of pixels on this line:
			int pixels=(int)(Length/output.SampleDistance);
			
			if(pixels<=0){
				pixels=1;
			}
			
			// Run along the line as a 0-1 progression value.
			float deltaProgress=1f/(float)pixels;
			
			// From but not including previous:
			float progressX=deltaProgress * dx;
			float progressY=deltaProgress * dy;
			
			// Figure out the first point:
			float pointX=previousX + progressX;
			float pointY=previousY + progressY;
			
			// For each of the pixels:
			for(int i=0;i<pixels;i++){
				
				// Add it:
				output.AddPoint(pointX,pointY);
				
				// Move:
				pointX+=progressX;
				pointY+=progressY;
				
			}
			
		}
		
		public override void StartNormal(out float x,out float y){
			
			if(Length==0f){
				x=0f;
				y=0f;
				return;
			}
			
			// Get previous:
			float previousX=Previous.X;
			float previousY=Previous.Y;
			
			// Get deltas:
			float dx=X-previousX;
			float dy=Y-previousY;
			
			x=-dy;
			y=dx;
			
			float length=(float)Math.Sqrt( (x*x)+(y*y) );
			
			x/=length;
			y/=length;
			
		}
		
		public override void EndNormal(out float x,out float y){
			StartNormal(out x,out y);
		}
		
		public override VectorPoint Copy(){
			
			StraightLinePoint point=new StraightLinePoint(X,Y);
			point.Length=Length;
			point.NormalX=NormalX;
			point.NormalY=NormalY;
			
			return point;
			
		}
		
		public override string ToString(){
			if(Close){
				return "closePath()";
			}
			
			return "lineTo("+X+","+Y+")";
		}
		
	}
	
}