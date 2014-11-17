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
using System.Collections;
using System.Collections.Generic;
using PowerUI.Css;

namespace PowerUI{
	
	/// <summary>
	/// This helps 'render' elements from the DOM into a set of 3D meshes.
	/// It also performs things such as alignment and line packing.
	/// </summary>
	
	public class Renderman{
		
		/// <summary>The current x location of the renderer in screen pixels from the left.</summary>
		public int PenX;
		/// <summary>The current y location of the renderer in screen pixels from the top.</summary>
		public int PenY;
		/// <summary>True if a layout event was requested.</summary>
		public bool DoLayout;
		/// <summary>The height of the current line being processed.</summary>
		public int LineHeight;
		/// <summary>The current depth the rendering is occuring at.</summary>
		public float Depth=0f;
		/// <summary>The deepest element rendered so far.</summary>
		public float MaxDepth=0f;
		/// <summary>Set to true when an element has been placed in the depth buffer.</summary>
		public bool DepthUsed;
		/// <summary>How far apart along z elements are placed on the UI.</summary>
		public float DepthResolution=0.05f;
		/// <summary>The x value that must not be exceeded by elements on a line. Used if the parent has fixed width.</summary>
		public int MaxX;
		/// <summary>The position of the text baseline.</summary>
		public int Baseline;
		/// <summary>The unity layer that all meshes should go into.</summary>
		public int RenderLayer;
		/// <summary>The tail of the batch linked list.</summary>
		public UIBatch LastBatch;
		/// <summary>The head of the batch linked list.</summary>
		public UIBatch FirstBatch;
		/// <summary>The length of the longest line so far.</summary>
		public int LargestLineWidth;
		/// <summary>Batches are pooled to prevent destroying and recreating meshes. This is the last in the pool linked list.</summary>
		public UIBatch LastPoolBatch;
		/// <summary>The base document being rendered.</summary>
		public Document RootDocument;
		/// <summary>Batches are pooled to prevent destroying and recreating meshes. This is the first in the pool linked list.</summary>
		public UIBatch FirstPoolBatch;
		/// <summary>The start of a linked list of styles that need to be painted.
		/// This linked list helps prevent layouts occuring as many updates only affect the appearance.</summary>
		public ElementStyle StylesToPaint;
		/// <summary>The start of a linked list of styles that need to be recomputed.</summary>
		public ElementStyle StylesToRecompute;
		/// <summary>A stack of transformations to apply to elements. Updated during a layout event.</summary>
		public TransformationStack Transformations=new TransformationStack();
		/// <summary>A linked list of elements on a line are kept. This is the last element on the current line.</summary>
		public ComputedStyle LastOnLine;
		/// <summary>A linked list of elements on a line are kept. This is the first element on the current line.</summary>
		public ComputedStyle FirstOnLine;
		
		/// <summary>The size the texture atlas should be when it gets created.</summary>
		public int AtlasSize=1024;
		/// <summary>A flag which notes if the vital parts of a layout are occuring. During this, the DOM delays updates.</summary>
		public bool LayoutOccuring;
		/// <summary>True if the rendering direction is left. This originates from the direction: css property.</summary>
		public bool GoingLeftwards;
		/// <summary>The batch that we are currently rendering to.</summary>
		public UIBatch CurrentBatch;
		/// <summary>A global atlas for non-isolated batches.</summary>
		public TextureAtlas SharedAtlas;
		/// <summary>The last child element of an element to be packed onto any line. Tracked for valign.</summary>
		public ComputedStyle LastPacked;
		/// <summary>The first child element of an element to be packed onto any line. Tracked for valign.</summary>
		public ComputedStyle FirstPacked;
		/// <summary>The active clipping boundary. Usually the bounds of the parent element.</summary>
		public BoxRegion ClippingBoundary;
		/// <summary>Essentially counts how many batches were issued. Used to define the render order.</summary>
		public int BatchDepth;
		/// <summary>An optional gameobject to parent all content to.</summary>
		public GameObject Node;
		/// <summary>Used if this renderer is generating a UI in the game world (e.g. on a billboard).</summary>
		public WorldUI InWorldUI;
		/// <summary>True if InWorldUI is not null and this renderer is generating a UI in the game world.</summary>
		public bool RenderingInWorld;
		/// <summary>The transform of a gameobject containing a cube collider.</summary>
		public Transform PhysicsModeCollider;
		/// <summary>A set of all fonts available to this renderer, indexed by font name.</summary>
		public Dictionary<string,DynamicFont> ActiveFonts;
		/// <summary>The filtering mode used by all fonts of this renderer.</summary>
		private FilterMode FontFilterMode=FilterMode.Point;
		/// <summary>How this renderman renders images; either on an atlas or with them 'as is'.</summary>
		private static RenderMode UIRenderMode=RenderMode.Atlas;
		
		
		/// <summary>Creates a new renderer for rendering in the world.</summary>
		/// <param name="atlasSize">The starting size of the texture atlas.
		/// A size of zero causes the RenderMode to be automatically set to NoAtlas.</param>
		public Renderman(WorldUI worldUI,int atlasSize):this(atlasSize){}
		
