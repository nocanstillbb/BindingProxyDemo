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
            //此方法只在.net frame work中有效，  .net core中无法正常使用，如果需要在.net core中使用需要添加额外的洗发水

            var random = new Random((int)DateTime.Now.Ticks);
            var result = random.Next();
            //下面这样设置值不会通知到ui
            _vm.Instance.ClrIntProp = result;

            ////下面这样设置值会通知到ui，但是重构名称不友好
            //((dynamic)_vm).ClrIntProp = result+1;

            //下面这样设置值会通知到ui 属性名称可以重构
            _vm.TrySetMember(nameof(_vm.Instance.ClrIntProp), result + 2);

            //因为是基于dynamic的+convert.changetype，  所以这样也能正常工作
            _vm.TrySetMember(nameof(_vm.Instance.ClrIntProp), $"{result + 2}");

            //这样不能正常工作
            _vm.TrySetMember(nameof(_vm.Instance.ClrIntProp), $"{result + 2}.{result + 2}");

            //所以名称可以放心重构，但类型还是要紧肾，代码里可以这样写
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
