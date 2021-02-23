using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BindingProxyDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BindingProxy<ClrClass> _vm;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = _vm = new BindingProxy<ClrClass>(new ClrClass());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //下面这样设置值不会通知到ui
            _vm.Instance.ClrIntProp = 55;
            
            // 常规操作 通知ui
            var setResult = _vm.TrySetMember(() => _vm.Instance.ClrIntProp,55);

            // 隐式转换 float -> int  ,通知ui
            setResult = _vm.TrySetMember(() => _vm.Instance.ClrIntProp , 55f);
            // 隐式转换 double -> int ,通知ui
            setResult = _vm.TrySetMember(() => _vm.Instance.ClrIntProp , 55d);

            //编译时报错
            //setResult = _vm.TrySetMember(() => _vm.Instance.ClrIntProp , "55");

            if (_vm.HasError)
            {
                Console.WriteLine(_vm.Error);
            }




        }

        //如果你想重写属性较验可以
        class YourClass : BindingProxy<ClrClass>
        {
            public YourClass(ClrClass instance) : base(instance)
            {

            }

            //重写这个方法 
            protected override string Validate(BindingProxy<ClrClass> proxy, string propName)
            {
                //这里虽然没有返回错误信息，但如果出现46行那种情况，不会走到这个方法，优先返回类型转换的错误出去。
                return null;
            }
        }
    }
}
