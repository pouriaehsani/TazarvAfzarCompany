/**
 * toast.js
 *
 * A small reusable wrapper around the Bootstrap 5 Toast component that ships
 * with the NiceAdmin template. It exposes a single global helper, window.showToast,
 * used across the admin area to report success / error / info messages for
 * create, edit and delete operations.
 *
 * Usage:
 *   showToast("success", "Article created.", "Your article was saved.");
 *   showToast("error",   "Oops",       "Something went wrong.");
 *
 * Types map to Bootstrap background utilities:
 *   success -> green, error -> red, warning -> amber, info -> blue.
 *
 * It also supports "post-redirect" toasts: a script can stash a payload in
 * sessionStorage (showToastOnNextPage) and it will be shown automatically on
 * the following page load. This lets AJAX-driven pages that reload after a
 * change, and server-side RedirectToAction flows, both surface a toast.
 */
(function () {
    "use strict";

    var STORAGE_KEY = "niceadmin.pendingToast";

    // Keep one fixed toast container so toasts stack nicely bottom-right.
    var container = null;

    function ensureContainer() {
        if (container && container.isConnected) {
            return container;
        }

        container = document.createElement("div");
        container.className = "toast-container position-fixed bottom-0 end-0 p-3";
        document.body.appendChild(container);

        return container;
    }

    /**
     * Builds and shows a single toast.
     *
     * @param {string} type - one of: success | error | warning | info.
     * @param {string} title - bold leading text (may be "").
     * @param {string} message - the body text.
     * @param {number} delayMs - auto-dismiss delay (default 4000).
     */
    function showToast(type, title, message, delayMs) {
        var colorMap = {
            success: "text-bg-success",
            error: "text-bg-danger",
            warning: "text-bg-warning",
            info: "text-bg-primary"
        };

        var colorClass = colorMap[type] || colorMap.info;
        var delay = (typeof delayMs === "number" && delayMs > 0)
            ? delayMs
            : 4000;

        var toastEl = document.createElement("div");
        toastEl.className = "toast align-items-center border-0 " + colorClass;
        toastEl.setAttribute("role", "alert");
        toastEl.setAttribute("aria-live", "assertive");
        toastEl.setAttribute("aria-atomic", "true");

        var flex = document.createElement("div");
        flex.className = "d-flex";

        var body = document.createElement("div");
        body.className = "toast-body";

        if (title) {
            var strong = document.createElement("strong");
            strong.textContent = title;
            body.appendChild(strong);

            // Two spaces visually separate the title from the message.
            body.appendChild(document.createTextNode("  "));
        }

        if (message) {
            body.appendChild(document.createTextNode(message));
        }

        flex.appendChild(body);

        var close = document.createElement("button");
        close.type = "button";
        close.className = "btn-close btn-close-white me-2 m-auto";
        close.setAttribute("data-bs-dismiss", "toast");
        close.setAttribute("aria-label", "Close");

        flex.appendChild(close);
        toastEl.appendChild(flex);

        ensureContainer().appendChild(toastEl);

        if (window.bootstrap && bootstrap.Toast) {
            var toast = new bootstrap.Toast(toastEl, { delay: delay });

            // Clean the node from the DOM after it has been dismissed/hidden.
            toastEl.addEventListener("hidden.bs.toast", function () {
                if (toastEl.parentNode) {
                    toastEl.parentNode.removeChild(toastEl);
                }
            });

            toast.show();
        } else {
            // Fallback when the Bootstrap bundle is not loaded: show it plainly
            // and remove it after the delay.
            toastEl.style.display = "block";
            setTimeout(function () {
                if (toastEl.parentNode) {
                    toastEl.parentNode.removeChild(toastEl);
                }
            }, delay);
        }
    }

    /**
     * Stores a toast payload so it is shown on the next page load. Use this
     * right before reloading/redirecting so the message survives the reload.
     */
    function showToastOnNextPage(type, title, message) {
        try {
            sessionStorage.setItem(STORAGE_KEY, JSON.stringify({
                type: type,
                title: title || "",
                message: message || ""
            }));
        } catch (e) {
            // sessionStorage can be unavailable (private mode / blocked).
            // Fall back to an immediate toast in that case.
            showToast(type, title, message);
        }
    }

    /**
     * Runs on page load: shows any pending toast. Two sources are supported:
     *
     *   1. A payload stashed in sessionStorage by showToastOnNextPage on the
     *      previous page (AJAX-driven reloads).
     *   2. A server-rendered element with [data-toast] attributes, produced
     *      from TempData after a server-side RedirectToAction (PRG pattern).
     */
    function flushPendingToast() {
        var raw = null;

        try {
            raw = sessionStorage.getItem(STORAGE_KEY);
        } catch (e) {
            raw = null;
        }

        if (raw) {
            sessionStorage.removeItem(STORAGE_KEY);

            try {
                var payload = JSON.parse(raw);
                showToast(payload.type, payload.title, payload.message);
                return;
            } catch (e) {
                // Ignore malformed payloads and continue to the DOM source.
            }
        }

        var serverEl = document.querySelector("[data-toast]");
        if (serverEl) {
            showToast(
                serverEl.getAttribute("data-toast-type") || "info",
                serverEl.getAttribute("data-toast-title") || "",
                serverEl.getAttribute("data-toast-message") || ""
            );
        }
    }

    // Expose the public API and flush any pending toast once the DOM is ready.
    window.showToast = showToast;
    window.showToastOnNextPage = showToastOnNextPage;

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", flushPendingToast);
    } else {
        flushPendingToast();
    }
})();
