using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using NContrib.Extensions;

namespace NContrib {

    public class IniConfigurationManager {

        public static int DefaultBufferSize = 2048;

        /// <summary>
        /// Default source file path for reading/writing when no other is specified.
        /// If this is left blank, the default path is the executing assembly name + .ini
        /// </summary>
        public static string DefaultSourceFilePath;

        private int _bufferSize;

        public int BufferSize {
            get { return _bufferSize == default(int) ? DefaultBufferSize : _bufferSize; }
            set { _bufferSize = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lpAppName">The name of the section containing the key name. If this parameter is NULL, the GetPrivateProfileString function copies all section names in the file to the supplied buffer.</param>
        /// <param name="lpKeyName">The name of the key whose associated string is to be retrieved. If this parameter is NULL, all key names in the section specified by the lpAppName parameter are copied to the buffer specified by the lpReturnedString parameter.</param>
        /// <param name="lpDefault">
        ///     A default string. If the lpKeyName key cannot be found in the initialization file, GetPrivateProfileString copies the default string to the lpReturnedString buffer. If this parameter is NULL, the default is an empty string, "".
        ///     Avoid specifying a default string with trailing blank characters. The function inserts a null character in the lpReturnedString buffer to strip any trailing blanks.
        /// </param>
        /// <param name="lpReturnedString">A pointer to the buffer that receives the retrieved string. </param>
        /// <param name="nSize">The size of the buffer pointed to by the lpReturnedString parameter, in characters.</param>
        /// <param name="lpFileName">The name of the initialization file. If this parameter does not contain a full path to the file, the system searches for the file in the Windows directory.</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetPrivateProfileStringW")]
        private static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName,
           string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "WritePrivateProfileStringW")]
        private static extern long WritePrivateProfileString(string section,
            string key, string val, string filePath);

        public string SourceFilePath { get; protected set; }

        /// <summary>
        /// Creates a new manager using the <see cref="DefaultSourceFilePath"/> if populated, otherwise the calling assembly name + .ini
        /// </summary>
        public IniConfigurationManager() {
            SourceFilePath = DefaultSourceFilePath ?? Path.GetFullPath(System.Reflection.Assembly.GetCallingAssembly().GetName().Name + ".ini");
            BufferSize = DefaultBufferSize;
        }

        /// <summary>
        /// Creates a new manager using the specified ini file path
        /// </summary>
        /// <param name="path"></param>
        public IniConfigurationManager(string path) : this(path, DefaultBufferSize) { }

        /// <summary>
        /// Creates a new manager using the specified ini path and the given buffer size.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="bufferSize"></param>
        public IniConfigurationManager(string path, int bufferSize) {
            SourceFilePath = Path.GetFullPath(path);
            BufferSize = bufferSize;
        }

        public string ReadValue(string section, string key) {
            return ReadValue<string>(section, key);
        }

        public string ReadValue(string section, string key, string defaultValue) {
            return ReadValue<string>(section, key, defaultValue);
        }

        public string ReadValue(string section, string key, string defaultValue, int bufferSize) {
            return ReadValue<string>(section, key, defaultValue, bufferSize);
        }

        public T ReadValue<T>(string section, string key) {
            return ReadValue<T>(section, key, null);
        }

        public T ReadValue<T>(string section, string key, string defaultValue) {
            return ReadValue<T>(section, key, defaultValue, BufferSize);
        }

        /// <summary>
        /// Reads a value if available. If not available, the <paramref name="defaultValue"/> is returned. Type converted to <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <param name="bufferSize"></param>
        /// <returns></returns>
        public T ReadValue<T>(string section, string key, string defaultValue, int bufferSize) {
            if (!File.Exists(SourceFilePath))
                throw new FileNotFoundException("Could not find INI file '" + SourceFilePath + "'", SourceFilePath);

            Trace.WriteLine("Reading key '" + key + "' from section '" + section + "' from file '" + SourceFilePath + "'");

            var sb = new StringBuilder(bufferSize);
            GetPrivateProfileString(section, key, defaultValue, sb, bufferSize, SourceFilePath);
            return sb.ToString().ConvertTo<T>();
        }

        public void WriteValue(string section, string key, string value) {
            WritePrivateProfileString(section, key, value, SourceFilePath);
        }
    }
}
