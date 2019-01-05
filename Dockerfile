FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

COPY Unpacker/*.csproj ./
RUN dotnet restore

COPY Unpacker/*.* ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:2.1-runtime
WORKDIR /app

RUN apt update -y && apt install -y unrar-free p7zip-full

COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Unpacker.dll"]
