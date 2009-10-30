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
    [ActiveRecord]
    public sealed class Language : ActiveRecord<Language>
    {
        [ActiveRecord]
        private sealed class LanguageEntity : ActiveRecord<LanguageEntity>
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
            Entities = new LazyList<LanguageEntity>();
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
                            _instance = SelectFirst();
                            if (_instance == null)
                            {
                                _instance = new Language();
                                _instance.Save();
                            }
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

        public IEnumerable<string> LanguageKeys
        {
            get
            {
                Dictionary<string, bool> sentItems = new Dictionary<string, bool>();
                foreach (LanguageEntity idx in Entities)
                {
                    if (!sentItems.ContainsKey(idx.Language))
                    {
                        sentItems[idx.Language] = true;
                        yield return idx.Language;
                    }
                }
            }
        }

        public void SetLanguageForUser(string language)
        {
            HttpContext.Current.Session["LanguageRecords.Language.UserLanguage"] = language;
        }

        public string UserLanguage
        {
            get
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
            set 
            {
                HttpContext.Current.Session["LanguageRecords.Language.UserLanguage"] = value;
            }
        }

        public string this[string key]
        {
            get
            {
                LanguageEntity retVal = Entities.Find(
                    delegate(LanguageEntity idx)
                        {
                            return idx.Key == key && UserLanguage == idx.Language;
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
                LanguageEntity le = new LanguageEntity();
                le.Key = key;
                le.Value = key;
                le.Language = "en";
                Entities.Add(le);
                Save();
                return le.Value;
            }
        }

        public string this[string key, string lang]
        {
            get
            {
                LanguageEntity retVal = Entities.Find(
                    delegate(LanguageEntity idx)
                        {
                            return idx.Key == key && lang == idx.Language;
                        });
                if (retVal != null)
                {
                    return retVal.Value;
                }
                return null;
            }
        }

        public void SetDefaultValue(string key, string defaultValue)
        {
            LanguageEntity retVal = Entities.Find(
                delegate(LanguageEntity idx)
                {
                    return idx.Key == key && UserLanguage == idx.Language;
                });
            if (retVal != null)
            {
                return;
            }
            LanguageEntity le = new LanguageEntity
            {
                Key = key,
                Value = defaultValue,
                Language = UserLanguage
            };
            Entities.Add(le);
            Save();
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
                Save();
                return le.Value;
            }
        }

        public void ChangeValue(string key, string language, string value)
        {
            ChangeValue(key, language, value, false);
        }

        public void ChangeValue(string key, string language, string value, bool skipSave)
        {
            LanguageEntity le = Entities.Find(
                delegate(LanguageEntity idx)
                    {
                        return idx.Key == key && idx.Language == (language == null ? UserLanguage : language);
                    });
            if (le != null)
            {
                le.Value = value;
                if (!skipSave)
                    le.Save();
            }
            else
            {
                LanguageEntity le2 = new LanguageEntity();
                le2.Key = key;
                le2.Language = language == null ? UserLanguage : language;
                le2.Value = value;
                Entities.Add(le2);
                if (!skipSave)
                    Save();
            }
        }

        [ActiveField]
        private LazyList<LanguageEntity> Entities { get; set; }

        public override void Save()
        {
            foreach (LanguageEntity idx in Entities)
            {
                idx.Prepare();
            }
            base.Save();
        }
    }
}