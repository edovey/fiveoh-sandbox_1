@if "%1"=="" goto error
@echo generating a resources file:
resgen textcontrol.resx TXTextControl.TextControlCore.%1.resources
@echo generating a satellite assembly:
al /out:txtextcontrol.resources.dll /v:16.0.900.500 /keyf:txtextcontrol.public.snk /c:%1 /delay+ /embed:TXTextControl.TextControlCore.%1.resources
@echo turning off verification for the assembly:
sn -Vr txtextcontrol.resources.dll
@echo.
@echo satellite assembly successfully generated.
@goto end
:error
@echo error: language tag missing
:end