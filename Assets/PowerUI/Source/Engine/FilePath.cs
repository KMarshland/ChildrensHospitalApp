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

namespace PowerUI{
	
	/// <summary>
	/// Represents a path to a file with a protocol.
	/// e.g. http://www.site.com/aFile.png
	/// The path may also be relative to some other path (aFile.png relative to http://www.site.com).
	/// </summary>
	
	public class FilePath{
		
		/// <summary>The raw Url/Path</summary>
		public string Url;
		/// <summary>An overriding base path which relative URLs will be relative to. See the HTML 'base' tag.</summary>
		private string Base;
		/// <summary>An overriding base target which any links will use if they have no target defined. See the HTML 'base' tag.</summary>
		private string Target;
		/// <summary>The protocol to use when accessing this file.</summary>
		public string Protocol;
		/// <summary>The pieces of the path split up by /.</summary>
		public string[] Segments;
		/// <summary>The raw Url that this path is relative to.</summary>
		public string RelativeTo;
		/// <summary>True if this path was extended with a resolution specific name.</summary>
		public bool UsedResolution;
		/// <summary>The hash section of the URL, if there is one.
		/// Separated here because it should not be sent to the server.</summary>
		public string Hash;
		
		
		/// <summary>Creates a new filepath from the given url.</summary>
		/// <param name="url">The url of the file.</param>
		public FilePath(string url){
			SetPath(url,null,true);
		}
		
		/// <summary>Creates a new filepath from the given url.</summary>
		/// <param name="url">The url of the file.</param>
		/// <param name="relativeTo">The path that url is relative to.</param>
		public FilePath(string url,string relativeTo){
			SetPath(url,relativeTo,true);
		}
		
		/// <summary>Creates a new filepath from the given url.</summary>
		/// <param name="url">The url of the file.</param>
		/// <param name="relativeTo">The path that url is relative to.</param>
		/// <param name="useResolution">True if the resolution string should be appended to the name.</param>
		public FilePath(string url,string relativeTo,bool useResolution){
			SetPath(url,relativeTo,useResolution);
		}
		
		/// <summary>Applies the given url to this filepath object.</summary>
		/// <param name="url">The url of the file.</param>
		/// <param name="relativeTo">The path that url is relative to.</param>
		private void SetPath(string url,string relativeTo){
			SetPath(url,relativeTo,true);
		}
		
		/// <summary>Applies the given url to this filepath object.</summary>
		/// <param name="url">The url of the file.</param>
		/// <param name="relativeTo">The path that url is relative to.</param>
		private void SetPath(string url,string relativeTo,bool useResolution){
			Url=url;
			
			// First, chop off the # at the end of the URL if there is one:
			string[] hashPieces=url.Split(new string[]{"#"},2,StringSplitOptions.None);
			
			// Did we have one?
			if(hashPieces.Length==2){
				// Yes - grab it:
				Hash=hashPieces[1];
				
				// And set the url to parse to being the rest of it:
				Url=url=hashPieces[0];
			}
			
			// Split the URL by its protocol:
			string[] protocolPieces=url.Split(new string[]{"://"},2,StringSplitOptions.None);
			string address="";
			if(protocolPieces.Length==2){
				Protocol=protocolPieces[0].ToLower();
				address=protocolPieces[1];
			}else{
				Protocol="";
				address=protocolPieces[0];
				
				// We have a relative path.
				if(relativeTo!=null){
					if(address.StartsWith("/")){
						// Get the part before the first non-protocol / and use that.
						string[] relToProtocolPieces=relativeTo.Split(new string[]{"://"},2,StringSplitOptions.None);
						if(relToProtocolPieces.Length==2){
							string host=relToProtocolPieces[1].Split('/')[0];
							SetPath(relToProtocolPieces[0]+"://"+host+address,null);
						}else{
							// No protocol.
							string host=relToProtocolPieces[0].Split('/')[0];
							SetPath(host+address,null);
						}
					}else{
						if(address.StartsWith("../")){
							address=address.Substring(3,address.Length-3);
						}
						if(relativeTo.EndsWith("/")){
							SetPath(relativeTo+address,null);
						}else{
							SetPath(relativeTo+"/"+address,null);
						}
					}
					return;
				}
			}
			
			if(address!="" && address[0]=='/'){
				address=address.Substring(1);
			}
			if(address!="" && address[address.Length-1]=='/'){
				address=address.Substring(0,address.Length-1);
			}
			Segments=address.Split('/');
			
			if(useResolution){
				
				if(ScreenInfo.ResolutionName!=null){
					
					// Get the protocol handler:
					FileProtocol handler=Handler;
					
					// Does the file protocol require an appended name?
					if(handler==null || handler.UseResolution){
					
						// We should now append -ScreenInfo.ResolutionName into the name, before the dot. That's simply this:
						File=Filename+"-"+ScreenInfo.ResolutionName+"."+Filetype;
						
						return;
					}
					
				}
				
				UsedResolution=(ScreenInfo.ResolutionScale!=1f);
				
			}
			
		}
		
		/// <summary>The protocol which will handle this URL.</summary>
		public FileProtocol Handler{
			get{
				return FileProtocols.Get(Protocol);
			}
		}
		
		/// <summary>Gets the filename without the filetype.</summary>
		public string Filename{
			get{
				string[] pieces=File.Split('.');
				if(pieces.Length==1){
					return pieces[0];
				}
				// Return all but the last one.
				int top=pieces.Length-1;
				string filename=null;
				for(int i=0;i<top;i++){
					if(i==0){
						filename=pieces[i];
					}else{
						filename+="."+pieces[i];
					}
				}
				return filename;
			}
		}
		
