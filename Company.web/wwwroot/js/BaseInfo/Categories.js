/**
 * BaseInfo > Categories panel.
 *
 * Drives the category management UI:
 *  - "New Category" enables the name field and stores a new category.
 *  - "Edit" loads an existing category into the form and renames it.
 *  - "Delete" removes a category (the server refuses in-use categories).
 *
 * All requests carry the anti-forgery token as a header, matching the
 * [ValidateAntiForgeryToken] attribute on the server actions.
 */
document.addEventListener("DOMContentLoaded", initCategoriesPanel);

function initCategoriesPanel() {

    const urls = {
        create: "/Admin/BaseInfo/Create",
        update: "/Admin/BaseInfo/Update",
        baseInfo: "/Admin/BaseInfo/BaseInfo",
        delete: function (id) { return "/Admin/BaseInfo/Delete/" + id; }
    };

    const btnNewCategory = document.getElementById("btnNewCategory");
    const btnSaveCategory = document.getElementById("btnSaveCategory");
    const btnCancelCategory = document.getElementById("btnCancelCategory");
    const categoryName = document.getElementById("categoryName");
    const categoryId = document.getElementById("categoryId");
    const categoryError = document.getElementById("categoryError");
    const categoryMessage = document.getElementById("categoryMessage");
    const saveCategoryIcon = document.getElementById("saveCategoryIcon");
    const saveCategoryText = document.getElementById("saveCategoryText");
    const tableBody = document.getElementById("categoryTableBody");

    if (!btnNewCategory || !btnSaveCategory || !categoryName) {
        return;
    }

    let isSaving = false;

    // ============================
    // Helpers
    // ============================

    /**
     * Reads the anti-forgery token rendered by @Html.AntiForgeryToken().
     */
    function getToken() {
        const tokenInput = document.querySelector(
            "input[name='__RequestVerificationToken']"
        );

        return tokenInput ? tokenInput.value : "";
    }

    function showError(message) {
        categoryMessage.textContent = "";
        categoryError.textContent = message || "Could not save the category.";
    }

    function showMessage(message) {
        categoryError.textContent = "";
        categoryMessage.textContent = message || "";
    }

    /**
     * Toggles the form's busy state so the user cannot fire duplicate
     * requests or edit the name while a request is in flight.
     */
    function setLoading(loading) {
        isSaving = loading;

        btnSaveCategory.disabled = loading;
        btnCancelCategory.disabled = loading;
        categoryName.readOnly = loading;

        saveCategoryIcon.className = loading ? "bi bi-arrow-clockwise" : "bi bi-check-lg";
        saveCategoryText.textContent = loading ? "Saving..." : "Save";
    }

    /**
     * Checks the currently rendered table for a category with the same name.
     * Used only as a fast client-side guard on create; the server always
     * re-validates uniqueness authoritatively.
     */
    function isDuplicateName(name) {
        const rows = tableBody ? tableBody.querySelectorAll("tr") : [];

        return Array.prototype.some.call(rows, function (row) {
            const cell = row.children[1];

            return cell && cell.textContent.trim().toLowerCase() === name.toLowerCase();
        });
    }

    /**
     * POSTs a FormData payload to the given URL with the anti-forgery header
     * and parses the JSON response. Resolves with the payload; throws with a
     * user-facing message on failure.
     */
    async function post(url, body) {

        const response = await fetch(url, {
            method: "POST",
            headers: {
                "RequestVerificationToken": getToken()
            },
            body: body
        });

        let payload = null;

        try {
            payload = await response.json();
        } catch (e) {
            payload = null;
        }

        if (!response.ok) {
            throw new Error(
                (payload && payload.message) ? payload.message : "Request failed."
            );
        }

        return payload;
    }

    // ============================
    // Form state
    // ============================

    /**
     * Enables the form for either creating or editing a category.
     */
    function enableForm() {
        categoryName.disabled = false;
        btnSaveCategory.disabled = false;
        btnCancelCategory.disabled = false;

        categoryName.focus();
    }

    /**
     * Clears the form back to its idle, disabled state.
     */
    function resetCategoryForm() {
        categoryId.value = "";
        categoryName.value = "";

        categoryName.disabled = true;
        categoryName.readOnly = false;

        btnSaveCategory.disabled = true;
        btnCancelCategory.disabled = true;

        showError("");
        showMessage("");
    }

    // ============================
    // Create / Update
    // ============================

    /**
     * Saves the category. When no id is loaded it creates a new category;
     * when an id is loaded it updates (renames) that category.
     */
    async function saveCategory() {

        const id = categoryId.value;
        const name = categoryName.value.trim();

        if (name === "") {
            showError("Please enter category name.");
            categoryName.focus();
            return;
        }

        // Client-side duplicate guard only applies to creating, where no id
        // has been chosen yet. The server re-validates either way.
        if (id === "" && isDuplicateName(name)) {
            showError("This category already exists.");
            categoryName.focus();
            return;
        }

        if (isSaving) {
            return;
        }

        setLoading(true);
        showError("");
        showMessage("");

        const body = new FormData();
        body.append("Title", name);

        const isEdit = id !== "";

        if (isEdit) {
            body.append("Id", id);
        }

        try {

            const payload = isEdit
                ? await post(urls.update, body)
                : await post(urls.create, body);

            setLoading(false);
            showMessage(payload && payload.message ? payload.message : "Category saved.");

            // Reload so the table reflects the change with the same
            // server-rendered markup used for the initial page load.
            setTimeout(function () {
                window.location.assign(urls.baseInfo);
            }, 700);

        } catch (error) {
            setLoading(false);
            showError(error.message || "Network error. Please try again.");
        }
    }

    // ============================
    // Delete
    // ============================

    /**
     * Deletes the category with the given id after user confirmation.
     */
    async function deleteCategory(id) {

        if (!confirm("Are you sure you want to delete this category?")) {
            return;
        }

        showError("");
        showMessage("");

        const body = new FormData();

        try {

            const payload = await post(urls.delete(id), body);

            showMessage(payload && payload.message ? payload.message : "Category deleted.");

            setTimeout(function () {
                window.location.assign(urls.baseInfo);
            }, 700);

        } catch (error) {
            showError(error.message || "Could not delete the category.");
        }
    }

    // ============================
    // Events
    // ============================

    btnNewCategory.addEventListener("click", function () {

        categoryId.value = "";
        categoryName.value = "";

        showError("");
        showMessage("");

        enableForm();

    });

    btnSaveCategory.addEventListener("click", saveCategory);

    categoryName.addEventListener("keydown", function (event) {

        if (event.key === "Enter") {
            event.preventDefault();
            saveCategory();
        }

    });

    btnCancelCategory.addEventListener("click", resetCategoryForm);

    document
        .querySelectorAll(".btn-edit-category")
        .forEach(function (button) {

            button.addEventListener("click", function () {

                // Load the category into the form so the next save performs
                // an update instead of a create.
                categoryId.value = this.dataset.id;
                categoryName.value = this.dataset.name;

                enableForm();

            });

        });

    document
        .querySelectorAll(".btn-delete-category")
        .forEach(function (button) {

            button.addEventListener("click", function () {

                deleteCategory(this.dataset.id);

            });

        });

}
