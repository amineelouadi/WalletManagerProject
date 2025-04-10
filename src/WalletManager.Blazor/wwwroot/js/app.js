// App-specific JavaScript functions

window.showToast = (message, type) => {
    // This function can be used when we add toast notifications
    console.log(`${type}: ${message}`);
};

window.scrollToTop = () => {
    window.scrollTo(0, 0);
};

window.setPageTitle = (title) => {
    document.title = title;
};

// Format currency values
window.formatCurrency = (value, currency = 'USD') => {
    return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: currency
    }).format(value);
};

// Format date values
window.formatDate = (date, format = 'short') => {
    const d = new Date(date);
    
    if (format === 'short') {
        return d.toLocaleDateString();
    } else if (format === 'long') {
        return d.toLocaleDateString(undefined, { 
            weekday: 'long', 
            year: 'numeric', 
            month: 'long', 
            day: 'numeric' 
        });
    } else {
        return d.toLocaleDateString();
    }
};

// Handle Bootstrap Modal
window.showModal = (id) => {
    const modalElement = document.getElementById(id);
    if (modalElement) {
        const modal = new bootstrap.Modal(modalElement);
        modal.show();
    }
};

window.hideModal = (id) => {
    const modalElement = document.getElementById(id);
    if (modalElement) {
        const modal = bootstrap.Modal.getInstance(modalElement);
        if (modal) {
            modal.hide();
        }
    }
};
