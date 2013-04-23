using System.Collections;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using openABAP.Compiler;
using openABAP.Runtime;



using System;

namespace openABAP.Coco {



public class Parser {
	public const int _EOF = 0;
	public const int _number = 1;
	public const int _string = 2;
	public const int _ident = 3;
	public const int _SPACE = 4;
	public const int _DOT = 5;
	public const int _MINUS = 6;
	public const int _PLUS = 7;
	public const int _MULTIPLY = 8;
	public const int _DIVIDE = 9;
	public const int _PROGRAM = 10;
	public const int _CLASS = 11;
	public const int _DEFINITION = 12;
	public const int _IMPLEMENTATION = 13;
	public const int _ENDCLASS = 14;
	public const int _PUBLIC = 15;
	public const int _PROTECTED = 16;
	public const int _PRIVATE = 17;
	public const int _SECTION = 18;
	public const int _METHODS = 19;
	public const int _CLASS_METHODS = 20;
	public const int _METHOD = 21;
	public const int _ENDMETHOD = 22;
	public const int _WRITE = 23;
	public const int _DATA = 24;
	public const int _CLASS_DATA = 25;
	public const int _TYPE = 26;
	public const int _LENGTH = 27;
	public const int _DECIMALS = 28;
	public const int _NO_SIGN = 29;
	public const int _MOVE = 30;
	public const int _to = 31;
	public const int maxT = 45;

	const bool T = true;
	const bool x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;

public openABAP.Compiler.Compiler Context = null;
	
//------------------------------------------------------------------------



	public Parser(Scanner scanner) {
		this.scanner = scanner;
		errors = new Errors();
	}

	void SynErr (int n) {
		if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
		errDist = 0;
	}

	public void SemErr (string msg) {
		if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
		errDist = 0;
	}
	
	void Get () {
		for (;;) {
			t = la;
			la = scanner.Scan();
			if (la.kind <= maxT) { ++errDist; break; }

			la = t;
		}
	}
	
	void Expect (int n) {
		if (la.kind==n) Get(); else { SynErr(n); }
	}
	
	bool StartOf (int s) {
		return set[s, la.kind];
	}
	
	void ExpectWeak (int n, int follow) {
		if (la.kind == n) Get();
		else {
			SynErr(n);
			while (!StartOf(follow)) Get();
		}
	}


