namespace _SpellboundHollow.Scripts.Core
{
    // Это статический класс, ему не нужно висеть на объекте в сцене.
    // Он просто предоставляет удобный инструмент.
    public static class TextFormatter
    {
        // Пока что у нас нет системы выбора персонажа, так что сделаем временную заглушку.
        // TODO: Заменить эту переменную на реальную проверку пола персонажа из GameManager.
        private static bool _isPlayerFemale = true; 

        /// <summary>
        /// Форматирует строку, заменяя токены вида {женский/мужской} на правильный вариант.
        /// </summary>
        public static string Format(string text)
        {
            // Здесь мы можем добавить обработку и других токенов, например, имени игрока.
            return ProcessGenderTokens(text);
        }

        private static string ProcessGenderTokens(string text)
        {
            // Ищем все вхождения фигурных скобок
            while (text.Contains("{") && text.Contains("}"))
            {
                int startIndex = text.IndexOf('{');
                int endIndex = text.IndexOf('}');
                
                // Вырезаем содержимое скобок, например, "внучка/внук"
                string tokenContent = text.Substring(startIndex + 1, endIndex - startIndex - 1);
                
                // Разделяем по слэшу
                string[] options = tokenContent.Split('/');
                
                // Выбираем нужный вариант в зависимости от пола
                string replacement = _isPlayerFemale ? options[0] : options[1];
                
                // Заменяем весь токен {внучка/внук} на выбранное слово
                text = text.Replace("{" + tokenContent + "}", replacement);
            }
            return text;
        }
    }
}