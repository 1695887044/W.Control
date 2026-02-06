using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace W.Controls.Attributes
{
    // 非空校验
    public class RequiredAttribute : ValidationBaseAttribute
    {
        public RequiredAttribute() => ErrorMessage = "此项必填";
        public override bool IsValid(object? value) => value != null && !string.IsNullOrWhiteSpace(value.ToString());
    }

    // 正则校验
    public class RegexAttribute : ValidationBaseAttribute
    {
        private readonly string _pattern;
        public RegexAttribute(string pattern) => _pattern = pattern;
        public override bool IsValid(object? value) => value == null || System.Text.RegularExpressions.Regex.IsMatch(value.ToString(), _pattern);
    }
}
