using Dalamud.Configuration;
using Dalamud.Plugin;
using Newtonsoft.Json;

namespace Endcounter
{
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; }
        public bool Enabled { get; set; } = true;
        public bool EndStartOfCombat { get; set; } = false;
        public bool EndEndOfCombat { get; set; } = true;

        public bool ReminderHide { get; set; } = false;

        // Add any other properties or methods here.
        [JsonIgnore] private DalamudPluginInterface pluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
        }

        public void Save()
        {
            this.pluginInterface.SavePluginConfig(this);
        }
    }
}
