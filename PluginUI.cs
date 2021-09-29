using Dalamud.Game.ClientState;
using Dalamud.Game.Internal.Gui;
using Dalamud.Game.Text;
using Dalamud.Plugin;
using ImGuiNET;
using System.Numerics;
using System.Text;

namespace Endcounter
{
    public class PluginUI
    {
        public bool SettingsVisible
        {
            get { return this.settingsVisible; }
            set { this.settingsVisible = value; }
        }
        public bool ReminderVisible
        {
            get { return this.reminderVisible; }
            set { this.reminderVisible = value; }
        }

        public DalamudPluginInterface pi;
        public Configuration config;
        private ChatGui chat;

        private bool Hide;
        private bool Enabled;
        private bool EndStartOfCombat;
        private bool EndEndOfCombat;

        private bool settingsVisible = false;
        private bool reminderVisible = false;
        private bool InCombatNow = false;
        private bool InCombatThen = false;

        private bool InDutyNow = false;
        private bool InDutyThen = false;

        private readonly XivChatEntry EndACTChatEntry = new XivChatEntry()
        {
            MessageBytes = Encoding.ASCII.GetBytes("end"),
            Type = XivChatType.Echo
        };

        private readonly XivChatEntry ClearACTChatEntry = new XivChatEntry()
        {
            MessageBytes = Encoding.ASCII.GetBytes("clear"),
            Type = XivChatType.Echo
        };

        public PluginUI(DalamudPluginInterface pi, Configuration config)
        {
            this.pi = pi;
            this.config = config;
            this.Enabled = config.Enabled;
            this.Hide = config.ReminderHide;
            this.EndEndOfCombat = config.EndEndOfCombat;
            this.EndStartOfCombat = config.EndStartOfCombat;
            this.chat = pi.Framework.Gui.Chat;
        }

        public void Draw()
        {
            if (SettingsVisible)
            {
                ImGui.SetNextWindowSizeConstraints(new Vector2(340, 140), new Vector2(340, 140));
                if (ImGui.Begin("Endcounter Settings", ref this.settingsVisible, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoResize))
                {
                    if (ImGui.Checkbox("Enabled", ref this.Enabled))
                    {
                        this.config.Enabled = this.Enabled;
                    }
                    ImGui.SameLine();
                    if (ImGui.Checkbox("Hide Duty reset reminder", ref this.Hide))
                    {
                        this.config.ReminderHide = this.Hide;
                    }

                    if (ImGui.Checkbox("Invoke 'end' at the end of combat", ref this.EndEndOfCombat))
                    {
                        this.config.EndEndOfCombat = this.EndEndOfCombat;
                    }
                    if (ImGui.Checkbox("Invoke 'end' at the start of combat", ref this.EndStartOfCombat))
                    {
                        this.config.EndStartOfCombat = this.EndStartOfCombat;
                    }
                }
            }

            if (ReminderVisible)
            {
                ImGui.SetNextWindowSizeConstraints(new Vector2(440, 90), new Vector2(440, 90));
                if (ImGui.Begin("Endcounter Duty", ref this.reminderVisible, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
                {
                    ImGui.Text("You have entered a duty, do you wish to clear all encounter data from ACT?");
                    if (ImGui.Button("Yes"))
                    {
                        this.chat.PrintChat(ClearACTChatEntry);
                    }
                    ImGui.SameLine();
                    if (ImGui.Button("No"))
                    {
                        this.ReminderVisible = false;
                    }
                    ImGui.SameLine();
                    if (ImGui.Checkbox("Don't show this again", ref this.Hide))
                    {
                        this.config.ReminderHide = this.Hide;
                    }

                }
                ImGui.End();
            }

            if (this.config.Enabled)
            {
                this.InCombatThen = this.InCombatNow;
                this.InCombatNow = this.pi.ClientState.Condition[ConditionFlag.InCombat];

                this.InDutyThen = this.InDutyNow;
                this.InDutyNow = this.pi.ClientState.Condition[ConditionFlag.BoundByDuty56];

                if (this.InDutyNow && !this.InDutyThen && !this.Hide)
                {
                    this.ReminderVisible = true;
                }
                
                if (!this.InCombatThen && this.InCombatNow && this.config.EndStartOfCombat)
                {
                    this.chat.PrintChat(EndACTChatEntry);
                }

                if (this.InCombatThen && !this.InCombatNow && this.config.EndEndOfCombat)
                {
                    this.chat.PrintChat(EndACTChatEntry);
                }
            }
        }
    }
}
