using System;
using System.Collections;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using Agent.Contrib.Hardware;

namespace tdw
{
    public class Program
    {
        
        private static Bitmap screen {get; set;}
        private static Timer _updateClockTimer { get; set; }

        // set the following to true to outline screen in emulator
        const int SCREEN_SIZE = 128;

        const int TICK_WIDTH = 4;
        const int TICK_HEIGHT = 7;
        const int BLINK_BEFORE = 15;

        private static ArrayList tde = new ArrayList();
        private static int ActiveTde = -1;
        private static bool tick_fast = false;

        private static DateTime currentTime;

        public event ButtonHelper.ButtonPress OnButtonPress;
        private static ButtonHelper buttonHelper;
        
        public static void Main()
        {
            screen = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            // display the time immediately
            currentTime = DateTime.Now;

            // create funnyh events
            tde.Add(new ToDoEvent(ToDoEventTypes.EVENT_CALL, currentTime.AddSeconds(80), "Call mother"));
            tde.Add(new ToDoEvent(ToDoEventTypes.EVENT_HATE, currentTime.AddMinutes(80), "Board meeting" ));
            
            UpdateTime(null);

            TimeSpan dueTime = new TimeSpan(0, 0, 0, 59 - currentTime.Second, 1000 - currentTime.Millisecond); // start timer at beginning of next minute
            TimeSpan period = new TimeSpan(0, 0, 1, 0, 0); // update time every minute
            //TimeSpan period = new TimeSpan(0, 0, 0, 15, 0); // update time every 5 sec
            // set up timer to refresh time every minute
            _updateClockTimer = new Timer(UpdateTime, null, dueTime, period); // start our update timer


            buttonHelper = ButtonHelper.Current;
            buttonHelper.OnButtonPress += buttonHelper_OnButtonPress;

            // go to sleep; time updates will happen automatically every minute
            Thread.Sleep(Timeout.Infinite);
        }

        private static Bitmap ReadHrs(int hour)
        {
            if (hour == 0)
                return new Bitmap(Resources.GetBytes(Resources.BinaryResources._12hours), Bitmap.BitmapImageType.Gif);
            else if (hour == 1)
                return new Bitmap(Resources.GetBytes(Resources.BinaryResources._01hours), Bitmap.BitmapImageType.Gif);
            else if (hour == 2)
                return new Bitmap(Resources.GetBytes(Resources.BinaryResources._02hours), Bitmap.BitmapImageType.Gif);
            else if (hour == 3)
                return new Bitmap(Resources.GetBytes(Resources.BinaryResources._03hours), Bitmap.BitmapImageType.Gif);
            else if (hour == 4)
                return new Bitmap(Resources.GetBytes(Resources.BinaryResources._04hours), Bitmap.BitmapImageType.Gif);
            else if (hour == 5)
                return new Bitmap(Resources.GetBytes(Resources.BinaryResources._05hours), Bitmap.BitmapImageType.Gif);
            else if (hour == 6)
                return new Bitmap(Resources.GetBytes(Resources.BinaryResources._06hours), Bitmap.BitmapImageType.Gif);
            else if (hour == 7)
                return new Bitmap(Resources.GetBytes(Resources.BinaryResources._07hours), Bitmap.BitmapImageType.Gif);
            else if (hour == 8)
                return new Bitmap(Resources.GetBytes(Resources.BinaryResources._08hours), Bitmap.BitmapImageType.Gif);
            else if (hour == 9)
                return new Bitmap(Resources.GetBytes(Resources.BinaryResources._09hours), Bitmap.BitmapImageType.Gif);
            else if (hour == 10)
                return new Bitmap(Resources.GetBytes(Resources.BinaryResources._10hours), Bitmap.BitmapImageType.Gif);
            else if (hour == 11)
                return new Bitmap(Resources.GetBytes(Resources.BinaryResources._11hours), Bitmap.BitmapImageType.Gif);
            else
                return new Bitmap(SCREEN_SIZE, SCREEN_SIZE);
        }

