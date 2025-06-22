using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.DTOs.Contract;
using Project.Exceptions;
using Project.Models;

namespace Project.Services;

public class ContractService : IContractService
{
    private readonly DatabaseContext _context;
    public ContractService(DatabaseContext context)
    {
        _context = context;
    }
    
    public async Task<decimal> CreateUpfrontContractAsync(CreateContractDto dto)
{
    await using var transaction = await _context.Database.BeginTransactionAsync();

    try
    {
        if ((dto.EndDate - dto.StartDate).TotalDays < 3 || (dto.EndDate - dto.StartDate).TotalDays > 30)
            throw new BadHttpRequestException("Contract duration must be between 3 and 30 days.");

        var client = await _context.Clients
                         .Include(c => c.Contracts)
                         .FirstOrDefaultAsync(c => c.Id == dto.ClientId)
                     ?? throw new NotFoundException("Client not found.");

        var product = await _context.Products
                          .FirstOrDefaultAsync(p => p.Id == dto.SoftwareProductId)
                      ?? throw new NotFoundException("Product not found.");
        
        if(product.UpfrontPrice == null)
            throw new BadHttpRequestException("Product does not have an upfront price.");
        
        var hasActiveContractOrSubscription = await _context.Contracts
            .AnyAsync(c => c.ClientId == dto.ClientId &&
                           c.SoftwareProductId == dto.SoftwareProductId &&
                           !c.IsCancelled &&
                           !c.IsSigned &&
                           c.EndDate >= DateTime.Now);

        if (hasActiveContractOrSubscription)
            throw new ConflictException("Client already has an active contract or subscription for this product.");

        var basePrice = product.UpfrontPrice ?? 0m;

        var validDiscounts = await _context.Discounts
            .Where(d => d.AppliesToUpfront &&
                        d.StartDate <= dto.StartDate &&
                        d.EndDate >= dto.StartDate &&
                        d.Products.Any(p => p.Id == dto.SoftwareProductId))
            .ToListAsync();

        var maxDiscount = validDiscounts.Count > 0 ? validDiscounts.Max(d => d.Percentage) : 0;

        // Loyalty discount
        var isReturningClient = await _context.Contracts.AnyAsync(c => c.ClientId == client.Id && c.IsSigned);
        if (isReturningClient) maxDiscount += 5;

        var priceWithDiscount = basePrice * (1 - (maxDiscount / 100m));

        var extraSupportCost = dto.ExtraSupportYears * 1000;

        var contract = new Contract
        {
            ClientId = client.Id,
            SoftwareProductId = product.Id,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            IsCancelled = false,
            IsSigned = false,
            ExtraSupportYears = dto.ExtraSupportYears,
            FinalPrice = priceWithDiscount + extraSupportCost
        };

        _context.Contracts.Add(contract);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
        return contract.FinalPrice;
    }
    catch (Exception)
    {
        await transaction.RollbackAsync();
        throw;
    }
}
    
    
    public async Task DeleteContractAsync(int contractId)
    {
        var contract = await _context.Contracts.FirstOrDefaultAsync(c => c.Id == contractId);

        if (contract == null)
            throw new NotFoundException("Contract not found.");
        

        _context.Contracts.Remove(contract);
        await _context.SaveChangesAsync();
    }

    public async Task<decimal> AddPaymentAsync(AddPaymentDto dto)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
        var contract = await _context.Contracts
            .Include(c => c.Payments)
            .FirstOrDefaultAsync(c => c.Id == dto.ContractId);

        if (contract == null)
            throw new NotFoundException("Valid contract not found.");
        if (contract.IsCancelled)
            throw new ConflictException("Contract has been cancelled.");
        if (contract.IsSigned)
            throw new ConflictException("Contract has already been signed.");
        if (contract.EndDate < DateTime.Now)
        {
            contract.IsCancelled = true;
            await _context.SaveChangesAsync();
            throw new ConflictException("Contract has expired.");
        }

        var totalPaid = contract.Payments.Sum(p => p.Amount);
        var newTotal = totalPaid + dto.Amount;

        if (newTotal > contract.FinalPrice)
            throw new ConflictException("Payment exceeds contract price.");

        var payment = new Payment
        {
            Amount = dto.Amount,
            ContractId = contract.Id,
            Date = DateTime.Now
        };

        _context.Payments.Add(payment);

        if (newTotal == contract.FinalPrice)
            contract.IsSigned = true;

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
        return contract.FinalPrice - newTotal;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    


}