using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AutoCoder.Telnet.Models
{
  public class PrintTrafficItemCommand : ICommand
  {
    Action<object> _executeDelegate;

    public PrintTrafficItemCommand(Action<object> executeDelegate)
    {
      _executeDelegate = executeDelegate;
    }

    public void Execute(object parameter)
    {
      _executeDelegate(parameter);
    }

    public bool CanExecute(object parameter) { return true; }

    public event EventHandler CanExecuteChanged;
  }

}
