# BindingProxyDemo

```c#
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

```
