Feature: Suppliers Management

Background:
    Given he iniciado sesión como "Admin"
    And navego a la página Proveedores

# -------------------------------------------------------------------------
# ESCENARIO 1: CREATE - Unhappy Paths con Pairwise y Valores Limite
# -------------------------------------------------------------------------
Scenario Outline: Insertar proveedor con diferentes datos invalidos desde la página Proveedores
    When hago click en el botón "Agregar Proveedor"
    And ingreso el nombre del proveedor "<Name>"
    And ingreso el nit "<Nit>"
    And ingreso el telefono "<Phone>"
    And ingreso el email "<Email>"
    And ingreso el nombre de contacto "<Contact>"
    And ingreso la direccion "<Address>"
    And hago click en el botón "Guardar" del formulario de proveedor
    Then se debe mostrar el mensaje de proveedor "<ExpectedResult>"

    Examples:
      | Name | Phone | Nit | Email | Contact | Address | ExpectedResult |
      | [VACIO] | [VACIO] | [VACIO] | [VACIO] | [VACIO] | [VACIO] | El nombre es obligatorio. |
      | [VACIO] | 71723456 | 10203040 | ventas@proveedor.bo | 1234567 | Av. Oquendo #555, Edificio "Sol" - @Oficina4 | El nombre es obligatorio. |
      | [VACIO] | 445123 | 10203040-5 | ventasproveedor.com | Ana María de la O | C/1 | El nombre es obligatorio. |
      | [VACIO] | 12345 | 123456-7 | ventas@proveedorcom | A | Esta es una dirección de ejemplo increíblemente larga para exceder el límite máximo de cien caracteres y ver si la validación funciona correctamente. | El nombre es obligatorio. |
      | [VACIO] | 71723456789 | 1234567890123-4 | ventas @proveedor.com | Este es un nombre de contacto extremadamente largo que excede el límite de 50 caracteres | [VACIO] | El nombre es obligatorio. |
      | [VACIO] | 717-23-456 | 1020A304 | a@b.c | José-Luis | Av. Oquendo #555, Edificio "Sol" - @Oficina4 | El nombre es obligatorio. |
      | [VACIO] | [VACIO] | 1020 3040 | un-correo-electronico-demasiado-largo@dominio-gigante.com | [VACIO] | C/1 | El nombre es obligatorio. |
      | 1234567 | 71723456 | 10203040-5 | ventas@proveedorcom | Este es un nombre de contacto extremadamente largo que excede el límite de 50 caracteres | Av. Oquendo #555, Edificio "Sol" - @Oficina4 | Debe tener un formato válido (ej. usuario@dominio.com). |
      | 1234567 | 445123 | 123456-7 | ventas @proveedor.com | José-Luis | C/1 | Formato inválido. Ejemplo: 12345678-9 |
      | 1234567 | 12345 | 1234567890123-4 | a@b.c | [VACIO] | [VACIO] | Formato inválido. Ejemplo: 12345678-9 |
      | 1234567 | 71723456789 | 1020A304 | un-correo-electronico-demasiado-largo@dominio-gigante.com | [VACIO] | Av. Oquendo #555, Edificio "Sol" - @Oficina4 | Formato inválido. Ejemplo: 12345678-9 |
      | 1234567 | 717-23-456 | 1020 3040 | [VACIO] | 1234567 | C/1 | Formato inválido. Ejemplo: 12345678-9 |
      | 1234567 | [VACIO] | [VACIO] | ventas@proveedor.bo | Ana María de la O | Esta es una dirección de ejemplo increíblemente larga para exceder el límite máximo de cien caracteres y ver si la validación funciona correctamente. | El NIT es obligatorio. |
      | 1234567 | [VACIO] | [VACIO] | ventasproveedor.com | A | [VACIO] | El teléfono es obligatorio. |
      | Ferretería 24 Horas+ | 445123 | 1234567890123-4 | un-correo-electronico-demasiado-largo@dominio-gigante.com | 1234567 | Esta es una dirección de ejemplo increíblemente larga para exceder el límite máximo de cien caracteres y ver si la validación funciona correctamente. | No puede ser solo números ni contener caracteres inválidos. |
      | Ferretería 24 Horas+ | 12345 | 1020A304 | [VACIO] | Ana María de la O | [VACIO] | No puede ser solo números ni contener caracteres inválidos. |
      | Ferretería 24 Horas+ | 71723456789 | 1020 3040 | ventas@proveedor.bo | A | Av. Oquendo #555, Edificio "Sol" - @Oficina4 | No puede ser solo números ni contener caracteres inválidos. |
      | Ferretería 24 Horas+ | 717-23-456 | [VACIO] | ventasproveedor.com | Este es un nombre de contacto extremadamente largo que excede el límite de 50 caracteres | C/1 | No puede ser solo números ni contener caracteres inválidos. |
      | Ferretería 24 Horas+ | [VACIO] | 10203040 | ventas@proveedorcom | José-Luis | Esta es una dirección de ejemplo increíblemente larga para exceder el límite máximo de cien caracteres y ver si la validación funciona correctamente. | No puede ser solo números ni contener caracteres inválidos. |
      | Ferretería 24 Horas+ | 71723456 | 10203040-5 | ventas @proveedor.com | [VACIO] | [VACIO] | No puede ser solo números ni contener caracteres inválidos. |
      | Ferretería 24 Horas+ | 445123 | 123456-7 | a@b.c | 1234567 | Av. Oquendo #555, Edificio "Sol" - @Oficina4 | No puede ser solo números ni contener caracteres inválidos. |
      | Sol | 12345 | 1020 3040 | ventas@proveedor.bo | Ana María de la O | [VACIO] | Debe tener al menos 4 caracteres. |
      | Sol | 71723456789 | [VACIO] | ventasproveedor.com | [VACIO] | C/1 | Debe tener al menos 4 caracteres. |
      | Sol | 717-23-456 | 10203040 | ventas@proveedorcom | 1234567 | Esta es una dirección de ejemplo increíblemente larga para exceder el límite máximo de cien caracteres y ver si la validación funciona correctamente. | Debe tener al menos 4 caracteres. |
      | Sol | [VACIO] | 10203040-5 | ventas @proveedor.com | A | [VACIO] | Debe tener al menos 4 caracteres. |
      | Sol | 71723456 | 123456-7 | a@b.c | Este es un nombre de contacto extremadamente largo que excede el límite de 50 caracteres | C/1 | Debe tener al menos 4 caracteres. |
      | Sol | 445123 | 1234567890123-4 | un-correo-electronico-demasiado-largo@dominio-gigante.com | José-Luis | Esta es una dirección de ejemplo increíblemente larga para exceder el límite máximo de cien caracteres y ver si la validación funciona correctamente. | Debe tener al menos 4 caracteres. |
      | Sol | 12345 | 1020A304 | [VACIO] | Ana María de la O | Av. Oquendo #555, Edificio "Sol" - @Oficina4 | Debe tener al menos 4 caracteres. |
      | Este es un nombre de proveedor demasiado largo para ser válido | 717-23-456 | 123456-7 | [VACIO] | Este es un nombre de contacto extremadamente largo que excede el límite de 50 caracteres | Esta es una dirección de ejemplo increíblemente larga para exceder el límite máximo de cien caracteres y ver si la validación funciona correctamente. | Debe tener menos de 31 caracteres. |
      | Este es un nombre de proveedor demasiado largo para ser válido | [VACIO] | 1234567890123-4 | ventas@proveedorcom | José-Luis | [VACIO] | Debe tener menos de 31 caracteres. |
      | Este es un nombre de proveedor demasiado largo para ser válido | 71723456 | 1020A304 | un-correo-electronico-demasiado-largo@dominio-gigante.com | A | [VACIO] | Debe tener menos de 31 caracteres. |
      | Este es un nombre de proveedor demasiado largo para ser válido | 445123 | 1020 3040 | [VACIO] | 1234567 | C/1 | Debe tener menos de 31 caracteres. |
      | Este es un nombre de proveedor demasiado largo para ser válido | 12345 | [VACIO] | ventas@proveedor.bo | Este es un nombre de contacto extremadamente largo que excede el límite de 50 caracteres | Av. Oquendo #555, Edificio "Sol" - @Oficina4 | Debe tener menos de 31 caracteres. |
      | Este es un nombre de proveedor demasiado largo para ser válido | 71723456789 | 10203040 | ventasproveedor.com | Ana María de la O | Esta es una dirección de ejemplo increíblemente larga para exceder el límite máximo de cien caracteres y ver si la validación funciona correctamente. | Debe tener menos de 31 caracteres. |
      | Este es un nombre de proveedor demasiado largo para ser válido | 71723456 | 10203040-5 | a@b.c | [VACIO] | [VACIO] | Debe tener menos de 31 caracteres. |
      | Import-Sur, S.R.L.+ | 445123 | [VACIO] | a@b.c | A | Av. Oquendo #555, Edificio "Sol" - @Oficina4 | No puede ser solo números ni contener caracteres inválidos. |
      | Import-Sur, S.R.L.+ | 12345 | 10203040 | un-correo-electronico-demasiado-largo@dominio-gigante.com | Este es un nombre de contacto extremadamente largo que excede el límite de 50 caracteres | Esta es una dirección de ejemplo increíblemente larga para exceder el límite máximo de cien caracteres y ver si la validación funciona correctamente. | No puede ser solo números ni contener caracteres inválidos. |
      | Import-Sur, S.R.L.+ | 71723456789 | 10203040-5 | [VACIO] | José-Luis | Av. Oquendo #555, Edificio "Sol" - @Oficina4 | No puede ser solo números ni contener caracteres inválidos. |
      | Import-Sur, S.R.L.+ | 717-23-456 | 1234567890123-4 | ventas@proveedor.bo | [VACIO] | [VACIO] | No puede ser solo números ni contener caracteres inválidos. |
      | Import-Sur, S.R.L.+ | [VACIO] | 1020A304 | ventasproveedor.com | 1234567 | [VACIO] | No puede ser solo números ni contener caracteres inválidos. |
      | Import-Sur, S.R.L.+ | 71723456 | 1020 3040 | ventas@proveedorcom | Ana María de la O | C/1 | No puede ser solo números ni contener caracteres inválidos. |
      | Import-Sur, S.R.L.+ | 445123 | 123456-7 | ventas @proveedor.com | [VACIO] | Esta es una dirección de ejemplo increíblemente larga para exceder el límite máximo de cien caracteres y ver si la validación funciona correctamente. | No puede ser solo números ni contener caracteres inválidos. |
      | [VACIO] | 71723456 | 1234567890123-4 | a@b.c | Ana María de la O | C/1 | El nombre es obligatorio. |
      | 1234567 | 717-23-456 | [VACIO] | un-correo-electronico-demasiado-largo@dominio-gigante.com | A | Av. Oquendo #555, Edificio "Sol" - @Oficina4 | El NIT es obligatorio. |
      | Ferretería 24 Horas+ | 71723456789 | 10203040 | a@b.c | José-Luis | [VACIO] | No puede ser solo números ni contener caracteres inválidos. |
      | Sol | [VACIO] | 1020A304 | ventas@proveedor.bo | Este es un nombre de contacto extremadamente largo que excede el límite de 50 caracteres | Av. Oquendo #555, Edificio "Sol" - @Oficina4 | Debe tener al menos 4 caracteres. |
      | Este es un nombre de proveedor demasiado largo para ser válido | 445123 | 123456-7 | ventasproveedor.com | 1234567 | C/1 | Debe tener menos de 31 caracteres. |
      | Import-Sur, S.R.L.+ | 12345 | 10203040-5 | ventas@proveedorcom | 1234567 | Av. Oquendo #555, Edificio "Sol" - @Oficina4 | No puede ser solo números ni contener caracteres inválidos. |
      | [VACIO] | 71723456789 | 1020 3040 | [VACIO] | Ana María de la O | Esta es una dirección de ejemplo increíblemente larga para exceder el límite máximo de cien caracteres y ver si la validación funciona correctamente. | El nombre es obligatorio. |

