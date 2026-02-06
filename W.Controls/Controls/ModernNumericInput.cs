using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace W.Controls.Controls
{
    public class ModernNumericInput : Control
    {
        private DispatcherTimer _repeatTimer;
        private bool _isIncrementing;

        static ModernNumericInput()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ModernNumericInput),
                new FrameworkPropertyMetadata(typeof(ModernNumericInput)));
        }

        public ModernNumericInput()
        {
            _repeatTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            _repeatTimer.Tick += (s, e) => {
                if (_repeatTimer.Interval.TotalMilliseconds > 30) // 加速效果
                    _repeatTimer.Interval = TimeSpan.FromMilliseconds(_repeatTimer.Interval.TotalMilliseconds * 0.9);
                ChangeValue(_isIncrementing);
            };
        }

        #region Dependency Properties
        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(ModernNumericInput),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, CoerceValue));

        public double Minimum { get; set; } = 0;
        public double Maximum { get; set; } = 100;
        public double Increment { get; set; } = 1;
        #endregion

        private static object CoerceValue(DependencyObject d, object baseValue)
        {
            var ctrl = (ModernNumericInput)d;
            double val = (double)baseValue;
            return Math.Max(ctrl.Minimum, Math.Min(ctrl.Maximum, val));
        }

        private void ChangeValue(bool increment) => Value += (increment ? 1 : -1) * Increment;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var upBtn = GetTemplateChild("PART_UpButton") as Button;
            var downBtn = GetTemplateChild("PART_DownButton") as Button;

            if (upBtn != null) SetupRepeatButton(upBtn, true);
            if (downBtn != null) SetupRepeatButton(downBtn, false);
        }

        private void SetupRepeatButton(Button btn, bool increment)
        {
            btn.PreviewMouseDown += (s, e) => {
                _isIncrementing = increment;
                ChangeValue(increment);
                _repeatTimer.Interval = TimeSpan.FromMilliseconds(400); // 初始延迟
                _repeatTimer.Start();
            };
            btn.PreviewMouseUp += (s, e) => _repeatTimer.Stop();
            btn.MouseLeave += (s, e) => _repeatTimer.Stop();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Up) { ChangeValue(true); e.Handled = true; }
            if (e.Key == Key.Down) { ChangeValue(false); e.Handled = true; }
            base.OnKeyDown(e);
        }
    }
}
