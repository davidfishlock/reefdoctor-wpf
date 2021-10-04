using ReefDoctorId.ViewModels;
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Controls;

namespace ReefDoctorId.WPF.Controls
{
    /// <summary>
    /// Interaction logic for PasswordDialog.xaml
    /// </summary>
    public partial class PasswordDialog : OverlayBase
    {
        public PasswordDialog()
        {
            InitializeComponent();
        }

        public string SecureStringToString(SecureString value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is MainViewModel vm)
            {
                var pass = (PasswordBox)sender;
                var passLength = pass.SecurePassword.Length;
                if (passLength > 0)
                {
                    vm.CurrentPassword = SecureStringToString(((PasswordBox)sender).SecurePassword);
                }
            }
        }
    }
}
