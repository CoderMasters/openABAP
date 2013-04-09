using System;

namespace openABAP.Runtime
{
	public class NamedType : TypeDescr
	{
		private string Name;
		private TypeDescr RefType;
		
		public NamedType (string name, TypeDescr aType=null)
		{
			this.Name = name;
			this.RefType = aType;
		}
		
		public void setRefType( TypeDescr aType)
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

