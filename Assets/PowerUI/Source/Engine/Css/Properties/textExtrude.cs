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
using UnityEngine;
using PowerUI.Css.Properties;
using System.Collections;
using System.Collections.Generic;
using InfiniText;
using Blaze;


namespace PowerUI.Css.Properties{
	
	/// <summary>
	/// Represents the text-extrude: css property. This makes text 3D - it's best used in WorldUI's.
	/// Usage is text-extrude:[extrusion, float]. e.g. text-extrude:2.4
	/// </summary>
	
	public class TextExtrude:CssProperty{
		
		/// <summary>Is the triangulator in inverse mode? You get interesting "inverted" shapes if you flip this!</summary>
		public const bool Inverse=false;
		/// <summary>Used to guage how many points in 3D space are used along a curve. The maximum "distance" in unscaled units between points.</summary>
		public static float CurveAccuracy=0.1f;
		
		
		public TextExtrude(){
			IsTextual=true;
			
			Type=ValueType.Single;
			
		}
		
		public override string[] GetProperties(){
			return new string[]{"text-extrude"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			// Get the text:
			TextRenderingProperty text=GetText(style);
			
			if(text==null){
				return;
			}
			
			if(value==null){
				
				// No extrude:
				text.Extrude=0f;
				
			}else{
				
				if(value.Single<0f){
					text.Extrude=0f;
				}else{
					text.Extrude=value.Single;
				}
				
			}
			
			// Apply the changes:
			text.SetText();
			
		}
		
	}
	
}

namespace PowerUI.Css{
	
	public partial class TextRenderingProperty{
		
		/// <summary>The computed 3D extruded text, if there is any.</summary>
		public Text3D Text3D;
		/// <summary>How far this text is being extruded.</summary>
		public float Extrude;
		
		
		/// <summary>Gets a 3D representation of this text.</summary>
		public Text3D Get3D(float scale,Color colour,ref float left,ref float top){
			
			// Create the 3D text object:
			Text3D text=new Text3D();
			
			int tris;
			int verts;
			GetExtrudeCounts(out verts,out tris,scale);
			
			// Create the sets:
			text.Vertices=new Vector3[verts];
			text.Triangles=new int[tris * 3];
			text.UVs=new Vector2[verts];
			text.Colours=new Color[verts];
			text.Normals=new Vector3[verts];
			
			for(int i=0;i<verts;i++){
				text.Colours[i]=colour;
			}
			
			// Get the tris/ verts now:
			GetExtrude(text,0,0,verts,tris,scale,ref left,ref top);
			
			text.CreateGameObject();
			
			return text;
			
		}
		
		/// <summary>How many verts/ tris must be used to render this text in 3D?</summary>
		public void GetExtrudeCounts(out int vertCount,out int triCount,float scale){
			
			// What's the curve accuracy?
			float accuracy=TextExtrude.CurveAccuracy;
			
			vertCount=0;
			triCount=0;
			
			if(Characters==null){
				return;
			}
			
			for(int i=0;i<Characters.Length;i++){
				
				Glyph character=Characters[i];
				
				if(character==null || character.Space){
					continue;
				}
				
				// Ensure that this path has correctly considered any holes in it:
				character.HoleSort();
				
				if(character.Width==0f){
					character.RecalculateMeta();
				}
				
				int faceCount=character.GetVertexCount(accuracy);
				
				if(faceCount==0){
					// Ignore
					continue;
				}
				
				// Add twice for both faces:
				vertCount+=faceCount*2;
				
				// Add the triangle counts too - tri's on the face first:
				int tris=(faceCount-2);
				
				triCount+=tris*2;
				
				// Triangles on the "sides" of the extrude (along the extrusion):
				triCount+=faceCount * 2;
				
			}
			
		}
		
