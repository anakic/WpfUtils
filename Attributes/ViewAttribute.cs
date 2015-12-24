using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thingie.WPF.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public class ViewAttribute : Attribute
    {
        Type _typeOfView;
        public Type TypeOfView
        {
            get { return _typeOfView; }
            set { _typeOfView = value; }
        }

        IEnumerable<object> _viewConstructorParams;
        public IEnumerable<object> ViewConstructorParams
        {
            get { return _viewConstructorParams; }
            set { _viewConstructorParams = value; }
        }

        string _propertyForTargetObject;
        public string PropertyForTargetObject
        {
            get { return _propertyForTargetObject; }
            set { _propertyForTargetObject = value; }
        }

        public string Context { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeOfView">The type of the view to use.</param>
        /// <param name="propertyForTargetObject">The property on the view that will hold the target object</param>
        /// <param name="viewConstructorParams">The parameters for the view constructor</param>
        public ViewAttribute(Type typeOfView, string propertyForTargetObject, params object[] viewConstructorParams)
            : this(null, typeOfView, propertyForTargetObject, viewConstructorParams)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeOfView">The type of the view to use.</param>
        /// <param name="propertyForTargetObject">The property on the view that will hold the target object</param>
        /// <param name="viewConstructorParams">The parameters for the view constructor</param>
        public ViewAttribute(string context, Type typeOfView, string propertyForTargetObject, params object[] viewConstructorParams)
        {
            _typeOfView = typeOfView;
            _viewConstructorParams = viewConstructorParams;
            _propertyForTargetObject = propertyForTargetObject;
        }
    }
}
