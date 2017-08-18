using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebInquirer.Models;

namespace WebInquirer.Classes
{
    public class DBHelpers
    {
        public static Test GetNewTestById(int id)
        {
            DatabaseEntities db = new DatabaseEntities();
            
            Tests dbTest = db.Tests.Find(id);
            if (dbTest == null) return null;

            // Return if test passed
            if (dbTest.passed == true) return null;

            Test test = new Test();
            test.Id = id;
            test.Name = dbTest.name;
            test.Description = dbTest.description;

            // Add questions for test
            foreach (Questions question in db.Questions.Where(e => e.idTest == id ))
            {
                // Collect options for question
                List<Option> options = new List<Option>();
                foreach(Entries entry in db.Entries.Where(e => e.idQuestion == question.id))
                {
                    options.Add(new Option
                    {
                        Id = entry.id,
                        Name = entry.name,
                        Checked = false
                    });
                }

                test.Questions.Add(new Question
                {
                    Id = question.id,
                    Name = question.name,
                    Type = (Types)question.type,
                    Options = options
                });
            }

            return test;
        }

        public static Test GetPassedTestById(int id)
        {
            DatabaseEntities db = new DatabaseEntities();

            Tests dbTest = db.Tests.Find(id);
            if (dbTest == null) return null;

            // Return if test not passed
            if (dbTest.passed != true) return null;

            Test test = new Test();
            test.Id = id;
            test.Name = dbTest.name;
            test.Description = dbTest.description;

            // Add ansvers for test
            foreach (var group in db.Answers.Where(e => e.idTest == id).GroupBy(e => e.idQuestion))
            {
                // Fill for Edit and Select

                Questions groupQuestion = db.Questions.Find(group.Key);

                if (groupQuestion.type == (int)Types.Edit || groupQuestion.type == (int)Types.Select)
                {
                    foreach (var groupItem in group)
                    {
                        test.Answers.Add(new Answer
                        {
                            Id = groupItem.idQuestion,
                            Name = groupQuestion.name,
                            Type = (Types)groupQuestion.type,
                            Value = groupItem.value,
                        });
                    }
                }
                // Fill for Ckeckbox
                else if (groupQuestion.type == (int)Types.Checkbox)
                {
                    List<Option> options = new List<Option>();

                    foreach (var groupItem in group)
                    {
                        options.Add(new Option
                        {
                            Id = groupItem.id,
                            Name  =  groupItem.value
                        });
                    }

                    test.Answers.Add(new Answer
                    {
                        Id = groupQuestion.id,
                        Name = groupQuestion.name,
                        Type = (Types)groupQuestion.type,
                        Options = options
                    });
                }
            }

            return test;
        }

        public static bool PassTest(Test test)
        {
            DatabaseEntities db = new DatabaseEntities();

            Tests dbTest = db.Tests.Find(test.Id);
            if (dbTest == null) return false;


            // Fill answers
            foreach(Question question in test.Questions)
            {
                if (question.Type == Types.Edit)
                {
                    if (question.Value == null) continue;

                    db.Answers.Add(new Answers
                    {
                        idTest = test.Id,
                        idQuestion = question.Id,
                        value = question.Value
                    });
                }
                else if (question.Type == Types.Checkbox)
                {

                    var options = question.Options.Where(e => e.Checked == true)
                        .Select(e => new Answers
                        {
                            idTest = test.Id,
                            idQuestion = question.Id,
                            value = db.Entries.Find(e.Id).name,
                        });

                    db.Answers.AddRange(options);
                }
                else if (question.Type == Types.Select)
                {
                    if (question.Value == null) continue;

                    db.Answers.Add(new Answers
                    {
                        idTest = test.Id,
                        idQuestion = question.Id,
                        value = db.Entries.Find(Int32.Parse(question.Value)).name
                    });
                }
            }

            dbTest.passed = true;
            db.Entry(dbTest).State = System.Data.Entity.EntityState.Modified;

            db.SaveChanges();

            return true;
        }

        public static bool RemoveTestById(int id)
        {
            DatabaseEntities db = new DatabaseEntities();

            Tests dbTest = db.Tests.Find(id);
            if (dbTest == null) return false;

            // Remove items for Answers table
            var answers = db.Answers.Where(e => e.idTest == id);
            db.Answers.RemoveRange(answers);

            // Modify item for Test table
            dbTest.passed = null;
            db.Entry(dbTest).State = System.Data.Entity.EntityState.Modified;

            db.SaveChanges();

            return true;
        }

        public static List<Tests> GetNewTests()
        {
            DatabaseEntities db = new DatabaseEntities();

            var tests = db.Tests.Where(e => e.passed != true);

            return tests.ToList();
        }

        public static List<Tests> GetPassedTests()
        {
            DatabaseEntities db = new DatabaseEntities();

            var tests = db.Tests.Where(e => e.passed == true);

            return tests.ToList();

        }

    }
}