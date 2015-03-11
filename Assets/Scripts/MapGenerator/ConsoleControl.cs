using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Linq.Expressions;

public class ConsoleControl : MonoBehaviour {

	public static ConsoleControl instance;

	static List<string> log = new List<string>();
	string command;
	public bool consoleOpen;
	public int maxRows = 10;
	
	public static Dictionary<string, Variable> availableVars;
	int relativeLine;

	bool consoleAllowed = false;
	Vector2 scrollPosition = new Vector2();
	
	// Use this for initialization
	void Start () {
		instance = this;

		consoleAllowed = Application.platform == RuntimePlatform.WindowsEditor;
		if (consoleAllowed){
			//float thebeforetime = Time.realtimeSinceStartup;

			//log = new List<string>();
			command = "";
			relativeLine = 0;
			
			availableVars = new Dictionary<string, Variable>();
			
			allowAllInAssembly();
			
			//Debug.Log("Startup time: " + (Time.realtimeSinceStartup - thebeforetime) + "s");
		}
	}

	void allowAllInAssembly(){
		//load pretty much everything
		Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
		foreach (Assembly a in assemblies){
			foreach (System.Type t in a.GetTypes()){
				//Debug.Log(t.ToString());
				allowClass(t);
			} 
		}
	}
	
	void allowClass(System.Type type){
		string s = type.ToString(); 
		availableVars[s] = new Variable(type);
		if (s.StartsWith("UnityEngine")){
			availableVars[s.Substring("UnityEngine".Length + 1)] = new Variable(type);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (consoleAllowed && Input.GetKeyDown(KeyCode.F1)){
			consoleOpen = !consoleOpen;
		}
	}
	
	void OnGUI(){
		if (consoleOpen){
			consoleUI();
		}
	}
	
	void consoleUI(){
		int commandHeight = 25;
		int padding = 5;
		
		string rLog = "";
		int logHeight = 0;
		int lineHeight = 22;
		for (int i = 0; i < log.Count; i++){
			if (i < maxRows){
				logHeight += lineHeight;
			}
			rLog += log[i] + "\n";
		}

		scrollPosition = GUI.BeginScrollView(
			new Rect(
				padding, Screen.height - 2*padding - commandHeight - logHeight, 
				Screen.width - 2*padding, logHeight
			), scrollPosition, 
			new Rect(0, 0, Screen.width - 6*padding, log.Count * lineHeight));
		GUI.TextArea(new Rect(
			0, 0, 
			Screen.width - 2*padding, log.Count * lineHeight
			), rLog);
		GUI.EndScrollView();

		int buttonWidth = 25;
		command = GUI.TextArea(new Rect(padding, Screen.height - padding - commandHeight, Screen.width - 2*padding - buttonWidth, commandHeight), command);
		if (Event.current.keyCode == KeyCode.Return || GUI.Button(new Rect(Screen.width - padding - buttonWidth, Screen.height - padding - commandHeight, buttonWidth, commandHeight), "->")) {
			evaluate();
		}
		
		if (Event.current.keyCode == KeyCode.UpArrow && Event.current.type == EventType.KeyUp) {
			relativeLine --;
			if (relativeLine < -1*log.Count){
				relativeLine = -1*log.Count;
			}
			
			int index = log.Count + 2*relativeLine;
			if (index < 0){
				command = "";
			} else if (index < log.Count){
				command = log[index].Trim(new char[]{'>'}).Trim();
			}
		}
		
		if (Event.current.keyCode == KeyCode.DownArrow && Event.current.type == EventType.KeyUp) {
			relativeLine ++;
			if (relativeLine > 0){
				relativeLine = 0;
			}
			
			int index = log.Count + 2*relativeLine;
			if (index < 0){
				command = "";
			} else if (index < log.Count){
				command = log[index].Trim(new char[]{'>'}).Trim();
			}
		}




	}
	
	void evaluate(){
		command = command.Trim();
		if (command == ""){
			return;
		}
		
		_log("> " + command);
		
		_log("\t" + inspect(eval(command)));
		
		command = "";
		relativeLine = 0;
	}

	string inspect(object o){
		return "<" + o.GetType().ToString() + ": " + o.ToString() + ">";
	}
	
	object eval(string com){
		
		/*Variable v = null;
		foreach (string s in availableVars.Keys){
			if (com.StartsWith(s)){
				com = com.Substring(s.Length);
				return eval(availableVars[s].GetObj(), com);
			}
		}*/

		return eval(null, com);
	}
	
	object eval(object o, string com){

		com = com.Trim(new char[]{' ', ';'});

		if (o == null){
			string remainingCom = "";
			object parsedCom = parseArg(com, out remainingCom);
			if (parsedCom != null){
				//Debug.Log(remainingCom);
				return eval(parsedCom, remainingCom);
			}

			return "null";
		}
		

		if (com == ""){
			return o;
		}
		
		//Debug.Log(com);
		
		if (com.StartsWith("[")){//the command is to index at an array
			string ind = new Regex(@"\[[\d]*\]").Match(com).Value;
			int index = int.Parse(ind.Trim(new char[]{'[', ']'}));
			
			System.Array arr = (System.Array)o;
			object nobj = arr.GetValue(index);
			string ncom = com.Substring(ind.Length);
			
			return eval(nobj, ncom);
		}
		
		bool isType = (o is System.Type);
		System.Type oType = isType ? ((System.Type)o) : o.GetType();
		
		if (com.StartsWith(".")){//it's either a function or a property
			com = com.Substring(1);//delete the period at the beginning
			
			int periodIndex = com.IndexOf(".");
			int bracketIndex = com.IndexOf("[");
			int parenIndex = com.IndexOf("(");
			int closeParenIndex = com.IndexOf(")");

			if (parenIndex > -1){

				int opened = 1;
				for (int i = parenIndex + 1; i < com.Length && opened != 0; i++){
					if (com[i] == '('){
						opened ++;

					} else if (com[i] == ')'){
						opened --;

						if (opened == 0){
							closeParenIndex = i;
							break;
						}
					}
				}

			}

			
			//Debug.Log(com + "   period: " + periodIndex + "; paren: " + parenIndex + 
			//          "; cparen: " + closeParenIndex + "; len: " + com.Length);
			
			if ((periodIndex <= parenIndex && periodIndex > -1) || parenIndex < 0){//it's a property
				var props = new Dictionary<string, object>();
				foreach(var prop in oType.GetProperties(BindingFlags.Instance|BindingFlags.Public|BindingFlags.Static)){
					try {
						props.Add(prop.Name, prop.GetValue(isType ? oType : o, null));
					} catch {}
				}
				foreach(var prop in oType.GetFields()){
					try {
						props.Add(prop.Name, prop.GetValue(isType ? oType : o));
					} catch {}
				}
				
				int fLength = 0;
				if (periodIndex == -1 && bracketIndex == -1){
					fLength = com.Length;
				} else if (periodIndex == -1){
					fLength = bracketIndex;
				} else if (bracketIndex == -1){
					fLength = periodIndex;
				} else {
					fLength = Mathf.Min(new int[]{bracketIndex, periodIndex});
				}
				
				string fCom = com.Substring(0, fLength);
				
				if (props.ContainsKey(fCom)){
					object nobj = props[fCom];
					string ncom = com.Substring(fCom.Length);
					
					return eval(nobj, ncom);
				} else {
					return "Unknown field or property";
				}
			} else if (closeParenIndex > -1){//it's a method
				//Debug.Log ("Method!");
				var meths = new Dictionary<string, List<MethodInfo>>();
				foreach (MethodInfo meth in oType.GetMethods(BindingFlags.Instance|BindingFlags.Public|BindingFlags.Static)){
					//Debug.Log(meth.Name);
					if (!meths.ContainsKey(meth.Name)){
						meths[meth.Name] = new List<MethodInfo>();
					}
					meths[meth.Name].Add(meth);
				}
				
				string fCom = com.Substring(0, parenIndex);
				string args = com.Substring(parenIndex + 1, closeParenIndex - parenIndex - 1);
				object[] parameters = parseArgs(args);

				if (meths.ContainsKey(fCom)){
					List<MethodInfo> options = meths[fCom];//all the overrides for this method
					
					//find the one with the right number of arguments
					foreach (MethodInfo meth in options){
						if (meth.GetParameters().Length == parameters.Length){//only check ones with the same number of params
							bool rightArgs = true;
							for (int i = 0; i < parameters.Length; i++){
								rightArgs = rightArgs && (meth.GetParameters()[i].ParameterType == parameters[i].GetType());//make sure that all the parameters are of the right types
							}
							
							if (rightArgs){
								//this is the right method, thank god
								//call it
								object result = meth.Invoke(o, parameters);
								string ncom = com.Substring(closeParenIndex + 1);
								//Debug.Log(result);
								
								return eval(result, ncom);
							}
						}
					}
				} else {//No known method
					return "Unknown method";
				}
			}//doesn't start with a period
		}

		//check if it's an operator
		if (com.IndexOfAny(new char[]{'+', '-', '*', '/'}) == 0){
			string ncom = com.Substring(1);
			object rE = eval(null, ncom);
			//Debug.Log(rE);
			return Operation(o, rE, com[0].ToString());
		}

		
		return "";
	}

	object Operation(object left, object right, string operat){
		try {
			ConstantExpression cLeft = Expression.Constant(left);
			ConstantExpression cRight = Expression.Constant(right);
			
			BinaryExpression op = null;

			if (operat == "+"){
				op = Expression.Add(cLeft, cRight);
			} else if (operat == "-"){
				op = Expression.Subtract(cLeft, cRight);
			}  else if (operat == "*"){
				op = Expression.Multiply(cLeft, cRight);
			}  else if (operat == "/"){
				op = Expression.Divide(cLeft, cRight);
			} else {
				return "Not a supported operator";
			}

			var objectMember = Expression.Convert(op, typeof(object));
			var getterLambda = Expression.Lambda<System.Func<object>>(objectMember);
			var getter = getterLambda.Compile();
			
			return getter(); // Throws an exception if + is not defined
			
		} catch (System.Exception e){
			return e;
		}
	}

	object[] parseArgs(string argumentString){

		//Debug.Log(argumentString);
		List<string> args = new List<string>();
		string cArg = "";
		int parenLevel = 0;
		for (int i = 0; i < argumentString.Length; i++){
			if (argumentString[i] == ',' && parenLevel == 0){
				if (cArg != ""){
					args.Add(cArg);
					cArg = "";
				}
			} else {
				cArg += argumentString[i];
				if (argumentString[i] == '('){
					parenLevel ++;
				} else if (argumentString[i] == ')'){
					parenLevel --;
				}
			}
		}
		if (cArg != ""){
			args.Add(cArg);
			cArg = "";
		}

		object[] parameters = new object[args.Count];
		//Debug.Log("Args: " + com.Substring(parenIndex + 1, closeParenIndex - parenIndex - 1));
		for (int i = 0; i < args.Count; i++){
			string arg = args[i].Trim();
			//Debug.Log(arg);

			parameters[i] = parseArg(arg);
		}

		return parameters;
	}

	object parseArg(string arg){
		string s;
		return parseArg(arg, out s);
	}

	object parseArg(string arg, out string remainingArg){
		arg = arg.Trim();

		string strMatch = arg.Match("(\\n|^)\\\"([^\\\"])*\\\"");
		string intMatch = arg.Match(@"(\n|^)(\d)*");
		string floatMatch = arg.Match(@"((\n|^)(\d)+\.(\d)+[fF]?)|((\n|^)(\d)+[fF])");

		object outVar;

		remainingArg = "";
		
		if (strMatch.Trim() != ""){//check if it's a string
			remainingArg = arg.Substring(strMatch.Length);
			return strMatch;
		} else if (floatMatch.Trim() != ""){//check if it's a float
			float floatVal = 0f;
			remainingArg = arg.Substring(floatMatch.Length);

			floatMatch = floatMatch.Trim().TrimEnd(new char[]{' ', 'f', 'F'});

			if (float.TryParse(floatMatch, out floatVal)){
				return floatVal;
			}
		} else if (intMatch.Trim() != ""){//check if it's an integer
			int intVal = 0;
			remainingArg = arg.Substring(intMatch.Length);

			intMatch = intMatch.Trim();
			
			if (int.TryParse(intMatch, out intVal)){
				return intVal;
			}
		} else if (arg.StartsWith("new")){//check if it's an object
			string oArg = arg;

			arg = arg.Substring(3).Trim();
			string oType = arg.Match(@"([^\(]+)").Trim();
			string oArgs = arg.Match(@"\(([^\)]+)\)");
			oArgs = oArgs.Substring(1, oArgs.Length - 2);

			Variable v;
			if (availableVars.TryGetValue(oType, out v)){
				System.Type rType = v.GetRealType();
				object[] cparams = parseArgs(oArgs);

				if (rType != null){
					int cpi = oArg.IndexOf(")");
					remainingArg = oArg.Substring(cpi > -1 ? cpi + 1 : oArg.Length - 1);
					//Debug.Log(remainingArg);
					return System.Activator.CreateInstance(rType, cparams);
				}
			}
		} else if (arg.StartsWith("var ")){//check if it's a variable declaration
			arg = arg.Substring("var ".Length);

			//Debug.Log(arg);
			int endOfNameIndex = arg.IndexOfAny(new char[]{' ', '='});
			string name = arg.Substring(0, endOfNameIndex);
			string val = arg.Substring(endOfNameIndex).Trim(new char[]{' ', '=', ';'});

			object obj = parseArg(val);
			availableVars[name] = new Variable(obj);
			return obj;
		} else if (arg.StartsWith("(")){//it's something within parentheses (probably)
			string paren = parenString(arg);
			if (paren != ""){
				remainingArg = arg.Substring(paren.Length);
				return eval(paren.Substring(1, paren.Length - 2));
			}
		} else if (parseVariable(arg, out outVar, out remainingArg)){//maybe it's a variable
			return outVar;
		}

		return null;
	}

	bool parseVariable(string arg, out object result, out string remainingArg){
		result = null;
		remainingArg = "";
		Variable v;

		for (int i = 0; i < arg.Length; i++){
			if (availableVars.TryGetValue(arg.Substring(0, arg.Length - i), out v)){
				result = v.GetObj();
				remainingArg = arg.Substring(arg.Length - i);
				bool works = remainingArg.Match(@"(\n|^)([\.\[\(])") != "";

				//Debug.Log(remainingArg);

				return works || remainingArg == "";
			}
		}

		return false;
	}

	string parenString(string com){
		int parenIndex = com.IndexOf("(");
		int closeParenIndex = com.IndexOf(")");
		
		if (parenIndex > -1 && closeParenIndex > -1){

			bool closed = false;

			int opened = 1;
			for (int i = parenIndex + 1; i < com.Length && opened != 0; i++){
				if (com[i] == '('){
					opened ++;
					
				} else if (com[i] == ')'){
					opened --;
					
					if (opened == 0){
						closeParenIndex = i;
						closed = true;
						break;
					}
				}
			}

			if (closed){
				return com.Substring(parenIndex, closeParenIndex - parenIndex + 1);
			}
		}

		return "";
	}

	void _log(string s){
		log.Add(s);
	}

	public static void Log(string s){
		log.Add(s);
	}
	
	public class Variable {
		
		//System.Type T;
		public object obj;
		
		public Variable(object nObj){
			//T = nObj.GetType();
			obj = nObj;
		}
		
		public dynamic GetObj(){
			return obj;
		}

		public System.Type GetRealType(){
			if (obj is System.Type){
				return obj as System.Type;
			} else {
				return obj.GetType();
			}
		}
		
	}
}
