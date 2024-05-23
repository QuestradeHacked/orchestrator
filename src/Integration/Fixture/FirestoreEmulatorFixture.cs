using Google.Cloud.Firestore;
using Integration.Config;

namespace Integration.Fixture;

public class FirestoreEmulatorFixture
{
    private readonly AppSettings _appSettings = new();

    private string _endpoint { get; }

    private string _projectId { get; }

    public FirestoreEmulatorFixture()
    {
        _endpoint = $"{_appSettings.GetFirestoreHost()}:{_appSettings.GetFirestorePort()}";
        _projectId = _appSettings.GetFirestoreProjectId();

        Environment.SetEnvironmentVariable("FIRESTORE_EMULATOR_HOST", _endpoint);
        Environment.SetEnvironmentVariable("FIRESTORE_PROJECT_ID",_projectId);

    }

    public FirestoreDb CreateFirestoreDb(){
        var builder = new FirestoreDbBuilder
        {
            ProjectId = _projectId,
            EmulatorDetection = Google.Api.Gax.EmulatorDetection.EmulatorOnly
        };

        return builder.Build();
    }
}
