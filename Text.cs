using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FormatFile
{
    abstract class TextStruct
    {
        protected string text;

        public abstract void FormatText();
    }

    interface ISplit
    {
        void Split();
    }

    class Text : TextStruct, ISplit
    {
        Paragraph[] paragraphs;

        public Text(string text)
        {
            this.text = text;
            Split();
            FormatText();
        }

        public void Split()
        {
            MatchCollection mc = Regex.Matches(text, Environment.NewLine);
            List<Paragraph> paragraphs = new List<Paragraph>();
            int pos = 0;
            string temp;
            foreach (Match m in mc)
            {
                temp = text.Substring(pos, m.Index - pos);
                if (Regex.Match(temp, "[.!?]$").Success)
                {
                    paragraphs.Add(new Paragraph(temp));
                    pos = m.Index + m.Length;
                }
            }
            temp = text.Substring(pos).Trim();
            if (!string.IsNullOrWhiteSpace(temp))
                paragraphs.Add(new Paragraph(temp));
            this.paragraphs = paragraphs.ToArray();
        }

        public override void FormatText()
        {
            foreach (Paragraph p in paragraphs)
                p.FormatText();
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, paragraphs.Select(s => s.ToString()).ToArray());
        }
    }

    class Paragraph : TextStruct, ISplit
    {
        Sentence[] sentences;

        public Paragraph(string text)
        {
            this.text = text;
            Split();
        }

        public void Split()
        {
            MatchCollection mc = Regex.Matches(text, @"[.!?]+");
            List<Sentence> sentences = new List<Sentence>();
            int pos = 0;
            foreach (Match m in mc)
            {
                sentences.Add(new Sentence(text.Substring(pos, m.Index - pos + m.Length)));
                pos = m.Index + m.Length + 1;
            }
            this.sentences = sentences.ToArray();
        }

        public override void FormatText()
        {
            foreach (Sentence s in sentences)
                s.FormatText();
        }

        public override string ToString()
        {
            string[] strs = new string[sentences.Length];
            strs[0] = sentences.First().InsertToStart("\t").ToString();
            Array.Copy(sentences.Skip(1).Select(s => s.ToString()).ToArray(), 0, strs, 1, strs.Length - 1);
            return string.Join(" ", strs);
        }
    }

    class Sentence : TextStruct
    {
        public Sentence(string text)
        {
            this.text = text;
        }

        public override void FormatText()
        {
            StringBuilder builder = new StringBuilder(text.Trim());
            builder.Insert(0, builder[0].ToString().ToUpper()).Remove(1, 1);//Первая заглавная
            builder.Replace(Environment.NewLine, "");//Удаление концов строк
            MatchCollection mc = Regex.Matches(text, @"([\.,:;?!]+)([^ \.,:;?!])");//После знаков препинания пробелы
            foreach (Match m in mc)
                builder.Replace(m.Value, m.Groups[1].Value + ' ' + m.Groups[2].Value);
            mc = Regex.Matches(text, @" ([\.,:;?!]+)");//Перед знаками препинания нет пробелов
            foreach (Match m in mc)
                builder.Replace(m.Value, m.Groups[1].Value);
            text = builder.ToString();
        }

        public Sentence InsertToStart(string s)
        {
            text = s + text;
            return this;
        }

        public override string ToString()
        {
            return text;
        }
    }
}
