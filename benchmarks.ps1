dotnet build .\src\ILLightenComparer.Benchmarks\ILLightenComparer.Benchmarks.csproj -c release -o bin\publish

cd .\src\ILLightenComparer.Benchmarks\bin\publish\

dotnet ILLightenComparer.Benchmarks.dll

cd $PSScriptRoot
