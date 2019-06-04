using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dvinun.DesignPatterns.Behavioral
{
    // Demo to interpret the human voice commands and interpret using the interpreter design pattern.
    // The scenario here is that the IOIDevice device will connect to both appliances (Lighting systems, Alarms, Dod Feeder etc.)
    // and Virtual Assistant like Alexa. 
    // The Virtual Assistant will pass on the voice commands to IOT Device 
    // The IOTDevice will interpret the voice commands using interpreter pattern 
    // and then issues appropriate commands to appliances to do their job.

    // We user regex grammar style to design the sub-interpreters.
    partial class Interpreter
    {
        public static void PlayDemo()
        {
            Alexa johnsAlexa = new Alexa();

            IOTDevice johnsIOTDevice = new VinnysHomeAutomation();
            johnsIOTDevice.Connect(new WindowCurtains("John's Room Curtains"));
            johnsIOTDevice.Connect(new WindowCurtains("Kitchen Curtains"));
            johnsIOTDevice.Connect(new AirConditioning("Central AC"));
            johnsIOTDevice.Connect(new DogFeeder("Dog Sonu"));
            johnsIOTDevice.Connect(new DogFeeder("Dog Ana"));
            johnsIOTDevice.Connect(new Dishwasher("Dish-washer"));
            johnsIOTDevice.Connect(new LightingSystem("Living Room Lighting"));
            johnsIOTDevice.Connect(new LightingSystem("Kitchen Lights"));
            johnsIOTDevice.Connect(new LightingSystem("Backyard Lights"));
            johnsIOTDevice.Connect(new Sprinkler("Garden Sprinkler"));
            johnsIOTDevice.Connect(new Blender("Smoothie"));
            johnsIOTDevice.Connect(new Alarm("Alarm"));

            johnsAlexa.Interface(johnsIOTDevice);

            johnsAlexa.Run();
        }
    }
}
