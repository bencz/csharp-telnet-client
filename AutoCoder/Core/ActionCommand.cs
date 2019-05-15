using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace AutoCoder.Core
{
  /// <summary>
  /// class stores an Action delegate that implements the ICommand interface.
  /// </summary>
  public class ActionCommand : ICommand
  {
    readonly Action<object> _execute;
    readonly Predicate<object> _canExecute;

    public ActionCommand(Action<object> executeDelegate)
      : this(executeDelegate, null )
    {
    }

    public ActionCommand(Action<object> executeDelegate, Predicate<object> canExecuteDelegate)
    {
      _execute = executeDelegate;
      _canExecute = canExecuteDelegate;
    }

    public void Execute(object parameter)
    {
      _execute(parameter);
    }

    public bool CanExecute(object parameter) 
    {
      if (_canExecute == null)
        return true;
      else
        return _canExecute(parameter);
    }

    public event EventHandler CanExecuteChanged;
  }
}
