using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{
    internal class ColoredListBoxItem
    {
        public string Text { get; set; }

        public Color color;
        public Color Color { get; set; }

        public ColoredListBoxItem(string text, Color color)
        {
            this.Text = text;
            this.Color = color;
        }

        public override string ToString() => this.Text;
    }
}
