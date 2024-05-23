FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
EXPOSE 80
EXPOSE 443

# Copy project resources
COPY src src
COPY nuget.config ./
COPY .editorconfig ./
COPY Questrade.FinCrime.Orchestrator.sln Questrade.FinCrime.Orchestrator.sln
RUN dotnet restore --locked-mode ./src/Orchestrator/Orchestrator.csproj --configfile nuget.config

# Test steps
FROM build as test
RUN dotnet restore --locked-mode src/Unit/Unit.csproj --configfile nuget.config
RUN dotnet restore --locked-mode src/Integration/Integration.csproj --configfile nuget.config
ENTRYPOINT ["dotnet", "test" ]

# Publishing the application
FROM build AS publish
RUN dotnet publish src/Orchestrator/Orchestrator.csproj -c Release -o /app/Questrade.FinCrime.Orchestrator --no-restore

# Final image wrap-up
FROM gcr.io/qt-shared-services-3w/dotnet:6.0 as runtime
WORKDIR /app
COPY --from=publish /app/Questrade.FinCrime.Orchestrator .
USER dotnet
CMD [ "dotnet", "Orchestrator.dll" ]

