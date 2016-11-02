copy nugetpackager\NuGet.exe %4
copy nugetpackager\Package.nuspec %4
md %4content\%2\
copy %1 %4content\%2\
%9nuget pack
echo y | del %4content\%2\*.*