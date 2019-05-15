using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttackMapEditor
{
    public static class ChildrenDeconstructionHelper
    {
        public static void Deconstruct<Direction, Node>(this KeyValuePair<Direction, Node> tuple, out Direction key, out Node value)
        {
            key = tuple.Key;
            value = tuple.Value;
        }
    }
}