		/// <summary>Get the verts/ tris/ uv of this in 3D.</summary>
		public void GetExtrude(Text3D text,int vertIndex,int triIndex,int vertCount,int triCount,float scale,ref float left,ref float top){
			
			// What's the curve accuracy?
			float accuracy=TextExtrude.CurveAccuracy;
			
			if(Characters==null){
				return;
			}
			
			// Create a triangulator - it will "select" a letter one at a time:
			Triangulator triangulator=new Triangulator(text.Vertices,0,0);
			triangulator.Clockwise=TextExtrude.Inverse;
			
			for(int c=0;c<Characters.Length;c++){
				
				Glyph character=Characters[c];
				
				if(character==null || character.Space){
					continue;
				}
				
				if(character.Width==0f){
					character.RecalculateMeta();
				}
				
				if(Kerning!=null){
					left+=Kerning[c] * FontSize;
				}
				
				int frontIndex=vertIndex;
				
				// The set of first nodes - these essentially identify where contours start. 0 is always present if there is at least one contour.
				List<int> firstNodes=new List<int>();
				
				// Compute the vertices:
				character.GetVertices(text.Vertices,text.Normals,accuracy,left,-top,scale,ref vertIndex,firstNodes);
				
				// Duplicate and offset:
				int count=vertIndex-frontIndex;
				
				if(count==0){
					// Ignore
					continue;
				}
				
				// Future note:
				// - VectorPath.HoleSort seems to sort letters like i too.
				// - This results in a count of 1 (we're expecting 2)
				// - For each contour (foreach firstNodes), triangulate individually.
				// - Ensure the triangle count matches too. May need to move firstNodes set.
				
				// Select these verts in the triangulator:
				triangulator.Select(frontIndex,count);
				
				// Dupe the verts:
				for(int v=0;v<count;v++){
					
					Vector3 vert=text.Vertices[frontIndex+v];
					vert.z=Extrude;
					text.Vertices[vertIndex+v]=vert;
					
					text.Normals[vertIndex+v]=new Vector3(0f,0f,1f);
					text.Normals[frontIndex+v]=new Vector3(0f,0f,-1f);
				}
				
				// How many tri's in this char? (single face only)
				int charTris=count-2;
				
				// Perform triangulation of the front face:
				triangulator.Triangulate(text.Triangles,charTris,triIndex);
				
				// Seek:
				int triIndexCount=charTris*3;
				
				triIndex+=triIndexCount;
				
				// Duplicate the results for the back face and invert them:
				for(int t=0;t<charTris;t++){
					
					text.Triangles[triIndex]=count+text.Triangles[triIndex-triIndexCount];
					triIndex++;
					
					// Last two are inverted as the back face looks the other way:
					text.Triangles[triIndex]=count+text.Triangles[triIndex-triIndexCount+1];
					triIndex++;
					
					text.Triangles[triIndex]=count+text.Triangles[triIndex-triIndexCount-1];
					triIndex++;
					
				}
				
				// Time for the sides!
				
				// How many contours?
				int contourCount=firstNodes.Count;
				
				// Get the current first:
				int currentFirst=firstNodes[0];
				int contourIndex=1;
				
				
				// Get the "next" first:
				int nextFirst;
				
				if(contourCount>1){
					nextFirst=firstNodes[1];
				}else{
					nextFirst=-1;
				}
				
				// Get the back face vertex indices:
				int backIndex=vertIndex;
				
				// For each edge..
				for(int i=0;i<count;i++){
					
					// Get the next index:
					int next=i+1;
					
					if(next==count || next==nextFirst){
						// Loop back instead:
						next=currentFirst;
						
						// Advance to the next one:
						currentFirst=nextFirst;
						contourIndex++;
						
						// Update nextFirst too:
						if(contourIndex<contourCount){
							nextFirst=firstNodes[contourIndex];
						}else{
							nextFirst=-1;
						}
						
					}
					
					// Add two triangles from front face -> back face:
					
					text.Triangles[triIndex]=frontIndex+i;
					triIndex++;
					
					text.Triangles[triIndex]=backIndex+i;
					triIndex++;
					
					text.Triangles[triIndex]=backIndex+next;
					triIndex++;
					
					text.Triangles[triIndex]=frontIndex+next;
					triIndex++;
					
					text.Triangles[triIndex]=frontIndex+i;
					triIndex++;
					
					text.Triangles[triIndex]=backIndex+next;
					triIndex++;
					
				}
				
				// Seek vert index over the backface:
				vertIndex+=count;
				
				// Move left along:
				left+=(character.AdvanceWidth * FontSize)+LetterSpacing;
				
			}
			
		}
		
	}
	
}



