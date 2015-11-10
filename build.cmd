:: This script builds the projects located in this repository. Alternatively
:: the projects can be build from the Visual Studio IDE.
::
:: System Requirements:
::    + Visual Studio 2013
::    + Visual Studio 2013 Update 5
::    + Sandcastle Help File Builder from  https://github.com/EWSoftware/SHFB/releases
::        - Install SHFB
::        - Install Sandcastle Help File Builder Visual Studio Package
::        - Install Visual Studio 2013 schemas
::        - Install Visual Studio MAML snippets

@ECHO OFF
SETLOCAL ENABLEDELAYEDEXPANSION

:: IMPORTANT: SET THESE VALUES AS APPROPRIATE ON YOUR MACHINE
SET MSBUILD_EXE=C:\Program Files (x86)\MSBuild\12.0\Bin\MSBuild.exe
SET SHFBROOT=C:\Program Files (x86)\EWSoftware\Sandcastle Help File Builder
SET SOLUTION=Xliff.OM.sln

IF "%1" == "clean" (
  CALL :Clean
) ELSE (
  CALL :Build
)
GOTO :Exit


:Build
ECHO Building Debug^|Any CPU
"%MSBUILD_EXE%" /p:Configuration=Debug,Platform="Any CPU" %SOLUTION%

ECHO Building Release^|Any CPU
"%MSBUILD_EXE%" /p:Configuration=Release,Platform="Any CPU" %SOLUTION%

GOTO :Exit


:Clean
ECHO Cleaning Debug^|Any CPU
"%MSBUILD_EXE%" /t:Clean /p:Configuration=Debug,Platform="Any CPU" %SOLUTION%

ECHO Cleaning Release^|Any CPU
"%MSBUILD_EXE%" /t:Clean /p:Configuration=Release,Platform="Any CPU" %SOLUTION%

GOTO :Exit


:Exit
ENDLOCAL
