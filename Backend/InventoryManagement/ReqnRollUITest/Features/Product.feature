Feature: Products Management

Background:
    Given he iniciado sesión como "Admin"
    And navego a la página Productos

# -------------------------------------------------------------------------
# ESCENARIO 1: CREATE - Unhappy Paths con Pairwise y Valores Limite
# -------------------------------------------------------------------------
Scenario Outline: Insertar producto con diferentes datos invalidos desde la página Productos
    When hago click en el botón "Agregar Producto"
    And ingreso el nombre "<Name>"
    And ingreso la descripción "<Description>"
    And ingreso el codigo serial "<SerialCode>"
    And ingreso el total stock "<Stock>"
    And hago click en el botón "Agregar" del formulario
    Then se debe mostrar el mensaje "<ExpectedResult>"

    Examples:
      | Name                 | Description                                 | SerialCode | Stock   | ExpectedResult                                     |
      | [VACIO]              | Pro                                         | 1fdv2      | -12     | El nombre es obligatorio.                          |
      | [VACIO]              | Este innovador set de juguetes educativos combina diversión y aprendizaje, incluyendo bloques de construcción, figuras de animales, letras y números, rompecabezas y materiales interactivos diseñados para estimular la creatividad, la coordinación, la motricidad fina y el pensamiento lógico en niños de diferentes edades. Fabricado con materiales seguros y duraderos, su diseño atractivo permite horas de entretenimiento mientras fomenta habilidades cognitivas esenciales, promoviendo el desarrollo integral | 12 | 32768 | El nombre es obligatorio. |
      | [VACIO]              | Este es un producto para ancianos@$%        | 125421     | 10      | El nombre es obligatorio.                          |
      | [VACIO]              | Este es un producto saludable para ancianos | 32768      | [VACIO] | El nombre es obligatorio.                          |
      | Pe                   | Pro                                         | 12         | 10      | Debe tener al menos 3 caracteres.                  |
      | Pe                   | Este innovador set de juguetes educativos combina diversión y aprendizaje, incluyendo bloques de construcción, figuras de animales, letras y números, rompecabezas y materiales interactivos diseñados para estimular la creatividad, la coordinación, la motricidad fina y el pensamiento lógico en niños de diferentes edades. Fabricado con materiales seguros y duraderos, su diseño atractivo permite horas de entretenimiento mientras fomenta habilidades cognitivas esenciales, promoviendo el desarrollo integral | 125421 | [VACIO] | Debe tener al menos 3 caracteres. |
      | Pe                   | Este es un producto para ancianos@$%        | 32768      | -12     | Debe tener al menos 3 caracteres.                  |
      | Pe                   | Este es un producto saludable para ancianos | 12354      | [VACIO] | Debe tener al menos 3 caracteres.                  |
      | Pe                   | [VACIO]                                     | [VACIO]    | -12     | Debe tener al menos 3 caracteres.                  |
      | Pe                   | [VACIO]                                     | 12354      | 32768   | Debe tener al menos 3 caracteres.                  |
      | Set completo de juguetes educativos interactivos de madera, plástico y materiales seguros para niños de 3 a 12 años | Este innovador set de juguetes educativos combina diversión y aprendizaje, incluyendo bloques de construcción, figuras de animales, letras y números, rompecabezas y materiales interactivos diseñados para estimular la creatividad, la coordinación, la motricidad fina y el pensamiento lógico en niños de diferentes edades. Fabricado con materiales seguros y duraderos, su diseño atractivo permite horas de entretenimiento mientras fomenta habilidades cognitivas esenciales, promoviendo el desarrollo integral | 1fdv2 | [VACIO] | Debe tener un máximo de 100 caracteres. |
      | Set completo de juguetes educativos interactivos de madera, plástico y materiales seguros para niños de 3 a 12 años | Este es un producto para ancianos@$% | 32768 | -12 | Debe tener un máximo de 100 caracteres. |
      | Set completo de juguetes educativos interactivos de madera, plástico y materiales seguros para niños de 3 a 12 años | Este es un producto saludable para ancianos | [VACIO] | 32768 | Debe tener un máximo de 100 caracteres. |
      | Set completo de juguetes educativos interactivos de madera, plástico y materiales seguros para niños de 3 a 12 años | [VACIO] | 1fdv2 | 10 | Debe tener un máximo de 100 caracteres. |
      | Set completo de juguetes educativos interactivos de madera, plástico y materiales seguros para niños de 3 a 12 años | [VACIO] | 12 | [VACIO] | Debe tener un máximo de 100 caracteres. |
      | Set completo de juguetes educativos interactivos de madera, plástico y materiales seguros para niños de 3 a 12 años | Pro | 125421 | -12 | Debe tener un máximo de 100 caracteres. |
      | Laptop$ Dell XPS 14@ | Este es un producto para ancianos@$%        | [VACIO]    | 10      | No se permiten caracteres extraños o sólo números. |
      | Laptop$ Dell XPS 14@ | Este es un producto saludable para ancianos | 1fdv2      | [VACIO] | No se permiten caracteres extraños o sólo números. |
      | Laptop$ Dell XPS 14@ | [VACIO]                                     | 12         | -12     | No se permiten caracteres extraños o sólo números. |
      | Laptop$ Dell XPS 14@ | [VACIO]                                     | 125421     | [VACIO] | No se permiten caracteres extraños o sólo números. |
      | Laptop$ Dell XPS 14@ | Pro                                         | 32768      | -12     | No se permiten caracteres extraños o sólo números. |
      | Laptop$ Dell XPS 14@ | Este innovador set de juguetes educativos combina diversión y aprendizaje, incluyendo bloques de construcción, figuras de animales, letras y números, rompecabezas y materiales interactivos diseñados para estimular la creatividad, la coordinación, la motricidad fina y el pensamiento lógico en niños de diferentes edades. Fabricado con materiales seguros y duraderos, su diseño atractivo permite horas de entretenimiento mientras fomenta habilidades cognitivas esenciales, promoviendo el desarrollo integral | 12354 | 32768 | No se permiten caracteres extraños o sólo números. |
      | Laptop Dell XPS 14   | Este es un producto saludable para ancianos | 12         | [VACIO] | Debe ingresar 5 digitos.                           |
      | Laptop Dell XPS 14   | [VACIO]                                     | 125421     | -12     | La descripción es obligatoria.                     |
      | Laptop Dell XPS 14   | [VACIO]                                     | 32768      | 32768   | La descripción es obligatoria.                     |
      | Laptop Dell XPS 14   | Pro                                         | 12354      | 10      | Debe tener al menos 5 caracteres.                  |
      | Laptop Dell XPS 14   | Este innovador set de juguetes educativos combina diversión y aprendizaje, incluyendo bloques de construcción, figuras de animales, letras y números, rompecabezas y materiales interactivos diseñados para estimular la creatividad, la coordinación, la motricidad fina y el pensamiento lógico en niños de diferentes edades. Fabricado con materiales seguros y duraderos, su diseño atractivo permite horas de entretenimiento mientras fomenta habilidades cognitivas esenciales, promoviendo el desarrollo integral | [VACIO] | [VACIO] | Debe tener un máximo de 500 caracteres. |
      | Laptop Dell XPS 14   | Este es un producto para ancianos@$%        | 1fdv2      | -12     | No se permiten caracteres extraños.                |
      | [VACIO]              | [VACIO]                                     | 32768      | 10      | El nombre es obligatorio.                          |
      | [VACIO]              | [VACIO]                                     | 12354      | [VACIO] | El nombre es obligatorio.                          |
      | [VACIO]              | Pro                                         | [VACIO]    | -12     | El nombre es obligatorio.                          |
      | [VACIO]              | Este innovador set de juguetes educativos combina diversión y aprendizaje, incluyendo bloques de construcción, figuras de animales, letras y números, rompecabezas y materiales interactivos diseñados para estimular la creatividad, la coordinación, la motricidad fina y el pensamiento lógico en niños de diferentes edades. Fabricado con materiales seguros y duraderos, su diseño atractivo permite horas de entretenimiento mientras fomenta habilidades cognitivas esenciales, promoviendo el desarrollo integral | 1fdv2 | [VACIO] | El nombre es obligatorio. |
      | [VACIO]              | Este es un producto para ancianos@$%        | 12         | -12     | El nombre es obligatorio.                          |
      | [VACIO]              | Este es un producto saludable para ancianos | 125421     | 32768   | El nombre es obligatorio.                          |
   
