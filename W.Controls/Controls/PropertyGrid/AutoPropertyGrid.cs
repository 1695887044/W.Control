using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using W.Controls.Attributes;
using W.Controls.Controls.PropertyGrid;
using W.Controls.Icons;

namespace W.Controls.Controls
{
    public class AutoPropertyGrid : Control
    {
        public List<IControlGenerator> Generators { get; } = new List<IControlGenerator>();
        public List<IControlProcessor> Processors { get; } = new List<IControlProcessor>();

        public static readonly DependencyProperty BindingObjectProperty =
            DependencyProperty.Register(
                nameof(BindingObject),
                typeof(object),
                typeof(AutoPropertyGrid),
                new PropertyMetadata(null, OnBindingObjectChanged)
            );

        public object BindingObject
        {
            get => GetValue(BindingObjectProperty);
            set => SetValue(BindingObjectProperty, value);
        }

        static AutoPropertyGrid()
        {
            // 关联默认样式
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(AutoPropertyGrid),
                new FrameworkPropertyMetadata(typeof(AutoPropertyGrid))
            );
        }

        public AutoPropertyGrid()
        {
            Generators.Add(new EnumGenerator());
            Generators.Add(new TextBlockGenerator());
            Generators.Add(new TextBoxGenerator());
            Generators.Add(new CheckBoxGenerator());
            Generators.Add(new DatePickerGenerator());
            Generators.Add(new BoolStateGenerator());
            Generators.Add(new NumericRangeGenerator());
            Generators.Add(new TypeGenerator());
        }

