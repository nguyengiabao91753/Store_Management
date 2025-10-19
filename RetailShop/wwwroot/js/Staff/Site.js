// Common JavaScript functions for Staff area

// Global utility functions
const StaffUtils = {
    // Format currency
    formatCurrency: function (amount) {
        return '$' + parseFloat(amount).toFixed(2);
    },

    // Format date
    formatDate: function (date) {
        const options = { weekday: 'short', day: '2-digit', month: 'short', year: 'numeric' };
        return date.toLocaleDateString('en-US', options);
    },

    // Format time
    formatTime: function (date) {
        return date.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit' });
    },

    // Show notification
    showNotification: function (message, type = 'info') {
        // You can implement toast notification here
        console.log(`[${type.toUpperCase()}] ${message}`);
        alert(message);
    },

    // Confirm action
    confirm: function (message) {
        return confirm(message);
    }
};

// Export for use in other scripts
window.StaffUtils = StaffUtils;