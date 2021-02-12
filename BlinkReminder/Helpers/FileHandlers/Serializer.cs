using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace BlinkReminder.Helpers.FileHandlers
{
    internal static class Serializer
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Serializes the given object to the given path via Binary formatting
        /// </summary>
        internal static void SerializeObj(Object o, string serializePath, string parentDir)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(parentDir);

            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            try
            {
                IFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(serializePath, FileMode.Create, FileAccess.Write);

                formatter.Serialize(stream, o);
                stream.Close();

                logger.Info(o.ToString() + " serialization successful");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Serialization failed!");
            }
        }

        /// <summary>
        /// Deserializes the given object at path via Binary formatting
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static object DeserializeObject(string path)
        {
            if (File.Exists(path) && new FileInfo(path).Length > 0)
            {
                try
                {
                    IFormatter formatter = new BinaryFormatter();
                    FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);

                    return formatter.Deserialize(stream);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Deserialization failed!");
                }
            }

            return null;
        }
    }
}
