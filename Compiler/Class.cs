using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace openABAP.Compiler
{
	/// <summary>
	/// enumeration of possible visibility of methods and data for class definition.
	/// public, protected, private
	/// </summary> 
	public enum Visibility { priv, prot, publ }; 
	
	/// <summary>
	/// the abap class command is used to store data declarations and method declarations.
	/// </summary>
	/// <exception cref='CompilerError'>
	/// Is thrown when the compiler error.
	/// </exception>
	public class Class : Command
	{
		public string Name;
		
		public DataList   Attributes = new DataList();
		public MethodList Methods    = new MethodList();  

		public Class (string name)
		{
			this.Name = name;
		}
		
		/// <summary>
		/// add a method-defintion to the class.
		/// </summary>
		/// <param name='method'>
		/// Method.
		/// </param>
		/// <exception cref='CompilerError'>
		/// Is thrown when the method already exists in the class.
		/// </exception>
		public void AddMethod( Method method ) 
		{
			try {
				Methods.Add( method.Name, method );
			} catch (ArgumentException) {
				throw new CompilerError("method " + method.Name + " already defined in class " + this.Name ); 
			}
		}
		
		/// <summary>
		/// get method defintion by method name.
		/// </summary>
		/// <returns>
		/// The method.
		/// </returns>
		/// <param name='name'>
		/// Name.
		/// </param>
		/// <exception cref='CompilerError'>
		/// Is thrown when no method with this name is declared for the class.
		/// </exception>
		public Method GetMethod( string name )
		{
			Method result = null;
			if ( ! this.Methods.TryGetValue(name, out result) )
			{
				throw new CompilerError( "no definition for method " + name + " in class " + this.Name );
			}
			return result;
		}
		
		/// <summary>
		/// Adds the attribute, which means a data declaration.
		/// </summary>
		/// <param name='data'>
		/// Data.
		/// </param>
		/// <exception cref='CompilerError'>
		/// Is thrown when an attribute with same name is already defined.
		/// </exception>
		public void AddAttribute( Data data ) 
		{ 
			try {
				Attributes.Add( data.Name, data );
			} catch (ArgumentException) {
				throw new CompilerError("data " + data.Name + " already defined in class " + this.Name ); 
			}
		}
		
		/// <summary>
		/// Get an attribute (a data) by name.
		/// </summary>
		/// <returns>
		/// The attribute.
		/// </returns>
		/// <param name='name'>
		/// Name.
		/// </param>
		/// <exception cref='CompilerError'>
		/// Is thrown when no attribute with given name is defined in class.
		/// </exception>
		public Data GetAttribute( string name )
		{
			Data result = null;
			if ( ! this.Attributes.TryGetValue(name, out result) )
			{
				throw new CompilerError("no definition of attribute " + name + " in class " + this.Name );
			}
			return result;
		}
		
		/// <summary>
		/// writes the class to cil file completely, including fields and methods.
		/// </summary>
		/// <param name='cil'>
		/// Stream.
		/// </param>
		public void WriteCil( CilFile cil ) {

			cil.Classname = this.Name;
			
			cil.WriteLine(".class private auto ansi beforefieldinit " + this.Name + " extends [mscorlib]System.Object");
			cil.WriteLine("{");

			//attributes
			foreach (KeyValuePair<string, Data> pair in this.Attributes) {
				pair.Value.WriteCil(cil);				
			}			

			//entry point static method Main 
			cil.WriteLine(".method private static hidebysig default void Main (string[] args)  cil managed");
            cil.WriteLine("{");
			cil.WriteLine(".entrypoint");
			cil.WriteLine("	.locals init (class {0}.{1} V_0)", cil.Namespace, cil.Classname);
			cil.WriteLine("newobj instance void class {0}.{1}::'.ctor'()", cil.Namespace, cil.Classname);
			cil.WriteLine("stloc.0"); 
			cil.WriteLine("ldloc.0"); 
			cil.WriteLine("callvirt instance void class {0}.{1}::Run()", cil.Namespace, cil.Classname);
			cil.WriteLine("ret ");
			cil.WriteLine("} // end of method Main");			

			//class-constructor
		    cil.WriteLine(".method private static specialname rtspecialname default void '.cctor' ()  cil managed");
			cil.WriteLine("{");
			//init static attributes
			foreach (KeyValuePair<string, Data> pair in this.Attributes) {
				if ( pair.Value.StaticMember ) {
					pair.Value.WriteCilNew(cil);
				}
			}						
			cil.WriteLine("ret");
		    cil.WriteLine("} // end of method .cctor");
			
			//constructor
			cil.WriteLine(".method public hidebysig specialname rtspecialname instance default void '.ctor' ()  cil managed ");
			cil.WriteLine("{");
			//init instance attributes
			foreach (KeyValuePair<string, Data> pair in this.Attributes) {
				if ( ! pair.Value.StaticMember ) {
					pair.Value.WriteCilNew(cil);	
				}
			}						
			//call super constructor 
			cil.WriteLine("ldarg.0 ");
			cil.WriteLine("call instance void object::'.ctor'()");
			cil.WriteLine("ret ");
			cil.WriteLine("} // end of method .ctor");
			
			//methods
			foreach (KeyValuePair<string, Method> pair in this.Methods) {
				pair.Value.WriteCil(cil);				
			}			
			
			cil.WriteLine("} // end of class");	
		}

		public void BuildAssembly (ILGenerator il)
		{
		}

		public System.Type BuildAssembly ( ModuleBuilder mb )
		{
	        TypeBuilder tb = mb.DefineType( this.Name, TypeAttributes.Public );
	        Type[] parameterTypes = { };
			ConstructorBuilder ctor1 = null;
			ILGenerator ctorIL = null;

			//attributes
			foreach (KeyValuePair<string, Data> pair in this.Attributes) {
				pair.Value.BuildAssembly( tb );
			}	

			// define class constructor
//	        ctor1 = tb.DefineConstructor(
//	            MethodAttributes.Public | MethodAttributes.Static, 
//	            CallingConventions.Standard, 
//	            parameterTypes);
//        	ctorIL = ctor1.GetILGenerator();
//			// init static attributes of the class
//			foreach (KeyValuePair<string, Data> pair in this.Attributes) {
//				if ( pair.Value.StaticMember ) {
//					pair.Value.BuildAssembly( ctorIL );
//				}
//			}

	        // define constructor 
	        ctor1 = tb.DefineConstructor(
	            MethodAttributes.Public, 
	            CallingConventions.Standard, 
	            parameterTypes);
         	ctorIL = ctor1.GetILGenerator();

			// call constructor of System.Object
			ctorIL.Emit(OpCodes.Ldarg_0);
        	ctorIL.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));
        
			// init attributes of the class
			foreach (KeyValuePair<string, Data> pair in this.Attributes) {
				if ( ! pair.Value.StaticMember ) {
					pair.Value.BuildAssembly( ctorIL );
				}
			}
			ctorIL.Emit(OpCodes.Ret);

			//methods
			foreach (KeyValuePair<string, Method> pair in this.Methods) {
				pair.Value.BuildAssembly(tb);				
			}	

			return tb.CreateType();
		}

	}
	
	/// <summary>
	/// list of classes sorted by name. quick access to a class by binary tree search algorithm.
	/// </summary>
	public class ClassList : SortedList<string, Class>
	{
	}
}

