/**
 * BaseInfo > Categories panel.
 *
 * Handles every category action on this page:
 *   - "New Category"  -> clears the form and lets the user create a category.
 *   - "Edit"          -> loads a category into the form for renaming.
 *   - "Save"          -> creates (no id) or updates (with id) the category.
 *   - "Delete"        -> deletes a category after confirmation.
 *
 * The server owns all business rules (validation, uniqueness, referential
 * integrity). This script only talks to the JSON endpoints and reloads the
 * list from the server after a successful change.
 *
 * Note on the anti-forgery token: it is sent BOTH as a form field
 * (__RequestVerificationToken) and as the conventional RequestVerificationToken
 * header, so [ValidateAntiForgeryToken] always accepts the request.
 */
(function () {
    "use strict";

    document.addEventListener("DOMContentLoaded", initCategoriesPanel);

    function initCategoriesPanel() {

        var endpoints = {
            create: "/Admin/BaseInfo/Create",
            update: "/Admin/BaseInfo/Update",
            baseInfo: "/Admin/BaseInfo/BaseInfo",
            deleteById: function (id) {
                return "/Admin/BaseInfo/Delete/" + id;
            }
        };

        var els = {
            newCategory: document.getElementById("btnNewCategory"),
            saveCategory: document.getElementById("btnSaveCategory"),
            cancelCategory: document.getElementById("btnCancelCategory"),
            name: document.getElementById("categoryName"),
            id: document.getElementById("categoryId"),
            error: document.getElementById("categoryError"),
            message: document.getElementById("categoryMessage"),
            saveIcon: document.getElementById("saveCategoryIcon"),
            saveText: document.getElementById("saveCategoryText"),
            tableBody: document.getElementById("categoryTableBody")
        };

        // If any required element is missing, attach nothing so we fail quietly
        // instead of throwing errors that break the whole page.
        if (!els.newCategory || !els.saveCategory || !els.name) {
            return;
        }

        var isBusy = false;

        /* ------------------------------------------------------------------
         * Small UI helpers
         * ------------------------------------------------------------------ */

        function readToken() {
            var tokenInput = document.querySelector(
                "input[name='__RequestVerificationToken']"
            );

            return tokenInput ? tokenInput.value : "";
        }

        function showError(message) {
            els.message.textContent = "";
            els.error.textContent = message || "Could not save the category.";
        }

        function showMessage(message) {
            els.error.textContent = "";
            els.message.textContent = message || "";
        }

        function setBusy(busy) {
            isBusy = busy;

            els.saveCategory.disabled = busy;
            els.cancelCategory.disabled = busy;
            els.name.readOnly = busy;

            els.saveIcon.className = busy
                ? "bi bi-arrow-clockwise"
                : "bi bi-check-lg";

            els.saveText.textContent = busy ? "Saving..." : "Save";
        }

        function enableForm() {
            els.name.disabled = false;
            els.saveCategory.disabled = false;
            els.cancelCategory.disabled = false;
            els.name.focus();
        }

        function resetForm() {
            els.id.value = "";
            els.name.value = "";
            els.name.disabled = true;
            els.name.readOnly = false;
            els.saveCategory.disabled = true;
            els.cancelCategory.disabled = true;
            showError("");
            showMessage("");
        }

        /* ------------------------------------------------------------------
         * Server communication
         * ------------------------------------------------------------------ */

        /**
         * POSTs a FormData payload to the given URL and returns the parsed JSON
         * on success. Throws an Error with a user-friendly message otherwise.
         */
        function post(url, body) {

            // Extra fields are ignored by [FromForm] model binders, so adding
            // the token as a field here is harmless and makes the request work
            // even when only the form-field style is enabled on the server.
            body.append("__RequestVerificationToken", readToken());

            return fetch(url, {
                method: "POST",
                headers: {
                    "RequestVerificationToken": readToken()
                },
                body: body
            }).then(function (response) {

                return response.json().then(function (payload) {
                    if (!response.ok) {
                        var message =
                            (payload && payload.message)
                                ? payload.message
                                : "Request failed.";

                        throw new Error(message);
                    }

                    return payload;
                });

            });
        }

        /**
         * After a successful save/delete we reload the page so the table is
         * re-rendered from the server with the same markup as first load.
         */
        function reloadList() {
            setTimeout(function () {
                window.location.assign(endpoints.baseInfo);
            }, 600);
        }

        /* ------------------------------------------------------------------
         * Actions
         * ------------------------------------------------------------------ */

        function saveCategory() {

            var id = els.id.value;
            var name = els.name.value.trim();

            if (name === "") {
                showError("Please enter category name.");
                els.name.focus();
                return;
            }

            if (isBusy) {
                return;
            }

            setBusy(true);
            showError("");
            showMessage("");

            var body = new FormData();
            body.append("Title", name);

            var isEdit = id !== "";

            if (isEdit) {
                body.append("Id", id);
            }

            post(isEdit ? endpoints.update : endpoints.create, body)
                .then(function (payload) {
                    setBusy(false);
                    showMessage(
                        (payload && payload.message) ? payload.message : "Category saved."
                    );
                    reloadList();
                })
                .catch(function (error) {
                    setBusy(false);
                    showError(error.message || "Network error. Please try again.");
                });
        }

        function startEdit(button) {
            els.id.value = button.dataset.id;
            els.name.value = button.dataset.name;
            enableForm();
        }

        function deleteCategory(button) {
            var id = button.dataset.id;

            if (!confirm("Are you sure you want to delete this category?")) {
                return;
            }

            showError("");
            showMessage("");

            post(endpoints.deleteById(id), new FormData())
                .then(function (payload) {
                    showMessage(
                        (payload && payload.message) ? payload.message : "Category deleted."
                    );
                    reloadList();
                })
                .catch(function (error) {
                    showError(error.message || "Could not delete the category.");
                });
        }

        /* ------------------------------------------------------------------
         * Wire up events
         * ------------------------------------------------------------------ */

        els.newCategory.addEventListener("click", function () {
            els.id.value = "";
            els.name.value = "";
            showError("");
            showMessage("");
            enableForm();
        });

        els.saveCategory.addEventListener("click", saveCategory);

        els.name.addEventListener("keydown", function (event) {
            if (event.key === "Enter") {
                event.preventDefault();
                saveCategory();
            }
        });

        els.cancelCategory.addEventListener("click", resetForm);

        // Event delegation on the table body: works even if the rows are
        // re-rendered after a save, and requires only one listener per action.
        if (els.tableBody) {
            els.tableBody.addEventListener("click", function (event) {
                var button = event.target.closest
                    ? event.target.closest("button")
                    : null;

                if (!button) {
                    return;
                }

                if (button.classList.contains("btn-edit-category")) {
                    startEdit(button);
                } else if (button.classList.contains("btn-delete-category")) {
                    deleteCategory(button);
                }
            });
        }
    }
})();
