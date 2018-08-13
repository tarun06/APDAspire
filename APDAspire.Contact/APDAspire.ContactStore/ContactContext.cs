using APDAspire.Model;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;

namespace APDAspire.ContactStore
{
    public class ContactContext
    {
        private readonly IMongoDatabase database = null;
        private readonly string collectionName;

        public ContactContext(IOptions<Setting> settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            this.collectionName = settings.Value.ContactCollection;

            var client = new MongoClient(settings.Value.ConnectionString);
            if (client != null)
                database = client.GetDatabase(settings.Value.Database);

        }

        public IMongoCollection<ContactModel> ContactData
        {
            get
            {
                return database.GetCollection<ContactModel>(this.collectionName);
            }
        }
    }
}
