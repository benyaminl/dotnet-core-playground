namespace TodoApi.Models.Requests
{
    public class PostJSON 
    {
        public string Nama {get; set;} = null!;
        public string Alamat { get; set;} = null!;
        public string Telepon {get; set;} = null!;
    }
}