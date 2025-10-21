using Gabriel.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Numerics;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>();
var app = builder.Build();

app.MapGet("/", () => "Programa abrindo.");


// POST /api/consumo/cadastrar

app.MapPost("/api/consumo/cadastrar", ([FromBody] ConsumoDeAgua novoConsumo, [FromServices] AppDbContext ctx) =>
{
    ConsumoDeAgua? resultadoCpf = ctx.ConsumoDeAguas.FirstOrDefault((x => x.Cpf == novoConsumo.Cpf));
    ConsumoDeAgua? resultadoMes = ctx.ConsumoDeAguas.FirstOrDefault((x => x.Mes == novoConsumo.Mes));
    ConsumoDeAgua? resultadoAno = ctx.ConsumoDeAguas.FirstOrDefault((x => x.Ano == novoConsumo.Ano));

    /// Testa se o Cpf, Ano e mes não são os mesmos
    if ((resultadoCpf is null) && (resultadoAno is null) && (resultadoAno is null))
    {
        /// Testa  se o mês é maior que 11 e menor que 1
        if (novoConsumo.Mes >= 11 || novoConsumo.Mes <= 0) return Results.BadRequest("O mês está errado!");

        /// Testa  se o ano é menor que 2000
        if (novoConsumo.Ano < 2000) return Results.BadRequest("O ano está errado!");

        /// Testa  se o M3Consumidos é menor que 0
        if (novoConsumo.M3Consumidos < 0) return Results.BadRequest("O M3Consumidos está errado!");

        // /// Testa  se a Bandeira é verde, amarela ou vermelha (com a letra maiúscula)
        // if (novoConsumo.Bandeira != "Verde" || novoConsumo.Bandeira != "Amarela" || novoConsumo.Bandeira != "Vermelha") return Results.BadRequest("A bandeira está errada");


        /// Calculos:
        /// 
        /// Consumo mínimo faturável
        if (novoConsumo.M3Consumidos < 10)
        {
            novoConsumo.M3Consumidos = 10;
        }

        ///Tarifa única por faixa
        /// 
        double tarifa = 0;

        if ((novoConsumo.M3Consumidos > 0) && (novoConsumo.M3Consumidos <= 10))
        {
            tarifa = 2.5;

        }
        else if ((novoConsumo.M3Consumidos > 10) && (novoConsumo.M3Consumidos < 21))
        {
            tarifa = 3.50;
        }
        else if ((novoConsumo.M3Consumidos > 20) && (novoConsumo.M3Consumidos < 51))
        {
            tarifa = 5.00;
        }
        else
        {
            tarifa = 6.50;
        }

        novoConsumo.Tarifa = tarifa;

        ///Bandeira hídrica (acrescímo percentual)

        novoConsumo.ConsumoFaturado = novoConsumo.M3Consumidos;

        novoConsumo.ValorAgua = (int)(novoConsumo.ConsumoFaturado * tarifa);

        if (novoConsumo.Bandeira is "Vermelha")
        {
            novoConsumo.AdicionalBandeira = novoConsumo.ConsumoFaturado + (novoConsumo.ConsumoFaturado *0.20);
        }
        else if (novoConsumo.Bandeira is "Amarela")
        {
            novoConsumo.AdicionalBandeira = novoConsumo.ConsumoFaturado + (novoConsumo.ConsumoFaturado * 0.10);
        }
        else
        {
            novoConsumo.AdicionalBandeira = novoConsumo.ConsumoFaturado;
        }
        
        /// Taxa de esgoto

        double taxaEsgoto = 0;
        if (novoConsumo.PossuiEsgoto)
        {
            novoConsumo.TaxaEsgoto = (novoConsumo.ValorAgua + novoConsumo.AdicionalBandeira) * 0.80;
        }

        novoConsumo.TaxaEsgoto = taxaEsgoto;

        /// Total geral
        novoConsumo.Total = novoConsumo.ValorAgua + novoConsumo.AdicionalBandeira + taxaEsgoto;
        
        ctx.ConsumoDeAguas.Add(novoConsumo);
        ctx.SaveChanges();
        return Results.Created("", novoConsumo);
    }
    return Results.Conflict("Não é possível cadastrar no banco de dados com o Cpf, mês e ano sendo os mesmos");
});

// GET /api/consumo/listar
app.MapGet("/api/consumo/listar", ([FromServices] AppDbContext ctx) => {
    if (ctx.ConsumoDeAguas.Any()) { return Results.Ok(ctx.ConsumoDeAguas); }
    return Results.NotFound("Lista Vazia");
});

// GET /api/consumo/buscar/{cpf}/{mes}/{ano}

app.MapGet("/api/consumo/buscar/{cpf}/{mes}/{ano}", ([FromServices]AppDbContext ctx, [FromRoute]String cpf, int mes, int ano) =>
{
    ConsumoDeAgua? resultadoCpf = ctx.ConsumoDeAguas.FirstOrDefault(x => x.Cpf == cpf);
    ConsumoDeAgua? resultadoMes = ctx.ConsumoDeAguas.FirstOrDefault(x => x.Ano == ano);
    ConsumoDeAgua? resultadoAno = ctx.ConsumoDeAguas.FirstOrDefault(x => x.Mes == mes);

    // Testa se oss resultados são incorretos.
    if (resultadoCpf is null || resultadoAno is null || resultadoMes is null) return Results.NotFound("Consumo não encontrado");
    return Results.Ok(resultadoCpf);

});

// DELETE /api/consumo/remover/{cpf}/{mes}/{ano}

app.MapDelete("/api/consumo/remover/{cpf}/{mes}/{ano}", ([FromServices] AppDbContext ctx, [FromRoute] String cpf, int mes, int ano) =>
{
    ConsumoDeAgua? resultadoCpf = ctx.ConsumoDeAguas.FirstOrDefault(x => x.Cpf == cpf);
    ConsumoDeAgua? resultadoMes = ctx.ConsumoDeAguas.FirstOrDefault(x => x.Ano == ano);
    ConsumoDeAgua? resultadoAno = ctx.ConsumoDeAguas.FirstOrDefault(x => x.Mes == mes);


    if (resultadoCpf is null || resultadoAno is null || resultadoMes is null) { return Results.NotFound("Consumo não encontrado"); }

    ctx.ConsumoDeAguas.Remove(resultadoCpf);
    ctx.SaveChanges();
    return Results.Ok(resultadoCpf + " deletado com sucesso.");

});


/// GET /api/consumo/total-geral
app.MapGet("/api/consumo/total-geral", (AppDbContext ctx, double total) =>
{
    double TotalContas = 0.00;
    if (ctx.ConsumoDeAguas.Any())
    {
        TotalContas += total;
        return Results.Ok("totalGeral" + TotalContas);
    }
    return Results.NotFound("Lista um total para calcular.");
});

/// Calculos:


app.Run();
