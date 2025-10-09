using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Controllers;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure.Persistence;
using Xunit;
using Moq;
using InventoryManagement.Application.Interfaces; 


namespace InventoryManagement.Application.Tests.Suppliers
{
    public class SuppliersControllerTests
    {
        private readonly Mock<ISupplierRepository> _mockRepo;
        private readonly SuppliersController _controller;
        public SuppliersControllerTests()
        {
            _mockRepo = new Mock<ISupplierRepository>();
            _controller = new SuppliersController(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllSuppliers_ReturnsOkWithList()
        {
            // Arrange
            var suppliers = new List<Supplier>
        {
            new Supplier { Id = 1, Name = "Proveedor1", Status = 1 },
            new Supplier { Id = 2, Name = "Proveedor2", Status = 1 }
        };

            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(suppliers);
            // Act
            var result = await _controller.GetAllSuppliers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedSuppliers = Assert.IsAssignableFrom<List<Supplier>>(okResult.Value);
            Assert.Equal(2, returnedSuppliers.Count);

        }

        [Fact]
        public async Task GetSupplierById_ReturnsOk_WhenSupplierExists()
        {
            var supplier = new Supplier { Id = 1, Name = "Proveedor1", Status = 1 };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(supplier);

            var result = await _controller.GetSupplierById(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedSupplier = Assert.IsType<Supplier>(okResult.Value);
            Assert.Equal("Proveedor1", returnedSupplier.Name);
        }

        [Fact]
        public async Task GetSupplierById_ReturnsNotFound_WhenSupplierDoesNotExist()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Supplier?)null);

            var result = await _controller.GetSupplierById(99);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetSupplierById_ShouldReturnOk_WhenSupplierExistsButIsInactive()
        {
            // Arrange
            var supplier = new Supplier { Id = 2, Name = "Proveedor1", Status = 0 }; // Estado inactivo

            _mockRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync((Supplier?)null);

            var result = await _controller.GetSupplierById(2);

            Assert.IsType<NotFoundObjectResult>(result);
        }


        [Fact]
        public async Task CreateSupplier_ReturnsCreatedAtAction()
        {
            // Arrange
            var dto = new CreateSupplierDto
            {
                Name = "NuevoProveedorTest",
                Nit = "123",
                Address = "Calle Falsa",
                Phone = "555-1234",
                Email = "proveedortest@test.com",
                ContactName = "Juan Perez"
            };

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Supplier>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateSupplier(dto, 1);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedSupplier = Assert.IsType<Supplier>(createdResult.Value);
            Assert.Equal("NuevoProveedorTest", returnedSupplier.Name);
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<Supplier>()), Times.Once);
        }
        [Fact]
        public async Task UpdateSupplier_ReturnsOk_WhenSupplierExists()
        {
            var existingSupplier = new Supplier { Id = 1, Name = "Proveedor1", Status = 1 };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingSupplier);
            _mockRepo.Setup(r => r.UpdateAsync(existingSupplier)).Returns(Task.CompletedTask);

            var updateDto = new UpdateSupplierDto
            {
                Name = "ProveedorActualizado",
                Nit = "999",
                Address = "Nueva dirección",
                Phone = "555-0000",
                Email = "nuevo@test.com",
                ContactName = "Maria Lopez"
            };

            var result = await _controller.UpdateSupplier(1, updateDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var updatedSupplier = Assert.IsType<Supplier>(okResult.Value);
            Assert.Equal("ProveedorActualizado", updatedSupplier.Name);
            _mockRepo.Verify(r => r.UpdateAsync(existingSupplier), Times.Once);
        }
        [Fact]
        public async Task UpdateSupplier_ReturnsNotFound_WhenSupplierDoesNotExist()
        {
            // Arrange
            var updateDto = new UpdateSupplierDto
            {
                Name = "ProveedorNoExiste",
                Nit = "000",
                Address = "Sin dirección",
                Phone = "000-0000",
                Email = "noexiste@test.com",
                ContactName = "Sin Nombre"
            };

            // Simulamos que el repositorio no encuentra el proveedor
            _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Supplier?)null);

            // Act
            var result = await _controller.UpdateSupplier(99, updateDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Proveedor no encontrado.", notFoundResult.Value);
        }
        [Fact]
        public async Task UpdateSupplier_ReturnsNotFound_WhenSupplierIsInactive()
        {
            // Arrange
            var inactiveSupplier = new Supplier { Id = 2, Name = "ProveedorInactivo", Status = 0 };
            var updateDto = new UpdateSupplierDto
            {
                Name = "IntentoActualizarInactivo",
                Nit = "111",
                Address = "Sin dirección",
                Phone = "000-0000",
                Email = "inactivo@test.com",
                ContactName = "Sin Nombre"
            };

            // Simulamos que el repositorio devuelve el proveedor, pero está inactivo
            _mockRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(inactiveSupplier);

            // Act
            var result = await _controller.UpdateSupplier(2, updateDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Proveedor no encontrado.", notFoundResult.Value);
        }

        [Fact]
        public async Task DeactivateSupplier_ReturnsForbid_WhenUserIsNotAdmin()
        {
            var result = await _controller.DeactivateSupplier(1, "Employee");
            Assert.IsType<ForbidResult>(result);
        }
        [Fact]
        public async Task DeactivateSupplier_ReturnsOk_WhenAdmin()
        {
            var supplier = new Supplier { Id = 1, Name = "Proveedor1", Status = 1 };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(supplier);
            _mockRepo.Setup(r => r.UpdateAsync(supplier)).Returns(Task.CompletedTask);

            var result = await _controller.DeactivateSupplier(1, "Admin");

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(0, supplier.Status);
            _mockRepo.Verify(r => r.UpdateAsync(supplier), Times.Once);
        }
        [Fact]
        public async Task DeactivateSupplier_ReturnsNotFound_WhenAdminButSupplierDoesNotExist()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Supplier?)null); // Proveedor no existe

            // Act
            var result = await _controller.DeactivateSupplier(99, "Admin");

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Proveedor no encontrado.", notFoundResult.Value);
        }

        [Fact]
        public async Task ActivateSupplier_ReturnsOk_WhenAdmin()
        {
            var supplier = new Supplier { Id = 1, Name = "Proveedor1", Status = 0 };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(supplier);
            _mockRepo.Setup(r => r.UpdateAsync(supplier)).Returns(Task.CompletedTask);

            var result = await _controller.ActivateSupplier(1, "Admin");

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(1, supplier.Status);
            _mockRepo.Verify(r => r.UpdateAsync(supplier), Times.Once);
        }
        [Fact]
        public async Task ActivateSupplier_ReturnsForbid_WhenUserIsNotAdmin()
        {
            // Arrange
            var supplier = new Supplier { Id = 1, Name = "Proveedor1", Status = 0 };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(supplier);

            // Act
            var result = await _controller.ActivateSupplier(1, "Employee"); // Usuario no admin

            // Assert
            Assert.IsType<ForbidResult>(result);
            // El estado del proveedor no debe cambiar
            Assert.Equal(0, supplier.Status);
            // No se debe llamar a UpdateAsync
            _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Supplier>()), Times.Never);

        }
        [Fact]
        public async Task ActivateSupplier_ReturnsNotFound_WhenAdminButSupplierDoesNotExist()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Supplier?)null);

            // Act
            var result = await _controller.ActivateSupplier(1, "Admin");

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Proveedor no encontrado.", notFoundResult.Value);

            // Verifica que UpdateAsync no se haya llamado
            _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Supplier>()), Times.Never);
        }

    }
}
