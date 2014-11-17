//--------------------------------------//               PowerUI////        For documentation or //    if you have any issues, visit//        powerUI.kulestar.com////    Copyright � 2013 Kulestar Ltd//          www.kulestar.com//--------------------------------------using System;namespace PowerUI{		/// <summary>	/// This class represents resolution information. Use this and Document.SetResolution if you are targeting different resolutions.	/// </summary>		public class ResolutionInfo{				/// <summary>The name for this resolution mode, e.g. "ipad".		/// This name is tacked onto the end of image, animation and video files so you can select one at a different resolution.</summary>		public string Name;		/// <summary>All px values will be multiplied by this scale when this resolution is active.</summary>		public float Scale=1f;						/// <param name="scale">All px values will be multiplied by this scale when this resolution is active.</param>		public ResolutionInfo(float scale){			Scale=scale;		}				/// <summary>The name for this resolution mode, e.g. "ipad".		/// This name is tacked onto the end of image, animation and video files so you can select one at a different resolution.		/// if you don't want this to occur, use background-resolution:fixed-resolution.</summary>		/// <param name="scale">All px values will be multiplied by this scale when this resolution is active.</param>		public ResolutionInfo(string name,float scale){			Name=name;			Scale=scale;		}			}	}