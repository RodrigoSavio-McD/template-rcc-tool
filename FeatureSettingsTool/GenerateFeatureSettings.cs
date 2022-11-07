using FeatureSettingsLib.Models;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using System.Reflection;

namespace FeatureSettingsTool
{
    public class GenerateFeatureSettings
    {
        public async Task GenerateJsonFiles()
        {
            var currentPath = Directory.GetParent(Environment.CurrentDirectory)?.FullName;
            var typeInherited = typeof(GenerateFeatureSettings);
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;

            var dllFiles = Directory.GetFiles(baseDir)
                .Where(name => name.EndsWith(".dll"))
                .ToList();

            var featureSettingsClasses = dllFiles.Select(Assembly.LoadFile)
                .SelectMany(type => type.GetTypes())
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeInherited));

            foreach (var featureSettingsClass in featureSettingsClasses)
            {
                var generator = new JSchemaGenerator();
                generator.GenerationProviders.Add(new StringEnumGenerationProvider());
                var schema = generator.Generate(featureSettingsClass);

                var attribute = (FeatureAttribute?)featureSettingsClass.GetCustomAttribute(typeof(FeatureAttribute));

                var featureSchema = new JSchema
                {
                    ExtensionData =
                    {
                        {
                            attribute!.Name, new JSchema
                            {
                                ExtensionData =
                                {
                                    {
                                        attribute.SubdomainName, schema
                                    }
                                }
                            }
                        }
                    }
                };

                var schemaFilePath = string.Concat(currentPath, "\\", featureSettingsClass.Name, "-featureSettings.json");
                await File.WriteAllTextAsync(schemaFilePath, featureSchema.ToString());
            }
        }
    }
}
