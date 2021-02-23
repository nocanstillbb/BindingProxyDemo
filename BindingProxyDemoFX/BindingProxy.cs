using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BindingProxyDemo
{
    public class BindingProxy<T> : DynamicObject, INotifyPropertyChanged, IDataErrorInfo
    {

        class Mytuple
        {
            public Mytuple(PropertyInfo a, string b)
            {
                PropInfo = a;
                SetErrorInfo = b;
            }
            public PropertyInfo PropInfo { get; set; }
            public string SetErrorInfo { get; set; }
        }

        private static readonly Dictionary<string, Dictionary<string, Mytuple>> Properties = new Dictionary<string, Dictionary<string, Mytuple>>();


        public readonly T Instance;
        private readonly string typeName;

        private string Validate(string propName)
        {
            if (!string.IsNullOrWhiteSpace(Properties[typeName][propName]?.SetErrorInfo))
            {
                return Properties[typeName][propName]?.SetErrorInfo;
            }
            return Validate(this, propName);
        }
        protected virtual string Validate(BindingProxy<T> proxy, string propName)
        {
            return null;
        }
        public string Error
        {
            get
            {
                string result = null;
                if (Properties.ContainsKey(typeName))
                    result = string.Join("\n", Properties[typeName].Values.Select(t => this[t.PropInfo.Name]).Where(t => t != null));
                else
                    result = string.Join("\n", typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public).Select(t => this[t.Name]).Where(t => t != null));
                return result;
            }

        }
        public bool HasError
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Error);
            }
        }


        public string this[string columnName]
        {
            get
            {
                return Validate(columnName);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public BindingProxy(T instance)
        {
            this.Instance = instance;

            var type = typeof(T);
            typeName = type.FullName;
            if (typeName != null && !Properties.ContainsKey(typeName))
                SetProperties(type, typeName);
            UpdateErrorInfo();

        }
        private static void SetProperties(Type type, string typeName)
        {
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var dict = props.ToDictionary(prop => prop.Name, t => new Mytuple(t, null));
            Properties.Add(typeName, dict);
        }
        public override bool TryGetMember(GetMemberBinder binder,
            out object result)
        {
            if (Properties[typeName].ContainsKey(binder.Name))
            {
                result = Properties[typeName][binder.Name].PropInfo.GetValue(Instance, null);
                return true;
            }
            result = null;
            return false;
        }

        public bool TrySetMember<TT>(Expression<Func<TT>> expression, TT value)
        {
            var name = string.Empty;
            if (expression.Body is MemberExpression mp)
            {
                name = (expression.Body as MemberExpression).Member.Name;
            }
            else if (expression.Body is UnaryExpression ue && ue.Operand is MemberExpression mp2)
            {
                name = mp2.Member.Name;
            }

            return setMemberByName(value, name);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var name = binder.Name;
            return setMemberByName(value, name);
        }

        private bool setMemberByName(object value, string name)
        {
            if (!Properties[typeName].ContainsKey(name))
                return false;
            if (Properties[typeName][name].PropInfo.GetValue(Instance).Equals(value))
            {
                return true;
            }
            try
            {

                Properties[typeName][name].PropInfo.SetValue(Instance, Convert.ChangeType(value, Properties[typeName][name].PropInfo.PropertyType), null);
                Properties[typeName][name].SetErrorInfo = null;
            }
            catch (Exception ex)
            {
                if (ex is InvalidCastException || ex is FormatException || ex is OverflowException || ex is ArgumentException)
                {
                    const string str = "属性:[{0}] 值:[{2}] 格式校验异常:{1}";
                    Properties[typeName][name].SetErrorInfo = string.Format(str, name, ex.Message, value.ToString());
                    UpdateErrorInfo();
                    return false;
                }
                throw;
            }
            UpdateErrorInfo();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            return true;
        }

        const string indexerName = "Item[]";
        public void UpdateErrorInfo()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Error)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasError)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(indexerName));
        }
    }
}
