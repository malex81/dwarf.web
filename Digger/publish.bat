@set output=..\output\Digger

@rd /S /Q %output%
dotnet publish "Dwarf.Digger\Dwarf.Digger.csproj" --runtime win-x64 -c debug --self-contained true -o "%output%"