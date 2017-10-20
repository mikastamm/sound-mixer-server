using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SoundMixerServer
{
    class XMLManager
    {
        public static string serialize<T>(T toSerialize)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(T));

                using (StringWriter textWriter = new StringWriter())
                {
                    serializer.Serialize(textWriter, toSerialize);
                    return textWriter.ToString();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("An Exception occured while trying to serialize Object to XML" + ex.Message);
                return null;
            }
        }

        public static T deserialize<T>(string toDeserialize) where T : new()
        {
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                StringReader reader = new StringReader(toDeserialize);
                return (T)serializer.Deserialize(reader);
            }
            catch(Exception ex)
            {
                Console.WriteLine("An Exception occured while trying to deserialize Object from XML"+ ex.Message);
                return default(T);
            }
        }

        public static bool WriteToXmlFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
        {
            TextWriter writer = null;
            bool success;
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                var serializer = new XmlSerializer(typeof(T));
                writer = new StreamWriter(filePath, append);
                serializer.Serialize(writer, objectToWrite);
                success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An Exception occured while trying to Write to XML FIle: " + ex.Message);
                success = false;
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }

            return success;
        }

        public static T ReadFromXmlFile<T>(string filePath) where T : new()
        {
            try
            {
                TextReader reader = null;

                var serializer = new XmlSerializer(typeof(T));
                reader = new StreamReader(filePath);
                T a = (T)serializer.Deserialize(reader);

                if (reader != null)
                    reader.Close();

                return a;
            }
            catch(Exception ex)
            {
                if (ex is FileNotFoundException || ex is DirectoryNotFoundException)
                {
                    Console.WriteLine("An Exception occured while trying to read from XML File: " + ex.Message);
                    return default(T);
                }

                throw;
            }
        }
    }
}
