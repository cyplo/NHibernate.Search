@echo off
rem I wanted to put this in the nant build file, but I had very annoying problems with 64-bit java running from the 32-bit nant process.
pushd %~dp0
java.exe -cp ..\..\..\..\..\Tools\Antlr\antlr-3.2.jar org.antlr.Tool -o Generated HqlSqlWalker.g
popd