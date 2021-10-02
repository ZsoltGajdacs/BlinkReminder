using System;

namespace BRCore.Utils
{
    /// <summary>
    /// Keeps the path strings for all serialized files
    /// </summary>
    internal static class FilesLocation
    {
        /// <summary>
        /// Gives back the location of the dir where all the files are saved
        /// </summary>
        /// <returns></returns>
        internal static string GetSaveDirPath()
        {
            string userAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return userAppData + "\\BlinkReminder";
        }

        /// <summary>
        /// Gives back the name of the save file
        /// </summary>
        /// <returns></returns>
        internal static string GetSaveFileName()
        {
            return "settings.json";
        }

        /// <summary>
        /// Gives back the full path to the save file
        /// </summary>
        /// <returns></returns>
        internal static string GetSavePath()
        {
            string dirName = GetSaveDirPath();
            if (!dirName.EndsWith("\\", System.StringComparison.Ordinal))
            {
                dirName += "\\";
            }

            return dirName + GetSaveFileName();
        }
    }
}
