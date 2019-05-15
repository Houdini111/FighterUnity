using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AttackMapEditor
{
    public class Classification
    {
        public Color color;
        public Mode mode;
        public Shape shape;

        public static Classification getNormal()
        {
            return new Classification()
            {
                color = Color.FromRgb(152, 152, 152),
                mode = Mode.Normal,
                shape = Shape.Circle,
            };
        }

        public static Classification getIntermediary()
        {
            return new Classification()
            {
                color = Color.FromRgb(246, 161, 66),
                mode = Mode.Inter,
                shape = Shape.Diamond,
            };
        }

        public static Classification getFinal()
        {
            return new Classification()
            {
                color = Color.FromRgb(245, 73, 66),
                mode = Mode.Final,
                shape = Shape.Square,
            };
        }

        public static Classification GetClassificationByMode(Mode m)
        {
            if (m == Mode.Normal) { return getNormal(); }
            if (m == Mode.Inter) { return getIntermediary(); }
            if (m == Mode.Final) { return getFinal(); }
            return getNormal();
        }

        public override string ToString()
        {
            return mode.ToString();
        }
    }
}
