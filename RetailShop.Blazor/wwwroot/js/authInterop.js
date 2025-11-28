// authInterop.js - small helper to access auth info from localStorage
window.authInterop = {
    getCurrentUser: function () {
        try {
            return localStorage.getItem('currentUser');
        } catch (e) {
            console.error('[authInterop] getCurrentUser error', e);
            return null;
        }
    },
    getToken: function () {
        try {
            return localStorage.getItem('authToken');
        } catch (e) {
            console.error('[authInterop] getToken error', e);
            return null;
        }
    }
};