		/// <summary>Creates a new renderer for rendering in the world.</summary>
		public Renderman(WorldUI worldUI):this(){
			Node=worldUI.gameObject;
			RenderingInWorld=true;
			InWorldUI=worldUI;
		}
		
		/// <summary>Creates a new renderer and a new document.</summary>
		/// <param name="atlasSize">The starting size of the texture atlas.</param>
		public Renderman(int atlasSize):this(){
			AtlasSize=atlasSize;
			if(AtlasSize==0){
				UIRenderMode=RenderMode.NoAtlas;
			}
		}
		
		/// <summary>Creates a new renderer and a new document.</summary>
		public Renderman(){
			ClippingBoundary=new BoxRegion(0,0,Screen.width,Screen.height);
			ActiveFonts=new Dictionary<string,DynamicFont>();
			RootDocument=new Document(this);
			RootDocument.location=new FilePath("resources://","",false);
		}
		
		/// <summary>Creates a renderman for use in the Editor with AOT compiling Nitro.</summary>
		public Renderman(bool aot){}
		
		
		/// <summary>Gets the parent gameobject for this renderman, if there is one.</summary>
		public GameObject Parent{
			get{
				if(Node!=null){
					return Node;
				}
				return UI.GUINode;
			}
		}
		
		/// <summary>Creates the texture atlas when it's first needed.</summary>
		public void CreateAtlas(){
			if(SharedAtlas!=null){
				return;
			}
			// Setup our shared atlas:
			SharedAtlas=new TextureAtlas(AtlasSize);
			
			// Next, any existing batches that are not isolated will now be given the shared atlas:
			UIBatch current=FirstBatch;
			while(current!=null){
				current.AtlasCreated(SharedAtlas);
				current=current.BatchAfter;
			}
		}
		
		/// <summary>Called when the physics input mode changes. 
		/// Ensures all batches in this renderer are on the correct new input mode.</summary>
		/// <param name="mode">The new input mode to use.</param>
		public void SetInputMode(InputMode mode){
			UIBatch current=FirstBatch;
			while(current!=null){
				current.SetInputMode(mode);
				current=current.BatchAfter;
			}
			
			current=FirstPoolBatch;
			while(current!=null){
				current.SetInputMode(mode);
				current=current.BatchAfter;
			}
			
			if(RenderingInWorld && mode==InputMode.Screen){
				if(PhysicsModeCollider==null){
					GameObject colliderGameObject=new GameObject();
					colliderGameObject.name="PowerUI-BatchBox";
					PhysicsModeCollider=colliderGameObject.transform;
					PhysicsModeCollider.parent=Parent.transform;
					PhysicsModeCollider.localPosition=Vector3.zero;
					PhysicsModeCollider.localRotation=Quaternion.identity;
					// Most importantly of all, let's give it a box collider:
					colliderGameObject.AddComponent<BoxCollider>();
					RelocateCollider();
				}
			}else if(PhysicsModeCollider!=null){
				GameObject.Destroy(PhysicsModeCollider.gameObject);
			}
		}
		
		/// <summary>How this renderman renders images; either on an atlas or with them 'as is'.</summary>
		public RenderMode RenderMode{
			get{
				return UIRenderMode;
			}
			set{
				UIRenderMode=value;
				RequestLayout();
			}
		}
		
		/// <summary>The text filter mode. If you're using lots of WorldUI's its best to have this on Bilinear.</summary>
		public FilterMode TextFilterMode{
			get{
				return FontFilterMode;
			}
			set{
				if(value==FontFilterMode){
					return;
				}

				FontFilterMode=value;
				
				if(ActiveFonts!=null){
					foreach(KeyValuePair<string,DynamicFont> kvp in ActiveFonts){
						kvp.Value.FilterMode=value;
					}
				}
			}
		}
		
		/// <summary>Figures out where the box collider should be if we're in physics mode.</summary>
		public void RelocateCollider(){
			if(PhysicsModeCollider==null || !RenderingInWorld){
				return;
			}
			
			// Where should it be positioned, based on the origin?
			PhysicsModeCollider.localPosition=new Vector3(
															(0.5f-InWorldUI.OriginLocation.x)*(float)InWorldUI.pixelWidth,
															(0.5f-InWorldUI.OriginLocation.y)*(float)InWorldUI.pixelHeight,
															0f
														 );
			
			// Scale the collider:
			PhysicsModeCollider.localScale=new Vector3((float)InWorldUI.pixelWidth,InWorldUI.PixelHeightF,0.01f);
			
		}
		
