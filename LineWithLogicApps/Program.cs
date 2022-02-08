using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}


app.MapPost("/sendemail", async (IHttpClientFactory _httpClientFactory, 
    IConfiguration _configuration,
    Email email) =>
{
    var client = _httpClientFactory.CreateClient();

    var req = new LogicApp
    {
        Email = email.MailAddress,
        Date = DateTime.Now.ToString(),
        Subject = email.Subject,
        Body = email.Body
    };

    var jsonData = JsonSerializer.Serialize(req);

    using var result = await client.PostAsync(
                _configuration["LOGIC_APP_URL"],
                new StringContent(jsonData, Encoding.UTF8, "application/json"));
    result.EnsureSuccessStatusCode();

})
.Accepts<Email>("application/json");

app.Run();


internal record Email
{
    public string? MailAddress { get; set; }
    public string? Subject { get; set; }
    public string? Body { get; set; }

}

internal record LogicApp
{
    public string? Email { get; set; }
    public string? Subject { get; set; }
    public string? Body { get; set; }
    public string? Date { get; set; }
}