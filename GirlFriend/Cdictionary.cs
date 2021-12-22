using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GirlFriend
{
    internal class Cdictionary
    {
        private List<string> _randomList = new();

        private List<ParseItem> _patternList = new();

        Dictionary<string, List<string>> _templateDictionary = new();

        public List<string> Random
        { get => _randomList; }

        public List<ParseItem> Pattern
        { get => _patternList; }

        public Dictionary<string, List<string>> Template
        { get => _templateDictionary; }

        public Cdictionary()
        {
            MakeRandomList();
            MakePatternList();
            MakeTemplateDictionary();
        }

        private void MakeRandomList()
        { 
            string[] r_lines = File.ReadAllLines(
                @"dics\random.txt",
                System.Text.Encoding.UTF8
                );

            foreach (string line in r_lines)
            {
                string str = line.Replace("\n", "");
                if (line != "")
                {
                    _randomList.Add(str);
                }
            } 
        }

        private void MakePatternList()
        { 
            string[] p_lines = File.ReadAllLines(
                @"dics\pattern.txt",
                System.Text.Encoding.UTF8
                );

            List<string> new_lines = new();

            foreach (string line in p_lines)
            {
                string str = line.Replace("\n", "");
                if (line != "")
                {
                    new_lines.Add(str);
                }
            }

            foreach (string line in new_lines)
            {
                // add code
                // line replace \\t -> \t
                string rep_line = line.Replace("\\t", "\t");
                string[] carveLine = rep_line.Split(new char[] { '\t' });
                _patternList.Add(
                    new ParseItem(
                        carveLine[0],
                        carveLine[1])
                    );
            }
        }

        // テンプレート辞書を作成する。
        private void MakeTemplateDictionary()
        {
            string[] t_lines = File.ReadAllLines(
                @"dics\template.txt",
                System.Text.Encoding.UTF8
                );

            List<string> new_lines = new();

            foreach (string line in t_lines)
            {
                string str = line.Replace("\n", "");

                if (line != "")
                {
                    new_lines.Add(str);
                }
            }

            foreach (string line in new_lines)
            {
                // add code
                // line replace \\t -> \t
                string rep_line = line.Replace("\\t", "\t");
                string[] carveLine = rep_line.Split(new Char[] { '\t' });

                if (!_templateDictionary.ContainsKey(carveLine[0]))
                {
                    _templateDictionary.Add(carveLine[0], new List<string> { });
                }

                _templateDictionary[carveLine[0]].Add(carveLine[1]);
            }
        }

        public void Study(string input, List<string[]> parts)
        {
            string userInput = input.Replace("\n", "");
            StudyRandom(userInput);
            StudyPattern(userInput, parts);
            StudyTemplate(parts);
        }

        // ランダム辞書に追加する
        public void StudyRandom(string userInput)
        {
            if (_randomList.Contains(userInput) == false)
            {
                _randomList.Add(userInput);
            }
        }

        // パターン辞書に追加する
        public void StudyPattern(string userInput, List<string[]> parts)
        {
            foreach (string[] morpheme in parts)
            {
                if (Analyzer.KeyWordCheck(morpheme[1]).Success)
                {
                    ParseItem? depend = null;

                    foreach (ParseItem item in _patternList)
                    {
                        if (!string.IsNullOrEmpty(item.Match(userInput)))
                        {
                            depend = item;
                            break;
                        }
                    }

                    if (depend != null)
                    {
                        depend.AddPhrase(userInput);
                    }
                    else
                    {
                        _patternList.Add(new ParseItem(
                            morpheme[0],
                            userInput));
                    }
                }
            }
        }

        // ユーザーの発言をテンプレート学習して、テンプレート辞書に追加する。
        public void StudyTemplate(List<string[]> parts)
        {
            string tempStr = "";
            int count = 0;

            foreach (string[] morpheme in parts)
            {
                if (Analyzer.KeyWordCheck(morpheme[1]).Success)
                {
                    morpheme[0] = "%noun%";
                    count++;
                }

                tempStr += morpheme[0];
            }

            if (count > 0)
            {
                string num = Convert.ToString(count);

                if (!_templateDictionary.ContainsKey(num))
                {
                    _templateDictionary.Add(num, new List<string>());
                }

                if (!_templateDictionary[num].Contains(tempStr))
                { 
                    _templateDictionary[num].Add(tempStr);
                }
            }
        }

        public void Save()
        {
            File.WriteAllLines(
                @"dics\random.txt",
                _randomList,
                System.Text.Encoding.UTF8
                );

            List<string> patternLine = new();

            foreach (ParseItem item in _patternList)
            {
                patternLine.Add(item.MakeLine());
            }

            File.WriteAllLines(
                @"dics\pattern.txt",
                patternLine,
                System.Text.Encoding.UTF8
                );

            List<string> templateLine = new();

            foreach (var dic in _templateDictionary)
            {
                foreach (var temp in dic.Value)
                {
                    templateLine.Add(dic.Key + "\\t" + temp);
                }

                templateLine.Sort();

                File.WriteAllLines(
                    @"dics\template.txt",
                    templateLine,
                    System.Text.Encoding.UTF8
                    );
            }
        }
    }
}