		/// <summary>Gets the full filename with the type included.</summary>
		public string File{
			get{
				return Segments[Segments.Length-1];
			}
			set{
				Segments[Segments.Length-1]=value;
				
				// Update URL:
				string url="";
				
				for(int i=0;i<Segments.Length;i++){
					
					if(i!=0){
						url+="/";
					}
					
					url+=Segments[i];
				}
				
				Url=Protocol+"://"+url;
				
			}
		}
		
		/// <summary>Gets the type of file this path points to.</summary>
		public string Filetype{
			get{
				string[] pieces=File.Split('.');
				return pieces[pieces.Length-1];
			}
		}
		
		/// <summary>Gets the full directory path this file is in.</summary>
		public string Directory{
			get{
				string result="";
				int segmentCount=Segments.Length-1;
				for(int i=0;i<segmentCount;i++){
					result+=Segments[i]+"/";
				}
				return result;
			}
		}
		
		/// <summary>Gets the url of this file without the protocol.</summary>
		public string Path{
			get{
				return Directory+File;
			}
		}
		
		/// <summary>The host name of this path.</summary>
		public string host{
			get{
				if(Segments!=null && Segments.Length>0){
					return Segments[0];
				}
				
				return "";
			}
		}
		
		/// <summary>The hostname with no port number.</summary>
		public string hostname{
			get{
				// Grab the full host:
				string hostName=host;
				
				if(hostName==""){
					return "";
				}
				
				// Split by colon (if there is one):
				string[] pieces=hostName.Split(new string[]{":"},2,StringSplitOptions.None);
				
				// Return the first piece:
				return pieces[0];
			}
		}
		
		/// <summary>The port number, if there is one.</summary>
		public string port{
			get{
				// Grab the full host:
				string hostName=host;
				
				if(hostName==""){
					return "";
				}
				
				// Split by colon (if there is one):
				string[] pieces=hostName.Split(new string[]{":"},2,StringSplitOptions.None);
				
				if(pieces.Length==1){
					// Return the second piece:
					return pieces[1];
				}
				
				return "";
			}
		}
		
		/// <summary>The full path.</summary>
		public string href{
			get{
				return Url;
			}
		}
		
		/// <summary>The directory which anything relative to this path is relative to. Also see the html 'base' tag.</summary>
		public string basepath{
			get{
				if(Base!=null){
					return Base;
				}
				string fullPath="";
				
				if(Segments!=null && Segments.Length==1){
					fullPath=Segments[0];
					
					if(fullPath.ToLower().EndsWith(".html")){
						// It's actually just a file. Clear it.
						// Note that this only happens with the resources:// protocol (thus it only affects html files).
						fullPath="";
					}
					
				}else{
					fullPath=Directory;
					
					// Grab the file name (which might be the hostname!)
					string file=File;
					
					if(!file.Contains(".")){
						// It's also a directory.
						fullPath+=File+"/";
					}
				}
				
				return Protocol+"://"+fullPath;
			}
			set{
				Base=value;
			}
		}
		
		/// <summary>Changes the documents target override. Any links with no target use this; Also see the html 'base' tag. Default is null.</summary>
		public string target{
			get{
				return Target;
			}
			set{
				Target=value;
			}
		}
		
		/// <summary>This locations protocol, including the trailing colon (":").</summary>
		public string protocol{
			get{
				return Protocol+":";
			}
		}
		
		/// <summary>The hash section of the URL, if there is one.</summary>
		public string hash{
			get{
				if(Hash==null){
					return "";
				}
				
				return Hash;
			}
		}
		
		/// <summary>Gets everything between host and the search string.</summary>
		public string pathname{
			get{
				string result="/";
				int segmentCount=Segments.Length-1;
				
				for(int i=1;i<segmentCount;i++){
					result+=Segments[i]+"/";
				}
				
				if(Segments.Length>0){
					// Also got a file - stick that on the end too.
					result+=file;
				}
				
				return result;
			}
		}
		
		/// <summary>The file without a query string.</summary>
		public string file{
			get{
				string[] pieces=File.Split(new string[]{"?"},2,StringSplitOptions.None);
				
				return pieces[0];
			}
		}
		
		/// <summary>Gets the query string, if there is one.</summary>
		public string search{
			get{
				string[] pieces=Url.Split(new string[]{"?"},2,StringSplitOptions.None);
				
				if(pieces.Length==2){
					return pieces[1];
				}
				
				return "";
			}
		}
		
		/// <summary>Does this location have full access to the game? Depends on the protocol. E.g. http does not.</summary>
		public bool fullAccess{
			get{
				FileProtocol protocol=GetProtocol();
				
				if(protocol==null){
					return false;
				}
				
				return protocol.FullAccess(this);
			}
		}
		
		/// <summary>Is this a web location?</summary>
		public bool web{
			get{
				string proto=Protocol;
				
				return (proto=="http" || proto=="https");
			}
		}
		
		/// <summary>Gets the file protocol for this location. Default is resources://.</summary>
		public FileProtocol GetProtocol(){
			return FileProtocols.Get(Protocol);
		}
		
		/// <summary>Gets the full path as a string.</summary>
		public override string ToString(){
			if(Hash==null){
				return Url;
			}
			
			return Url+"#"+Hash;
		}
		
	}
	
}