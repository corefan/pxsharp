using System;
using System.IO;
using System.Text;

namespace PxSharp.ApiGen {
    public class OutputFile : IDisposable {
        int indent = 0;
        string file = "";
        StringBuilder buffer = new StringBuilder();

        public int IndentLevel {
            get { return indent; }
            set { indent = value; }
        }

        public OutputFile (string path) {
            file = path;
        }

        public void Append (string text) {
            buffer.Append(text);
        }

        public void AppendFormat (string text, params object[] args) {
            buffer.Append(String.Format(text, args));
        }

        public void BeginLine () {
            Append(new string(' ', indent * 4));
        }

        public void EndLine () {
            Append("\r\n");
        }

        public void AppendLine (string text) {
            BeginLine();
            Append(text);
            EndLine();
        }

        public void AppendLineFormat (string text, params object[] args) {
            BeginLine();
            Append(String.Format(text, args));
            EndLine();
        }

        public void AppendLine () {
            AppendLine("");
        }

        public void Indented (Action action) {
            IndentLevel += 1;
            action();
            IndentLevel -= 1;
        }

        public void Save () {
            if (File.Exists(file)) {
                File.Delete(file);
            }

            File.WriteAllText(file, buffer.ToString());
        }

        void IDisposable.Dispose () {
            Save();
        }
    }
}
