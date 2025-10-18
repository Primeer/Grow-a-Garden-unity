using System;
using UnityEngine;

namespace Game.Scripts.Core.SaveSystem
{
    /// <summary>
    /// Утилиты для безопасной миграции данных сохранений между версиями.
    /// Предоставляет набор методов для модификации JSON структур при обновлении схемы данных.
    /// </summary>
    public static class SaveMigrationUtils
    {
        /// <summary>
        /// Безопасно выполняет миграцию данных с обработкой ошибок и возвратом дефолтного значения при неудаче.
        /// </summary>
        /// <typeparam name="T">Тип возвращаемого значения</typeparam>
        /// <param name="jsonData">JSON строка для обработки</param>
        /// <param name="deserializer">Функция десериализации</param>
        /// <param name="defaultValue">Значение по умолчанию при ошибке</param>
        /// <returns>Десериализованный объект или значение по умолчанию</returns>
        /// <example>
        /// Использовать когда:
        /// - Нужно безопасно конвертировать данные с возможностью ошибки
        /// - Требуется fallback на дефолтные значения при корруптированных данных
        /// - Выполняется сложная миграция, которая может завершиться неудачно
        /// 
        /// Пример: SafeMigrate(oldJson, json => JsonUtility.FromJson&lt;NewData&gt;(json), new NewData())
        /// </example>
        public static T SafeMigrate<T>(string jsonData, Func<string, T> deserializer, T defaultValue = default)
        {
            try
            {
                return deserializer(jsonData);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.LogWarning($"Failed to migrate data, using default value: {defaultValue}");
                return defaultValue;
            }
        }

        /// <summary>
        /// Добавляет новое поле с дефолтным значением в JSON объект, если поле отсутствует.
        /// </summary>
        /// <typeparam name="T">Тип дефолтного значения</typeparam>
        /// <param name="jsonData">Исходная JSON строка</param>
        /// <param name="fieldName">Имя добавляемого поля</param>
        /// <param name="defaultValue">Дефолтное значение для поля</param>
        /// <returns>JSON с добавленным полем или исходный JSON при ошибке</returns>
        /// <example>
        /// Использовать когда:
        /// - В новой версии добавлено поле, которого нет в старых сохранениях
        /// - Нужно обеспечить обратную совместимость при добавлении функций
        /// - Старые данные должны получить разумные дефолтные значения
        /// 
        /// Пример: AddFieldWithDefault(oldJson, "BiomeId", 0) // Добавляет BiomeId: 0
        /// Сценарий: Добавили систему биомов, старые сохранения получают биом по умолчанию
        /// </example>
        public static string AddFieldWithDefault<T>(string jsonData, string fieldName, T defaultValue)
        {
            try
            {
                var tempObj = JsonUtility.FromJson<object>(jsonData);
                var tempJson = JsonUtility.ToJson(tempObj);
                
                if (!tempJson.Contains($"\"{fieldName}\""))
                {
                    var insertIndex = tempJson.LastIndexOf('}');
                    var fieldJson = $",\"{fieldName}\":{JsonUtility.ToJson(defaultValue)}";
                    tempJson = tempJson.Insert(insertIndex, fieldJson);
                }
                
                return tempJson;
            }
            catch
            {
                return jsonData;
            }
        }

        /// <summary>
        /// Удаляет поле из JSON объекта, включая корректную обработку запятых.
        /// </summary>
        /// <param name="jsonData">Исходная JSON строка</param>
        /// <param name="fieldName">Имя удаляемого поля</param>
        /// <returns>JSON без удаленного поля или исходный JSON при ошибке</returns>
        /// <example>
        /// Использовать когда:
        /// - Поле устарело и больше не используется в новой версии
        /// - Нужно очистить структуру от ненужных данных
        /// - Изменилась архитектура и старые поля мешают
        /// 
        /// Пример: RemoveField(oldJson, "DeprecatedField")
        /// Сценарий: Убрали старую систему опыта, удаляем поле "Experience"
        /// </example>
        public static string RemoveField(string jsonData, string fieldName)
        {
            try
            {
                var startPattern = $"\"{fieldName}\":";
                var startIndex = jsonData.IndexOf(startPattern);
                
                if (startIndex == -1)
                    return jsonData;

                var valueStartIndex = startIndex + startPattern.Length;
                var valueEndIndex = FindFieldValueEnd(jsonData, valueStartIndex);
                
                var commaBeforeIndex = startIndex > 0 && jsonData[startIndex - 1] == ',' ? startIndex - 1 : startIndex;
                var commaAfterIndex = valueEndIndex < jsonData.Length - 1 && jsonData[valueEndIndex] == ',' ? valueEndIndex + 1 : valueEndIndex;
                
                return jsonData.Remove(commaBeforeIndex, commaAfterIndex - commaBeforeIndex);
            }
            catch
            {
                return jsonData;
            }
        }

