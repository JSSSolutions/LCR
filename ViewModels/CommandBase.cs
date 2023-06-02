using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CommandBase
{
    // $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$

    public class CmdBase<T> : ICommand
    {
        public event EventHandler CanExecuteChanged = delegate { };
        Action<T> _TargetExecuteMethod;
        Func<T, bool> _TargetCanExecuteMethod;

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        public CmdBase(Action<T> ExecuteMethod)
        {
            _TargetExecuteMethod = ExecuteMethod;
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        public CmdBase(Action<T> ExecuteMethod, Func<T, bool> CanExecuteMethod)
        {
            _TargetExecuteMethod = ExecuteMethod;
            _TargetCanExecuteMethod = CanExecuteMethod;
        }

        // *********************************************************************

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }

        #region ICommand Members

        // #####################################################################

        bool ICommand.CanExecute(object Parameter)
        {

            if (_TargetCanExecuteMethod != null)
            {
                T tparm = (T)Parameter;
                return _TargetCanExecuteMethod(tparm);
            }

            if (_TargetExecuteMethod != null)
            {
                return true;
            }

            return false;
        }

        // *********************************************************************

        void ICommand.Execute(object Parameter)
        {
            if (_TargetExecuteMethod != null)
            {
                _TargetExecuteMethod((T)Parameter);
            }
        }

        #endregion
    }
}