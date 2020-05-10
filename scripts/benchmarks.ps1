$ErrorActionPreference = "Stop"

dotnet build .\src\ILLightenComparer.Benchmarks\ILLightenComparer.Benchmarks.csproj -c release -o bin\publish

bin\publish\ILLightenComparer.Benchmarks.exe
