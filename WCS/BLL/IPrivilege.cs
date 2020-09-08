using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Privilege
{
    public interface IPrivilege
    {
        
    }

    public static class Authentication
    {
        public static void Event<T>(this T example,object sender, RoutedEventArgs e) where T: IPrivilege
        {
            
        }
        public static string AuthenticationCheck(string funcId)
        {
            string errText = string.Empty;
            return errText;
        }
    }

    public struct Privilege
    {
        string _privilegeId;
        string _funcId;
        string _description;
    }
}
