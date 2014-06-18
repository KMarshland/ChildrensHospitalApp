using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class WireAction {

	static List<WireAction> actions = new List<WireAction>();
	static WireAction empty = new WireAction(null);

	public WireAction parent;
	protected List<WireAction> childActions = new List<WireAction>();

	string name;
	string target;
	Action run; 

	bool hasRun = false;

	public WireAction( WireAction nparent){
		actions.Add(this);
		parent = nparent;
		if (parent != null){
			parent.childActions.Add(this);
		}
		run = emptyRun;
	}

	public void generateRun(){
		run = generatedRun;
		hasRun = true;
	}

	public void generatedRun(){
		foreach (WireAction w in childActions){
			w.run();
		}
	}

	public void create(){
		WireElement c = ClassElement.GetClassByName(target);
		UnityFromWire.bufferedDisplay.Add(c);
	}

	public void delete(){
		WireElement c = ClassElement.GetClassByName(target);
		UnityFromWire.bufferedDisplay.Remove(c);
	}

	public Action Run {
		get {
			if (!hasRun){
				generateRun();
			}
			return run;
		}
		set {
			hasRun = true;
			run = value;
		}
	}

	public string Name {
				get {
						return name;
				}
				set {
						name = value;
				}
	}

	public string Target {
				get {
						return target;
				}
				set {
						target = value;
				}
		}

	static void emptyRun(){

	}

	public static WireAction GetActionByName(string name){
		foreach (WireAction w in actions){
			if (w.name == name){
				return w;
			}
		}
		return empty;
	}
}
