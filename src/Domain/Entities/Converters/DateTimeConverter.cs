using Google.Cloud.Firestore;

namespace Domain.Entities.Converters;

public class DateTimeConverter : IFirestoreConverter<DateTime>
{
    public DateTime FromFirestore(object value) => ((Timestamp)value).ToDateTime();

    public object ToFirestore(DateTime value) => DateTime.SpecifyKind(value, DateTimeKind.Utc);
}
