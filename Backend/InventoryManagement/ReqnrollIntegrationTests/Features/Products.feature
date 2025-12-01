Feature: Products Management

Background:
    Given La base de datos esta disponible
    And Existe un usuario administrador creado
    And Existe una categoria creada previamente

# -------------------------------------------------------------------------
# ESCENARIO 1: CREATE - Happy Path
# -------------------------------------------------------------------------

Scenario Outline: Crear producto valido (Happy Path)
    When Creo un producto con serialCode <SerialCode>, nombre "<Name>", descripcion "<Description>" y stock <Stock>
    Then La respuesta producto debe ser 200 OK
    And El producto debe estar guardado en la base de datos

    Examples:
      | SerialCode | Name              | Description                  | Stock |
      | 1001       | Laptop Pro        | Laptop de alta gama          | 10    |
      | 1002       | Mouse Gaming      | Mouse con iluminacion RGB    | 50    |

# -------------------------------------------------------------------------
# ESCENARIO 2: CREATE - Unhappy Path
# -------------------------------------------------------------------------

Scenario: Intentar crear producto con datos invalidos (Unhappy Path)
    When Creo un producto con serialCode 10031, nombre "Laptop Pro", descripcion "Laptop de Gama Alta" y stock -5
    Then La respuesta producto debe ser 400 Bad Request
    And El producto no debe estar guardado en la base de datos

# -------------------------------------------------------------------------
# ESCENARIO 3: SELECT - Happy Path
# -------------------------------------------------------------------------

Scenario: Obtener todos los productos (Happy Path)
    Given Existen 3 productos creados previamente
    When Solicito la lista de productos
    Then La respuesta producto debe ser 200 OK
    And La lista de productos debe contener al menos 3 productos

# -------------------------------------------------------------------------
# ESCENARIO 4: UPDATE - Happy Path
# -------------------------------------------------------------------------

Scenario Outline: Actualizar producto exitosamente (Happy Path)
    Given Existe un producto creado previamente con serialCode 1001 y nombre "Laptop Dell"
    When Actualizo el producto con serialCode <NewSerialCode>, nombre "<NewName>", descripcion "<NewDescription>" y stock <NewStock>
    Then La respuesta producto debe ser 200 OK
    And El producto debe estar actualizado en la base de datos con nombre "<NewName>"

    Examples:
      | NewSerialCode | NewName                  | NewDescription           | NewStock |
      | 5432          | Laptop Actualizado       | Descripcion actualizada  | 50       |
      | 6789          | Laptop HP                | Nueva laptop empresarial | 25       |

# -------------------------------------------------------------------------
# ESCENARIO 5: UPDATE - Unhappy Path
# -------------------------------------------------------------------------

Scenario: Intentar actualizar producto con datos invalidos (Unhappy Path)
    Given Existe un producto creado previamente con serialCode 1001 y nombre "Laptop Dell"
    When Actualizo el producto con serialCode -10, nombre "Laptop actualizado", descripcion "Descripcion actualizada" y stock 100
    Then La respuesta producto debe ser 400 Bad Request
    And El producto no debe estar actualizado en la base de datos

# -------------------------------------------------------------------------
# ESCENARIO 6: DELETE - Happy Path
# -------------------------------------------------------------------------

Scenario: Eliminar producto exitosamente (Happy Path)
    When Creo un producto para eliminar con serialCode 9999, nombre "Producto Temporal" y descripcion "Para eliminar"
    Then El producto creado debe tener status 1
    When Elimino el producto como administrador
    Then La respuesta producto debe ser 200 OK
    And El producto debe tener status 0 en la base de datos