		/// <summary>Increases the current depth value.</summary>
		public void IncreaseDepth(){
			Depth+=DepthResolution;
			if(Depth>MaxDepth){
				MaxDepth=Depth;
			}
		}
		
		/// <summary>Clears all batches from this renderer.</summary>
		public void Clear(){
			UIBatch current=FirstBatch;
			FirstBatch=LastBatch=null;
			
			while(current!=null){
				current.Destroy();
				current=current.BatchAfter;
			}
			
			current=FirstPoolBatch;
			FirstPoolBatch=LastPoolBatch=null;
			while(current!=null){
				current.Destroy();
				current=current.BatchAfter;
			}
			
			if(!RenderingInWorld && UI.MainCameraPool!=null){
				// It's the main UI and it has a camera pool. Destroy it too.
				UI.MainCameraPool.Destroy();
				UI.MainCameraPool=null;
			}
			
			RootDocument.clear();
		}
		
		/// <summary>Destroys this renderman when it's no longer needed.</summary>
		public void Destroy(){
			Clear();
			
			if(SharedAtlas!=null){
				SharedAtlas.Destroy();
				SharedAtlas=null;
			}
		}
		
		/// <summary>Adds a texture to the texture atlas.</summary>
		/// <param name="texture">The texture to add.</param>
		public void AddTexture(Texture2D texture){
			CreateAtlas();
			SharedAtlas.Add(texture);
		}
		
		/// <summary>Sets up the current batch based on the isolation settings requested by a property.</summary>
		/// <param name="property">The displayable property which wants the batch.</param>
		public void SetupBatch(DisplayableProperty property){
			SetupBatch(property,null);
		}
		
		/// <summary>Sets up the current batch based on the isolation settings requested by a property.</summary>
		/// <param name="property">The displayable property which wants the batch.</param>
		/// <param name="fontTexture">The font texture to use with this batch.</param>
		public void SetupBatch(DisplayableProperty property,Texture2D fontTexture){
			
			if(UI.MainCameraPool!=null && !RenderingInWorld){
				// This is the main UI and it also has a camera pool.
				// Are we now attempting to create a batch on top of an inline camera?
				
				// Let the camera pool check:
				if(UI.MainCameraPool.CheckCameraRequired()){
					// Clear the current batch - it can't be shared any further.
					CurrentBatch=null;
				}
				
			}
			
			if(property.Isolated){
				if(property.GotBatchAlready){
					// The property already got a batch on this layout - it doesn't need another.
					return;
				}
				
				// Isolated properties always get a new batch every time.
				CurrentBatch=GetPooledBatch();
				
				if(CurrentBatch==null){
					CurrentBatch=new UIBatch(this);
				}
				
				property.GotBatchAlready=true;
				
				// And push it to the active stack:
				AddBatch(CurrentBatch);
				
				// Make sure it knows if it is isolated or not:
				CurrentBatch.SetIsolation(property);
			}else{
				
				if(CurrentBatch!=null && !CurrentBatch.Isolated){
					// Re-use existing batch.
					
					if(fontTexture==null || fontTexture==CurrentBatch.FontTexture ){
						// Font didn't change either.
						return;
					}else if(CurrentBatch.FontTexture==null){
						// Apply the font texture right away:
						CurrentBatch.ChangeFontTexture(fontTexture);
						return;
					}
					
				}
				
				// Pull a batch from the pool and set it to currentbatch. May need to generate a new one.
				CurrentBatch=GetPooledBatch();
				
				if(CurrentBatch==null){
					CurrentBatch=new UIBatch(this);
				}
				
				// And push it to the active stack:
				AddBatch(CurrentBatch);
				
				// Make sure it knows if it is isolated or not:
				CurrentBatch.SetIsolation(null);
				
				// Apply the current font texture:
				CurrentBatch.ChangeFontTexture(fontTexture);
			}
			
			// Finally, prepare it for layout:
			CurrentBatch.PrepareForLayout();
		}
		
		/// <summary>Gets a batch from the pool. Null if the pool is empty.</summary>
		public UIBatch GetPooledBatch(){
			if(FirstPoolBatch==null){
				return null;
			}
			
			UIBatch result=FirstPoolBatch;
			FirstPoolBatch=result.BatchAfter;
			result.BatchAfter=null;
			return result;
		}
		
