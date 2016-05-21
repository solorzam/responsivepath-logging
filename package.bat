msbuild ResponsivePath.SqlLogDatabase\ResponsivePath.SqlLogDatabase.sqlproj /p:Configuration=Release
msbuild ResponsivePath.Logging\ResponsivePath.Logging.csproj /p:Configuration=Release
.nuget\NuGet pack ResponsivePath.Logging\ResponsivePath.Logging.csproj -Prop Configuration=Release
