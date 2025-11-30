Feature: Categories Management

# -------------------------------------------------------------------------
# ESCENARIO 1: Registro de Categorias (Happy Path)
# -------------------------------------------------------------------------

Scenario Outline: Crear categorias validas (Happy Path)
    Given Que soy un usuario autenticado
    When Intento crear una categoria con nombre "<Name>" y descripcion "<Description>"
    Then La respuesta debe ser 201 Created
    
    Examples:
      | Name          | Description       |
      | Electronics   | Gadgets and more  |
      | Home & Garden | Decoration items  |


# -------------------------------------------------------------------------
# ESCENARIO 2: Registro de Categorias (Unhappy Path)
# -------------------------------------------------------------------------

Scenario: Intentar crear categoria sin nombre (Unhappy Path)
    Given Que soy un usuario autenticado
    When Intento crear una categoria con nombre "" y descripcion "Invalid"
    Then La respuesta debe ser 400 Bad Request

# -------------------------------------------------------------------------
# ESCENARIO 3: Actualizacion de Categoria (Happy Path)
# -------------------------------------------------------------------------

Scenario Outline: Actualizar categorias exitosamente (Happy Path)
    Given Existe una categoria con ID 1 llamada "OldName"
    When Actualizo la categoria 1 con nombre "<NewName>"
    Then La respuesta debe ser 204 No Content
    
    Examples:
      | NewName       |
      | Updated Tech  |
      | Updated Home  |

# -------------------------------------------------------------------------
# ESCENARIO 4: Actualizacion de Categoria (Unhappy Path)
# -------------------------------------------------------------------------

Scenario: Intentar actualizar con nombre nulo (Unhappy Path)
    Given Existe una categoria con ID 1
    When Actualizo la categoria 1 con nombre ""
    Then La respuesta debe ser 400 Bad Request

# -------------------------------------------------------------------------
# ESCENARIO 5: Obetener Categorias (Happy Path)
# -------------------------------------------------------------------------

Scenario: Obtener todas las categorias (Happy Path)
    When Solicito la lista de categorias
    Then La respuesta debe ser 200 OK
    And La lista no debe estar vacia

# -------------------------------------------------------------------------
# ESCENARIO 5: Eliminar Categoria (Happy Path)
# -------------------------------------------------------------------------

Scenario: Eliminar una categoria (Happy Path)
    Given Existe una categoria para eliminar
    When Elimino la categoria
    Then La respuesta debe ser 204 No Content