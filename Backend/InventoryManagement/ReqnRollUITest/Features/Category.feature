Feature: Categories Management

Background:
    Given he iniciado sesión como "Admin"
    And navego a la página Productos

# --- CREATE (Insertar) con Pairwise ---
Scenario Outline: Insertar categoría con diferentes datos invalidos desde la página Productos
    When hago click en el botón "Agregar Categoría"
    And ingreso el nombre "<Name>"
    And ingreso la descripción "<Description>"
    And hago click en el botón "Agregar" del formulario
    Then se debe mostrar el mensaje "<ExpectedResult>"

    Examples:
      | Name                | Description                                   | ExpectedResult                                     |
      | [VACIO]             | [VACIO]                                       | El nombre es obligatorio.                          |
      | [VACIO]             | Pro                                           | El nombre es obligatorio.                          |
      | [VACIO]             | Este innovador set de juguetes educativos combina diversión y aprendizaje, incluyendo bloques de construcción, figuras de animales, letras y números, rompecabezas y materiales interactivos diseñados para estimular la creatividad, la coordinación, la motricidad fina y el pensamiento lógico en niños de diferentes edades. Fabricado con materiales seguros y duraderos, su diseño atractivo permite horas de entretenimiento mientras fomenta habilidades cognitivas esenciales, promoviendo el desarrollo integral    | El nombre es obligatorio. |
      | [VACIO]             | Este es un producto para ancianos@$%          | El nombre es obligatorio.                          |
      | [VACIO]             | Este es un producto saludable para los perros | El nombre es obligatorio.                          |
      | Pe                  | Pro                                           | Debe tener al menos 3 caracteres.                  |
      | Pe                  | Este innovador set de juguetes educativos combina diversión y aprendizaje, incluyendo bloques de construcción, figuras de animales, letras y números, rompecabezas y materiales interactivos diseñados para estimular la creatividad, la coordinación, la motricidad fina y el pensamiento lógico en niños de diferentes edades. Fabricado con materiales seguros y duraderos, su diseño atractivo permite horas de entretenimiento mientras fomenta habilidades cognitivas esenciales, promoviendo el desarrollo integral    | Debe tener al menos 3 caracteres.   |
      | Pe                  | Este es un producto para ancianos@$%          | Debe tener al menos 3 caracteres.                  |
      | Pe                  | Este es un producto saludable para los perros | Debe tener al menos 3 caracteres.                  |
      | Pe                  | [VACIO]                                       | Debe tener al menos 3 caracteres.                  |
      | Set completo de juguetes educativos interactivos de madera, plástico y materiales seguros para niños de 3 a 12 años | Este innovador set de juguetes educativos combina diversión y aprendizaje, incluyendo bloques de construcción, figuras de animales, letras y números, rompecabezas y materiales interactivos diseñados para estimular la creatividad, la coordinación, la motricidad fina y el pensamiento lógico en niños de diferentes edades. Fabricado con materiales seguros y duraderos, su diseño atractivo permite horas de entretenimiento mientras fomenta habilidades cognitivas esenciales, promoviendo el desarrollo integral    | No puede tener más de 50 caracteres. |
      | Set completo de juguetes educativos interactivos de madera, plástico y materiales seguros para niños de 3 a 12 años | Este es un producto para ancianos@$%    | No puede tener más de 50 caracteres. |
      | Set completo de juguetes educativos interactivos de madera, plástico y materiales seguros para niños de 3 a 12 años | Este es un producto saludable para los perros   | No puede tener más de 50 caracteres. |
      | Set completo de juguetes educativos interactivos de madera, plástico y materiales seguros para niños de 3 a 12 años | [VACIO]     | No puede tener más de 50 caracteres. |
      | Set completo de juguetes educativos interactivos de madera, plástico y materiales seguros para niños de 3 a 12 años | Pro     | No puede tener más de 50 caracteres. |
      | Comida para perros@ | Este es un producto para ancianos@$%          | No se permiten números, espacios seguidos ni espacios al principio ni el final. |
      | Comida para perros@ | Este es un producto saludable para los perros | No se permiten números, espacios seguidos ni espacios al principio ni el final. |
      | Comida para perros@ | [VACIO]                                       | No se permiten números, espacios seguidos ni espacios al principio ni el final. |
      | Comida para perros@ | Pro                                           | No se permiten números, espacios seguidos ni espacios al principio ni el final. |
      | Comida para perros@ | Este innovador set de juguetes educativos combina diversión y aprendizaje, incluyendo bloques de construcción, figuras de animales, letras y números, rompecabezas y materiales interactivos diseñados para estimular la creatividad, la coordinación, la motricidad fina y el pensamiento lógico en niños de diferentes edades. Fabricado con materiales seguros y duraderos, su diseño atractivo permite horas de entretenimiento mientras fomenta habilidades cognitivas esenciales, promoviendo el desarrollo integral     | No se permiten números, espacios seguidos ni espacios al principio ni el final. |
      | Comida para perros  | [VACIO]                                       | La descripción es obligatoria.                     |
      | Comida para perros  | Pro                                           | Debe tener al menos 5 caracteres.                  |
      | Comida para perros  | Este innovador set de juguetes educativos combina diversión y aprendizaje, incluyendo bloques de construcción, figuras de animales, letras y números, rompecabezas y materiales interactivos diseñados para estimular la creatividad, la coordinación, la motricidad fina y el pensamiento lógico en niños de diferentes edades. Fabricado con materiales seguros y duraderos, su diseño atractivo permite horas de entretenimiento mientras fomenta habilidades cognitivas esenciales, promoviendo el desarrollo integral    | No puede tener más de 500 caracteres. |
      | Comida para perros  | Este es un producto para ancianos@$%          | No se permiten caracteres extraños.                |
   

