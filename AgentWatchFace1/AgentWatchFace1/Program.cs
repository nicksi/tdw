﻿using System;
using System.Collections;

using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;

namespace AgentWatchFace1
{
    public class Program
    {
        static Bitmap _display;
        static Timer _updateClockTimer;

        // set the following to true to outline screen in emulator
        const bool DISPLAY_BORDER_BOX = true;
        const int SCREEN_SIZE = 128;

        public static ArrayList tde = new ArrayList();

        public static void Main()
        {

            // initialize our display buffer
            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            // obtain the current time
            DateTime currentTime = DateTime.Now;
            // create funnyh events
            tde.Add(new ToDoEvent { Type = ToDoEventTypes.EVENT_CALL, DueDate = currentTime.AddMinutes(10), Label = "Call mother"});
            tde.Add(new ToDoEvent { Type = ToDoEventTypes.EVENT_HATE, DueDate = currentTime.AddMinutes(80), Label = "Board meeting" });

            // display the time immediately
            UpdateTime(null);


            // set up timer to refresh time every minute
            TimeSpan dueTime = new TimeSpan(0, 0, 0, 59 - currentTime.Second, 1000 - currentTime.Millisecond); // start timer at beginning of next minute
            TimeSpan period = new TimeSpan(0, 0, 0, 1, 0); // update time every minute
            _updateClockTimer = new Timer(UpdateTime, null, dueTime, period); // start our update timer

            // go to sleep; time updates will happen automatically every minute
            Thread.Sleep(Timeout.Infinite);
        }

        private static Bitmap ReadHrs(int hour)
        {

            if (hour == 0)
                return new Bitmap(Resources.GetBytes(Resources.BinaryResources._12hours), Bitmap.BitmapImageType.Gif);
            else if (hour == 1)
                return new Bitmap(Resources.GetBytes(Resources.BinaryResources._01hours), Bitmap.BitmapImageType.Gif);
            else
                return new Bitmap(SCREEN_SIZE, SCREEN_SIZE);

            //hrs.Add(new Bitmap(Resources.GetBytes(Resources.BinaryResources._01hours), Bitmap.BitmapImageType.Gif));
            //hrs.Add(new Bitmap(Resources.GetBytes(Resources.BinaryResources._12hours), Bitmap.BitmapImageType.Gif));
            //hrs.Add(new Bitmap(Resources.GetBytes(Resources.BinaryResources._12hours), Bitmap.BitmapImageType.Gif));
            //hrs.Add(new Bitmap(Resources.GetBytes(Resources.BinaryResources._12hours), Bitmap.BitmapImageType.Gif));
            //hrs.Add(new Bitmap(Resources.GetBytes(Resources.BinaryResources._12hours), Bitmap.BitmapImageType.Gif));
            //hrs.Add(new Bitmap(Resources.GetBytes(Resources.BinaryResources._12hours), Bitmap.BitmapImageType.Gif));
            //hrs.Add(new Bitmap(Resources.GetBytes(Resources.BinaryResources._12hours), Bitmap.BitmapImageType.Gif));
            //hrs.Add(new Bitmap(Resources.GetBytes(Resources.BinaryResources._12hours), Bitmap.BitmapImageType.Gif));
            //hrs.Add(new Bitmap(Resources.GetBytes(Resources.BinaryResources._12hours), Bitmap.BitmapImageType.Gif));
            //hrs.Add(new Bitmap(Resources.GetBytes(Resources.BinaryResources._12hours), Bitmap.BitmapImageType.Gif));
            //hrs.Add(new Bitmap(Resources.GetBytes(Resources.BinaryResources._12hours), Bitmap.BitmapImageType.Gif));
        }

