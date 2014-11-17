//--------------------------------------
//         Nitro Script Engine
//          Wrench Framework
//
//        For documentation or 
//    if you have any issues, visit
//         nitro.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using System.Reflection;
using System.Collections;
using System.Reflection.Emit;
using Wrench;


namespace Nitro{
	
	/// <summary>
	/// This is the heart of Nitro. A debuggable IL stream that all nitro outputs IL into.
	/// It is essentially a wrapper for the ILGenerator class, enabling the output to be logged.
	/// It will automatically log its content by setting defining an 'ildebug' compile time variable.
	/// </summary>
	
	public class NitroIL{
		
		/// <summary>If debugging is active, this is simply a counter used to track the line number.</summary>
		public int ILLine;
		/// <summary>The debugging output as a string, if debugging is active.</summary>
		public string DebugOutput;
		/// <summary>The internal ILGenerator that all IL will actually be emitted to.</summary>
		public ILGenerator Generator;
		
		
		/// <summary>Creates a new Nitro IL generator with the given system ILGenerator.</summary>
		/// <param name="generator">The system ILGenerator that this class will output all IL to.</param>
		public NitroIL(ILGenerator generator){
			Generator=generator;
		}
		
		/// <summary>Outputs the given textual string to the DebugOutput variable, appending a line number.</summary>
		/// <param name="line">The line of text to add to the output.</param>
		public void Output(string line){
			if(DebugOutput==null){
				DebugOutput=ILLine+": "+line;
			}else{
				DebugOutput+="\n"+ILLine+": "+line;
			}
			ILLine++;
		}
		
		/// <summary>Defines a label that marks a position in the IL.</summary>
		/// <returns>A new label.</returns>
		public Label DefineLabel(){
			return Generator.DefineLabel();
		}
		
		/// <summary>Marks the location of the given label in the IL.</summary>
		/// <param name="label">The label to locate.</param>
		public void MarkLabel(Label label){
			#if ildebug
				Output("LABEL:");
			#endif
			Generator.MarkLabel(label);
		}
		
		/// <summary>Adds the specified instruction into the IL.</summary>
		/// <param name="opcode">The instruction to add.</param>
		public void Emit(OpCode opcode){
			#if ildebug
				Output(opcode.ToString());
			#endif
			Generator.Emit(opcode);
		}
		
		/// <summary>Adds the specified instruction and the local field into the IL.</summary>
		/// <param name="opcode">The instruction to add.</param>
		/// <param name="builder">The local builder that should be outputted into the IL.</param>
		public void Emit(OpCode opcode,LocalBuilder builder){
			#if ildebug
				Output(opcode+" local "+builder.LocalIndex);
			#endif
			Generator.Emit(opcode,builder);
		}
		
		/// <summary>Adds the specified instruction and the given constructor to call into the IL.</summary>
		/// <param name="opcode">The instruction to add.</param>
		/// <param name="constructor">The constructor info that should be outputted into the IL.</param>
		public void Emit(OpCode opcode,ConstructorInfo constructor){
			#if ildebug
				Output(opcode+" new "+constructor.DeclaringType);
			#endif
			Generator.Emit(opcode,constructor);
		}
		
		/// <summary>Adds the specified instruction and the given method to call into the IL.</summary>
		/// <param name="opcode">The instruction to add.</param>
		/// <param name="method">The method info that should be outputted into the IL.</param>
		public void Emit(OpCode opcode,MethodInfo method){
			#if ildebug
				Output(opcode+" "+method.DeclaringType+" "+method.ToString());
			#endif
			Generator.Emit(opcode,method);
		}
		
		/// <summary>Adds the specified instruction and the given field into the IL.</summary>
		/// <param name="opcode">The instruction to add.</param>
		/// <param name="field">The field that should be outputted into the IL.</param>
		public void Emit(OpCode opcode,FieldInfo field){
			#if ildebug
				Output(opcode+" "+field);
			#endif
			Generator.Emit(opcode,field);
		}
		
		/// <summary>Adds the specified instruction and numerical argument into the IL.</summary>
		/// <param name="opcode">The instruction to add.</param>
		/// <param name="value">A numerical argument to add into the IL.</param>
		public void Emit(OpCode opcode,byte value){
			#if ildebug
				Output(opcode+" "+value);
			#endif
			Generator.Emit(opcode,value);
		}
		
