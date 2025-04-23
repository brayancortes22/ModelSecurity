// Constante con la URL base de la API
const API_BASE_URL = "http://localhost:5187";

// Clase para gestionar todas las llamadas a la API
class ApiService {
    constructor(baseUrl) {
        this.baseUrl = baseUrl;
    }

    // Método genérico para hacer peticiones GET
    async get(endpoint) {
        try {
            const response = await fetch(`${this.baseUrl}${endpoint}`);
            if (!response.ok) {
                throw new Error(`Error HTTP: ${response.status}`);
            }
            return await response.json();
        } catch (error) {
            console.error(`Error en petición GET a ${endpoint}:`, error);
            throw error;
        }
    }

    // Método genérico para hacer peticiones POST
    async post(endpoint, data) {
        try {
            const response = await fetch(`${this.baseUrl}${endpoint}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(data)
            });
            if (!response.ok) {
                const errorData = await response.json().catch(() => null);
                throw new Error(errorData?.message || `Error HTTP: ${response.status}`);
            }
            return await response.json();
        } catch (error) {
            console.error(`Error en petición POST a ${endpoint}:`, error);
            throw error;
        }
    }

    // Método genérico para hacer peticiones PUT
    async put(endpoint, id, data) {
        try {
            const response = await fetch(`${this.baseUrl}${endpoint}/${id}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(data)
            });
            if (!response.ok) {
                const errorData = await response.json().catch(() => null);
                throw new Error(errorData?.message || `Error HTTP: ${response.status}`);
            }
            return await response.json();
        } catch (error) {
            console.error(`Error en petición PUT a ${endpoint}/${id}:`, error);
            throw error;
        }
    }

    // Método genérico para hacer peticiones PATCH
    async patch(endpoint, id, data) {
        try {
            const response = await fetch(`${this.baseUrl}${endpoint}/${id}`, {
                method: 'PATCH',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(data)
            });
            if (!response.ok) {
                const errorData = await response.json().catch(() => null);
                throw new Error(errorData?.message || `Error HTTP: ${response.status}`);
            }
            return await response.json();
        } catch (error) {
            console.error(`Error en petición PATCH a ${endpoint}/${id}:`, error);
            throw error;
        }
    }

    // Método genérico para hacer peticiones DELETE
    async delete(endpoint, id) {
        try {
            const response = await fetch(`${this.baseUrl}${endpoint}/${id}`, {
                method: 'DELETE'
            });
            if (!response.ok) {
                const errorData = await response.json().catch(() => null);
                throw new Error(errorData?.message || `Error HTTP: ${response.status}`);
            }
            // Puede no devolver contenido (204 No Content)
            if (response.status !== 204) {
                return await response.json();
            }
            return true;
        } catch (error) {
            console.error(`Error en petición DELETE a ${endpoint}/${id}:`, error);
            throw error;
        }
    }

    // Método para soft delete (eliminación lógica)
    async softDelete(endpoint, id) {
        try {
            const response = await fetch(`${this.baseUrl}${endpoint}/${id}/soft`, {
                method: 'DELETE'
            });
            if (!response.ok) {
                const errorData = await response.json().catch(() => null);
                throw new Error(errorData?.message || `Error HTTP: ${response.status}`);
            }
            // Puede no devolver contenido (204 No Content)
            if (response.status !== 204) {
                return await response.json();
            }
            return true;
        } catch (error) {
            console.error(`Error en petición de soft delete a ${endpoint}/${id}/soft:`, error);
            throw error;
        }
    }

    // ==================== MÉTODOS ESPECÍFICOS POR ENTIDAD ====================

    // ===== APRENDICES =====
    async getAprendices() {
        return this.get('/api/aprendiz');
    }

    async getAprendizById(id) {
        return this.get(`/api/aprendiz/${id}`);
    }

    async createAprendiz(aprendiz) {
        return this.post('/api/aprendiz', aprendiz);
    }

    async updateAprendiz(id, aprendiz) {
        return this.put('/api/aprendiz', id, aprendiz);
    }

    async patchAprendiz(id, aprendiz) {
        return this.patch('/api/aprendiz', id, aprendiz);
    }

    async deleteAprendiz(id) {
        return this.delete('/api/aprendiz', id);
    }

    async softDeleteAprendiz(id) {
        return this.softDelete('/api/aprendiz', id);
    }

    // ===== USUARIOS =====
    async getUsuarios() {
        return this.get('/api/user');
    }

    async getUsuarioById(id) {
        return this.get(`/api/user/${id}`);
    }

    async createUsuario(usuario) {
        return this.post('/api/user', usuario);
    }

    async updateUsuario(id, usuario) {
        return this.put('/api/user', id, usuario);
    }

    async patchUsuario(id, usuario) {
        return this.patch('/api/user', id, usuario);
    }

    async deleteUsuario(id) {
        return this.delete('/api/user', id);
    }

    async softDeleteUsuario(id) {
        return this.softDelete('/api/user', id);
    }

    // ===== CENTROS =====
    async getCentros() {
        return this.get('/api/center');
    }

    async getCentroById(id) {
        return this.get(`/api/center/${id}`);
    }

    async createCentro(centro) {
        return this.post('/api/center', centro);
    }

    async updateCentro(id, centro) {
        return this.put('/api/center', id, centro);
    }

    async patchCentro(id, centro) {
        return this.patch('/api/center', id, centro);
    }

    async deleteCentro(id) {
        return this.delete('/api/center', id);
    }

    async softDeleteCentro(id) {
        return this.softDelete('/api/center', id);
    }

    // ===== ROLES =====
    async getRoles() {
        return this.get('/api/rol');
    }

    async getRolById(id) {
        return this.get(`/api/rol/${id}`);
    }

    async createRol(rol) {
        return this.post('/api/rol', rol);
    }

    async updateRol(id, rol) {
        return this.put('/api/rol', id, rol);
    }

    async patchRol(id, rol) {
        return this.patch('/api/rol', id, rol);
    }

    async deleteRol(id) {
        return this.delete('/api/rol', id);
    }

    async softDeleteRol(id) {
        return this.softDelete('/api/rol', id);
    }

    // ===== PROGRAMAS =====
    async getProgramas() {
        return this.get('/api/program');
    }

    async getProgramaById(id) {
        return this.get(`/api/program/${id}`);
    }

    async createPrograma(programa) {
        return this.post('/api/program', programa);
    }

    async updatePrograma(id, programa) {
        return this.put('/api/program', id, programa);
    }

    async patchPrograma(id, programa) {
        return this.patch('/api/program', id, programa);
    }

    async deletePrograma(id) {
        return this.delete('/api/program', id);
    }

    async softDeletePrograma(id) {
        return this.softDelete('/api/program', id);
    }

    // ===== SEDES =====
    async getSedes() {
        return this.get('/api/sede');
    }

    async getSedeById(id) {
        return this.get(`/api/sede/${id}`);
    }

    async createSede(sede) {
        return this.post('/api/sede', sede);
    }

    async updateSede(id, sede) {
        return this.put('/api/sede', id, sede);
    }

    async patchSede(id, sede) {
        return this.patch('/api/sede', id, sede);
    }

    async deleteSede(id) {
        return this.delete('/api/sede', id);
    }

    async softDeleteSede(id) {
        return this.softDelete('/api/sede', id);
    }

    // ===== CONCEPTOS =====
    async getConceptos() {
        return this.get('/api/concept');
    }

    async getConceptoById(id) {
        return this.get(`/api/concept/${id}`);
    }

    async createConcepto(concepto) {
        return this.post('/api/concept', concepto);
    }

    async updateConcepto(id, concepto) {
        return this.put('/api/concept', id, concepto);
    }

    async patchConcepto(id, concepto) {
        return this.patch('/api/concept', id, concepto);
    }

    async deleteConcepto(id) {
        return this.delete('/api/concept', id);
    }

    async softDeleteConcepto(id) {
        return this.softDelete('/api/concept', id);
    }
    
    // ===== PERSONAS =====
    async getPersonas() {
        return this.get('/api/person');
    }

    async getPersonaById(id) {
        return this.get(`/api/person/${id}`);
    }

    async createPersona(persona) {
        return this.post('/api/person', persona);
    }

    async updatePersona(id, persona) {
        return this.put('/api/person', id, persona);
    }

    async patchPersona(id, persona) {
        return this.patch('/api/person', id, persona);
    }

    async deletePersona(id) {
        return this.delete('/api/person', id);
    }

    async softDeletePersona(id) {
        return this.softDelete('/api/person', id);
    }
    
    // ===== INSTRUCTORES =====
    async getInstructores() {
        return this.get('/api/instructor');
    }

    async getInstructorById(id) {
        return this.get(`/api/instructor/${id}`);
    }

    async createInstructor(instructor) {
        return this.post('/api/instructor', instructor);
    }

    async updateInstructor(id, instructor) {
        return this.put('/api/instructor', id, instructor);
    }

    async patchInstructor(id, instructor) {
        return this.patch('/api/instructor', id, instructor);
    }

    async deleteInstructor(id) {
        return this.delete('/api/instructor', id);
    }

    async softDeleteInstructor(id) {
        return this.softDelete('/api/instructor', id);
    }
    
    // ===== MÓDULOS =====
    async getModulos() {
        return this.get('/api/module');
    }

    async getModuloById(id) {
        return this.get(`/api/module/${id}`);
    }

    async createModulo(modulo) {
        return this.post('/api/module', modulo);
    }

    async updateModulo(id, modulo) {
        return this.put('/api/module', id, modulo);
    }

    async patchModulo(id, modulo) {
        return this.patch('/api/module', id, modulo);
    }

    async deleteModulo(id) {
        return this.delete('/api/module', id);
    }

    async softDeleteModulo(id) {
        return this.softDelete('/api/module', id);
    }
    
    // ===== PROCESOS =====
    async getProcesos() {
        return this.get('/api/process');
    }

    async getProcesoById(id) {
        return this.get(`/api/process/${id}`);
    }

    async createProceso(proceso) {
        return this.post('/api/process', proceso);
    }

    async updateProceso(id, proceso) {
        return this.put('/api/process', id, proceso);
    }

    async patchProceso(id, proceso) {
        return this.patch('/api/process', id, proceso);
    }

    async deleteProceso(id) {
        return this.delete('/api/process', id);
    }

    async softDeleteProceso(id) {
        return this.softDelete('/api/process', id);
    }
    
    // ===== REGIONALES =====
    async getRegionales() {
        return this.get('/api/regional');
    }

    async getRegionalById(id) {
        return this.get(`/api/regional/${id}`);
    }

    async createRegional(regional) {
        return this.post('/api/regional', regional);
    }

    async updateRegional(id, regional) {
        return this.put('/api/regional', id, regional);
    }

    async patchRegional(id, regional) {
        return this.patch('/api/regional', id, regional);
    }

    async deleteRegional(id) {
        return this.delete('/api/regional', id);
    }

    async softDeleteRegional(id) {
        return this.softDelete('/api/regional', id);
    }
    
    // ===== ESTADOS =====
    async getEstados() {
        return this.get('/api/state');
    }

    async getEstadoById(id) {
        return this.get(`/api/state/${id}`);
    }

    async createEstado(estado) {
        return this.post('/api/state', estado);
    }

    async updateEstado(id, estado) {
        return this.put('/api/state', id, estado);
    }

    async patchEstado(id, estado) {
        return this.patch('/api/state', id, estado);
    }

    async deleteEstado(id) {
        return this.delete('/api/state', id);
    }

    async softDeleteEstado(id) {
        return this.softDelete('/api/state', id);
    }
    
    // ===== TIPOS MODALIDAD =====
    async getTiposModalidad() {
        return this.get('/api/typemodality');
    }

    async getTipoModalidadById(id) {
        return this.get(`/api/typemodality/${id}`);
    }

    async createTipoModalidad(tipoModalidad) {
        return this.post('/api/typemodality', tipoModalidad);
    }

    async updateTipoModalidad(id, tipoModalidad) {
        return this.put('/api/typemodality', id, tipoModalidad);
    }

    async patchTipoModalidad(id, tipoModalidad) {
        return this.patch('/api/typemodality', id, tipoModalidad);
    }

    async deleteTipoModalidad(id) {
        return this.delete('/api/typemodality', id);
    }

    async softDeleteTipoModalidad(id) {
        return this.softDelete('/api/typemodality', id);
    }
    
    // ===== FORMULARIOS =====
    async getFormularios() {
        return this.get('/api/form');
    }

    async getFormularioById(id) {
        return this.get(`/api/form/${id}`);
    }

    async createFormulario(formulario) {
        return this.post('/api/form', formulario);
    }

    async updateFormulario(id, formulario) {
        return this.put('/api/form', id, formulario);
    }

    async patchFormulario(id, formulario) {
        return this.patch('/api/form', id, formulario);
    }

    async deleteFormulario(id) {
        return this.delete('/api/form', id);
    }

    async softDeleteFormulario(id) {
        return this.softDelete('/api/form', id);
    }
    
    // ===== EMPRESAS =====
    async getEmpresas() {
        return this.get('/api/enterprise');
    }

    async getEmpresaById(id) {
        return this.get(`/api/enterprise/${id}`);
    }

    async createEmpresa(empresa) {
        return this.post('/api/enterprise', empresa);
    }

    async updateEmpresa(id, empresa) {
        return this.put('/api/enterprise', id, empresa);
    }

    async patchEmpresa(id, empresa) {
        return this.patch('/api/enterprise', id, empresa);
    }

    async deleteEmpresa(id) {
        return this.delete('/api/enterprise', id);
    }

    async softDeleteEmpresa(id) {
        return this.softDelete('/api/enterprise', id);
    }
}

// Instancia global del servicio de API
const apiService = new ApiService(API_BASE_URL);

// Función global para simplificar las llamadas a la API desde script.js
async function apiRequest(method, endpoint, data = null) {
    method = method.toUpperCase();
    
    try {
        switch (method) {
            case 'GET':
                return await apiService.get(endpoint);
            case 'POST':
                return await apiService.post(endpoint, data);
            case 'PUT':
                const id = endpoint.split('/').pop();
                const basePath = endpoint.substring(0, endpoint.lastIndexOf('/'));
                return await apiService.put(basePath, id, data);
            case 'PATCH':
                const patchId = endpoint.split('/').pop();
                const patchBasePath = endpoint.substring(0, endpoint.lastIndexOf('/'));
                return await apiService.patch(patchBasePath, patchId, data);
            case 'DELETE':
                const deleteId = endpoint.split('/').pop();
                const deleteBasePath = endpoint.substring(0, endpoint.lastIndexOf('/'));
                return await apiService.delete(deleteBasePath, deleteId);
            default:
                throw new Error(`Método HTTP no soportado: ${method}`);
        }
    } catch (error) {
        console.error(`Error en apiRequest (${method} ${endpoint}):`, error);
        throw error;
    }
}