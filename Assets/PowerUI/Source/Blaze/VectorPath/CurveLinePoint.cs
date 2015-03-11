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
	/// A node which immediately follows a bezier curve.
	/// </summary>
	
	public partial class CurveLinePoint:QuadLinePoint{
		
		/// <summary>The x coordinate of the 2nd control point.</summary>
		public float Control2X;
		/// <summary>The y coordinate of the 2nd control point.</summary>
		public float Control2Y;
		/// <summary>The x axis of the normal at the 2nd control point.</summary>
		public float NormalC2X;
		/// <summary>The y axis of the normal at the 2nd control point.</summary>
		public float NormalC2Y;
		
		
		/// <summary>Creates a new curve node for the given point.</summary>
		public CurveLinePoint(float x,float y):base(x,y){}
		
		public override void Transform(VectorTransform transform){
			
			float x=Control2X;
			Control2X=(transform.XScale * x + transform.Scale01 * Control2Y + transform.Dx);
			Control2Y=(transform.Scale10 * x + transform.YScale * Control2Y + transform.Dy);
			
			base.Transform(transform);
			
		}
		
		public override void MultiplyNormals(float by){
			
			NormalC2X*=by;
			NormalC2Y*=by;
			base.MultiplyNormals(by);
			
		}
		
		public override void RecalculateBounds(VectorPath path){
			
			// Take control point into account too:
			if(Control2X<path.MinX){
				path.MinX=Control2X;
			}
			
			if(Control2Y<path.MinY){
				path.MinY=Control2Y;
			}
			
			// Width/height are used as max to save some memory:
			if(Control2X>path.Width){
				path.Width=Control2X;
			}
			
			if(Control2Y>path.Height){
				path.Height=Control2Y;
			}
			
			base.RecalculateBounds(path);
			
		}
		
		/// <summary>Samples this line at the given t value.</summary>
		public override void SampleAt(float t,out float x,out float y){
			
			float previousX3=Previous.X*3f;
			float control1X3=Control1X*3f;
			float control2X3=Control2X*3f;
			
			float previousY3=Previous.Y*3f;
			float control1Y3=Control1Y*3f;
			float control2Y3=Control2Y*3f;
			
			float tSquare=t*t;
			float tCube=tSquare*t;
			
			x = Previous.X + (-previousX3 + t * (previousX3 - Previous.X * t)) * t
			+ (control1X3 + t * (-2f * control1X3 + control1X3 * t)) * t
			+ (control2X3 - control2X3 * t) * tSquare
			+ X * tCube;
			
			y = Previous.Y + (-previousY3 + t * (previousY3 - Previous.Y * t)) * t
			+ (control1Y3 + t * (-2f * control1Y3 + control1Y3 * t)) * t
			+ (control2Y3 - control2Y3 * t) * tSquare
			+ Y * tCube;
			
		}
		
		/// <summary>Extrudes this point along its normal by the given distance.</summary>
		public override void Extrude(float by){
			
			Control2X+=NormalC2X*by;
			Control2Y+=NormalC2Y*by;
			
			base.Extrude(by);
			
		}
		
		public override void RecalculateCurveNormals(){
			
			float x0;
			float y0;
			float x1;
			float y1;
			float x2;
			float y2;
			
			StraightLineNormal(Control1X-Previous.X,Control1Y-Previous.Y,out x0,out y0);
			StraightLineNormal(Control2X-Control1X,Control2Y-Control1Y,out x1,out y1);
			StraightLineNormal(X-Control2X,Y-Control2Y,out x2,out y2);
			
			x0=(x0+x1)/2f;
			y0=(y0+y1)/2f;
			
			float length=(float)Math.Sqrt((x0*x0) + (y0*y0));
			
			NormalC1X=x0/length;
			NormalC1Y=y0/length;
			
			x2=(x1+x2)/2f;
			y2=(y1+y2)/2f;
			
			length=(float)Math.Sqrt((x2*x2) + (y2*y2));
			
			NormalC2X=x2/length;
			NormalC2Y=y2/length;
			
		}
		
		public override VectorPoint Split(float t,VectorPath path){
			
			float invert=1f-t;
			
			float p0x=Previous.X;
			float p0y=Previous.Y;
			
			float p1x=Control1X;
			float p1y=Control1Y;
			
			float p2x=Control2X;
			float p2y=Control2Y;
			
			float p3x=X;
			float p3y=Y;
			
			// The new points:
			float p4x=p0x * invert + p1x * t;
			float p4y=p0y * invert + p1y * t;
			
			float p5x=p1x * invert + p2x * t;
			float p5y=p1y * invert + p2y * t;
			
			float p6x=p2x * invert + p3x * t;
			float p6y=p2y * invert + p3y * t;
			
			float p7x=p4x * invert + p5x * t;
			float p7y=p4y * invert + p5y * t;
			
			float p8x=p5x * invert + p6x * t;
			float p8y=p5y * invert + p6y * t;
			
			float p9x=p7x * invert + p8x * t;
			float p9y=p7y * invert + p8y * t;
			
			
			// This curve will become the new 1st half:
			Control1X=p4x;
			Control1Y=p4y;
			
			Control2X=p7x;
			Control2Y=p7y;
			
			X=p9x;
			Y=p9y;
			
			// Create the next one:
			CurveLinePoint point=new CurveLinePoint(p3x,p3y);
			
			point.Control1X=p8x;
			point.Control1Y=p8y;
			point.Control2X=p6x;
			point.Control2Y=p6y;
			
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
		
		public override void NormalAt(float t,out float x,out float y){
			
			/*
			float tSquare=t*t;
			float inverse=1f-t;
			float inverseSquare=inverse*inverse;
			
			inverseSquare*=3f;
			inverse*=6f;
			tSquare*=3f;
			
			float p1=(inverseSquare - inverse * t);
			float p2=(inverse*t - tSquare);
			
			y=-(- Previous.X * inverseSquare
			+ Control1X * p1
			+ Control2X * p2
			+ X * tSquare);
			
			x=- Previous.Y * inverseSquare
			+ Control1Y * p1
			+ Control2Y * p2
			+ Y * tSquare;
			*/
			
			float x0;
			float y0;
			float x1;
			float y1;
			float x2;
			float y2;
			
			SampleAt(t-0.01f,out x0,out y0);
			SampleAt(t,out x1,out y1);
			SampleAt(t+0.01f,out x2,out y2);
			
			StraightLineNormal(x1-x0,y1-y0,out x0,out y0);
			StraightLineNormal(x2-x1,y2-y1,out x2,out y2);
			
			x=(x0+x2)/2f;
			y=(y0+y2)/2f;
			
			/*
			// Normalise:
			
			float length=(float)Math.Sqrt( (x*x)+(y*y) );
			
			x/=length;
			y/=length;
			*/
		}
		
		public override void StartNormal(out float x,out float y){
			StraightLineNormal(Control1X-Previous.X,Control1Y-Previous.Y,out x,out y);
		}
		
		public override void EndNormal(out float x,out float y){
			StraightLineNormal(X-Control2X,Y-Control2Y,out x,out y);
		}
		
		public override void ComputeLinePoints(PointReceiver output){
			
			// Divide length by the amount we advance per pixel to get the number of pixels on this line:
			int pixels=(int)(Length/output.SampleDistance);
			
			if(pixels<=0){
				pixels=1;
			}
			
			// Run along the line as a 0-1 progression value.
			float deltaProgress=1f/(float)pixels;
			
			// From but not including previous:
			float t=deltaProgress;
			
			float x;
			float y;
			float previousX;
			float previousY;
			
			float control1X3=Control1X*3f;
			float control2X3=Control2X*3f;
			float control1Y3=Control1Y*3f;
			float control2Y3=Control2Y*3f;
			
			float extrudeBy=output.ExtrudeBy;
			
			if(extrudeBy==0f){
				x=X;
				y=Y;
				previousX=Previous.X;
				previousY=Previous.Y;
			}else{
				x=X + (NormalX * extrudeBy);
				y=Y + (NormalY * extrudeBy);
				previousX=Previous.X + (Previous.NormalX * extrudeBy);
				previousY=Previous.Y + (Previous.NormalY * extrudeBy);
				extrudeBy*=3f;
				
				control1X3+=(NormalC1X * extrudeBy);
				control1Y3+=(NormalC1Y * extrudeBy);
				
				control2X3+=(NormalC2X * extrudeBy);
				control2Y3+=(NormalC2Y * extrudeBy);
			}
			
			float previousX3=previousX*3f;
			float previousY3=previousY*3f;
			
			// For each of the pixels:
			for(int i=0;i<pixels;i++){
				
				float tSquare=t*t;
				float tCube=tSquare*t;
				
				float pointX = previousX + (-previousX3 + t * (previousX3 - previousX * t)) * t
				+ (control1X3 + t * (-2f * control1X3 + control1X3 * t)) * t
				+ (control2X3 - control2X3 * t) * tSquare
				+ x * tCube;
				
				float pointY = previousY + (-previousY3 + t * (previousY3 - previousY * t)) * t
				+ (control1Y3 + t * (-2f * control1Y3 + control1Y3 * t)) * t
				+ (control2Y3 - control2Y3 * t) * tSquare
				+ y * tCube;
				
				// Add it:
				output.AddPoint(pointX,pointY);
				
				// Move progress:
				t+=deltaProgress;
				
			}
			
		}
		
		public override VectorPoint Copy(){
			
			CurveLinePoint point=new CurveLinePoint(X,Y);
			point.Length=Length;
			point.NormalX=NormalX;
			point.NormalY=NormalY;
			
			point.Control1X=Control1X;
			point.Control1Y=Control1Y;
			point.Control2X=Control2X;
			point.Control2Y=Control2Y;
			
			point.NormalC1X=NormalC1X;
			point.NormalC1Y=NormalC1Y;
			point.NormalC2X=NormalC2X;
			point.NormalC2Y=NormalC2Y;
			
			return point;
			
		}
		
		public override string ToString(){
			return "curveTo("+Control1X+","+Control1Y+","+Control2X+","+Control2Y+","+X+","+Y+")";
		}
		
		public override void Multiply(float by){
			Control2X*=by;
			Control2Y*=by;
			base.Multiply(by);
		}
		
		public override void Squash(float by){
			Control2Y*=by;
			base.Squash(by);
		}
		
		public override void Sheer(float by){
			Control2X+=Control2Y*by;
			base.Sheer(by);
		}
		
	}
	
}