Feature: Purchases Management

Background:
    Given he iniciado sesión como "Admin"
    And navego a la página Compras
    And existe un proveedor activo para compras
    And existen productos activos para compras

# -------------------------------------------------------------------------
# ESCENARIO 1: CREATE - Happy Paths con 1 producto y 3 inserts diferentes
# -------------------------------------------------------------------------
Scenario Outline: Crear compra con un solo producto válido
    When hago click en el botón "Agregar Compra"
    And selecciono el proveedor disponible
    And agrego el producto "<Name>" con cantidad <Cantidad> y precio unitario <Precio>
    And hago click en el botón "Registrar Compra"
    Then el modal de compra debe cerrarse automaticamente
    And la compra debe aparecer en la tabla con total "<ExpectedTotal>"

    Examples:
      | Name             | Cantidad | Precio | ExpectedTotal |
      | Laptop Dell      | 5        | 100.00 | 500,00        |
      | Teclado Mecanico | 10       | 50.00  | 500,00        |
      | Laptop Dell      | 15       | 180.00 | 2700,00       |

# -------------------------------------------------------------------------
# ESCENARIO 2: CREATE - Happy Path con 2 productos
# -------------------------------------------------------------------------
Scenario: Crear compra con múltiples productos válidos
    When hago click en el botón "Agregar Compra"
    And selecciono el proveedor disponible
    And agrego el producto "Laptop Dell" con cantidad 3 y precio unitario 150.00
    And agrego el producto "Teclado Mecanico" con cantidad 10 y precio unitario 50.00
    And hago click en el botón "Registrar Compra"
    Then el modal de compra debe cerrarse automaticamente
    And la compra debe aparecer en la tabla con total "950,00"

# -------------------------------------------------------------------------
# ESCENARIO 3: CREATE - Happy Path con 1 producto y verificación de stock
# -------------------------------------------------------------------------
Scenario: Crear compra y verificar actualización de stock
    Given el producto "Laptop Dell" tiene stock inicial de 10
    When hago click en el botón "Agregar Compra"
    And selecciono el proveedor disponible
    And agrego el producto "Laptop Dell" con cantidad 5 y precio unitario 100.00
    And hago click en el botón "Registrar Compra"
    Then el modal de compra debe cerrarse automaticamente
    And el stock del producto "Laptop Dell" debe ser 15