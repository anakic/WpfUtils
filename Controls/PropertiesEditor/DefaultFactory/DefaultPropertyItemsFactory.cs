using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Thingie.WPF.Controls.PropertiesEditor.DefaultFactory.Attributes;
using Thingie.WPF.Controls.PropertiesEditor.Proxies;
using Thingie.WPF.Resources;
using Thingie.WPF.Controls.PropertiesEditor.Proxies.Base;
using Thingie.WPF.Attributes;

namespace Thingie.WPF.Controls.PropertiesEditor.DefaultFactory
{
	/// <summary>
	/// The default implementation of the IPropertyProxyFactory interface. This implementation
	/// uses attributes specified (Viewable, Editable, ChoiceEditable...) on types and properties
	/// to generate the appropriate property proxies. 
	/// </summary>
	public class DefaultPropertyItemsFactory : IPropertyProxyFactory
    {
        public virtual IEnumerable<PropertyProxy> CreatePropertyItems(object target)
        {
            IEnumerable<PropertyInfo> properties = target.GetType().GetProperties();
            if (target.GetType().IsDefined(typeof(ViewableAttribute), true) == false)
            {
                properties = properties.Where(pi => pi.IsDefined(typeof(ViewableAttribute), false));
            }
            return properties.Select(pInfo => CreateItem(target, pInfo)).OrderBy(px => px.Order).ToList();
        }

        protected virtual PropertyProxy CreateItem(object target, PropertyInfo property)
        {
            PropertyProxy proxy;

            //1. pronadji (ako postoji) relevantni editor atribut na propertyju
            ViewableAttribute viewableAtt = null;
            if (property.IsDefined(typeof(ViewableAttribute), true))
                viewableAtt = (ViewableAttribute)property.GetCustomAttributes(typeof(ViewableAttribute), true)[0];

            //2. odluci kojeg tipa ce bit proxy za property na temelju atributa
            if (viewableAtt != null)
            {
                if (viewableAtt is EditableCustomAttribute)
                    proxy = new CustomPropertyProxy((viewableAtt as EditableCustomAttribute).ControlType);
                else if (viewableAtt is EditableTextAttribute)
                {
                    EditableTextAttribute edTextAtt = viewableAtt as EditableTextAttribute;
                    proxy = new TextPropertyProxy() { Big = edTextAtt.Big, AcceptsReturn = edTextAtt.AcceptsReturn, AcceptsTab = edTextAtt.AcceptsTab };
                }
                else if (viewableAtt is EditableFilePathAttribute)
                {
                    EditableFilePathAttribute pathAtt = viewableAtt as EditableFilePathAttribute;
                    proxy = new BrowseFilePropertyProxy() { CheckFileExists = pathAtt.CheckExists, Filter = pathAtt.Filter };
                }
                else if (viewableAtt is EditableFolderPathAttribute)
                {
                    EditableFolderPathAttribute pathAtt = viewableAtt as EditableFolderPathAttribute;
                    proxy = new BrowseFolderPropertyProxy() { CheckFolderExists = pathAtt.CheckExists };
                }
                else if (viewableAtt is EditableMultiChoiceAttribute)
                {
                    throw new NotImplementedException("Radi remake-a ChoicePropertyProxy-a ovo vise ne radi, popraviti kada zatreba!");
                    //proxy = new CustomPropertyProxy(typeof(ListEditor), (editorAtt as EditableChoiceAttribute).GetChoices(target), (editorAtt as EditableChoiceAttribute).DisplayMemberPath);
                }
                else if (viewableAtt is EditableChoiceAttribute)
                {
                    EditableChoiceAttribute choiceAtt = viewableAtt as EditableChoiceAttribute;
                    if (choiceAtt.Choices != null)
                        proxy = new ChoicePropertyProxy(choiceAtt.Choices);
                    else if (choiceAtt.ChoicesProperty != null)
                        proxy = new ChoicePropertyProxy(choiceAtt.ChoicesProperty);
                    else
                        proxy = new ChoicePropertyProxy(choiceAtt.TypeOfCollectionWithChoices, choiceAtt.ConstructorArgs);

                    var choiceProxy = proxy as ChoicePropertyProxy;
                    choiceProxy.DisplayMemberPath = choiceAtt.DisplayMemberPath;
                    choiceProxy.IsEditable = choiceAtt.IsTextEditable;
                    choiceProxy.IsAsync = choiceAtt.IsAsync;
                }
                else if (viewableAtt is EditableShortcutAttribute)
                {
                    proxy = new ShortcutPropertyProxy();
                }
                else if (viewableAtt is EditableAttribute)
                    proxy = GetDefaultProxyForProperty(property);
                else //if (editorAtt is ViewableAttribute)
                    proxy = new ReadonlyPropertyProxy(); ;
            }
            else
            {
                proxy = GetDefaultProxyForProperty(property);
            }

            //3. inicijaliziraj uobicajene propertyje proxyja
            proxy.Target = target;
            proxy.Property = property;
            proxy.Description = property.GetCustomAttributes().OfType<System.ComponentModel.DescriptionAttribute>().Select(att => att.Description).SingleOrDefault();

            //4. inicijaliziraj display propertyje (ime, kategorija, order)
            NameAttributeBase nameAttribute = property.CustomAttributes.OfType<NameAttributeBase>().FirstOrDefault();
            if (nameAttribute != null)
                proxy.Name = nameAttribute.Name;
            else
                proxy.Name = property.Name;

            CategoryAttributeBase categoryAttribute = property.GetCustomAttributes(true).OfType<CategoryAttributeBase>().FirstOrDefault();
            if (categoryAttribute != null)
                proxy.Category = categoryAttribute.Category;
            else
                proxy.Category = Localizations.General;

            OrderAttribute orderAttribute = property.GetCustomAttributes(true).OfType<OrderAttribute>().FirstOrDefault();
            if (orderAttribute != null)
                proxy.Order = orderAttribute.Order;
            else
                proxy.Order = int.MaxValue;

            //editability condition
            if (viewableAtt is EditableAttribute editableAtt && !string.IsNullOrWhiteSpace(editableAtt.When))
                (proxy as EditablePropertyProxy).SetAvailabilityCondition(editableAtt.When);

            return proxy;
        }

        //potrazi custom editor atribut na tipu, 
        //- ako ga ima napravi odgovarajuci custom editor proxy
        //- ako ga nema, onda kreiraj defaultni proxy za taj tip property-ja:
        //  - dropdown za enum 
        //  - checkbox za bool
        //  - obicni text za ostalo.
        private PropertyProxy GetDefaultProxyForProperty(PropertyInfo property)
        {
            PropertyProxy proxy;

            if (property.PropertyType.IsDefined(typeof(TypeCustomEditorAttribute), true))
            {
                TypeCustomEditorAttribute typeEditorAtt = (TypeCustomEditorAttribute)property.PropertyType.GetCustomAttributes(typeof(TypeCustomEditorAttribute), true)[0];
                proxy = new CustomPropertyProxy(typeEditorAtt.EditorType);
            }
            else
            {
                if (property.PropertyType.IsEnum)
                {
                    if (property.PropertyType.GetCustomAttributes(true).OfType<FlagsAttribute>().Count() > 0)
                        proxy = new CustomPropertyProxy(typeof(FlagsEnumPicker));
                    else
                        proxy = new ChoicePropertyProxy(Enum.GetValues(property.PropertyType));
                }
                else if (property.PropertyType == typeof(bool))
                    proxy = new BoolPropertyProxy();
                else
                    proxy = new TextPropertyProxy();
            }
            return proxy;
        }
    }
}