        static void UpdateTime(object state)
        {
           
            Debug.Print("Tick");
            
            // clear our display buffer
            _display.Clear();

            //update our local time reference
            //Device.Time.CurrentTime = DateTime.Now; //just normal
            //currentTime = currentTime.AddMinutes(1); //speedy time
            //Device.Time.CurrentTime = new DateTime(2011, 12, 16, 12, 4, 0, 0); //hard coded time

            // Get current time
            DateTime currentTime = DateTime.Now;
            int nowSecond = currentTime.Second;
            int nowMinute = currentTime.Minute;
            int nowHour = currentTime.Hour;

            Debug.Print(nowMinute.ToString());

            #region draw hours
            // use big spritesheet for now
            _display.DrawImage(0, 0, ReadHrs(nowHour%12), 0,  0 , SCREEN_SIZE, SCREEN_SIZE);
            #endregion

            #region draw minutes
                DrawTicks(nowMinute );
            #endregion

            #region draw date
            Font bigFont = Resources.GetFont(Resources.FontResources.ubuntu15c);
            PaintCentered(currentTime.ToString("dd MMM, ddd").ToUpper(), bigFont, Color.White, SCREEN_SIZE/4 + 2);

            #endregion

            #region draw todo
            for (int i = 0; i < tde.Count; ++i)
            {
                if (((ToDoEvent)tde[i]).DueDate > currentTime)
                {
                    ToDoEvent item = (ToDoEvent)tde[i];
                    
                    Font smallFont = Resources.GetFont(Resources.FontResources.ubuntu15c);

                    TimeSpan ts = item.DueDate - currentTime;
                    
                    if ( ts.Days > 0 )

                        PaintCentered(
                            "in " + ts.Hours + "d " + ts.Minutes + "h",
                            smallFont,
                            Color.White,
                            SCREEN_SIZE * 3/4 - 10
                            );

                    else 

                        PaintCentered(
                            "in " + ts.Hours + "h " + ts.Minutes + "m",
                            smallFont,
                            Color.White,
                            SCREEN_SIZE * 3/4 - 10
                            );

                    
                    if ( item.Label.Length > 0)
                    {
                        if (MeasureString(item.Label, smallFont) > 40)
                        {
                            string fragment = item.Label.Substring(0, item.Label.Length-1);
                            while (MeasureString(fragment, smallFont) > 35)
                                fragment = fragment.Substring(0, fragment.Length - 1);
                            _display.DrawText(fragment + "..." , smallFont, Color.White, SCREEN_SIZE/4 + 24, SCREEN_SIZE/2);
                        }
                        else
                            _display.DrawText(item.Label, smallFont, Color.White, SCREEN_SIZE/4 + 24, SCREEN_SIZE/2);
                    }
                    break;
                }
            }
            #endregion


            #region misc
            // draw a border around the screen, if desired.
            if (DISPLAY_BORDER_BOX)
            {
                _display.DrawRectangle(Color.White, 1, 0, 0, SCREEN_SIZE, SCREEN_SIZE, 0, 0, Color.White, 0, 0, Color.White, 0, 0, 0);
            }
            #endregion


            _display.Flush(); // flush the display buffer to the display
            Debug.Print("Tack");
        }

        private static void DrawTick(int x, int y, int width, int height)
        {
            _display.DrawRectangle(Color.White, 0, x, y, width, height, 0, 0, Color.White, x, y, Color.White, x+width, y+height, 0xFF );
        }

        public static void PaintCentered(string text, Font Font, Color Color, int y)
        {
            int x, y1 = 0;
            FindCenter(text, Font, out x, out y1);

            _display.DrawText(text, Font, Color, x, y);
        }

        public static int MeasureString(string text, Font font)
        {
            if (text == null || text.Trim() == "") return 0;
            int size = 0;
            for (int i = 0; i < text.Length; i++)
            {
                size += font.CharWidth(text[i]);
            }
            return size;
        }

        public static void PaintCentered(string text, Font Font, Color Color)
        {

            int x, y = 0;
            FindCenter(text, Font, out x, out y);

            _display.DrawText(text, Font, Color, x, y);
        }

        public static void FindCenter(string text, Font Font, out int x, out int y)
        {

            int size = MeasureString(text, Font);
            int center = SCREEN_SIZE / 2;
            int centerText = size / 2 - 2;
            x = center - centerText;

            y = center - (Font.Height / 2);

        }

