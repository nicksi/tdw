using System;
using Microsoft.SPOT;
using System.Threading;
using Microsoft.SPOT.Presentation.Media;

namespace tdw
{
    enum ToDoEventTypes
    {
        EVENT_CALL = 0,
        EVENT_MEET = 1,
        EVENT_ALARM = 2,
        EVENT_WARN = 3,
        EVENT_LOVE = 4,
        EVENT_HATE = 5
    }

    class ToDoEvent
    {
        private bool _inverse = false;
        
        public DateTime DueDate { get; set; }
        public ToDoEventTypes Type { get; set; }
        public string Label { get; set; }
        public bool Active { get; set; }

        public ToDoEvent(ToDoEventTypes type, DateTime dueDate, string label)
        {
            Type = type;
            DueDate = dueDate;
            Label = label;
            Active = false;
        }

        
        /// <summary>
        /// Draw event on 64 x 48 bitmap
        /// </summary>
        /// <param name="bitmap">Bitmap to draw on</param>
        /// <param name="time">Current time</param>
        public void PaintEvent(Bitmap bitmap, DateTime time)
        { 
            Font timeFont = Resources.GetFont(Resources.FontResources.ubuntu16c);
            Font smallFont = Resources.GetFont(Resources.FontResources.ubuntu12c);
            Painter painter = new Painter(bitmap);
            TimeSpan ts = DueDate - time;
            Bitmap todotypes;

            // draw separator
            bitmap.DrawLine(Color.White, 1, 0, 0, 64, 0);
            
            // if inverse draw background and use inverted icons
            if (_inverse)
            {
                bitmap.DrawRectangle(Color.White, 0, 0, 3, 64, 18, 0, 0, Color.White, 0, 0, Color.White, 64, 48, 0xFF);
                todotypes = new Bitmap(Resources.GetBytes(Resources.BinaryResources.todotypes_inverted), Bitmap.BitmapImageType.Gif);
            }
            else
                todotypes = new Bitmap(Resources.GetBytes(Resources.BinaryResources.todotypes), Bitmap.BitmapImageType.Gif);

            //// draw icon
            bitmap.DrawImage(0, 5, todotypes, 0, (int)Type * 16, 16, 16);
            
            if (ts.Days > 0)

                bitmap.DrawTextInRect(
                    ts.Days + "days",
                    16,
                    0,
                    48,
                    18,
                    Bitmap.DT_AlignmentCenter,
                    _inverse?Color.Black:Color.White,
                    timeFont
                    );
            else if ( ts.Days == 0 && ts.Minutes == 0 )
                bitmap.DrawTextInRect(
                    "NOW!",
                    16,
                    0,
                    48,
                    20,
                    Bitmap.DT_AlignmentCenter,
                    _inverse ? Color.Black : Color.White,
                    timeFont
                    );

            else
                bitmap.DrawTextInRect(
                    ts.Hours + "h " + ts.Minutes + "m",
                    16,
                    0,
                    48,
                    20,
                    Bitmap.DT_AlignmentCenter,
                    _inverse ? Color.Black : Color.White,
                    timeFont
                    );


            if (Label.Length > 0)
            {
                bitmap.DrawTextInRect(Label, 0, 19, 64, 30, Bitmap.DT_WordWrap|Bitmap.DT_TruncateAtBottom, Color.White, smallFont);
            }
        }

        /// <summary>
        /// Draw one todo event on provided bitmap
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="time"></param>
        public void DrawEvent(Bitmap bitmap, DateTime time)
        {
            
            // decide if it is time to use inverted colors
            if ((DueDate - time).Ticks < 150000000f) // blink last 15 seconds
                _inverse = !_inverse;
            else if ((DueDate - time).Ticks < 60 * 150000000f) // show in inverted colors for last 15 minutes
                _inverse = true;
            else
                _inverse = false;

            // if bitmap is larger than 64 x 48 - create smaller one and copy back
            if (bitmap.Height != 48 || bitmap.Width != 64)
            {
                Bitmap smallBitmap = new Bitmap(64, 48);
                PaintEvent(smallBitmap, time);
                bitmap.DrawImage(32, 48, smallBitmap, 0, 0, 64, 48);
            }
            else
                PaintEvent(bitmap, time);

        }

        /// <summary>
        /// .NETMF TimeSPan has no Total Seconds property. I will calculate it here
        /// </summary>
        /// <returns></returns>
        public long TotalSeconds(DateTime time)
        {
            return (DueDate - time).Ticks / 10000000; // 1 tick = 100 nanoseconds
        }

    }
}
