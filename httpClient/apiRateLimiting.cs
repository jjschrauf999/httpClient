using Objects;
using Newtonsoft.Json;

public class ApiRateLimiting
{
    public required int perDayLimit {get;set;}
    public required int rateLimitPerSecond {get;set;}
    public required int rateLimitPerMinute {get;set;}
    public required string currentRateLimitFilePath {get;set;}
    public required TimeSpan rateLimtResetTime {get;set;} 
    private Objects.ApiRateLimiting _currentRateLimiting = new Objects.ApiRateLimiting();
    private static string _logFolder = "/fantasyFootball/logs/";
    private static string _logFile = "log.txt";
    public required HttpClientExtension httpClientExtension;

    public async Task<HttpResponseMessage> CallHttpClient(HttpMethod httpMethod, string route, object parameters)
    {
        if(RateLimitReached())
        {
            throw new Exception("Request limit reached.");
        }
        else
        {
            Thread.Sleep(1000);
            return await httpClientExtension.SendRequest(httpMethod,route,parameters);
        }
    }
    public async Task<HttpResponseMessage> CallHttpClientData(HttpMethod httpMethod, string route, object obj)
    {
        if(RateLimitReached())
        {
            throw new Exception("Request limit reached.");
        }
        else
        {
            Thread.Sleep(1000);
            return await httpClientExtension.SendRequestData(httpMethod,route, obj);
        }
    }

    private void ResetCurrentRateLimiting()
    {
        if(DateTime.Now.TimeOfDay < rateLimtResetTime)
        {
            _currentRateLimiting.startDateTime = DateTime.Now.Date.AddDays(-1) + rateLimtResetTime;
            _currentRateLimiting.endDateTime = DateTime.Now.Date + rateLimtResetTime;
        }
        else
        {
            _currentRateLimiting.startDateTime = DateTime.Now.Date + rateLimtResetTime;
            _currentRateLimiting.endDateTime = DateTime.Now.Date.AddDays(1) + rateLimtResetTime;
        }
        
        _currentRateLimiting.requestsTotal = 0;
        _currentRateLimiting.requestsInterval = 0;
        _currentRateLimiting.rateLimitReached = false;

        Log("Current rate limiting reset.");
    }

    private bool RateLimitReached()
    {
        string currentRateLimitFile = Directory.GetCurrentDirectory() + currentRateLimitFilePath;
        
        if(File.Exists(currentRateLimitFile))
        {
            string rateLimitFileText = File.ReadAllText(currentRateLimitFile);
            _currentRateLimiting = JsonConvert.DeserializeObject<Objects.ApiRateLimiting>(rateLimitFileText);
        }

        if(DateTime.Now < _currentRateLimiting.startDateTime || DateTime.Now >= _currentRateLimiting.endDateTime)
        {
            ResetCurrentRateLimiting();
        }

        if(_currentRateLimiting.requestsTotal < perDayLimit)
        {
            RateLimiting();
        }
        else
        {
            _currentRateLimiting.rateLimitReached = true;
        }

        JsonHelper.WriteToFile(_currentRateLimiting, currentRateLimitFile);
        Log("requestsTotal: " + _currentRateLimiting.requestsTotal + " requestsInterval: " + _currentRateLimiting.requestsInterval);

        return _currentRateLimiting.rateLimitReached;
    }

    private void RateLimiting()
    {
        if(rateLimitPerSecond != 0 && rateLimitPerMinute != 0)
        {
            throw new Exception("Rate limiting specified for both seconds and minutes");
        }
        else if(rateLimitPerSecond != 0)
        {
            if(_currentRateLimiting.requestsInterval < rateLimitPerSecond)
            {
                _currentRateLimiting.requestsInterval++;
            }
            else
            {
                Log("Thread sleeping for 2 seconds.");
                Thread.Sleep(2000);
                _currentRateLimiting.requestsInterval = 1;
            }
        }
        else if (rateLimitPerMinute != 0)
        {
            if(_currentRateLimiting.requestsInterval < rateLimitPerMinute)
            {
                _currentRateLimiting.requestsInterval++;
            }
            else
            {
                Log("Thread sleeping for 2 minutes.");
                Thread.Sleep(120000);
                _currentRateLimiting.requestsInterval = 1;
            }
        }
        else
        {
            throw new Exception("Rate limiting not specified");
        }

        _currentRateLimiting.requestsTotal++;
        _currentRateLimiting.rateLimitReached = false;
    }

    private void Log (string logMessage)
    {
        string logPath = Directory.GetCurrentDirectory() + _logFolder;
        string logFile = logPath + DateTime.Now.ToShortDateString().Replace("/","-") + "_" + _logFile;

        if(!Directory.Exists(logPath))
        {
            Directory.CreateDirectory(logPath);
        }

        if(!File.Exists(logFile))
        {
            using (FileStream fs = File.Create(logFile)){}
        }

        File.AppendAllText(logFile, DateTime.Now.ToString() + " " + logMessage  + Environment.NewLine);
    }
}
