# Система версионирования сохранений

## Принципы работы версионирования

### Основная концепция
Система версионирования позволяет обновлять структуру сохранений без потери данных игроков. При изменении формата данных старые сохранения автоматически мигрируют к новой версии через цепочку конвертеров.

### Архитектура версионирования

#### 1. Структура версионированных данных
```csharp
[Serializable]
public struct VersionedSaveData
{
    public int Version;
    public string JsonData;
}
```

#### 2. Базовый интерфейс для версионированных Saver'ов
```csharp
public interface IVersionedSaver
{
    int CurrentVersion { get; }
    string MigrateFromVersion(string jsonData, int fromVersion, int toVersion);
    string SerializeData(object data);
    object DeserializeData(string jsonData, int version);
}
```

#### 3. Абстрактный базовый класс
```csharp
public abstract class VersionedSaverBase<T> : IVersionedSaver
{
    public abstract int CurrentVersion { get; }
    
    protected readonly Dictionary<int, Func<string, string>> m_migrators = new();
    
    protected void RegisterMigrator(int fromVersion, Func<string, string> migrator)
    {
        m_migrators[fromVersion] = migrator;
    }
    
    public string MigrateFromVersion(string jsonData, int fromVersion, int toVersion)
    {
        string currentData = jsonData;
        
        for (int version = fromVersion; version < toVersion; version++)
        {
            if (m_migrators.TryGetValue(version, out var migrator))
            {
                currentData = migrator(currentData);
            }
            else
            {
                Debug.LogWarning($"No migrator found for version {version} to {version + 1}");
            }
        }
        
        return currentData;
    }
    
    public abstract string SerializeData(object data);
    public abstract object DeserializeData(string jsonData, int version);
}
```

### Типы миграций

#### 1. Добавление полей (Backward compatible)
- Новые поля добавляются со значениями по умолчанию
- Старые сохранения автоматически получают дефолтные значения

#### 2. Удаление полей (Breaking change)
- Удаленные поля игнорируются при загрузке
- Требуется миграция для сохранения важных данных

#### 3. Изменение типов данных (Breaking change)
- Конвертация типов через специальные миграторы
- Обработка ошибок конвертации

#### 4. Реструктуризация данных (Breaking change)
- Изменение иерархии объектов
- Перенос данных между структурами

## Шаги реализации версионирования

### Этап 1: Создание базовой инфраструктуры

1. **Создать SaveVersion.cs** - константы версий
2. **Создать VersionedSaverBase.cs** - базовый класс
3. **Создать SaveMigrationUtils.cs** - утилиты миграции
4. **Создать SaveVersionManager.cs** - менеджер версий

### Этап 2: Рефакторинг существующих Saver'ов

1. **PlantsSaver**:
   - Наследовать от VersionedSaverBase
   - Добавить CurrentVersion = 1
   - Обернуть Data в VersionedSaveData

2. **MoneySaver**:
   - Добавить версионирование для int данных
   - CurrentVersion = 1

3. **InventorySaver**:
   - Наследовать от VersionedSaverBase
   - CurrentVersion = 1

### Этап 3: Реализация миграции

1. **Создать миграторы для каждого Saver'а**:
   ```csharp
   protected override void RegisterMigrators()
   {
       RegisterMigrator(1, MigrateFrom1To2);
       RegisterMigrator(2, MigrateFrom2To3);
   }
   ```

2. **Реализовать методы миграции**:
   ```csharp
   private string MigrateFrom1To2(string jsonData)
   {
       var oldData = JsonUtility.FromJson<DataV1>(jsonData);
       var newData = new DataV2
       {
           // Конвертация старых данных
       };
       return JsonUtility.ToJson(newData);
   }
   ```

### Этап 4: Обработка загрузки

1. **Определение версии сохранения**:
   ```csharp
   private int GetSaveVersion(string jsonData)
   {
       try
       {
           var versionedData = JsonUtility.FromJson<VersionedSaveData>(jsonData);
           return versionedData.Version;
       }
       catch
       {
           return 1; // Старые сохранения без версии
       }
   }
   ```

2. **Автоматическая миграция при загрузке**:
   ```csharp
   private string LoadAndMigrate()
   {
       string rawData = GP_Player.GetString(SAVE_KEY);
       int saveVersion = GetSaveVersion(rawData);
       
       if (saveVersion < CurrentVersion)
       {
           rawData = MigrateFromVersion(rawData, saveVersion, CurrentVersion);
           // Опционально: пересохранить мигрированные данные
       }
       
       return rawData;
   }
   ```

### Этап 5: Обработка ошибок

1. **Fallback стратегии**:
   - Сброс к значениям по умолчанию при критических ошибках
   - Логирование проблем миграции
   - Бэкап старых сохранений

2. **Валидация данных**:
   - Проверка целостности после миграции
   - Санитизация некорректных значений

### Этап 6: Тестирование

1. **Unit тесты для миграторов**
2. **Интеграционные тесты для полного цикла**
3. **Тестирование на реальных сохранениях разных версий**

## Пример использования

### Обновление PlantsSaver с версии 1 до 2

**Версия 1 (текущая)**:
```csharp
public struct Data
{
    public List<PlantData> Plants;
    public long SaveTimeTicks;
}
```

**Версия 2 (добавляем биомы)**:
```csharp
public struct DataV2
{
    public List<PlantData> Plants;
    public long SaveTimeTicks;
    public int BiomeId; // Новое поле
}
```

**Миграция**:
```csharp
private string MigrateFrom1To2(string jsonData)
{
    var oldData = JsonUtility.FromJson<Data>(jsonData);
    var newData = new DataV2
    {
        Plants = oldData.Plants,
        SaveTimeTicks = oldData.SaveTimeTicks,
        BiomeId = 0 // Значение по умолчанию
    };
    return JsonUtility.ToJson(newData);
}
```

## Преимущества системы

1. **Безопасность данных** - игроки не теряют прогресс при обновлениях
2. **Гибкость разработки** - можно свободно изменять структуры данных
3. **Автоматизация** - миграция происходит прозрачно для игрока
4. **Отслеживаемость** - каждое изменение документируется через версии
5. **Откат** - возможность вернуться к предыдущим версиям при проблемах

## Рекомендации

1. **Всегда увеличивайте версию** при изменении структуры данных
2. **Документируйте изменения** в каждой версии
3. **Тестируйте миграции** на реальных данных
4. **Сохраняйте обратную совместимость** где это возможно
5. **Используйте семантическое версионирование** для критичных изменений