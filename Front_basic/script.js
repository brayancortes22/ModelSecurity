// Espera a que el DOM esté completamente cargado
document.addEventListener('DOMContentLoaded', function() {
    // Configuración de navegación entre secciones
    setupNavigation();
    
    // Configuración de botones para mostrar/ocultar formularios
    setupFormToggling();
    
    // Configuración de formularios para crear/editar entidades
    setupFormSubmissions();
    
    // Configuración de carga de datos iniciales
    setupDataLoading();
});

// Función para configurar la navegación entre secciones
function setupNavigation() {
    const navButtons = document.querySelectorAll('.nav-button');
    const sections = document.querySelectorAll('.entity-section');
    
    navButtons.forEach(button => {
        button.addEventListener('click', function() {
            // Remueve la clase 'active' de todos los botones y secciones
            navButtons.forEach(btn => btn.classList.remove('active'));
            sections.forEach(section => section.classList.remove('active'));
            
            // Agrega la clase 'active' al botón clickeado
            this.classList.add('active');
            
            // Encuentra y activa la sección correspondiente
            const sectionId = this.getAttribute('data-section');
            document.getElementById(sectionId).classList.add('active');
        });
    });
}

// Función para configurar la visibilidad de formularios
function setupFormToggling() {
    // Botones para mostrar formularios
    document.getElementById('showAprendizForm').addEventListener('click', () => toggleForm('aprendizForm'));
    document.getElementById('showUsuarioForm').addEventListener('click', () => toggleForm('usuarioForm'));
    document.getElementById('showCentroForm').addEventListener('click', () => toggleForm('centroForm'));
    document.getElementById('showRolForm').addEventListener('click', () => toggleForm('rolForm'));
    document.getElementById('showProgramaForm').addEventListener('click', () => toggleForm('programaForm'));
    document.getElementById('showSedeForm').addEventListener('click', () => toggleForm('sedeForm'));
    document.getElementById('showConceptoForm').addEventListener('click', () => toggleForm('conceptoForm'));
    
    // Botones para cancelar formularios
    document.querySelectorAll('.cancel-button').forEach(button => {
        button.addEventListener('click', function() {
            // Encuentra el formulario padre y lo oculta
            const form = this.closest('.entity-form');
            form.classList.add('hidden');
            
            // Resetea el formulario
            form.querySelector('form').reset();
        });
    });
}

// Función para alternar la visibilidad de un formulario
function toggleForm(formId) {
    const form = document.getElementById(formId);
    form.classList.toggle('hidden');
}

// Función para configurar envío de formularios
function setupFormSubmissions() {
    // Formulario de Aprendiz
    setupFormSubmit('formAprendiz', aprendizFormToDto, apiService.createAprendiz.bind(apiService), apiService.updateAprendiz.bind(apiService), loadAprendices);
    
    // Formulario de Usuario
    setupFormSubmit('formUsuario', usuarioFormToDto, apiService.createUsuario.bind(apiService), apiService.updateUsuario.bind(apiService), loadUsuarios);
    
    // Formulario de Centro
    setupFormSubmit('formCentro', centroFormToDto, apiService.createCentro.bind(apiService), apiService.updateCentro.bind(apiService), loadCentros);
    
    // Formulario de Rol
    setupFormSubmit('formRol', rolFormToDto, apiService.createRol.bind(apiService), apiService.updateRol.bind(apiService), loadRoles);
    
    // Formulario de Programa
    setupFormSubmit('formPrograma', programaFormToDto, apiService.createPrograma.bind(apiService), apiService.updatePrograma.bind(apiService), loadProgramas);
    
    // Formulario de Sede
    setupFormSubmit('formSede', sedeFormToDto, apiService.createSede.bind(apiService), apiService.updateSede.bind(apiService), loadSedes);
    
    // Formulario de Concepto
    setupFormSubmit('formConcepto', conceptoFormToDto, apiService.createConcepto.bind(apiService), apiService.updateConcepto.bind(apiService), loadConceptos);
}

