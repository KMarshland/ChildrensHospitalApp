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


namespace PowerUI{
	
	/// <summary>
	/// Stores information about text being rendered in actual 3D.
	/// To use this, use the text-extrude CSS property. This allows you full formatting power and PowerUI can batch accordingly.
	/// </summary>
	
	public class Text3D{
		
		public Vector2[] UVs;
		public int[] Triangles;
		public Color[] Colours;
		public Vector3[] Normals;
		public Vector3[] Vertices;
		
		
		
		public Mesh CreateMesh(){
			
			Mesh mesh=new Mesh();
			mesh.vertices=Vertices;
			mesh.uv=UVs;
			mesh.normals=Normals;
			mesh.colors=Colours;
			mesh.triangles=Triangles;
			
			mesh.RecalculateBounds();
			
			return mesh;
			
		}
		
		public void CreateGameObject(){
			
			GameObject go=new GameObject();
			MeshFilter filter=go.AddComponent<MeshFilter>();
			MeshRenderer render=go.AddComponent<MeshRenderer>();
			
			filter.mesh=CreateMesh();
			render.material=new Material(Shader.Find("Diffuse"));
			
		}
		
	}
	
}