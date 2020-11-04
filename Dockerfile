FROM mcr.microsoft.com/dotnet/core/sdk:2.2.105 AS build

RUN curl -sL https://deb.nodesource.com/setup_10.x |  bash -
RUN apt-get install -y nodejs build-essential

WORKDIR /source

COPY *.sln .
COPY CuotasApp/*.csproj ./CuotasApp/
COPY LDAP_Utils/*.csproj ./LDAP_Utils/
RUN dotnet restore

COPY . .
WORKDIR /source
RUN dotnet publish -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2.3
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "CuotasApp.dll"]
