using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SonarQube.MSBuild.Tasks.IntegrationTests
{
    public class BuildProperty
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string Value { get; set; }
    }

    public class BuildLog
    {
        public BuildLog()
        {
            this.Targets = new List<string>();
            this.Tasks = new List<string>();
            this.Warnings = new List<string>();
            this.Errors = new List<string>();
            this.BuildProperties = new List<BuildProperty>();
        }

        public List<BuildProperty> BuildProperties { get; set; }

        public List<string> Targets { get; set; }

        public List<string> Tasks { get; set; }

        public List<string> Warnings { get; set; }

        public List<string> Errors { get; set; }

        public bool BuildSucceeded { get; set; }

        [XmlIgnore]
        public string FilePath { get; private set; }

        public void Save(string filePath)
        {
            SerializeObjectToFile(filePath, this);
            this.FilePath = filePath;
        }

        public static BuildLog Load(string filePath)
        {
            BuildLog log = null;

            using (var streamReader = new StreamReader(filePath))
            using (var reader = XmlReader.Create(streamReader))
            {
                var serializer = new XmlSerializer(typeof(BuildLog));
                log = (BuildLog)serializer.Deserialize(reader);
            }
            log.FilePath = filePath;

            return log;
        }

        private static void SerializeObjectToFile(string filePath, object objectToSerialize)
        {
            var settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = Encoding.UTF8,
                IndentChars = "  "
            };

            using (var stream = new MemoryStream())
            using (var writer = XmlWriter.Create(stream, settings))
            {
                var serializer = new XmlSerializer(objectToSerialize.GetType());
                serializer.Serialize(writer, objectToSerialize, new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty }));
                var xml = Encoding.UTF8.GetString(stream.ToArray());
                File.WriteAllText(filePath, xml);
            }
        }
    }
}
