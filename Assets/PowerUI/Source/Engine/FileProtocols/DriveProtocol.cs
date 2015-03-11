#if UNITY_STANDALONE || UNITY_ANDROID
using UnityEngine;
using PowerUI.Css;
 
namespace PowerUI{
   
    /// <summary>
    /// This protocol is used if you have an image file on the screen.
    /// You must use file://pathToImage.png to access it.
    /// Nota that this will not work on all platforms (namely it won't work on webplayer)
    /// </summary>
   
    public class DriveProtocol:FileProtocol{
       
        /// <summary>Returns all protocol names:// that can be used for this protocol.</summary>
        public override string[] GetNames(){
            return new string[]{"file"};
        }
       
        /// <summary>Attempts to get a graphic from the given location using this protocol.</summary>
        /// <param name="package">The image request. GotGraphic must be called on this when the protocol is done.</param>
        /// <param name="path">The location of the file to retrieve using this protocol.</param>
        public override void OnGetGraphic(ImagePackage package,FilePath path){
			
			// Create it:
            Texture2D tex = new Texture2D(0,0);
			
			// Load it:
            tex.LoadImage(System.IO.File.ReadAllBytes(path.Path));
			
			// Let the package know:
            package.GotGraphic(tex);
        }
		
		// Textual stuff; usually .html files
		public override void OnGetText(TextPackage package,FilePath path){
			
			// Get the bytes:
			string data=System.Text.Encoding.UTF8.GetString(System.IO.File.ReadAllBytes(path.Path));
			
			// Let the package know:
			package.GotText(data,null);
			
		}
		
		// Raw binary data
		public override void OnGetData(DataPackage package,FilePath path){
			
			// Get the bytes:
			byte[] data=System.IO.File.ReadAllBytes(path.Path);
			
			// Let the package know:
			package.GotData(data,null);
			
		}
       
    }
   
}
#endif