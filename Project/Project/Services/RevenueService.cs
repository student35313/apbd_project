using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.DTOs.Client;
using Project.Exceptions;

namespace Project.Services;

public class RevenueService : IRevenueService
{
    private readonly DatabaseContext _context;
    private readonly IExchangeRateService _exchangeRateService;

    public RevenueService(DatabaseContext context, IExchangeRateService exchangeRateService)
    {
        _context = context;
        _exchangeRateService = exchangeRateService;
    }

    public async Task<decimal> GetRevenueForClientAsync(RevenueRequestDto dto)
    {
        if (dto.ClientId == 0 || dto.ClientId == null)
            throw new BadHttpRequestException("Client ID is required.");
        
        var clientExists = await _context.Clients.AnyAsync(c => c.Id == dto.ClientId);
        if (!clientExists)
            throw new NotFoundException("Client not found.");
        
        var contracts = _context.Contracts
            .Where(c => c.ClientId == dto.ClientId && (!dto.Predicted ? c.IsSigned : true) && !c.IsCancelled);

        var total = await contracts.SumAsync(c => c.FinalPrice);

        var payments = _context.Payments
            .Where(p => p.Contract.ClientId == dto.ClientId && (!dto.Predicted ? p.Contract.IsSigned : true) && !p.Contract.IsCancelled);

        var totalPayments = await payments.SumAsync(p => p.Amount);

        var value = dto.Predicted ? total : totalPayments;
        return await ConvertToCurrencyAsync(value, dto.Currency);
    }

    public async Task<decimal> GetRevenueForProductAsync(RevenueRequestDto dto)
    {
        if (dto.ProductId == 0 || dto.ProductId == null)
            throw new BadHttpRequestException("Product ID is required.");
        
        var productExists = await _context.Products.AnyAsync(p => p.Id == dto.ProductId);
        if (!productExists)
            throw new NotFoundException("Product not found.");
        
        var contracts = _context.Contracts
            .Where(c => c.SoftwareProductId == dto.ProductId && (!dto.Predicted ? c.IsSigned : true) && !c.IsCancelled);

        var total = await contracts.SumAsync(c => c.FinalPrice);

        var payments = _context.Payments
            .Where(p => p.Contract.SoftwareProductId == dto.ProductId && (!dto.Predicted ? p.Contract.IsSigned : true) && !p.Contract.IsCancelled);

        var totalPayments = await payments.SumAsync(p => p.Amount);

        var value = dto.Predicted ? total : totalPayments;
        return await ConvertToCurrencyAsync(value, dto.Currency);
    }

    private async Task<decimal> ConvertToCurrencyAsync(decimal amount, string currency)
    {
        if (currency.ToUpper() == "PLN")
            return amount;

        var rate = await _exchangeRateService.GetExchangeRateAsync("PLN", currency);
        return amount / rate;
    }
}