using Newtonsoft.Json;
using System;
using System.IO;

namespace BlinkReminder.Helpers.FileHandlers
{
    internal static class Serializer
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        internal static bool JsonObjectSerialize<T>(string saveDir, string filePath, 
            ref T serializable, DoBackup doBackup)
        {
            CreateDirIfDoesntExist(saveDir);
            CreateBackupIfNeeded(filePath, doBackup);

            TextWriter writer = null;
            bool isSaveSuccessful = false;
            try
            {
                string output = JsonConvert.SerializeObject(serializable);
                writer = new StreamWriter(filePath, false);
                writer.Write(output);

                isSaveSuccessful = true;
            }
            catch(Exception ex)
            {
                logger.Error(ex, "Serialization failed!");
            }
            finally
            {
                if (writer != null) writer.Close();
            }

            return isSaveSuccessful;
        }

        internal static T JsonObjectDeserialize<T>(string filePath)
        {
            T deserializedObj = default;
            if (new FileInfo(filePath).Exists)
            {
                TextReader reader = null;
                try
                {
                    reader = new StreamReader(filePath);
                    string fileContents = reader.ReadToEnd();

                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };

                    deserializedObj = JsonConvert.DeserializeObject<T>(fileContents, settings);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Deserialization failed!");
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                }
            }

            return deserializedObj;
        }

        private static void CreateDirIfDoesntExist(string dirName)
        {
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }
        }

        private static void CreateBackupIfNeeded(string path, DoBackup doBackup)
        {
            if (DoBackup.Yes == doBackup && File.Exists(path))
            {
                string backupPath = path + ".bak";
                if (File.Exists(backupPath))
                {
                    File.Delete(backupPath);
                }

                File.Copy(path, backupPath);
            }
        }
    }

    internal enum DoBackup
    {
        Yes,
        No
    }
}
