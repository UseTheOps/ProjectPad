using ProjectPad.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPadUWP
{
    public static class LocalizationHelper
    {
        public static string FormatNameForSettings(string name)
        {
            return string.Format("Accèder aux paramètres, vous êtes connectés en tant que {0}", name);
        }

        public static string FormatRecentProject(RecentProject prj)
        {
            if (prj == null)
                return "erreur";
            return FormatRecentProject(prj.Name, prj.LastChange);
        }
        public static string FormatRecentProject(string name, DateTime lastChange)
        {
            return string.Format("{0}, dernier accès le {1:dd MMM HH:mm}", name, lastChange);
        }
    }
}
