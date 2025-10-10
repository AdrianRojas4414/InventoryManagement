using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Controllers;
using InventoryManagement.Domain.Entities;
using System.Globalization;

namespace InventoryManagement.Application.Tests.Purchases;

public class PurchasesControllerTests
{
    private readonly Mock<IPurchaseRepository> _mockRepo;
    private readonly PurchasesController _controller;

    public PurchasesControllerTests()
    {
        _mockRepo = new Mock<IPurchaseRepository>();
        _controller = new PurchasesController(_mockRepo.Object);
    }

    private Purchase GetSamplePurchase()
    {
        var supplier = new Supplier { Id = 1, Name = "TechCorp" };
        var category = new Category { Id = 1, Name = "Laptops" };
        var product = new Product { Id = 101, Name = "Laptop Gamer", CategoryId = 1, Category = category };

        var purchase = new Purchase
        {
            Id = 1,
            SupplierId = supplier.Id,
            Supplier = supplier,
            TotalPurchase = 1500,
            CreationDate = DateTime.UtcNow,
            CreatedByUserId = 4,
            PurchaseDetails = new List<PurchaseDetail>()
        };

        var purchaseDetail = new PurchaseDetail { Purchase = purchase, Product = product, Quantity = 1, UnitPrice = 1500 };
        purchase.PurchaseDetails.Add(purchaseDetail);
        return purchase;
    }

    [Fact]
    public async Task CreatePurchase_WhenCalled_ReturnsOk()
    {
        var purchaseDto = new CreatePurchaseDto { SupplierId = 1, PurchaseDetails = new List<CreatePurchaseDetailDto>() };
        
        var result = await _controller.CreatePurchase(purchaseDto, 4);

        Assert.IsType<OkObjectResult>(result);
        _mockRepo.Verify(r => r.AddAsync(purchaseDto, 4), Times.Once);
    }

    [Fact]
    public async Task CreatePurchase_WhenRepositoryThrowsInvalidOperation_ReturnsBadRequest()
    {
        var purchaseDto = new CreatePurchaseDto { SupplierId = 1, PurchaseDetails = new List<CreatePurchaseDetailDto>() };
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<CreatePurchaseDto>(), It.IsAny<short>())).ThrowsAsync(new InvalidOperationException("Error de operación"));

        var result = await _controller.CreatePurchase(purchaseDto, 4);

        Assert.IsType<BadRequestObjectResult>(result);
    }
    
    [Fact]
    public async Task CreatePurchase_WhenRepositoryThrowsGenericException_ReturnsInternalServerError()
    {
        var purchaseDto = new CreatePurchaseDto { SupplierId = 1, PurchaseDetails = new List<CreatePurchaseDetailDto>() };
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<CreatePurchaseDto>(), It.IsAny<short>())).ThrowsAsync(new Exception("Error genérico"));

        var result = await _controller.CreatePurchase(purchaseDto, 4);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task GetAllPurchases_AsAdmin_ReturnsOkWithMappedPurchases()
    {
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Purchase> { GetSamplePurchase() });

        var result = await _controller.GetAllPurchases("Admin");

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsAssignableFrom<List<PurchaseResponseDto>>(okResult.Value);
        Assert.Single(response);
        Assert.Equal("TechCorp", response[0].Proveedor);
        Assert.Contains("BOB", response[0].Total);
    }

    [Theory]
    [InlineData("Employee")]
    [InlineData("Guest")]
    public async Task GetAllPurchases_AsNonAdmin_ReturnsForbid(string role)
    {
        var result = await _controller.GetAllPurchases(role);

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task GetUserPurchases_WhenCalled_ReturnsOkWithMappedPurchases()
    {
        _mockRepo.Setup(r => r.GetByUserIdAsync(4)).ReturnsAsync(new List<Purchase> { GetSamplePurchase() });
        
        var result = await _controller.GetUserPurchases(4);
        
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsAssignableFrom<List<PurchaseResponseDto>>(okResult.Value);
        Assert.Single(response);
    }

    [Fact]
    public async Task GetPurchaseById_WhenPurchaseNotFound_ReturnsNotFound()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Purchase?)null);

        var result = await _controller.GetPurchaseById(99, 4, "Admin");

        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    [Fact]
    public async Task GetPurchaseById_AsOwner_ReturnsOk()
    {
        var samplePurchase = GetSamplePurchase(); // CreatedByUserId es 4
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(samplePurchase);
        
        var result = await _controller.GetPurchaseById(1, 4, "Employee");
        
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetPurchaseById_AsNonAdminAndNotOwner_ReturnsForbid()
    {
        var samplePurchase = GetSamplePurchase(); // CreatedByUserId es 4
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(samplePurchase);

        var result = await _controller.GetPurchaseById(1, 99, "Employee");

        Assert.IsType<ForbidResult>(result);
    }
    
    [Fact]
    public async Task UpdatePurchase_AsAdmin_ReturnsOk()
    {
        var samplePurchase = GetSamplePurchase();
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(samplePurchase);

        var result = await _controller.UpdatePurchase(1, "Admin");

        Assert.IsType<OkObjectResult>(result);
        _mockRepo.Verify(r => r.UpdateAsync(samplePurchase), Times.Once);
    }
    
    [Theory]
    [InlineData("Employee")]
    [InlineData("Guest")]
    public async Task UpdatePurchase_AsNonAdmin_ReturnsForbid(string role)
    {
        var result = await _controller.UpdatePurchase(1, role);

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task UpdatePurchase_WhenPurchaseNotFound_ReturnsNotFound()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Purchase?)null);
        
        var result = await _controller.UpdatePurchase(99, "Admin");
        
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    [Fact]
    public async Task UpdatePurchase_WhenRepositoryThrowsException_ReturnsInternalServerError()
    {
        var samplePurchase = GetSamplePurchase();
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(samplePurchase);
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Purchase>())).ThrowsAsync(new Exception("Error de DB"));

        var result = await _controller.UpdatePurchase(1, "Admin");

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }
}