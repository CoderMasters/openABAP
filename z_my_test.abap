class CL_HA_TESTFRAME_ABAP definition
  public
*"*  inheriting from CL_HA_TEST
  create public .

*"* public components of class CL_HA_TESTFRAME_ENQ
*"* do not include other source files here!!!
public section.

  class-methods CLASS_CONSTRUCTOR .

  methods INIT_TEST_EXEC
    redefinition .
  methods STEP_END
    redefinition .
  methods STEP_START
    redefinition .
  methods VERIFY_TEST_EXEC
    redefinition .
*"* ??? methods INIT_SUBTEST_EXEC???
    redefinition .
  methods RUN
    redefinition .
protected section.
*"* protected components of class CL_HA_TESTFRAME_ABAP
*"* do not include other source files here!!!
private section.
*"* private components of class CL_HA_TESTFRAME_ABAP
*"* do not include other source files here!!!





ENDCLASS.



CLASS CL_HA_TESTFRAME_ENQ IMPLEMENTATION.

*"*method CLASS_CONSTRUCTOR.
 *"* sv_cl_name = 'CL_HA_TESTFRAME_ABAP'.
*"*  sv_cl_short_text = 'Test frame for abap load tests'.
 *"* sv_cl_step_max = -1.
 *"* sv_cl_step_min = 2.
*"*  sv_cl_parallel_use = '1'.
*"*  sv_cl_reset_needed = '0'.

*"*????????
*"*????????
*"*????????

*"*endmethod.

method RUN.
DO 1000 TIMES.
Write:/'Hello, World!'.
ENDDO.
endmethod.


ENDCLASS.
