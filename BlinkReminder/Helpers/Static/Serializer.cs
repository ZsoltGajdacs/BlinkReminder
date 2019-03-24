using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace BlinkReminder.Helpers
{
    internal static class Serializer
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Serializes the given object to the given path
        /// </summary>
        /// <param name="o"></param>
        /// <param name="serializePath"></param>
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
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Serialization failed!");
            }
        }
    }
}
