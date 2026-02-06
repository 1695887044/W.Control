using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using W.Controls.Attributes;

namespace W.Controls
{
    public class ExampleModel
    {
        public ICommand OperatorCommand { get; set; }

        #region 运行参数组
        [Display(Name = "输入图像源", GroupName = "运行参数", Description = "选择待处理的图像来源")]
        public string InputImageSource { get; set; } = "本地文件";

        [Display(
            Name = "ROI范围",
            GroupName = "运行参数",
            Description = "感兴趣区域（格式：X1,Y1,X2,Y2"
        )]
        public string RoiRange { get; set; } = "0,0,1000,800";

        [Display(Name = "处理优先级", GroupName = "运行参数", Description = "任务处理的优先级")]
        public ProcessPriority Priority { get; set; } = ProcessPriority.Normal;

        [Group(Name = "按钮区1", ColumnWeight = 4)]
        [Display(Name = "按钮1", GroupName = "运行参数", Description = "任务处理的优先级")]
        public bool Button1 { get; set; }

        [Group(Name = "按钮区1", ColumnWeight = 4)]
        [Display(Name = "按钮2", GroupName = "运行参数", Description = "任务处理的优先级")]
        public bool Button3 { get; set; }

        [Group(Name = "按钮区1", ColumnWeight = 4)]
        [Display(Name = "按钮3", GroupName = "运行参数", Description = "任务处理的优先级")]
        public bool Button4 { get; set; }

        [Group(Name = "按钮区1", ColumnWeight = 6)]
        [Range(0, 255)]
        [ReadOnly(true)]
        [Display(Name = "按钮4", GroupName = "运行参数", Description = "任务处理的优先级")]
        public bool Button2 { get; set; }
        [Group(Name = "按钮区1", ColumnWeight = 6)]
        [Range(0, 255)]
        [ReadOnly(true)]
        [Display(Name = "按钮14", GroupName = "运行参数", Description = "任务处理的优先级")]
        public bool Butto2n2 { get; set; }

        [Group(Name = "按钮区2")]
        [Display(Name = "按钮1", GroupName = "运行参数", Description = "任务处理的优先级")]
        public bool Butto3 { get; set; }

        [Group(Name = "按钮区2")]
        [Display(Name = "按钮2", GroupName = "运行参数", Description = "任务处理的优先级")]
        public bool But4n2 { get; set; }

        [Group(Name = "自定义控件")]
        [PropertyItem(typeof(UserControl1))]
        [Display(Name = "按钮2", GroupName = "自定义", Description = "任务处理的优先级")]
        public string Cust { get; set; }

        #endregion

        #region 基本参数组
        [NumericRange(0,255,5)]
        [Display(Name = "对比度", GroupName = "基本参数", Description = "图像对比度（0-200）")]
        public int Contrast { get; set; } = 100;

        [Display(Name = "亮度", GroupName = "基本参数", Description = "图像亮度（0-200）")]
        public int Brightness { get; set; } = 100;

        [Command(
            Command = nameof(OperatorCommand),
            CommandParam = "A",
            AncestorType = typeof(Window),
            Mode = System.Windows.Data.RelativeSourceMode.FindAncestor
        )]
        [ReadOnly(false)]
        [Display(
            Name = "是否灰度化",
            GroupName = "基本参数",
            Description = "处理前是否将图像转为灰度图"
        )]
        public bool IsGrayscale { get; set; } = true;
        #endregion

        #region 流程控制组
        [Display(Name = "执行方式", GroupName = "流程控制", Description = "任务执行的方式")]
        public ExecutionMode ExecutionMode { get; set; } = ExecutionMode.Sequential;

        [Display(
            Name = "执行延迟",
            GroupName = "流程控制",
            Description = "任务执行前的延迟时间（毫秒）"
        )]
        public int ExecutionDelay { get; set; } = 0;

        [Display(Name = "重试次数", GroupName = "流程控制", Description = "任务失败后的重试次数")]
        public int RetryCount { get; set; } = 2;
        #endregion
    }

    /// <summary>
    /// 处理优先级枚举
    /// </summary>
    public enum ProcessPriority
    {
        [Display(Name = "低")]
        Low,

        [Display(Name = "正常")]
        Normal,

        [Display(Name = "高")]
        High,

        [Display(Name = "最高")]
        Highest,
    }

    /// <summary>
    /// 执行方式枚举
    /// </summary>
    public enum ExecutionMode
    {
        [Display(Name = "串行执行")]
        Sequential,

        [Display(Name = "并行执行")]
        Parallel,

        [Display(Name = "异步执行")]
        Async,
    }
}
