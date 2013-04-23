using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace openABAP.Compiler
{
	public class ClassBuilder
	{
		private TypeBuilder tb = null; 
		private MethodBuilder mb = null;
		private Dictionary<string, MethodBuilder> mbList = new Dictionary<string, MethodBuilder>();
		private Dictionary<string, FieldBuilder>  fbList = new Dictionary<string, FieldBuilder>();
		public System.Type CreatedClass = null;
		private ILGenerator cil = null;  //ILGenerator for the constructor
		private ILGenerator ccil = null;  //ILGenerator for the class-constructor

		public ClassBuilder (TypeBuilder tb)
		{
			this.tb = tb;
			System.Type[] param = {};

			ConstructorBuilder ctor = tb.DefineConstructor( MethodAttributes.Public, CallingConventions.Standard, param );
			cil = ctor.GetILGenerator();

			ctor = tb.DefineConstructor(MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, param);
			ccil = ctor.GetILGenerator();
		}

		/// <summary>
		/// creates a MethodBuilder and stores is in this.mbList
		/// </summary>
		/// <param name="name"></param>
		/// <param name="attr"></param>
		public MethodBuilder DefineMethod(string name, MethodAttributes attr)
		{
			this.mb = tb.DefineMethod(name, attr);
			mbList.Add (name, this.mb);
			return this.mb;
		}

		public MethodBuilder GetMethodBuilder(string name)
		{
			if (mbList.TryGetValue(name, out mb)) {
				return mb;
			} else {
				throw new CompilerError( "unknown method " + name );
			}
		}

		public FieldBuilder DefineField (string name, Runtime.TypeDescr type, FieldAttributes attr)
		{
			ILGenerator il;
			FieldBuilder fb = tb.DefineField(name, type.getRuntimeType(), attr);
			fbList.Add (name, fb);

			// emit initializing of fild in constructor
			System.Type rtc = type.getRuntimeType ();
			System.Type[] types = null;
			if ((attr & FieldAttributes.Static) == 0) {
				cil.Emit (OpCodes.Ldarg_0);
				il = cil;
			} else {
				il = ccil;
			}
			switch (type.getInternalType ()) {
			case Runtime.InternalType.c:
				il.Emit (OpCodes.Ldc_I4, type.getLength ()); //push 1st argument = text length
				il.Emit (OpCodes.Ldstr, "");                      //push 2nd argument = inital string
				types = new Type[2];
				types [0] = typeof(int);
				types [1] = typeof(string);
				il.Emit (OpCodes.Newobj, rtc.GetConstructor (types));
				break;
			case Runtime.InternalType.n: 
				break;
			case Runtime.InternalType.d: 
				break;
			case Runtime.InternalType.t: 
				break;
			case Runtime.InternalType.i:
				il.Emit (OpCodes.Ldc_I4, 0);                   //push 1st argument = initial integer value
				types = new Type[1];
				types [0] = typeof(int);
				il.Emit (OpCodes.Newobj, rtc.GetConstructor (types));
				break;
			case Runtime.InternalType.p: 
				il.Emit (OpCodes.Ldc_R8, 0.0);                 //push 1st argument = initial integer value
				types = new Type[1];
				types [0] = typeof(int);
				il.Emit (OpCodes.Newobj, rtc.GetConstructor (types));
				break;
			}
			if ((attr & FieldAttributes.Static) != 0) {
				il.Emit (OpCodes.Stsfld, fb);
			} else {
				il.Emit (OpCodes.Stfld, fb);
			}

			return fb;
		}

		public FieldInfo GetFieldInfo(string name)
		{
			FieldBuilder fb;
			fbList.TryGetValue(name, out fb);
			return fb;
		}

		public void EndClass()
		{
			//end of constructor 
			cil.Emit(OpCodes.Ret);
			ccil.Emit(OpCodes.Ret);

			this.CreatedClass = tb.CreateType();
		}

		public void EmitConstructor( )
		{
			Type[] parameterTypes = { };
			ConstructorBuilder ctor = null;
			ILGenerator il = null;

			// define class constructor
			ctor = tb.DefineConstructor(
				MethodAttributes.Public | MethodAttributes.Static, 
				CallingConventions.Standard, 
				parameterTypes);
			il = ctor.GetILGenerator();
			// init static attributes of the class
			foreach (KeyValuePair<string, FieldBuilder> pair in this.fbList) {
				//if ( pair.Value.StaticMember ) {
				//	pair.Value.BuildAssembly( il );
				//}
			}
			il.Emit(OpCodes.Ret);
			
			// define constructor 
			/*
			ctor = tb.DefineConstructor(
				MethodAttributes.Public, 
				CallingConventions.Standard, 
				parameterTypes);
			il = ctor.GetILGenerator();
			*/
			
			// call constructor of System.Object
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));

			// init instance attributes of the class
			foreach (KeyValuePair<string, FieldBuilder> pair in this.fbList) {
				//if ( ! pair.Value.StaticMember ) {
				//	pair.Value.BuildAssembly( il );
				//}
			}		
		}
	}
}