        static void UpdateTime(object state)
        {
           
            Debug.Print("Tick");
            Painter painter = new Painter(screen);
            
            // clear our display buffer
            screen.Clear();

            //update our local time reference
            //Device.Time.CurrentTime = DateTime.Now; //just normal
            //currentTime = currentTime.AddMinutes(1); //speedy time

            // Get current time
            currentTime = DateTime.Now;
            int nowSecond = currentTime.Second;
            int nowMinute = currentTime.Minute;
            int nowHour = currentTime.Hour;

            Debug.Print(nowMinute.ToString());

            #region draw hours
            // use big spritesheet for now
            screen.DrawImage(0, 0, ReadHrs(nowHour%12), 0,  0, screen.Width, screen.Height);
            #endregion

            #region draw minutes
                DrawTicks(nowMinute );
            #endregion

            #region draw date
            Font bigFont = Resources.GetFont(Resources.FontResources.ubuntu15c);
            painter.PaintCentered(currentTime.ToString("dd MMM, ddd").ToUpper(), bigFont, false, SCREEN_SIZE/4 );

            #endregion

            #region draw todo
            //screen.DrawRectangle(Color.White, 1, 32, 48, 64, 48, 0, 0, Color.White, 0, 0, Color.White, 0, 0, 0);
            // if three are some events
            if (tde.Count > 0)
            {
                // if we have no active event - choose the nearest one (always first cell in array list)
                if (ActiveTde < 0)
                {
                    ActiveTde = 0;
                }

                ToDoEvent evt = (ToDoEvent)tde[ActiveTde];
                evt.DrawEvent(screen, currentTime);

                // if active evenet is overdue for more than 1 minute - remove from the list
                if (evt.TotalSeconds(currentTime) < -60)
                {
                    tde.Remove(ActiveTde);
                    ActiveTde = -1;

                    StopTicking();
                }
                // else if Active  event is not the first one check if the first one is due next minutes and switch
                else if (ActiveTde != 0)
                {
                    long interval = ((ToDoEvent)tde[0]).TotalSeconds(currentTime);
                    if ( interval < 60 + BLINK_BEFORE ) 
                    { 
                        ActiveTde = 0;
                        StartTicking((int)(interval - BLINK_BEFORE));
                    }
                    else if ( interval < 120 )
                    {
                        ActiveTde = 0;
                    }
                }
                // if current event is the first one - check if it is time to start ticking
                else if (ActiveTde == 0)
                {
                    long interval = ((ToDoEvent)tde[0]).TotalSeconds(currentTime);
                    if ( interval < 60 + BLINK_BEFORE)
                    {
                        StartTicking((int)(interval - BLINK_BEFORE));
                    }
                }
            }
            #endregion


            screen.Flush(); // flush the display buffer to the display
            Debug.Print("Tack");
        }

        private static void StartTicking(int seconds)
        {
            if (!tick_fast)
            {
                // stop fast ticking after @seconds@
                _updateClockTimer.Change(seconds, 1000);
                tick_fast = true;
            }
        }

        private static void StopTicking()
        {
            if (tick_fast)
            {
                // stop fast ticking
                TimeSpan dueTime = new TimeSpan(0, 0, 0, 59 - currentTime.Second, 1000 - currentTime.Millisecond); // start timer at beginning of next minute
                TimeSpan period = new TimeSpan(0, 0, 1, 0, 0); // update time every minute
                _updateClockTimer.Change(dueTime, period);
                tick_fast = false;
            }
        }

