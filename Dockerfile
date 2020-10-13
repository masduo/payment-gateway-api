# BUILD
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as publish

WORKDIR /work
COPY ./src ./src
COPY ./stylecop ./stylecop

RUN dotnet publish -c Release -o /compiled-app /nologo ./src

# RUN
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

ENV ASPNETCORE_URLS http://+:5001
WORKDIR /app
EXPOSE 5001
COPY --from=publish ./compiled-app .

ENTRYPOINT dotnet PaymentGateway.Api.dll