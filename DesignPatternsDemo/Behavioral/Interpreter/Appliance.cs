using System;

namespace Dvinun.DesignPatterns.Behavioral
{
    partial class Interpreter
    {
        enum ApplianceType
        {
            None,
            Curtain,
            AirConditioning,
            DogFeeder,
            Dishwasher,
            LightingSystem,
            Sprinkler,
            Blender,
            Alarm
        }

        abstract class Appliance : IAction
        {
            string nickName;

            public Appliance(string nickName)
            {
                this.nickName = nickName;
                Console.WriteLine(" Adding Appliance - {0}", nickName);
            }

            public abstract ApplianceType GetApplianceType();
            public string GetApplianceNickName() { return nickName; }

            public void Perform(ActionType action, ActionParams actionParams)
            {
                switch (action)
                {
                    case ActionType.TurnOn:
                        this.TurnOn(actionParams);
                        break;
                    case ActionType.TurnOff:
                        this.TurnOff(actionParams);
                        break;
                    case ActionType.Feed:
                        this.Feed(actionParams);
                        break;
                    case ActionType.Close:
                        this.Close(actionParams);
                        break;
                    case ActionType.Open:
                        this.Open(actionParams);
                        break;
                    case ActionType.Prepare:
                        this.Prepare(actionParams);
                        break;
                    case ActionType.Set:
                        this.Set(actionParams);
                        break;
                }
            }
            public string GetNickName() { return nickName; }

            public virtual void TurnOn(ActionParams actionParams) { }
            public virtual void TurnOff(ActionParams actionParams) { }
            public virtual void Feed(ActionParams actionParams) { }
            public virtual void Close(ActionParams actionParams) { }
            public virtual void Open(ActionParams actionParams) { }
            public virtual void Set(ActionParams actionParams) { }
            public virtual void Prepare(ActionParams actionParams) { }
        }

        class WindowCurtains : Appliance
        {
            public WindowCurtains(string nickName) : base(nickName) { }

            override public ApplianceType GetApplianceType()
            {
                return ApplianceType.Curtain;
            }

            public override void Close(ActionParams actionParams)
            {
                Console.WriteLine(GetNickName() + "-Closing the curtain.");
            }

            public override void Open(ActionParams actionParams)
            {
                Console.WriteLine(GetNickName() + "-Opening the curtain.");
            }

            public override void TurnOn(ActionParams actionParams) => Open(actionParams);
        }

        class AirConditioning : Appliance
        {
            public AirConditioning(string nickName) : base(nickName) { }

            override public ApplianceType GetApplianceType()
            {
                return ApplianceType.AirConditioning;
            }

            public override void TurnOn(ActionParams actionParams)
            {
                Console.WriteLine(GetNickName() + "-Turning on.");
            }
            public override void TurnOff(ActionParams actionParams)
            {
                Console.WriteLine(GetNickName() + "-Turning off.");
            }
        }

        class DogFeeder : Appliance
        {
            public DogFeeder(string nickName) : base(nickName) { }

            override public ApplianceType GetApplianceType()
            {
                return ApplianceType.DogFeeder;
            }

            public override void Feed(ActionParams actionParams)
            {
                Console.WriteLine(GetNickName() + "-Feeding the dog.");
            }
        }

        class Dishwasher : Appliance
        {
            public Dishwasher(string nickName) : base(nickName) { }

            override public ApplianceType GetApplianceType()
            {
                return ApplianceType.Dishwasher;
            }

            public override void TurnOn(ActionParams actionParams)
            {
                Set(actionParams);
            }

            public override void Set(ActionParams actionParams)
            {
                Console.WriteLine(GetNickName() + "-Washing the dish for {0}.", actionParams.Duration);
            }
        }

        class LightingSystem : Appliance
        {
            public LightingSystem(string nickName) : base(nickName) { }

            override public ApplianceType GetApplianceType()
            {
                return ApplianceType.LightingSystem;
            }

            public override void TurnOn(ActionParams actionParams)
            {
                Console.WriteLine(GetNickName() + "-Turning on.");
            }
            public override void TurnOff(ActionParams actionParams)
            {
                Console.WriteLine(GetNickName() + "-Turning off.");
            }
        }

        class Sprinkler : Appliance
        {
            public Sprinkler(string nickName) : base(nickName) { }

            override public ApplianceType GetApplianceType()
            {
                return ApplianceType.Sprinkler;
            }

            public override void TurnOn(ActionParams actionParams)
            {
                Console.WriteLine(GetNickName() + "-Turning on for {0} mins.", actionParams.Duration);
            }
            public override void TurnOff(ActionParams actionParams)
            {
                Console.WriteLine(GetNickName() + "-Turning off.");
            }
        }

        class Blender : Appliance
        {
            public Blender(string nickName) : base(nickName) { }

            override public ApplianceType GetApplianceType()
            {
                return ApplianceType.Blender;
            }

            public override void Prepare(ActionParams actionParams)
            {
                Console.WriteLine(GetNickName() + "-Scheduled to prepare smoothie for {0}.", string.IsNullOrEmpty(actionParams.TimePeriod) ? actionParams.Duration : actionParams.TimePeriod);
            }
        }

        class Alarm : Appliance
        {
            public Alarm(string nickName) : base(nickName) { }

            override public ApplianceType GetApplianceType()
            {
                return ApplianceType.Alarm;
            }

            public override void Set(ActionParams actionParams)
            {
                Console.WriteLine(GetNickName() + "-Alarm set for {0}.", string.IsNullOrEmpty(actionParams.TimePeriod) ? actionParams.Duration : actionParams.TimePeriod);
            }
        }
    }
}