        private static void OnBindingObjectChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            var control = (AutoPropertyGrid)d;
            control.UpdatePropertyGrid();
        }

        /// <summary>
        /// 核心逻辑：解析属性并生成TabControl+输入控件
        /// </summary>
        private void UpdatePropertyGrid()
        {
            if (BindingObject == null)
                return;

            var tabControl = GetTemplateChild("PART_TabControl") as TabControl;
            if (tabControl == null)
                throw new InvalidOperationException("模板中未找到PART_TabControl");

            tabControl.Items.Clear();
            // 1. 反射获取所有带Display特性的属性
            var properties = BindingObject
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => Attribute.IsDefined(p, typeof(DisplayAttribute)))
                .ToList();

            // 2. 按Display特性的GroupName分组 排序分组
            var groupedProperties = properties
                .OrderBy(p =>
                {
                    var displayAttr = p.GetCustomAttribute<DisplayAttribute>();

                    return displayAttr?.GetOrder();
                })
                .GroupBy(p =>
                {
                    var displayAttr = p.GetCustomAttribute<DisplayAttribute>();
                    return displayAttr?.GroupName ?? "默认分组";
                })
                .OrderBy(g => g.Key);
            foreach (var group in groupedProperties)
            {
                var tabItem = new TabItem { Header = group.Key };

                // 进行二级分组（GroupAttribute）
                var secondLevelGroups = group
                    .GroupBy(p =>
                    {
                        var attr = p.GetCustomAttribute<GroupAttribute>();
                        return attr?.Name ?? "默认分组";
                    })
                    .OrderBy(g =>
                    {
                        // 技巧：二级分组也可以尝试按组内第一个属性的 Order 排序
                        return g.First().GetCustomAttribute<DisplayAttribute>()?.GetOrder()
                            ?? 10000;
                    });

                // 统一调用二级分组的生成方法
                tabItem.Content = CreateGroupContent(secondLevelGroups);

                tabControl.Items.Add(tabItem);
            }
            // 默认选中第一个Tab
            if (tabControl.Items.Count > 0)
                tabControl.SelectedIndex = 0;
        }

        private UIElement CreateGroupContent(IEnumerable<IGrouping<string, PropertyInfo>> groups)
        {
            var mainPanel = new StackPanel();
            foreach (var group in groups)
            {
                // 1. 创建分组容器（卡片感的核心）
                var groupContainer = new StackPanel { Margin = new Thickness(0, 0, 0, 20) };
                var expander = new ModernExpander
                {
                    IsExpanded = true, // 默认展开
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Margin = new Thickness(0, 0, 0, 10),
                    Icon = FluentIcons.菜单,
                    // Style = (Style)Application.Current.TryFindResource("ModernExpanderStyle")
                };
                expander.Header = group.Key.Equals("默认分组") ? "属性栏" : group.Key;
                // 3. 生成该组内的所有属性行
                var propertiesList = group.ToList();
                var rowsContent = CreateGroupContent(propertiesList);

                groupContainer.Children.Add(rowsContent);
                expander.Content = groupContainer;
                mainPanel.Children.Add(expander);
            }

            return mainPanel;
        }

        /// <summary>
        /// 生成单个分组的内容（标签+输入控件的网格布局）
        /// </summary>
        private UIElement CreateGroupContent(List<PropertyInfo> properties)
        {
            var mainStack = new StackPanel { Margin = new Thickness(10, 5, 10, 5) };
            Grid currentActiveGrid = null;
            int currentUsedWeight = 0;

            foreach (var prop in properties)
            {
                var weight = prop.GetCustomAttribute<GroupAttribute>()?.ColumnWeight ?? 12;

                if (currentActiveGrid == null || (currentUsedWeight + weight) > 12)
                {
                    currentActiveGrid = new Grid
                    {
                        Margin = new Thickness(-12, 0, -12, 5), // 负边距抵消 Cell 的 12px Margin
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                    };
                    // 依然使用 12 列网格系统
                    for (int i = 0; i < 12; i++)
                        currentActiveGrid.ColumnDefinitions.Add(
                            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                        );

                    mainStack.Children.Add(currentActiveGrid);
                    currentUsedWeight = 0;
                }

                var display = prop.GetCustomAttribute<DisplayAttribute>();
                var cell = CreateSinglePropertyCell(prop, display);

                Grid.SetColumn(cell, currentUsedWeight);
                Grid.SetColumnSpan(cell, weight);

                currentActiveGrid.Children.Add(cell);
                currentUsedWeight += weight;
            }
            return mainStack;
        }

        // 辅助方法：生成带 Label 的小盒子
        private UIElement CreateSinglePropertyCell(
            PropertyInfo prop,
            DisplayAttribute display,
            PropertyLayoutDirection direction = PropertyLayoutDirection.Left
        )
        {
            var att = prop.GetCustomAttribute<PropertyItemAttribute>();
            // 1. 基础生产：创建控件
            var control = CreateControl(prop, BindingObject);
            // 2. 初始化容器
            var rootGrid = new Grid(); // 用于承载 Label 和 Control 的容器
            var controlWrapper = new StackPanel(); // 用于承载 Control 和 ErrorTip 的容器

            // 3. 构建上下文
            var context = new ControlContext
            {
                Property = prop,
                BindingSource = BindingObject,
                Control = control,
                WrapPanel = controlWrapper,
                RootCellGrid = rootGrid,
            };

            // 4. 执行流水线处理器 (按需排列顺序)
            var pipeline = new List<IControlProcessor>
            {
                new LayoutProcessor(display),
                new CommandProcessor(),
                new ValidationProcessor(),
                new PermissionProcessor(),
            };
            pipeline.AddRange(Processors); // 用户自定义的处理器
            foreach (var processor in pipeline)
            {
                processor.Execute(context);
            }

            return context.RootCellGrid;
        }

        public FrameworkElement CreateControl(PropertyInfo prop, object bindingSource)
        {
            // 按照优先级排序，寻找第一个匹配的生成器
            var generator = Generators
                .OrderBy(g => g.Priority)
                .FirstOrDefault(g => g.CanProcess(prop, prop.PropertyType));

            if (generator != null)
            {
                return generator.Create(prop, bindingSource);
            }

            // Fallback: 默认返回一个只读的 TextBlock 显示信息
            return new TextBlock
            {
                Text = $"Unsupported: {prop.PropertyType.Name}",
                Foreground = Brushes.Gray,
            };
        }

        // 提供给用户扩展的方法
        public void RegisterGenerator(IControlGenerator generator)
        {
            Generators.Add(generator);
        }

        public void RegisterGenerator(IControlProcessor process)
        {
            Processors.Add(process);
        }
    }
}
