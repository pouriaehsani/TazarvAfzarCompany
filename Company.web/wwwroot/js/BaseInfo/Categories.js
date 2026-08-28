/**
 * BaseInfo > Categories panel.
 *
 * "New Category" enables the name field, "Save" posts it to
 * /Admin/BaseInfo/Create and stores it in the database.
 */
document.addEventListener("DOMContentLoaded", initCategoriesPanel);

function initCategoriesPanel() {

    const urls = {
        create: "/Admin/BaseInfo/Create",
        baseInfo: "/Admin/BaseInfo/BaseInfo"
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

    function getToken() {
        const tokenInput = document.querySelector(
            "input[name='__RequestVerificationToken']"
        );

        return tokenInput ? tokenInput.value : "";
    }

    function showError(message) {
        categoryMessage.textContent = "";
        categoryError.textContent = message || "خطا در ذخیره‌ی دسته‌بندی.";
    }

    function showMessage(message) {
        categoryError.textContent = "";
        categoryMessage.textContent = message || "";
    }

    function setLoading(loading) {
        isSaving = loading;

        btnSaveCategory.disabled = loading;
        btnCancelCategory.disabled = loading;
        categoryName.readOnly = loading;

        saveCategoryIcon.className = loading ? "bi bi-arrow-clockwise" : "bi bi-check-lg";
        saveCategoryText.textContent = loading ? "Saving..." : "Save";
    }

    function isDuplicateName(name) {
        const rows = tableBody ? tableBody.querySelectorAll("tr") : [];

        return Array.prototype.some.call(rows, function (row) {
            const cell = row.children[1];

            return cell && cell.textContent.trim().toLowerCase() === name.toLowerCase();
        });
    }

    // ============================
    // Form state
    // ============================

    function enableForm() {
        categoryName.disabled = false;
        btnSaveCategory.disabled = false;
        btnCancelCategory.disabled = false;

        categoryName.focus();
    }

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
    // Create
    // ============================

    async function saveCategory() {

        const id = categoryId.value;
        const name = categoryName.value.trim();

        if (name === "") {
            showError("Please enter category name.");
            categoryName.focus();
            return;
        }

        if (id === "" && isDuplicateName(name)) {
            showError("این دسته‌بندی از قبل وجود دارد.");
            categoryName.focus();
            return;
        }

        if (isSaving) {
            return;
        }

        // Edit is not implemented yet - keep the current behaviour.
        if (id !== "") {
            console.log("Edit Category:", id, name);
            return;
        }

        setLoading(true);
        showError("");
        showMessage("");

        const body = new FormData();
        body.append("Title", name);

        try {

            const response = await fetch(urls.create, {
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
                setLoading(false);
                showError(payload && payload.message ? payload.message : "خطا در ذخیره‌ی دسته‌بندی.");
                categoryName.focus();
                return;
            }

            showMessage(payload && payload.message ? payload.message : "ذخیره شد.");

            // Reload so the new row (and its Edit/Delete buttons) is rendered
            // by the same server-side markup as the rest of the table.
            setTimeout(function () {
                window.location.assign(urls.baseInfo);
            }, 700);

        } catch (error) {
            setLoading(false);
            showError("خطای شبکه. لطفاً دوباره تلاش کنید.");
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

                categoryId.value = this.dataset.id;
                categoryName.value = this.dataset.name;

                enableForm();

            });

        });

    document
        .querySelectorAll(".btn-delete-category")
        .forEach(function (button) {

            button.addEventListener("click", function () {

                const id = this.dataset.id;

                if (!confirm("Are you sure you want to delete this category?")) {
                    return;
                }

                // Delete is not implemented yet.
                console.log("Delete Category:", id);

            });

        });

}