		/// <summary>Adds the given batch to the main linked list for processing.</summary>
		public void AddBatch(UIBatch batch){
			if(FirstBatch==null){
				FirstBatch=LastBatch=batch;
			}else{
				LastBatch=LastBatch.BatchAfter=batch;
			}
		}
		
		/// <summary>Called when the given dynamic fonts atlas gets rebuilt.</summary>
		/// <param name="font">The font whose atlas was rebuilt.</param>
		public void OnFontAtlasRebuild(DynamicFont font){
			if(ActiveFonts==null || font==null || font.Renderer==this){
				return;
			}
			
			// Does this renderer use this fonts Font?
			// This is important as a DynamicFont is Renderer specific.
			Font rawFont=font.RawFont;
			
			foreach(KeyValuePair<string,DynamicFont> kvp in ActiveFonts){
				if(kvp.Value.RawFont==rawFont){
					// Yes it does - let's redraw:
					RequestLayout();
					break;
				}
			}
		}
		
		/// <summary>Gets the font with the given name. May load it from the cache or generate a new one.</summary>
		/// <param name="fontName">The name of the font to find.</param>
		/// <returns>A dynamic font if found; null otherwise.</returns>
		public DynamicFont GetOrCreateFont(string fontName){
			if(fontName==null){
				return null;
			}
			
			DynamicFont result;
			ActiveFonts.TryGetValue(fontName,out result);
			
			if(result==null){
				result=DynamicFont.LoadFrom(fontName,this);
				
				if(result!=null){
					result.FilterMode=FontFilterMode;
					ActiveFonts[fontName]=result;
				}
				
			}
			
			return result;
		}
		
		/// <summary>Gets the font with the given name.</summary>
		/// <param name="fontName">The name of the font to find.</param>
		/// <returns>A dynamic font if found; null otherwise.</returns>
		public DynamicFont GetFont(string fontName){
			if(fontName==null){
				return null;
			}
			
			DynamicFont result;
			ActiveFonts.TryGetValue(fontName,out result);
			return result;
		}
		
		/// <summary>Sets up this renderer so that it's ready to start packing child elements of
		/// a given element into lines.</summary>
		/// <param name="element">The parent element whose children will be packed.</param>
		public void BeginLinePack(Element element){
			ComputedStyle computed=element.Style.Computed;
			
			if(computed.Display==DisplayType.Block||computed.FixedWidth){
				// Block elements are 100% wide unless stated otherwise (ie with a fixed width).
				MaxX=computed.InnerWidth;
				
				if(element.VScrollbar){
					MaxX-=14;
					if(MaxX<0){
						MaxX=0;
					}
				}
			}else if(computed.Display==DisplayType.InlineBlock || computed.Display==DisplayType.TableCell){
				// An inline block element uses it's parents size as the maximum.
				// If it exceeds the space left on a line it will jump to the next line anyway.
				if(element.parentNode!=null){
					MaxX=element.parentNode.Style.Computed.InnerWidth;
				}
			
			}
			
			// Kids of elements that don't line pack are packed into the lines of the first parent which does.
			PenX=0;
			PenY=0;
			LargestLineWidth=0;
			computed.ContentWidth=0;
			computed.ContentHeight=0;
			FirstPacked=LastPacked=null;
			GoingLeftwards=(computed.DrawDirection==DirectionType.RTL);
		}
		
		/// <summary>Aligns all elements that have been packed so far based on the alignment settings
		/// in a given parent element.</summary>
		/// <param name="element">The element that defines the alignment.</param>
		public void HorizontalAlign(Element element){
			ComputedStyle computed=element.Style.Computed;
			
			if(computed.AutoMarginX && computed.Display==DisplayType.Block){
				// Pixel width includes the margin size, so we must take that from it first.
				int pixelWidth=computed.PixelWidth-computed.MarginLeft-computed.MarginRight;
				computed.MarginLeft=computed.MarginRight=(element.parentNode.Style.Computed.InnerWidth-pixelWidth)/2;
				computed.PixelWidth=pixelWidth+computed.MarginLeft+computed.MarginRight;
			}
			
			if(!GoingLeftwards && (computed.HorizontalAlign==HorizontalAlignType.Auto || computed.HorizontalAlign==HorizontalAlignType.Left)){
				// Ok how it is.
				return;
			}
			
			int lineWidth=0;
			int elementsOnLine=0;
			
			ComputedStyle current=FirstPacked;
			ComputedStyle firstOnLine=current;
			
			while(current!=null){
				// Find if this child is on a new line.
				// If it is, align everything.
				lineWidth+=current.PixelWidth;
				elementsOnLine++;
				
				if(current.NextOnLine==null){
					// This is the last element on the line.
					AlignLine(firstOnLine,current,elementsOnLine,lineWidth,computed);
					elementsOnLine=0;
					lineWidth=0;
					firstOnLine=current.NextPacked;
				}
				
				current=current.NextPacked;
			}	
		}
		
