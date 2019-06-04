using System;
using System.Collections.Generic;
using System.Linq;

namespace Dvinun.DesignPatterns.Behavioral
{
    partial class Interpreter
    {
        abstract class IOTDevice
        {
            protected List<Appliance> appliances;
            public abstract void NewCommand(string command);

            public IOTDevice()
            {
                appliances = new List<Appliance>();
            }

            public void Connect(Appliance appliance)
            {
                appliances.Add(appliance);
            }

            public Appliance[] GetApplianceNames()
            {
                return appliances.ToArray();
            }
        }

        class VinnysHomeAutomation : IOTDevice
        {
            override public void NewCommand(string command)
            {
                if (string.IsNullOrEmpty(command)) return;

                Console.WriteLine("");
                Console.WriteLine("========== VOICE COMMAND START ===========");
                Console.WriteLine("Received new voice command {0}", command);
                Instruction instruction = new Instruction(command.Trim(), GetApplianceNames());
                MainExpression expression = new MainExpression();
                bool matchFound = expression.Interpret(instruction);
                Console.WriteLine(this.GetType().Name + (matchFound ? "-Match found." : "-Match not found."), command);

                if (matchFound)
                {
                    var applianceResult = appliances.Single(item =>
                                            item.GetApplianceNickName().ToLower() == instruction.GetApplianceNickName().ToLower());

                    Appliance appliance = applianceResult as Appliance;
                    appliance.Perform(instruction.GetAction(), instruction.GetActionParams());
                }
                Console.WriteLine("========== VOICE COMMAND END ===========");
            }
        }
    }
}