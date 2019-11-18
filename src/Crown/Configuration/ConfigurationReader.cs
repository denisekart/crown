using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using Newtonsoft.Json.Serialization;

namespace Crown.Configuration
{
    public class ConfigurationReader
    {
        public ConfigurationReader()
        {
            
        }

        public ConfigurationReader(string solutionRoot)
        {
            _configurationLayers.Insert(0,()=>GetConfigurationFromPath(Path.Combine(solutionRoot,".crown")));            
        }
        private CrownConfiguration _configuration;

        public CrownConfiguration Configuration =>
            _configuration ?? (_configuration = GetFirstNonNullLayer());

        private CrownConfiguration GetFirstNonNullLayer()
        {
            foreach (var configurationLayer in _configurationLayers)
            {
                var layerInstance = configurationLayer();
                if (layerInstance != null)
                    return layerInstance;
            }
            throw new ArgumentOutOfRangeException(nameof(_configurationLayers));
        }

        private static readonly ConcurrentDictionary<string, ConfigurationReader> _instances=new ConcurrentDictionary<string, ConfigurationReader>();

        public static ConfigurationReader InstanceFor(string solutionRoot)
        {
            if (string.IsNullOrWhiteSpace(solutionRoot))
            {
                return Instance;
            }

            if (_instances.ContainsKey(solutionRoot))
            {
                if (_instances.TryGetValue(solutionRoot, out var value))
                    return value;
            }
            var newInstance = new ConfigurationReader(solutionRoot);
            _instances.TryAdd(solutionRoot, newInstance);

            return newInstance;
        }

        private static readonly Lazy<ConfigurationReader> _instance = new Lazy<ConfigurationReader>(()=>new ConfigurationReader(),System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);
        public static ConfigurationReader Instance => _instance.Value;

        private readonly List<Func<CrownConfiguration>> _configurationLayers = new List<Func<CrownConfiguration>>
        {
            () => GetConfigurationFromPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".crown")),
            GetDefaultConfiguration
        };

        private static CrownConfiguration GetConfigurationFromPath(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    return JsonConvert.DeserializeObject<CrownConfiguration>(File.ReadAllText(path), new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    });
                }
                catch
                {
                    // ignored
                }
            }

            return null;
        }
        private static CrownConfiguration GetDefaultConfiguration()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Crown.Configuration.defaultConfiguration.json";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();

                return JsonConvert.DeserializeObject<CrownConfiguration>(result, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
            }
        }
    }

    public class CrownConfiguration
    {
        public string ActiveProfile { get; set; }
        public List<CrownDiagnostic> Diagnostics { get; set; } = new List<CrownDiagnostic>();
        public Dictionary<string, CrownProfile> Profiles { get; set; } = new Dictionary<string, CrownProfile>();

        public Dictionary<string, string> MessageFormats { get; set; } = new Dictionary<string, string>();

        public string GetMessageFormat(string value)
        {
            var key = MessageFormats.Keys.FirstOrDefault(x => x.ToLower() == value.ToLower());
            if (key == null)
                return "{0}";
            return MessageFormats[key];
        }
        public CrownProfile CurrentProfile()
        {
            return Profiles.ContainsKey(ActiveProfile)
                ? Profiles[ActiveProfile]
                : Profiles[Profiles.Keys.First(x => x.Equals("default", StringComparison.OrdinalIgnoreCase))];
        }
    }

    public class CrownProfile
    {
        public List<string> EnabledDiagnostics { get; set; }
        public List<string> DisabledDiagnostics { get; set; }

        public bool IsDiagnosticEnabled(string diagnosticId)
        {
            if (EnabledDiagnostics != null && EnabledDiagnostics.Any())
            {
                //dismissive mode
                if (EnabledDiagnostics.Any(x => x == diagnosticId))
                {
                    if (DisabledDiagnostics != null && DisabledDiagnostics.Any(x => x == diagnosticId))
                        return false;
                    return true;
                }

                return false;
            }

            if (DisabledDiagnostics != null && DisabledDiagnostics.Any())
            {
                //permissive mode
                if (DisabledDiagnostics.Any(x => x == diagnosticId))
                    return false;
                return true;
            }

            return true;
        }
    }

    public class CrownCodeFix
    {
        public string DiagnosticIdReference { get; set; }
        public string Title { get; set; }
        public string MessageFormat { get; set; }
    }

    public class CrownDiagnostic
    {
        public string Id { get; set; }
        public string Category { get; set; }
        public string Title { get; set; }
        public string MessageFormat { get; set; }
        public CrownSeverity Severity { get; set; }
        public CrownCodeFix CodeFix { get; set; }
    }

    public enum CrownSeverity
    {
        Info,
        Warning,
        Error,
        None
    }
}
