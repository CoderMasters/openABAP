using System;

namespace openABAP.Runtime
{
	public class BuildinType : TypeDescr
	{
		private InternalType Type;
		private int Length;
		
		public BuildinType ( InternalType intType = InternalType.c, int len = 0 )
		{
			this.Type = intType;
			if (len == 0) {
				this.Length = getDefaultLength( intType );
			} else {
				this.setLength( len );
			}
		}

		public void setLength( int len )
		{
			switch (this.Type) {
				case InternalType.c: this.Length = len; break;
				case InternalType.n: this.Length = len; break;
				default:             break;//error
			}
		}

		public InternalType getInternalType()
		{
			return this.Type;
		}

		public string getRuntimeClassname()
		{
			switch (this.Type) {
				case InternalType.c: return CharValue.RuntimeClassname;
				case InternalType.n: return "";
				case InternalType.d: return "";
				case InternalType.t: return "";
				case InternalType.i: return IntValue.RuntimeClassname;
				case InternalType.p: return DecValue.RuntimeClassname;
				default:             return ""; //error
			}
		}

		public System.Type getRuntimeType()
		{
			switch (this.Type) {
				case InternalType.c: return typeof(CharValue);
				case InternalType.n: return null;
				case InternalType.d: return null;
				case InternalType.t: return null;
				case InternalType.i: return typeof(IntValue);
				case InternalType.p: return typeof(DecValue);
				default:             return null; //error
			}
		}
		public int getLength() {
			return this.Length;
		}
		
		public static int getDefaultLength( InternalType intType )
		{
			switch (intType) {
				case InternalType.c: return 1;
				case InternalType.n: return 1;
				case InternalType.d: return 8;
				case InternalType.t: return 6;
				case InternalType.i: return 4;
				case InternalType.p: return 6;
			    default:             return 0;  //fehler
			}
		}		
		
	}
}

