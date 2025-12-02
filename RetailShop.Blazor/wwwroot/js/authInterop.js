// authInterop.js - small helper to access auth info from localStorage
window.authInterop = {
    // Original user methods (for staff/admin)
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
    },
    
    // Customer authentication methods
    setCustomerToken: function (token) {
        try {
            localStorage.setItem('customerToken', token);
        } catch (e) {
            console.error('[authInterop] setCustomerToken error', e);
        }
    },
    getCustomerToken: function () {
        try {
            return localStorage.getItem('customerToken');
        } catch (e) {
            console.error('[authInterop] getCustomerToken error', e);
            return null;
        }
    },
    removeCustomerToken: function () {
        try {
            localStorage.removeItem('customerToken');
        } catch (e) {
            console.error('[authInterop] removeCustomerToken error', e);
        }
    },
    setCustomerInfo: function (customerInfo) {
        try {
            localStorage.setItem('customerInfo', JSON.stringify(customerInfo));
        } catch (e) {
            console.error('[authInterop] setCustomerInfo error', e);
        }
    },
    getCustomerInfo: function () {
        try {
            const info = localStorage.getItem('customerInfo');
            return info ? JSON.parse(info) : null;
        } catch (e) {
            console.error('[authInterop] getCustomerInfo error', e);
            return null;
        }
    },
    removeCustomerInfo: function () {
        try {
            localStorage.removeItem('customerInfo');
        } catch (e) {
            console.error('[authInterop] removeCustomerInfo error', e);
        }
    },
    clearCustomerAuth: function () {
        try {
            localStorage.removeItem('customerToken');
            localStorage.removeItem('customerInfo');
        } catch (e) {
            console.error('[authInterop] clearCustomerAuth error', e);
        }
    }
};
