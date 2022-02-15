using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApi.Models {
    [Table("nilai")]
    public class NilaiModel {
        [ForeignKey("nrp")]
        public string nrp {get; set;} = null!;
        
        public string matkul {get; set;} = null!;
        
        public byte nilai {get; set;}

        [JsonIgnore]
        public MhsModel mhs {get; set;}
    }
}