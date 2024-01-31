using System;
using System.Text;
using JsonFx.Json;

namespace EntryPoint.Save
{
    public static class JsonFxSerializer
    {
        public const string TypeHintName = "_c";

        public static readonly JsonConverter[] JsonConverters =
            { };

        public static T Parse<T>(string json, bool hasTypeHint = false)
        {
            return (T) Parse(typeof(T), json, hasTypeHint);
        }

        public static object Parse(Type type, string json, bool hasTypeHint = false)
        {
            var settings = new JsonReaderSettings();
            if (hasTypeHint)
                settings.TypeHintName = TypeHintName;

            foreach (var converter in JsonConverters)
            {
                settings.AddTypeConverter(converter);
            }

            var reader = new JsonReader(json, settings);

            return reader.Deserialize(type);
        }

        public static string Serialize<T>(T settings, bool isPrettyPrint, bool hasTypeHint, bool canSortByFieldName = false)
        {
            var sb = new StringBuilder();
            using (var writer = new JsonWriter(sb))
            {
                if (isPrettyPrint)
                {
                    // убираем табы из JSON
                    writer.Settings.Tab = " ";
                    writer.Settings.PrettyPrint = true;

                    // NOTE: делаем переводы строки платформо-независимыми
                    writer.Settings.NewLine = "\n";
                }
                else
                {
                    writer.Settings.PrettyPrint = false;
                }

                if (hasTypeHint)
                    writer.Settings.TypeHintName = TypeHintName;

#if UNITY_EDITOR || FORCE_DEBUG_MENU
                // NOTE: canSortByFieldName - костыль внутри JsonFX, чтобы можно было получать одинаковый порядок полей на выходе для 
                // сравнения/дебага - может создавать приличный оверхед и в рантайме он ни к чему.
                if (canSortByFieldName && UnityEngine.Application.isPlaying)
                    throw new Exception("SerializeSettingsJson: canSortByFieldName should only be used in editor mode");

#endif

                writer.Settings.CanSortByFieldName = canSortByFieldName;

                foreach (var converter in JsonConverters)
                {
                    writer.Settings.AddTypeConverter(converter);
                }

                writer.Write(settings);
            }

            return sb.ToString();
        }

        public static void RegisterTypeHints()
        {
            // для этих типов форсим отсутствие Type Hint, т.к. для них не бывает наследников и TypeHint избыточен в этом случае
            // TypeCoercionUtility.SkipTypeHintAlias(typeof(UserManager));
            // TypeCoercionUtility.SkipTypeHintAlias(typeof(UserBuilding));
            //
            // // Quest
            // TypeCoercionUtility.SkipTypeHintAlias(typeof(UserQuest));
            // TypeCoercionUtility.RegisterTypeHintAlias("ACQ", typeof(AccumulateCoinsUserQuest));
            // TypeCoercionUtility.RegisterTypeHintAlias("AMQ", typeof(AccumulateManagerCoinsUserQuest));
        }
    }
}