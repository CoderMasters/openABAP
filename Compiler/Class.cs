using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;

namespace openABAP.Compiler
{
	/// <summary>
	/// the abap class command is used to store data declarations and method declarations.
	/// </summary>
	/// <exception cref='CompilerError'>
	/// Is thrown when the compiler error.
	/// </exception>
	public class Class
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

		public void BuildAssembly (ILGenerator il)
		{
		}

		public System.Type BuildAssembly ( ModuleBuilder mb, ISymbolDocumentWriter doc )
		{
	        TypeBuilder tb = mb.DefineType( this.Name, TypeAttributes.Public );
	        Type[] parameterTypes = { };
			ConstructorBuilder ctor = null;
			ILGenerator il = null;

			//attributes
			foreach (KeyValuePair<string, Data> pair in this.Attributes) {
				pair.Value.BuildAssembly( tb );
			}	

			// define class constructor
	        ctor = tb.DefineConstructor(
	            MethodAttributes.Public | MethodAttributes.Static, 
	            CallingConventions.Standard, 
	            parameterTypes);
        	il = ctor.GetILGenerator();
			// init static attributes of the class
			foreach (KeyValuePair<string, Data> pair in this.Attributes) {
				if ( pair.Value.StaticMember ) {
					pair.Value.BuildAssembly( il );
				}
			}
			il.Emit(OpCodes.Ret);

	        // define constructor 
	        ctor = tb.DefineConstructor(
	            MethodAttributes.Public, 
	            CallingConventions.Standard, 
	            parameterTypes);
         	il = ctor.GetILGenerator();

			// call constructor of System.Object
			il.Emit(OpCodes.Ldarg_0);
        	il.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));
        
			// init attributes of the class
			foreach (KeyValuePair<string, Data> pair in this.Attributes) {
				if ( ! pair.Value.StaticMember ) {
					pair.Value.BuildAssembly( il );
				}
			}
			il.Emit(OpCodes.Ret);

			//methods
			foreach (KeyValuePair<string, Method> pair in this.Methods) {
				pair.Value.BuildAssembly(tb, doc);				
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

