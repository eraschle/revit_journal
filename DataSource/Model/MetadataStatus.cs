namespace DataSource.Model
{
    public enum MetadataStatus { Initial, Valid, Repairable, Error }

    public static class Metadata
    {
        private const string Initial = "Initial";
        private const string Valid = "Valid";
        private const string Repairable = "Repairable";
        private const string Error = "Error";
        private const string Unknown = "Unknown";

        public static string GetStatusName(MetadataStatus metadata)
        {
            switch (metadata)
            {
                case MetadataStatus.Initial:
                    return Initial;
                case MetadataStatus.Valid:
                    return Valid;
                case MetadataStatus.Repairable:
                    return Repairable;
                case MetadataStatus.Error:
                    return Error;
                default:
                    return Unknown;
            }
        }
    }
}