	bool WeakSeparator(int n, int syFol, int repFol) {
		int kind = la.kind;
		if (kind == n) {Get(); return true;}
		else if (StartOf(repFol)) {return false;}
		else {
			SynErr(n);
			while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind])) {
				Get();
				kind = la.kind;
			}
			return StartOf(syFol);
		}
	}

	
	void openABAP() {
		Expect(10);
		Expect(3);
		Context.DefineProgram(t.val); 
		Expect(5);
		while (la.kind == 11) {
			class_command();
		}
	}

	void class_command() {
		Expect(11);
		Expect(3);
		if (la.kind == 12) {
			class_definition(t.val);
		} else if (la.kind == 13) {
			class_implementation(t.val);
		} else SynErr(46);
	}

	void class_definition(string name) {
		Expect(12);
		Expect(5);
		Context.DefineClass(name); 
		if (la.kind == 15) {
			public_section();
		}
		if (la.kind == 16) {
			protected_section();
		}
		if (la.kind == 17) {
			private_section();
		}
		Expect(14);
		Expect(5);
	}

	void class_implementation(string name) {
		Expect(13);
		Expect(5);
		Context.ImplementClass( name ); 
		while (la.kind == 21) {
			method_command();
		}
		Expect(14);
		Expect(5);
		Context.EndClass( ); 
	}

	void public_section() {
		Expect(15);
		Expect(18);
		Expect(5);
		Context.CurrentVisibility = Visibility.publ; 
		while (StartOf(1)) {
			section();
		}
	}

	void protected_section() {
		Expect(16);
		Expect(18);
		Expect(5);
		Context.CurrentVisibility = Visibility.prot; 
		while (StartOf(1)) {
			section();
		}
	}

	void private_section() {
		Expect(17);
		Expect(18);
		Expect(5);
		Context.CurrentVisibility = Visibility.prot; 
		while (StartOf(1)) {
			section();
		}
	}

	void method_command() {
		Expect(21);
		Expect(3);
		Context.ImplementMethod( t ); 
		Expect(5);
		while (StartOf(2)) {
			command_block();
		}
		Expect(22);
		Expect(5);
		Context.EndMethod( t ); 
	}

	void section() {
		if (la.kind == 19) {
			methods_command();
		} else if (la.kind == 20) {
			class_methods_command();
		} else if (la.kind == 24) {
			data_command();
		} else if (la.kind == 25) {
			class_data_command();
		} else SynErr(47);
	}

	void methods_command() {
		Expect(19);
		Expect(3);
		Context.DefineMethod(t, isStatic:false); 
		Expect(5);
	}

	void class_methods_command() {
		Expect(20);
		Expect(3);
		Context.DefineMethod(t, isStatic:true); 
		Expect(5);
	}

	void data_command() {
		Expect(24);
		if (la.kind == 3) {
			data_parameter(false);
		} else if (la.kind == 32) {
			Get();
			data_parameter(false);
			if (la.kind == 33) {
				Get();
				data_parameter(false);
			}
		} else SynErr(48);
		Expect(5);
	}

	void class_data_command() {
		Expect(25);
		if (la.kind == 3) {
			data_parameter(true);
		} else if (la.kind == 32) {
			Get();
			data_parameter(true);
			if (la.kind == 33) {
				Get();
				data_parameter(true);
			}
		} else SynErr(49);
		Expect(5);
	}

	void data_parameter(bool isStatic) {
		Expect(3);
		string name = t.val; TypeDescr type = new Runtime.BuildinType(); 
		if (la.kind == 26) {
			data_type(out type);
		}
		Context.DefineField(name, type, isStatic); 
	}

	void data_type(out TypeDescr type) {
		Expect(26);
		type = null; BuildinType bt; 
		if (la.kind == 3) {
			Get();
			type = new Runtime.NamedType(t.val); 
		} else if (StartOf(3)) {
			buildintype(out bt);
			type = bt; 
		} else SynErr(50);
	}

	void buildintype(out Runtime.BuildinType type) {
		type = null; 
		if (la.kind == 34 || la.kind == 35 || la.kind == 36) {
			fixlentype(out type);
		} else if (la.kind == 37 || la.kind == 38) {
			varlentype(out type);
		} else if (la.kind == 39) {
			packedtype(out type);
		} else SynErr(51);
	}

	void command_block() {
		if (la.kind == 24) {
			data_command();
		} else if (la.kind == 23) {
			write_command();
		} else if (la.kind == 30) {
			move_command();
		} else if (la.kind == 3 || la.kind == 4) {
			compute_command();
		} else SynErr(52);
	}

	void write_command() {
		Expect(23);
		Write cmd = new Write(t); 
		if (la.kind == 32) {
			Get();
			write_parameter(cmd);
			while (la.kind == 33) {
				Get();
				cmd = new Write(t); 
				write_parameter(cmd);
			}
		} else if (StartOf(4)) {
			write_parameter(cmd);
		} else SynErr(53);
		Expect(5);
	}

	void move_command() {
		Compiler.IfValue aValue;   
		Expect(30);
		Move cmd = new Move(t); 
		value(out cmd.Source);
		Expect(31);
		variable(out cmd.Target);
		Expect(5);
		Context.EmitCommand( cmd); 
	}

	void compute_command() {
		openABAP.Compiler.IfVariable target; 
		variable(out target);
		Compute cmd = new Compute(t); cmd.Target = target; 
		Expect(44);
		expression(out cmd.Expression);
		Expect(5);
		cmd.End(t); Context.EmitCommand( cmd ); 
	}

	void fixlentype(out Runtime.BuildinType aType) {
		aType = null; 
		if (la.kind == 34) {
			Get();
			aType = new Runtime.BuildinType( Runtime.InternalType.i ); 
		} else if (la.kind == 35) {
			Get();
			aType = new Runtime.BuildinType( Runtime.InternalType.d ); 
		} else if (la.kind == 36) {
			Get();
			aType = new Runtime.BuildinType( Runtime.InternalType.t ); 
		} else SynErr(54);
	}

	void varlentype(out Runtime.BuildinType aType) {
		aType = null; 
		if (la.kind == 37) {
			Get();
			aType = new Runtime.BuildinType( Runtime.InternalType.c ); 
		} else if (la.kind == 38) {
			Get();
			aType = new Runtime.BuildinType( Runtime.InternalType.n ); 
		} else SynErr(55);
		if (la.kind == 27) {
			Get();
			Expect(1);
			aType.setLength ( Convert.ToInt32(t.val) ); 
		}
	}

	void packedtype(out Runtime.BuildinType aType) {
		aType = null; 
		Expect(39);
		aType = new Runtime.BuildinType( Runtime.InternalType.p ); 
		if (la.kind == 27) {
			Get();
			Expect(1);
			aType.setLength( Convert.ToInt32(t.val) ); 
		}
		if (la.kind == 28) {
			Get();
			Expect(1);
		}
		if (la.kind == 29) {
			Get();
		}
	}

	void write_parameter(Write cmd) {
		if (la.kind == 9) {
			format(cmd);
		}
		value(out cmd.Value);
		cmd.End(t); Context.EmitCommand( cmd ); 
	}

	void format(Write cmd) {
		Expect(9);
		cmd.Format = t.val; 
	}

	void value(out Compiler.IfValue value) {
		value = null; IfVariable var=null; 
		if (la.kind == 1 || la.kind == 2) {
			literal(out value);
		} else if (la.kind == 3 || la.kind == 4) {
			variable(out var);
			value = var; 
		} else SynErr(56);
	}

	void literal(out Compiler.IfValue value) {
		value = null; 
		if (la.kind == 2) {
			Get();
			value = new Compiler.StringLiteral( t.val.Substring(1,t.val.Length-2) ); 
		} else if (la.kind == 1) {
			Get();
			value = new Compiler.IntegerLiteral( Convert.ToInt32( t.val) ); 
		} else SynErr(57);
	}

	void variable(out Compiler.IfVariable variable) {
		Scanner.ignore_space = false; 
		while (la.kind == 4) {
			Get();
		}
		Expect(3);
		variable = Context.GetVariable( t.val ); 
		while (la.kind == 6 || la.kind == 42 || la.kind == 43) {
			if (la.kind == 42) {
				instance_member();
			} else if (la.kind == 43) {
				class_member();
			} else {
				struct_field();
			}
		}
		if (la.kind == 7) {
			Get();
			Expect(1);
		}
		if (la.kind == 40) {
			Get();
			Expect(1);
			Expect(41);
		}
		while (la.kind == 4) {
			Get();
		}
		Scanner.ignore_space = true; 
	}

	void instance_member() {
		Expect(42);
		Expect(3);
	}

	void class_member() {
		Expect(43);
		Expect(3);
	}

	void struct_field() {
		Expect(6);
		Expect(3);
	}

	void expression(out Compiler.IfExpression e) {
		e=null; string op; IfExpression e2; 
		if (la.kind == 6) {
			Get();
			term(out e2);
			e = new UnaryExpression ("-", e2 ); 
		} else if (StartOf(5)) {
			term(out e);
		} else SynErr(58);
		while (la.kind == 6 || la.kind == 7) {
			addition(out op);
			term(out e2);
			e = new BinaryExpression(e, op, e2); 
		}
	}

	void term(out Compiler.IfExpression e) {
		string op; IfExpression e2; e=null; 
		factor(out e);
		while (la.kind == 8 || la.kind == 9) {
			multiply(out op);
			factor(out e2);
			e = new BinaryExpression(e, op, e2); 
		}
	}

	void addition(out string op) {
		if (la.kind == 7) {
			Get();
		} else if (la.kind == 6) {
			Get();
		} else SynErr(59);
		op = t.val; 
	}

	void factor(out Compiler.IfExpression e) {
		Compiler.IfValue val; e=null; 
		if (StartOf(6)) {
			value(out val);
			e = new ExpressionLeaf( val ); 
		} else if (la.kind == 40) {
			Get();
			expression(out  e);
			Expect(41);
		} else SynErr(60);
	}

	void multiply(out string op) {
		if (la.kind == 8) {
			Get();
		} else if (la.kind == 9) {
			Get();
		} else SynErr(61);
		op = t.val; 
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		openABAP();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, x,x,x,x, x,x,x},
		{x,T,T,T, T,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,T,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x},
		{x,T,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x}

	};
} // end Parser


