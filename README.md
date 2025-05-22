# create Sln

dotnet new sln -n MyApp

# Create the Web API project

dotnet new webapi -n MyApp.Api

# Create Class Library

dotnet new classlib -n MyApp.Core

# Add refernces of CsProj to sln

dotnet sln add MyApp.Api/MyApp.Api.csproj
dotnet sln add MyApp.Core/MyApp.Core.csproj
dotnet sln add MyApp.Infrastructure/MyApp.Infrastructure.csproj

# csProj to CsProej

dotnet add MyApp.Infrastructure/MyApp.Infrastructure.csproj reference MyApp.Core/MyApp.Core.csproj
