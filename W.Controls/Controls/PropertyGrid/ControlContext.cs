using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using W.Controls.Attributes;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.ComponentModel.DataAnnotations;
using System.Security.Permissions;

namespace W.Controls.Controls.PropertyGrid
{
    /// <summary>
    /// 装饰器模式
    /// </summary>
    public class ControlContext
    {
        public PropertyInfo Property { get; set; }
        public object BindingSource { get; set; }
        public FrameworkElement Control { get; set; } // 具体的输入控件（TextBox, ComboBox等）
        public Panel WrapPanel { get; set; }        // 控件的直接包装容器
        public Grid RootCellGrid { get; set; }      // 包含 Label 和 Control 的最外层格子
    }

    public interface IControlProcessor
    {
        void Execute(ControlContext context);
    }
    public class ValidationProcessor : IControlProcessor
    {
        public void Execute(ControlContext context)
        {
            var validators = context.Property.GetCustomAttributes<ValidationBaseAttribute>().ToList();
            if (!validators.Any()) return;

            // 创建一个用于显示错误的 TextBlock
            var errorText = new TextBlock
            {
                FontSize = 10,
                Foreground = Brushes.Red,
                Visibility = Visibility.Collapsed,
                Margin = new Thickness(2, 2, 0, 0)
            };

            // 将错误提示加到控件下方的包装容器中
            if (context.WrapPanel is StackPanel sp) sp.Children.Add(errorText);

            // 注入逻辑
            if (context.Control is TextBox tb)
            {
                tb.LostFocus += (s, e) => {
                    var firstError = validators.FirstOrDefault(v => !v.IsValid(tb.Text));
                    if (firstError != null)
                    {
                        tb.BorderBrush = Brushes.Red;
                        errorText.Text = firstError.ErrorMessage;
                        errorText.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        tb.ClearValue(TextBox.BorderBrushProperty);
                        errorText.Visibility = Visibility.Collapsed;
                    }
                };
            }
        }
    }
    public class CommandProcessor : IControlProcessor
    {
        public void Execute(ControlContext context)
        {
            var attr = context.Property.GetCustomAttribute<CommandAttribute>();
            if (attr == null) return;
            //var actionBtn = new Button
            //{
            //    Content = "...",
            //    Width = 30,
            //    Height = 30,
            //    Margin = new Thickness(5, 0, 0, 0),
            //    VerticalAlignment = VerticalAlignment.Center
            //};

            ApplyCommandBinding(context.Control, attr, context.BindingSource);

            //if (context.RootCellGrid.ColumnDefinitions.Count < 3)
            //    context.RootCellGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            //Grid.SetColumn(actionBtn, context.RootCellGrid.ColumnDefinitions.Count - 1);
            //context.RootCellGrid.Children.Add(actionBtn);
        }
        private void ApplyCommandBinding(FrameworkElement target, CommandAttribute attr, object BindingObject)
        {
            if (attr == null || string.IsNullOrEmpty(attr.Command))
                return;
            // 1. 创建绑定对象
            Binding commandBinding = new Binding(attr.Command);

            // 2. 处理 RelativeSource 逻辑
            if (attr.Mode != RelativeSourceMode.Self)
            {
                commandBinding.RelativeSource = new RelativeSource
                {
                    Mode = attr.Mode,
                    AncestorType = attr.AncestorType,
                    AncestorLevel = attr.AncestorLevel,
                };
            }
            else
            {
                commandBinding.Source = BindingObject;
            }

            // 3. 确定绑定目标
            // 如果是 Button，绑定 Command 属性；如果是 TextBox，可以绑定到行为或特定属性
            if (target is ButtonBase button)
            {
                BindingOperations.SetBinding(button, ButtonBase.CommandProperty, commandBinding);

                // 处理 CommandParameter
                if (attr.CommandParam != null)
                {
                    button.CommandParameter = attr.CommandParam;
                }
            }
            // 这里可以根据需要扩展，比如给 TextBox 绑定回车命令等
        }
    }
    public class LayoutProcessor : IControlProcessor
    {
        private readonly DisplayAttribute _display;
        private readonly PropertyLayoutDirection _direction;

