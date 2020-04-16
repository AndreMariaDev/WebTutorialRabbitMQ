using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace WebTutorialRabbitMQ.Model
{
    [DataContract]
    public class LotesEletronicos 
    {
        [DataMember]
        public string cod_interface { get; set; }

        [DataMember]
        public string msg_id { get; set; }

        [DataMember]
        public List<LoteEletronico> Lotes { get; set; }
    }

    [DataContract]
    public class LoteEletronico
    {
        [DataMember]
        public string NumeroLoteFaturamento { get; set; }

        [DataMember]
        public string num_contrato { get; set; }

        [DataMember]
        public List<Ficha> Fichas { get; set; }
    }

    [DataContract]
    public class Ficha
    {
        [DataMember]
        public string NumeroFicha { get; set; }

        [DataMember]
        public List<Guia> Guias { get; set; }
    }

    [DataContract]
    public class Guia
    {
        [DataMember]
        public string guia_prestador { get; set; }

        [DataMember]
        public string end_arquivo { get; set; }
    }
}
