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

/// <summary>
/// There may be times when you want to display an image for which you only have the
/// Texture2D object. In this case, you can add it to the cache with a given name and then
/// access it in PowerUI using cache://theNameYouUsed. The most useful method for this is <see cref="PowerUI.ImageCache.Add"/>.
/// </summary>

public static class ImageCache{
	
	/// <summary>The set of all cached textures.</summary>
	private static Dictionary<string,Texture2D> Lookup=new Dictionary<string,Texture2D>();
	
	/// <summary>Adds an image to the cache.</summary>
	/// <param name="address">The name to use to find your image.</param>
	/// <param name="image">The image to store in the cache.</param>
	public static void Add(string address,Texture2D image){
		Lookup[address]=image;
	}
	
	/// <summary>Gets a named image from the cache.</summary>
	/// <param name="address">The name of the image to find.</param>
	/// <returns>A Texture2D if it's found; null otherwise.</returns>
	public static Texture2D Get(string address){
		Texture2D result;
		Lookup.TryGetValue(address,out result);
		return result;
	}
	
	/// <summary>Removes an image from the cache.</summary>
	/// <param name="address">The name of the image to remove.</param>
	public static void Remove(string address){
		Lookup.Remove(address);
	}
	
	/// <summary>Clears the cache of all its contents.</summary>
	public static void Clear(){
		Lookup.Clear();
	}

}