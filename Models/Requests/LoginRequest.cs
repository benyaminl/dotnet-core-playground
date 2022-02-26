namespace TodoApi.Models.Requests {
    
    public class LoginRequest {
        public string user {get; set;} = null!;
        public string pass {get; set;} = null!;
    }
}