/*
    Copyright 2018 Olli Helin / GainIT
    This file is part of Kerppi, a free software released under the terms of the
    GNU General Public License v3: http://www.gnu.org/licenses/gpl-3.0.en.html
*/

using System.Xml;
using System.Xml.Serialization;

namespace Kerppi.DataModel
{
    /// <summary>
    /// This helper class is a data gatherer to comply with the General Data Protection Regulation.
    /// </summary>
    public class DataSubjectData
    {
        public class Address
        {
            public string PostalAddress { get; set; }
            public string PostalCode { get; set; }
        }

        public class KerppiPerson
        {
            public string Name { get; set; }
            public string ContactInfo { get; set; }
            public Address AddressInfo { get; set; }
            public string IdCode { get; set; }
            public string Information { get; set; }
            public string ContactPersonName { get; set; }
            public string ClientName { get; set; }
            [XmlArrayItem("TaskDescription")]
            public string[] TaskData { get; set; }
        }

        public class KerppiDataSubject
        {
            public KerppiPerson Person { get; set; }
        }

        public KerppiDataSubject DataSubject { get; set; }

        public DataSubjectData()
        {
            DataSubject = new KerppiDataSubject();
        }

        public DataSubjectData(Client client, string[] taskData = null, bool contactPersonDataOnly = false)
        {
            DataSubject = new KerppiDataSubject();
            if (contactPersonDataOnly)
            {
                DataSubject.Person = new KerppiPerson
                {
                    Name = client.ContactPersonName,
                    ContactInfo = client.ContactPersonContactInfo,
                    AddressInfo = new Address
                    {
                        PostalAddress = client.ContactPersonPostalAddress,
                        PostalCode = client.ContactPersonPostalCode
                    },
                    ClientName = client.Name
                };
                return;
            }

            DataSubject.Person = new KerppiPerson
            {
                Name = client.Name,
                ContactInfo = client.ContactInfo,
                AddressInfo = new Address
                {
                    PostalAddress = client.PostalAddress,
                    PostalCode = client.PostalCode
                },
                IdCode = client.IdCode,
                Information = client.Information,
                ContactPersonName = client.ContactPersonName,
                TaskData = taskData
            };
        }

        public string PrintData()
        {
            string xml = StringSerializer.ToString(DataSubject);
            var xmldoc = new XmlDocument();
            xmldoc.LoadXml(xml);
            xmldoc.DocumentElement.SetAttribute("xmlns", "GainIT-KerppiDataSubject");
            var sw = new Utf8StringWriter();
            xmldoc.Save(sw);
            return sw.ToString();
        }
    }
}