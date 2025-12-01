Feature: Purchases Management

Background:
    Given La base de datos esta disponible
    And Existe un usuario administrador creado
    And Existe una categoria para compras
    And Existe un proveedor para compras

# -------------------------------------------------------------------------
# ESCENARIO 1: CREATE PURCHASE - Happy Path
# -------------------------------------------------------------------------

Scenario: Crear compra valida con actualizacion de stock (Happy Path)
    Given Existen 2 productos para comprar con stock inicial
        | SerialCode | Name             | Stock |
        | 1001       | Laptop Dell      | 10    |
        | 1002       | Teclado Mecanico | 20    |
    When Creo una compra con los siguientes detalles
        | ProductSerialCode | Quantity | UnitPrice |
        | 1001              | 5        | 100.00    |
        | 1002              | 10       | 50.00     |
    Then La respuesta compra debe ser 200 OK
    And La compra debe estar guardada en la base de datos
    And El total de la compra debe ser 1000.00
    And El stock del producto con serialCode 1001 debe ser 15
    And El stock del producto con serialCode 1002 debe ser 30