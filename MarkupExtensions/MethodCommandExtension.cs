using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace Thingie.WPF.MarkupExtensions
{
    public enum MethodNotFoundBehavior
    {
        ThrowException,
        LogToTrace,
        Ignore
    }

    //todo: probati naslijediti Binding pa dodati funkcionalnost bindanja na metodu - prednosti: moze se iskoristiti binding za naciljati target umjesto da se fiksno koristi datacontext. 
    public class MethodCommandExtension : MarkupExtension, ICommand
    {
        public MethodNotFoundBehavior MethodNotFoundBehavior { get; set; }

        public string ExecMethod { get; set; }
        public string CanExecMember { get; set; }
        public bool GuessCanExecMember { get; set; }

        FrameworkElement _targetElement;
        Action<object> _execMethod;
        Func<object, bool> _canExecMethod = (o) => false;

        public MethodCommandExtension() : this(null, null) { }
        public MethodCommandExtension(string execMethod) : this(execMethod, null) { }
        public MethodCommandExtension(string execMethod, string canExecMember)
        {
            ExecMethod = execMethod;
            CanExecMember = canExecMember;
            GuessCanExecMember = true;
            MethodNotFoundBehavior = MarkupExtensions.MethodNotFoundBehavior.ThrowException;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecMethod(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _execMethod(parameter);
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var targetProvider = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
            _targetElement = targetProvider.TargetObject as FrameworkElement;
            if (_targetElement != null)
            {
                _targetElement.DataContextChanged += (s, e) => { Init(); };
                if (_targetElement.DataContext != null)
                    Init();
            }
            return this;
        }

        private void Init()
        {
            object target = _targetElement.DataContext;

            _execMethod = (parameter) =>
            {
                MethodInfo mi = target.GetType().GetMethod(ExecMethod);
                if (mi == null)
                {
                    LogError(string.Format("Error binding to method {0} - method not found!", ExecMethod));
                }
                else
                {
                    if (mi.GetParameters().Count() == 0)
                        mi.Invoke(target, null);
                    else if (mi.GetParameters().Count() == 1)
                        mi.Invoke(target, new object[] { parameter });
                    else
                    {
                        LogError(string.Format("Error binding to method {0} - method signature invalid! Signature with zero or one parameters required.", ExecMethod));
                    }
                }
            };

            _canExecMethod = (parameter) =>
            {
                bool canExecute = true;

                if (target != null)
                {
                    MemberInfo canExecMemberInfo = null;
                    if (!string.IsNullOrEmpty(CanExecMember))
                    {
                        canExecMemberInfo = target.GetType().GetMember(CanExecMember).SingleOrDefault();
                        if (canExecMemberInfo == null)
                            LogError(string.Format("Error binding to CanExec member {0} - member not found", CanExecMember));
                    }
                    else
                    {
                        if (GuessCanExecMember)
                            canExecMemberInfo = target.GetType().GetMember("Can" + ExecMethod).SingleOrDefault();
                    }

                    MethodInfo getMethod = null;
                    if (canExecMemberInfo != null)
                    {
                        if (canExecMemberInfo is PropertyInfo)
                            getMethod = (canExecMemberInfo as PropertyInfo).GetGetMethod();
                        else if (canExecMemberInfo is MethodInfo)
                            getMethod = (canExecMemberInfo as MethodInfo);
                    }

                    if (getMethod != null)
                    {
                        if (getMethod.GetParameters().Count() == 0)
                            canExecute = (bool)getMethod.Invoke(target, null);
                        else if (getMethod.GetParameters().Count() == 1)
                            canExecute = (bool)getMethod.Invoke(target, new object[] { parameter });
                        else
                            LogError(string.Format("Error binding to CanExec method '{0}' - method signature invalid! Signature with zero or one parameters required.", getMethod.Name));
                    }
                }
                else
                {
                    canExecute = false;
                }

                return canExecute;
            };
        }

        private void LogError(string errorMessage)
        {
            string errorMessageFull = string.Format("MethodCommand error (Exec:'{0}'/CanExec:'{1}'). Message: {2}", ExecMethod, CanExecMember, errorMessage);

            switch (MethodNotFoundBehavior)
            {
                case MethodNotFoundBehavior.ThrowException:
                    throw new ArgumentException(errorMessageFull);
                case MethodNotFoundBehavior.LogToTrace:
                    Trace.WriteLine(errorMessageFull);
                    break;
                case MethodNotFoundBehavior.Ignore:
                    break;
                default:
                    break;
            }
        }
    }
}
