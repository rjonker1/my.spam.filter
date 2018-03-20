using System.Linq;
using My.Spam.Filter.Core;
using My.Spam.Filter.Learning;

namespace My.Spam.Filter.Classifying
{
    public class Classifier
    {
        private readonly SpamFilter _filter;
        private readonly Learner _learner;

        public Classifier(SpamFilter filter, Learner learner)
        {
            _filter = filter;
            _learner = learner;
        }

        public MessageType Classify(string message)
        {
            var words = _filter.MessageToWords(message);
            var facts = _filter.BaseFacts.Where(fact => words.Contains(fact.Key))
                .ToDictionary(dict => dict.Key, dict => dict.Value);

            var messageType = facts.Sum(dict => dict.Value.Spam) > facts.Sum(dict => dict.Value.Ham)
                ? MessageType.Spam
                : MessageType.Ham;

            _learner.Learn(message, messageType, words);

            if (_filter.NeedToUpdateInsignifantWords())
            {
                _filter.UpdateInsignifantWords();
            }

            return messageType;
        }
    }
}
