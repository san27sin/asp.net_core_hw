using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace asp.net_core_hw
{
    internal class Program
    {      

        static async Task Main()
        {
            try
            {
                List<Task<Request>> taskList = new List<Task<Request>>();//создаем коллекцию задач
                HttpClient client = new HttpClient();

                for (int id = 4; id <= 13; id++)
                {
                    taskList.Add(Response(client, id.ToString()));
                }

                if (Task.WhenAll(taskList).Wait(10_000))//статический метод ждет пока вся наша колллекция методов выполнится
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (var text in taskList)
                    {
                        sb.Append(text.Result.userId + "\n" + text.Result.id + "\n" + text.Result.title + "\n" + text.Result.body + "\n\n");
                    }

                    using (StreamWriter sw = new StreamWriter("result.txt"))
                    {
                        sw.WriteLine(sb.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("Прерывание программы!");
                }

                Console.WriteLine("Программа завершена!");
                Console.ReadKey();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }

        }



        static async Task<Request> Response(HttpClient client, string id)
        {
            HttpResponseMessage response = await client.GetAsync("https://jsonplaceholder.typicode.com/posts/" + id);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            Request request = JsonConvert.DeserializeObject<Request>(responseBody);
            return request;
        }

        public class Request
        {
            public string userId { get; set; }
            public string id { get; set; }
            public string title { get; set; }
            public string body { get; set; }
        }


    }
}
