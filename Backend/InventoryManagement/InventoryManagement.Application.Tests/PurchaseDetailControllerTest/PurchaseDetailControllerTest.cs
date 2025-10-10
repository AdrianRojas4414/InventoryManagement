using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Controllers;
using InventoryManagement.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace InventoryManagement.Application.Tests.PurchaseDetails;

public class PurchaseDetailsControllerTests
{
    private readonly Mock<IPurchaseDetailRepository> _mockRepo;
    private readonly PurchaseDetailsController _controller;

    public PurchaseDetailsControllerTests()
    {
        _mockRepo = new Mock<IPurchaseDetailRepository>();
        _controller = new PurchaseDetailsController(_mockRepo.Object);
    }
    
    // Helper que crea un objeto de prueba válido pero simple
    private PurchaseDetail CreateSampleDetail()
    {
        return new PurchaseDetail
        {
            PurchaseId = 1,
            ProductId = 101,
            Quantity = 5,
            UnitPrice = 10.0m,
            // Se crean objetos mínimos solo para satisfacer las propiedades 'required'
            Purchase = new Purchase { Supplier = new Supplier(), PurchaseDetails = new List<PurchaseDetail>() },
            Product = new Product 
                                { 
                                    Name = "Sample Product",
                                    // Dale un nombre a la categoría de ejemplo
                                    Category = new Category { Name = "Sample Category" }, 
                                    CategoryId = 1 
                                }
        };
    }

    [Fact]
    public async Task GetAllPurchaseDetails_AsAdmin_ReturnsOkWithDetails()
    {
        var detailsList = new List<PurchaseDetail> { CreateSampleDetail() };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(detailsList);

        var result = await _controller.GetAllPurchaseDetails("Admin");

        var okResult = Assert.IsType<OkObjectResult>(result);
        var details = Assert.IsAssignableFrom<List<PurchaseDetail>>(okResult.Value);
        Assert.Single(details);
    }

    [Fact]
    public async Task GetAllPurchaseDetails_AsNonAdmin_ReturnsForbid()
    {
        var result = await _controller.GetAllPurchaseDetails("Employee");
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task GetDetailsByPurchaseId_WhenDetailsExist_ReturnsOkWithDetails()
    {
        var purchaseId = 1;
        var detailsList = new List<PurchaseDetail> { CreateSampleDetail() };
        _mockRepo.Setup(r => r.GetByPurchaseIdAsync(purchaseId)).ReturnsAsync(detailsList);

        var result = await _controller.GetDetailsByPurchaseId(purchaseId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var details = Assert.IsAssignableFrom<List<PurchaseDetail>>(okResult.Value);
        Assert.NotEmpty(details);
    }

    [Theory]
    [InlineData(true)]  // Caso 1: El repositorio devuelve una lista vacía
    [InlineData(false)] // Caso 2: El repositorio devuelve null
    public async Task GetDetailsByPurchaseId_WhenDetailsDoNotExist_ReturnsNotFound(bool returnEmptyList)
    {
        var purchaseId = 99;
        var repoResponse = returnEmptyList ? new List<PurchaseDetail>() : null;
        _mockRepo.Setup(r => r.GetByPurchaseIdAsync(purchaseId)).ReturnsAsync(repoResponse!);

        var result = await _controller.GetDetailsByPurchaseId(purchaseId);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task AddPurchaseDetail_AsAdmin_ReturnsOk()
    {
        var newDetail = CreateSampleDetail();
        
        var result = await _controller.AddPurchaseDetail(newDetail, "Admin");
        
        Assert.IsType<OkObjectResult>(result);
        _mockRepo.Verify(r => r.AddAsync(newDetail), Times.Once);
    }

    [Fact]
    public async Task AddPurchaseDetail_AsNonAdmin_ReturnsForbid()
    {
        var newDetail = CreateSampleDetail();

        var result = await _controller.AddPurchaseDetail(newDetail, "Employee");

        Assert.IsType<ForbidResult>(result);
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<PurchaseDetail>()), Times.Never);
    }

    [Fact]
    public async Task AddPurchaseDetail_WhenRepositoryThrowsException_ReturnsInternalServerError()
    {
        var newDetail = CreateSampleDetail();
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<PurchaseDetail>())).ThrowsAsync(new Exception("Database error"));
        
        var result = await _controller.AddPurchaseDetail(newDetail, "Admin");
        
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }
    
    [Fact]
    public async Task UpdatePurchaseDetail_AsAdmin_ReturnsOk()
    {
        var detailToUpdate = CreateSampleDetail();
        
        var result = await _controller.UpdatePurchaseDetail(detailToUpdate, "Admin");
        
        Assert.IsType<OkObjectResult>(result);
        _mockRepo.Verify(r => r.UpdateAsync(detailToUpdate), Times.Once);
    }

    [Fact]
    public async Task UpdatePurchaseDetail_AsNonAdmin_ReturnsForbid()
    {
        var detailToUpdate = CreateSampleDetail();
        
        var result = await _controller.UpdatePurchaseDetail(detailToUpdate, "Employee");
        
        Assert.IsType<ForbidResult>(result);
        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<PurchaseDetail>()), Times.Never);
    }

    [Fact]
    public async Task UpdatePurchaseDetail_WhenRepositoryThrowsException_ReturnsInternalServerError()
    {
        var detailToUpdate = CreateSampleDetail();
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<PurchaseDetail>())).ThrowsAsync(new Exception("Database error"));
        
        var result = await _controller.UpdatePurchaseDetail(detailToUpdate, "Admin");
        
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }
}