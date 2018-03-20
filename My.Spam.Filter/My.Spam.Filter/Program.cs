using System;
using System.IO;
using My.Spam.Filter.Classifying;
using My.Spam.Filter.Core;
using My.Spam.Filter.Learning;

namespace My.Spam.Filter
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var filter = new SpamFilter();
            var learner = new Learner(filter);

            string line;

            var learnFile = File.OpenText(@"Data\LearnData.txt");
            while ((line = learnFile.ReadLine()) != null)
            {
                learner.Learn(line.Substring(line.IndexOf(',') + 1), line.ToLower().StartsWith("spam,") ? MessageType.Spam : MessageType.Ham);
            }

            learnFile.Close();
            filter.Transparent();

            var classifier = new Classifier(filter, learner);
            var testFile = File.OpenText(@"Data\TestData.txt");
            while ((line = testFile.ReadLine()) != null)
            {
                var expectedType = line.ToLower().StartsWith("spam,") ? MessageType.Spam : MessageType.Ham;
                var actualType = classifier.Classify(line.Substring(line.IndexOf(',') + 1));

                Console.Write(actualType != expectedType ? "Wrong - \t{0}{1}" : "Correct - \t{0}{1}",
                    line.Substring(0, 40) + "...", Environment.NewLine);
            }

            testFile.Close();

            Console.ReadLine();
        }
    }
}
