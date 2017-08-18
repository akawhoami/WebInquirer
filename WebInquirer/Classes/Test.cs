using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebInquirer.Models;

namespace WebInquirer.Classes
{
    public class Test
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Question> Questions { get; set; }
        public List<Answer> Answers { get; set; }

        public Test()
        {
            this.Questions = new List<Question>();
            this.Answers = new List<Answer>();
        }
    }

    public class Question
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Types Type { get; set; }
        public List<Option> Options { get; set; }
        public string Value { get; set; }
    }

    public class Answer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Types Type { get; set; }
        public List<Option> Options { get; set; }
        public string Value { get; set; }
    }

    public class Option
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Checked { get; set; }
    }

    public enum Types
    {
        Edit = 1,
        Checkbox = 2,
        Select = 3
    }
}