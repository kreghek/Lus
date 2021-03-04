using System;

using Terminal.Gui;

namespace Lus.TextClient
{
    class BattlefieldViewer : View
    {
        int w = 40;
        int h = 40;

        public bool WantCursorPosition { get; set; } = false;

        public BattlefieldViewer(int x, int y) : base(new Rect(x, y, 40, 40))
        {
        }

        public Size GetContentSize()
        {
            return new Size(w, h);
        }

        public void SetCursorPosition(Point pos)
        {
            throw new NotImplementedException();
        }

        public Matrix Matrix { get; set; }

        public override void Redraw(Rect bounds)
        {
            if (Matrix is null)
            {
                return;
            }

            for (var i = 0; i < w; i++)
            {
                for (var j = 0; j < h; j++)
                {
                    Move(i, j);

                    if (Matrix.Items[i, j] == 0)
                    {
                        Driver.SetAttribute(new Terminal.Gui.Attribute(Color.DarkGray, Color.Black));
                        Driver.AddStr("░");
                    }
                    else if (Matrix.Items[i, j] == 1)
                    {
                        Driver.SetAttribute(new Terminal.Gui.Attribute(Color.BrightBlue, Color.Black));
                        Driver.AddStr("@");
                    }
                    else if (Matrix.Items[i, j] == 2)
                    {
                        Driver.SetAttribute(new Terminal.Gui.Attribute(Color.BrightRed, Color.Black));
                        Driver.AddStr("@");
                    }
                }
            }
        }
    }
}
