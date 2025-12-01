Feature: Suppliers Management

Background:
    Given La base de datos esta disponible
    And Existe un usuario administrador creado

# -------------------------------------------------------------------------
# ESCENARIO 1: CREATE - Happy Path
# -------------------------------------------------------------------------

Scenario Outline: Crear proveedor valido (Happy Path)
    When Creo un proveedor con nombre "<Name>", nit "<Nit>", telefono "<Phone>", email "<Email>", contacto "<Contact>" y direccion "<Address>"
    Then La respuesta proveedor debe ser 201 Created
    And El proveedor debe estar guardado en la base de datos

    Examples:
      | Name              | Nit        | Phone          | Email              | Contact      | Address                  |
      | Proveedor Tech    | 1234567    | +591 7777777   | tech@test.com      | Juan Perez   | Avenida Siempre Viva 123 |
      | Distribuidora XYZ | 9876543    | +591 6666666   | xyz@test.com       | Maria Lopez  | Calle Los Olivos 456     |

# -------------------------------------------------------------------------
# ESCENARIO 2: CREATE - Unhappy Path
# -------------------------------------------------------------------------

Scenario: Intentar crear proveedor con datos invalidos (Unhappy Path)
    When Creo un proveedor con nombre "Proveedor x", nit "1234569", telefono "telefono muy largo que excede los 25 caracteres", email "ema@test.com", contacto "Juan Robles" y direccion "Avenida Aroma"
    Then La respuesta proveedor debe ser 400 Bad Request
    And El proveedor no debe estar guardado en la base de datos

# -------------------------------------------------------------------------
# ESCENARIO 3: SELECT - Happy Path
# -------------------------------------------------------------------------

Scenario: Obtener todos los proveedores (Happy Path)
    Given Existen 3 proveedores creados previamente
    When Solicito la lista de proveedores
    Then La respuesta proveedor debe ser 200 OK
    And La lista de proveedores debe contener al menos 3 proveedores

# -------------------------------------------------------------------------
# ESCENARIO 4: UPDATE - Happy Path
# -------------------------------------------------------------------------

Scenario Outline: Actualizar proveedor exitosamente (Happy Path)
    Given Existe un proveedor creado previamente con nombre "Proveedor ABC" y nit "1234567890"
    When Actualizo el proveedor con nombre "<NewName>", nit "<NewNit>", telefono "<NewPhone>", email "<NewEmail>", contacto "<NewContact>" y direccion "<NewAddress>"
    Then La respuesta proveedor debe ser 200 OK
    And El proveedor debe estar actualizado en la base de datos con nombre "<NewName>"

    Examples:
      | NewName                   | NewNit     | NewPhone       | NewEmail           | NewContact  | NewAddress          |
      | Proveedor Actualizado     | 9876543    | +591 7654321   | update@test.com    | Pedro V     | Calle Falsa 123     |
      | Distribuidora Nueva       | 5555555    | +591 8888888   | nueva@test.com     | Ana Garcia  | Zona Sur 789        |

# -------------------------------------------------------------------------
# ESCENARIO 5: UPDATE - Unhappy Path
# -------------------------------------------------------------------------

Scenario: Intentar actualizar proveedor con datos invalidos (Unhappy Path)
    Given Existe un proveedor creado previamente con nombre "Proveedor ABC" y nit "1234567890"
    When Actualizo el proveedor con nombre "Proveedor x", nit "1234569", telefono "telefono muy largo que excede los 25 caracteres", email "ema@test.com", contacto "Juan Robles" y direccion "Avenida Aroma"
    Then La respuesta proveedor debe ser 400 Bad Request
    And El proveedor no debe estar actualizado en la base de datos

# -------------------------------------------------------------------------
# ESCENARIO 6: DELETE - Happy Path
# -------------------------------------------------------------------------

Scenario: Eliminar proveedor exitosamente (Happy Path)
    When Creo un proveedor para eliminar con nombre "Proveedor Temporal" y nit "9999999999"
    Then El proveedor creado debe tener status 1
    When Elimino el proveedor como administrador
    Then La respuesta proveedor debe ser 200 OK
    And El proveedor debe tener status 0 en la base de datos