		/// <summary>Horizontally aligns a line based on alignment settings in the given computed style.</summary>
		/// <param name="first">The style of the first element on the line.</param>
		/// <param name="last">The style of the last element on the line.</param>
		/// <param name="elementCount">The number of elements on this line.</param>
		/// <param name="lineLength">The width of the line in pixels.</param>
		/// <param name="parent">The style which defines the alignment.</param>
		private void AlignLine(ComputedStyle first,ComputedStyle last,int elementCount,int lineLength,ComputedStyle parent){
			if(elementCount==0){
				return;
			}
			
			// Is this the last line?
			bool lastLine=(last.NextPacked==null || last.NextPacked.Display==DisplayType.Block);
			
			HorizontalAlignType align=parent.HorizontalAlign;
			
			if(lastLine){
				align=parent.HorizontalAlignLast;
				
				if(align==HorizontalAlignType.Auto){
					// Pick an alignment based on parent's HorizontalAlign and GoingLeft.
					align=parent.HorizontalAlign;
					
					if(align==HorizontalAlignType.Justify){
						// Left or right:
						align=HorizontalAlignType.Auto;
					}
				}
				
			}
			
			if(align==HorizontalAlignType.Auto){
				if(GoingLeftwards){
					align=HorizontalAlignType.Right;
				}else{
					align=HorizontalAlignType.Left;
				}
			}
			
			if(align!=HorizontalAlignType.Left){
				// Does the last element on the line end with a space? If so, act like the space isn't there by reducing line length by it.
				
				lineLength-=last.EndSpaceSize;
				
			}
			
			// How many pixels each element will be moved over:
			int offsetBy=0;
			// True if the text is going to be justified.
			bool justify=false;
			// How many pixels we add to offsetBy each time we shift an element over:
			int justifyDelta=0;
			
			if(align==HorizontalAlignType.Center){
				// We're centering - shift by half the 'spare' pixels on this row.
				
				// How many pixels of space this line has left / 2:
				offsetBy=(parent.InnerWidth-lineLength)/2;
				
			}else if(align==HorizontalAlignType.Right){
				// How many pixels of space this line has left:
				offsetBy=parent.InnerWidth-lineLength;
				
			}else if(align==HorizontalAlignType.Justify){
				// Justify. This is where the total spare space on the line gets shared out evenly
				// between the elements on this line.
				// So, we take the spare space and divide it up by the elements on this line:
				justifyDelta=(parent.InnerWidth-lineLength)/elementCount;
				
				if(GoingLeftwards){
					// Make sure the first word starts in the correct spot if we're going leftwards:
					lineLength=parent.InnerWidth;
					// And also we actually want to be taking a little less each time, so invert justifyDelta:
					justifyDelta=-justifyDelta;
				}
				
				justify=true;
			}
			
			if(GoingLeftwards){
				// Everything is locally positioned off to the left.
				// Because of this, we need to shift them over the entire size of the row:
				offsetBy+=lineLength;
				// In this case it can also be left aligned.
			}
			
			ComputedStyle current=first;
			int counter=0;
			
			while(current!=null&&counter<elementCount){
				
				// Shift the element over by the offset.
				current.ParentOffsetLeft+=offsetBy;
				
				if(justify){
					offsetBy+=justifyDelta;
				}
				
				counter++;
				current=current.NextPacked;
			}
		}
		
		/// <summary>Lets the renderer know that the given parent element has finished
		/// packing all of its kids. This allows alignment to occur next.</summary>
		/// <param name="element">The element that is done packing.</param>
		public void EndLinePack(Element element){
			ComputedStyle computed=element.Style.Computed;
			CompleteLine(computed);
			
			if(!computed.FixedHeight){
				computed.InnerHeight=PenY;
				computed.SetPixelHeight(true);
			}
			
			computed.ContentHeight=PenY;
			computed.ContentWidth=LargestLineWidth;
			
			if(element.HScrollbar){
				computed.ContentHeight+=14;
			}
			
			if(element.VScrollbar && computed.WhiteSpace==WhiteSpaceType.Normal){
				computed.ContentWidth+=14;
			}
			
			if(!computed.FixedWidth&&computed.Display!=DisplayType.Block){
				computed.InnerWidth=LargestLineWidth;
				computed.SetPixelWidth(true);
			}
			
			if(element.VerticalScrollbar!=null){
				element.VerticalScrollbar.RecalculateTab();
				DoLayout=false;
			}
			
			if(element.HorizontalScrollbar!=null){
				element.HorizontalScrollbar.RecalculateTab();
				DoLayout=false;
			}
			
			// Next, perform any horizontal alignment.
			// This must be done here as we don't know the full width/height of the element until after EndLinePack.
			HorizontalAlign(element);
		}
		
