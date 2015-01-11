using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;

public class ConsoleControl : MonoBehaviour {

	List<string> log;
	string command;
	public bool consoleOpen;

	Dictionary<string, Variable> availableVars;
	int relativeLine;

	// Use this for initialization
	void Start () {
		log = new List<string>();
		command = "MapLabel.FixPositionFor(-1)";
		relativeLine = 0;

		availableVars = new Dictionary<string, Variable>();

		allowClass(typeof(MapMaker));
		allowClass(typeof(Floor));
		allowClass(typeof(MapCameraControl));
		allowClass(typeof(MapLabel));

		//Debug.Log(eval("MapLabel.FixPositionFor(0)"));

	}

	void allowClass(System.Type type){
		availableVars[type.ToString()] = new Variable(type);
	}

	// Update is called once per frame
	void Update () {
		if (Application.platform == RuntimePlatform.WindowsEditor && Input.GetKeyDown(KeyCode.F1)){
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
		int maxRows = 5;

		string rLog = "";
		int logHeight = 0;
		int lineHeight = 25;
		for (int i = 0; i < log.Count; i++){
			if (i < maxRows){
				logHeight += lineHeight;
			}
			rLog += log[i] + "\n";
		}

		GUI.TextArea(new Rect(
			padding, Screen.height - 2*padding - commandHeight - logHeight, 
			Screen.width - 2*padding, logHeight
			), rLog);

		command = GUI.TextArea(new Rect(padding, Screen.height - padding - commandHeight, Screen.width - 2*padding, commandHeight), command);
		if (Event.current.keyCode == KeyCode.Return) {
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

		log.Add("> " + command);

		log.Add("\t" + eval(command));

		command = "";
	}

	string eval(string com){

		Variable v = null;
		foreach (string s in availableVars.Keys){
			if (com.StartsWith(s)){
				v = availableVars[s];
				com = com.Substring(s.Length);
				break;
			}
		}
		if (v != null){
			return eval(v.GetObj(), com);
		}

		return "";
	}

	string eval(object o, string com){

		if (o == null){
			return "null";
		}
		
		com = com.Trim(new char[]{' ', ';'});
		if (com == ""){
			return "<" + o.GetType().ToString() + ": " + o.ToString() + ">";
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

			//Debug.Log(com + "   period: " + periodIndex + "; paren: " + parenIndex);

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
				//TODO: parse out parameters
				string[] args = com.Substring(parenIndex + 1, closeParenIndex - parenIndex - 1).Split(
					new string[]{",", ", "}, System.StringSplitOptions.RemoveEmptyEntries);
				object[] parameters = new object[args.Length];
				//Debug.Log("Args: " + com.Substring(parenIndex + 1, closeParenIndex - parenIndex - 1));
				for (int i = 0; i < args.Length; i++){
					string arg = args[i].Trim();

					int intVal = 0;
					float floatVal = 0f;

					if (arg.StartsWith("\"") && arg.EndsWith("\"")){//check if it's a string
						parameters[i] = arg.Trim(new char[]{'"'});
					} else if (int.TryParse(arg, out intVal)){
						parameters[i] = intVal;
					} else if (float.TryParse(arg, out floatVal)){
						parameters[i] = floatVal;
					}

					//Debug.Log(parameters[i] + ", " + arg);
				}

				if (closeParenIndex == parenIndex + 1){
					parameters = new object[]{};
				}

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


		return "";
	}
	
	public class Variable {

		System.Type T;
		public object obj;

		public Variable(object nObj){
			T = nObj.GetType();
			obj = nObj;
		}
		
		public dynamic GetObj(){
			return obj;
		}
		
	}
}
