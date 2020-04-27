using DevExpress.Xpf.Editors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GTIFramework.Common
{
    /// <summary>
    /// Validation Helper 클래스
    /// </summary>
    public class ValidationHelper
    {
        /// <summary>
        /// 컨트롤에 대해 유효성 검사를 한다.
        /// </summary>
        /// <param name="control">대상 컨트롤</param>
        /// <param name="rule">적용할 Validation 규칙</param>
        /// <param name="msg">보여줄 메시지</param>
        /// <returns></returns>
        public static bool Validate(Control control, ValidationRule rule, string msg)
        {
            bool isValid = true;
            DependencyProperty dp = null;
            object controlVal = null;

            try
            {
                if (control is TextEdit)
                {
                    dp = TextEdit.TextProperty;
                    controlVal = (control as TextEdit).DisplayText;
                }
                else if (control is PasswordBoxEdit)
                {
                    dp = PasswordBoxEdit.TextProperty;
                    controlVal = (control as PasswordBoxEdit).Text;
                }
                else if (control is ComboBoxEdit)
                {
                    dp = ComboBoxEdit.SelectedItemValueProperty;
                    controlVal = (control as ComboBoxEdit).DisplayText;
                }
                else if (control is SpinEdit)
                {
                    dp = SpinEdit.TextProperty;
                    controlVal = (control as TextEdit).Text;
                }
                else if (control is DateEdit)
                {
                    dp = DateEdit.TextProperty;
                    controlVal = (control as DateEdit).Text;
                }
                else
                {
                    return true;
                }

                Validation.ClearInvalid(control.GetBindingExpression(dp));

                ValidationError validationError = new ValidationError(rule, control.GetBindingExpression(dp));
                ValidationResult result = validationError.RuleInError.Validate(controlVal, new System.Globalization.CultureInfo("ko-KR"));
                validationError.ErrorContent = msg;

                if (!result.IsValid)
                    Validation.MarkInvalid(control.GetBindingExpression(dp), validationError);

                isValid = result.IsValid;
            }
            catch (Exception)
            {
                throw null;
            }

            return isValid;
        }


        /// <summary>
        /// Validate 초기화
        /// </summary>
        /// <param name="control">대상 컨트롤</param>
        /// <param name="rule">적용할 Validation 규칙</param>
        /// <param name="msg">보여줄 메시지</param>
        /// <returns></returns>
        public static bool InitValidate(Control control, ValidationRule rule)
        {
            bool isValid = true;
            DependencyProperty dp = null;
            object controlVal = null;

            try
            {
                if (control is TextEdit)
                {
                    dp = TextEdit.TextProperty;
                    controlVal = (control as TextEdit).DisplayText;
                }
                else if (control is PasswordBoxEdit)
                {
                    dp = PasswordBoxEdit.TextProperty;
                    controlVal = (control as PasswordBoxEdit).Text;
                }
                else if (control is ComboBoxEdit)
                {
                    dp = ComboBoxEdit.SelectedItemValueProperty;
                    controlVal = (control as ComboBoxEdit).DisplayText;
                }
                else if (control is SpinEdit)
                {
                    dp = SpinEdit.TextProperty;
                    controlVal = (control as TextEdit).Text;
                }
                else if (control is DateEdit)
                {
                    dp = DateEdit.TextProperty;
                    controlVal = (control as DateEdit).Text;
                }
                else
                {
                    return true;
                }

                Validation.ClearInvalid(control.GetBindingExpression(dp));

                ValidationError validationError = new ValidationError(rule, control.GetBindingExpression(dp));
                ValidationResult result = validationError.RuleInError.Validate("1", new System.Globalization.CultureInfo("ko-KR"));

                if (!result.IsValid)
                    Validation.MarkInvalid(control.GetBindingExpression(dp), validationError);

                isValid = result.IsValid;
            }
            catch (Exception)
            {
                throw;
            }

            return isValid;
        }
    }
}
