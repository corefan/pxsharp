using System;
using System.Text.RegularExpressions;

namespace PxSharp.ApiGen {
    public abstract class Base<T> where T : Base<T> {
        public Regex Regex;
        public Match Match;
        public Context Context;
        public Action<T> OnMatched;
        public Action<T> OnParsingDone;
        public Func<T, string> NativeNameResolver;

        public string NativeName {
            get { return NativeNameResolver((T) this); }
        }

        public string TempName {
            get { return NativeName + "_tmp"; }
        }

        public string this[string group] {
            get { return Match.Groups[group].Value.Trim(); }
        }

        public string this[int group] {
            get { return Match.Groups[group].Value.Trim(); }
        }

        public bool Has (string group) {
            return String.IsNullOrEmpty(this[group]) == false;
        }
        
        public bool Has (int group) {
            return String.IsNullOrEmpty(this[group]) == false;
        }

        public T Clone (Match m) {
            T clone = (T) MemberwiseClone();
            clone.Match = m;
            clone.OnCloned((T) this);
            return clone;
        }

        public void InvokeOnMatched () {
            if (OnMatched != null) {
                OnMatched((T) this);
            }
        }
        
        public void InvokeOnParsingDone () {
            if (OnParsingDone != null) {
                OnParsingDone((T) this);
            }
        }

        public virtual void OnCloned (T parent) {

        }

        public override string ToString () {
            if (Match == null)
                return "NULL";

            return NativeName;
        }
    }
}
