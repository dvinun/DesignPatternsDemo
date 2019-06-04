namespace Dvinun.DesignPatterns.Behavioral
{
    partial class Interpreter
    {
        interface IAction
        {
            void TurnOn(ActionParams actionParams);
            void TurnOff(ActionParams actionParams);
            void Feed(ActionParams actionParams);
            void Close(ActionParams actionParams);
            void Open(ActionParams actionParams);
            void Set(ActionParams actionParams);
            void Prepare(ActionParams actionParams);
        }

        class ActionParams
        {
            public string Duration { get; set; }
            public string TimePeriod { get; set; }
        }

        enum ActionType
        {
            None,
            TurnOn,
            TurnOff,
            Feed,
            Close,
            Open,
            Prepare,
            Set
        }
    }
}