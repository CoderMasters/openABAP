using System;
using System.Collections.Generic;

namespace openABAP.Runtime
{
	public enum InternalType{ c,n,d,t,i,p,f };
		
	public interface Type
	{
		InternalType getInternalType();
		string getRuntimeClassname();
		int getLength();
	}
	
	public class TypeList : Dictionary<string, Type>
	{
	}
}

