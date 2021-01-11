# BindingProxyDemo

```c#
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
```