// Función genérica para manejar envío de formularios
function setupFormSubmit(formId, formToDtoFn, createFn, updateFn, reloadDataFn) {
    const form = document.getElementById(formId);
    
    form.addEventListener('submit', async function(e) {
        e.preventDefault();
        
        try {
            const formData = new FormData(form);
            const dto = formToDtoFn(formData);
            
            if (dto.id) {
                // Si tiene ID, es actualización
                await updateFn(dto.id, dto);
                showMessage('Entidad actualizada correctamente');
            } else {
                // Si no tiene ID, es creación
                await createFn(dto);
                showMessage('Entidad creada correctamente');
            }
            
            // Resetea y oculta el formulario
            form.reset();
            form.closest('.entity-form').classList.add('hidden');
            
            // Recarga los datos
            reloadDataFn();
        } catch (error) {
            showError('Error al guardar: ' + error.message);
        }
    });
}

// Funciones de Conversión de Formulario a DTO
function aprendizFormToDto(formData) {
    return {
        id: parseInt(formData.get('id')) || 0,
        userId: parseInt(formData.get('userId')),
        previousProgram: formData.get('previousProgram'),
        active: formData.get('active') === 'on'
    };
}

function usuarioFormToDto(formData) {
    return {
        id: parseInt(formData.get('id')) || 0,
        personId: parseInt(formData.get('personId')),
        username: formData.get('username'),
        password: formData.get('password'),
        active: formData.get('active') === 'on'
    };
}

function centroFormToDto(formData) {
    return {
        id: parseInt(formData.get('id')) || 0,
        name: formData.get('name'),
        regionalId: parseInt(formData.get('regionalId')),
        active: formData.get('active') === 'on'
    };
}

function rolFormToDto(formData) {
    return {
        id: parseInt(formData.get('id')) || 0,
        name: formData.get('name'),
        description: formData.get('description'),
        active: formData.get('active') === 'on'
    };
}

function programaFormToDto(formData) {
    return {
        id: parseInt(formData.get('id')) || 0,
        name: formData.get('name'),
        code: formData.get('code'),
        duration: parseInt(formData.get('duration')),
        active: formData.get('active') === 'on'
    };
}

function sedeFormToDto(formData) {
    return {
        id: parseInt(formData.get('id')) || 0,
        name: formData.get('name'),
        address: formData.get('address'),
        centerId: parseInt(formData.get('centerId')),
        active: formData.get('active') === 'on'
    };
}

function conceptoFormToDto(formData) {
    return {
        id: parseInt(formData.get('id')) || 0,
        name: formData.get('name'),
        description: formData.get('description'),
        active: formData.get('active') === 'on'
    };
}

// Función para configurar la carga de datos
function setupDataLoading() {
    // Configurar botones de carga
    document.getElementById('loadAprendices').addEventListener('click', loadAprendices);
    document.getElementById('loadUsuarios').addEventListener('click', loadUsuarios);
    document.getElementById('loadCentros').addEventListener('click', loadCentros);
    document.getElementById('loadRoles').addEventListener('click', loadRoles);
    document.getElementById('loadProgramas').addEventListener('click', loadProgramas);
    document.getElementById('loadSedes').addEventListener('click', loadSedes);
    document.getElementById('loadConceptos').addEventListener('click', loadConceptos);
}

// Funciones para cargar datos de las diferentes entidades
async function loadAprendices() {
    try {
        const aprendices = await apiService.getAprendices();
        const aprendicesList = document.getElementById('aprendicesList');
        renderEntityList(aprendices, aprendicesList, renderAprendiz, handleAprendizActions);
    } catch (error) {
        showError('Error al cargar aprendices: ' + error.message);
    }
}