# -------------------------------------------------------------------------
# ESCENARIO 2: CREATE - Happy Path
# -------------------------------------------------------------------------
Scenario: Insertar proveedor con datos validos desde la pagina Proveedores
    When hago click en el botón "Agregar Proveedor"
    And ingreso el nombre del proveedor "DistribuidoraABC"
    And ingreso el nit "12365487"
    And ingreso el telefono "25662147"
    And ingreso el email "ventas@gmail.com"
    And ingreso el nombre de contacto "Juan Perez"
    And ingreso la direccion "Av. Siempre Viva 123"
    And hago click en el botón "Guardar" del formulario de proveedor
    Then el modal de proveedor debe cerrarse automaticamente
    And el proveedor "Distribuidora ABC" debe aparecer en la tabla

# -------------------------------------------------------------------------
# ESCENARIO 3: SELECT - Happy Path
# -------------------------------------------------------------------------
Scenario: Mostrar el listado de proveedores en la página Proveedores
    Given que existe al menos 1 proveedor creado previamente
    Then debe mostrarse la tabla de proveedores
    And la tabla de proveedores debe contener al menos un registro
    And cada registro de proveedor debe mostrar enlaces "Editar" y "Eliminar"

# -------------------------------------------------------------------------
# ESCENARIO 4: UPDATE - Unhappy Paths con Pairwise y Valores Limite
# -------------------------------------------------------------------------
Scenario Outline: Editar un proveedor existente con datos invalidos
    Given que existe un proveedor creado previamente con nombre "Distribuidora ABC"
    When hago click en el botón "Editar" del proveedor "Distribuidora ABC"
    And actualizo el nombre del proveedor a "<Name>"
    And actualizo el nit a "<Nit>"
    And actualizo el telefono a "<Phone>"
    And actualizo el email a "<Email>"
    And actualizo el nombre de contacto a "<Contact>"
    And actualizo la direccion a "<Address>"
    And hago click en el botón "Guardar Cambios" del formulario de proveedor
    Then se debe mostrar el mensaje de proveedor "<ExpectedResult>"

    Examples:
      | Name | Phone | Nit | Email | Contact | Address | ExpectedResult |
      | [VACIO] | [VACIO] | [VACIO] | [VACIO] | [VACIO] | [VACIO] | El nombre es obligatorio. |
      | [VACIO] | 71723456 | 10203040 | ventas@proveedor.bo | 1234567 | Av. Oquendo #555, Edificio "Sol" - @Oficina4 | El nombre es obligatorio. |
      | [VACIO] | 445123 | 10203040-5 | ventasproveedor.com | Ana María de la O | C/1 | El nombre es obligatorio. |
      | [VACIO] | 12345 | 123456-7 | ventas@proveedorcom | A | Esta es una dirección de ejemplo increíblemente larga para exceder el límite máximo de cien caracteres y ver si la validación funciona correctamente. | El nombre es obligatorio. |
      | [VACIO] | 71723456789 | 1234567890123-4 | ventas @proveedor.com | Este es un nombre de contacto extremadamente largo que excede el límite de 50 caracteres | [VACIO] | El nombre es obligatorio. |
      | [VACIO] | 717-23-456 | 1020A304 | a@b.c | José-Luis | Av. Oquendo #555, Edificio "Sol" - @Oficina4 | El nombre es obligatorio. |
      | [VACIO] | [VACIO] | 1020 3040 | un-correo-electronico-demasiado-largo@dominio-gigante.com | [VACIO] | C/1 | El nombre es obligatorio. |
      | 1234567 | 71723456 | 10203040-5 | ventas@proveedorcom | Este es un nombre de contacto extremadamente largo que excede el límite de 50 caracteres | Av. Oquendo #555, Edificio "Sol" - @Oficina4 | Debe tener un formato válido (ej. usuario@dominio.com). |
      | 1234567 | 445123 | 123456-7 | ventas @proveedor.com | José-Luis | C/1 | Formato inválido. Ejemplo: 12345678-9 |
      | 1234567 | 12345 | 1234567890123-4 | a@b.c | [VACIO] | [VACIO] | Formato inválido. Ejemplo: 12345678-9 |
      | 1234567 | 71723456789 | 1020A304 | un-correo-electronico-demasiado-largo@dominio-gigante.com | [VACIO] | Av. Oquendo #555, Edificio "Sol" - @Oficina4 | Formato inválido. Ejemplo: 12345678-9 |
      | 1234567 | 717-23-456 | 1020 3040 | [VACIO] | 1234567 | C/1 | Formato inválido. Ejemplo: 12345678-9 |
      | 1234567 | [VACIO] | [VACIO] | ventas@proveedor.bo | Ana María de la O | Esta es una dirección de ejemplo increíblemente larga para exceder el límite máximo de cien caracteres y ver si la validación funciona correctamente. | El NIT es obligatorio. |
      | 1234567 | [VACIO] | [VACIO] | ventasproveedor.com | A | [VACIO] | El teléfono es obligatorio. |
      | Ferretería 24 Horas+ | 445123 | 1234567890123-4 | un-correo-electronico-demasiado-largo@dominio-gigante.com | 1234567 | Esta es una dirección de ejemplo increíblemente larga para exceder el límite máximo de cien caracteres y ver si la validación funciona correctamente. | No puede ser solo números ni contener caracteres inválidos. |
      | Ferretería 24 Horas+ | 12345 | 1020A304 | [VACIO] | Ana María de la O | [VACIO] | No puede ser solo números ni contener caracteres inválidos. |
      | Ferretería 24 Horas+ | 71723456789 | 1020 3040 | ventas@proveedor.bo | A | Av. Oquendo #555, Edificio "Sol" - @Oficina4 | No puede ser solo números ni contener caracteres inválidos. |
      | Ferretería 24 Horas+ | 717-23-456 | [VACIO] | ventasproveedor.com | Este es un nombre de contacto extremadamente largo que excede el límite de 50 caracteres | C/1 | No puede ser solo números ni contener caracteres inválidos. |
      | Ferretería 24 Horas+ | [VACIO] | 10203040 | ventas@proveedorcom | José-Luis | Esta es una dirección de ejemplo increíblemente larga para exceder el límite máximo de cien caracteres y ver si la validación funciona correctamente. | No puede ser solo números ni contener caracteres inválidos. |
      | Ferretería 24 Horas+ | 71723456 | 10203040-5 | ventas @proveedor.com | [VACIO] | [VACIO] | No puede ser solo números ni contener caracteres inválidos. |
      | Ferretería 24 Horas+ | 445123 | 123456-7 | a@b.c | 1234567 | Av. Oquendo #555, Edificio "Sol" - @Oficina4 | No puede ser solo números ni contener caracteres inválidos. |
      | Sol | 12345 | 1020 3040 | ventas@proveedor.bo | Ana María de la O | [VACIO] | Debe tener al menos 4 caracteres. |
      | Sol | 71723456789 | [VACIO] | ventasproveedor.com | [VACIO] | C/1 | Debe tener al menos 4 caracteres. |
      | Sol | 717-23-456 | 10203040 | ventas@proveedorcom | 1234567 | Esta es una dirección de ejemplo increíblemente larga para exceder el límite máximo de cien caracteres y ver si la validación funciona correctamente. | Debe tener al menos 4 caracteres. |
      | Sol | [VACIO] | 10203040-5 | ventas @proveedor.com | A | [VACIO] | Debe tener al menos 4 caracteres. |
      | Sol | 71723456 | 123456-7 | a@b.c | Este es un nombre de contacto extremadamente largo que excede el límite de 50 caracteres | C/1 | Debe tener al menos 4 caracteres. |
      | Sol | 445123 | 1234567890123-4 | un-correo-electronico-demasiado-largo@dominio-gigante.com | José-Luis | Esta es una dirección de ejemplo increíblemente larga para exceder el límite máximo de cien caracteres y ver si la validación funciona correctamente. | Debe tener al menos 4 caracteres. |
      | Sol | 12345 | 1020A304 | [VACIO] | Ana María de la O | Av. Oquendo #555, Edificio "Sol" - @Oficina4 | Debe tener al menos 4 caracteres. |
      | Este es un nombre de proveedor demasiado largo para ser válido | 717-23-456 | 123456-7 | [VACIO] | Este es un nombre de contacto extremadamente largo que excede el límite de 50 caracteres | Esta es una dirección de ejemplo increíblemente larga para exceder el límite máximo de cien caracteres y ver si la validación funciona correctamente. | Debe tener menos de 31 caracteres. |
      | Este es un nombre de proveedor demasiado largo para ser válido | [VACIO] | 1234567890123-4 | ventas@proveedorcom | José-Luis | [VACIO] | Debe tener menos de 31 caracteres. |
      | Este es un nombre de proveedor demasiado largo para ser válido | 71723456 | 1020A304 | un-correo-electronico-demasiado-largo@dominio-gigante.com | A | [VACIO] | Debe tener menos de 31 caracteres. |
      | Este es un nombre de proveedor demasiado largo para ser válido | 445123 | 1020 3040 | [VACIO] | 1234567 | C/1 | Debe tener menos de 31 caracteres. |
      | Este es un nombre de proveedor demasiado largo para ser válido | 12345 | [VACIO] | ventas@proveedor.bo | Este es un nombre de contacto extremadamente largo que excede el límite de 50 caracteres | Av. Oquendo #555, Edificio "Sol" - @Oficina4 | Debe tener menos de 31 caracteres. |
      | Este es un nombre de proveedor demasiado largo para ser válido | 71723456789 | 10203040 | ventasproveedor.com | Ana María de la O | Esta es una dirección de ejemplo increíblemente larga para exceder el límite máximo de cien caracteres y ver si la validación funciona correctamente. | Debe tener menos de 31 caracteres. |
      | Este es un nombre de proveedor demasiado largo para ser válido | 71723456 | 10203040-5 | a@b.c | [VACIO] | [VACIO] | Debe tener menos de 31 caracteres. |
      | Import-Sur, S.R.L.+ | 445123 | [VACIO] | a@b.c | A | Av. Oquendo #555, Edificio "Sol" - @Oficina4 | No puede ser solo números ni contener caracteres inválidos. |
      | Import-Sur, S.R.L.+ | 12345 | 10203040 | un-correo-electronico-demasiado-largo@dominio-gigante.com | Este es un nombre de contacto extremadamente largo que excede el límite de 50 caracteres | Esta es una dirección de ejemplo increíblemente larga para exceder el límite máximo de cien caracteres y ver si la validación funciona correctamente. | No puede ser solo números ni contener caracteres inválidos. |
      | Import-Sur, S.R.L.+ | 71723456789 | 10203040-5 | [VACIO] | José-Luis | Av. Oquendo #555, Edificio "Sol" - @Oficina4 | No puede ser solo números ni contener caracteres inválidos. |
      | Import-Sur, S.R.L.+ | 717-23-456 | 1234567890123-4 | ventas@proveedor.bo | [VACIO] | [VACIO] | No puede ser solo números ni contener caracteres inválidos. |
      | Import-Sur, S.R.L.+ | [VACIO] | 1020A304 | ventasproveedor.com | 1234567 | [VACIO] | No puede ser solo números ni contener caracteres inválidos. |
      | Import-Sur, S.R.L.+ | 71723456 | 1020 3040 | ventas@proveedorcom | Ana María de la O | C/1 | No puede ser solo números ni contener caracteres inválidos. |
      | Import-Sur, S.R.L.+ | 445123 | 123456-7 | ventas @proveedor.com | [VACIO] | Esta es una dirección de ejemplo increíblemente larga para exceder el límite máximo de cien caracteres y ver si la validación funciona correctamente. | No puede ser solo números ni contener caracteres inválidos. |
      | [VACIO] | 71723456 | 1234567890123-4 | a@b.c | Ana María de la O | C/1 | El nombre es obligatorio. |
      | 1234567 | 717-23-456 | [VACIO] | un-correo-electronico-demasiado-largo@dominio-gigante.com | A | Av. Oquendo #555, Edificio "Sol" - @Oficina4 | El NIT es obligatorio. |
      | Ferretería 24 Horas+ | 71723456789 | 10203040 | a@b.c | José-Luis | [VACIO] | No puede ser solo números ni contener caracteres inválidos. |
      | Sol | [VACIO] | 1020A304 | ventas@proveedor.bo | Este es un nombre de contacto extremadamente largo que excede el límite de 50 caracteres | Av. Oquendo #555, Edificio "Sol" - @Oficina4 | Debe tener al menos 4 caracteres. |
      | Este es un nombre de proveedor demasiado largo para ser válido | 445123 | 123456-7 | ventasproveedor.com | 1234567 | C/1 | Debe tener menos de 31 caracteres. |
      | Import-Sur, S.R.L.+ | 12345 | 10203040-5 | ventas@proveedorcom | 1234567 | Av. Oquendo #555, Edificio "Sol" - @Oficina4 | No puede ser solo números ni contener caracteres inválidos. |
      | [VACIO] | 71723456789 | 1020 3040 | [VACIO] | Ana María de la O | Esta es una dirección de ejemplo increíblemente larga para exceder el límite máximo de cien caracteres y ver si la validación funciona correctamente. | El nombre es obligatorio. |

