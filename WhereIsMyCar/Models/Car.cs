using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Device.Location;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WhereIsMyCar.Models
{
    [DataContract]
    public class Car
    {
        [DataMember]
        public GeoCoordinate Geo { get; set; }
        [DataMember]
        public String Address
        {
            get;
            set;
        }

        [DataMember]
        public String Name
        {
            get;
            set;
        }

        public Car()
        {
            Name = "Ma voiture";
        }
    }
}
