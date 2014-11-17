using System;
using UnityEngine;

/// <summary>
/// This class is used in the NitroExample.
/// It shows that Nitro can call C# functions directly and work with your games objects.
/// Note that you can block calls like this with nitro security domains.
/// </summary>

public static class NitroExampleClass{
	
	public static void Hello(){
		
		Debug.Log("Hello from C#!");
		
	}
	
}