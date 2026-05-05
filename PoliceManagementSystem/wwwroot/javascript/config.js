/**
 * config.js
 * Global configuration and helper functions for API communication.
 * All pages include this file to get access to the token and fetch helpers.
 */

/** Base URL for all API requests. */
const API_BASE_URL = "http://localhost:5000/api";

/**
 * REQ-73: Retrieves the stored JWT token from localStorage.
 * @returns {string|null} The JWT token or null.
 */
function getToken() {
    return localStorage.getItem("token");
}

/**
 * REQ-73: Returns headers for authenticated API requests.
 * @returns {Object} Headers with Content-Type and Authorization.
 */
function authHeaders() {
    return {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${getToken()}`
    };
}

/**
 * REQ-73: Redirects to login page if user is not authenticated.
 */
function requireAuth() {
    if (!getToken()) {
        window.location.href = "login.html";
    }
}

/**
 * REQ-75: Returns the current user's role from localStorage.
 * @returns {string|null} The user role (Admin, ChiefInspector, StationHead, Agent).
 */
function getCurrentRole() {
    return localStorage.getItem("role");
}

/**
 * REQ-75: Checks if the current user has one of the allowed roles.
 * @param {string[]} roles - Array of allowed roles.
 * @returns {boolean} True if user has an allowed role.
 */
function hasRole(roles) {
    return roles.includes(getCurrentRole());
}

/**
 * REQ-73: Logs out the user and redirects to login page.
 */
function logout() {
    localStorage.removeItem("token");
    localStorage.removeItem("username");
    localStorage.removeItem("role");
    window.location.href = "login.html";
}

/**
 * REQ-73, REQ-75: Authenticated fetch wrapper.
 * Redirects to login automatically if token is expired (401).
 * @param {string} url - The API endpoint URL.
 * @param {Object} options - Fetch options (method, body, etc).
 * @returns {Promise<Response>} The fetch response.
 */
async function apiFetch(url, options = {}) {
    const response = await fetch(url, {
        ...options,
        headers: authHeaders()
    });

    if (response.status === 401) {
        logout();
        return;
    }

    return response;
}