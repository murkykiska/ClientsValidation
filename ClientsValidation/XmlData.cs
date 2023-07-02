using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace ClientsValidation
{
    [Serializable()]
    [XmlRoot("Client")]
    public class Client
    {
        [Required]
        [XmlElement("FIO")]
        public string FIO { get; set; }
        [XmlElement("RegNumber")]
        public int RegNumber { get; set; }
        [Required]
        [XmlElement("DiasoftID")]
        public string DiasoftID { get; set; }
        [Required]
        [XmlElement("Registrator")]
        public string Registrator { get; set; }
        public int RegistratorID { get; set; }
        public Client() { }
        public void SetRegistratorID(int id)
        {
            RegistratorID = id;
        }
    }
    [Serializable()]
    [XmlRoot("Clients")]
    public class ClientsCollection
    {
        [XmlElement("Client")]
        public List<Client> Clients { get; set; }
        public ClientsCollection()
        {
            Clients = new List<Client>();
        }
    }

    [Serializable()]
    [XmlRoot("Registrator")]
    public class Registrator : IEquatable<Registrator>
    {
        [XmlElement("ID")]
        public int ID { get; set; }
        [XmlElement("Name")]
        public string Name { get; set; }
        public Registrator() { }
        public Registrator(int iD, string name)
        {
            ID = iD;
            Name = name;
        }
        public static bool operator ==(Registrator r1, Registrator r2)
        {
            if (object.ReferenceEquals(r1, r2))
                return true;
            if (object.ReferenceEquals(r1, null) || object.ReferenceEquals(r2, null))
                return false;
            return r1.Name == r2.Name;
        }
        public static bool operator !=(Registrator r1, Registrator r2)
        {
            return !(r1 == r2);
        }

        public bool Equals(Registrator? other)
        {
            return this.Name == other.Name;
        }
        public override bool Equals(object? obj)
        {
            return Equals(obj as Registrator);
        }
    }
    [Serializable()]
    [XmlRoot("Registrators")]
    public class RegistratorsCollection
    {
        [XmlElement("Registrator")]
        public List<Registrator> Registrators { get; set; }
        public RegistratorsCollection()
        {
            Registrators = new List<Registrator>();
        }
    }
}
