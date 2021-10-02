using BRCore.Update.DTO;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace BRCore.Update
{
    public class UpdateRunner
    {
        public VersionDto CurrentVersion { get; private set; }

        public UpdateRunner()
        {
            int majVer = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileMajorPart;
            int minVer = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileMinorPart;
            int revVer = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileBuildPart;
            CurrentVersion = new VersionDto(majVer, minVer, revVer);
        }

        public async Task RunUpdate(string updateLinkUrl)
        {
            UpdateHandler updater = new UpdateHandler();
            bool isOkToLaunch = await updater.DownloadUpdate(updateLinkUrl);

            if (isOkToLaunch)
            {
                updater.RunUpdate();
            }
        }

        public async Task<UpdateResultDto> CheckUpdate()
        {
            UpdateCheck checker = new UpdateCheck(CurrentVersion);
            return await checker.GetUpdateUrl();
        }
    }
}
