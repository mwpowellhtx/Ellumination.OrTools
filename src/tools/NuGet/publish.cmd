@echo off

setlocal

rem We do not publish the API key as part of the script itself.
if "%my_nuget_api_key%" equ "" (
    echo You are prohibited from making these sorts of changes.
    goto :end
)

rem Default list delimiter is Comma (,).
:redefine_delim
if "%delim%" equ "" (
    set delim=,
)
rem Redefine the delimiter when a Dot (.) is discovered.
rem Anticipates potentially accepting Delimiter as a command line arg.
if "%delim%" equ "." (
    set delim=
    goto :redefine_delim
)

rem Go ahead and pre-seed the Projects up front.
:set_projects
set projects=
rem Setup All Projects
set all_projects=Ellumination.OrTools.Core
set all_projects=%all_projects%%delim%Ellumination.OrTools.ConstraintSolver.Core
set all_projects=%all_projects%%delim%Ellumination.OrTools.LinearSolver.Core
set all_projects=%all_projects%%delim%Ellumination.OrTools.Sat.Core
set all_projects=%all_projects%%delim%Ellumination.OrTools.Sat.Parameters.Core
set all_projects=%all_projects%%delim%Ellumination.OrTools.Sat.Parameters
rem Setup Constraint Solver
set constraint_projects=Ellumination.OrTools.Core
set constraint_projects=%constraint_projects%%delim%Ellumination.OrTools.ConstraintSolver.Core
rem Setup Routing
set routing_projects=Ellumination.OrTools.Core
rem TODO: TBD: does it require a dependency on our ConstraintSolver core package?
rem set routing_projects=Ellumination.OrTools.ConstraintSolver.Core
set routing_projects=%routing_projects%%delim%Ellumination.OrTools.ConstraintSolver.Routing
rem Setup Linear Solver
set linear_projects=Ellumination.OrTools.Core
set linear_projects=%linear_projects%%delim%Ellumination.OrTools.LinearSolver.Core
rem Setup Sat Solver
set sat_projects=Ellumination.OrTools.Core
set sat_projects=%sat_projects%%delim%Ellumination.OrTools.Sat.Core
set sat_projects=%sat_projects%%delim%Ellumination.OrTools.Sat.Parameters.Core
set sat_projects=%sat_projects%%delim%Ellumination.OrTools.Sat.Parameters
rem Setup Sat Parameters
set sat_params_projects=Ellumination.OrTools.Sat.Parameters.Core
set sat_params_projects=%sat_params_projects%%delim%Ellumination.OrTools.Sat.Parameters
rem Setup Solver Projects
set solver_projects=Ellumination.OrTools.Core
set solver_projects=%solver_projects%%delim%Ellumination.OrTools.ConstraintSolver.Core
set solver_projects=%solver_projects%%delim%Ellumination.OrTools.LinearSolver.Core
set solver_projects=%solver_projects%%delim%Ellumination.OrTools.Sat.Core
rem We decided to let internally delivered packages rest in their respective build pipelines.

:parse_args

rem Done parsing the args.
if "%1" equ "" (
    goto :end_args
)

:set_dry_run
if "%1" equ "--dry" (
    set dry=true
    goto :next_arg
)
if "%1" equ "--dry-run" (
    set dry=true
    goto :next_arg
)

:set_config
if "%1" equ "--config" (
    set config=%2
    shift
    goto :next_arg
)

:set_publish_local
if "%1" equ "--local" (
    set function=local
    goto :next_arg
)

:set_publish_nuget
if "%1" equ "--nuget" (
    set function=nuget
    goto :next_arg
)

:add_routing_projects
if "%1" equ "--routing" (
    if "%projects%" equ "" (
        set projects=%routing_projects%
    ) else (
        set projects=%projects%%delim%%routing_projects%
    )
	goto :next_arg
)

:add_constraint_projects
if "%1" equ "--constraint-solver" (
    if "%projects%" equ "" (
        set projects=%constraint_projects%
    ) else (
        set projects=%projects%%delim%%constraint_projects%
    )
	goto :next_arg
)

