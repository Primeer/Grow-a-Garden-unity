using GamePush;

namespace Game.Scripts.Common.Utilities
{
    public static class LocalizationUtils
    {
        private static Language? m_language;
        
        public static string GetLocalized(string en, string ru)
        {
            m_language ??= GP_Language.Current();
            
            return m_language == Language.Russian ? ru : en;
            // return en;
        }
    }
}