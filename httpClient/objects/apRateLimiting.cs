namespace Objects
{
    public class ApiRateLimiting
    {
        public DateTime startDateTime {get;set;}
        public DateTime endDateTime {get;set;}
        public int requestsTotal {get;set;}
        public int requestsInterval {get;set;}
        public bool rateLimitReached {get;set;}
    }
}