if "%1" equ "--constraint" (
    if "%projects%" equ "" (
        set projects=%constraint_projects%
    ) else (
        set projects=%projects%%delim%%constraint_projects%
    )
	goto :next_arg
)

:add_linear_projects
if "%1" equ "--linear-solver" (
    if "%projects%" equ "" (
        set projects=%linear_projects%
    ) else (
        set projects=%projects%%delim%%linear_projects%
    )
	goto :next_arg
)

if "%1" equ "--linear" (
    if "%projects%" equ "" (
        set projects=%linear_projects%
    ) else (
        set projects=%projects%%delim%%linear_projects%
    )
	goto :next_arg
)

:add_solver_projects
if "%1" equ "--solvers" (
    if "%projects%" equ "" (
        set projects=%solver_projects%
    ) else (
        set projects=%projects%%delim%%solver_projects%
    )
	goto :next_arg
)

:add_sat_projects
if "%1" equ "--sat" (
    if "%projects%" equ "" (
        set projects=%sat_projects%
    ) else (
        set projects=%projects%%delim%%sat_projects%
    )
    goto :next_arg
)

:add_sat_params_projects
if "%1" equ "--sat-params" (
    if "%projects%" equ "" (
        set projects=%sat_params_projects%
    ) else (
        set projects=%projects%%delim%%sat_params_projects%
    )
    goto :next_arg
)

:add_all_projects
if "%1" equ "--all" (
    set projects=%all_projects%
    goto :next_arg
)

rem Add a single Project to the Projects list.
:add_project
if "%1" equ "--project" (
    if "%projects%" equ "" (
        set projects=%2
    ) else (
        set projects=%projects%%delim%%2
    )
    shift
    goto :next_arg
)

:next_arg

shift

goto :parse_args

:end_args

:verify_args

:verify_function
if "%function%" equ "" (
    set function=local
)

:verify_projects
if "%projects%" equ "" (
    rem In which case, there is nothing else to do.
    echo Nothing to process.
    goto :end
)

:verify_config
if "%config%" equ "" (
    rem Assumes Release Configuration when not otherwise specified.
    set config=Release
)

:publish_projects

:set_vars
set xcopy_exe=xcopy.exe
set xcopy_opts=/y /f
set xcopy_dest_dir=G:\Dev\NuGet\local\packages
rem Expecting NuGet to be found in the System Path.
set nuget_exe=NuGet.exe
set nuget_push_verbosity=detailed
set nuget_push_source=https://api.nuget.org/v3/index.json

set nuget_push_opts=%nuget_push_opts% %my_nuget_api_key%
set nuget_push_opts=%nuget_push_opts% -Verbosity %nuget_push_verbosity%
set nuget_push_opts=%nuget_push_opts% -NonInteractive
set nuget_push_opts=%nuget_push_opts% -Source %nuget_push_source%

rem Do the main areas here.
pushd ..\..

rem echo projects = %projects%

if not "%projects%" equ "" (
    echo Processing '%config%' configuration for '%projects%' ...
)
:next_project
if not "%projects%" equ "" (
    for /f "tokens=1* delims=%delim%" %%p in ("%projects%") do (
        if "%function%" equ "nuget" call :publish_nuget %%p
        if "%function%" equ "local" call :publish_local %%p
        set projects=%%q
        goto :next_project
    )
)

popd

goto :end

:publish_local
for %%f in ("%1\bin\%config%\%1.*.nupkg") do (
    if "%dry%" equ "true" (
        echo Dry run: %xcopy_exe% %%f %xcopy_dest_dir% %xcopy_opts%
    ) else (
        echo Running: %xcopy_exe% %%f %xcopy_dest_dir% %xcopy_opts%
        %xcopy_exe% %%f %xcopy_dest_dir% %xcopy_opts%
    )
)
exit /b

:publish_nuget
for %%f in ("%1\bin\%config%\%1.*.nupkg") do (
    if "%dry%" equ "true" (
        echo Dry run: %nuget_exe% push "%%f"%nuget_push_opts%
    ) else (
        echo Running: %nuget_exe% push "%%f"%nuget_push_opts%
        %nuget_exe% push "%%f"%nuget_push_opts%
    )
)
exit /b

:end

endlocal

pause
