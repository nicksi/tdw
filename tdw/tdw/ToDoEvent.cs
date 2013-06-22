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
        private Timer _timer;
        private bool _inverse = false;
        
        public DateTime DueDate { get; set; }
        public ToDoEventTypes Type { get; set; }
        public string Label { get; set; }

        public ToDoEvent(ToDoEventTypes type, DateTime dueDate, string label)
        {
            Type = type;
            DueDate = dueDate;
            Label = label;

            DateTime currentTime = DateTime.Now;
            // warn 10 seconds before event
            if ( currentTime < DueDate.AddSeconds(-10))
            {
                // setup timer
                TimeSpan due = (DueDate.AddSeconds(-10) - currentTime);
                TimeSpan period = new TimeSpan(0, 0, 1);
                _timer = new Timer(ShowAlert, this, due, period);
            }
        }

        
        private void ShowAlert(object tde)
        {
            DateTime currentTime = DateTime.Now;
            // stop after 30 sec
            if ( (currentTime - DueDate).Seconds >= 30)
            {
                _timer.Dispose();
            }
            else{

                // blink event
                _inverse = !_inverse;

                Bitmap bitmap = new Bitmap(64,48);

                ((ToDoEvent)tde).DrawEvent(bitmap, currentTime);

                bitmap.Flush(32, 48, 64, 48);

                // vibrate

            }

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
                bitmap.DrawRectangle(Color.White, 0, 0, 3, 64, 20, 0, 0, Color.White, 0, 0, Color.White, 64, 48, 0xFF);
                todotypes = new Bitmap(Resources.GetBytes(Resources.BinaryResources.todotypes_inverted), Bitmap.BitmapImageType.Gif);
            }
            else
                todotypes = new Bitmap(Resources.GetBytes(Resources.BinaryResources.todotypes), Bitmap.BitmapImageType.Gif);

            //// draw icon
            bitmap.DrawImage(0, 4, todotypes, 0, Type.GetHashCode() * 16, 16, 16);
            
            if (ts.Days > 0)

                bitmap.DrawTextInRect(
                    ts.Days + "days",
                    16,
                    0,
                    48,
                    20,
                    Bitmap.DT_AlignmentCenter,
                    _inverse?Color.Black:Color.White,
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
                bitmap.DrawTextInRect(Label, 0, 22, 64, 26, Bitmap.DT_WordWrap|Bitmap.DT_TruncateAtBottom, Color.White, smallFont);
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
            if ((DueDate - time).Milliseconds < 15 * 1000f) // blink last 15 seconds
                _inverse = !_inverse;
            else if ((DueDate - time).Milliseconds < 15 * 60 * 1000f) // show in inverted colors for last 15 minutes
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

    }
}
