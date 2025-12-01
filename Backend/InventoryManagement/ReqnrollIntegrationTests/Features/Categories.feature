Feature: Categories Management

Background:
    Given La base de datos esta disponible
    And Existe un usuario administrador creado

# -------------------------------------------------------------------------
# ESCENARIO 1: CREATE - Happy Path
# -------------------------------------------------------------------------

Scenario Outline: Crear categoria valida (Happy Path)
    When Creo una categoria con nombre "<Name>" y descripcion "<Description>"
    Then La respuesta debe ser 200 OK
    And La categoria debe estar guardada en la base de datos

    Examples:
      | Name               | Description                           |
      | Tecnologia         | Productos de tecnologia de gama alta  |
      | Hogar y Jardineria | Productos de decoracion               |

# -------------------------------------------------------------------------
# ESCENARIO 2: CREATE - Unhappy Path
# -------------------------------------------------------------------------

Scenario: Intentar crear categoria con datos invalidos (Unhappy Path)
    When Creo una categoria con nombre "Tecnologia" y descripcion "Esta es una descripci�n extremadamente larga que est� dise�ada para fallar la validaci�n de 500 caracteres. Repetiremos esta frase varias veces para asegurarnos de que el l�mite se exceda con creces. Esta es una descripci�n extremadamente larga que est� dise�ada para fallar la validaci�n de 500 caracteres. Repetiremos esta frase varias veces para asegurarnos de que el l�mite se exceda con creces. Esta es una descripci�n extremadamente larga que est� dise�ada para fallar la validaci�n de 500 caracteres. Repetiremos esta frase varias veces para asegurarnos de que el l�mite se exceda con creces."
    Then La respuesta debe ser 400 Bad Request
    And La categoria no debe estar guardada en la base de datos

# -------------------------------------------------------------------------
# ESCENARIO 3: SELECT - Happy Path
# -------------------------------------------------------------------------

Scenario: Obtener todas las categorias (Happy Path)
    Given Existen 3 categorias creadas previamente
    When Solicito la lista de categorias
    Then La respuesta debe ser 200 OK
    And La lista debe contener al menos 3 categorias

# -------------------------------------------------------------------------
# ESCENARIO 4: UPDATE - Happy Path
# -------------------------------------------------------------------------

Scenario Outline: Actualizar categoria exitosamente (Happy Path)
    Given Existe una categoria creada previamente con nombre "Tecnologia" y descripcion " Productos de Tecnologia"
    When Actualizo la categoria con nombre "<Name>" y descripcion "<Description>"
    Then La respuesta debe ser 200 OK
    And La categoria debe estar actualizada en la base de datos con nombre "<Name>"

    Examples:
      | Name                     | Description                            |
      | Tecnologia (Actualizado) | Productos de tecnologia de gama alta   |
      | Hogar (Actualizado)      | Productos del hogar                    |

# -------------------------------------------------------------------------
# ESCENARIO 5: UPDATE - Unhappy Path
# -------------------------------------------------------------------------

Scenario: Intentar actualizar categoria con datos invalidos (Unhappy Path)
    Given Existe una categoria creada previamente con nombre "Tecnologia" y descripcion " Productos de Tecnologia"
    When Actualizo la categoria con nombre "Tecnologia" y descripcion "Esta es una descripci�n extremadamente larga que est� dise�ada para fallar la validaci�n de 500 caracteres. Repetiremos esta frase varias veces para asegurarnos de que el l�mite se exceda con creces. Esta es una descripci�n extremadamente larga que est� dise�ada para fallar la validaci�n de 500 caracteres. Repetiremos esta frase varias veces para asegurarnos de que el l�mite se exceda con creces. Esta es una descripci�n extremadamente larga que est� dise�ada para fallar la validaci�n de 500 caracteres. Repetiremos esta frase varias veces para asegurarnos de que el l�mite se exceda con creces."
    Then La respuesta debe ser 400 Bad Request
    And La categoria no debe estar actualizada en la base de datos

# -------------------------------------------------------------------------
# ESCENARIO 6: DELETE - Happy Path
# -------------------------------------------------------------------------

Scenario: Eliminar categoria exitosamente (Happy Path)
    When Creo una categoria para eliminar con nombre "Categoria a Eliminar" y descripcion "Descripci�n temporal"
    Then La categoria creada debe tener status 1
    When Elimino la categoria como administrador
    Then La respuesta debe ser 200 OK
    And La categoria debe tener status 0 en la base de datos