        /// <summary>
        /// Находит конец значения поля в JSON, учитывая вложенные объекты и массивы.
        /// Вспомогательный метод для корректного парсинга JSON структур.
        /// </summary>
        /// <param name="json">JSON строка</param>
        /// <param name="startIndex">Индекс начала значения поля</param>
        /// <returns>Индекс конца значения поля</returns>
        private static int FindFieldValueEnd(string json, int startIndex)
        {
            int depth = 0;
            bool inString = false;
            bool escaped = false;

            for (int i = startIndex; i < json.Length; i++)
            {
                char c = json[i];

                if (escaped)
                {
                    escaped = false;
                    continue;
                }

                if (c == '\\')
                {
                    escaped = true;
                    continue;
                }

                if (c == '"')
                {
                    inString = !inString;
                    continue;
                }

                if (inString)
                    continue;

                if (c == '{' || c == '[')
                {
                    depth++;
                }
                else if (c == '}' || c == ']')
                {
                    depth--;
                }
                else if ((c == ',' || c == '}') && depth == 0)
                {
                    return i;
                }
            }

            return json.Length;
        }

        /// <summary>
        /// Переименовывает поле в JSON объекте.
        /// </summary>
        /// <param name="jsonData">Исходная JSON строка</param>
        /// <param name="oldFieldName">Старое имя поля</param>
        /// <param name="newFieldName">Новое имя поля</param>
        /// <returns>JSON с переименованным полем</returns>
        /// <example>
        /// Использовать когда:
        /// - Переименовали переменную/свойство в коде
        /// - Хотите улучшить naming convention
        /// - Нужно привести к единому стилю именования
        /// 
        /// Пример: RenameField(oldJson, "hp", "Health")
        /// Сценарий: Стандартизируем именование, меняем сокращения на полные названия
        /// </example>
        public static string RenameField(string jsonData, string oldFieldName, string newFieldName)
        {
            return jsonData.Replace($"\"{oldFieldName}\":", $"\"{newFieldName}\":");
        }

        /// <summary>
        /// Конвертирует тип поля в JSON объекте, используя пользовательскую функцию преобразования.
        /// </summary>
        /// <typeparam name="TFrom">Исходный тип поля</typeparam>
        /// <typeparam name="TTo">Целевой тип поля</typeparam>
        /// <param name="jsonData">Исходная JSON строка</param>
        /// <param name="fieldName">Имя конвертируемого поля</param>
        /// <param name="converter">Функция преобразования типов</param>
        /// <returns>JSON с конвертированным полем или исходный JSON при ошибке</returns>
        /// <example>
        /// Использовать когда:
        /// - Изменили тип данных поля (int → float, string → enum и т.д.)
        /// - Нужно преобразовать формат хранения данных
        /// - Реструктуризация требует изменения типов
        /// 
        /// Пример: ConvertFieldType&lt;int, float&gt;(oldJson, "Speed", speed => speed / 100f)
        /// Сценарий: Раньше скорость хранили в int (0-100), теперь в float (0.0-1.0)
        /// </example>
        public static string ConvertFieldType<TFrom, TTo>(string jsonData, string fieldName, Func<TFrom, TTo> converter)
        {
            try
            {
                var pattern = $"\"{fieldName}\":";
                var startIndex = jsonData.IndexOf(pattern);
                
                if (startIndex == -1)
                    return jsonData;

                var valueStartIndex = startIndex + pattern.Length;
                var valueEndIndex = FindFieldValueEnd(jsonData, valueStartIndex);
                
                var oldValueJson = jsonData.Substring(valueStartIndex, valueEndIndex - valueStartIndex);
                var oldValue = JsonUtility.FromJson<TFrom>(oldValueJson);
                var newValue = converter(oldValue);
                var newValueJson = JsonUtility.ToJson(newValue);
                
                return jsonData.Remove(valueStartIndex, valueEndIndex - valueStartIndex)
                              .Insert(valueStartIndex, newValueJson);
            }
            catch
            {
                return jsonData;
            }
        }

        /// <summary>
        /// Создает резервную копию данных сохранения в PlayerPrefs с временной меткой.
        /// </summary>
        /// <param name="saveKey">Ключ сохранения</param>
        /// <param name="data">Данные для резервного копирования</param>
        /// <example>
        /// Использовать когда:
        /// - Перед рискованной миграцией данных
        /// - Нужно обеспечить возможность отката
        /// - Критически важные данные требуют дополнительной защиты
        /// 
        /// Пример: BackupSaveData("plants", oldPlantsData)
        /// Сценарий: Перед масштабной реструктуризацией данных растений создаем бэкап
        /// </example>
        public static void BackupSaveData(string saveKey, string data)
        {
            try
            {
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var backupKey = $"{saveKey}_backup_{timestamp}";
                PlayerPrefs.SetString(backupKey, data);
                PlayerPrefs.Save();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}