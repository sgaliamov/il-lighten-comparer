$ErrorActionPreference = "Stop"

dotnet build .\src\ILLightenComparer.Benchmarks\ILLightenComparer.Benchmarks.csproj -c release -o bin\publish

try {
    cd .\src\ILLightenComparer.Benchmarks\bin\publish\
    dotnet ILLightenComparer.Benchmarks.dll
}
finally {
    cd $PSScriptRoot
}
