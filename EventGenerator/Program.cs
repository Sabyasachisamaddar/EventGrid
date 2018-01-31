using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace EventGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var eventNew = MakeRequestEvent();
            eventNew.Wait();
            Console.WriteLine(eventNew.Result.Content.ReadAsStringAsync().Result);

            Console.ReadKey();
        }

        private static async Task<HttpResponseMessage> MakeRequestEvent()
        {
            string topicEndpoint = "https://sabyeventgridtopic.westeurope-1.eventgrid.azure.net/api/events";
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("aeg-sas-key", "JvVqQvq5pVLD5DjggG8nEtqvA24jCvI8yoaB5Bt3M0Y=");

            List<CustomEvent<Account>> events = new List<CustomEvent<Account>>();
            var customEvent = new CustomEvent<Account>();
            customEvent.EventType = "eventgridtest";
            customEvent.Subject = "/testing";
            customEvent.Data = new Account( "Saby","Male" );
            events.Add(customEvent);

            string jsonContent = JsonConvert.SerializeObject(events);
            Console.WriteLine(jsonContent);

            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            return await httpClient.PostAsync(topicEndpoint, content);
        }
    }

    internal class Account
    {
        public string Name;
        public string Gender;

        public Account(string name , string gender)
        {
            Name = name;
            Gender = gender;
        }
    }

    public class CustomEvent<T>
    {

        public string Id { get; private set; }

        public string EventType { get; set; }

        public string Subject { get; set; }

        public string EventTime { get; private set; }

        public T Data { get; set; }

        public CustomEvent()
        {
            Id = Guid.NewGuid().ToString();

            DateTime localTime = DateTime.Now;
            DateTime utcTime = DateTime.UtcNow;
            DateTimeOffset localTimeAndOffset = new DateTimeOffset(localTime, TimeZoneInfo.Local.GetUtcOffset(localTime));
            EventTime = localTimeAndOffset.ToString("o");
        }
    }
}