Scenario: Insertar categoria con datos validos desde la pagina Productos
    When hago click en el botón "Agregar Categoría"
    And ingreso el nombre "Comida para perros"
    And ingreso la descripción "Es un producto para perros"
    And hago click en el botón "Agregar" del formulario
    Then el modal debe cerrarse automaticamente
    And la categoría "Comida para perros" debe aparecer en la tabla

# --- SELECT (Mostrar) Happy Path ---
Scenario: Mostrar el listado de categorías en la página Productos
    Given que existe al menos 1 categoría creada previamente
    Then debe mostrarse la tabla de categorías
    And la tabla debe contener al menos un registro
    And cada registro debe mostrar enlaces "Editar" y "Eliminar"

# --- UPDATE (Editar) ---
Scenario Outline: Editar una categoría existente con datos invalidos
    Given que existe una categoría creada previamente con nombre “Comida para perros” con descripción “Este es un producto saludable para los perros”
    When hago click en el botón “Editar” de la categoría “Comida para perros”
    And actualizo el nombre a "<Name>"
    And actualizo la descripción a "<Description>"
    And hago click en el botón "Actualizar" del formulario
    Then se debe mostrar el mensaje "<ExpectedResult>"

    Examples:
      | Name                | Description                                   | ExpectedResult                                     |
      | [VACIO]             | [VACIO]                                       | El nombre es obligatorio.                          |
      | [VACIO]             | Pro                                           | El nombre es obligatorio.                          |
      | [VACIO]             | Este innovador set de juguetes educativos combina diversión y aprendizaje, incluyendo bloques de construcción, figuras de animales, letras y números, rompecabezas y materiales interactivos diseñados para estimular la creatividad, la coordinación, la motricidad fina y el pensamiento lógico en niños de diferentes edades. Fabricado con materiales seguros y duraderos, su diseño atractivo permite horas de entretenimiento mientras fomenta habilidades cognitivas esenciales, promoviendo el desarrollo integral    | El nombre es obligatorio. |
      | [VACIO]             | Este es un producto para ancianos@$%          | El nombre es obligatorio.                          |
      | [VACIO]             | Este es un producto saludable para los perros | El nombre es obligatorio.                          |
      | Pe                  | Pro                                           | Debe tener al menos 3 caracteres.                  |
      | Pe                  | Este innovador set de juguetes educativos combina diversión y aprendizaje, incluyendo bloques de construcción, figuras de animales, letras y números, rompecabezas y materiales interactivos diseñados para estimular la creatividad, la coordinación, la motricidad fina y el pensamiento lógico en niños de diferentes edades. Fabricado con materiales seguros y duraderos, su diseño atractivo permite horas de entretenimiento mientras fomenta habilidades cognitivas esenciales, promoviendo el desarrollo integral    | Debe tener al menos 3 caracteres.   |
      | Pe                  | Este es un producto para ancianos@$%          | Debe tener al menos 3 caracteres.                  |
      | Pe                  | Este es un producto saludable para los perros | Debe tener al menos 3 caracteres.                  |
      | Pe                  | [VACIO]                                       | Debe tener al menos 3 caracteres.                  |
      | Set completo de juguetes educativos interactivos de madera, plástico y materiales seguros para niños de 3 a 12 años | Este innovador set de juguetes educativos combina diversión y aprendizaje, incluyendo bloques de construcción, figuras de animales, letras y números, rompecabezas y materiales interactivos diseñados para estimular la creatividad, la coordinación, la motricidad fina y el pensamiento lógico en niños de diferentes edades. Fabricado con materiales seguros y duraderos, su diseño atractivo permite horas de entretenimiento mientras fomenta habilidades cognitivas esenciales, promoviendo el desarrollo integral    | No puede tener más de 50 caracteres. |
      | Set completo de juguetes educativos interactivos de madera, plástico y materiales seguros para niños de 3 a 12 años | Este es un producto para ancianos@$%    | No puede tener más de 50 caracteres. |
      | Set completo de juguetes educativos interactivos de madera, plástico y materiales seguros para niños de 3 a 12 años | Este es un producto saludable para los perros   | No puede tener más de 50 caracteres. |
      | Set completo de juguetes educativos interactivos de madera, plástico y materiales seguros para niños de 3 a 12 años | [VACIO]     | No puede tener más de 50 caracteres. |
      | Set completo de juguetes educativos interactivos de madera, plástico y materiales seguros para niños de 3 a 12 años | Pro     | No puede tener más de 50 caracteres. |
      | Comida para perros@ | Este es un producto para ancianos@$%          | No se permiten números, espacios seguidos ni espacios al principio ni el final. |
      | Comida para perros@ | Este es un producto saludable para los perros | No se permiten números, espacios seguidos ni espacios al principio ni el final. |
      | Comida para perros@ | [VACIO]                                       | No se permiten números, espacios seguidos ni espacios al principio ni el final. |
      | Comida para perros@ | Pro                                           | No se permiten números, espacios seguidos ni espacios al principio ni el final. |
      | Comida para perros@ | Este innovador set de juguetes educativos combina diversión y aprendizaje, incluyendo bloques de construcción, figuras de animales, letras y números, rompecabezas y materiales interactivos diseñados para estimular la creatividad, la coordinación, la motricidad fina y el pensamiento lógico en niños de diferentes edades. Fabricado con materiales seguros y duraderos, su diseño atractivo permite horas de entretenimiento mientras fomenta habilidades cognitivas esenciales, promoviendo el desarrollo integral     | No se permiten números, espacios seguidos ni espacios al principio ni el final. |
      | Comida para perros  | [VACIO]                                       | La descripción es obligatoria.                     |
      | Comida para perros  | Pro                                           | Debe tener al menos 5 caracteres.                  |
      | Comida para perros  | Este innovador set de juguetes educativos combina diversión y aprendizaje, incluyendo bloques de construcción, figuras de animales, letras y números, rompecabezas y materiales interactivos diseñados para estimular la creatividad, la coordinación, la motricidad fina y el pensamiento lógico en niños de diferentes edades. Fabricado con materiales seguros y duraderos, su diseño atractivo permite horas de entretenimiento mientras fomenta habilidades cognitivas esenciales, promoviendo el desarrollo integral    | No puede tener más de 500 caracteres. |
      | Comida para perros  | Este es un producto para ancianos@$%          | No se permiten caracteres extraños.                |

Scenario: Editar una categoría existente con datos validos
    Given que existe una categoría creada previamente con nombre “Comida para perros” con descripción “Este es un producto saludable para los perros”
    When hago click en el botón “Editar” de la categoría “Comida para perros”
    And actualizo el nombre a "Comida para gatos"
    And actualizo la descripción a "Este es un producto saludable para los gatos"
    And hago click en el botón "Actualizar" del formulario
    Then el modal debe cerrarse automaticamente
    And la categoria se actualizo correctamente en la tabla

# --- DELETE (Deshabilitar) Happy Path ---
Scenario: Deshabilitar categoría correctamente
    Given existe una categoría activa con nombre “Comida para gatos” y descripción “Este es un producto saludable para los gatos”
    When hago click en el botón Deshabilitar de la categoría “Comida para gatos”
    And confirmo la eliminación en el modal
    Then la categoría “Comida para gatos” ya aparece como “Inactivo” en la tabla