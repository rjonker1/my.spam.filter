using System;
using System.Collections.Generic;
using System.Linq;

namespace My.Spam.Filter.Core
{
    public enum MessageType
    {
        Spam,
        Ham
    }

    public class BaseFact
    {
        private const decimal Significance = 0.5m;

        public void AdvanceSpamCount()
        {
            Spam++;
        }

        public void AdvanceHamCount()
        {
            Ham++;
        }

        //The significance of a word computed like this: (Spam−Ham)/(Spam+Ham). 
        //If this is less than a pre-defined significance value (currently 0.5), the word is moved.
        public bool IsTransparent => (Math.Abs((Spam - Ham)) / (Spam + Ham)) <= Significance;

        public decimal Spam { get; private set; }
        public decimal Ham { get; private set; }
    }

    public class SpamFilter
    {
        private const int Max = 1000;
        private int _trigger = 0;

        //initiate an update of the list of the insignificant words 
        //according to a pre-defined frequency(currently after each 1000 new messages).
        public bool NeedToUpdateInsignifantWords()
        {
            return ++_trigger > Max;
        }

        public void UpdateInsignifantWords()
        {
            _trigger = 0;
            Transparent();
        }

        public List<string> TransparentList = new List<string> { "the", "she", "yes", "too", "red", "one" };
        public Dictionary<string, BaseFact> BaseFacts = new Dictionary<string, BaseFact>();

        /// <summary>
        /// This method is responsible for splitting the sentence into words and removing those words that are found to be insignificant in defining a message. 
        /// The list of insignificant words (TransparentList) is originally initialized with a few words only (and that's one point for improvement), 
        /// and updated with a constant frequency or manually.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public string[] MessageToWords(string message)
        {
            //Adding < and > to punctuation chars to break around HTML tags, but without removing content like link addresses, that definitely can be used to identify spam.
            var punctuations = new char[] { ' ','<','>'};

            var lowerMessage = message.ToLower();
            var messagePunctuations = lowerMessage.Where(char.IsPunctuation).Distinct().ToList();
            punctuations.ToList().AddRange(messagePunctuations);

            //Removing words less than 2 characters long - it is mostly a shortcut to not blow up the list of insignificant words.
            var words = lowerMessage.Split(punctuations).Where(word => word.Length > 2).ToArray();

            //Do not remove duplicate words.The number of occurrences is very important to the classification later, so we should count the words exactly as they appear.
            // See for instance the word 'love'.It can be part of a spam and ham message too, however actual counting of the training data show that is much more frequent in spam than in ham.
            // A distinct counting may give the wrong identification for a message with such a word.
            return words.Except(TransparentList).ToArray();
        }

        /// <summary>
        /// Move the insignificant words from the BaseFacts list to the TransparentList. 
        /// </summary>
        public void Transparent()
        {
            var insignifcantWords = BaseFacts.Where(fact => fact.Value.IsTransparent)
                .ToDictionary(dict => dict.Key, dict => dict.Value).Keys.ToList();
            TransparentList.AddRange(insignifcantWords);

            var signifcantWords = BaseFacts.Where(fact => !fact.Value.IsTransparent)
                .ToDictionary(dict => dict.Key, dict => dict.Value);
            BaseFacts = signifcantWords;
        }
    }
}
