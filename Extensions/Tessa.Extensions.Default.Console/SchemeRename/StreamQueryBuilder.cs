using System;
using System.IO;
using Tessa.Platform.Data;

namespace Tessa.Extensions.Default.Console.SchemeRename
{
    public sealed class StreamQueryBuilder :
        IQueryBuilder
    {
        #region Fields

        private readonly Dbms dbms;
        private readonly TextWriter writer;
        private bool requiresWhitespace;
        private bool requiresComma;
        private int indent;

        #endregion

        #region Constructors

        public StreamQueryBuilder(TextWriter writer, Dbms dbms)
        {
            this.dbms = dbms;
            this.writer = writer;
        }

        #endregion

        #region IQueryBuilder Overrides

        public Dbms Dbms
        {
            get { return this.dbms; }
        }

        public IQueryBuilder Append(string value)
        {
            this.writer.Write(value);
            this.requiresComma = false;

            var length = value.Length;
            if (length > 0)
            {
                var ch = value[length - 1];
                this.requiresWhitespace = ch != '(' && !char.IsWhiteSpace(ch);
            }

            return this;
        }

        public IQueryBuilder IncreaseIndent()
        {
            this.indent++;
            return this;
        }

        public IQueryBuilder DecreaseIndent()
        {
            if (this.indent > 0)
                this.indent--;
            return this;
        }

        public int Indent
        {
            get { return this.indent; }
        }

        public bool RequiresWhitespace
        {
            get { return this.requiresWhitespace; }
        }

        public IQueryBuilder RequireComma()
        {
            this.requiresComma = true;
            return this;
        }

        public bool RequiresComma
        {
            get { return this.requiresComma; }
        }

        string IQueryBuilder.Build()
        {
            throw new NotSupportedException();
        }

        void IQueryBuilder.Clear()
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}