		/// <summary>Lets the renderer know the current line doesn't fit anymore elements
		/// and has been finished.</summary>
		/// <param name="parentStyle">The computed style of the element holding this line.</param>
		public void CompleteLine(ComputedStyle parentStyle){
		
			if(PenX>LargestLineWidth){
				LargestLineWidth=PenX;
			}
			
			// Compute some alignment next.
			// Firstly, place the Pen on the line:
			
			if(PenY!=0 && parentStyle!=null){
				// Not the first line. Apply the parent computed style's line-height.
				float lineHeight=parentStyle.LineHeight;
				
				if(lineHeight!=1f){
					LineHeight=(int)(LineHeight*lineHeight);
				}
				
			}
			
			PenY+=LineHeight;
			
			// Next, align the elements and apply their top offset.
			ComputedStyle current=FirstOnLine;
			
			while(current!=null){
				// Calculate the offset to where the top left corner is:
				current.ParentOffsetTop=PenY-Baseline+current.Baseline-current.PixelHeight;
				current=current.NextOnLine;
			}
			
			FirstOnLine=null;
			LastOnLine=null;
			LineHeight=0;
			Baseline=0;
			PenX=0;
		}
		
		/// <summary>Puts the given element onto the current line.</summary>
		/// <param name="element">The element to pack.</param>
		public void PackOnLine(Element element){
			ComputedStyle computed=element.Style.Computed;
			
			if(computed.Display==DisplayType.None){
				return;
			}
			
			if(computed.Display==DisplayType.Inline){
				// Pack it's kids to this line.
				// This makes sure things like <b><i>some text</i></b> is correctly line wrapped in the top parent element.
				List<Element> kids=element.KidsToRender;
				
				if(kids==null){
					// Inline with no kids - attempt to pack it to this line; might not be successful.
					AddToLine(computed,element.parentNode);
					return;
				}
				
				for(int i=0;i<kids.Count;i++){
					PackOnLine(kids[i]);
				}
				
				return;
			}else if(computed.Position==PositionType.Relative){
				// Add it to the line (e.g. adding a div):
				// We don't want to add fixed or absolute objects as they should be placed seperately.
				AddToLine(computed,element.parentNode);
			}
		}
		
		/// <summary>Adds the given style to the current line.</summary>
		/// <param name="style">The style to add.</param>
		private void AddToLine(ComputedStyle style,Element parentNode){
			// Don't call with inline elements - block or inline-block only.
			ComputedStyle parentStyle=(parentNode==null)?null:parentNode.Style.Computed;
			
			if( (style.Display==DisplayType.Block) || ((parentStyle==null || parentStyle.WhiteSpace==WhiteSpaceType.Normal) && ((PenX+style.PixelWidth)>MaxX) )){
				// Doesn't fit here.
				CompleteLine(parentStyle);
			}
			
			style.NextPacked=null;
			style.NextOnLine=null;
			
			if(GoingLeftwards){
				PenX+=style.PixelWidth;
				style.ParentOffsetLeft=-PenX;
			}else{
				style.ParentOffsetLeft=PenX;
				PenX+=style.PixelWidth;
			}
			
			if(style.PixelHeight>LineHeight){
				LineHeight=style.PixelHeight;
			}
			
			if(style.Baseline>Baseline){
				Baseline=style.Baseline;
			}
			
			if(FirstPacked==null){
				FirstPacked=LastPacked=style;
			}else{
				LastPacked=LastPacked.NextPacked=style;
			}
			
			if(FirstOnLine==null){
				FirstOnLine=LastOnLine=style;
			}else{
				LastOnLine=LastOnLine.NextOnLine=style;
			}
			
			if(style.Display==DisplayType.Block){
				// A second newline after the block too.
				CompleteLine(parentStyle);
			}
		}
		
		/// <summary>Checks if the given box coordinates are outside the current clipping boundary.
		/// If they are, the box is considered invisible.</summary>
		/// <param name="left">The x coordinate of the left edge of the box
		/// in pixels from the left of the screen.</param>
		/// <param name="top">The y coordinate of the top edge of the box
		/// in pixels from the top edge of the screen.</param>
		/// <param name="width">The width of the box.</param>
		/// <param name="height">The height of the box (extends down the screen).</param>
		/// <returns>True if the box was outside the current clipping boundary.</returns>
		public bool IsInvisible(int left,int top,int width,int height){
			return (left>ClippingBoundary.MaxX || top>ClippingBoundary.MaxY || (left+width)<ClippingBoundary.X || (top+height)<ClippingBoundary.Y);
		}
		
