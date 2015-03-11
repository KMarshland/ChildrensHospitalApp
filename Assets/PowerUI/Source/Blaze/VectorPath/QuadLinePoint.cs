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
	/// A node which immediately follows a quadratic curve.
	/// </summary>
	
	public partial class QuadLinePoint:VectorLine{
		
		/// <summary>The x coordinate of the 1st control point.</summary>
		public float Control1X;
		/// <summary>The y coordinate of the 1st control point.</summary>
		public float Control1Y;
		/// <summary>The x axis of the normal at the 1st control point.</summary>
		public float NormalC1X;
		/// <summary>The y axis of the normal at the 1st control point.</summary>
		public float NormalC1Y;
		
		
		/// <summary>Creates a new curve node for the given point.</summary>
		public QuadLinePoint(float x,float y):base(x,y){}
		
		public override void Transform(VectorTransform transform){
			
			float x=Control1X;
			Control1X=(transform.XScale * x + transform.Scale01 * Control1Y + transform.Dx);
			Control1Y=(transform.Scale10 * x + transform.YScale * Control1Y + transform.Dy);
			
			base.Transform(transform);
			
		}
		
		public override void MultiplyNormals(float by){
			
			NormalC1X*=by;
			NormalC1Y*=by;
			base.MultiplyNormals(by);
			
		}
		
		public override void RecalculateCurveNormals(){
			
			float x0;
			float y0;
			float x1;
			float y1;
			
			StraightLineNormal(Control1X-Previous.X,Control1Y-Previous.Y,out x0,out y0);
			StraightLineNormal(X-Control1X,Y-Control1Y,out x1,out y1);
			
			x0=(x0 + x1)/2f;
			y0=(y0 + y1)/2f;
			
			float length=(float)Math.Sqrt((x0*x0) + (y0*y0));
			
			NormalC1X=x0/length;
			NormalC1Y=y0/length;
			
		}
		
		public override void StartNormal(out float x,out float y){
			StraightLineNormal(Control1X-Previous.X,Control1Y-Previous.Y,out x,out y);
		}
		
		public override void EndNormal(out float x,out float y){
			StraightLineNormal(X-Control1X,Y-Control1Y,out x,out y);
		}
		
		public override VectorPoint Split(float t,VectorPath path){
			
			float invert=1f-t;
			
			float p0x=Previous.X;
			float p0y=Previous.Y;
			
			float p1x=Control1X;
			float p1y=Control1Y;
			
			float p2x=X;
			float p2y=Y;
			
			// The new points:
			float p3x=p0x * invert + p1x * t;
			float p3y=p0y * invert + p1y * t;
			
			float p4x=p1x * invert + p2x * t;
			float p4y=p1y * invert + p2y * t;
			
			float p5x=p3x * invert + p4x * t;
			float p5y=p3y * invert + p4y * t;
			
			// This curve will become the new 1st half:
			Control1X=p3x;
			Control1Y=p3y;
			
			X=p5x;
			Y=p5y;
			
			path.PathNodeCount++;
			
			// Create the next one:
			QuadLinePoint point=new QuadLinePoint(p2x,p2y);
			
			point.Control1X=p4x;
			point.Control1Y=p4y;
			
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
		
		public virtual void NormalAt(float t,out float x,out float y){
			
			/*
			float t2=2f*t;
			float controlFactor=2f-(4f * t);
			float invertSquare=-2f+t2;
			
			// Compute the tangent and flip them (this is why x is on y):
			y=-(invertSquare*Previous.X + controlFactor*Control1X + t2*X);
			x=invertSquare*Previous.Y + controlFactor*Control1Y + t2*Y;
			
			// Normalise:
			
			float length=(float)Math.Sqrt( (x*x)+(y*y) );
			
			x/=length;
			y/=length;
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
			
			float length=(float)Math.Sqrt( (x*x)+(y*y) );
			
			x/=length;
			y/=length;
			
		}
		
		public void StraightLineNormal(float dx,float dy,out float x,out float y){
			
			x=-dy;
			y=dx;
			
			float length=(float)Math.Sqrt( (x*x)+(y*y) );
			
			x/=length;
			y/=length;
			
		}
		
		public override void ComputeLinePoints(PointReceiver output){
			
			// Get previous:
			float x;
			float y;
			float previousX;
			float previousY;
			float control1X2=Control1X * 2f;
			float control1Y2=Control1Y * 2f;
			
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
				
				extrudeBy*=2f;
				control1X2+=(NormalC1X * extrudeBy);
				control1Y2+=(NormalC1Y * extrudeBy);
			}
			
			// Divide length by the amount we advance per pixel to get the number of pixels on this line:
			int pixels=(int)(Length/output.SampleDistance);
			
			if(pixels<=0){
				pixels=1;
			}
			
			// Run along the line as a 0-1 progression value.
			float deltaProgress=1f/(float)pixels;
			
			// From but not including previous:
			float t=deltaProgress;
			float invert=(1f-t);
			
			// For each of the pixels:
			for(int i=0;i<pixels;i++){
				
				float tSquare=t*t;
				float controlFactor=t*invert;
				float invertSquare=invert*invert;
				
				// Figure out the point:
				float pointX=invertSquare*previousX + controlFactor*control1X2 + tSquare*x;
				float pointY=invertSquare*previousY + controlFactor*control1Y2 + tSquare*y;
				
				// Add it:
				output.AddPoint(pointX,pointY);
				
				// Move progress:
				t+=deltaProgress;
				invert-=deltaProgress;
				
			}
			
		}
		
		public override void RecalculateBounds(VectorPath path){
			
			// Take control point into account too:
			if(Control1X<path.MinX){
				path.MinX=Control1X;
			}
			
			if(Control1Y<path.MinY){
				path.MinY=Control1Y;
			}
			
			// Width/height are used as max to save some memory:
			if(Control1X>path.Width){
				path.Width=Control1X;
			}
			
			if(Control1Y>path.Height){
				path.Height=Control1Y;
			}
			
			// Start figuring out the length..
			float vaX=Previous.X-(2f*Control1X)+X;
			float vaY=Previous.Y-(2f*Control1Y)+Y;
			
			float vbX=(2f*Control1X) - (2f*Previous.X);
			float vbY=(2f*Control1Y) - (2f*Previous.Y);
			
			float a=4f*((vaX*vaX) + (vaY*vaY));
			
			float b=4f*((vaX*vbX) + (vaY*vbY));
			
			float c=(vbX*vbX) + (vbY*vbY);
			
			float rootABC = 2f*(float)Math.Sqrt(a+b+c);
			float rootA = (float)Math.Sqrt(a);
			float aRootA = 2f*a*rootA;
			
			if(aRootA==0f){
				
				Length=0f;
				
			}else{
				
				float rootC = 2f*(float)Math.Sqrt(c);
				float bA = b/rootA;
				
				Length=(
					aRootA * rootABC + rootA*b*(rootABC-rootC) + (4f*c*a - b*b)*(float)Math.Log(
						(2f*rootA+bA+rootABC) / (bA+rootC)
					)
				) / (4f*aRootA);
			
			}
			
			base.RecalculateBounds(path);
			
		}
		
		/// <summary>Samples this line at the given t value.</summary>
		public override void SampleAt(float t,out float x,out float y){
			
			float invert=(1f-t);
			float tSquare=t*t;
			float controlFactor=t*invert*2f;
			float invertSquare=invert*invert;
			
			// Figure out the point:
			x=invertSquare*Previous.X + controlFactor*Control1X + tSquare*X;
			y=invertSquare*Previous.Y + controlFactor*Control1Y + tSquare*Y;
			
		}
		
		/// <summary>Is this a curve line?</summary>
		public override bool IsCurve{
			get{
				return true;
			}
		}
		
		/// <summary>Extrudes this point along its normal by the given distance.</summary>
		public override void Extrude(float by){
			
			Control1X+=NormalC1X*by;
			Control1Y+=NormalC1Y*by;
			
			base.Extrude(by);
			
		}
		
		public override VectorPoint Copy(){
			
			QuadLinePoint point=new QuadLinePoint(X,Y);
			point.Length=Length;
			point.NormalX=NormalX;
			point.NormalY=NormalY;
			
			point.Control1X=Control1X;
			point.Control1Y=Control1Y;
			
			point.NormalC1X=NormalC1X;
			point.NormalC1Y=NormalC1Y;
			
			return point;
			
		}
		
		public override string ToString(){
			return "quadraticCurveTo("+Control1X+","+Control1Y+","+X+","+Y+")";
		}
		
		public override void Multiply(float by){
			Control1X*=by;
			Control1Y*=by;
			base.Multiply(by);
		}
		
		public override void Squash(float by){
			Control1Y*=by;
			base.Squash(by);
		}
		
		public override void Sheer(float by){
			Control1X+=Control1Y*by;
			base.Sheer(by);
		}
		
	}
	
}