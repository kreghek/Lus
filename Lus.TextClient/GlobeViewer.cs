﻿using Terminal.Gui;

namespace Lus.TextClient
{
    sealed class GlobeViewer : View 
    {
        int w = 40;
        int h = 40;

        public GlobeViewer(int x, int y) : base(new Rect(x, y, 40, 40))
        {
            CanFocus = true;
        }

        public Size GetContentSize()
        {
            return new Size(w, h);
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
                        Driver.SetAttribute(new Attribute(Color.DarkGray, Color.Black));
                        Driver.AddStr("?");
                    }
                    else if (Matrix.Items[i, j] == 1)
                    {
                        Driver.SetAttribute(new Attribute(Color.Green, Color.Black));
                        Driver.AddStr("Y");
                    }
                    else if (Matrix.Items[i, j] == 2)
                    {
                        Driver.SetAttribute(new Attribute(Color.Gray, Color.Black));
                        Driver.AddStr("0");
                    }
                    else if (Matrix.Items[i, j] == 3)
                    {
                        Driver.SetAttribute(new Attribute(Color.Brown, Color.Black));
                        Driver.AddStr("_");
                    }
                    else if (Matrix.Items[i, j] == 4)
                    {
                        Driver.SetAttribute(new Attribute(Color.BrightBlue, Color.Black));
                        Driver.AddStr("@");
                    }
                    else if (Matrix.Items[i, j] == 5)
                    {
                        Driver.SetAttribute(new Attribute(Color.BrightBlue, Color.Black));
                        Driver.AddStr("░");
                    }
                }
            }
        }
    }
}
