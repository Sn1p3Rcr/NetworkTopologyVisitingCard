import {
    getAuth, setAuth, clearAuth, isAdmin, isAuthenticated,
    authApi, projectsApi, reviewsApi, technologiesApi, topologiesApi
} from './api.js';

const { createApp, ref, computed, onMounted, watch } = Vue;

createApp({
    setup() {
        const authenticated = ref(isAuthenticated());
        const auth = ref(getAuth());
        const section = ref('dashboard');
        const loading = ref(false);
        const toast = ref({ show: false, message: '', type: 'success' });

        const loginForm = ref({ email: 'admin@topology.com', password: 'Admin123!' });
        const registerForm = ref({ email: '', password: '', fullName: '' });
        const authMode = ref('login');

        const projects = ref([]);
        const reviews = ref([]);
        const technologies = ref([]);
        const topologies = ref([]);

        const reviewProjectFilter = ref('');
        const topologyProjectFilter = ref('');

        const modal = ref({ show: false, title: '', entity: '', mode: 'create', data: {} });

        const stats = computed(() => ({
            projects: projects.value.length,
            reviews: reviews.value.length,
            technologies: technologies.value.length,
            topologies: topologies.value.length
        }));

        const filteredReviews = computed(() => {
            if (!reviewProjectFilter.value) return reviews.value;
            const id = Number(reviewProjectFilter.value);
            return reviews.value.filter(r => r.projectId === id);
        });

        const filteredTopologies = computed(() => {
            if (!topologyProjectFilter.value) return topologies.value;
            const id = Number(topologyProjectFilter.value);
            return topologies.value.filter(t => t.projectId === id);
        });

        const canWrite = computed(() => authenticated.value);
        const canAdmin = computed(() => isAdmin());

        function showToast(message, type = 'success') {
            toast.value = { show: true, message, type };
            setTimeout(() => { toast.value.show = false; }, 3500);
        }

        async function runAsync(fn) {
            loading.value = true;
            try {
                await fn();
            } catch (e) {
                showToast(e.message || 'Ошибка запроса', 'danger');
                if (!isAuthenticated()) authenticated.value = false;
            } finally {
                loading.value = false;
            }
        }

        async function loadAll() {
            const [p, r, t, top] = await Promise.all([
                projectsApi.list(),
                reviewsApi.list(),
                technologiesApi.list(),
                topologiesApi.list()
            ]);
            projects.value = p;
            reviews.value = r;
            technologies.value = t;
            topologies.value = top;
        }

        async function login() {
            await runAsync(async () => {
                const result = await authApi.login(loginForm.value.email, loginForm.value.password);
                setAuth({ token: result.token, email: result.email, roles: result.roles, expires: result.expires });
                auth.value = getAuth();
                authenticated.value = true;
                await loadAll();
                showToast('Вход выполнен');
            });
        }

        async function register() {
            await runAsync(async () => {
                const result = await authApi.register(
                    registerForm.value.email,
                    registerForm.value.password,
                    registerForm.value.fullName
                );
                setAuth({ token: result.token, email: result.email, roles: result.roles, expires: result.expires });
                auth.value = getAuth();
                authenticated.value = true;
                await loadAll();
                showToast('Регистрация успешна');
            });
        }

        function logout() {
            clearAuth();
            auth.value = null;
            authenticated.value = false;
            projects.value = [];
            reviews.value = [];
            technologies.value = [];
            topologies.value = [];
            section.value = 'dashboard';
        }

        function openModal(entity, mode, item = null) {
            const defaults = {
                project: { title: '', description: '', technologies: '', imageUrl: '', author: '' },
                review: { authorName: '', content: '', rating: 5, projectId: projects.value[0]?.id || 0 },
                technology: { name: '', description: '', category: '' },
                topology: { name: '', description: '', topologyData: '{"nodes":[],"edges":[]}', projectId: projects.value[0]?.id || 0 }
            };
            modal.value = {
                show: true,
                entity,
                mode,
                title: mode === 'create' ? 'Создание' : 'Редактирование',
                data: item ? { ...item } : { ...defaults[entity] }
            };
        }

        function closeModal() {
            modal.value.show = false;
        }

        async function saveModal() {
            const { entity, mode, data } = modal.value;
            await runAsync(async () => {
                if (entity === 'project') {
                    const body = {
                        title: data.title,
                        description: data.description,
                        technologies: data.technologies,
                        imageUrl: data.imageUrl,
                        author: data.author
                    };
                    if (mode === 'create') await projectsApi.create(body);
                    else await projectsApi.update(data.id, body);
                } else if (entity === 'review') {
                    const body = {
                        authorName: data.authorName,
                        content: data.content,
                        rating: Number(data.rating),
                        projectId: Number(data.projectId)
                    };
                    if (mode === 'create') await reviewsApi.create(body);
                    else await reviewsApi.update(data.id, body);
                } else if (entity === 'technology') {
                    const body = { name: data.name, description: data.description, category: data.category };
                    if (mode === 'create') await technologiesApi.create(body);
                    else await technologiesApi.update(data.id, body);
                } else if (entity === 'topology') {
                    const body = {
                        name: data.name,
                        description: data.description,
                        topologyData: data.topologyData,
                        projectId: Number(data.projectId)
                    };
                    if (mode === 'create') await topologiesApi.create(body);
                    else await topologiesApi.update(data.id, body);
                }
                closeModal();
                await loadAll();
                showToast('Сохранено');
            });
        }

        async function removeItem(entity, id) {
            if (!confirm('Удалить запись?')) return;
            await runAsync(async () => {
                if (entity === 'project') await projectsApi.remove(id);
                else if (entity === 'review') await reviewsApi.remove(id);
                else if (entity === 'technology') await technologiesApi.remove(id);
                else if (entity === 'topology') await topologiesApi.remove(id);
                await loadAll();
                showToast('Удалено');
            });
        }

        function formatDate(value) {
            if (!value) return '—';
            return new Date(value).toLocaleDateString('ru-RU');
        }

        function projectTitle(id) {
            return projects.value.find(p => p.id === id)?.title || `Проект #${id}`;
        }

        onMounted(async () => {
            if (authenticated.value) {
                await runAsync(loadAll);
            }
        });

        watch(section, async (val) => {
            if (authenticated.value && val !== 'dashboard' && projects.value.length === 0) {
                await runAsync(loadAll);
            }
        });

        return {
            authenticated, auth, section, loading, toast,
            loginForm, registerForm, authMode,
            projects, reviews, technologies, topologies,
            reviewProjectFilter, topologyProjectFilter,
            modal, stats, filteredReviews, filteredTopologies,
            canWrite, canAdmin,
            login, register, logout,
            openModal, closeModal, saveModal, removeItem,
            formatDate, projectTitle
        };
    }
}).mount('#app');
