/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using Ra.Brix.Data;
using Ra.Brix.Types;
using System.Web;
using System.Collections.Generic;

namespace LanguageRecords
{
    public sealed class Language
    {
        [ActiveType]
        private sealed class LanguageEntity : ActiveType<LanguageEntity>
        {
            [ActiveField]
            public string Key { get; set; }

            [ActiveField]
            public string Value { get; set; }

            [ActiveField]
            public string Language { get; set; }

            internal void Prepare()
            {
                Value = Value.Replace("\r\n\r\n", "<br/><br/>").Replace("\n\n", "<br/><br/>");
            }
        }

        private static Language _instance;

        private Language()
        {
            Entities = new List<LanguageEntity>(LanguageEntity.Select());
        }

        public static void Reset()
        {
            _instance = null;
        }

        public static Language Instance
        {
            get 
            {
                if (_instance == null)
                {
                    lock (typeof(Language))
                    {
                        if (_instance == null)
                        {
                            _instance = new Language();
                        }
                    }
                }
                return _instance;
            }
        }

        public IEnumerable<string> Keys
        {
            get 
            {
                Dictionary<string, bool> sentItems = new Dictionary<string, bool>();
                foreach (LanguageEntity idx in Entities)
                {
                    if (!sentItems.ContainsKey(idx.Key))
                    {
                        sentItems[idx.Key] = true;
                    }
                }
                foreach (string idx in sentItems.Keys)
                {
                    yield return idx;
                }
            }
        }

        public void SetLanguageForUser(string language)
        {
            HttpContext.Current.Session["LanguageRecords.Language.UserLanguage"] = language;
        }

        private string _language = "en";
        public string UserLanguage
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return _language;
                }
                else
                {
                    string defaultLanguage = HttpContext.Current.Request.UserLanguages != null &&
                        HttpContext.Current.Request.UserLanguages[0].Length > 0 ?
                        HttpContext.Current.Request.UserLanguages[0] :
                                                                                                                                                          "en";
                    if (HttpContext.Current == null)
                        return "en";
                    return HttpContext.Current.Session["LanguageRecords.Language.UserLanguage"] == null ? 
                        defaultLanguage :
                        HttpContext.Current.Session["LanguageRecords.Language.UserLanguage"].ToString();
                }
            }
            set 
            {
                if (HttpContext.Current == null)
                {
                    _language = value;
                }
                else
                {
                    HttpContext.Current.Session["LanguageRecords.Language.UserLanguage"] = value;
                }
            }
        }

        public void SetDefaultValue(string key, string defaultValue)
        {
            LanguageEntity retVal = Entities.Find(
                delegate(LanguageEntity idx)
                {
                    return idx.Key == key && idx.Language == "en";
                });
            if (retVal != null)
            {
                return;
            }
            LanguageEntity le = new LanguageEntity
            {
                Key = key,
                Value = defaultValue,
                Language = "en"
            };
            le.Save();
            Entities.Add(le);
        }

        public string this[string key]
        {
            get
            {
                LanguageEntity retVal = Entities.Find(
                    delegate(LanguageEntity idx)
                    {
                        return idx.Key == key && idx.Language == UserLanguage;
                    });
                if (retVal != null)
                {
                    return retVal.Value;
                }
                retVal = Entities.Find(
                    delegate(LanguageEntity idx)
                    {
                        return idx.Key == key && idx.Language == "en";
                    });
                if (retVal != null)
                {
                    return retVal.Value;
                }
                return key;
            }
        }

        public string this[string key, string language, string defaultValue]
        {
            get
            {
                LanguageEntity retVal = Entities.Find(
                    delegate(LanguageEntity idx)
                        {
                            return idx.Key == key && (language ?? UserLanguage) == idx.Language;
                        });
                if (retVal != null)
                {
                    return retVal.Value;
                }
                LanguageEntity le = new LanguageEntity
                {
                    Key = key,
                    Value = defaultValue,
                    Language = language ?? UserLanguage
                };
                Entities.Add(le);
                le.Save();
                return le.Value;
            }
        }

        public void ChangeValue(string key, string language, string value)
        {
            LanguageEntity le = Entities.Find(
                delegate(LanguageEntity idx)
                {
                    return idx.Key == key && idx.Language == (language == null ? UserLanguage : language);
                });
            if (le != null)
            {
                le.Value = value;
                le.Save();
            }
            else
            {
                LanguageEntity le2 = new LanguageEntity();
                le2.Key = key;
                le2.Language = language == null ? UserLanguage : language;
                le2.Value = value;
                Entities.Add(le2);
                le2.Save();
            }
        }

        private List<LanguageEntity> Entities { get; set; }
    }
}