using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GTIFramework.Common
{
    /// <summary>
    /// 필수 입력 항목 유효성 검사를 위한 Validator
    /// </summary>
    public class RequiredValidator : ValidationRule
    {
        /// <summary>
        /// 값에 대한 유효성 검사
        /// </summary>
        /// <param name="value">유효성 검사 대상 값</param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()) || string.IsNullOrWhiteSpace(value.ToString()))
                return new ValidationResult(false, "필수입력항목입니다.");

            return ValidationResult.ValidResult;
        }
    }
}
