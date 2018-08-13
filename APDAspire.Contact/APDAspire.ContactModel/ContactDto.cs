using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace APDAspire.Model
{  

    [DisplayName("Contact")]
    [BsonIgnoreExtraElements]
    public class ContactDto
    {
        [BsonId]
        public Guid Contact_Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DOB { get; set; }
        public IList<string> EmailId { get; set; }
        public IList<string> PhoneNumber { get; set; }
    }
}
