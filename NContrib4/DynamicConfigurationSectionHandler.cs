using System.Configuration;
using System.Xml;

namespace NContrib4 {

    /// <summary>
    /// Serves as a configuration section handler for standard .NET configuraiton files
    /// Uses DynamicXElement to access configuration data
    /// </summary>
    public class DynamicConfigurationSectionHandler : IConfigurationSectionHandler {

        public object Create(object parent, object configContext, XmlNode section) {

            return new DynamicXElement(section);
        }
    }
}
