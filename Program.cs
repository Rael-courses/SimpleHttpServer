using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;
using System.Text.Json;

namespace SimpleHttpServer;
internal class Program
{
  private static void Main(string[] args)
  {
    // Time delay to wait for the server to start, so we can attach the debugger
    Task.Delay(500).Wait();

    var server = new HttpListener();
    server.Prefixes.Add("http://localhost:8080/");
    server.Start();

    Console.WriteLine("Server started at http://localhost:8080/");

    while (true)
    {

      var context = server.GetContext();
      var request = context.Request;
      var response = context.Response;

      try
      {
        Console.WriteLine($"Request: {request.Url}");

        if (request.HttpMethod == "GET" && (request.RawUrl == "/health" || request.RawUrl == "/"))
        {
          response.ContentType = "application/json";
          response.StatusCode = 200;
          response.OutputStream.Write(Encoding.UTF8.GetBytes("OK"));
        }
        else if (request.RawUrl == "/api/welcome")
        {
          if (request.HttpMethod == "GET")
          {
            response.ContentType = "application/json";
            response.StatusCode = 200;
            var content = new
            {
              path = request.RawUrl,
              message = "Welcome to our simple HTTP server"
            };
            var json = JsonSerializer.Serialize(content);

            response.OutputStream.Write(Encoding.UTF8.GetBytes(json));
          }
          else if (request.HttpMethod == "POST")
          {
            try
            {
              using var reader = new StreamReader(request.InputStream, Encoding.UTF8);
              var requestBody = reader.ReadToEnd();

              var requestData = JsonSerializer.Deserialize<RequestData>(requestBody);
              if (requestData == null)
              {
                throw new JsonException("Invalid JSON format");
              }
              Validator.ValidateObject(requestData, new ValidationContext(requestData));

              var name = requestData.name;

              var responseContent = new
              {
                path = request.RawUrl,
                message = $"Welcome to our simple HTTP server, {name}"
              };
              var json = JsonSerializer.Serialize(responseContent);

              response.ContentType = "application/json";
              response.StatusCode = 200;
              response.OutputStream.Write(Encoding.UTF8.GetBytes(json));
            }
            catch (Exception ex) when (ex is ValidationException || ex is JsonException)
            {
              response.StatusCode = 400;
              var errorContent = JsonSerializer.Serialize(new { error = ex.Message });
              response.OutputStream.Write(Encoding.UTF8.GetBytes(errorContent));
            }
          }
        }
        else if (request.HttpMethod == "GET" && request.RawUrl == "/home")
        {
          response.ContentType = "text/html";
          response.StatusCode = 200;
          var content = "<h1>Welcome to our simple HTTP server</h1>";
          response.OutputStream.Write(Encoding.UTF8.GetBytes(content));
        }
        else
        {
          response.StatusCode = 404;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        response.StatusCode = 500;

      }
      finally
      {
        response.Close();
      }
    }
  }

  internal class RequestData
  {
    [Required(AllowEmptyStrings = false)]
    public string name { get; init; }

    public RequestData(string name)
    {
      this.name = name;
    }
  }
}
