using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Framework
{
    [System.Serializable]
    public class Wrapper<T>
    {
        public List<T> Items;
    }


    [System.Serializable]
    public class KeyValuePair
    {
        public string Key;
        public string Value;

        public KeyValuePair(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }


    [System.Serializable]
    public class DictionaryWrapper
    {
        public List<KeyValuePair> Items;

        public DictionaryWrapper(Dictionary<string, string> dictionary)
        {
            Items = new List<KeyValuePair>();
            foreach (var kvp in dictionary)
            {
                Items.Add(new KeyValuePair(kvp.Key, kvp.Value));
            }
        }
    }

    public static class GoogleSheetHelper
    {
        public static void GetConfig<T>(string sheetId, string gridId, Action<List<T>> callback, string tsvCachePath = null, string jsonCachePath = null)
        {
            GetSheetTSVText(sheetId, gridId, tsvText =>
            {
                TryWriteTextToFile(tsvCachePath, tsvText);
                var json = ConvertTSVTextToJsonListObject(tsvText);
                TryWriteTextToFile(jsonCachePath, json);
                callback?.Invoke(/*JsonConvert.DeserializeObject<List<T>>(json)*/DeserializeJsonToList<T>(json));
            });
        }

        public static List<T> DeserializeJsonToList<T>(string json)
        {
            var array = JsonUtility.FromJson<Wrapper<T>>(WrapArray(json)).Items;
            return new List<T>(array);
        }


        private static string WrapArray(string json)
        {
            return "{\"Items\":" + json + "}";
        }


        public static void GetSheetTSVText(string sheetId, string gridId, Action<string> callback)
        {
            string url = $@"https://docs.google.com/spreadsheets/d/{sheetId}/export?gid={gridId}&format=tsv";
            LoadTextFromWeb(url, callback);
        }

        private static void LoadTextFromWeb(string url, Action<string> callBack)
        {
            WWW request = new WWW(url);
            while (!request.isDone)
            {
                request.MoveNext();
            }
            callBack?.Invoke(request.text);
        }

        public static string ConvertTSVTextToJsonListObject(string tsvText)
        {
            var tsv = new List<string[]>();
            var lines = tsvText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            foreach (string line in lines)
                tsv.Add(line.Split('\t'));

            var properties = lines[0].Split('\t');
            var listObjResult = new List<Dictionary<string, string>>();

            for (int i = 1; i < lines.Length; i++)
            {
                var objResult = new Dictionary<string, string>();
                for (int j = 0; j < properties.Length; j++)
                {
                    var cellText = tsv[i][j];
                    if (!string.IsNullOrEmpty(cellText))
                        objResult.Add(properties[j], tsv[i][j]);
                }

                listObjResult.Add(objResult);
            }

            return ConvertToJson(listObjResult);//JsonConvert.SerializeObject(listObjResult);
        }


        public static string ConvertToJson(List<Dictionary<string, string>> data)
        {
            var jsonBuilder = new StringBuilder();
            jsonBuilder.Append("[");

            for (int i = 0; i < data.Count; i++)
            {
                var dictionary = data[i];
                jsonBuilder.Append("{");

                int j = 0;
                foreach (var kvp in dictionary)
                {
                    jsonBuilder.Append($"\"{EscapeString(kvp.Key)}\":\"{EscapeString(kvp.Value)}\"");

                    if (j < dictionary.Count - 1)
                    {
                        jsonBuilder.Append(",");
                    }
                    j++;
                }

                jsonBuilder.Append("}");

                if (i < data.Count - 1)
                {
                    jsonBuilder.Append(",");
                }
            }

            jsonBuilder.Append("]");
            return jsonBuilder.ToString();
        }


        private static string EscapeString(string str)
        {
            return str.Replace("\"", "\\\"");
        }


        private static void TryWriteTextToFile(string path, string text)
        {
            if (string.IsNullOrEmpty(path))
            {
                Logger.Log(text);
                return;
            }

            if (!File.Exists(path))
            {
                var fs = new FileStream(path, FileMode.Create);
                fs.Dispose();
            }

            File.WriteAllText(path, text);
        }
    }
}