        // 可以在构造函数中传入布局方向，或者从特性中读取
        public LayoutProcessor(DisplayAttribute display, PropertyLayoutDirection direction = PropertyLayoutDirection.Left)
        {
            _display = display;
            _direction = direction;
        }

        public void Execute(ControlContext context)
        {
            var grid = context.RootCellGrid;
            var label = CreateLabel(context.Property, _display);
            var wrapper = context.WrapPanel; // 这个容器里装的是真正的输入控件

            // 清理旧定义防止冲突
            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();

            switch (_direction)
            {
                case PropertyLayoutDirection.Left:
                    // 经典的左右布局：[Label(85) | Control(*)]
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(85) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                    Grid.SetColumn(label, 0);
                    Grid.SetColumn(wrapper, 1);
                    label.VerticalAlignment = VerticalAlignment.Center;
                    break;

                case PropertyLayoutDirection.Top:
                    // 现代的上下布局：[Label]
                    //               [Control]
                    grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                    Grid.SetRow(label, 0);
                    Grid.SetRow(wrapper, 1);
                    label.Margin = new Thickness(2, 0, 0, 6);
                    break;

                case PropertyLayoutDirection.Right:
                    // 标签在右：[Control(*) | Label(85)]
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(85) });

                    Grid.SetColumn(wrapper, 0);
                    Grid.SetColumn(label, 1);
                    label.HorizontalAlignment = HorizontalAlignment.Right;
                    break;

                case PropertyLayoutDirection.Bottom:
                    // 标签在下：[Control]
                    //          [Label]
                    grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                    Grid.SetRow(wrapper, 0);
                    Grid.SetRow(label, 1);
                    label.Margin = new Thickness(2, 6, 0, 0);
                    break;
            }

            // 将生成的元素添加到容器中
            grid.Children.Add(label);
            grid.Children.Add(wrapper);

            // 初始化包装器：确保护送真正的输入控件进入
            if (!wrapper.Children.Contains(context.Control))
            {
                wrapper.Children.Add(context.Control);
            }
        }

        private TextBlock CreateLabel(PropertyInfo prop, DisplayAttribute display)
        {
            return new TextBlock
            {
                Text = display?.GetName() ?? prop.Name,
                FontSize = 13,
                Foreground = new SolidColorBrush(Color.FromRgb(71, 85, 105)),
                TextTrimming = TextTrimming.CharacterEllipsis,
                ToolTip = display?.GetDescription()
            };
        }
    }
    public class PermissionProcessor : IControlProcessor
    {
        public void Execute(ControlContext context)
        {
            var attr = context.Property.GetCustomAttribute<PermissionAttribute>();
            if (attr == null) return;

            // 模拟权限检查逻辑 (实际项目中对接你的 UserManager 或 AuthService)
            bool hasPermission = CheckUserHasRole(attr.RequiredRole);

            if (!hasPermission)
            {
                if (attr.HideIfDenied)
                {
                    // 方案 A：直接隐藏整个格子
                    context.RootCellGrid.Visibility = Visibility.Collapsed;
                }
                else
                {
                    // 方案 B：禁用控件，并添加提示
                    context.Control.IsEnabled = false;
                    context.Control.Opacity = 0.5;

                    // 装饰一下：在 Label 旁边加个小锁图标
                    var label = context.RootCellGrid.Children.OfType<TextBlock>().FirstOrDefault();
                    if (label != null)
                    {
                        label.Text += " 🔒";
                        label.ToolTip = $"需要 {attr.RequiredRole} 权限";
                    }
                }
            }
        }

        private bool CheckUserHasRole(string role)
        {
            // 这里对接你的业务系统
            // 例如：return GlobalUser.CurrentRole == role;
            return false;
        }
    }

}