async function loadUsuarios() {
    try {
        const usuarios = await apiService.getUsuarios();
        const usuariosList = document.getElementById('usuariosList');
        renderEntityList(usuarios, usuariosList, renderUsuario, handleUsuarioActions);
    } catch (error) {
        showError('Error al cargar usuarios: ' + error.message);
    }
}

async function loadCentros() {
    try {
        const centros = await apiService.getCentros();
        const centrosList = document.getElementById('centrosList');
        renderEntityList(centros, centrosList, renderCentro, handleCentroActions);
    } catch (error) {
        showError('Error al cargar centros: ' + error.message);
    }
}

async function loadRoles() {
    try {
        const roles = await apiService.getRoles();
        const rolesList = document.getElementById('rolesList');
        renderEntityList(roles, rolesList, renderRol, handleRolActions);
    } catch (error) {
        showError('Error al cargar roles: ' + error.message);
    }
}

async function loadProgramas() {
    try {
        const programas = await apiService.getProgramas();
        const programasList = document.getElementById('programasList');
        renderEntityList(programas, programasList, renderPrograma, handleProgramaActions);
    } catch (error) {
        showError('Error al cargar programas: ' + error.message);
    }
}

async function loadSedes() {
    try {
        const sedes = await apiService.getSedes();
        const sedesList = document.getElementById('sedesList');
        renderEntityList(sedes, sedesList, renderSede, handleSedeActions);
    } catch (error) {
        showError('Error al cargar sedes: ' + error.message);
    }
}

async function loadConceptos() {
    try {
        const conceptos = await apiService.getConceptos();
        const conceptosList = document.getElementById('conceptosList');
        renderEntityList(conceptos, conceptosList, renderConcepto, handleConceptoActions);
    } catch (error) {
        showError('Error al cargar conceptos: ' + error.message);
    }
}

// Función genérica para renderizar listas de entidades
function renderEntityList(entities, container, renderFn, actionsFn) {
    // Limpiamos el contenedor
    container.innerHTML = '';
    
    if (!entities || entities.length === 0) {
        container.innerHTML = '<p class="empty-message">No hay elementos para mostrar</p>';
        return;
    }
    
    // Creamos un elemento para cada entidad
    entities.forEach(entity => {
        const itemDiv = document.createElement('div');
        itemDiv.className = 'data-item';
        
        const contentDiv = document.createElement('div');
        contentDiv.className = 'data-content';
        contentDiv.innerHTML = renderFn(entity);
        
        const actionsDiv = document.createElement('div');
        actionsDiv.className = 'entity-actions';
        
        const editButton = document.createElement('button');
        editButton.className = 'edit-button';
        editButton.textContent = 'Editar';
        editButton.addEventListener('click', () => actionsFn.edit(entity));
        
        const deleteButton = document.createElement('button');
        deleteButton.className = 'delete-button';
        deleteButton.textContent = 'Eliminar';
        deleteButton.addEventListener('click', () => actionsFn.delete(entity.id));
        
        actionsDiv.appendChild(editButton);
        actionsDiv.appendChild(deleteButton);
        
        itemDiv.appendChild(contentDiv);
        itemDiv.appendChild(actionsDiv);
        
        container.appendChild(itemDiv);
    });
}

// Funciones para renderizar cada tipo de entidad
function renderAprendiz(aprendiz) {
    return `
        <p><strong>ID:</strong> ${aprendiz.id}</p>
        <p><strong>ID Usuario:</strong> ${aprendiz.userId}</p>
        <p><strong>Programa Anterior:</strong> ${aprendiz.previousProgram || 'N/A'}</p>
        <p><strong>Activo:</strong> ${aprendiz.active ? 'Sí' : 'No'}</p>
    `;
}

function renderUsuario(usuario) {
    return `
        <p><strong>ID:</strong> ${usuario.id}</p>
        <p><strong>ID Persona:</strong> ${usuario.personId}</p>
        <p><strong>Usuario:</strong> ${usuario.username}</p>
        <p><strong>Activo:</strong> ${usuario.active ? 'Sí' : 'No'}</p>
    `;
}

