using FeatureSettingsTool;

namespace FeatureSettingsToolTests
{
    public class GenerateFeatureSettingsTest
    {
        [Fact]
        public async Task Test1()
        {
            var feature = new GenerateFeatureSettings();

            await feature.GenerateJsonFiles();
        }
    }
}