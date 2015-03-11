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
using PowerUI.Css;

namespace PowerUI{

	/// <summary>
	/// Represents a transformation applied to the vertices of an element on the UI.
	/// It generates a matrix when applied. This matrix is then applied to any existing matrix
	/// (the current top of a stack) and the result is pushed to the same stack.
	/// The matrix at the top of the stack is the one which is actually applied to elements.
	/// </summary>

	public class Transformation{
		
		/// <summary>Has this transformation got an active skew?</summary>
		private bool HasSkew;
		/// <summary>True if any component of this transformation changed.</summary>
		private bool _Changed;
		/// <summary>A skew to apply.</summary>
		private Matrix4x4 _Skew=Matrix4x4.identity;
		/// <summary>The location of the transform origin.</summary>
		private Vector3 _Origin;
		/// <summary>The fully resolved matrix (i.e. the parent matrix and my local one).</summary>
		private Matrix4x4 _Matrix;
		/// <summary>A translation to apply in world units.</summary>
		private Vector3 _Translate;
		/// <summary>Rotation to apply.</summary>
		private Quaternion _Rotation=Quaternion.identity;
		/// <summary>The parent transform, if any.</summary>
		public Transformation Parent;
		/// <summary>The location of the origin relative to the top corner
		/// of the element this transformation is on.</summary>
		private Vector3 _OriginOffset;
		/// <summary>The matrix that represents only this transformation.</summary>
		private Matrix4x4 _LocalMatrix;
		/// <summary>Scale to apply.</summary>
		private Vector3 _Scale=Vector3.one;
		/// <summary>The location of the origin. May be relative (to this element)
		/// or fixed (fixed place on the screen).</summary>
		private PositionType _OriginPosition=PositionType.Relative;
		
		private bool _OriginOffsetPercX;
		private bool _OriginOffsetPercY;
		
		/// <summary>Read only version of the fully resolved matrix - parent one included.</summary>
		public Matrix4x4 Matrix{
			get{
				return _Matrix;
			}
		}
		
		/// <summary>Read only version of the matrix that represents this transformation.</summary>
		public Matrix4x4 LocalMatrix{
			get{
				return _LocalMatrix;
			}
		}
		
		/// <summary>Calculates where the transformation origin should go in screen space.</summary>
		/// <param name="relativeTo">The computed style of the element that the origin will be
		/// relative to if the origin position is 'Relative'</param>
		private void CalculateOrigin(ComputedStyle relativeTo){
			// We need to figure out where the origin is and then apply the parent transformation to it.
			_Origin=_OriginOffset;
			
			if(_OriginOffsetPercX){
				_Origin.x*=relativeTo.PixelWidth;
			}
			
			if(_OriginOffsetPercY){
				_Origin.y*=relativeTo.PixelHeight;
			}
			
			if(_OriginPosition==PositionType.Relative){
				_Origin.x+=relativeTo.OffsetLeft;
				_Origin.y+=relativeTo.OffsetTop;
			}
			
			// Map origin to world space:
			Renderman renderer=relativeTo.Element.Document.Renderer;
			_Origin=renderer.PixelToWorldUnit(_Origin.x,_Origin.y,relativeTo.ZIndex);
			
			if(Parent!=null){
				_Origin=Parent.Apply(_Origin);
			}
		}
		
		/// <summary>Recalculates the matrices if this transformation has changed.</summary>
		public void RecalculateMatrix(ComputedStyle style){
			
			if(Changed){
				CalculateOrigin(style);
				_Changed=false;
				
				_LocalMatrix=Matrix4x4.TRS(_Origin,Quaternion.identity,Vector3.one);
				
				// Skew:
				if(HasSkew){
					_LocalMatrix*=_Skew;
				}
				
				_LocalMatrix*=Matrix4x4.TRS(_Translate,_Rotation,_Scale);
				
				_LocalMatrix*=Matrix4x4.TRS(-_Origin,Quaternion.identity,Vector3.one);
			}
			
			if(Parent!=null){
				_Matrix=Parent.Matrix*_LocalMatrix;
			}else{
				_Matrix=_LocalMatrix;
			}
		}
		
		/// <summary>True if any property of this transformation changed.</summary>
		public bool Changed{
			get{
				return _Changed;
			}
		}
		
		/// <summary>The position of the origin. May be fixed (i.e. in exact screen pixels) or
		/// relative to the element that this transformation is on.</summary>
		public PositionType OriginPosition{
			get{
				return _OriginPosition;
			}
			set{
				_OriginPosition=value;
				_Changed=true;
			}
		}
		
		/// <summary>The position of the origin relative to the top left corner of the element.</summary>
		public Css.Value OriginOffset{
			set{
				_OriginOffset=Vector3.zero;
				if(value!=null){
					Css.Value x=value[0];
					Css.Value y=value[1];
					
					_OriginOffset=new Vector3(value.GetFloat(0),value.GetFloat(1),0f);
					
					_OriginOffsetPercX=(x!=null && x.Type==Css.ValueType.Percentage);
					_OriginOffsetPercY=(y!=null && y.Type==Css.ValueType.Percentage);
					
				}else{
					_OriginOffsetPercX=false;
					_OriginOffsetPercY=false;
				}
				_Changed=true;
			}
		}
		
		/// <summary>A scale transformation to apply (post process).</summary>
		public Vector3 Scale{
			get{
				return _Scale;
			}
			set{
				_Scale=value;
				_Changed=true;
			}
		}
		
		/// <summary>A translate transformation to apply (post process).</summary>
		public Vector3 Translate{
			get{
				return _Translate;
			}
			set{
				_Translate=value;
				_Changed=true;
			}
		}
		
		/// <summary>A rotation to apply (post process).</summary>
		public Quaternion Rotation{
			get{
				return _Rotation;
			}
			set{
				_Rotation=value;
				_Changed=true;
			}
		}
		
		/// <summary>A skew to apply (post process).</summary>
		public Matrix4x4 Skew{
			get{
				return _Skew;
			}
			set{
				_Skew=value;
				_Changed=true;
				HasSkew=(value!=Matrix4x4.identity);
			}
		}
		
		/// <summary>Applies this transformation to the given vertex.</summary>
		/// <param name="point">The vertex to transform.</param>
		/// <returns>The transformed vertex.</returns>
		public Vector3 Apply(Vector4 point){
			point.w=1f;
			return _Matrix*point;
		}
		
		/// <summary>Applies this transformation to the given vertex.</summary>
		/// <param name="point">The vertex to transform.</param>
		/// <returns>The transformed vertex.</returns>
		public Vector3 ApplyInverse(Vector4 point){
			point.w=1f;
			return _Matrix.inverse*point;
		}
		
	}
	
}