function renderCentro(centro) {
    return `
        <p><strong>ID:</strong> ${centro.id}</p>
        <p><strong>Nombre:</strong> ${centro.name}</p>
        <p><strong>ID Regional:</strong> ${centro.regionalId}</p>
        <p><strong>Activo:</strong> ${centro.active ? 'Sí' : 'No'}</p>
    `;
}

function renderRol(rol) {
    return `
        <p><strong>ID:</strong> ${rol.id}</p>
        <p><strong>Nombre:</strong> ${rol.name}</p>
        <p><strong>Descripción:</strong> ${rol.description || 'N/A'}</p>
        <p><strong>Activo:</strong> ${rol.active ? 'Sí' : 'No'}</p>
    `;
}

function renderPrograma(programa) {
    return `
        <p><strong>ID:</strong> ${programa.id}</p>
        <p><strong>Nombre:</strong> ${programa.name}</p>
        <p><strong>Código:</strong> ${programa.code}</p>
        <p><strong>Duración:</strong> ${programa.duration} horas</p>
        <p><strong>Activo:</strong> ${programa.active ? 'Sí' : 'No'}</p>
    `;
}

function renderSede(sede) {
    return `
        <p><strong>ID:</strong> ${sede.id}</p>
        <p><strong>Nombre:</strong> ${sede.name}</p>
        <p><strong>Dirección:</strong> ${sede.address}</p>
        <p><strong>ID Centro:</strong> ${sede.centerId}</p>
        <p><strong>Activo:</strong> ${sede.active ? 'Sí' : 'No'}</p>
    `;
}

function renderConcepto(concepto) {
    return `
        <p><strong>ID:</strong> ${concepto.id}</p>
        <p><strong>Nombre:</strong> ${concepto.name}</p>
        <p><strong>Descripción:</strong> ${concepto.description || 'N/A'}</p>
        <p><strong>Activo:</strong> ${concepto.active ? 'Sí' : 'No'}</p>
    `;
}

// Funciones para manejar acciones de cada entidad
const handleAprendizActions = {
    edit: (aprendiz) => {
        const form = document.getElementById('formAprendiz');
        document.getElementById('aprendizId').value = aprendiz.id;
        document.getElementById('aprendizUserId').value = aprendiz.userId;
        document.getElementById('aprendizPreviousProgram').value = aprendiz.previousProgram || '';
        document.getElementById('aprendizActive').checked = aprendiz.active;
        
        document.getElementById('aprendizForm').classList.remove('hidden');
    },
    delete: async (id) => {
        if (confirm('¿Estás seguro de que deseas eliminar este aprendiz?')) {
            try {
                await apiService.deleteAprendiz(id);
                showMessage('Aprendiz eliminado correctamente');
                loadAprendices();
            } catch (error) {
                showError('Error al eliminar aprendiz: ' + error.message);
            }
        }
    }
};

const handleUsuarioActions = {
    edit: (usuario) => {
        const form = document.getElementById('formUsuario');
        document.getElementById('usuarioId').value = usuario.id;
        document.getElementById('usuarioPersonId').value = usuario.personId;
        document.getElementById('usuarioUsername').value = usuario.username;
        document.getElementById('usuarioPassword').value = ''; // Por seguridad no mostramos la contraseña
        document.getElementById('usuarioActive').checked = usuario.active;
        
        document.getElementById('usuarioForm').classList.remove('hidden');
    },
    delete: async (id) => {
        if (confirm('¿Estás seguro de que deseas eliminar este usuario?')) {
            try {
                await apiService.deleteUsuario(id);
                showMessage('Usuario eliminado correctamente');
                loadUsuarios();
            } catch (error) {
                showError('Error al eliminar usuario: ' + error.message);
            }
        }
    }
};