# -------------------------------------------------------------------------
# ESCENARIO 5: UPDATE - Happy Path
# -------------------------------------------------------------------------
Scenario: Editar un proveedor existente con datos validos
    Given que existe un proveedor creado previamente con nombre "Distribuidora ABC"
    When hago click en el botón "Editar" del proveedor "Distribuidora ABC"
    And actualizo el nombre del proveedor a "Distribuidora XYZ"
    And actualizo el nit a "12159853215"
    And actualizo el telefono a "63258454"
    And actualizo el email a "contacto@gmail.com"
    And actualizo el nombre de contacto a "Maria Lopez"
    And actualizo la direccion a "Calle Falsa 456"
    And hago click en el botón "Guardar Cambios" del formulario de proveedor
    Then el modal de proveedor debe cerrarse automaticamente
    And el proveedor se actualizo correctamente en la tabla

# -------------------------------------------------------------------------
# ESCENARIO 6: DELETE - Happy Path
# -------------------------------------------------------------------------
Scenario: Deshabilitar proveedor correctamente
    Given existe un proveedor activo con nombre "Distribuidora XYZ"
    When hago click en el botón Deshabilitar del proveedor "Distribuidora XYZ"
    And confirmo la eliminación del proveedor en el modal
    Then el proveedor "Distribuidora XYZ" ya aparece como "Inactivo" en la tabla