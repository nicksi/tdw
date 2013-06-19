/*
 * Created by Rob Chartier (https://github.com/nothingmn)
 */



using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace tdw
{

    public enum Buttons
    {
        Top = 2,
        Middle = 0,
        Bottom = 1
    }

    public enum ButtonDirection
    {
        Down = 0,
        Up = 1
    }

    public class ButtonHelper
    {
        private static object _lock = new object();
        private static ButtonHelper current;
        public static ButtonHelper Current
        {
            get
            {
                lock (_lock)
                {
                    if (current == null) current = new ButtonHelper((Cpu.Pin)Buttons.Top, (Cpu.Pin)Buttons.Middle,
                                                    (Cpu.Pin)Buttons.Bottom);
                }
                return current;
            }
        }
        public delegate void ButtonPress(Buttons button, InterruptPort port, ButtonDirection direction, DateTime time);

        public event ButtonPress OnButtonPress;

        private InterruptPort topSwitch;
        private InterruptPort middleSwitch;
        private InterruptPort bottomSwitch;

        private ButtonHelper()
        {
        }

        private ButtonHelper(Cpu.Pin button1, Cpu.Pin button2, Cpu.Pin button3)
        {

            topSwitch = new InterruptPort(button1, false, Port.ResistorMode.Disabled,
                                            Port.InterruptMode.InterruptEdgeBoth);
            middleSwitch = new InterruptPort(button2, false, Port.ResistorMode.Disabled,
                                                Port.InterruptMode.InterruptEdgeBoth);
            bottomSwitch = new InterruptPort(button3, false, Port.ResistorMode.Disabled,
                                                Port.InterruptMode.InterruptEdgeBoth);
            topSwitch.OnInterrupt += topSwitch_OnInterrupt;
            middleSwitch.OnInterrupt += topSwitch_OnInterrupt;
            bottomSwitch.OnInterrupt += topSwitch_OnInterrupt;
        }

        private void topSwitch_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            //data1 is the is the number of the pin of the switch
            //data2 is the value if the button is pushed or released; 0 = down, 1 = up
            Debug.Print("data1:" + data1.ToString() + ", data2:" + data2.ToString());
            if (OnButtonPress != null)
            {
                Buttons _button = (Buttons)data1;
                ButtonDirection _direction = (ButtonDirection)data2;
                if (data1 == (uint)topSwitch.Id) OnButtonPress(_button, topSwitch, _direction, time);
                if (data1 == (uint)middleSwitch.Id) OnButtonPress(_button, middleSwitch, _direction, time);
                if (data1 == (uint)bottomSwitch.Id) OnButtonPress(_button, bottomSwitch, _direction, time);
            }

        }

    }
}
