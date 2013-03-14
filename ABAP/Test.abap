"--------------------------------------------------------------
"  PROGRAMM Hello
"--------------------------------------------------------------
"  Test verschiedener ABAP-Befehler
"--------------------------------------------------------------
PROGRAM hello.

"--------------------------------------------------------------
CLASS TestClass DEFINITION.
"--------------------------------------------------------------
  PUBLIC SECTION.
    
    METHODS Run.

    DATA zahl TYPE p.
    DATA text type c length 30.
    
ENDCLASS.   "TestClass

"--------------------------------------------------------------
CLASS TestClass IMPLEMENTATION.
"--------------------------------------------------------------

"--------------------------------------------------------------
"   Method RUN
"--------------------------------------------------------------
"   Diese Methode wird beim Programmstart aufgerufen
"--------------------------------------------------------------
  METHOD Run.
    WRITE 'Hello Bielefeld !'.
    WRITE / 'Dieses ABAP-Programm läuft in der der .NET runtime (bzw. Mono).'.
    zahl = 2 * ( 10 + 11 ).
    WRITE: / 'Die Antwort auf die Frage des Sinn des Lebens, des Seins und Alles ist: ', zahl.
    move 'abcdefghijklmnopqrstuvwxyz' to text.
    WRITE: / text.
  ENDMETHOD.
  
ENDCLASS.   "TestClass
