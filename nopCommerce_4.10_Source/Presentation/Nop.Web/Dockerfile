##FROM microsoft/aspnetcore:2.1-nanoserver-1709 AS base
#FROM microsoft/dotnet:sdk AS build
#WORKDIR /app
#EXPOSE 80
#
##FROM microsoft/aspnetcore-build:2.1-nanoserver-1709 AS build
#FROM microsoft/dotnet:sdk AS build
#WORKDIR /src
#COPY NopCommerce.sln ./
#COPY Presentation/Nop.Web/Nop.Web.csproj Presentation/Nop.Web/
#COPY Libraries/Nop.Services/Nop.Services.csproj Libraries/Nop.Services/
#COPY Libraries/Nop.Core/Nop.Core.csproj Libraries/Nop.Core/
#COPY Libraries/Nop.Data/Nop.Data.csproj Libraries/Nop.Data/
#COPY Presentation/Nop.Web.Framework/Nop.Web.Framework.csproj Presentation/Nop.Web.Framework/
#RUN dotnet restore -nowarn:msb3202,nu1503
#COPY . .
#WORKDIR /src/Presentation/Nop.Web
#RUN dotnet build -c Release -o /app
#
#FROM build AS publish
#RUN dotnet publish -c Release -o /app
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app .
#ENTRYPOINT ["dotnet", "Nop.Web.dll"]
#

FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "aspnetapp.dll"]