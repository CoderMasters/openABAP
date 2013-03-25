using System;

namespace openABAP.Runtime
{
	public class NamedType : Type
	{
		private string Name;
		private Type RefType;
		
		public NamedType (string name, Type aType=null)
		{
			this.Name = name;
			this.RefType = aType;
		}
		
		public void setRefType( Type aType)
		{
			this.RefType = aType;
		}
		
		public string getName() 
		{
			return this.Name;
		}
		
		public InternalType getInternalType()
		{
			return this.RefType.getInternalType();
		}
		public string getRuntimeClassname()
		{
			return this.RefType.getRuntimeClassname();
		}
		public System.Type getRuntimeType ()
		{
			return this.RefType.getRuntimeType();
		}
		
		public int getLength()
		{
			return this.RefType.getLength();
		}

	}
}

