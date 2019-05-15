using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttackMapEditor
{
    ///This is a placeholder class for functions to be used in Unity.
    ///Here, there is only the names of the functions to be used in Unity.

    public interface IActionType
    {
        bool shouldExecute();
        bool execute();
    }

    public class AttackAction : IActionType
    {
        public bool execute()
        {
            throw new NotImplementedException();
        }

        public bool shouldExecute()
        {
            throw new NotImplementedException();
        }
    }
}
