using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace W.Controls.Controls
{
    [TemplatePart(Name = "PART_HourList", Type = typeof(ListBox))]
    [TemplatePart(Name = "PART_MinuteList", Type = typeof(ListBox))]
    [TemplatePart(Name = "PART_SecondList", Type = typeof(ListBox))]
    public class TimePicker : Control
    {
        private ListBox _hourList;
        private ListBox _minuteList;
        private ListBox _secondList;
        private bool _isInternalUpdating = false; // 防止循环触发

        static TimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimePicker), new FrameworkPropertyMetadata(typeof(TimePicker)));
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(DateTime), typeof(TimePicker),
                new FrameworkPropertyMetadata(DateTime.Now, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        public DateTime Value
        {
            get => (DateTime)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public bool IsDropDownOpen
        {
            get => (bool)GetValue(IsDropDownOpenProperty);
            set => SetValue(IsDropDownOpenProperty, value);
        }

        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(TimePicker), new PropertyMetadata(false));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _hourList = GetTemplateChild("PART_HourList") as ListBox;
            _minuteList = GetTemplateChild("PART_MinuteList") as ListBox;
            _secondList = GetTemplateChild("PART_SecondList") as ListBox;

            // 1. 初始化数据源
            if (_hourList != null) _hourList.ItemsSource = Enumerable.Range(0, 24).Select(i => i.ToString("D2")).ToList();
            if (_minuteList != null) _minuteList.ItemsSource = Enumerable.Range(0, 60).Select(i => i.ToString("D2")).ToList();
            if (_secondList != null) _secondList.ItemsSource = Enumerable.Range(0, 60).Select(i => i.ToString("D2")).ToList();

            // 2. 绑定选中事件
            if (_hourList != null) _hourList.SelectionChanged += OnSelectionChanged;
            if (_minuteList != null) _minuteList.SelectionChanged += OnSelectionChanged;
            if (_secondList != null) _secondList.SelectionChanged += OnSelectionChanged;

            UpdateInternalSelection();
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TimePicker tp) tp.UpdateInternalSelection();
        }

        // 内部选中状态同步到 Value
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isInternalUpdating || _hourList?.SelectedItem == null || _minuteList?.SelectedItem == null || _secondList?.SelectedItem == null) return;

            int h = int.Parse(_hourList.SelectedItem.ToString());
            int m = int.Parse(_minuteList.SelectedItem.ToString());
            int s = int.Parse(_secondList.SelectedItem.ToString());

            _isInternalUpdating = true;
            Value = new DateTime(Value.Year, Value.Month, Value.Day, h, m, s);
            _isInternalUpdating = false;
        }

        // Value 同步到内部选中状态
        private void UpdateInternalSelection()
        {
            if (_isInternalUpdating || _hourList == null) return;

            _isInternalUpdating = true;

            _hourList.SelectedItem = Value.Hour.ToString("D2");
            _minuteList.SelectedItem = Value.Minute.ToString("D2");
            _secondList.SelectedItem = Value.Second.ToString("D2");

            // 优雅的滚动：让选中的数字滚到视觉正中央
            ScrollToCenter(_hourList);
            ScrollToCenter(_minuteList);
            ScrollToCenter(_secondList);

            _isInternalUpdating = false;
        }
        private void ScrollToCenter(ListBox lb)
        {
            if (lb == null || lb.SelectedItem == null) return;

            // 解决“切换流畅”的关键：
            // 不要直接跳，而是稍微给一点缓冲时间让渲染完成
            Dispatcher.BeginInvoke(new Action(() =>
            {
                // 找到 ScrollViewer
                var scrollViewer = GetVisualChild<ScrollViewer>(lb);
                if (scrollViewer != null)
                {
                    // 计算目标偏移量（每一项高度是45）
                    int index = lb.SelectedIndex;
                    double targetOffset = index * 45;

                    // 使用平滑滚动（如果你的环境支持，否则 ScrollIntoView 也可以）
                    lb.ScrollIntoView(lb.SelectedItem);
                }
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        // 辅助方法：获取内部滚动条
        private T GetVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T t) return t;
                var result = GetVisualChild<T>(child);
                if (result != null) return result;
            }
            return null;
        }
    }
}
