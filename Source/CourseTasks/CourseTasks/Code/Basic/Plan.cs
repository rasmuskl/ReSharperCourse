using System.Collections.Generic;
using Basic.Support;

namespace Basic
{
    public class Plan
    {
        private readonly string _name;
        private readonly IEnumerable<Document> _documents;
        private readonly SecurityClassification _securityClassification;

        public Plan(string name, IEnumerable<Document> documents, SecurityClassification securityClassification)
        {
            _name = name;
            _documents = documents;
            _securityClassification = securityClassification;
        }

        public string Name
        {
            get { return _name; }
        }

        public IEnumerable<Document> Documents
        {
            get { return _documents; }
        }

        public SecurityClassification SecurityClassification
        {
            get { return _securityClassification; }
        }
    }
}