		/// <summary>Sets the clipping boundary from the given computed style.</summary>
		/// <param name="style">The computed style to find the clipping boundary from.</param>
		public void SetBoundary(ComputedStyle style){
			bool visibleX=(style.OverflowX==OverflowType.Visible);
			bool visibleY=(style.OverflowY==OverflowType.Visible);
			
			if(visibleX && visibleY){
				return;
			}
			
			BoxRegion newBoundary=null;
			
			if(visibleX){
				newBoundary=new BoxRegion(ClippingBoundary.X,style.OffsetTop+style.BorderTop,ClippingBoundary.Width,style.PaddedHeight);
			}else if(visibleY){
				newBoundary=new BoxRegion(style.OffsetLeft+style.BorderLeft,ClippingBoundary.Y,style.PaddedWidth,ClippingBoundary.Height);
			}else{
				newBoundary=new BoxRegion(style.OffsetLeft+style.BorderLeft,style.OffsetTop+style.BorderTop,style.PaddedWidth,style.PaddedHeight);
			}
			
			if(style.Clip){
				newBoundary.ClipBy(ClippingBoundary);
			}
			
			ClippingBoundary=newBoundary;
		}
		
		/// <summary>Resets the clipping boundary back to the whole screen.</summary>
		public void ResetBoundary(){
			if(RenderingInWorld){
				ClippingBoundary.Set(0,0,InWorldUI.pixelWidth,InWorldUI.pixelHeight);
			}else{
				ClippingBoundary.Set(0,0,ScreenInfo.ScreenX,ScreenInfo.ScreenY);
			}
		}
		
		/// <summary>Resets all values in the renderer. Called before each layout.</summary>
		public void Reset(){
			PenX=0;
			PenY=0;
			Depth=0f;
			Baseline=0;
			MaxDepth=0f;
			BatchDepth=0;
			LineHeight=0;
			ResetBoundary();
			DepthUsed=false;
			CurrentBatch=null;
			
			if(!RenderingInWorld){
				// This is the main UI renderer.
				
				// Clear the root node:
				Node=null;
				
				if(UI.MainCameraPool!=null){
					UI.MainCameraPool.Reset();
				}
			}
		}
		
		/// <summary>The layer to put this Renderer in. Simply an alias for RenderWithCamera.</summary>
		public int Layer{
			set{
				RenderWithCamera(value);
			}
			get{
				return RenderLayer;
			}
		}
		
		/// <summary>Puts all batches of this renderer into the given unity layer.</summary>
		/// <param name="id">The ID of the unity layer.</param>
		public void RenderWithCamera(int id){
			RenderLayer=id;
			
			// Set the layer of each batch:
			UIBatch current=FirstBatch;
			
			while(current!=null){
				current.RenderWithCamera(id);
				current=current.BatchAfter;
			}
		}
		
		/// <summary>Asks the renderer to perform a layout next update.</summary>
		public void RequestLayout(){
			DoLayout=true;
		}
		
		/// <summary>Asks the renderer to perform a paint on the given style object next update.</summary>
		/// <param name="style">The style to paint.</param>
		public void RequestPaint(Css.ElementStyle style){
			if(DoLayout||style.IsPainting){
				return;
			}
			
			style.IsPainting=true;
			
			if(StylesToPaint==null){
				StylesToPaint=style;
				style.Next=null;
			}else{
				style.Next=StylesToPaint;
				StylesToPaint=style;
			}
		}
		
		/// <summary>Update causes all changes to be applied and layouts to occur.</summary>
		public void Update(){
			
			if(DoLayout){
				
				// Layout RootDocument.
				Layout();
				
			}else if(StylesToPaint!=null){
				// Repaint - these events fire from changes to things like colour/z-index etc;
				// Things which don't affect the position.
				// It's done down here incase a layout request is made.
				// If a layout request was made, it would overwrite any paint efforts made making them a waste of time.
				
				Css.ElementStyle style=StylesToPaint;
				StylesToPaint=null;
				
				while(style!=null){
					style.IsPainting=false;
					style.Computed.Repaint();
					style=style.Next;
				}
				
				// Flush all batches:
				UIBatch toFlush=FirstBatch;
				
				while(toFlush!=null){
					toFlush.Flush();
					toFlush=toFlush.BatchAfter;
				}
				
			}
			
			/*
			// Flush all atlas data (note: checks internally if it changed):
			UIBatch currentAtlas=FirstBatch;
			
			while(currentAtlas!=null){
				currentAtlas.FlushAtlas();
				currentAtlas=currentAtlas.BatchAfter;
			}*/
			
			if(SharedAtlas!=null){
				// Flush the main atlas:
				SharedAtlas.Flush();
			}
			
		}
		
