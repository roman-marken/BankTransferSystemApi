using System.Text.Json.Serialization;
using BankTransferSystemApi.Infrastructure.Repositories;
using BankTransferSystemApi.Services.Implementations;
using BankTransferSystemApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    // Переліки у JSON серіалізуються/читаються як рядки: "Premium" замість 2.
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddOpenApi();

// ============================================================
// Реєстрація залежностей у DI-контейнері.
//
// Data Access Layer — Singleton: In-Memory стан має бути спільним
// для всіх запитів протягом усього життя застосунку.
// ============================================================
builder.Services.AddSingleton<IAccountRepository, InMemoryAccountRepository>();
builder.Services.AddSingleton<ITransactionRepository, InMemoryTransactionRepository>();

// Чисті утиліти без стану — Transient.
builder.Services.AddTransient<IFeeCalculator, TieredFeeCalculator>();

// Бізнес-сервіси — Scoped: один екземпляр на HTTP-запит.
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ITransferService, TransferService>();

// ============================================================
// Рівень 3 (бонус): вибір реалізації IAuditLogger динамічно
// з appsettings.json — секція "BankingAudit": { "Provider": "File" | "Console" }.
// ============================================================
var auditProvider = builder.Configuration["BankingAudit:Provider"];
if (string.Equals(auditProvider, "File", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddSingleton<IAuditLogger, FileAuditLogger>();
}
else
{
    builder.Services.AddSingleton<IAuditLogger, ConsoleAuditLogger>();
}

Console.WriteLine($"[DI] IAuditLogger → {(string.Equals(auditProvider, "File", StringComparison.OrdinalIgnoreCase) ? nameof(FileAuditLogger) : nameof(ConsoleAuditLogger))} (BankingAudit:Provider = {auditProvider ?? "не задано"})");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.Run();
