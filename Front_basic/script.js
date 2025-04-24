document.addEventListener('DOMContentLoaded', () => {
    // Navegación entre secciones
    const navButtons = document.querySelectorAll('.nav-button');
    const sections = document.querySelectorAll('.entity-section');

    navButtons.forEach(button => {
        button.addEventListener('click', () => {
            const targetSection = button.getAttribute('data-section');
            
            // Actualiza clase activa en botones
            navButtons.forEach(btn => btn.classList.remove('active'));
            button.classList.add('active');
            
            // Muestra la sección correspondiente
            sections.forEach(section => {
                if (section.id === targetSection) {
                    section.classList.add('active');
                } else {
                    section.classList.remove('active');
                }
            });
        });
    });

    // Configuración de botones y formularios
    setupEntitySection('aprendices', 'aprendiz', '/api/Aprendiz');
    setupEntitySection('usuarios', 'usuario', '/api/User');
    setupEntitySection('centros', 'centro', '/api/Center');
    setupEntitySection('roles', 'rol', '/api/Rol');
    setupEntitySection('programas', 'programa', '/api/Program');
    setupEntitySection('sedes', 'sede', '/api/Sede');
    setupEntitySection('conceptos', 'concepto', '/api/Concept');
    setupEntitySection('personas', 'persona', '/api/Person');
    setupEntitySection('instructores', 'instructor', '/api/Instructor');
    setupEntitySection('modulos', 'modulo', '/api/Module');
    setupEntitySection('procesos', 'proceso', '/api/Process');
    setupEntitySection('regionales', 'regional', '/api/Regional');
    setupEntitySection('estados', 'estado', '/api/State');
    setupEntitySection('tiposModalidad', 'tipoModalidad', '/api/TypeModality');
    setupEntitySection('formularios', 'formulario', '/api/Form');
    setupEntitySection('empresas', 'empresa', '/api/Enterprise');

    // Función general para configurar cada sección de entidad
    function setupEntitySection(sectionId, entityName, apiEndpoint) {
        const section = document.getElementById(sectionId);
        const loadButton = document.getElementById(`load${capitalize(sectionId)}`);
        const showFormButton = document.getElementById(`show${capitalize(entityName)}Form`);
        const entityForm = document.getElementById(`${entityName}Form`);
        const form = document.getElementById(`form${capitalize(entityName)}`);
        const entityList = document.getElementById(`${sectionId}List`);
        const cancelButton = entityForm.querySelector('.cancel-button');

        // Botón para cargar entidades
        loadButton.addEventListener('click', () => {
            fetchEntities(apiEndpoint, entityList, entityName);
        });

        // Botón para mostrar formulario
        showFormButton.addEventListener('click', () => {
            resetForm(form, entityName);
            entityForm.classList.remove('hidden');
        });

        // Botón para cancelar formulario
        cancelButton.addEventListener('click', () => {
            entityForm.classList.add('hidden');
        });

        // Envío del formulario
        form.addEventListener('submit', (e) => {
            e.preventDefault();
            const formData = getFormData(form);
            const id = parseInt(formData.id);
            
            // Determinar si es crear, actualizar completo o actualizar parcial
            if (id > 0) {
                // Si es una edición, preguntamos si quiere hacer actualización parcial (PATCH) o completa (PUT)
                const usePartialUpdate = confirm(`¿Desea realizar una actualización parcial de ${entityName}? 
                - Sí: Solo actualiza los campos modificados (PATCH)
                - No: Actualiza todos los campos (PUT)`);
                
                if (usePartialUpdate) {
                    saveEntityPartial(form, apiEndpoint, entityList, entityForm, entityName, id);
                } else {
                    saveEntity(form, apiEndpoint, entityList, entityForm, entityName);
                }
            } else {
                // Si es creación, siempre usamos POST
                saveEntity(form, apiEndpoint, entityList, entityForm, entityName);
            }
        });
    }

    // Función para obtener entidades de la API
    async function fetchEntities(apiEndpoint, listElement, entityName) {
        try {
            const response = await apiRequest('GET', apiEndpoint);
            displayEntities(response, listElement, entityName, apiEndpoint);
        } catch (error) {
            showError(`Error al cargar ${entityName}s: ${error.message}`);
        }
    }

    // Función para guardar una entidad (crear o actualizar)
    async function saveEntity(form, apiEndpoint, listElement, formContainer, entityName) {
        const formData = getFormData(form);
        const id = parseInt(formData.id);
        const method = id > 0 ? 'PUT' : 'POST';
        const url = id > 0 ? `${apiEndpoint}/${id}` : apiEndpoint;
        
        try {
            await apiRequest(method, url, formData);
            formContainer.classList.add('hidden');
            fetchEntities(apiEndpoint, listElement, entityName);
            showSuccess(`${capitalize(entityName)} guardado correctamente`);
        } catch (error) {
            showError(`Error al guardar ${entityName}: ${error.message}`);
        }
    }

    // Función para guardar parcialmente una entidad (PATCH)
    async function saveEntityPartial(form, apiEndpoint, listElement, formContainer, entityName, id) {
        // Obtiene el formulario actual
        const currentFormData = getFormData(form);
        
        try {
            // Obtenemos los datos originales para comparar
            const originalData = await apiRequest('GET', `${apiEndpoint}/${id}`);
            
            // Creamos un objeto solo con los campos modificados
            const changedData = {};
            // Siempre incluimos el ID
            changedData.id = id;
            
            // Comparamos cada campo para detectar cambios
            Object.keys(currentFormData).forEach(key => {
                // Si es un campo boolean (checkbox)
                if (typeof currentFormData[key] === 'boolean') {
                    if (currentFormData[key] !== originalData[key]) {
                        changedData[key] = currentFormData[key];
                    }
                } 
                // Si es un campo numérico
                else if (typeof currentFormData[key] === 'number') {
                    if (currentFormData[key] !== originalData[key]) {
                        changedData[key] = currentFormData[key];
                    }
                }
                // Para campos string
                else {
                    if (currentFormData[key] !== originalData[key]) {
                        changedData[key] = currentFormData[key];
                    }
                }
            });
            
            // Si no hay cambios, mostramos mensaje y salimos
            if (Object.keys(changedData).length <= 1) { // Solo contiene el ID
                showInfo("No se detectaron cambios para actualizar");
                formContainer.classList.add('hidden');
                return;
            }
            
            // Enviamos solo los campos modificados con PATCH
            await apiRequest('PATCH', `${apiEndpoint}/${id}`, changedData);
            formContainer.classList.add('hidden');
            fetchEntities(apiEndpoint, listElement, entityName);
            showSuccess(`${capitalize(entityName)} actualizado parcialmente`);
            
        } catch (error) {
            showError(`Error al actualizar parcialmente ${entityName}: ${error.message}`);
        }
    }

    // Función para mostrar entidades en una lista
    function displayEntities(entities, listElement, entityName, apiEndpoint) {
        if (!Array.isArray(entities)) {
            showError(`La respuesta del servidor no tiene el formato esperado.`);
            return;
        }

        if (entities.length === 0) {
            listElement.innerHTML = `<p>No se encontraron ${entityName}s.</p>`;
            return;
        }

        // Crear contenedor con scroll horizontal
        let html = '<div class="table-container">';
        html += '<table><thead><tr>';
        
        // Obtener encabezados de la primera entidad
        const firstEntity = entities[0];
        const headers = Object.keys(firstEntity);
        
        // Mostrar encabezados, excepto los que empiezan con '_'
        headers.forEach(header => {
            if (!header.startsWith('_')) {
                html += `<th>${header}</th>`;
            }
        });
        
        // Añadir columna de acciones con clase especial para fijarla
        html += '<th class="actions-column">Acciones</th></tr></thead><tbody>';
        
        // Mostrar datos
        entities.forEach(entity => {
            html += '<tr>';
            headers.forEach(header => {
                if (!header.startsWith('_')) {
                    const value = entity[header];
                    if (typeof value === 'boolean') {
                        html += `<td>${value ? 'Sí' : 'No'}</td>`;
                    } else if (value === null || value === undefined) {
                        html += `<td>-</td>`;
                    } else {
                        html += `<td>${value}</td>`;
                    }
                }
            });
            
            // Columna de acciones con clase especial para fijarla
            html += `<td class="actions-column">
                <button class="edit-button" data-id="${entity.id}" data-entity="${entityName}" data-endpoint="${apiEndpoint}">
                    Editar
                </button>
                <button class="delete-button" data-id="${entity.id}" data-entity="${entityName}" data-endpoint="${apiEndpoint}">
                    Eliminar
                </button>
                <button class="soft-delete-button" data-id="${entity.id}" data-entity="${entityName}" data-endpoint="${apiEndpoint}">
                    Desactivar
                </button>
            </td></tr>`;
        });
        
        html += '</tbody></table></div>';
        listElement.innerHTML = html;
        
        // Agregar event listeners para editar y eliminar
        listElement.querySelectorAll('.edit-button').forEach(button => {
            button.addEventListener('click', () => {
                const id = button.getAttribute('data-id');
                const entity = button.getAttribute('data-entity');
                const endpoint = button.getAttribute('data-endpoint');
                editEntity(id, entity, endpoint);
            });
        });
        
        listElement.querySelectorAll('.delete-button').forEach(button => {
            button.addEventListener('click', () => {
                const id = button.getAttribute('data-id');
                const entity = button.getAttribute('data-entity');
                const endpoint = button.getAttribute('data-endpoint');
                if (confirm(`¿Está seguro que desea ELIMINAR PERMANENTEMENTE este ${entity}? Esta acción no se puede deshacer.`)) {
                    deleteEntity(id, entity, endpoint, listElement);
                }
            });
        });
        
        // Agregar event listeners para soft delete (desactivar)
        listElement.querySelectorAll('.soft-delete-button').forEach(button => {
            button.addEventListener('click', () => {
                const id = button.getAttribute('data-id');
                const entity = button.getAttribute('data-entity');
                const endpoint = button.getAttribute('data-endpoint');
                if (confirm(`¿Está seguro que desea DESACTIVAR este ${entity}? El registro quedará marcado como inactivo pero no se eliminará.`)) {
                    softDeleteEntity(id, entity, endpoint, listElement);
                }
            });
        });
    }

    // Función para editar una entidad
    async function editEntity(id, entityName, apiEndpoint) {
        try {
            const entity = await apiRequest('GET', `${apiEndpoint}/${id}`);
            const form = document.getElementById(`form${capitalize(entityName)}`);
            const formContainer = document.getElementById(`${entityName}Form`);
            
            // Llenar formulario con datos de la entidad
            populateForm(form, entity);
            
            // Mostrar formulario
            formContainer.classList.remove('hidden');
            
            // Hacer scroll al formulario
            formContainer.scrollIntoView({ behavior: 'smooth' });
        } catch (error) {
            showError(`Error al cargar ${entityName} para editar: ${error.message}`);
        }
    }

    // Función para eliminar una entidad
    async function deleteEntity(id, entityName, apiEndpoint, listElement) {
        try {
            await apiRequest('DELETE', `${apiEndpoint}/${id}`);
            fetchEntities(apiEndpoint, listElement, entityName);
            showSuccess(`${capitalize(entityName)} eliminado correctamente`);
        } catch (error) {
            showError(`Error al eliminar ${entityName}: ${error.message}`);
        }
    }
    
    // Función para desactivar una entidad (soft delete)
    async function softDeleteEntity(id, entityName, apiEndpoint, listElement) {
        try {
            // Usar el método softDelete del servicio API
            await apiService.softDelete(apiEndpoint, id);
            fetchEntities(apiEndpoint, listElement, entityName);
            showSuccess(`${capitalize(entityName)} desactivado correctamente`);
        } catch (error) {
            showError(`Error al desactivar ${entityName}: ${error.message}`);
        }
    }

    // Función para obtener los datos del formulario
    function getFormData(form) {
        const formData = {};
        
        // Obtener todos los inputs
        const inputs = form.querySelectorAll('input, select, textarea');
        
        inputs.forEach(input => {
            const name = input.name;
            
            if (!name) return;
            
            if (input.type === 'checkbox') {
                formData[name] = input.checked;
            } else if (input.type === 'number') {
                formData[name] = input.value ? parseInt(input.value) : 0;
            } else {
                formData[name] = input.value;
            }
        });
        
        return formData;
    }

    // Función para llenar formulario con datos
    function populateForm(form, data) {
        const inputs = form.querySelectorAll('input, select, textarea');
        
        inputs.forEach(input => {
            const name = input.name;
            
            if (!name || !(name in data)) return;
            
            if (input.type === 'checkbox') {
                input.checked = Boolean(data[name]);
            } else {
                input.value = data[name] !== null ? data[name] : '';
            }
        });
    }

    // Función para resetear formulario
    function resetForm(form, entityName) {
        form.reset();
        document.getElementById(`${entityName}Id`).value = "0";
    }

    // Utilitarios
    function capitalize(str) {
        return str.charAt(0).toUpperCase() + str.slice(1);
    }

    function showSuccess(message) {
        alert(message); // En una aplicación real, usarías un componente toast
    }
    
    function showInfo(message) {
        alert(`Info: ${message}`); // En una aplicación real, usarías un componente toast
    }

    function showError(message) {
        alert(`Error: ${message}`); // En una aplicación real, usarías un componente toast
    }
});