public class Errors {
	public int count = 0;                                    // number of errors detected
	public System.IO.TextWriter errorStream = Console.Out;   // error messages go to this stream
	public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text

	public virtual void SynErr (int line, int col, int n) {
		string s;
		switch (n) {
			case 0: s = "EOF expected"; break;
			case 1: s = "number expected"; break;
			case 2: s = "string expected"; break;
			case 3: s = "ident expected"; break;
			case 4: s = "SPACE expected"; break;
			case 5: s = "DOT expected"; break;
			case 6: s = "MINUS expected"; break;
			case 7: s = "PLUS expected"; break;
			case 8: s = "MULTIPLY expected"; break;
			case 9: s = "DIVIDE expected"; break;
			case 10: s = "PROGRAM expected"; break;
			case 11: s = "CLASS expected"; break;
			case 12: s = "DEFINITION expected"; break;
			case 13: s = "IMPLEMENTATION expected"; break;
			case 14: s = "ENDCLASS expected"; break;
			case 15: s = "PUBLIC expected"; break;
			case 16: s = "PROTECTED expected"; break;
			case 17: s = "PRIVATE expected"; break;
			case 18: s = "SECTION expected"; break;
			case 19: s = "METHODS expected"; break;
			case 20: s = "CLASS_METHODS expected"; break;
			case 21: s = "METHOD expected"; break;
			case 22: s = "ENDMETHOD expected"; break;
			case 23: s = "WRITE expected"; break;
			case 24: s = "DATA expected"; break;
			case 25: s = "CLASS_DATA expected"; break;
			case 26: s = "TYPE expected"; break;
			case 27: s = "LENGTH expected"; break;
			case 28: s = "DECIMALS expected"; break;
			case 29: s = "NO_SIGN expected"; break;
			case 30: s = "MOVE expected"; break;
			case 31: s = "to expected"; break;
			case 32: s = "\":\" expected"; break;
			case 33: s = "\",\" expected"; break;
			case 34: s = "\"i\" expected"; break;
			case 35: s = "\"d\" expected"; break;
			case 36: s = "\"t\" expected"; break;
			case 37: s = "\"c\" expected"; break;
			case 38: s = "\"n\" expected"; break;
			case 39: s = "\"p\" expected"; break;
			case 40: s = "\"(\" expected"; break;
			case 41: s = "\")\" expected"; break;
			case 42: s = "\"->\" expected"; break;
			case 43: s = "\"=>\" expected"; break;
			case 44: s = "\"=\" expected"; break;
			case 45: s = "??? expected"; break;
			case 46: s = "invalid class_command"; break;
			case 47: s = "invalid section"; break;
			case 48: s = "invalid data_command"; break;
			case 49: s = "invalid class_data_command"; break;
			case 50: s = "invalid data_type"; break;
			case 51: s = "invalid buildintype"; break;
			case 52: s = "invalid command_block"; break;
			case 53: s = "invalid write_command"; break;
			case 54: s = "invalid fixlentype"; break;
			case 55: s = "invalid varlentype"; break;
			case 56: s = "invalid value"; break;
			case 57: s = "invalid literal"; break;
			case 58: s = "invalid expression"; break;
			case 59: s = "invalid addition"; break;
			case 60: s = "invalid factor"; break;
			case 61: s = "invalid multiply"; break;

			default: s = "error " + n; break;
		}
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}

	public virtual void SemErr (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}
	
	public virtual void SemErr (string s) {
		errorStream.WriteLine(s);
		count++;
	}
	
	public virtual void Warning (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
	}
	
	public virtual void Warning(string s) {
		errorStream.WriteLine(s);
	}
} // Errors


public class FatalError: Exception {
	public FatalError(string m): base(m) {}
}
}