# -------------------------------------------------------------------------
# ESCENARIO 2: CREATE - Happy Path
# -------------------------------------------------------------------------
Scenario: Insertar producto con datos validos desde la pagina Productos
    When hago click en el botón "Agregar Producto"
    And ingreso el nombre "Laptop Dell XPS 15"
    And ingreso la descripción "Este es un producto tecnologico"
    And ingreso el codigo serial "23456"
    And ingreso el total stock "10"
    And hago click en el botón "Agregar" del formulario
    Then el modal debe cerrarse automaticamente
    And el producto "Laptop Dell XPS 15" debe aparecer en la tabla

# -------------------------------------------------------------------------
# ESCENARIO 3: SELECT - Happy Path
# -------------------------------------------------------------------------
Scenario: Mostrar el listado de productos en la página Productos
    Given que existe al menos 1 producto creado previamente
    Then debe mostrarse la tabla de productos
    And la tablas debe contener al menos un registro
    And cada registro debe mostrar enlaces "Editar" y "Eliminar"

# -------------------------------------------------------------------------
# ESCENARIO 4: UPDATE - Unhappy Paths con Pairwise y Valores Limite
# -------------------------------------------------------------------------
Scenario Outline: Editar un producto existente con datos invalidos
    Given que existe un producto creado previamente con nombre “Laptop Dell XPS 15”
    When hago click en el botón “Editar” del producto “Laptop Dell XPS 15”
    And actualizo el nombre a "<Name>"
    And actualizo la descripción a "<Description>"
    And actualizo el codigo serial a "<SerialCode>"
    And actualizo el stock a "<Stock>"
    And hago click en el botón "Actualizar" del formulario
    Then se debe mostrar el mensaje "<ExpectedResult>"

    Examples:
      | Name                 | Description                                 | SerialCode | Stock   | ExpectedResult                                     |
      | [VACIO]              | Pro                                         | 1fdv2      | -12     | El nombre es obligatorio.                          |
      | [VACIO]              | Este innovador set de juguetes educativos combina diversión y aprendizaje, incluyendo bloques de construcción, figuras de animales, letras y números, rompecabezas y materiales interactivos diseñados para estimular la creatividad, la coordinación, la motricidad fina y el pensamiento lógico en niños de diferentes edades. Fabricado con materiales seguros y duraderos, su diseño atractivo permite horas de entretenimiento mientras fomenta habilidades cognitivas esenciales, promoviendo el desarrollo integral | 12 | 32768 | El nombre es obligatorio. |
      | [VACIO]              | Este es un producto para ancianos@$%        | 125421     | 10      | El nombre es obligatorio.                          |
      | [VACIO]              | Este es un producto saludable para ancianos | 32768      | [VACIO] | El nombre es obligatorio.                          |
      | Pe                   | Pro                                         | 12         | 10      | Debe tener al menos 3 caracteres.                  |
      | Pe                   | Este innovador set de juguetes educativos combina diversión y aprendizaje, incluyendo bloques de construcción, figuras de animales, letras y números, rompecabezas y materiales interactivos diseñados para estimular la creatividad, la coordinación, la motricidad fina y el pensamiento lógico en niños de diferentes edades. Fabricado con materiales seguros y duraderos, su diseño atractivo permite horas de entretenimiento mientras fomenta habilidades cognitivas esenciales, promoviendo el desarrollo integral | 125421 | [VACIO] | Debe tener al menos 3 caracteres. |
      | Pe                   | Este es un producto para ancianos@$%        | 32768      | -12     | Debe tener al menos 3 caracteres.                  |
      | Pe                   | Este es un producto saludable para ancianos | 12354      | [VACIO] | Debe tener al menos 3 caracteres.                  |
      | Pe                   | [VACIO]                                     | [VACIO]    | -12     | Debe tener al menos 3 caracteres.                  |
      | Pe                   | [VACIO]                                     | 12354      | 32768   | Debe tener al menos 3 caracteres.                  |
      | Set completo de juguetes educativos interactivos de madera, plástico y materiales seguros para niños de 3 a 12 años | Este innovador set de juguetes educativos combina diversión y aprendizaje, incluyendo bloques de construcción, figuras de animales, letras y números, rompecabezas y materiales interactivos diseñados para estimular la creatividad, la coordinación, la motricidad fina y el pensamiento lógico en niños de diferentes edades. Fabricado con materiales seguros y duraderos, su diseño atractivo permite horas de entretenimiento mientras fomenta habilidades cognitivas esenciales, promoviendo el desarrollo integral | 1fdv2 | [VACIO] | Debe tener un máximo de 100 caracteres. |
      | Set completo de juguetes educativos interactivos de madera, plástico y materiales seguros para niños de 3 a 12 años | Este es un producto para ancianos@$% | 32768 | -12 | Debe tener un máximo de 100 caracteres. |
      | Set completo de juguetes educativos interactivos de madera, plástico y materiales seguros para niños de 3 a 12 años | Este es un producto saludable para ancianos | [VACIO] | 32768 | Debe tener un máximo de 100 caracteres. |
      | Set completo de juguetes educativos interactivos de madera, plástico y materiales seguros para niños de 3 a 12 años | [VACIO] | 1fdv2 | 10 | Debe tener un máximo de 100 caracteres. |
      | Set completo de juguetes educativos interactivos de madera, plástico y materiales seguros para niños de 3 a 12 años | [VACIO] | 12 | [VACIO] | Debe tener un máximo de 100 caracteres. |
      | Set completo de juguetes educativos interactivos de madera, plástico y materiales seguros para niños de 3 a 12 años | Pro | 125421 | -12 | Debe tener un máximo de 100 caracteres. |
      | Laptop$ Dell XPS 14@ | Este es un producto para ancianos@$%        | [VACIO]    | 10      | No se permiten caracteres extraños o sólo números. |
      | Laptop$ Dell XPS 14@ | Este es un producto saludable para ancianos | 1fdv2      | [VACIO] | No se permiten caracteres extraños o sólo números. |
      | Laptop$ Dell XPS 14@ | [VACIO]                                     | 12         | -12     | No se permiten caracteres extraños o sólo números. |
      | Laptop$ Dell XPS 14@ | [VACIO]                                     | 125421     | [VACIO] | No se permiten caracteres extraños o sólo números. |
      | Laptop$ Dell XPS 14@ | Pro                                         | 32768      | -12     | No se permiten caracteres extraños o sólo números. |
      | Laptop$ Dell XPS 14@ | Este innovador set de juguetes educativos combina diversión y aprendizaje, incluyendo bloques de construcción, figuras de animales, letras y números, rompecabezas y materiales interactivos diseñados para estimular la creatividad, la coordinación, la motricidad fina y el pensamiento lógico en niños de diferentes edades. Fabricado con materiales seguros y duraderos, su diseño atractivo permite horas de entretenimiento mientras fomenta habilidades cognitivas esenciales, promoviendo el desarrollo integral | 12354 | 32768 | No se permiten caracteres extraños o sólo números. |
      | Laptop Dell XPS 14   | Este es un producto saludable para ancianos | 12         | [VACIO] | Debe ingresar 5 digitos.                           |
      | Laptop Dell XPS 14   | [VACIO]                                     | 125421     | -12     | La descripción es obligatoria.                     |
      | Laptop Dell XPS 14   | [VACIO]                                     | 32768      | 32768   | La descripción es obligatoria.                     |
      | Laptop Dell XPS 14   | Pro                                         | 12354      | 10      | Debe tener al menos 5 caracteres.                  |
      | Laptop Dell XPS 14   | Este innovador set de juguetes educativos combina diversión y aprendizaje, incluyendo bloques de construcción, figuras de animales, letras y números, rompecabezas y materiales interactivos diseñados para estimular la creatividad, la coordinación, la motricidad fina y el pensamiento lógico en niños de diferentes edades. Fabricado con materiales seguros y duraderos, su diseño atractivo permite horas de entretenimiento mientras fomenta habilidades cognitivas esenciales, promoviendo el desarrollo integral | [VACIO] | [VACIO] | Debe tener un máximo de 500 caracteres. |
      | Laptop Dell XPS 14   | Este es un producto para ancianos@$%        | 1fdv2      | -12     | No se permiten caracteres extraños.                |
      | [VACIO]              | [VACIO]                                     | 32768      | 10      | El nombre es obligatorio.                          |
      | [VACIO]              | [VACIO]                                     | 12354      | [VACIO] | El nombre es obligatorio.                          |
      | [VACIO]              | Pro                                         | [VACIO]    | -12     | El nombre es obligatorio.                          |
      | [VACIO]              | Este innovador set de juguetes educativos combina diversión y aprendizaje, incluyendo bloques de construcción, figuras de animales, letras y números, rompecabezas y materiales interactivos diseñados para estimular la creatividad, la coordinación, la motricidad fina y el pensamiento lógico en niños de diferentes edades. Fabricado con materiales seguros y duraderos, su diseño atractivo permite horas de entretenimiento mientras fomenta habilidades cognitivas esenciales, promoviendo el desarrollo integral | 1fdv2 | [VACIO] | El nombre es obligatorio. |
      | [VACIO]              | Este es un producto para ancianos@$%        | 12         | -12     | El nombre es obligatorio.                          |
      | [VACIO]              | Este es un producto saludable para ancianos | 125421     | 32768   | El nombre es obligatorio.                          |

# -------------------------------------------------------------------------
# ESCENARIO 5: UPDATE - Happy Path
# -------------------------------------------------------------------------
Scenario: Editar un producto existente con datos validos
    Given que existe un producto creado previamente con nombre “Laptop Dell XPS 15”
    When hago click en el botón “Editar” del producto “Laptop Dell XPS 15”
    And actualizo el nombre a "Tablet XPS 15"
    And actualizo la descripción a "Este es un producto tecnologico del 2022"
    And actualizo el codigo serial a "23456"
    And actualizo el stock a "20"
    And hago click en el botón "Guardar Cambios" del formulario
    Then el modal debe cerrarse automaticamente
    And el producto se actualizo correctamente en la tabla

# -------------------------------------------------------------------------
# ESCENARIO 6: DELETE - Happy Path
# -------------------------------------------------------------------------
Scenario: Deshabilitar producto correctamente
    Given existe un producto activo con nombre “Tablet XPS 15”
    When hago click en el botón Deshabilitar del producto “Tablet XPS 15”
    And confirmo la eliminación en el modal
    Then el producto “Tablet XPS 15” ya aparece como “Inactivo” en la tabla