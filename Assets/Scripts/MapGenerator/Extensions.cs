using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public static class Extensions {

	public static string Match(this string str, string pattern){
		return new Regex(pattern).Match(str).Value;
	}
}