const handleCentroActions = {
    edit: (centro) => {
        const form = document.getElementById('formCentro');
        document.getElementById('centroId').value = centro.id;
        document.getElementById('centroName').value = centro.name;
        document.getElementById('centroRegionalId').value = centro.regionalId;
        document.getElementById('centroActive').checked = centro.active;
        
        document.getElementById('centroForm').classList.remove('hidden');
    },
    delete: async (id) => {
        if (confirm('¿Estás seguro de que deseas eliminar este centro?')) {
            try {
                await apiService.deleteCentro(id);
                showMessage('Centro eliminado correctamente');
                loadCentros();
            } catch (error) {
                showError('Error al eliminar centro: ' + error.message);
            }
        }
    }
};

const handleRolActions = {
    edit: (rol) => {
        const form = document.getElementById('formRol');
        document.getElementById('rolId').value = rol.id;
        document.getElementById('rolName').value = rol.name;
        document.getElementById('rolDescription').value = rol.description || '';
        document.getElementById('rolActive').checked = rol.active;
        
        document.getElementById('rolForm').classList.remove('hidden');
    },
    delete: async (id) => {
        if (confirm('¿Estás seguro de que deseas eliminar este rol?')) {
            try {
                await apiService.deleteRol(id);
                showMessage('Rol eliminado correctamente');
                loadRoles();
            } catch (error) {
                showError('Error al eliminar rol: ' + error.message);
            }
        }
    }
};

const handleProgramaActions = {
    edit: (programa) => {
        const form = document.getElementById('formPrograma');
        document.getElementById('programaId').value = programa.id;
        document.getElementById('programaName').value = programa.name;
        document.getElementById('programaCode').value = programa.code;
        document.getElementById('programaDuration').value = programa.duration;
        document.getElementById('programaActive').checked = programa.active;
        
        document.getElementById('programaForm').classList.remove('hidden');
    },
    delete: async (id) => {
        if (confirm('¿Estás seguro de que deseas eliminar este programa?')) {
            try {
                await apiService.deletePrograma(id);
                showMessage('Programa eliminado correctamente');
                loadProgramas();
            } catch (error) {
                showError('Error al eliminar programa: ' + error.message);
            }
        }
    }
};

const handleSedeActions = {
    edit: (sede) => {
        const form = document.getElementById('formSede');
        document.getElementById('sedeId').value = sede.id;
        document.getElementById('sedeName').value = sede.name;
        document.getElementById('sedeAddress').value = sede.address;
        document.getElementById('sedeCenterId').value = sede.centerId;
        document.getElementById('sedeActive').checked = sede.active;
        
        document.getElementById('sedeForm').classList.remove('hidden');
    },
    delete: async (id) => {
        if (confirm('¿Estás seguro de que deseas eliminar esta sede?')) {
            try {
                await apiService.deleteSede(id);
                showMessage('Sede eliminada correctamente');
                loadSedes();
            } catch (error) {
                showError('Error al eliminar sede: ' + error.message);
            }
        }
    }
};

const handleConceptoActions = {
    edit: (concepto) => {
        const form = document.getElementById('formConcepto');
        document.getElementById('conceptoId').value = concepto.id;
        document.getElementById('conceptoName').value = concepto.name;
        document.getElementById('conceptoDescription').value = concepto.description || '';
        document.getElementById('conceptoActive').checked = concepto.active;
        
        document.getElementById('conceptoForm').classList.remove('hidden');
    },
    delete: async (id) => {
        if (confirm('¿Estás seguro de que deseas eliminar este concepto?')) {
            try {
                await apiService.deleteConcepto(id);
                showMessage('Concepto eliminado correctamente');
                loadConceptos();
            } catch (error) {
                showError('Error al eliminar concepto: ' + error.message);
            }
        }
    }
};

// Funciones para mostrar mensajes
function showMessage(message) {
    alert(message); // Versión básica, podría mejorarse con un componente de toast/snackbar
}

function showError(message) {
    alert('Error: ' + message);
}