        private static void DrawTicks(int nowMinute)
        {

            
            for (int i = 0; i <= nowMinute; ++i)
            {
                PointAndSize p = GetTickPosition(i);
                if (i%5 == 0)
                    //screen.DrawEllipse(Color.White, 1, p.X + p.WIDTH/2 , p.Y + p.HEIGHT/2, TICK_WIDTH / 2, TICK_WIDTH / 2, Color.Black, 0, 0, Color.Black, TICK_WIDTH, TICK_WIDTH, 0XFF);
                    screen.DrawRectangle(Color.White, 1, p.X, p.Y, p.WIDTH, p.HEIGHT, 0, 0, Color.Black, p.X, p.Y, Color.Black, p.X + p.WIDTH, p.Y + p.HEIGHT, 0xFF);
                else
                    screen.DrawRectangle(Color.White, 0, p.X, p.Y, p.WIDTH, p.HEIGHT, 0, 0, Color.White, p.X, p.Y, Color.White, p.X + p.WIDTH, p.Y + p.HEIGHT, 0xFF);
            }

        }

        public static PointAndSize GetTickPosition(int minute)
        {
            const int TICK_PER_SIDE = 15;
            const int TICK_GAP = 3;
            const int TICK_OFFSET = 2;
            const int TICK_CORNER_SIZE = 8;
            const int TICK_BOX_STEP = TICK_WIDTH + TICK_GAP; 

            if (minute >= 0 && minute < TICK_PER_SIDE / 2 + 1)
                // top right
                return new PointAndSize(SCREEN_SIZE / 2 - TICK_WIDTH / 2 + minute * TICK_BOX_STEP, TICK_OFFSET, TICK_WIDTH, TICK_HEIGHT);
            else if (minute >= TICK_PER_SIDE / 2 + 1 && minute < TICK_PER_SIDE + TICK_PER_SIDE / 2 + 1)
                // right
                return new PointAndSize(SCREEN_SIZE - TICK_HEIGHT - TICK_OFFSET, TICK_OFFSET + TICK_CORNER_SIZE + TICK_GAP + (minute - (TICK_PER_SIDE / 2 + 1)) * TICK_BOX_STEP, TICK_HEIGHT, TICK_WIDTH);
            else if (minute >= TICK_PER_SIDE + TICK_PER_SIDE / 2 + 1 && minute < TICK_PER_SIDE * 2 + TICK_PER_SIDE / 2 + 1)
                // bottom
                return new PointAndSize(SCREEN_SIZE - TICK_OFFSET - TICK_CORNER_SIZE - TICK_BOX_STEP - (minute - (TICK_PER_SIDE + TICK_PER_SIDE / 2 + 1)) * TICK_BOX_STEP, SCREEN_SIZE - TICK_OFFSET - TICK_HEIGHT, TICK_WIDTH, TICK_HEIGHT);
            else if (minute >= TICK_PER_SIDE * 2 + TICK_PER_SIDE / 2 + 1 && minute < TICK_PER_SIDE * 3 + TICK_PER_SIDE / 2 + 1)
                // left
                return new PointAndSize(TICK_OFFSET, SCREEN_SIZE - TICK_OFFSET - TICK_CORNER_SIZE - TICK_BOX_STEP - (minute - (TICK_PER_SIDE * 2 + TICK_PER_SIDE / 2 + 1)) * TICK_BOX_STEP, TICK_HEIGHT, TICK_WIDTH);
            else if (minute >= TICK_PER_SIDE * 3 + TICK_PER_SIDE / 2 + 1 && minute < TICK_PER_SIDE * 4)
                // top left
                return new PointAndSize(TICK_OFFSET + TICK_CORNER_SIZE + TICK_GAP + (minute - (TICK_PER_SIDE * 3 + TICK_PER_SIDE / 2 + 1)) * TICK_BOX_STEP, TICK_OFFSET, TICK_WIDTH, TICK_HEIGHT);

            return new PointAndSize(0, 0, 0, 0);
        }

        private static void buttonHelper_OnButtonPress(Buttons button, InterruptPort port, ButtonDirection direction,
                                        DateTime time)
        {
            Debug.Print("button: " + button.ToString() + " direction:" + direction.ToString());            
        }

    }
}
