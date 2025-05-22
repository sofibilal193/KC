#create Sln

# Create the Web API project

dotnet new webapi -n MyApp.Api

# Create Class Library

dotnet new classlib -n MyApp.Core

dotnet sln add MyApp.Api/MyApp.Api.csproj
dotnet sln add MyApp.Core/MyApp.Core.csproj
dotnet sln add MyApp.Infrastructure/MyApp.Infrastructure.csproj

# csProj to CsProej

dotnet add MyApp.Infrastructure/MyApp.Infrastructure.csproj reference MyApp.Core/MyApp.Core.csproj

# API depends on Core and Infrastructure

dotnet add MyApp.Api/MyApp.Api.csproj reference MyApp.Core/MyApp.Core.csproj
dotnet add MyApp.Api/MyApp.Api.csproj reference MyApp.Infrastructure/MyApp.Infrastructure.csproj
