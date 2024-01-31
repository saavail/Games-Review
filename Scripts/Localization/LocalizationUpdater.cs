using UnityEngine;

namespace Localization
{
    public class LocalizationUpdater : MonoBehaviour
    {
        public string docId;
        public int gid;
        public string resourcesPath = "Archer/Resources/";
        public string resourcesLocalizationFileName = "Strings.txt";    
        public string resourcesVersionFileName = "StringsVersion.txt";

        public string localizationStringsPath = "";
    }
}