		/// <summary>Adds the specified instruction and numerical argument into the IL.</summary>
		/// <param name="opcode">The instruction to add.</param>
		/// <param name="value">A numerical argument to add into the IL.</param>
		public void Emit(OpCode opcode,sbyte value){
			#if ildebug
				Output(opcode+" "+value);
			#endif
			Generator.Emit(opcode,value);
		}
		
		/// <summary>Adds the specified instruction and numerical argument into the IL.</summary>
		/// <param name="opcode">The instruction to add.</param>
		/// <param name="value">A numerical argument to add into the IL.</param>
		public void Emit(OpCode opcode,double value){
			#if ildebug
				Output(opcode+" "+value);
			#endif
			Generator.Emit(opcode,value);
		}
		
		/// <summary>Adds the specified instruction and numerical argument into the IL.</summary>
		/// <param name="opcode">The instruction to add.</param>
		/// <param name="value">A numerical argument to add into the IL.</param>
		public void Emit(OpCode opcode,float value){
			#if ildebug
				Output(opcode+" "+value);
			#endif
			Generator.Emit(opcode,value);
		}
		
		/// <summary>Adds the specified instruction and numerical argument into the IL.</summary>
		/// <param name="opcode">The instruction to add.</param>
		/// <param name="value">A numerical argument to add into the IL.</param>
		public void Emit(OpCode opcode,short value){
			#if ildebug
				Output(opcode+" "+value);
			#endif
			Generator.Emit(opcode,value);
		}
		
		/// <summary>Adds the specified instruction and numerical argument into the IL.</summary>
		/// <param name="opcode">The instruction to add.</param>
		/// <param name="value">A numerical argument to add into the IL.</param>
		public void Emit(OpCode opcode,int value){
			#if ildebug
				Output(opcode+" "+value);
			#endif
			Generator.Emit(opcode,value);
		}
		
		/// <summary>Adds the specified instruction and numerical argument into the IL.</summary>
		/// <param name="opcode">The instruction to add.</param>
		/// <param name="value">A numerical argument to add into the IL.</param>
		public void Emit(OpCode opcode,long value){
			#if ildebug
				Output(opcode+" "+value);
			#endif
			Generator.Emit(opcode,value);
		}
		
		/// <summary>Adds the specified instruction and textual argument into the IL.</summary>
		/// <param name="opcode">The instruction to add.</param>
		/// <param name="value">A textual argument to add into the IL.</param>
		public void Emit(OpCode opcode,string value){
			#if ildebug
				Output(opcode+" '"+value+"'");
			#endif
			Generator.Emit(opcode,value);
		}
		
		/// <summary>Adds the specified instruction and system type argument into the IL.</summary>
		/// <param name="opcode">The instruction to add.</param>
		/// <param name="value">A system type argument to add into the IL.</param>
		public void Emit(OpCode opcode,Type type){
			#if ildebug
				Output(opcode+" "+type);
			#endif
			Generator.Emit(opcode,type);
		}
		
		/// <summary>Adds the specified instruction and label argument into the IL. E.g. for jump operations.</summary>
		/// <param name="opcode">The instruction to add.</param>
		/// <param name="value">A label argument to add into the IL.</param>
		public void Emit(OpCode opcode,Label label){
			#if ildebug
				Output(opcode+" [LABEL]");
			#endif
			Generator.Emit(opcode,label);
		}
		
		
		/// <summary>Creates a new local variable with the specified system type.</summary>
		/// <param name="localType">The type of the value this variable will hold.</param>
		/// <returns>A new local builder.</returns>
		public LocalBuilder DeclareLocal(Type localType){
			return Generator.DeclareLocal(localType);
		}
		
		/// <summary>Done is called when this IL stream is completed. When debug is active, it
		/// automatically logs the DebugOutput.</summary>
		/// <param name="message">An extra message to add to the output.</param>
		public void Done(string message){
			#if ildebug
				if(message==null){
					message="IL:\n";
				}else{
					message+="\nIL:\n";
				}
				if(DebugOutput==null){
					message+="[EMPTY]";
				}else{
					message+=DebugOutput;
				}
				Log.Add(message);
			#endif
		}
		
	}
	
}