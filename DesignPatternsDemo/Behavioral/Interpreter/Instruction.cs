using System;

namespace Dvinun.DesignPatterns.Behavioral
{
    partial class Interpreter
    {
        // Context
        class Instruction
        {
            string applianceNickName;
            ApplianceType applianceType;
            ActionType actionType;
            ActionParams actionParams;
            string virtualAssistantName;
            string greetingInterjection;
            string command = string.Empty;
            string currentCommand = string.Empty;
            Appliance[] appliances = null;

            public Instruction(string command, Appliance[] appliances)
            {
                this.command = command.Trim();
                this.currentCommand = command.Trim();
                this.appliances = appliances;
            }

            public string GetCommand() { return command; }
            public string GetCurrentCommand() { return currentCommand; }
            public void SetCurrentCommand(string currentCommand) { this.currentCommand = currentCommand.Trim(); }

            internal Appliance[] GetAppliances()
            {
                return appliances;
            }

            internal string GetApplianceNickName()
            {
                return applianceNickName;
            }

            internal ApplianceType GetApplianceType()
            {
                return applianceType;
            }

            internal ActionType GetAction()
            {
                return actionType;
            }

            internal ActionParams GetActionParams()
            {
                return actionParams;
            }

            internal void SetVirtualAssistant(string assistant)
            {
                virtualAssistantName = assistant;
            }

            internal void SetApplianceName(string applianceNickName)
            {
                this.applianceNickName = applianceNickName;
            }

            internal void SetApplianceType(ApplianceType applianceType)
            {
                this.applianceType = applianceType;
            }

            internal void SetActionType(ActionType actionType)
            {
                this.actionType = actionType;
            }

            internal void SetActionParams(ActionParams actionParams)
            {
                this.actionParams = actionParams;
            }

            internal void SetGreetingInterjection(string interjection)
            {
                greetingInterjection = interjection;
            }
        }
    }
}