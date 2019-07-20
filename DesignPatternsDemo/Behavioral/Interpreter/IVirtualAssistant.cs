using System.Threading;

namespace Dvinun.DesignPatterns.Behavioral
{
    partial class Interpreter
    {
        interface IVirtualAssistant { }

        class Alexa : IVirtualAssistant
        {
            IOTDevice iotDevice;

            internal void Interface(IOTDevice iotDevice)
            {
                this.iotDevice = iotDevice;
            }

            internal void Run()
            {
                // received voice commands from human
                iotDevice.NewCommand("Hi Alexa! Turn-on the central AC.");
                Thread.Sleep(3000);

                iotDevice.NewCommand("Hi Alexa! Close the kitchen curtains.");
                Thread.Sleep(3000);

                iotDevice.NewCommand("Alexa! Turn-on the dish-washer for 1 hour.");
                Thread.Sleep(3000);

                iotDevice.NewCommand("Alexa! Turn-off kitchen lights.");
                Thread.Sleep(3000);

                // This will fail the user will not adress Alexa before the command.
                // In reality, this shouldnt have come here bcoz Alexa only listens 
                // commands addressed to it only. Everything is fair in love and code :-) 
                iotDevice.NewCommand("Turn-on the garden sprinkler for 20 mins.");
                Thread.Sleep(3000);

                iotDevice.NewCommand("Alexa! Feed my dog Ana.");
                Thread.Sleep(3000);

                iotDevice.NewCommand("Alexa! Prepare the smoothie for tomorrow morning.");
                Thread.Sleep(3000);

                iotDevice.NewCommand("Alexa! Set the alarm for tomorrow 6am.");
                Thread.Sleep(3000);
            }
        }
    }
}