        private static void DrawTicks(int nowMinute)
        {

            int MinuteCounter = 0;
            const int TICK_WIDTH = 4;
            const int TICK_HEIGHT = 7;
            const int TICK_GAP = 3;
            const int TICK_OFFSET = 2;
            const int TICK_CORNER_SIZE = 8;
            const int TICK_BOX_STEP = TICK_WIDTH + TICK_GAP; 
            
            const int TICK_PER_SIDE = 15;
            
            // let's try to draw in code
            // top row - right half
            for (int i = 0; i < TICK_PER_SIDE / 2 && MinuteCounter < nowMinute; ++i)
            {
                DrawTick(SCREEN_SIZE / 2 - TICK_WIDTH / 2 + TICK_OFFSET + i * TICK_BOX_STEP, TICK_OFFSET, TICK_WIDTH, TICK_HEIGHT);
                ++MinuteCounter;
            }
            //// draw top right corner
            //if (MinuteCounter < nowMinute)
            //{
            //    _display.DrawImage(SCREEN_SIZE - TICK_OFFSET - TICK_CORNER_SIZE, TICK_OFFSET, MinuteCorners, 0, 0, TICK_CORNER_SIZE, TICK_CORNER_SIZE);
            //    ++MinuteCounter;
            //}

            //if (MinuteCounter < nowMinute)
            //{
            //    _display.DrawImage(SCREEN_SIZE - TICK_OFFSET - TICK_CORNER_SIZE, TICK_OFFSET, MinuteCorners, 0, TICK_CORNER_SIZE, TICK_CORNER_SIZE, TICK_CORNER_SIZE);
            //    ++MinuteCounter;
            //}

            // draw right side
            for (int i = 0; i < TICK_PER_SIDE && MinuteCounter < nowMinute; ++i)
            {
                DrawTick(SCREEN_SIZE - TICK_HEIGHT - TICK_OFFSET, TICK_OFFSET + TICK_CORNER_SIZE + TICK_GAP + i * TICK_BOX_STEP, TICK_HEIGHT, TICK_WIDTH);
                ++MinuteCounter;
            }

            // drow bottm right corner

            //if (MinuteCounter < nowMinute)
            //{
            //    _display.DrawImage(SCREEN_SIZE - TICK_OFFSET - TICK_CORNER_SIZE, SCREEN_SIZE - TICK_OFFSET - TICK_CORNER_SIZE, MinuteCorners, 0, 2 * TICK_CORNER_SIZE, TICK_CORNER_SIZE, TICK_CORNER_SIZE);
            //    ++MinuteCounter;
            //}

            //if (MinuteCounter < nowMinute)
            //{
            //    _display.DrawImage(SCREEN_SIZE - TICK_OFFSET - TICK_CORNER_SIZE, SCREEN_SIZE - TICK_OFFSET - TICK_CORNER_SIZE, MinuteCorners, 0, 3 * TICK_CORNER_SIZE, TICK_CORNER_SIZE, TICK_CORNER_SIZE);
            //    ++MinuteCounter;
            //}

            // draw bottom side

            for (int i = 0; i < TICK_PER_SIDE && MinuteCounter < nowMinute; ++i)
            {
                DrawTick(SCREEN_SIZE - TICK_OFFSET - TICK_CORNER_SIZE - TICK_GAP - TICK_BOX_STEP - i * TICK_BOX_STEP, SCREEN_SIZE - TICK_OFFSET - TICK_HEIGHT, TICK_WIDTH, TICK_HEIGHT);
                ++MinuteCounter;
            }


            //// draw bottom left corner
            //if (MinuteCounter < nowMinute)
            //{
            //    _display.DrawImage(TICK_OFFSET, SCREEN_SIZE - TICK_OFFSET - TICK_CORNER_SIZE, MinuteCorners, 0, 4 * TICK_CORNER_SIZE, TICK_CORNER_SIZE, TICK_CORNER_SIZE);
            //    ++MinuteCounter;
            //}

            //if (MinuteCounter < nowMinute)
            //{
            //    _display.DrawImage(TICK_OFFSET, SCREEN_SIZE - TICK_OFFSET - TICK_CORNER_SIZE, MinuteCorners, 0, 5 * TICK_CORNER_SIZE, TICK_CORNER_SIZE, TICK_CORNER_SIZE);
            //    ++MinuteCounter;
            //}

            // draw left side
            for (int i = 0; i < TICK_PER_SIDE && MinuteCounter < nowMinute; ++i)
            {
                DrawTick(TICK_OFFSET, SCREEN_SIZE - TICK_OFFSET - TICK_CORNER_SIZE - TICK_GAP - TICK_BOX_STEP - i * TICK_BOX_STEP, TICK_HEIGHT, TICK_WIDTH);
                ++MinuteCounter;
            }


            // draw top left corner
            //if (MinuteCounter < nowMinute)
            //{
            //    _display.DrawImage(TICK_OFFSET, TICK_OFFSET, MinuteCorners, 0, 6 * TICK_CORNER_SIZE, TICK_CORNER_SIZE, TICK_CORNER_SIZE);
            //    ++MinuteCounter;
            //}

            //if (MinuteCounter < nowMinute)
            //{
            //    _display.DrawImage(TICK_OFFSET, TICK_OFFSET, MinuteCorners, 0, 7 * TICK_CORNER_SIZE, TICK_CORNER_SIZE, TICK_CORNER_SIZE);
            //    ++MinuteCounter;
            //}

            // draw top row left half
            for (int i = 0; i < TICK_PER_SIDE && MinuteCounter < nowMinute; ++i)
            {
                DrawTick(TICK_OFFSET + TICK_CORNER_SIZE +  i * TICK_BOX_STEP, TICK_OFFSET, TICK_WIDTH, TICK_HEIGHT);
                ++MinuteCounter;
            }
        }


    }
}