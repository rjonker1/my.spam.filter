using My.Spam.Filter.Core;

namespace My.Spam.Filter.Learning
{
    public class Learner
    {
        private readonly SpamFilter _filter;

        public Learner(SpamFilter filter)
        {
            this._filter = filter;
        }

        /// <summary>
        /// record the number of occurrences of a specific word in a message of specified type... 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <param name="words"></param>
        public void Learn(string message, MessageType messageType, string[] words = null)
        {
            var wordsToProcess = words ?? _filter.MessageToWords(message);
            foreach (var word in wordsToProcess)
            {
                var fact = _filter.BaseFacts.ContainsKey(word) ? _filter.BaseFacts[word] : new BaseFact();
                if (messageType == MessageType.Spam)
                    fact.AdvanceSpamCount(); //if a word occurs in a message flagged as spam, its spam counter will advance,
                else
                    fact.AdvanceHamCount(); //if it occurs in a ham message, its ham counter will advance. 

                _filter.BaseFacts[word] = fact;
            }
        }
    }
}
