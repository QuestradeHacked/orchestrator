using Orchestrator.Config;
using Orchestrator.Extensions;

var builder = WebApplication.CreateBuilder(args);

var orchestratorConfiguration = new OrchestratorConfiguration();
builder.Configuration.Bind("Orchestrator", orchestratorConfiguration);
orchestratorConfiguration.Validate();

builder.RegisterServices(orchestratorConfiguration);

var app = builder.Build()
    .Configure();

app.Run();
