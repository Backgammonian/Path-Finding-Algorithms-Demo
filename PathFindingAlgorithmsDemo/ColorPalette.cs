using System.Collections.Generic;
using System.Drawing;

namespace PathFindingAlgorithmsDemo
{
    public class ColorPalette
    {
        private readonly Dictionary<FieldElements, Color> _colors;

        public ColorPalette()
        {
            _colors = new Dictionary<FieldElements, Color>();
        }

        public int Count => _colors.Count;

        public Color this[FieldElements element]
        {
            get => _colors[element];
            private set => _colors[element] = value;
        }

        public void Add(FieldElements element, Color color)
        {
            _colors.Add(element, color);
        }

        public void Remove(FieldElements element)
        {
            if (_colors.ContainsKey(element))
            {
                _colors.Remove(element);
            }
        }
    }
}
