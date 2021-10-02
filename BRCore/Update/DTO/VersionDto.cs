namespace BRCore.Update.DTO
{
    public sealed class VersionDto
    {
        public int MajorVersion { get; private set; }
        public int MinorVersion { get; private set; }
        public int RevisionVersion { get; private set; }
        public string VersionText { get; private set; }

        public VersionDto(int majorVersion, int minorVersion, int revisionVersion)
        {
            MajorVersion = majorVersion;
            MinorVersion = minorVersion;
            RevisionVersion = revisionVersion;

            VersionText = string.Format("%s.%s.%s", majorVersion, minorVersion, revisionVersion);
        }
    }
}