		/// <summary>Relocates all DOM elements by calculating their onscreen position.
		/// Each element may allocate sections of the 3D mesh (blocks) which are then flushed out
		/// into the unity mesh and onto the screen.</summary>
		public void Layout(){
			DoLayout=false;
			Reset();
			
			// Optimise all atlases (note: checks internally if it changed):
			/*
			UIBatch currentAtlas=FirstBatch;
			
			while(currentAtlas!=null){
				currentAtlas.OptimizeAtlas();
				currentAtlas=currentAtlas.BatchAfter;
			}
			*/
			
			if(SharedAtlas!=null){
				// Optimize the main atlas (almost always does nothing at all):
				SharedAtlas.Optimize();
			}
			
			// First, push all batches to the pool:
			// Note that no isolated batches enter either the queue or the pool until their no longer isolated.
			FirstPoolBatch=FirstBatch;
			LastPoolBatch=LastBatch;
			FirstBatch=LastBatch=null;
			
			// Note: Batches are Prepared For Layout as they are added.
			
			// Step one - Prepare each font for allocate. This deallocates all their characters.
			foreach(KeyValuePair<string,DynamicFont> kvp in ActiveFonts){
				kvp.Value.PrepareForAllocate();
			}
			
			LayoutOccuring=true;
			
			// Step two - allocate characters. Finds which are in use.
			// This must be done first as we need to know the dimensions of characters before the PositionLocally pass.
			RootDocument.html.AllocateText();
			
			// Step three - Prepare the fonts for layout. This requests the in use characters.
			foreach(KeyValuePair<string,DynamicFont> kvp in ActiveFonts){
				kvp.Value.PrepareForLayout();
			}
			
			// Step four - position elements locally:
			// This sets their ParentOffset values and as a result finds their PixelWidth.
			RootDocument.html.PositionLocally();
			
			if(Input.Focused!=null){
				Input.Focused.Handler.OnRenderPass();
				
				// Make sure it didn't schedule anything.
				if(StylesToPaint!=null){
					Css.ElementStyle style=StylesToPaint;
					StylesToPaint=null;
					
					while(style!=null){
						style.IsPainting=false;
						style=style.Next;
					}
				}
				DoLayout=false;
			}
			
			// Next up, position them globally:
			// This calculates OffsetLeft/Top and also fires the render event on the computed style object.
			RootDocument.html.PositionGlobally(null);
			
			LayoutOccuring=false;
			
			// Tell each batch we're done laying them out:
			UIBatch currentBatch=FirstBatch;
			
			while(currentBatch!=null){
				currentBatch.CompletedLayout();
				currentBatch=currentBatch.BatchAfter;
			}
			
			// And destroy any remaining batches in the pool:
			currentBatch=FirstPoolBatch;
			FirstPoolBatch=null;
			
			while(currentBatch!=null){
				currentBatch.Destroy();
				currentBatch=currentBatch.BatchAfter;
			}
			
			if(StylesToPaint!=null){
				// Clear the isPainting flag.
				Css.ElementStyle style=StylesToPaint;
				StylesToPaint=null;
				
				while(style!=null){
					style.IsPainting=false;
					style=style.Next;
				}
			}
		}
		
		/// <summary>Converts the given screen coordinate to world coordinates.</summary>
		/// <param name="px">The screen x coordinate in pixels from the left.</param>
		/// <param name="py">The screen y coordinate in pixels from the top.</param>
		/// <param name="depth">The z depth.</param>
		public Vector3 PixelToWorldUnit(float px,float py,float depth){
			if(RenderingInWorld){
				py=InWorldUI.PixelHeightF-py;
				
				return new Vector3(
					(InWorldUI.WorldScreenOrigin.x + px),
					(InWorldUI.WorldScreenOrigin.y + py),
					-depth
								);
			}else{
			
				py=ScreenInfo.ScreenYFloat-py;
				float depthFactor=1f-(depth*ScreenInfo.DepthDepreciation);
				
				// The camera is placed on negative z so the actual depth value is inverted (as below).
				return new Vector3(
					(ScreenInfo.WorldScreenOrigin.x + px*ScreenInfo.WorldPerPixel.x) * depthFactor,
					(ScreenInfo.WorldScreenOrigin.y + py*ScreenInfo.WorldPerPixel.y) * depthFactor,
					-depth
								);
			}
		}
		
	}
	
}